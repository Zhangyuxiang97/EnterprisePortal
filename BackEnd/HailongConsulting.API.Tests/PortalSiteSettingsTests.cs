using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using HailongConsulting.API.Common;
using HailongConsulting.API.Controllers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HailongConsulting.API.Tests;

public class PortalSiteSettingsTests : IAsyncLifetime
{
    private const string Endpoint = "/api/config/site-settings";
    private readonly SymmetricSecurityKey _key = new(RandomNumberGenerator.GetBytes(32));
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        var databaseName = Guid.NewGuid().ToString();
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        builder.Services.AddControllers().AddApplicationPart(typeof(PortalSiteSettingsController).Assembly);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = _key
            };
        });
        builder.Services.AddAuthorization();
        _app = builder.Build();
        _app.UseAuthentication();
        _app.UseAuthorization();
        _app.MapControllers();
        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    private void SignIn(string role)
    {
        var token = new JwtSecurityToken(claims: [new Claim(ClaimTypes.Role, role)],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
    }

    private async Task<PortalSiteSettingsDto> ReadSettings()
    {
        var response = await _client.GetAsync(Endpoint);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PortalSiteSettingsDto>>();
        Assert.True(body!.Success);
        return body.Data!;
    }

    [Fact]
    public async Task ReadinessEndpointWorksBeforeFirstSettingsSave()
    {
        var response = await _client.GetAsync("/health/ready");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AnonymousReadReturnsDefaultsWithoutCreatingDatabaseRow()
    {
        var settings = await ReadSettings();
        Assert.False(string.IsNullOrWhiteSpace(settings.Company.FullName));
        Assert.False(string.IsNullOrWhiteSpace(settings.Contact.Address.FullAddress));
        Assert.NotEmpty(settings.Faqs);
        using var scope = _app.Services.CreateScope();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().PortalSiteSettings.ToListAsync());
    }

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("user", HttpStatusCode.Forbidden)]
    public async Task SavingRequiresAdministrator(string? role, HttpStatusCode expected)
    {
        var settings = await ReadSettings();
        if (role is not null) SignIn(role);
        var response = await _client.PutAsJsonAsync(Endpoint, settings);
        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task AdministratorSaveIsVisibleToAnonymousReaderAndUpdatesSingleton()
    {
        var settings = await ReadSettings();
        settings.Company.FullName = "测试公司";
        settings.Contact.Phone = "010-12345678";
        settings.Contact.WorkingHours.Weekdays = "全天开放";
        settings.Faqs = [new PortalFaqDto { Question = "如何联系？", Answer = "请致电新的联系电话。" }];
        SignIn("admin");
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
        settings = await ReadSettings();
        settings.Contact.Phone = "010-87654321";
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
        _client.DefaultRequestHeaders.Authorization = null;
        var saved = await ReadSettings();
        Assert.Equal("测试公司", saved.Company.FullName);
        Assert.Equal("010-87654321", saved.Contact.Phone);
        Assert.Equal("全天开放", saved.Contact.WorkingHours.Weekdays);
        Assert.Single(saved.Faqs);
        using var scope = _app.Services.CreateScope();
        Assert.Single(await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().PortalSiteSettings.ToListAsync());
    }

    [Fact]
    public async Task StaleVersionAndMissingVersionCannotOverwriteAnotherAdministratorsSave()
    {
        var first = await ReadSettings();
        var second = await ReadSettings();
        Assert.Equal("defaults", first.Version);
        SignIn("admin");
        first.Company.FullName = "先保存的公司";
        var savedResponse = await _client.PutAsJsonAsync(Endpoint, first);
        Assert.Equal(HttpStatusCode.OK, savedResponse.StatusCode);
        var saved = (await savedResponse.Content.ReadFromJsonAsync<ApiResponse<PortalSiteSettingsDto>>())!.Data!;
        Assert.NotEqual("defaults", saved.Version);
        Assert.NotNull(saved.UpdatedAt);
        second.Company.FullName = "过期内容";
        Assert.Equal(HttpStatusCode.Conflict, (await _client.PutAsJsonAsync(Endpoint, second)).StatusCode);
        second.Version = null;
        Assert.Equal(HttpStatusCode.Conflict, (await _client.PutAsJsonAsync(Endpoint, second)).StatusCode);
        Assert.Equal("先保存的公司", (await ReadSettings()).Company.FullName);
        saved.Company.FullName = "合并后保存";
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync(Endpoint, saved)).StatusCode);
    }

    [Fact]
    public async Task DatabaseVersionRejectsUpdatesFromAnEarlierRead()
    {
        SignIn("admin");
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync(Endpoint, await ReadSettings())).StatusCode);
        using var firstScope = _app.Services.CreateScope();
        using var secondScope = _app.Services.CreateScope();
        var firstDb = firstScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var secondDb = secondScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var first = await firstDb.PortalSiteSettings.SingleAsync();
        var second = await secondDb.PortalSiteSettings.SingleAsync();
        first.Version = Guid.NewGuid().ToString("N");
        await firstDb.SaveChangesAsync();
        second.Version = Guid.NewGuid().ToString("N");
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondDb.SaveChangesAsync());
    }

    [Fact]
    public async Task InvalidCoordinatesAndMissingStructureCannotOverwriteSettings()
    {
        var settings = await ReadSettings();
        SignIn("admin");
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
        settings.Contact.Map.Latitude = 91;
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
        settings.Contact = null!;
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
        Assert.InRange((await ReadSettings()).Contact.Map.Latitude, -90, 90);
    }

    [Fact]
    public async Task IncompleteJsonAndInvalidEmailAreRejected()
    {
        var settings = await ReadSettings();
        SignIn("admin");
        var missingGroups = JsonContent.Create(new { settings.Company, settings.Contact });
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsync(Endpoint, missingGroups)).StatusCode);
        settings.Contact.Email = "invalid-email";
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
    }

    [Fact]
    public async Task EmptyFaqAndOversizedArraysAreRejected()
    {
        var settings = await ReadSettings();
        SignIn("admin");
        settings.Faqs = [new PortalFaqDto { Question = " ", Answer = "回答" }];
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
        settings.Faqs = [];
        settings.Transportation.Bus.Routes = Enumerable.Repeat("公交线路", 51).ToList();
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsJsonAsync(Endpoint, settings)).StatusCode);
    }
}

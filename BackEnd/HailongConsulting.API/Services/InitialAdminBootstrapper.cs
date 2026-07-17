using HailongConsulting.API.Common.Helpers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

/// <summary>
/// Creates a random first administrator when the user table is empty.
/// Credentials are written outside the web root and never included in application logs.
/// </summary>
public static class InitialAdminBootstrapper
{
    public static async Task BootstrapInitialAdminAsync(this IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        const string username = "admin";
        const string email = "admin@localhost";
        var password = PasswordHelper.GenerateRandomPassword(12);
        var credentialPath = await WriteCredentialsFileAsync(username, password);

        dbContext.Users.Add(new User
        {
            Username = username.Trim(),
            Password = PasswordHelper.HashPassword(password),
            Email = email.Trim(),
            RealName = "系统管理员",
            Role = "admin",
            Status = 1
        });

        await dbContext.SaveChangesAsync();
        logger.LogWarning("Initial administrator {Username} was generated. Credentials were written to {CredentialPath}; delete the file after first login.", username, credentialPath);
        Console.WriteLine();
        Console.WriteLine("========== 初始管理员账号（请立即保存） ==========");
        Console.WriteLine($"用户名: {username}");
        Console.WriteLine($"密码: {password}");
        Console.WriteLine($"凭据文件: {credentialPath}");
        Console.WriteLine("首次登录后请删除该凭据文件，并修改密码。");
        Console.WriteLine("================================================");
    }

    private static async Task<string> WriteCredentialsFileAsync(string username, string password)
    {
        var credentialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs", "bootstrap");
        Directory.CreateDirectory(credentialDirectory);

        var credentialPath = Path.Combine(credentialDirectory, "initial-admin-credentials.txt");
        var contents = $"Generated at (UTC): {DateTime.UtcNow:O}{Environment.NewLine}" +
                       $"Username: {username}{Environment.NewLine}" +
                       $"Password: {password}{Environment.NewLine}" +
                       $"Delete this file after the first successful login.{Environment.NewLine}";

        await File.WriteAllTextAsync(credentialPath, contents);
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(credentialPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }

        return credentialPath;
    }
}

using System.Text;
using HailongConsulting.API.Common.Helpers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Middleware;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.RateLimiting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置 WebRootPath（从配置文件读取或使用默认值）
var rootPath = builder.Configuration["FileUpload:RootPath"] ?? "wwwroot";
if (string.IsNullOrEmpty(builder.Environment.WebRootPath))
{
    builder.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), rootPath);
}

// 确保 wwwroot 目录存在
if (!Directory.Exists(builder.Environment.WebRootPath))
{
    Directory.CreateDirectory(builder.Environment.WebRootPath);
}

// 配置Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10 * 1024 * 1024,
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // 配置日期时间格式：yyyy-MM-dd HH:mm:ss
        options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new CustomNullableDateTimeConverter());
    });

// 配置数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("缺少 ConnectionStrings:DefaultConnection。请使用部署脚本生成的运行时密钥文件或显式配置连接字符串。");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
    {
        // 启用连接弹性（自动重试）
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        
        // 设置命令超时时间
        mySqlOptions.CommandTimeout(30);
    });
    
    // 在开发环境启用详细错误信息
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// 配置JWT认证
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException("缺少或不安全的 Jwt:Key。请使用部署脚本生成至少 32 字节的随机密钥。");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userIdValue = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var tokenVersionValue = context.Principal?.FindFirst("token_version")?.Value;
            if (!int.TryParse(userIdValue, out var userId) || !int.TryParse(tokenVersionValue, out var tokenVersion))
            {
                context.Fail("访问令牌缺少有效的用户状态声明。");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null || user.Status == 0 || user.IsDeleted != 0 || user.TokenVersion != tokenVersion)
            {
                context.Fail("访问令牌已失效。");
            }
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    var trustedProxy = builder.Configuration["ReverseProxy:TrustedProxy"];
    if (!string.IsNullOrWhiteSpace(trustedProxy))
    {
        if (!IPAddress.TryParse(trustedProxy, out var trustedProxyAddress))
        {
            throw new InvalidOperationException("ReverseProxy:TrustedProxy 必须是有效的 IP 地址。");
        }

        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
        options.KnownProxies.Add(trustedProxyAddress);
    }
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: $"{context.Connection.RemoteIpAddress}:{context.Request.Path}",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置AutoMapper
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

// 注册服务
builder.Services.AddSingleton<IHashIdService, HashIdService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHtmlContentSanitizer, HtmlContentSanitizer>();
builder.Services.AddScoped<FileHelper>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IGlobalSearchRepository, GlobalSearchRepository>();
builder.Services.AddScoped<IGlobalSearchService, GlobalSearchService>();

// 新增服务 - 统一附件、公告、信息发布管理
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IInfoPublicationRepository, InfoPublicationRepository>();
builder.Services.AddScoped<IInfoPublicationService, InfoPublicationService>();

// 访问统计服务
builder.Services.AddScoped<IVisitStatisticRepository, VisitStatisticRepository>();
builder.Services.AddScoped<IVisitStatisticService, VisitStatisticService>();

// 区域字典服务
builder.Services.AddScoped<IRegionDictionaryRepository, RegionDictionaryRepository>();
builder.Services.AddScoped<IRegionDictionaryService, RegionDictionaryService>();

// 用户管理服务
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// 系统日志服务
builder.Services.AddScoped<ISystemLogRepository, SystemLogRepository>();
builder.Services.AddScoped<ISystemLogService, SystemLogService>();

builder.Services.AddScoped<JwtHelper>();

// 配置Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "海隆咨询 API",
        Version = "v1",
        Description = "海隆咨询官网后端API文档",
        Contact = new OpenApiContact
        {
            Name = "海隆咨询",
            Email = "contact@hailong.com"
        }
    });

    // 配置JWT认证
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // 包含XML注释
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

await app.Services.ApplyDatabaseMigrationsAsync();
await app.Services.BootstrapInitialAdminAsync(app.Configuration);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "海隆咨询 API v1");
        c.RoutePrefix = string.Empty; // 设置Swagger UI为根路径
    });
}

// 使用全局异常处理中间件
app.UseExceptionHandling();

// 使用Serilog请求日志
app.UseSerilogRequestLogging();

app.UseForwardedHeaders();

if (app.Configuration.GetValue<bool>("ReverseProxy:HttpsRedirectEnabled"))
{
    app.UseHttpsRedirection();
}

// 启用静态文件服务（用于访问上传的附件）
app.UseStaticFiles();

// 使用CORS
app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// 使用系统日志中间件（在认证授权之后，这样可以获取用户信息）
app.UseMiddleware<SystemLogMiddleware>();

app.MapControllers();

// 启动日志
Log.Information("海隆咨询 API 启动成功");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序启动失败");
}
finally
{
    Log.CloseAndFlush();
}

# 海隆咨询官网后端API

基于 .NET 8 Web API 构建的海隆咨询官网后端服务系统。

## 📋 项目概述

**项目名称**: 海隆咨询官网后端API (HailongConsulting.API)

**技术架构**: RESTful API + 分层架构

**开发状态**: ✅ 已完成并投入使用

## 🛠 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 | Web API框架 |
| Entity Framework Core | 8.0.29 | ORM框架 |
| MySQL | 8.0+ | 关系型数据库 |
| Pomelo.EntityFrameworkCore.MySql | 8.0.3 | MySQL数据库提供程序 |
| JWT Bearer | 8.0.29 | 身份认证 |
| AutoMapper | 15.1.3 | 对象映射 |
| Serilog.AspNetCore | 8.0.3 | 日志记录 |
| Swagger/OpenAPI | 6.5.0 | API文档 |

## 📁 项目结构

```
HailongConsulting.API/
├── Controllers/              # API控制器层
│   ├── AnnouncementController.cs       # 公告管理
│   ├── AttachmentController.cs         # 附件管理
│   ├── AuthController.cs               # 认证授权
│   ├── ConfigController.cs             # 系统配置
│   ├── HomeController.cs               # 首页数据
│   ├── InfoPublicationController.cs    # 信息发布
│   ├── RegionDictionaryController.cs   # 区域字典
│   ├── SearchController.cs             # 全局搜索
│   ├── StatisticsController.cs         # 统计分析
│   ├── SystemLogController.cs          # 系统日志
│   └── UserController.cs               # 用户管理
├── Services/                 # 业务逻辑层
│   ├── IAuthService.cs / AuthService.cs
│   ├── IAnnouncementService.cs / AnnouncementService.cs
│   ├── IAttachmentService.cs / AttachmentService.cs
│   ├── IConfigService.cs / ConfigService.cs
│   ├── IHomeService.cs / HomeService.cs
│   ├── IInfoPublicationService.cs / InfoPublicationService.cs
│   ├── IGlobalSearchService.cs / GlobalSearchService.cs
│   ├── IRegionDictionaryService.cs / RegionDictionaryService.cs
│   ├── IUserService.cs / UserService.cs
│   ├── ISystemLogService.cs / SystemLogService.cs
│   ├── IVisitStatisticService.cs / VisitStatisticService.cs
│   └── FileUploadService.cs
├── Repositories/             # 数据访问层
│   ├── IRepository.cs / Repository.cs
│   ├── IUnitOfWork.cs / UnitOfWork.cs
│   ├── IAnnouncementRepository.cs / AnnouncementRepository.cs
│   ├── IAttachmentRepository.cs / AttachmentRepository.cs
│   ├── IConfigRepository.cs / ConfigRepository.cs
│   ├── IGlobalSearchRepository.cs / GlobalSearchRepository.cs
│   ├── IInfoPublicationRepository.cs / InfoPublicationRepository.cs
│   ├── IRegionDictionaryRepository.cs / RegionDictionaryRepository.cs
│   ├── IUserRepository.cs / UserRepository.cs
│   ├── ISystemLogRepository.cs / SystemLogRepository.cs
│   └── IVisitStatisticRepository.cs / VisitStatisticRepository.cs
├── Models/
│   ├── Entities/            # 数据库实体
│   │   ├── User.cs
│   │   ├── Announcement.cs
│   │   ├── Attachment.cs
│   │   ├── InfoPublication.cs
│   │   ├── RegionDictionary.cs
│   │   ├── SystemLog.cs
│   │   ├── VisitStatistic.cs
│   │   ├── CarouselBanner.cs
│   │   ├── CompanyProfile.cs
│   │   ├── BusinessScope.cs
│   │   ├── CompanyQualification.cs
│   │   ├── CompanyHonor.cs
│   │   ├── MajorAchievement.cs
│   │   └── FriendlyLink.cs
│   └── DTOs/                # 数据传输对象
│       ├── LoginDto.cs
│       ├── UserDto.cs
│       ├── AnnouncementDto.cs
│       ├── AttachmentDto.cs
│       ├── InfoPublicationDto.cs
│       ├── ConfigDto.cs
│       ├── GlobalSearchDto.cs
│       ├── StatisticsDto.cs
│       ├── SystemLogDto.cs
│       └── RegionDictionaryDto.cs
├── Data/
│   └── ApplicationDbContext.cs    # EF Core数据库上下文
├── Common/
│   ├── ApiResponse.cs             # 统一响应格式
│   ├── PagedResult.cs             # 分页结果
│   ├── MappingProfile.cs          # AutoMapper配置
│   └── Helpers/
│       ├── JwtHelper.cs           # JWT工具类
│       ├── PasswordHelper.cs      # 密码加密工具
│       ├── FileHelper.cs          # 文件处理工具
│       └── CustomDateTimeConverter.cs  # 日期时间转换器
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs  # 全局异常处理
│   └── SystemLogMiddleware.cs          # 系统日志记录
├── wwwroot/
│   └── uploads/                   # 文件上传目录
│       └── attachments/
│           ├── image/             # 图片附件
│           ├── document/          # 文档附件
│           └── other/             # 其他附件
├── logs/                          # 日志文件目录
├── Program.cs                     # 应用程序入口
├── appsettings.json              # 配置文件
└── appsettings.Development.json  # 开发环境配置
```

## 🚀 快速开始

### 1. 环境要求

- **.NET 8 SDK** - [下载地址](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL 8.0+** - 数据库服务器
- **Visual Studio 2022** 或 **VS Code** - 开发工具

### 2. 克隆项目

```bash
git clone <repository-url>
cd BackEnd/HailongConsulting.API
```

### 3. 配置数据库

修改 `appsettings.json` 中的数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=hailong_consulting;User=root;Password=your_password;CharSet=utf8mb4;"
  }
}
```

### 4. 创建数据库

执行项目根目录下的SQL脚本：

```bash
# 进入SQL目录
cd ../../SQL

# 创建数据库结构
mysql -u root -p < 01_hailong_consulting_schema.sql

# 导入初始数据
mysql -u root -p < 02_hailong_consulting_init_data.sql
```

### 5. 安装依赖

```bash
cd ../BackEnd/HailongConsulting.API
dotnet restore
```

### 6. 运行项目

```bash
dotnet run
```

或使用 Visual Studio 直接运行（F5）。

### 7. 访问API文档

项目启动后，访问：

- **Swagger UI**: http://localhost:5000 或 https://localhost:5001
- **API Base URL**: http://localhost:5000/api

首次启动且用户表为空时，会生成 `admin` 账号。随机密码仅写入运行目录的
`logs/bootstrap/initial-admin-credentials.txt`；完成首次登录后请修改密码并删除该文件。

门户每次页面导航会新增一条访问记录。旧版按日期和页面汇总的历史记录无法还原
真实的独立访客和访问来源；这些指标从本次改动后的新增记录开始按访客 IP 统计。

## 📡 核心API端点

### 认证模块 (`/api/auth`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| POST | `/api/auth/login` | 用户登录 | ❌ |
| POST | `/api/auth/change-password` | 修改密码 | ✅ |
| GET | `/api/auth/me` | 获取当前用户信息 | ✅ |

### 公告管理 (`/api/announcements`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/announcements` | 获取公告列表（分页） | ❌ |
| GET | `/api/announcements/{id}` | 获取公告详情 | ❌ |
| POST | `/api/announcements` | 创建公告 | ✅ |
| PUT | `/api/announcements/{id}` | 更新公告 | ✅ |
| DELETE | `/api/announcements/{id}` | 删除公告 | ✅ |

**查询参数**:
- `businessType`: 业务类型（GOV_PROCUREMENT/CONSTRUCTION）
- `noticeType`: 公告类型
- `province`: 省份
- `city`: 城市
- `keyword`: 关键词搜索
- `pageIndex`: 页码（默认1）
- `pageSize`: 每页大小（默认10）

### 信息发布 (`/api/info-publications`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/info-publications` | 获取信息列表 | ❌ |
| GET | `/api/info-publications/{id}` | 获取信息详情 | ❌ |
| POST | `/api/info-publications` | 创建信息 | ✅ |
| PUT | `/api/info-publications/{id}` | 更新信息 | ✅ |
| DELETE | `/api/info-publications/{id}` | 删除信息 | ✅ |

**查询参数**:
- `type`: 信息类型（COMPANY_NEWS/POLICY_REGULATION）
- `category`: 二级分类
- `keyword`: 关键词搜索

### 附件管理 (`/api/attachments`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| POST | `/api/attachments/upload` | 上传附件 | ✅ |
| GET | `/api/attachments/{id}` | 获取附件信息 | ❌ |
| DELETE | `/api/attachments/{id}` | 删除附件 | ✅ |
| GET | `/api/attachments/by-related` | 获取关联附件列表 | ❌ |

**支持的文件类型**:
- 图片: jpg, jpeg, png, gif, bmp, webp
- 文档: pdf, doc, docx, xls, xlsx, ppt, pptx, txt
- 其他: zip, rar, 7z

### 系统配置 (`/api/config`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/config/banners` | 获取轮播图列表 | ❌ |
| GET | `/api/config/company-profile` | 获取企业简介 | ❌ |
| GET | `/api/config/business-scope` | 获取业务范围 | ❌ |
| GET | `/api/config/qualifications` | 获取企业资质 | ❌ |
| GET | `/api/config/honors` | 获取企业荣誉 | ❌ |
| GET | `/api/config/achievements` | 获取重要业绩 | ❌ |
| GET | `/api/config/friendly-links` | 获取友情链接 | ❌ |
| POST/PUT/DELETE | `/api/config/*` | 配置管理操作 | ✅ |

### 全局搜索 (`/api/search`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/search` | 全局搜索 | ❌ |

**查询参数**:
- `keyword`: 搜索关键词
- `businessType`: 业务类型
- `startDate`: 开始日期
- `endDate`: 结束日期
- `province`: 省份
- `city`: 城市

### 首页数据 (`/api/home`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/home/statistics` | 获取首页统计数据 | ❌ |
| GET | `/api/home/latest-announcements` | 获取最新公告 | ❌ |

### 统计分析 (`/api/statistics`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/statistics/dashboard` | 获取仪表盘数据 | ✅ |
| GET | `/api/statistics/visits` | 获取访问统计 | ✅ |
| GET | `/api/statistics/announcements` | 获取公告统计 | ✅ |

### 区域字典 (`/api/regions`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/regions` | 获取区域列表 | ❌ |
| GET | `/api/regions/provinces` | 获取省份列表 | ❌ |
| GET | `/api/regions/cities/{provinceCode}` | 获取城市列表 | ❌ |
| GET | `/api/regions/districts/{cityCode}` | 获取区县列表 | ❌ |

### 系统日志 (`/api/system-logs`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/system-logs` | 获取系统日志列表 | ✅ |
| GET | `/api/system-logs/{id}` | 获取日志详情 | ✅ |

### 用户管理 (`/api/users`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/users` | 获取用户列表 | ✅ |
| GET | `/api/users/{id}` | 获取用户详情 | ✅ |
| POST | `/api/users` | 创建用户 | ✅ |
| PUT | `/api/users/{id}` | 更新用户 | ✅ |
| DELETE | `/api/users/{id}` | 删除用户 | ✅ |

## 📦 统一响应格式

所有API响应都遵循统一格式：

### 成功响应

```json
{
  "success": true,
  "message": "操作成功",
  "data": {
    // 实际数据
  },
  "timestamp": "2025-12-16 12:00:00"
}
```

### 错误响应

```json
{
  "success": false,
  "message": "错误信息描述",
  "data": null,
  "timestamp": "2025-12-16 12:00:00"
}
```

### 分页响应

```json
{
  "success": true,
  "message": "查询成功",
  "data": {
    "pageIndex": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "hasPreviousPage": false,
    "hasNextPage": true,
    "items": [
      // 数据列表
    ]
  },
  "timestamp": "2025-12-16 12:00:00"
}
```

## 🔐 认证授权

### JWT Token认证

API使用JWT Bearer Token进行身份认证。

**获取Token**:

```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "your_password"
}
```

**响应**:

```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here",
    "userId": 1,
    "username": "admin",
    "realName": "管理员",
    "email": "admin@example.com",
    "role": "admin"
  }
}
```

**使用Token**:

在请求头中添加：

```
Authorization: Bearer {your_token}
```

### Token配置

在 `appsettings.json` 中配置：

```json
{
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "HailongConsulting.API",
    "Audience": "HailongConsulting.Client",
    "ExpireHours": 24
  }
}
```

## 📝 配置说明

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=hailong_consulting;User=root;Password=your_password;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "HailongConsulting.API",
    "Audience": "HailongConsulting.Client",
    "ExpireHours": 24
  },
  "FileUpload": {
    "RootPath": "wwwroot",
    "MaxFileSize": 10485760,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip"]
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

### CORS配置

默认配置允许所有来源。生产环境建议修改：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

## 📊 日志管理

### 日志配置

使用Serilog进行日志记录，日志文件位于 `logs/` 目录。

**日志级别**:
- `Debug`: 调试信息
- `Information`: 一般信息
- `Warning`: 警告信息
- `Error`: 错误信息
- `Fatal`: 致命错误

**日志文件**:
- 文件名格式: `log-{Date}.txt`
- 滚动策略: 每天一个文件
- 保留时间: 建议保留30天

### 系统日志

系统自动记录以下操作：
- 用户登录/登出
- 数据创建/更新/删除
- API请求（可选）
- 异常错误

查询系统日志：

```bash
GET /api/system-logs?pageIndex=1&pageSize=20
```

## 🔧 开发指南

### 添加新的API端点

1. **创建DTO**:

```csharp
// Models/DTOs/YourDto.cs
public class YourDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

2. **创建实体**:

```csharp
// Models/Entities/YourEntity.cs
public class YourEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

3. **创建仓储接口和实现**:

```csharp
// Repositories/IYourRepository.cs
public interface IYourRepository : IRepository<YourEntity>
{
    Task<YourEntity> GetByNameAsync(string name);
}

// Repositories/YourRepository.cs
public class YourRepository : Repository<YourEntity>, IYourRepository
{
    public YourRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<YourEntity> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
    }
}
```

4. **创建服务接口和实现**:

```csharp
// Services/IYourService.cs
public interface IYourService
{
    Task<ApiResponse<YourDto>> GetByIdAsync(int id);
}

// Services/YourService.cs
public class YourService : IYourService
{
    private readonly IYourRepository _repository;
    private readonly IMapper _mapper;
    
    public YourService(IYourRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<YourDto>> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        var dto = _mapper.Map<YourDto>(entity);
        return ApiResponse<YourDto>.SuccessResponse(dto);
    }
}
```

5. **创建控制器**:

```csharp
// Controllers/YourController.cs
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    private readonly IYourService _service;
    
    public YourController(IYourService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        return Ok(response);
    }
}
```

6. **注册服务**:

```csharp
// Program.cs
builder.Services.AddScoped<IYourRepository, YourRepository>();
builder.Services.AddScoped<IYourService, YourService>();
```

### 数据库迁移

使用EF Core Migrations管理数据库变更：

```bash
# 添加迁移
dotnet ef migrations add MigrationName

# 更新数据库
dotnet ef database update

# 回滚迁移
dotnet ef database update PreviousMigrationName

# 删除最后一个迁移
dotnet ef migrations remove
```

## 🚀 部署

### 发布应用

```bash
# 发布Release版本
dotnet publish -c Release -o ./publish

# 发布到指定运行时
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

### Docker部署

创建 `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HailongConsulting.API.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HailongConsulting.API.dll"]
```

构建和运行：

```bash
# 构建镜像
docker build -t hailong-api .

# 运行容器
docker run -d -p 5000:80 --name hailong-api hailong-api
```

### 生产环境配置

1. **修改连接字符串**（使用环境变量）
2. **更新JWT密钥**（至少32字符）
3. **配置HTTPS证书**
4. **设置CORS策略**
5. **配置日志级别**
6. **启用健康检查**

## 🐛 故障排查

### 常见问题

**1. 数据库连接失败**

```
检查项：
- MySQL服务是否运行
- 连接字符串是否正确
- 数据库用户权限
- 防火墙设置
```

**2. JWT认证失败**

```
检查项：
- Token是否过期
- JWT配置是否正确
- Authorization头格式: Bearer {token}
- 密钥长度是否足够（至少32字符）
```

**3. 文件上传失败**

```
检查项：
- wwwroot目录是否存在
- 文件大小是否超限
- 文件类型是否允许
- 目录写入权限
```

**4. CORS错误**

```
检查项：
- CORS策略配置
- 前端请求Origin
- 是否包含凭据
```

## 📚 相关文档

- [项目总体说明](../../README.md)
- [数据库设计文档](../../SQL/README.md)
- [前端门户文档](../../hailong-protral/README.md)
- [后台管理文档](../../hailong-admin/README.md)

## 📄 许可证

Copyright © 2025 河南海隆工程咨询有限公司

---

**最后更新**: 2025年12月16日  
**维护团队**: 海隆咨询技术部

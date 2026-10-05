# 海隆咨询后端 API

.NET 8 Web API，使用 EF Core 8、Pomelo MySQL、AutoMapper、JWT、Serilog、HtmlSanitizer 和 Swagger。依赖版本以 `HailongConsulting.API.csproj` 为准。

## 启动

先按 [数据库说明](../../SQL/README.md) 导入编号 00–10 的 SQL，再配置数据库连接和 JWT 密钥。API 自动迁移不能替代首次基础表与业务数据初始化。

从仓库根目录执行以下 PowerShell 示例，先替换占位值：

```powershell
$env:ConnectionStrings__DefaultConnection = '<本机 MySQL 连接字符串，数据库为 hailong_consulting>'
$env:Jwt__Key = '<至少 32 字节的本机随机密钥>'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project BackEnd/HailongConsulting.API --no-launch-profile --urls http://127.0.0.1:5000
```

API 显式使用 5000 端口，与两个前端的默认代理一致。真实凭据通过本机环境注入，不写入已跟踪文件；同一环境复用运行密钥。

| 地址 | 用途 |
| --- | --- |
| `/api/...` | 业务接口 |
| `/health` | 进程存活 |
| `/health/ready` | 数据库及门户设置表读取成功，失败返回 503 |
| `/` | Development 环境 Swagger UI |
| `/swagger/v1/swagger.json` | Development 环境接口定义 |
| `/uploads/...` | 上传文件与已恢复的初始化媒体 |

完整启动与部署说明见 [根 README](../../README.md) 和 [Ubuntu22 Docker 部署指南](../../docs/deployment/ubuntu22-docker.md)。

## 目录

```text
Controllers/          路由、授权及请求入口
Services/             业务、内容校验、附件引用、迁移与初始化
Repositories/         数据访问和查询投影
Data/                 EF Core 上下文
Models/DTOs/          请求及响应模型
Models/Entities/      业务实体与内容版本
Common/               映射、分页、密码与错误辅助
Middleware/           异常及操作日志
DatabaseMigrations/   内嵌版本化 SQL 迁移
SeedAssets/legacy/    已核验的旧站公开图片和 Word 附件
SiteSettingsDefaults.json  API 和门户共用的展示默认值
```

## 初始账户与会话

- 用户表为空时，首次启动生成 `admin` 及 12 位随机密码，写入进程工作目录下的 `logs/bootstrap/initial-admin-credentials.txt`。启动提示只给出用户名和文件位置，不输出密码。
- 已有用户时不会重新生成初始账号。首次登录后修改密码并删除凭据文件，获取方式见 [初始管理员](../../README.md#初始管理员)。
- 密码使用 ASP.NET Core Identity 的 PBKDF2 哈希。
- 登录请求验证非空；创建、重置和修改新密码要求至少 6 位。
- `/api/auth/login` 登录，`/api/auth/me` 查询当前用户，`/api/auth/refresh` 刷新令牌，`/api/auth/change-password` 修改密码，`/api/auth/logout` 撤销刷新令牌。退出不会立即撤销已经签发且仍有效的访问令牌。
- 最后一名未删除且启用的管理员不能被删除、停用或降级，保护在后端事务内执行。

## 查询与编辑约定

- 公告、资讯和企业资料的公开接口只返回启用内容。管理读取使用 `/api/announcements/manage`、`/api/info-publications/manage` 和 `/api/config/manage/...`，按角色授权。
- 后台查看详情不增加浏览量；列表只投影表格及摘要字段，正文在详情接口读取，地区批量查询。
- 公告、资讯及企业资料采用独立 `version`。更新请求携带读取时版本，冲突返回 409；浏览次数独立原子递增，不修改内容版本。
- `budgetAmount` 与 `awardAmount` 分别表示明确预算和最终中标 / 成交金额，单位为万元。金额字段省略时保留原值，显式 `null` 清空，0 为有效值。
- 请求验证必填、长度、枚举、分页及字段组合。HTML 经统一过滤器清洗；扫描图片正文可保存。
- 附件批量删除统一检查引用，任一文件在用则拒绝整批删除。上传选项通过 `/api/attachments/upload-options` 提供。
- 门户设置公开读取、仅管理员整份保存，并携带版本号，详见 [门户设置](../../docs/portal-settings.md)。
- 未知异常返回通用提示和 `traceId`，详细错误保留服务端；明确业务错误使用对应的 400 / 404 / 409。

接口路径、请求模型、字段限制和响应结构以实际控制器、DTO 及 Development Swagger 为准。

## 初始化、迁移与文件存储

`SQL/` 是空库首次初始化入口。API 每次启动按顺序执行内嵌迁移，数据库命名锁控制并发，已成功版本登记于 `schema_migrations`。新增结构应提供可重入 SQL，并同步首次初始化表结构；当前项目使用自有 SQL 迁移器。

启动时只将 `SeedAssets/legacy` 中缺失的文件补入 WebRoot 的 `uploads/legacy`，不覆盖已有文件。默认上传根目录为 `wwwroot`；Docker 将 `/app/wwwroot/uploads` 挂载到 `api-uploads`，与 Nginx 共享。日志位于 `logs`，Docker 使用 `api-logs` 卷。

部署账号需要迁移所需的建表 / 修改结构权限。更新前保留数据库、上传卷及 `.runtime/secrets.env`；首次 SQL 不会在已有数据目录自动重导。

## 测试

从仓库根目录执行：

```bash
dotnet test BackEnd/Protral.sln -c Release
```

测试覆盖权限、输入、版本冲突、查询、HTML / 图片、初始化资源、管理员保护及附件引用。MySQL 集成用例需要显式设置 `LOCAL_MYSQL_TEST_CONNECTION`，未设置时跳过；只接受 localhost / 127.0.0.1，使用随机 `hailong_cleanup_test_*` 数据库并清理，不使用连接指定的业务库。

[CI 工作流](../../.github/workflows/verify.yml) 使用独立 MySQL 验证 SQL、跨连接保护和迁移重入，并启动隔离服务执行后台浏览器回归。本机未运行的测试或远端执行状态应按实际输出判断。

最后更新：2026-10-05。

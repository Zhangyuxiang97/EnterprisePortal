# 海隆咨询官网

河南海隆工程咨询有限公司门户及后台管理系统，采用 Vue 3 前端、.NET 8 Web API 和 MySQL 8。门户与管理端分别构建，共用一个分层 API。

当前代码已完成本地开发与验证；正式部署状态以实际环境为准。主要分支为 `master`。

## 功能

- 门户：政府采购与建设工程公告、新闻中心、政策法规、企业简介、业务范围、资质、重要业绩、荣誉、联系方式及费用计算工具。
- 后台：内容编辑、门户设置、附件与引用管理、地区字典、用户、日志和统计。
- 公告：业务类型、采购子类、公告类型、地区与日期筛选，预算金额和中标 / 成交金额分别存储。
- 编辑保护：内容版本冲突检测、未保存提醒、会话刷新和失效后保留编辑内容。
- 访问控制：公开接口过滤停用内容，管理读取需要授权；后台查看不增加门户浏览量，后端保护最后一名有效管理员。

## 项目结构

```text
Protral/
├─ BackEnd/
│  ├─ HailongConsulting.API/       API、数据库迁移、展示默认值和初始化媒体
│  ├─ HailongConsulting.API.Tests/ 后端单元与集成测试
│  └─ Protral.sln
├─ hailong-protral/               门户、静态导航与首页模块配置
├─ hailong-admin/                 管理端
├─ SQL/                          按 00–10 编号的首次初始化 SQL 和复核报告
├─ config/                       非敏感部署参数示例
├─ docs/                         架构、配置、部署与维护文档
├─ nginx/                        前端镜像构建及反向代理配置
├─ scripts/                      数据转换、部署检查与回归工具
├─ portal-design-demos/          独立静态设计演示，不参与正式门户部署
├─ .github/workflows/verify.yml   自动验证
├─ docker-compose.yml
├─ deploy-ubuntu22-docker.sh      当前一键部署入口
└─ generate-runtime-secrets.sh    首次生成部署密钥
```

`.runtime/` 为本机运行目录，不提交 Git。两个前端分别维护自己的 `package.json` 和锁文件，根目录无需运行 `npm install`。

## 快速部署

当前部署主线是 **Ubuntu 22.04 + Docker Compose**。从服务器上的仓库根目录执行：

```bash
cp config/deployment.env.example config/deployment.env
chmod +x deploy-ubuntu22-docker.sh
sudo ./deploy-ubuntu22-docker.sh
```

脚本首次生成 `.runtime/secrets.env`，后续复用；构建镜像并启动服务后，检查 MySQL、API 就绪状态和门户可访问性。首次初始化 SQL 由 MySQL 在空数据目录启动时按编号执行。

| 服务 | 本地开发默认端口 | Docker 宿主默认端口 |
| --- | ---: | ---: |
| 门户 | 3000 | 8082，可通过 `PORTAL_HTTP_PORT` 调整 |
| 后台 | 3001 | 8080 |
| API | 5000，按下文显式启动 | 5001，通过 Nginx 代理 |

本地后台浏览器回归默认连接 3002，运行测试时需显式使用该端口或设置 `ADMIN_TEST_URL`；普通 `npm run dev` 默认使用 3001。端口占用时以终端输出为准。

完整安装、配置、更新与回退步骤见 [Ubuntu22 Docker 部署指南](docs/deployment/ubuntu22-docker.md)。更新前备份数据库、上传卷及运行密钥；不要通过删除数据卷重新初始化已有环境。

## 本地开发

需要 Node.js 20 或更高版本、.NET 8 SDK、MySQL 8 和 Git。

### 1. 获取代码及准备数据库

```bash
git clone https://github.com/Zhangyuxiang97/EnterprisePortal.git Protral
cd Protral
```

按 [数据库初始化说明](SQL/README.md) 的顺序导入 `SQL/00_*.sql` 至 `SQL/10_*.sql`。仅执行表结构和基础资料文件会缺少公告、资讯、地区及企业媒体记录。API 启动迁移用于升级已有结构，不能替代首次完整初始化。

### 2. 启动 API

在本机终端注入数据库连接及至少 32 字节的随机 JWT 密钥，不把真实凭据写入已跟踪文件。以下为 PowerShell 示例，先替换占位值：

```powershell
$env:ConnectionStrings__DefaultConnection = '<本机 MySQL 连接字符串，数据库为 hailong_consulting>'
$env:Jwt__Key = '<至少 32 字节的本机随机密钥>'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project BackEnd/HailongConsulting.API --no-launch-profile --urls http://127.0.0.1:5000
```

同一环境复用 JWT 密钥；改变密钥会使已有令牌失效。API 显式监听 5000，两个前端代理到该地址。

- 就绪检查：`http://127.0.0.1:5000/health/ready`，验证数据库和门户设置表可读取。
- 进程检查：`http://127.0.0.1:5000/health`。
- API 文档：Development 环境的根路径 Swagger UI，接口定义为 `/swagger/v1/swagger.json`。

### 3. 启动门户和后台

分别打开两个终端，从仓库根目录执行：

```bash
# 门户：http://127.0.0.1:3000
cd hailong-protral
npm ci
npm run dev
```

```bash
# 后台：http://127.0.0.1:3001
cd hailong-admin
npm ci
npm run dev
```

两个前端的 `VITE_API_BASE_URL` 默认为 `/api`；开发服务器将 `/api` 和 `/uploads` 代理到 `http://127.0.0.1:5000`。API 使用其他地址时，在启动 Vite 的终端设置 `API_PROXY_TARGET`。

## 初始管理员

用户表为空时，API 首次启动生成用户名 **`admin`** 和 **12 位随机密码**。

- 本地：查看 API 启动输出提示的 `logs/bootstrap/initial-admin-credentials.txt` 路径，该文件位于 API 进程的工作目录。
- Docker：从仓库目录执行以下命令读取容器中的凭据文件：

```bash
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env exec api cat /app/logs/bootstrap/initial-admin-credentials.txt
```

首次登录后修改密码并删除该凭据文件。密码不会输出到应用日志，不应写入 README 或 Git。已有用户时重启不会重新生成账号。登录只检查密码非空；创建、重置和修改新密码要求至少 6 位。

## 配置位置

| 内容 | 位置 / 维护入口 |
| --- | --- |
| 公司名称、标语、联系方式、地图、交通及 FAQ | 后台“门户设置”，存储于 `portal_site_settings` |
| 同一组字段的初始化及回退默认值 | `BackEnd/HailongConsulting.API/SiteSettingsDefaults.json`，API 和门户共用 |
| 导航、页脚链接、首页模块启用与顺序 | `hailong-protral/config/site-config.json`，修改后重新构建门户 |
| 企业简介正文、业务、资质、荣誉、业绩 | 后台“企业资料”，存储于对应业务表 |
| 门户 Docker 宿主端口 | `config/deployment.env`，示例为 `config/deployment.env.example` |
| 数据库和 JWT 运行密钥 | `.runtime/secrets.env`，不提交 Git |
| 前端构建参数 | 各前端 `.env.*` |
| API 非敏感默认配置 | `appsettings*.json`，运行时可通过环境变量覆盖 |

详见 [配置存储位置](docs/configuration/storage.md) 和 [门户站点设置](docs/portal-settings.md)。

## 初始化数据与上传资源

仓库已包含可用于首次部署的生成结果，无需提供原始 MDB 或再次抓取旧站：

- **2526 条公告**、**43 篇资讯**，以及地区字典、企业简介、业务范围和友情链接。
- 公告复核覆盖业务类型、采购子类、地区、预算和中标 / 成交金额。金额统一以万元存储，两个字段独立；证据不足的金额或地区保留为空，原因记录在复核报告。
- **4 项证照资料、6 组企业荣誉**，恢复 **28 张图片和 1 个 Word 附件**。重要业绩缺少真实来源，初始化保持空白，可由后台补充。
- 资源存储在 API 的 `SeedAssets/legacy`，启动时仅将缺失文件补入 `/uploads/legacy/`，不覆盖已存在文件。其余缺失附件和外链限制见数据库文档。

Docker 的 `mysql-data` 保存数据库，`api-uploads` 保存上传文件，`api-logs` 保存日志及初始凭据文件；Nginx 和 API 共用上传卷。更新与备份时须保留数据库和上传文件。

首次 SQL 只会在 MySQL 数据目录为空时自动执行；更新已有环境不会重新导入全量历史数据。原始 MDB、中间 JSON、本机数据库和运行凭据均未提交。

完整数量、分类规则、未解决的证据问题和重新生成步骤见 [SQL README](SQL/README.md)。

## 构建与验证

从仓库根目录执行配置及后端检查：

```bash
node scripts/check-project-config.mjs
dotnet test BackEnd/Protral.sln -c Release
```

各子项目在自己的目录执行：

| 目录 | 安装、测试与构建 |
| --- | --- |
| `hailong-admin` | `npm ci`、`npm test`、`npm run build` |
| `hailong-protral` | `npm ci`、`node --test tests/*.test.mjs`、`npm run build` |
| `scripts` | `npm ci`、`node --test tests/*.test.js` |

后台浏览器回归需要本机独立测试环境及测试管理员，命令为 `npm run test:browser`；配置和清理范围见 [后台 README](hailong-admin/README.md)。MySQL 集成测试需显式提供 `LOCAL_MYSQL_TEST_CONNECTION`，使用随机临时数据库验证并发保护和迁移。

[GitHub Actions](.github/workflows/verify.yml) 在 push / pull request 时执行配置检查、测试和构建，并在独立 MySQL 中验证首次 SQL、迁移及浏览器操作。推送后执行状态以仓库 Actions 页面为准。

## 文档导航

- [全部文档](docs/README.md)
- [后端 API](BackEnd/HailongConsulting.API/README.md)
- [后台管理](hailong-admin/README.md)
- [前端门户](hailong-protral/README.md)
- [数据库初始化及来源复核](SQL/README.md)
- [部署指南](docs/deployment/ubuntu22-docker.md)
- [配置位置](docs/configuration/storage.md)
- [维护与故障排查](docs/operations/maintenance.md)
- [文档维护方案](implementation_plan.md)

最后更新：2026-10-05。

# 配置存储位置

| 内容 | 位置 | 生效方式 |
| --- | --- | --- |
| 公司标语、联系方式、交通和 FAQ | 数据库 `portal_site_settings` | 后台保存，门户刷新读取 |
| 同一组展示字段的默认值 | `BackEnd/HailongConsulting.API/SiteSettingsDefaults.json` | API 内嵌，门户构建直接引用，只有一个源文件 |
| 导航、页脚链接和首页模块 | `hailong-protral/config/site-config.json` | 门户重新构建 |
| 门户宿主端口 | `config/deployment.env` | 重新执行部署；示例文件可提交 |
| 数据库与 JWT 凭据 | `.runtime/secrets.env` | 运行时读取，不提交 Git |
| 前端 API 地址等构建参数 | 各前端现有 `.env*` | 构建时读取 |
| 后端非敏感配置 | API `appsettings*.json` | API 重启读取 |

根目录 `docker-compose.yml` 与 `deploy-ubuntu22-docker.sh` 是部署入口。两个前端拥有各自的 package.json 和锁文件，依赖在对应项目目录安装。

浏览器地图 Key 属于公开展示配置。门户把最近一次有效展示配置缓存到 localStorage，最多使用七天，每次完整打开页面仍优先请求服务器；请求失败最多尝试三次。缓存不包含管理员凭据、导航或部署参数。

后台整份设置保存携带版本号。版本不匹配时返回 409，并保留当前表单；重新加载最新内容后合并再保存。数据库版本字段也参与 EF 并发校验。

从门户返回首页时，为恢复滚动位置会立即挂载全部模块；首次打开时首屏和公司简介优先加载，其他模块接近可视区域后加载。

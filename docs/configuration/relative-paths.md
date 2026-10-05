# API 与上传路径

## 前端请求

两个前端的开发和生产配置均使用：

```env
VITE_API_BASE_URL=/api
```

业务请求以 `/api` 为基地址，上传资源以 `/uploads/...` 访问。

| 环境 | `/api` | `/uploads` |
| --- | --- | --- |
| 本地 Vite | 代理到 API | 代理到 API |
| Docker Nginx | 代理到 `api:5000` | 读取共享上传卷 |

本地 API 默认代理地址为 `http://127.0.0.1:5000`。在启动 Vite 的终端设置 `API_PROXY_TARGET` 可修改该地址；门户默认端口为 3000，后台为 3001。

配置见 [门户 Vite](../../hailong-protral/vite.config.js)、[后台 Vite](../../hailong-admin/vite.config.js) 和 [Nginx 路由](../../nginx/conf.d/default.conf)。API 路由保留 `/api` 前缀，代理不移除前缀。

## 文件存储

API 默认 WebRoot 为 `wwwroot`，上传文件存于其下的 `uploads`。文件访问地址以服务器返回值为准，页面不重复拼接目录前缀。

Docker 挂载关系：

```text
api-uploads 卷
├─ API：/app/wwwroot/uploads
└─ Nginx：/usr/share/nginx/html/uploads（只读）
```

正文、封面和附件都需要保留实际文件。只备份数据库中的地址不能恢复文件；升级或迁移时同时备份数据库及整个上传卷。

## 初始化媒体

已核验的图片和 Word 附件随 API 的 `SeedAssets/legacy` 发布。启动时只补入上传目录中缺失的文件，对外路径为 `/uploads/legacy/...`。

文件大小、SHA-256、原始来源及缺失附件清单见 [数据库说明](../../SQL/README.md)。图片恢复使用仓库发布资源，不需要连接原站。

## 图片无法加载时

1. 在浏览器网络面板确认实际请求地址和 HTTP 状态。
2. 检查 `/uploads/...` 是否能直接访问，路径中是否出现重复的 `/api` 或 `/uploads`。
3. 检查 API 上传目录与 Nginx 卷挂载是否对应，并核对卷中的文件。
4. 本地开发检查 Vite 代理的 API 地址。
5. 迁移环境检查是否同时迁入数据库和上传文件。

部署及持久化配置见 [Docker Compose](../../docker-compose.yml) 和 [维护说明](../operations/maintenance.md)。

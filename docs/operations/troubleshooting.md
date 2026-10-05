# Docker 故障排查

当前部署入口为 `deploy-ubuntu22-docker.sh`，历史宿主机服务排查内容位于 `legacy/troubleshooting.md`。

## 门户无法访问

1. 核对 `config/deployment.env` 的 `PORTAL_HTTP_PORT`，默认访问 `http://服务器IP:8082`。
2. 从服务器访问对应端口，检查 `docker compose ps` 中 nginx 状态。
3. 检查 UFW、服务器安全组与外部反向代理是否放行实际宿主端口。
4. 容器内部监听 80，不代表宿主机也是 80。

## API 未就绪

```bash
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env ps
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env logs --tail=100 api mysql
curl -fsS http://127.0.0.1:5001/health/ready
```

- `/health/ready` 返回 503：检查数据库连接、版本化迁移及门户设置表的版本字段。
- Nginx 返回 502：检查 API 容器启动与健康状态。
- MySQL 认证失败：核对现有数据库账号和受限密钥文件；不要删除文件生成新密码。
- 构建失败：检查依赖下载和构建日志；当前脚本尚未停止旧容器。

## 门户设置未更新

- 后台保存出现冲突：说明版本已经变化，保留当前内容，重新加载最新配置后合并保存。
- 门户完整刷新仍显示旧内容：检查 `/api/config/site-settings`；请求失败时会使用七天内有效缓存或构建默认值，最多尝试三次。
- 后台首次保存前：使用唯一默认文件 `BackEnd/HailongConsulting.API/SiteSettingsDefaults.json`。
- 静态导航、首页模块和构建参数：修改后需要重新构建门户。

## 本地验证

```bash
node scripts/check-project-config.mjs
node --test hailong-protral/tests/*.test.mjs
bash -n deploy-ubuntu22-docker.sh
bash scripts/tests/deployment-readiness.test.sh
dotnet test BackEnd/Protral.sln
```

两个前端分别执行 `npm ci` 和 `npm run build`。部署验证测试使用替身，不访问 Docker、网络或真实凭据。

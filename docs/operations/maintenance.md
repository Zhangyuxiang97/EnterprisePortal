# Docker 日常维护

在仓库根目录使用 Docker Compose 管理服务。

## 状态和日志

在仓库根目录执行；部署参数文件存在时同时加载，默认门户端口为 8082。

```bash
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env ps
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env logs --tail=100 api nginx mysql
curl -fsS http://127.0.0.1:5001/health/ready
```

API 就绪检查会读取门户设置表的版本字段。单独 `/health` 只用于确认 API 进程存活。日志和首次管理员凭据可能包含敏感信息，不应提交 Git。

## 备份

升级前备份数据库、`api-uploads` 卷和受限密钥文件。数据库备份示例：

```bash
umask 077
backup_file="hailong-$(date +%Y%m%d-%H%M%S).sql"
mysql_root_password=$(sed -n 's/^MYSQL_ROOT_PASSWORD=//p' .runtime/secrets.env)
docker exec -e MYSQL_PWD="$mysql_root_password" hailong-mysql mysqldump --single-transaction --routines --events -u root hailong_consulting > "$backup_file"
unset mysql_root_password
```

不要直接 `source .runtime/secrets.env`：其中连接字符串是 dotenv 内容，不能作为 shell 脚本执行。备份文件包含业务数据，应存放到受控位置；定期在隔离数据库中验证恢复。上传卷的实际名称由 Compose 项目名决定，可用 `docker inspect hailong-api` 的 Mounts 确认。

## 更新

使用 [Ubuntu22 Docker 部署指南](../deployment/ubuntu22-docker.md) 中的流程。镜像先构建，再更新容器；不要在构建前停止整组服务。保留更新前的代码版本、镜像 ID 和备份。

不要通过删除数据卷来解决启动错误。变更数据库凭据需要同时处理数据库账号与运行配置，不能仅重新生成 `.runtime/secrets.env`。

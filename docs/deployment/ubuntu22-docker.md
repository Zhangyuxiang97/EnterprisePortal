# Ubuntu22 Docker 部署指南

## 部署入口与目录

从仓库根目录执行 `deploy-ubuntu22-docker.sh`。Compose 配置以根目录 `docker-compose.yml` 为准，镜像构建时自动构建两个前端和 .NET 8 API。Ubuntu 22.04 服务器需要网络访问 Docker 镜像源及依赖源。

```bash
cd /opt/hailong/project
cp config/deployment.env.example config/deployment.env
chmod +x deploy-ubuntu22-docker.sh
sudo ./deploy-ubuntu22-docker.sh
```

脚本会询问项目的绝对路径；后续部署复用 `.runtime/secrets.env`。该文件不提交 Git，权限为 600，包含数据库和 JWT 凭据，应安全备份。不要删除密钥文件后对现有数据库卷重新生成密码。

## 端口配置

| 宿主端口 | 用途 | 配置来源 |
| --- | --- | --- |
| 8082（默认） | 门户 | `PORTAL_HTTP_PORT` |
| 8080 | 管理后台 | Compose |
| 5001 | API 反向代理 | Compose |

门户地址默认为 `http://服务器IP:8082`。Nginx 容器内部仍监听 80。在 `config/deployment.env` 中修改 `PORTAL_HTTP_PORT`，脚本会同时用于 Compose、防火墙和完成提示。命令行环境变量优先于文件配置；不能与后台或 API 端口冲突。

```bash
# 示例：将门户宿主端口改为 80
sudo env PORTAL_HTTP_PORT=80 ./deploy-ubuntu22-docker.sh
```

直接执行 Compose 时请同时加载部署参数和密钥；若不使用自定义端口，可仅指定密钥文件。
临时环境变量只在当前命令中生效；长期使用自定义端口时，应同时更新 `config/deployment.env`。

```bash
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env config --quiet
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env ps
docker compose --env-file config/deployment.env --env-file .runtime/secrets.env logs --tail=100 api
```

## 更新与验证

更新前备份数据库和上传文件，保留当前代码版本、运行镜像 ID 与密钥。脚本先构建镜像，成功后执行 `up -d --no-build` 更新容器；构建失败不会主动停止旧服务。容器替换仍可能产生短暂中断。

API 启动时按顺序执行版本化数据库迁移。门户配置增加设置表和版本字段；升级不会覆盖管理员已保存的展示内容。

脚本必须完成以下检查才输出部署成功：MySQL 就绪、API `/health/ready` 返回成功、门户首页可访问、数据库存在业务表。API 就绪端点会读取门户设置表，以验证连接和迁移；HTTP 错误或等待超时会返回非零退出码。`/health` 仅表示 API 进程存活。

```bash
curl -fsS http://127.0.0.1:5001/health/ready
curl -fsS http://127.0.0.1:8082/ > /dev/null
```

首次管理员信息从容器内 `logs/bootstrap/initial-admin-credentials.txt` 获取，首次登录后修改密码。不要将凭据粘贴进工单或 Git。

## 故障与回退

- 构建失败：检查构建日志，修复后重试；尚未进入容器替换阶段。
- 启动或验证失败：查看 `ps` 和对应服务日志，检查数据库连接、迁移和端口占用。脚本不会自动删除数据卷或执行数据库回滚。
- 应用回退：恢复事先记录的镜像或代码版本，再更新容器。数据库迁移需先评估旧应用兼容性，不能只切换代码就假定数据库已回退。
- 修改门户端口后，核对外部安全组和反向代理；脚本只管理本机 UFW 规则。

历史部署文档位于 `legacy/`，已停止作为执行依据。

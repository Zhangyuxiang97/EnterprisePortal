# 🐳 Docker版本部署使用说明

## 📋 两种部署方式对比

### 传统部署 vs Docker部署

| 特性 | 传统部署 | Docker部署 |
|------|---------|-----------|
| **脚本文件** | [`deploy-centos7.sh`](deploy-centos7.sh) | [`deploy-centos7-docker.sh`](deploy-centos7-docker.sh) |
| **部署难度** | ⭐⭐ 简单 | ⭐⭐⭐⭐ 较复杂 |
| **学习成本** | ✅ 低 | ❌ 高（需要了解Docker） |
| **环境隔离** | ❌ 直接安装到系统 | ✅ 完全隔离 |
| **迁移便利** | ❌ 需重新配置 | ✅ 非常方便 |
| **资源占用** | ✅ 较低 | ⭐⭐ 稍高 |
| **调试难度** | ✅ 简单 | ⭐⭐⭐ 较复杂 |
| **适合新手** | ✅ 推荐 | ❌ 不推荐 |
| **部署时间** | 15-30分钟 | 20-40分钟 |

## 🚀 Docker版本快速部署

### 前提条件

- CentOS 7 64位系统
- 至少4GB内存（推荐8GB）
- 至少20GB可用磁盘空间
- 可访问互联网

### 部署步骤

#### 1. 上传项目文件

将整个项目上传到服务器的 `/opt/hailong/project` 目录。

```bash
# 在服务器上创建目录
mkdir -p /opt/hailong/project

# 使用WinSCP或其他工具上传项目文件
```

#### 2. 上传并运行部署脚本

```bash
# 上传 deploy-centos7-docker.sh 到 /root/ 目录

# 赋予执行权限
cd /root
chmod +x deploy-centos7-docker.sh

# 运行脚本
./deploy-centos7-docker.sh
```

#### 3. 按提示输入配置

脚本只询问项目路径；首次运行会自动生成并打印一次随机密钥，随后复用 `.runtime/secrets.env`：

```
项目文件路径 (默认: /opt/hailong/project): [回车]
确认开始部署? (y/n): y
```

#### 4. 等待部署完成

部署过程约需 **20-40分钟**，脚本会自动完成：

- ✅ 安装Docker和Docker Compose
- ✅ 安装Node.js
- ✅ 构建前端项目
- ✅ 生成docker-compose.yml配置
- ✅ 构建并启动所有Docker容器
- ✅ 配置防火墙

## 📦 Docker容器说明

部署完成后会创建3个Docker容器：

### 1. hailong-mysql
- **镜像**: mysql:8.0
- **端口**: 3306
- **功能**: MySQL数据库
- **数据持久化**: mysql-data卷

### 2. hailong-api
- **镜像**: 自动构建（基于.NET 8.0）
- **端口**: 5000（内部）
- **功能**: 后端API服务
- **数据持久化**: api-uploads卷、api-logs卷

### 3. hailong-nginx
- **镜像**: nginx:alpine
- **端口**: 80（前端门户）、8080（后台管理）、5001（API代理）
- **功能**: Web服务器和反向代理

## 🔧 常用Docker命令

### 查看容器状态

```bash
cd /opt/hailong/project
docker-compose ps
```

### 查看日志

```bash
# 查看所有容器日志
docker-compose logs -f

# 查看特定容器日志
docker-compose logs -f api
docker-compose logs -f mysql
docker-compose logs -f nginx
```

### 重启服务

```bash
# 重启所有服务
docker-compose restart

# 重启特定服务
docker-compose restart api
docker-compose restart nginx
docker-compose restart mysql
```

### 停止和启动

```bash
# 停止所有服务
docker-compose down

# 启动所有服务
docker-compose up -d

# 停止特定服务
docker-compose stop api

# 启动特定服务
docker-compose start api
```

### 进入容器

```bash
# 进入API容器
docker exec -it hailong-api bash

# 进入MySQL容器
docker exec -it hailong-mysql bash

# 在MySQL容器中执行SQL
docker exec -it hailong-mysql mysql -u root -p
```

### 查看资源使用

```bash
# 查看所有容器资源使用
docker stats

# 查看特定容器
docker stats hailong-api hailong-mysql hailong-nginx
```

## 💾 数据备份

### 备份数据库

```bash
# 备份数据库
docker exec hailong-mysql mysqldump -u root -p密码 hailong_consulting > backup.sql

# 恢复数据库
docker exec -i hailong-mysql mysql -u root -p密码 hailong_consulting < backup.sql
```

### 备份上传文件

```bash
# 备份上传的文件
docker run --rm -v protral_api-uploads:/data -v $(pwd):/backup alpine tar czf /backup/uploads-backup.tar.gz -C /data .

# 恢复上传的文件
docker run --rm -v protral_api-uploads:/data -v $(pwd):/backup alpine tar xzf /backup/uploads-backup.tar.gz -C /data
```

## 🔄 更新部署

### 更新后端代码

```bash
cd /opt/hailong/project

# 拉取最新代码
git pull

# 重新构建并启动API
docker-compose build api
docker-compose up -d api
```

### 更新前端代码

```bash
cd /opt/hailong/project

# 拉取最新代码
git pull

# 重新构建前端
cd hailong-admin
npm run build

cd ../hailong-protral
npm run build

# 重启Nginx
cd ..
docker-compose restart nginx
```

### 完全重新部署

```bash
cd /opt/hailong/project

# 停止并删除所有容器（保留数据）
docker-compose down

# 停止并删除所有容器和数据（⚠️ 会删除数据库）
docker-compose down -v

# 重新启动
docker-compose up -d --build
```

## 🔍 故障排查

### 容器无法启动

```bash
# 查看容器状态
docker-compose ps

# 查看详细日志
docker-compose logs 容器名

# 常见原因：
# - 端口被占用
# - 配置文件错误
# - 依赖服务未就绪
```

### 数据库连接失败

```bash
# 检查MySQL容器状态
docker-compose ps mysql

# 查看MySQL日志
docker-compose logs mysql

# 测试数据库连接
docker exec -it hailong-mysql mysql -u root -p
```

### API无法访问

```bash
# 查看API日志
docker-compose logs api

# 检查API健康状态
curl http://localhost:5001/api/home/statistics

# 进入API容器检查
docker exec -it hailong-api bash
```

### 前端页面空白

```bash
# 检查Nginx配置
docker exec hailong-nginx nginx -t

# 查看Nginx日志
docker-compose logs nginx

# 检查前端文件
docker exec hailong-nginx ls -la /usr/share/nginx/html/admin
docker exec hailong-nginx ls -la /usr/share/nginx/html/portal
```

## 🔐 安全建议

### 1. 轮换运行时密钥

```bash
# 密钥位于 .runtime/secrets.env（权限 600）。轮换 MySQL 密码时，
# 需先在数据库执行 ALTER USER，再同步更新该文件并重启相关服务。
```

### 2. 限制MySQL端口访问

编辑 `docker-compose.yml`，将MySQL端口改为仅本地访问：

```yaml
mysql:
  ports:
    - "127.0.0.1:3306:3306"  # 仅本地访问
```

### 3. 定期备份

创建自动备份脚本：

```bash
cat > /root/backup-docker.sh <<'EOF'
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR=/backup
mkdir -p $BACKUP_DIR

# 备份数据库
docker exec hailong-mysql mysqldump -u root -p密码 hailong_consulting | gzip > $BACKUP_DIR/mysql_${DATE}.sql.gz

# 删除7天前的备份
find $BACKUP_DIR -name "*.gz" -mtime +7 -delete
EOF

chmod +x /root/backup-docker.sh

# 添加定时任务（每天凌晨2点）
crontab -e
# 添加：0 2 * * * /root/backup-docker.sh
```

## 📊 性能监控

```bash
# 实时查看容器资源使用
docker stats

# 查看容器详细信息
docker inspect hailong-api

# 查看Docker磁盘使用
docker system df

# 清理未使用的资源
docker system prune -a
```

## 🆚 何时选择Docker部署？

### ✅ 适合使用Docker的场景

- 熟悉Docker和容器技术
- 需要快速迁移到其他服务器
- 多环境部署（开发、测试、生产）
- 需要环境隔离
- 团队协作开发

### ❌ 不适合使用Docker的场景

- 不熟悉Linux和Docker
- 只需要单一环境部署
- 追求最佳性能
- 需要频繁调试
- 快速上线，不想学习新技术

## 💡 建议

**对于新手用户**：
- 建议先使用传统部署方式（[`deploy-centos7.sh`](deploy-centos7.sh)）
- 等系统稳定运行后，再考虑学习Docker
- 参考：[`快速开始-CentOS7.md`](快速开始-CentOS7.md)

**对于有经验的用户**：
- Docker部署更适合生产环境
- 便于版本管理和快速回滚
- 易于迁移和扩展

## 📚 相关文档

- [`deploy-centos7.sh`](deploy-centos7.sh) - 传统部署脚本
- [`deploy-centos7-docker.sh`](deploy-centos7-docker.sh) - Docker部署脚本
- [`快速开始-CentOS7.md`](快速开始-CentOS7.md) - 传统部署指南
- [`Docker快速部署指南.md`](Docker快速部署指南.md) - Docker详细文档
- [`VMware-CentOS7-完整部署指南.md`](VMware-CentOS7-完整部署指南.md) - VMware部署指南

---

**祝您部署顺利！** 🎉

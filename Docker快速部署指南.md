# 🐳 Docker快速部署指南

> **重要提示**：对于不熟悉Linux和Docker的用户，强烈建议使用传统部署方式（参考[`快速开始-CentOS7.md`](快速开始-CentOS7.md)）

> **当前生产 Compose 规则**：仅 Nginx 发布宿主机端口；MySQL 和 API 仅在 Docker 内部网络可访问。前端由 `nginx/Dockerfile` 在 `docker compose up -d --build` 时构建，不需要再手工生成或挂载 `dist` 目录。本文后续历史示例如与仓库根目录的 `docker-compose.yml` 冲突，以该文件为准。

## 📋 Docker方案架构说明

### 容器架构图

```
┌─────────────────────────────────────────────────────────┐
│                    Docker Host (CentOS 7)               │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │           Nginx容器 (hailong-nginx)            │    │
│  │  端口映射:                                      │    │
│  │  - 80:80    → 前端门户                         │    │
│  │  - 8080:8080 → 后台管理                        │    │
│  │  - 5001:5001 → API代理                         │    │
│  └────────────────────────────────────────────────┘    │
│                         ↓                               │
│  ┌────────────────────────────────────────────────┐    │
│  │         后端API容器 (hailong-api)              │    │
│  │  内部端口: 5000                                 │    │
│  │  .NET 8.0 应用                                  │    │
│  └────────────────────────────────────────────────┘    │
│                         ↓                               │
│  ┌────────────────────────────────────────────────┐    │
│  │         MySQL容器 (hailong-mysql)              │    │
│  │  内部端口: 3306                                 │    │
│  │  数据库: hailong_consulting                     │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### 访问流程

1. **前端门户访问**: 
   - 浏览器 → `http://服务器IP:80` → Nginx容器 → 返回前端页面

2. **后台管理访问**: 
   - 浏览器 → `http://服务器IP:8080` → Nginx容器 → 返回后台页面

3. **API调用**: 
   - 前端 → `http://服务器IP:5001/api/xxx` → Nginx容器 → API容器(5000端口) → MySQL容器

### 容器说明

所有服务都运行在Docker容器中：
- ✅ **MySQL数据库** → `hailong-mysql`容器（MySQL 8.0）
- ✅ **后端API** → `hailong-api`容器（.NET 8.0）
- ✅ **Nginx** → `hailong-nginx`容器（前端静态文件 + API反向代理）

### 数据库自动初始化机制

MySQL容器使用官方镜像的自动初始化功能：

```yaml
volumes:
  - ./SQL:/docker-entrypoint-initdb.d:ro
```

**工作原理**：
1. MySQL容器首次启动时，会自动扫描 `/docker-entrypoint-initdb.d` 目录
2. 按**字母顺序**执行该目录下的所有 `.sql`、`.sql.gz`、`.sh` 文件
3. 执行顺序：
   - `hailong_consulting_init_data.sql` - 先执行（数据初始化）
   - `hailong_consulting_schema.sql` - 后执行（结构初始化）

**重要说明**：
- ⚠️ **仅在首次启动时执行**：如果数据卷 `mysql-data` 已存在数据，初始化脚本不会再次执行
- ✅ **自动创建数据库**：通过环境变量 `MYSQL_DATABASE=hailong_consulting` 自动创建
- ✅ **自动创建用户**：通过环境变量 `MYSQL_USER` 和 `MYSQL_PASSWORD` 自动创建应用用户
- 📝 **执行日志**：可通过 `docker-compose logs mysql` 查看初始化过程

**SQL文件说明**：
- [`SQL/hailong_consulting_schema.sql`](SQL/hailong_consulting_schema.sql:1) - 数据库表结构（15张表 + 1个视图）
- [`SQL/hailong_consulting_init_data.sql`](SQL/hailong_consulting_init_data.sql:1) - 初始数据（默认用户、区域字典等）

## 📦 部署文件准备

### 1. docker-compose.yml

项目根目录下的 [`docker-compose.yml`](docker-compose.yml:1) 文件定义了所有服务：

```yaml
version: '3.8'

services:
  # MySQL数据库
  mysql:
    image: mysql:8.0
    container_name: hailong-mysql
    restart: always
    env_file:
      - .runtime/secrets.env
    environment:
      MYSQL_DATABASE: hailong_consulting
      MYSQL_USER: hailong_app
    volumes:
      - mysql-data:/var/lib/mysql
      - ./SQL:/docker-entrypoint-initdb.d
    ports:
      - "3306:3306"
    networks:
      - hailong-network

  # 后端API
  api:
    build:
      context: ./BackEnd/HailongConsulting.API
      dockerfile: Dockerfile
    container_name: hailong-api
    restart: always
    env_file:
      - .runtime/secrets.env
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    ports:
      - "5000:5000"
    depends_on:
      - mysql
    networks:
      - hailong-network
    volumes:
      - api-uploads:/app/wwwroot/uploads

  # Nginx（前端+反向代理）
  nginx:
    image: nginx:alpine
    container_name: hailong-nginx
    restart: always
    ports:
      - "80:80"
      - "8080:8080"
      - "5001:5001"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf
      - ./nginx/conf.d:/etc/nginx/conf.d
      - ./hailong-admin/dist:/usr/share/nginx/html/admin
      - ./hailong-protral/dist:/usr/share/nginx/html/portal
    depends_on:
      - api
    networks:
      - hailong-network

networks:
  hailong-network:
    driver: bridge

volumes:
  mysql-data:
  api-uploads:
```

### 2. API的Dockerfile

[`BackEnd/HailongConsulting.API/Dockerfile`](BackEnd/HailongConsulting.API/Dockerfile:1) 用于构建后端API镜像：

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HailongConsulting.API.csproj", "./"]
RUN dotnet restore "HailongConsulting.API.csproj"
COPY . .
RUN dotnet build "HailongConsulting.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HailongConsulting.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "HailongConsulting.API.dll"]
```

### 3. Nginx配置

[`nginx/nginx.conf`](nginx/nginx.conf:1) 和 [`nginx/conf.d/default.conf`](nginx/conf.d/default.conf:1) 配置文件。

**当前配置**：HTTP部署（端口80、8080、5001）

**HTTPS配置**：参见下方"HTTPS部署配置"章节

## 🚀 快速部署步骤

### 前提条件：安装Docker和Docker Compose

```bash
# 1. 安装Docker
yum install -y yum-utils
yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
yum install -y docker-ce docker-ce-cli containerd.io

# 2. 启动Docker
systemctl start docker
systemctl enable docker

# 3. 安装Docker Compose
curl -L "https://github.com/docker/compose/releases/download/v2.20.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
chmod +x /usr/local/bin/docker-compose

# 4. 验证安装
docker --version
docker-compose --version
```

### 第1步：准备项目文件

```bash
# 将项目上传到服务器
cd /opt/hailong/project

# 确保以下文件存在：
# - docker-compose.yml
# - BackEnd/HailongConsulting.API/Dockerfile
# - nginx/nginx.conf
# - nginx/conf.d/default.conf
# - SQL/hailong_consulting_schema.sql（数据库结构）
# - SQL/hailong_consulting_init_data.sql（初始数据）

# 验证SQL文件
ls -lh SQL/
# 应该看到：
# hailong_consulting_schema.sql
# hailong_consulting_init_data.sql
# README.md
```

### 第2步：构建前端

```bash
# 安装Node.js（如果还没有）
curl -fsSL https://rpm.nodesource.com/setup_18.x | bash -
yum install -y nodejs

# 构建后台管理系统
cd /opt/hailong/project/hailong-admin
npm install --registry=https://registry.npmmirror.com
npm run build

# 构建前端门户
cd /opt/hailong/project/hailong-protral
npm install --registry=https://registry.npmmirror.com
npm run build
```

### 第3步：启动Docker容器

```bash
cd /opt/hailong/project

# 启动所有服务（首次启动会自动构建镜像）
docker-compose up -d

# 查看启动状态
docker-compose ps

# 查看日志
docker-compose logs -f
```

**启动过程说明**：
1. **MySQL容器**先启动
   - 创建数据库 `hailong_consulting`
   - 创建用户 `hailong_app`
   - 自动执行 `/docker-entrypoint-initdb.d` 目录下的SQL脚本：
     - `hailong_consulting_init_data.sql` - 初始化数据
     - `hailong_consulting_schema.sql` - 创建表结构
   - 等待健康检查通过（约10-30秒）

2. **API容器**等待MySQL健康检查通过后启动
   - 构建.NET应用镜像（首次启动需要几分钟）
   - 连接MySQL数据库
   - 启动API服务（监听5000端口）

3. **Nginx容器**最后启动
   - 加载前端静态文件
   - 配置API反向代理
   - 提供Web访问服务

**首次启动时间**：约3-5分钟（包含镜像构建和数据库初始化）

### 第4步：验证部署

```bash
# 检查容器状态（应该都是Up状态）
docker-compose ps

# 输出示例：
# NAME              STATUS          PORTS
# hailong-mysql     Up 2 minutes    0.0.0.0:3306->3306/tcp
# hailong-api       Up 1 minute     0.0.0.0:5000->5000/tcp
# hailong-nginx     Up 1 minute     0.0.0.0:80->80/tcp, 0.0.0.0:8080->8080/tcp, 0.0.0.0:5001->5001/tcp

# 测试API是否正常
curl http://localhost:5001/api/home/statistics

# 验证数据库初始化
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" -e "USE hailong_consulting; SHOW TABLES;"
# 应该看到15张表

# 检查初始数据
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" -e "USE hailong_consulting; SELECT COUNT(*) FROM users;"
# 应该有至少1条记录（admin用户）
```

### 第5步：访问系统

在浏览器中访问（假设服务器IP是192.168.1.100）：

- **前端门户**: http://192.168.1.100
- **后台管理**: http://192.168.1.100:8080
- **API测试**: http://192.168.1.100:5001/api/home/statistics

初始管理员凭据：首次启动时由应用随机生成，并仅输出到 API 容器日志和
`logs/bootstrap/initial-admin-credentials.txt`。请在首次登录后立即修改密码并删除该凭据文件。

**注意**：当前为HTTP部署，如需HTTPS请参考下方"HTTPS部署配置"章节。

## 🔧 Docker常用命令

### 容器管理

```bash
# 查看运行的容器
docker-compose ps

# 查看所有容器（包括停止的）
docker ps -a

# 启动所有服务
docker-compose up -d

# 停止所有服务
docker-compose down

# 重启某个服务
docker-compose restart api
docker-compose restart nginx
docker-compose restart mysql

# 停止某个服务
docker-compose stop api

# 启动某个服务
docker-compose start api
```

### 日志查看

```bash
# 查看所有服务日志
docker-compose logs -f

# 查看特定服务日志
docker-compose logs -f api
docker-compose logs -f nginx
docker-compose logs -f mysql

# 查看最近100行日志
docker-compose logs --tail=100 api
```

### 进入容器

```bash
# 进入API容器
docker exec -it hailong-api bash

# 进入MySQL容器
docker exec -it hailong-mysql bash

# 进入Nginx容器
docker exec -it hailong-nginx sh

# 在MySQL容器中执行SQL
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD"
```

### 数据管理

```bash
# 查看数据卷
docker volume ls

# 查看数据卷详情
docker volume inspect protral_mysql-data

# 备份MySQL数据
source .runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting > backup.sql

# 恢复MySQL数据
source .runtime/secrets.env && docker exec -i hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting < backup.sql
```

### 清理和重建

```bash
# 停止并删除所有容器
docker-compose down

# 停止并删除所有容器和数据卷（⚠️ 会删除数据库数据）
docker-compose down -v

# 重新构建并启动
docker-compose up -d --build

# 仅重新构建API
docker-compose build api
docker-compose up -d api
```

## 🔍 故障排查

### 问题1：容器无法启动

```bash
# 查看容器状态
docker-compose ps

# 查看详细日志
docker-compose logs api

# 常见原因：
# - 端口被占用
# - 配置文件错误
# - 依赖服务未就绪
```

### 问题2：数据库初始化失败

```bash
# 查看MySQL初始化日志
docker-compose logs mysql | grep -i "entrypoint"

# 常见错误：
# - SQL语法错误
# - 文件权限问题
# - 字符集问题
```

**解决方案**：
```bash
# 1. 停止并删除所有容器和数据卷
docker-compose down -v

# 2. 检查SQL文件是否存在且可读
ls -lh SQL/
cat SQL/hailong_consulting_schema.sql | head -20

# 3. 重新启动（会重新初始化数据库）
docker-compose up -d

# 4. 实时查看初始化过程
docker-compose logs -f mysql
```

### 问题3：API无法连接数据库

```bash
# 检查MySQL容器是否运行
docker-compose ps mysql

# 查看MySQL日志
docker-compose logs mysql

# 检查数据库是否初始化完成
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" -e "SHOW DATABASES;"

# 进入API容器测试连接
docker exec -it hailong-api bash
# 在容器内安装mysql客户端并测试
apt-get update && apt-get install -y default-mysql-client
mysql -h mysql -u hailong_app -p"$MYSQL_PASSWORD" hailong_consulting
```

**解决方案**：
- 确保MySQL容器健康检查通过：`docker-compose ps`（状态应为healthy）
- 检查数据库连接字符串配置
- 等待MySQL完全启动后重启API：`docker-compose restart api`
- 查看API日志：`docker-compose logs api`

### 问题4：前端无法访问

```bash
# 检查Nginx配置
docker exec hailong-nginx nginx -t

# 查看Nginx日志
docker-compose logs nginx

# 检查前端文件是否存在
docker exec hailong-nginx ls -la /usr/share/nginx/html/admin
docker exec hailong-nginx ls -la /usr/share/nginx/html/portal
```

**解决方案**：
- 确保前端已正确构建（dist目录存在）
- 检查nginx配置文件路径
- 重启Nginx容器：`docker-compose restart nginx`

### 问题5：端口被占用

```bash
# 查看端口占用
netstat -tlnp | grep :80
netstat -tlnp | grep :8080
netstat -tlnp | grep :5001

# 修改docker-compose.yml中的端口映射
# 例如将80改为8000：
# ports:
#   - "8000:80"
```

### 问题6：API构建失败

```bash
# 查看构建日志
docker-compose build api

# 常见原因：
# - .NET SDK版本不匹配
# - 项目文件路径错误
# - 网络问题导致NuGet包下载失败
```

**解决方案**：
- 检查Dockerfile中的.NET版本
- 确保项目文件结构正确
- 使用国内NuGet镜像源

## 📊 性能监控

```bash
# 查看容器资源使用
docker stats

# 查看特定容器资源使用
docker stats hailong-api hailong-mysql hailong-nginx

# 查看容器详细信息
docker inspect hailong-api
```

## 🔄 更新部署

### 更新后端API

```bash
cd /opt/hailong/project

# 拉取最新代码
git pull

# 重新构建并启动API
docker-compose build api
docker-compose up -d api

# 查看日志确认启动成功
docker-compose logs -f api
```

### 更新前端

```bash
# 重新构建前端
cd /opt/hailong/project/hailong-admin
npm run build

cd /opt/hailong/project/hailong-protral
npm run build

# 重启Nginx容器以加载新文件
docker-compose restart nginx
```

### 更新数据库

```bash
# 备份现有数据
source .runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting > backup_$(date +%Y%m%d).sql

# 执行新的SQL脚本
source .runtime/secrets.env && docker exec -i hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting < update.sql

# 重新初始化数据库（⚠️ 会删除所有数据）
docker-compose down -v  # 删除数据卷
docker-compose up -d    # 重新启动，自动执行初始化脚本
```

**注意**：
- ⚠️ 数据库初始化脚本只在**首次启动**时执行
- 如果需要重新初始化，必须先删除数据卷：`docker-compose down -v`
- 删除数据卷会**永久删除**所有数据库数据，请务必先备份

## 🔒 安全建议

### 1. 轮换运行时密钥

```bash
# 停止服务后，受控地更新 .runtime/secrets.env 中的 MySQL/JWT 密钥；
# MySQL 密码轮换还必须在数据库内执行 ALTER USER，随后再启动服务。
# 不要把密钥写回 docker-compose.yml、appsettings.json 或文档。
```

### 2. 配置防火墙

```bash
# 开放必要端口
firewall-cmd --permanent --add-port=80/tcp
firewall-cmd --permanent --add-port=8080/tcp
firewall-cmd --permanent --add-port=5001/tcp
firewall-cmd --reload

# 限制MySQL端口仅本地访问（推荐）
# 在docker-compose.yml中修改MySQL端口映射为：
# ports:
#   - "127.0.0.1:3306:3306"
```

### 3. 定期备份

```bash
# 创建备份脚本
cat > /root/backup-docker.sh <<'EOF'
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR=/backup

# 创建备份目录
mkdir -p $BACKUP_DIR

# 备份MySQL数据
source /opt/hailong/project/.runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting | gzip > $BACKUP_DIR/mysql_${DATE}.sql.gz

# 备份上传文件
docker cp hailong-api:/app/wwwroot/uploads $BACKUP_DIR/uploads_${DATE}
tar -czf $BACKUP_DIR/uploads_${DATE}.tar.gz -C $BACKUP_DIR uploads_${DATE}
rm -rf $BACKUP_DIR/uploads_${DATE}

# 删除7天前的备份
find $BACKUP_DIR -name "*.gz" -mtime +7 -delete

echo "Backup completed: $DATE"
EOF

chmod +x /root/backup-docker.sh

# 添加定时任务（每天凌晨2点执行）
crontab -e
# 添加：0 2 * * * /root/backup-docker.sh >> /var/log/backup.log 2>&1
```

## 🔐 HTTPS部署配置

### 当前部署方式说明

**当前配置为HTTP部署**，适合以下场景：
- ✅ 内网环境部署
- ✅ 开发测试环境
- ✅ 通过反向代理（如云服务商负载均衡器、CDN）提供HTTPS

**可以直接使用HTTP部署！** 无需配置HTTPS即可正常使用。

### HTTPS部署方案

如果需要直接提供HTTPS服务，有以下三种方案：

---

### 方案一：Let's Encrypt免费证书（推荐生产环境）

**适用场景**：有域名的生产环境

#### 步骤1：安装Certbot

```bash
# 安装Certbot
yum install -y certbot

# 或使用Docker方式
docker run -it --rm --name certbot \
  -v "/etc/letsencrypt:/etc/letsencrypt" \
  -v "/var/lib/letsencrypt:/var/lib/letsencrypt" \
  certbot/certbot certonly --standalone -d yourdomain.com
```

#### 步骤2：获取SSL证书

```bash
# 停止Nginx容器（certbot需要使用80端口）
docker-compose stop nginx

# 获取证书（替换yourdomain.com为你的域名）
certbot certonly --standalone \
  -d yourdomain.com \
  -d www.yourdomain.com \
  --email your@email.com \
  --agree-tos

# 证书将保存在：
# /etc/letsencrypt/live/yourdomain.com/fullchain.pem
# /etc/letsencrypt/live/yourdomain.com/privkey.pem
```

#### 步骤3：修改docker-compose.yml

```yaml
nginx:
  image: nginx:alpine
  container_name: hailong-nginx
  restart: always
  ports:
    - "80:80"
    - "443:443"      # 添加HTTPS端口
    - "8443:8443"    # 后台管理HTTPS
    - "5001:5001"
  volumes:
    - ./nginx/nginx.conf:/etc/nginx/nginx.conf
    - ./nginx/conf.d:/etc/nginx/conf.d
    - ./hailong-admin/dist:/usr/share/nginx/html/admin
    - ./hailong-protral/dist:/usr/share/nginx/html/portal
    - /etc/letsencrypt:/etc/letsencrypt:ro  # 挂载证书目录
  depends_on:
    - api
  networks:
    - hailong-network
```

#### 步骤4：创建HTTPS配置文件

创建 `nginx/conf.d/https.conf`：

```nginx
# 前端门户 - HTTPS
server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    # SSL证书配置
    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    
    # SSL优化配置
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;

    root /usr/share/nginx/html/portal;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # 安全头
    add_header Strict-Transport-Security "max-age=31536000" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
}

# 后台管理 - HTTPS
server {
    listen 8443 ssl http2;
    server_name yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    root /usr/share/nginx/html/admin;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}

# HTTP自动跳转HTTPS
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

#### 步骤5：重启服务

```bash
# 重启Nginx容器
docker-compose up -d nginx

# 查看日志
docker-compose logs -f nginx
```

#### 步骤6：配置证书自动续期

```bash
# 创建续期脚本
cat > /root/renew-cert.sh <<'EOF'
#!/bin/bash
docker-compose -f /opt/hailong/project/docker-compose.yml stop nginx
certbot renew --quiet
docker-compose -f /opt/hailong/project/docker-compose.yml start nginx
echo "Certificate renewed: $(date)"
EOF

chmod +x /root/renew-cert.sh

# 添加定时任务（每月1号凌晨3点）
crontab -e
# 添加：0 3 1 * * /root/renew-cert.sh >> /var/log/cert-renew.log 2>&1
```

---

### 方案二：自签名证书（仅测试用）

**适用场景**：内网测试环境，无域名

#### 步骤1：生成自签名证书

```bash
# 创建证书目录
mkdir -p /opt/hailong/ssl

# 生成自签名证书（有效期365天）
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout /opt/hailong/ssl/nginx-selfsigned.key \
  -out /opt/hailong/ssl/nginx-selfsigned.crt \
  -subj "/C=CN/ST=Henan/L=Zhengzhou/O=Hailong/CN=localhost"
```

#### 步骤2：修改docker-compose.yml

```yaml
nginx:
  volumes:
    - /opt/hailong/ssl:/etc/nginx/ssl:ro  # 添加这行
```

#### 步骤3：创建HTTPS配置

创建 `nginx/conf.d/https-self.conf`：

```nginx
server {
    listen 443 ssl http2;
    server_name _;

    ssl_certificate /etc/nginx/ssl/nginx-selfsigned.crt;
    ssl_certificate_key /etc/nginx/ssl/nginx-selfsigned.key;
    
    ssl_protocols TLSv1.2 TLSv1.3;

    root /usr/share/nginx/html/portal;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

**注意**：自签名证书会导致浏览器显示"不安全"警告，仅用于测试！

---

### 方案三：使用反向代理（最推荐）

**适用场景**：
- 使用云服务商负载均衡器（阿里云SLB、腾讯云CLB）
- 使用CDN服务（阿里云CDN、腾讯云CDN）
- 前置Nginx反向代理

**优点**：
- ✅ 证书管理更方便
- ✅ 可以使用云服务商提供的免费证书
- ✅ Docker容器保持HTTP，简化配置
- ✅ 支持负载均衡和高可用

**配置方式**：
1. 在云服务商控制台配置HTTPS证书
2. 配置负载均衡器转发规则：HTTPS(443) → HTTP(80/8080/5001)
3. Docker容器保持当前HTTP配置不变

**示例（阿里云SLB）**：
```
客户端 → HTTPS(443) → 阿里云SLB → HTTP(80) → Docker容器
```

---

### HTTP vs HTTPS 对比

| 特性 | HTTP部署 | HTTPS部署 |
|------|---------|-----------|
| **安全性** | ❌ 数据明文传输 | ✅ 数据加密传输 |
| **配置难度** | ✅ 简单 | ⭐⭐⭐ 复杂 |
| **证书管理** | 无需证书 | 需要管理证书 |
| **性能** | ✅ 稍快 | ⭐⭐⭐ 稍慢 |
| **浏览器信任** | ⚠️ 显示"不安全" | ✅ 显示安全锁 |
| **SEO** | ❌ 不利 | ✅ 有利 |
| **适用场景** | 内网/测试 | 生产环境 |

### 推荐方案总结

1. **内网部署** → 直接使用HTTP（当前配置）✅
2. **有域名的生产环境** → Let's Encrypt免费证书
3. **使用云服务** → 云负载均衡器 + 证书（最推荐）
4. **测试HTTPS** → 自签名证书

### 防火墙配置

如果启用HTTPS，需要开放相应端口：

```bash
# 开放HTTPS端口
firewall-cmd --permanent --add-port=443/tcp
firewall-cmd --permanent --add-port=8443/tcp
firewall-cmd --reload

# 或者开放HTTPS服务
firewall-cmd --permanent --add-service=https
firewall-cmd --reload
```

## ⚖️ Docker vs 传统部署对比

| 特性 | Docker部署 | 传统部署 |
|------|-----------|---------|
| **部署难度** | ⭐⭐⭐⭐ | ⭐⭐ |
| **学习成本** | 高 | 低 |
| **环境隔离** | ✅ 完全隔离 | ❌ 共享系统 |
| **资源占用** | 稍高 | 较低 |
| **迁移便利** | ✅ 非常方便 | ❌ 需重新配置 |
| **版本管理** | ✅ 容易 | ❌ 困难 |
| **调试难度** | ⭐⭐⭐ | ⭐⭐ |
| **适合新手** | ❌ 不适合 | ✅ 适合 |

### Docker的优点
- ✅ 环境隔离，不污染系统
- ✅ 易于迁移和扩展
- ✅ 版本管理方便
- ✅ 一键启动所有服务

### Docker的缺点
- ❌ 学习曲线陡峭
- ❌ 需要额外的资源开销
- ❌ 调试相对复杂
- ❌ 需要理解容器概念

### 传统部署的优点
- ✅ 简单直观，易于理解
- ✅ 性能开销小
- ✅ 调试方便
- ✅ 直接访问文件系统

### 传统部署的缺点
- ❌ 环境配置复杂
- ❌ 可能影响系统环境
- ❌ 迁移困难

## 💡 部署方案选择建议

### 推荐使用Docker的场景

✅ **适合Docker部署**：
- 熟悉Docker和容器技术
- 需要快速迁移到其他服务器
- 多环境部署（开发、测试、生产）
- 需要环境隔离
- 团队协作开发

### 推荐使用传统部署的场景

✅ **适合传统部署**：
- 不熟悉Linux和Docker
- 只需要单一环境部署
- 追求最佳性能
- 需要频繁调试
- 快速上线，不想学习新技术

## 🎯 快速决策指南

**如果您：**
- ❓ 不熟悉Linux → 使用[传统部署](快速开始-CentOS7.md)
- ❓ 不了解Docker → 使用[传统部署](快速开始-CentOS7.md)
- ❓ 只想快速部署 → 使用[传统部署](快速开始-CentOS7.md)
- ✅ 熟悉Docker → 使用Docker部署
- ✅ 需要快速迁移 → 使用Docker部署
- ✅ 多环境部署 → 使用Docker部署
- ✅ 想学习Docker → 可以尝试Docker部署

---

## 📚 相关文档

- [快速开始-CentOS7.md](快速开始-CentOS7.md) - 传统部署方式（推荐新手）
- [CentOS7部署指南.md](CentOS7部署指南.md) - 详细的传统部署指南
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - 故障排查手册

---

**建议：先用传统方式部署成功，系统跑起来后，再慢慢学习Docker！** 🎉

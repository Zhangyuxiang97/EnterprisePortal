# 🚀 海隆咨询官网 - Ubuntu 22.04 Docker部署指南

> 完整的Docker容器化部署方案，适用于Ubuntu 22.04 LTS

## 📋 目录

1. [环境说明](#环境说明)
2. [部署前准备](#部署前准备)
3. [快速部署](#快速部署)
4. [验证部署](#验证部署)
5. [访问系统](#访问系统)
6. [Docker管理](#docker管理)
7. [更新部署](#更新部署)
8. [常见问题](#常见问题)
9. [安全建议](#安全建议)

---

## 环境说明

### 系统要求

- **操作系统**: Ubuntu 22.04 LTS (Jammy Jellyfish)
- **内存**: 至少 4GB（推荐 8GB）
- **硬盘**: 至少 20GB 可用空间
- **CPU**: 2核心以上
- **网络**: 可访问互联网

### 支持的环境

- ✅ 物理服务器
- ✅ 云服务器（阿里云、腾讯云、AWS等）
- ✅ VMware虚拟机
- ✅ VirtualBox虚拟机
- ✅ WSL2 (Windows Subsystem for Linux)

### Docker部署优势

- ✅ **环境隔离** - 不污染系统环境
- ✅ **快速部署** - 一键启动所有服务
- ✅ **易于迁移** - 可快速迁移到其他服务器
- ✅ **版本管理** - 便于回滚和升级
- ✅ **统一管理** - 所有服务集中管理

### 部署架构

```
┌─────────────────────────────────────────┐
│         Nginx容器 (hailong-nginx)        │
│  ┌──────────┐  ┌──────────┐  ┌────────┐ │
│  │前端门户  │  │后台管理  │  │API代理 │ │
│  │  :80     │  │  :8080   │  │ :5001  │ │
│  └──────────┘  └──────────┘  └────────┘ │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│      .NET API容器 (hailong-api)         │
│         ASP.NET Core 8.0 :5000          │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│      MySQL容器 (hailong-mysql)          │
│           MySQL 8.0 :3306               │
└─────────────────────────────────────────┘
```

---

## 部署前准备

### 第1步：获取服务器IP地址

**目的**：获取服务器IP地址，用于本机SSH连接和后续浏览器访问。

在Ubuntu服务器上执行：

```bash
ip addr show
```

记录显示的IP地址，例如：`192.168.222.100`

**使用SSH连接服务器**：

在本机电脑（Windows/Mac/Linux）上使用SSH连接：

```bash
# 使用用户名和密码连接
ssh sean@192.168.222.100

# 或使用密钥连接
ssh -i ~/.ssh/id_rsa sean@192.168.222.100
```

**Windows用户可以使用**：
- PowerShell的ssh命令
- PuTTY工具
- Windows Terminal
- MobaXterm

### 第2步：获取项目文件

使用Git克隆项目到服务器：

```bash
# 安装Git
sudo apt update
sudo apt install -y git

# 创建项目目录
sudo mkdir -p /opt/hailong
cd /opt/hailong

# 克隆项目
sudo git clone https://github.com/BobVaughan6/EnterprisePortal.git project

# 进入项目目录
cd project
```

**更新项目代码**：

如果项目已存在，可以使用以下命令更新：

```bash
cd /opt/hailong/project
sudo git pull
```

### 第3步：检查项目结构

确保项目包含以下关键文件和目录：

```bash
ls -la
```

应该看到：
- ✅ [`deploy-ubuntu22-docker.sh`](deploy-ubuntu22-docker.sh) - 部署脚本
- ✅ [`docker-compose.yml`](docker-compose.yml) - Docker编排配置
- ✅ `BackEnd/` - 后端API目录
- ✅ `hailong-admin/` - 后台管理前端
- ✅ `hailong-protral/` - 前端门户
- ✅ `nginx/` - Nginx配置
- ✅ `SQL/` - 数据库初始化脚本

---

## 快速部署

### 一键部署流程

#### 第1步：赋予执行权限

```bash
cd /opt/hailong/project
sudo chmod +x deploy-ubuntu22-docker.sh
```

#### 第2步：运行部署脚本

```bash
sudo ./deploy-ubuntu22-docker.sh
```

#### 第3步：按提示输入配置

脚本只询问项目路径；首次运行会自动生成并打印一次随机密钥，随后复用 `.runtime/secrets.env`：

```
项目文件路径 (默认: /opt/hailong/project): [回车使用默认]
确认开始部署? (y/n): y
```

**推荐配置**：
- 首次部署会生成随机密钥并保存到权限为 600 的 `.runtime/secrets.env`
- 后续重跑脚本会复用该文件，避免数据库与应用配置失配

#### 第4步：等待部署完成

部署过程约需 **20-40分钟**，脚本会自动完成：

1. ✅ **安装Docker** - 安装最新版Docker Engine和Docker Compose
2. ✅ **安装Node.js** - 安装Node.js 18用于构建前端
3. ✅ **构建前端** - 构建后台管理系统和前端门户
4. ✅ **生成配置** - 生成docker-compose.yml配置文件
5. ✅ **构建镜像** - 构建.NET API Docker镜像
6. ✅ **启动容器** - 启动MySQL、API、Nginx容器
7. ✅ **初始化数据库** - 自动执行SQL初始化脚本
8. ✅ **配置防火墙** - 开放必要端口

**部署过程输出示例**：

```
========================================
第一步：检查项目文件
========================================
[INFO] 检查必需文件...
[INFO] 所有必需文件检查通过

========================================
第二步：安装Docker
========================================
[INFO] 安装最新版Docker...
[INFO] Docker安装成功: Docker version 24.0.7

========================================
第三步：检测Docker Compose
========================================
[INFO] 使用Docker Compose插件: Docker Compose version v2.24.0

========================================
第四步：安装Node.js
========================================
[INFO] 安装Node.js 18...
[INFO] Node.js安装成功: v18.19.0

========================================
第五步：更新docker-compose.yml配置
========================================
[INFO] docker-compose.yml配置完成

========================================
第六步：构建前端
========================================
[INFO] 构建后台管理系统...
[INFO] 后台管理系统构建成功
[INFO] 构建前端门户...
[INFO] 前端门户构建成功

========================================
第七步：启动Docker容器
========================================
[INFO] 启动所有服务...
[INFO] 等待服务启动...

========================================
第八步：验证部署
========================================
[INFO] MySQL已就绪
[INFO] API服务已就绪
[INFO] 数据库初始化成功，共 15 张表

========================================
第九步：配置防火墙
========================================
[INFO] 防火墙配置完成

========================================
部署完成！
========================================
```

---

## 验证部署

### 检查容器状态

```bash
cd /opt/hailong/project
docker compose ps
```

应该看到3个容器都在运行：

```
NAME              IMAGE                                    STATUS
hailong-mysql     mysql:8.0                               Up (healthy)
hailong-api       project-api                             Up (healthy)
hailong-nginx     nginx:alpine                            Up (healthy)
```

### 检查服务健康状态

```bash
# 检查MySQL
source .runtime/secrets.env && docker exec hailong-mysql mysqladmin ping -h localhost -p"$MYSQL_ROOT_PASSWORD"

# 检查API
curl http://localhost:5001/api/home/statistics

# 检查Nginx
curl http://localhost
```

### 查看容器日志

```bash
# 查看所有容器日志
docker compose logs

# 查看特定容器日志
docker compose logs mysql
docker compose logs api
docker compose logs nginx

# 实时查看日志
docker compose logs -f
```

### 验证数据库

```bash
# 进入MySQL容器
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD"

# 在MySQL中执行
USE hailong_consulting;
SHOW TABLES;
SELECT COUNT(*) FROM Users;
EXIT;
```

---

## 访问系统

### 浏览器访问

假设服务器IP是 `192.168.222.100`：

#### 1. 前端门户

```
http://192.168.222.100
```

用户可以浏览公司信息、新闻动态等公开内容。

#### 2. 后台管理系统

```
http://192.168.222.100:8080
```

**初始管理员信息：** 首次启动时由应用随机生成。请查看
`/var/www/hailong-api/logs/bootstrap/initial-admin-credentials.txt`，首次登录后立即修改密码并删除该文件。

**⚠️ 重要：首次登录后请立即修改密码！**

#### 3. API接口

```
http://192.168.222.100:5001/api/home/statistics
```

可以直接访问API接口进行测试。

### 端口说明

| 端口 | 服务 | 说明 |
|------|------|------|
| **80** | 前端门户 | 公开访问的企业门户网站 |
| **8080** | 后台管理 | 管理员后台系统 |
| **5001** | API接口 | RESTful API服务 |
| **3306** | MySQL | 数据库（仅容器内部访问） |
| **5000** | .NET API | API原始端口（仅容器内部访问） |

---

## Docker管理

### 常用命令

```bash
# 进入项目目录
cd /opt/hailong/project

# 查看容器状态
docker compose ps

# 查看所有日志
docker compose logs

# 实时查看日志
docker compose logs -f

# 查看特定容器日志
docker compose logs api
docker compose logs mysql
docker compose logs nginx

# 重启所有服务
docker compose restart

# 重启特定服务
docker compose restart api
docker compose restart nginx

# 停止所有服务
docker compose stop

# 启动所有服务
docker compose start

# 停止并删除容器（数据保留）
docker compose down

# 启动服务
docker compose up -d

# 重新构建并启动
docker compose up -d --build
```

### 进入容器

```bash
# 进入API容器
docker exec -it hailong-api bash

# 进入MySQL容器
docker exec -it hailong-mysql bash

# 进入Nginx容器
docker exec -it hailong-nginx sh
```

### 查看资源使用

```bash
# 查看容器资源使用情况
docker stats

# 查看磁盘使用
docker system df

# 查看卷信息
docker volume ls
```

---

## 更新部署

### 方式一：完整更新（推荐）

适用于前后端代码都有更新的情况：

```bash
# 1. 进入项目目录
cd /opt/hailong/project

# 2. 备份数据库（重要！）
source .runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting > backup_$(date +%Y%m%d_%H%M%S).sql

# 3. 拉取最新代码
sudo git reset --hard HEAD
sudo git pull

# 4. 停止并删除所有容器
docker compose down

# 5. 重新构建前端
cd hailong-admin
npm install
npm run build
cd ..

cd hailong-protral
npm install
npm run build
cd ..

# 6. 重新构建并启动所有容器
docker compose build --no-cache
docker compose up -d

# 7. 查看容器状态
docker compose ps

# 8. 查看日志确认启动成功
docker compose logs -f
```

### 方式二：仅更新前端

适用于只修改了前端代码：

```bash
# 1. 进入项目目录
cd /opt/hailong/project

# 2. 拉取最新代码
sudo git pull

# 3. 重新构建前端
cd hailong-admin
npm install
npm run build
cd ..

cd hailong-protral
npm install
npm run build
cd ..

# 4. 重启Nginx容器
docker compose restart nginx

# 5. 查看日志
docker compose logs -f nginx
```

### 方式三：仅更新后端

适用于只修改了后端代码：

```bash
# 1. 进入项目目录
cd /opt/hailong/project

# 2. 拉取最新代码
sudo git pull

# 3. 停止后端容器
docker compose stop api

# 4. 重新构建后端容器
docker compose build --no-cache api

# 5. 启动后端容器
docker compose up -d api

# 6. 查看日志
docker compose logs -f api
```

### 方式四：仅更新配置

适用于只修改了docker-compose.yml或nginx配置：

```bash
# 1. 进入项目目录
cd /opt/hailong/project

# 2. 拉取最新代码或手动修改配置
sudo git pull
# 或手动编辑
sudo nano docker-compose.yml

# 3. 重新加载配置并重启
docker compose down
docker compose up -d

# 4. 查看容器状态
docker compose ps
```

### 更新注意事项

⚠️ **重要提示**：

1. **数据备份**：更新前务必备份数据库
   ```bash
   source .runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting > backup_$(date +%Y%m%d_%H%M%S).sql
   ```

2. **查看变更**：更新前查看代码变更
   ```bash
   git fetch
   git log HEAD..origin/main --oneline
   git diff HEAD..origin/main
   ```

3. **测试环境**：重要更新建议先在测试环境验证

4. **回滚准备**：记录当前版本号，以便回滚
   ```bash
   git log -1 --oneline
   ```

### 回滚操作

如更新后出现问题，可回滚到之前版本：

```bash
# 1. 查看提交历史
git log --oneline

# 2. 回滚到指定版本
git reset --hard <commit-id>

# 3. 恢复数据库（如果需要）
source .runtime/secrets.env && docker exec -i hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting < backup_20260122_120000.sql

# 4. 重新部署
docker compose down
docker compose build --no-cache
docker compose up -d
```

---

## 常见问题

### 问题1：无法访问网站

**症状**：浏览器无法打开网站

**解决方案**：

```bash
# 1. 检查容器状态
docker compose ps

# 2. 检查防火墙
sudo ufw status

# 3. 开放端口
sudo ufw allow 80/tcp
sudo ufw allow 8080/tcp
sudo ufw allow 5001/tcp
sudo ufw reload

# 4. 检查Nginx日志
docker compose logs nginx

# 5. 测试本地访问
curl http://localhost
```

### 问题2：无法拉取Docker镜像

**症状**：提示 `failed to resolve reference` 或 `connection refused`

**原因**：无法连接到Docker Hub（网络问题）

**解决方案**：

```bash
# 配置Docker镜像加速器
sudo mkdir -p /etc/docker
sudo tee /etc/docker/daemon.json <<EOF
{
  "registry-mirrors": [
    "https://dockerproxy.com",
    "https://docker.nju.edu.cn",
    "https://docker.m.daocloud.io"
  ],
  "dns": ["8.8.8.8", "114.114.114.114"]
}
EOF

# 重启Docker
sudo systemctl daemon-reload
sudo systemctl restart docker

# 验证配置
docker info | grep -A 5 "Registry Mirrors"

# 测试网络连接
ping -c 3 dockerproxy.com

# 重新拉取镜像
cd /opt/hailong/project
docker compose pull
docker compose up -d
```

**备选方案：手动下载镜像**

如果镜像加速器仍然无法使用，可以手动下载镜像：

```bash
# 方案1：使用阿里云镜像
docker pull registry.cn-hangzhou.aliyuncs.com/library/mysql:8.0
docker tag registry.cn-hangzhou.aliyuncs.com/library/mysql:8.0 mysql:8.0

docker pull registry.cn-hangzhou.aliyuncs.com/library/nginx:alpine
docker tag registry.cn-hangzhou.aliyuncs.com/library/nginx:alpine nginx:alpine

# 方案2：从其他服务器传输
# 在有网络的服务器上
docker save mysql:8.0 nginx:alpine -o images.tar
scp images.tar root@目标服务器IP:/tmp/

# 在目标服务器上
docker load -i /tmp/images.tar
```

### 问题3：容器无法启动

**症状**：docker compose ps显示容器状态异常

**解决方案**：

```bash
# 1. 查看容器日志
docker compose logs 容器名

# 2. 检查Docker服务
sudo systemctl status docker

# 3. 重启Docker服务
sudo systemctl restart docker

# 4. 清理并重新构建
docker compose down
docker system prune -a
docker compose build --no-cache
docker compose up -d
```

### 问题4：MySQL初始化失败

**症状**：数据库表未创建或数据为空

**解决方案**：

```bash
# 1. 检查SQL文件
ls -la SQL/

# 2. 查看MySQL日志
docker compose logs mysql

# 3. 手动初始化数据库
source .runtime/secrets.env && docker exec -i hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting < SQL/init.sql

# 4. 验证表结构
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" -e "USE hailong_consulting; SHOW TABLES;"
```

### 问题5：API无法连接数据库

**症状**：API日志显示数据库连接错误

**解决方案**：

```bash
# 1. 检查MySQL容器健康状态
docker compose ps

# 2. 测试数据库连接
source .runtime/secrets.env && docker exec hailong-mysql mysqladmin ping -h localhost -p"$MYSQL_ROOT_PASSWORD"

# 3. 检查网络连接
docker network ls
docker network inspect hailong-network

# 4. 重启API容器
docker compose restart api

# 5. 查看API日志
docker compose logs -f api
```

### 问题6：前端构建失败

**症状**：npm run build报错

**解决方案**：

```bash
# 1. 使用国内镜像
npm config set registry https://registry.npmmirror.com

# 2. 清除缓存
npm cache clean --force

# 3. 删除node_modules重新安装
rm -rf node_modules package-lock.json
npm install

# 4. 重新构建
npm run build

# 5. 检查Node.js版本
node --version  # 应该是v18.x
```

### 问题7：端口被占用

**症状**：容器启动失败，提示端口已被占用

**解决方案**：

```bash
# 1. 查看端口占用
sudo netstat -tlnp | grep :80
sudo netstat -tlnp | grep :8080
sudo netstat -tlnp | grep :5001

# 2. 停止占用端口的进程
sudo kill -9 <PID>

# 3. 或修改docker-compose.yml中的端口映射
sudo nano docker-compose.yml
# 修改 "80:80" 为 "8000:80" 等

# 4. 重新启动
docker compose up -d
```

### 问题8：磁盘空间不足

**症状**：构建或运行时提示磁盘空间不足

**解决方案**：

```bash
# 1. 查看磁盘使用
df -h
docker system df

# 2. 清理未使用的镜像
docker image prune -a

# 3. 清理未使用的容器
docker container prune

# 4. 清理未使用的卷（谨慎使用）
docker volume prune

# 5. 清理构建缓存
docker builder prune

# 6. 一键清理所有未使用资源
docker system prune -a --volumes
```

### 问题9：Docker权限被拒绝

**症状**：`permission denied while trying to connect to the Docker daemon socket`

**原因**：当前用户没有权限访问Docker socket

**解决方案**：

**方案一：使用sudo（推荐）**

```bash
# 所有docker命令前加sudo
sudo docker compose ps
sudo docker compose logs -f
sudo docker compose restart
```

**方案二：将用户添加到docker组**

```bash
# 1. 将当前用户添加到docker组
sudo usermod -aG docker $USER

# 2. 刷新组权限（选择其一）
# 方法1：重新登录SSH
exit
# 然后重新SSH连接

# 方法2：使用newgrp
newgrp docker

# 方法3：重启系统
sudo reboot

# 3. 验证权限
docker ps
docker compose ps
```

**方案三：临时使用root用户**

```bash
# 切换到root用户
sudo su -

# 进入项目目录
cd /opt/hailong/project

# 执行docker命令
docker compose ps
```

**注意事项**：
- 部署脚本必须使用 `sudo` 运行
- 如果使用非root用户，建议添加到docker组
- 添加到docker组后需要重新登录才能生效

### 问题10：上传图片404无法访问

**症状**：访问 `http://服务器IP/uploads/...` 返回404错误

**原因**：Nginx配置中前端门户和后台管理没有配置 `/uploads/` 路径

**解决方案**：

```bash
# 1. 检查uploads目录是否存在
sudo docker exec hailong-nginx ls -la /usr/share/nginx/html/uploads/

# 2. 检查Nginx配置
sudo docker exec hailong-nginx cat /etc/nginx/conf.d/default.conf | grep -A 5 "location /uploads"

# 3. 如果配置缺失，更新nginx配置文件
cd /opt/hailong/project
sudo nano nginx/conf.d/default.conf

# 在前端门户(80端口)和后台管理(8080端口)的server块中添加：
# location /uploads/ {
#     alias /usr/share/nginx/html/uploads/;
#     expires 30d;
#     add_header Cache-Control "public, immutable";
#     add_header Access-Control-Allow-Origin *;
# }

# 4. 重启Nginx容器
sudo docker compose restart nginx

# 5. 验证配置
curl -I http://localhost/uploads/test.jpg
```

**完整的Nginx配置示例**：

```nginx
# 前端门户
server {
    listen 80;
    server_name _;
    root /usr/share/nginx/html/portal;
    index index.html;

    # 上传文件访问（必须添加）
    location /uploads/ {
        alias /usr/share/nginx/html/uploads/;
        expires 30d;
        add_header Cache-Control "public, immutable";
        add_header Access-Control-Allow-Origin *;
    }

    # SPA路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }
}

# 后台管理系统
server {
    listen 8080;
    server_name _;
    root /usr/share/nginx/html/admin;
    index index.html;

    # 上传文件访问（必须添加）
    location /uploads/ {
        alias /usr/share/nginx/html/uploads/;
        expires 30d;
        add_header Cache-Control "public, immutable";
        add_header Access-Control-Allow-Origin *;
    }

    # SPA路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

### 问题11：文件权限问题

**症状**：文件访问被拒绝或权限不足

**解决方案**：

```bash
# 1. 确保使用sudo或root用户
sudo ./deploy-ubuntu22-docker.sh

# 2. 检查文件权限
ls -la /opt/hailong/project

# 3. 修复权限
sudo chown -R root:root /opt/hailong/project
sudo chmod -R 755 /opt/hailong/project
```

---

## 安全建议

### 1. 修改默认密码

**修改管理员密码**：
- 登录后台管理系统 http://服务器IP:8080
- 进入用户管理，修改admin密码

**修改MySQL密码**：
```bash
# 进入MySQL容器
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD"

# 修改root密码
ALTER USER 'root'@'%' IDENTIFIED BY '新密码';
FLUSH PRIVILEGES;
EXIT;

# 更新docker-compose.yml中的密码
sudo nano docker-compose.yml
# 修改MYSQL_ROOT_PASSWORD和ConnectionStrings中的密码

# 重启容器
docker compose down
docker compose up -d
```

### 2. 配置防火墙

```bash
# 启用防火墙
sudo ufw enable

# 允许SSH（重要！避免被锁定）
sudo ufw allow 22/tcp

# 允许Web服务
sudo ufw allow 80/tcp
sudo ufw allow 8080/tcp
sudo ufw allow 5001/tcp

# 查看防火墙状态
sudo ufw status

# 如果是生产环境，建议限制8080和5001端口仅内网访问
sudo ufw delete allow 8080/tcp
sudo ufw allow from 192.168.1.0/24 to any port 8080
```

### 3. 定期备份

**自动备份脚本**：

```bash
# 创建备份脚本
sudo nano /opt/hailong/backup.sh
```

添加以下内容：

```bash
#!/bin/bash
BACKUP_DIR="/opt/hailong/backups"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR

# 备份数据库
source /opt/hailong/project/.runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting > $BACKUP_DIR/db_$DATE.sql

# 备份上传文件
docker cp hailong-api:/app/wwwroot/uploads $BACKUP_DIR/uploads_$DATE

# 压缩备份
tar -czf $BACKUP_DIR/backup_$DATE.tar.gz $BACKUP_DIR/db_$DATE.sql $BACKUP_DIR/uploads_$DATE
rm -rf $BACKUP_DIR/db_$DATE.sql $BACKUP_DIR/uploads_$DATE

# 删除30天前的备份
find $BACKUP_DIR -name "backup_*.tar.gz" -mtime +30 -delete

echo "备份完成: backup_$DATE.tar.gz"
```

设置定时任务：

```bash
# 赋予执行权限
sudo chmod +x /opt/hailong/backup.sh

# 添加定时任务（每天凌晨2点执行）
sudo crontab -e
# 添加以下行
0 2 * * * /opt/hailong/backup.sh >> /var/log/hailong-backup.log 2>&1
```

### 4. 更新系统

```bash
# 定期更新系统
sudo apt update
sudo apt upgrade -y

# 更新Docker镜像
cd /opt/hailong/project
docker compose pull
docker compose up -d
```

### 5. 监控日志

```bash
# 实时监控所有日志
docker compose logs -f

# 监控API日志
docker compose logs -f api

# 监控Nginx访问日志
docker exec hailong-nginx tail -f /var/log/nginx/access.log

# 监控MySQL慢查询
docker exec hailong-mysql tail -f /var/log/mysql/slow.log
```

### 6. 限制容器资源

编辑 [`docker-compose.yml`](docker-compose.yml)，为容器添加资源限制：

```yaml
services:
  api:
    # ... 其他配置
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 2G
        reservations:
          cpus: '1'
          memory: 1G
```

### 7. 使用HTTPS

生产环境建议配置SSL证书：

```bash
# 安装certbot
sudo apt install -y certbot python3-certbot-nginx

# 获取证书（需要域名）
sudo certbot --nginx -d yourdomain.com

# 证书会自动续期
sudo certbot renew --dry-run
```

---

## 🔧 常用运维命令

### 服务管理

```bash
# 查看所有容器
docker ps -a

# 查看容器详细信息
docker inspect hailong-api

# 查看容器资源使用
docker stats

# 导出容器日志
docker compose logs > logs_$(date +%Y%m%d).txt
```

### 数据库管理

```bash
# 备份数据库
source .runtime/secrets.env && docker exec hailong-mysql mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting > backup.sql

# 恢复数据库
source .runtime/secrets.env && docker exec -i hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" hailong_consulting < backup.sql

# 查看数据库大小
source .runtime/secrets.env && docker exec hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" -e "SELECT table_schema AS 'Database', ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) AS 'Size (MB)' FROM information_schema.tables WHERE table_schema='hailong_consulting';"
```

### 清理维护

```bash
# 清理日志
docker compose logs --tail=0 -f > /dev/null

# 清理未使用的资源
docker system prune -a

# 查看磁盘使用
docker system df -v

# 重建所有容器
docker compose down
docker compose up -d --force-recreate
```

---

## 🌐 域名绑定配置

如果您有域名并希望绑定到系统，请按以下步骤操作：

### 前提条件

- ✅ 已完成Docker部署
- ✅ 拥有已备案的域名（中国大陆服务器需要）
- ✅ 域名DNS已解析到服务器IP

### 第1步：DNS解析配置

在域名服务商（阿里云、腾讯云等）的DNS管理中添加解析记录：

| 记录类型 | 主机记录 | 记录值 | 说明 |
|---------|---------|--------|------|
| A | @ | 服务器IP | 主域名（example.com） |
| A | www | 服务器IP | www子域名（www.example.com） |
| A | admin | 服务器IP | 后台管理（admin.example.com） |
| A | api | 服务器IP | API接口（api.example.com） |

**示例**：
```
@ -> 192.168.222.100
www -> 192.168.222.100
admin -> 192.168.222.100
api -> 192.168.222.100
```

### 第2步：修改Nginx配置

编辑Nginx配置文件：

```bash
cd /opt/hailong/project
sudo nano nginx/conf.d/default.conf
```

#### 方案一：使用子域名（推荐）

修改配置文件中的 `server_name`：

```nginx
# API反向代理服务
server {
    listen 5001;
    server_name api.example.com;  # 修改为您的API域名
    # ... 其他配置保持不变
}

# 后台管理系统
server {
    listen 8080;
    server_name admin.example.com;  # 修改为您的后台域名
    # ... 其他配置保持不变
}

# 前端门户
server {
    listen 80;
    server_name example.com www.example.com;  # 修改为您的主域名
    # ... 其他配置保持不变
}
```

#### 方案二：使用端口访问

保持配置不变，通过端口访问：

```nginx
# 前端门户
server {
    listen 80;
    server_name example.com www.example.com;
    # ... 其他配置
}

# 后台管理
server {
    listen 8080;
    server_name example.com www.example.com;
    # ... 其他配置
}

# API接口
server {
    listen 5001;
    server_name example.com www.example.com;
    # ... 其他配置
}
```

### 第3步：配置HTTPS（推荐）

使用Let's Encrypt免费SSL证书：

#### 安装Certbot

```bash
sudo apt update
sudo apt install -y certbot python3-certbot-nginx
```

#### 获取SSL证书

**方案一：自动配置（推荐）**

```bash
# 为主域名申请证书
sudo certbot --nginx -d example.com -d www.example.com

# 为后台管理申请证书
sudo certbot --nginx -d admin.example.com

# 为API申请证书
sudo certbot --nginx -d api.example.com
```

**方案二：手动配置**

```bash
# 仅获取证书
sudo certbot certonly --nginx -d example.com -d www.example.com
```

然后手动修改Nginx配置：

```nginx
# 前端门户 - HTTPS
server {
    listen 443 ssl http2;
    server_name example.com www.example.com;

    ssl_certificate /etc/letsencrypt/live/example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/example.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    root /usr/share/nginx/html/portal;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    # 静态资源缓存
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}

# HTTP重定向到HTTPS
server {
    listen 80;
    server_name example.com www.example.com;
    return 301 https://$server_name$request_uri;
}
```

#### 自动续期

Certbot会自动添加续期任务，验证：

```bash
# 测试续期
sudo certbot renew --dry-run

# 查看定时任务
sudo systemctl status certbot.timer
```

### 第4步：更新前端API地址

修改前端环境变量，使用域名访问API：

```bash
cd /opt/hailong/project

# 更新后台管理系统
cd hailong-admin
cat > .env.production <<EOF
VITE_API_BASE_URL=https://api.example.com
EOF
npm run build

# 更新前端门户
cd ../hailong-protral
cat > .env.production <<EOF
VITE_API_BASE_URL=https://api.example.com
EOF
npm run build

cd ..
```

### 第5步：重启Nginx容器

```bash
cd /opt/hailong/project

# 重启Nginx使配置生效
docker compose restart nginx

# 查看日志确认
docker compose logs -f nginx
```

### 第6步：验证域名访问

在浏览器中访问：

- **前端门户**: https://example.com 或 https://www.example.com
- **后台管理**: https://admin.example.com:8080 或 https://example.com:8080
- **API接口**: https://api.example.com:5001 或 https://example.com:5001

### 域名配置示例

#### 示例1：完全使用子域名

```
前端门户: https://www.example.com
后台管理: https://admin.example.com
API接口: https://api.example.com
```

**Nginx配置**：
```nginx
# 前端门户
server {
    listen 443 ssl http2;
    server_name www.example.com;
    # SSL配置...
    root /usr/share/nginx/html/portal;
}

# 后台管理
server {
    listen 443 ssl http2;
    server_name admin.example.com;
    # SSL配置...
    root /usr/share/nginx/html/admin;
}

# API接口
server {
    listen 443 ssl http2;
    server_name api.example.com;
    # SSL配置...
    location / {
        proxy_pass http://api:5000;
    }
}
```

#### 示例2：使用端口区分

```
前端门户: https://example.com
后台管理: https://example.com:8080
API接口: https://example.com:5001
```

**Nginx配置**：
```nginx
# 前端门户
server {
    listen 443 ssl http2;
    server_name example.com;
    # SSL配置...
    root /usr/share/nginx/html/portal;
}

# 后台管理
server {
    listen 8080 ssl http2;
    server_name example.com;
    # SSL配置...
    root /usr/share/nginx/html/admin;
}

# API接口
server {
    listen 5001 ssl http2;
    server_name example.com;
    # SSL配置...
    location / {
        proxy_pass http://api:5000;
    }
}
```

### 常见问题

#### 问题1：域名无法访问

```bash
# 检查DNS解析
nslookup example.com
ping example.com

# 检查防火墙
sudo ufw status
sudo ufw allow 443/tcp

# 检查Nginx配置
docker exec hailong-nginx nginx -t

# 查看Nginx日志
docker compose logs nginx
```

#### 问题2：SSL证书申请失败

```bash
# 确保80端口可访问（Let's Encrypt验证需要）
sudo ufw allow 80/tcp

# 检查域名解析是否生效
nslookup example.com

# 查看详细错误
sudo certbot --nginx -d example.com --dry-run
```

#### 问题3：证书续期失败

```bash
# 手动续期
sudo certbot renew

# 查看续期日志
sudo journalctl -u certbot.timer

# 重启Nginx
docker compose restart nginx
```

### 安全建议

1. **强制HTTPS**：将所有HTTP请求重定向到HTTPS
2. **HSTS配置**：添加HTTP严格传输安全头
   ```nginx
   add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
   ```
3. **隐藏版本信息**：
   ```nginx
   server_tokens off;
   ```
4. **限制请求速率**：
   ```nginx
   limit_req_zone $binary_remote_addr zone=mylimit:10m rate=10r/s;
   limit_req zone=mylimit burst=20;
   ```

---

## 📚 相关文档

- [`deploy-ubuntu22-docker.sh`](deploy-ubuntu22-docker.sh) - Ubuntu Docker部署脚本
- [`docker-compose.yml`](docker-compose.yml) - Docker编排配置文件
- [`Docker快速部署指南.md`](Docker快速部署指南.md) - Docker快速入门
- [`DEPLOYMENT.md`](DEPLOYMENT.md) - 详细部署文档
- [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) - 故障排查指南
- [`MAINTENANCE.md`](MAINTENANCE.md) - 运维维护手册

---

## 🎯 部署检查清单

部署完成后，请检查以下项目：

- [ ] 所有容器都在运行（docker compose ps）
- [ ] 数据库已初始化（至少15张表）
- [ ] 前端门户可以访问（http://服务器IP）
- [ ] 后台管理可以访问（http://服务器IP:8080）
- [ ] API接口正常（http://服务器IP:5001/api/home/statistics）
- [ ] 已从初始管理员凭据文件获取随机密码并成功登录后台
- [ ] 已修改初始密码并删除凭据文件
- [ ] 防火墙已配置
- [ ] 备份脚本已设置
- [ ] 日志正常输出

---

## 💡 性能优化建议

### 1. 数据库优化

```bash
# 进入MySQL容器
source .runtime/secrets.env && docker exec -it hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD"

# 优化配置
SET GLOBAL max_connections = 500;
SET GLOBAL innodb_buffer_pool_size = 2147483648;  # 2GB
```

### 2. Nginx缓存

编辑 `nginx/conf.d/default.conf`，添加缓存配置：

```nginx
# 静态文件缓存
location ~* \.(jpg|jpeg|png|gif|ico|css|js)$ {
    expires 30d;
    add_header Cache-Control "public, immutable";
}
```

### 3. API性能

在 [`docker-compose.yml`](docker-compose.yml) 中调整API环境变量：

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ASPNETCORE_URLS=http://+:5000
  - ASPNETCORE_THREADPOOL_MINWORKERS=100
```

---

**祝您部署顺利！** 🎉

如有问题，请参考 [`TROUBLESHOOTING.md`](TROUBLESHOOTING.md) 或联系技术支持。

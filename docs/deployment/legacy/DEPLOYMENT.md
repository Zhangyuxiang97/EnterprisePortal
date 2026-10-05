> 历史归档：此文档保留旧部署方式，仅供查阅。当前部署请使用 [Ubuntu22 Docker 部署指南](../ubuntu22-docker.md)。

# 海隆咨询官网部署文档

本文档详细说明海隆咨询官网项目的完整部署流程。

## 📋 目录

- [环境要求](#环境要求)
- [服务器准备](#服务器准备)
- [数据库部署](#数据库部署)
- [后端API部署](#后端api部署)
- [前端部署](#前端部署)
- [Nginx配置](#nginx配置)
- [SSL证书配置](#ssl证书配置)
- [域名配置](#域名配置)
- [监控和日志](#监控和日志)
- [备份策略](#备份策略)
- [故障排查](#故障排查)

## 🖥 环境要求

### 服务器配置

**最低配置**:
- CPU: 2核
- 内存: 4GB
- 硬盘: 50GB SSD
- 带宽: 5Mbps

**推荐配置**:
- CPU: 4核
- 内存: 8GB
- 硬盘: 100GB SSD
- 带宽: 10Mbps

### 操作系统

- **推荐**: Ubuntu 20.04 LTS / CentOS 8
- **支持**: Debian 10+, RHEL 8+

### 软件版本

| 软件 | 版本 | 说明 |
|------|------|------|
| .NET Runtime | 7.0+ | 后端运行时 |
| MySQL | 8.0+ | 数据库 |
| Nginx | 1.18+ | Web服务器 |
| Node.js | 18.0+ | 前端构建（仅构建时需要） |
| Git | 2.0+ | 版本控制 |

## 🔧 服务器准备

### 1. 更新系统

**Ubuntu/Debian**:
```bash
sudo apt update
sudo apt upgrade -y
```

**CentOS/RHEL**:
```bash
sudo yum update -y
```

### 2. 安装基础软件

**Ubuntu/Debian**:
```bash
# 安装必要工具
sudo apt install -y curl wget git unzip

# 安装.NET Runtime
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y aspnetcore-runtime-7.0

# 安装MySQL
sudo apt install -y mysql-server

# 安装Nginx
sudo apt install -y nginx

# 安装Node.js（用于前端构建）
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs
```

**CentOS/RHEL**:
```bash
# 安装必要工具
sudo yum install -y curl wget git unzip

# 安装.NET Runtime
sudo rpm -Uvh https://packages.microsoft.com/config/centos/8/packages-microsoft-prod.rpm
sudo yum install -y aspnetcore-runtime-7.0

# 安装MySQL
sudo yum install -y mysql-server

# 安装Nginx
sudo yum install -y nginx

# 安装Node.js
curl -fsSL https://rpm.nodesource.com/setup_18.x | sudo bash -
sudo yum install -y nodejs
```

### 3. 配置防火墙

**Ubuntu (UFW)**:
```bash
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw allow 3306/tcp  # MySQL (仅内网)
sudo ufw enable
```

**CentOS (firewalld)**:
```bash
sudo firewall-cmd --permanent --add-service=ssh
sudo firewall-cmd --permanent --add-service=http
sudo firewall-cmd --permanent --add-service=https
sudo firewall-cmd --permanent --add-service=mysql
sudo firewall-cmd --reload
```

### 4. 创建部署用户

```bash
# 创建部署用户
sudo useradd -m -s /bin/bash hailong
sudo passwd hailong

# 添加到sudo组
sudo usermod -aG sudo hailong

# 切换到部署用户
su - hailong
```

## 💾 数据库部署

### 1. 启动MySQL服务

```bash
# 启动MySQL
sudo systemctl start mysql
sudo systemctl enable mysql

# 检查状态
sudo systemctl status mysql
```

### 2. 安全配置

```bash
# 运行安全配置脚本
sudo mysql_secure_installation

# 按提示操作：
# - 设置root密码
# - 删除匿名用户
# - 禁止root远程登录
# - 删除测试数据库
# - 重新加载权限表
```

### 3. 创建数据库和用户

```bash
# 登录MySQL
sudo mysql -u root -p

# 执行以下SQL命令
```

```sql
-- 创建数据库
CREATE DATABASE hailong_consulting CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 创建应用用户
CREATE USER 'hailong_app'@'localhost' IDENTIFIED BY 'your_strong_password';

-- 授予权限
GRANT SELECT, INSERT, UPDATE, DELETE ON hailong_consulting.* TO 'hailong_app'@'localhost';

-- 刷新权限
FLUSH PRIVILEGES;

-- 退出
EXIT;
```

### 4. 导入数据库结构和数据

```bash
# 克隆项目（如果还没有）
cd /home/hailong
git clone <repository-url> hailong-project
cd hailong-project/SQL

# 导入数据库结构
mysql -u root -p hailong_consulting < hailong_consulting_schema.sql

# 导入初始数据
mysql -u root -p hailong_consulting < hailong_consulting_init_data.sql

# 验证导入
mysql -u root -p -e "USE hailong_consulting; SHOW TABLES;"
```

### 5. 数据库优化配置

编辑MySQL配置文件：

```bash
sudo nano /etc/mysql/mysql.conf.d/mysqld.cnf
```

添加或修改以下配置：

```ini
[mysqld]
# 基础配置
max_connections = 200
max_allowed_packet = 64M

# InnoDB配置
innodb_buffer_pool_size = 2G
innodb_log_file_size = 256M
innodb_flush_log_at_trx_commit = 2
innodb_flush_method = O_DIRECT

# 查询缓存
query_cache_type = 1
query_cache_size = 128M

# 慢查询日志
slow_query_log = 1
slow_query_log_file = /var/log/mysql/slow-query.log
long_query_time = 2
```

重启MySQL：

```bash
sudo systemctl restart mysql
```

## 🚀 后端API部署

### 1. 准备部署目录

```bash
# 创建部署目录
sudo mkdir -p /var/www/hailong-api
sudo chown -R hailong:hailong /var/www/hailong-api
```

### 2. 构建和发布应用

**方式一：在本地构建后上传**

```bash
# 在开发机器上
cd BackEnd/HailongConsulting.API
dotnet publish -c Release -o ./publish

# 压缩发布文件
tar -czf hailong-api.tar.gz -C ./publish .

# 上传到服务器
scp hailong-api.tar.gz hailong@your-server:/tmp/
```

**方式二：在服务器上构建**

```bash
# 在服务器上
cd /home/hailong/hailong-project/BackEnd/HailongConsulting.API

# 安装.NET SDK（如果需要）
sudo apt install -y dotnet-sdk-7.0

# 构建发布
dotnet publish -c Release -o /var/www/hailong-api
```

### 3. 配置应用

```bash
cd /var/www/hailong-api

# 编辑配置文件
nano appsettings.json
```

修改配置：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=hailong_consulting;User=hailong_app;Password=your_strong_password;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters-long-for-production",
    "Issuer": "HailongConsulting.API",
    "Audience": "HailongConsulting.Client",
    "ExpireHours": 24
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

### 4. 创建systemd服务

```bash
sudo nano /etc/systemd/system/hailong-api.service
```

添加以下内容：

```ini
[Unit]
Description=Hailong Consulting API
After=network.target

[Service]
Type=notify
User=hailong
Group=hailong
WorkingDirectory=/var/www/hailong-api
ExecStart=/usr/bin/dotnet /var/www/hailong-api/HailongConsulting.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=hailong-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### 5. 启动服务

```bash
# 重新加载systemd
sudo systemctl daemon-reload

# 启动服务
sudo systemctl start hailong-api

# 设置开机自启
sudo systemctl enable hailong-api

# 检查状态
sudo systemctl status hailong-api

# 查看日志
sudo journalctl -u hailong-api -f
```

## 🌐 前端部署

### 1. 构建前端项目

**后台管理系统**:

```bash
cd /home/hailong/hailong-project/hailong-admin

# 安装依赖
npm install

# 配置生产环境API地址
nano .env.production
# VITE_API_BASE_URL=https://api.yourdomain.com

# 构建
npm run build

# 部署到Nginx目录
sudo mkdir -p /var/www/hailong-admin
sudo cp -r dist/* /var/www/hailong-admin/
sudo chown -R www-data:www-data /var/www/hailong-admin
```

**前端门户**:

```bash
cd /home/hailong/hailong-project/hailong-protral

# 安装依赖
npm install

# 配置生产环境API地址
nano .env.production
# VITE_API_BASE_URL=https://api.yourdomain.com

# 构建
npm run build

# 部署到Nginx目录
sudo mkdir -p /var/www/hailong-protral
sudo cp -r dist/* /var/www/hailong-protral/
sudo chown -R www-data:www-data /var/www/hailong-protral
```

## 🔧 Nginx配置

### 1. 创建配置文件

**后端API配置**:

```bash
sudo nano /etc/nginx/sites-available/hailong-api
```

```nginx
server {
    listen 80;
    server_name api.yourdomain.com;

    # 日志
    access_log /var/log/nginx/hailong-api-access.log;
    error_log /var/log/nginx/hailong-api-error.log;

    # 反向代理到.NET应用
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # 超时设置
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # 静态文件（上传的附件）
    location /uploads/ {
        alias /var/www/hailong-api/wwwroot/uploads/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
```

**后台管理系统配置**:

```bash
sudo nano /etc/nginx/sites-available/hailong-admin
```

```nginx
server {
    listen 80;
    server_name admin.yourdomain.com;

    root /var/www/hailong-admin;
    index index.html;

    # 日志
    access_log /var/log/nginx/hailong-admin-access.log;
    error_log /var/log/nginx/hailong-admin-error.log;

    # Gzip压缩
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/x-javascript application/xml+rss application/json application/javascript;

    # SPA路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }

    # 静态资源缓存
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # 安全头
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
}
```

**前端门户配置**:

```bash
sudo nano /etc/nginx/sites-available/hailong-protral
```

```nginx
server {
    listen 80;
    server_name www.yourdomain.com yourdomain.com;

    root /var/www/hailong-protral;
    index index.html;

    # 日志
    access_log /var/log/nginx/hailong-protral-access.log;
    error_log /var/log/nginx/hailong-protral-error.log;

    # Gzip压缩
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/x-javascript application/xml+rss application/json application/javascript;

    # SPA路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }

    # 静态资源缓存
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # 安全头
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
}
```

### 2. 启用站点

```bash
# 创建符号链接
sudo ln -s /etc/nginx/sites-available/hailong-api /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/hailong-admin /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/hailong-protral /etc/nginx/sites-enabled/

# 测试配置
sudo nginx -t

# 重启Nginx
sudo systemctl restart nginx

# 设置开机自启
sudo systemctl enable nginx
```

## 🔒 SSL证书配置

### 使用Let's Encrypt免费证书

```bash
# 安装Certbot
sudo apt install -y certbot python3-certbot-nginx

# 为所有域名申请证书
sudo certbot --nginx -d api.yourdomain.com
sudo certbot --nginx -d admin.yourdomain.com
sudo certbot --nginx -d www.yourdomain.com -d yourdomain.com

# 测试自动续期
sudo certbot renew --dry-run

# 设置自动续期（已自动配置）
sudo systemctl status certbot.timer
```

### 手动配置SSL（如果使用其他证书）

```nginx
server {
    listen 443 ssl http2;
    server_name api.yourdomain.com;

    ssl_certificate /path/to/your/certificate.crt;
    ssl_certificate_key /path/to/your/private.key;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # ... 其他配置
}

# HTTP重定向到HTTPS
server {
    listen 80;
    server_name api.yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

## 🌍 域名配置

### DNS记录配置

在域名服务商处添加以下DNS记录：

| 类型 | 主机记录 | 记录值 | TTL |
|------|---------|--------|-----|
| A | @ | 服务器IP | 600 |
| A | www | 服务器IP | 600 |
| A | api | 服务器IP | 600 |
| A | admin | 服务器IP | 600 |

### 验证DNS解析

```bash
# 检查DNS解析
nslookup yourdomain.com
nslookup www.yourdomain.com
nslookup api.yourdomain.com
nslookup admin.yourdomain.com

# 或使用dig
dig yourdomain.com
```

## 📊 监控和日志

### 1. 系统监控

**安装监控工具**:

```bash
# 安装htop
sudo apt install -y htop

# 安装netdata（可选）
bash <(curl -Ss https://my-netdata.io/kickstart.sh)
```

### 2. 应用日志

**后端API日志**:

```bash
# 查看应用日志
sudo journalctl -u hailong-api -f

# 查看Serilog日志
tail -f /var/www/hailong-api/logs/log-$(date +%Y%m%d).txt
```

**Nginx日志**:

```bash
# 访问日志
tail -f /var/log/nginx/hailong-*-access.log

# 错误日志
tail -f /var/log/nginx/hailong-*-error.log
```

**MySQL日志**:

```bash
# 错误日志
sudo tail -f /var/log/mysql/error.log

# 慢查询日志
sudo tail -f /var/log/mysql/slow-query.log
```

### 3. 日志轮转

创建日志轮转配置：

```bash
sudo nano /etc/logrotate.d/hailong
```

```
/var/www/hailong-api/logs/*.txt {
    daily
    rotate 30
    compress
    delaycompress
    notifempty
    create 0640 hailong hailong
    sharedscripts
}
```

## 💾 备份策略

### 1. 数据库备份

创建备份脚本：

```bash
sudo nano /usr/local/bin/backup-mysql.sh
```

```bash
#!/bin/bash

# 配置
BACKUP_DIR="/backup/mysql"
DB_NAME="hailong_consulting"
DB_USER="root"
DB_PASS="your_password"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/${DB_NAME}_${DATE}.sql.gz"

# 创建备份目录
mkdir -p $BACKUP_DIR

# 备份数据库
mysqldump -u$DB_USER -p$DB_PASS $DB_NAME | gzip > $BACKUP_FILE

# 删除30天前的备份
find $BACKUP_DIR -name "*.sql.gz" -mtime +30 -delete

echo "Backup completed: $BACKUP_FILE"
```

```bash
# 设置执行权限
sudo chmod +x /usr/local/bin/backup-mysql.sh

# 添加到crontab（每天凌晨2点执行）
sudo crontab -e
# 添加：0 2 * * * /usr/local/bin/backup-mysql.sh
```

### 2. 文件备份

```bash
sudo nano /usr/local/bin/backup-files.sh
```

```bash
#!/bin/bash

# 配置
BACKUP_DIR="/backup/files"
DATE=$(date +%Y%m%d_%H%M%S)

# 创建备份目录
mkdir -p $BACKUP_DIR

# 备份上传的文件
tar -czf $BACKUP_DIR/uploads_${DATE}.tar.gz /var/www/hailong-api/wwwroot/uploads/

# 删除30天前的备份
find $BACKUP_DIR -name "*.tar.gz" -mtime +30 -delete

echo "File backup completed"
```

```bash
# 设置执行权限
sudo chmod +x /usr/local/bin/backup-files.sh

# 添加到crontab（每周日凌晨3点执行）
sudo crontab -e
# 添加：0 3 * * 0 /usr/local/bin/backup-files.sh
```

## 🔍 故障排查

### 1. 后端API无法访问

**检查服务状态**:
```bash
sudo systemctl status hailong-api
```

**查看日志**:
```bash
sudo journalctl -u hailong-api -n 100
```

**常见问题**:
- 端口被占用：`sudo netstat -tlnp | grep 5000`
- 配置文件错误：检查appsettings.json
- 数据库连接失败：测试数据库连接

### 2. 前端页面无法访问

**检查Nginx状态**:
```bash
sudo systemctl status nginx
```

**测试Nginx配置**:
```bash
sudo nginx -t
```

**查看错误日志**:
```bash
sudo tail -f /var/log/nginx/error.log
```

### 3. 数据库连接问题

**检查MySQL状态**:
```bash
sudo systemctl status mysql
```

**测试连接**:
```bash
mysql -u hailong_app -p -h localhost hailong_consulting
```

**检查权限**:
```sql
SHOW GRANTS FOR 'hailong_app'@'localhost';
```

### 4. SSL证书问题

**检查证书有效期**:
```bash
sudo certbot certificates
```

**手动续期**:
```bash
sudo certbot renew
```

### 5. 性能问题

**检查系统资源**:
```bash
# CPU和内存
htop

# 磁盘使用
df -h

# 磁盘IO
iostat -x 1

# 网络连接
netstat -an | grep ESTABLISHED | wc -l
```

**优化建议**:
- 增加服务器资源
- 优化数据库查询
- 启用缓存
- 使用CDN

## 📝 部署检查清单

部署完成后，请检查以下项目：

- [ ] 数据库已创建并导入数据
- [ ] 后端API服务正常运行
- [ ] 前端页面可以正常访问
- [ ] SSL证书已配置并有效
- [ ] 域名解析正确
- [ ] 文件上传功能正常
- [ ] 登录功能正常
- [ ] 日志记录正常
- [ ] 备份脚本已配置
- [ ] 防火墙规则已设置
- [ ] 监控工具已安装
- [ ] 性能测试通过

## 🔄 更新部署

### 更新后端

```bash
# 停止服务
sudo systemctl stop hailong-api

# 备份当前版本
sudo cp -r /var/www/hailong-api /var/www/hailong-api.backup

# 部署新版本
cd /home/hailong/hailong-project
git pull
cd BackEnd/HailongConsulting.API
dotnet publish -c Release -o /var/www/hailong-api

# 启动服务
sudo systemctl start hailong-api

# 检查状态
sudo systemctl status hailong-api
```

### 更新前端

```bash
# 备份当前版本
sudo cp -r /var/www/hailong-admin /var/www/hailong-admin.backup
sudo cp -r /var/www/hailong-protral /var/www/hailong-protral.backup

# 构建新版本
cd /home/hailong/hailong-project
git pull

# 更新后台管理
cd hailong-admin
npm install
npm run build
sudo cp -r dist/* /var/www/hailong-admin/

# 更新前端门户
cd ../hailong-protral
npm install
npm run build
sudo cp -r dist/* /var/www/hailong-protral/

# 重启Nginx
sudo systemctl reload nginx
```

## 📞 技术支持

如遇到部署问题，请联系：

- **技术支持邮箱**: support@hailongzixun.com
- **技术支持电话**: 0371-55894666

---

**文档版本**: v1.0.0  
**最后更新**: 2025年12月16日  
**维护团队**: 海隆咨询技术部
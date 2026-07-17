#!/bin/bash

###############################################################################
# 海隆咨询官网 - Ubuntu 22.04 一键部署脚本
# 使用方法: chmod +x deploy-ubuntu22.sh && sudo ./deploy-ubuntu22.sh
###############################################################################

set -e  # 遇到错误立即退出

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# 打印函数
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_step() {
    echo -e "\n${GREEN}========================================${NC}"
    echo -e "${GREEN}$1${NC}"
    echo -e "${GREEN}========================================${NC}\n"
}

# 检查是否为root用户
if [ "$EUID" -ne 0 ]; then 
    print_error "请使用root用户运行此脚本"
    exit 1
fi

# 获取服务器IP
SERVER_IP=$(ip addr | grep 'inet ' | grep -v '127.0.0.1' | awk '{print $2}' | cut -d/ -f1 | head -n1)

print_step "欢迎使用海隆咨询官网一键部署脚本 (Ubuntu 22.04)"
echo "服务器IP: $SERVER_IP"
echo ""

read -p "项目文件路径 (默认: /opt/hailong/project): " PROJECT_PATH
PROJECT_PATH=${PROJECT_PATH:-/opt/hailong/project}

echo ""
print_info "配置信息："
echo "  MySQL/JWT 凭据: 将随机生成并写入受限文件"
echo "  项目路径: $PROJECT_PATH"
echo ""

read -p "确认开始部署? (y/n): " CONFIRM
if [ "$CONFIRM" != "y" ]; then
    print_warn "部署已取消"
    exit 0
fi

if [ ! -d "$PROJECT_PATH" ]; then
    print_error "项目路径不存在: $PROJECT_PATH"
    print_info "请先将项目文件上传到服务器"
    exit 1
fi

# 密钥仅在首次部署生成；以后重跑脚本会复用同一份文件，避免已有数据库和应用配置失配。
RUNTIME_SECRETS_DIR="$PROJECT_PATH/.runtime"
RUNTIME_SECRETS_FILE="$RUNTIME_SECRETS_DIR/secrets.env"
SECRETS_CREATED=false
mkdir -p "$RUNTIME_SECRETS_DIR"
chmod 700 "$RUNTIME_SECRETS_DIR"

if [ -f "$RUNTIME_SECRETS_FILE" ]; then
    MYSQL_ROOT_PASSWORD=$(sed -n 's/^MYSQL_ROOT_PASSWORD=//p' "$RUNTIME_SECRETS_FILE")
    MYSQL_APP_PASSWORD=$(sed -n 's/^MYSQL_PASSWORD=//p' "$RUNTIME_SECRETS_FILE")
    JWT_SECRET=$(sed -n 's/^Jwt__Key=//p' "$RUNTIME_SECRETS_FILE")
    if [ -z "$MYSQL_ROOT_PASSWORD" ] || [ -z "$MYSQL_APP_PASSWORD" ] || [ -z "$JWT_SECRET" ]; then
        print_error "运行时密钥文件格式不完整: $RUNTIME_SECRETS_FILE"
        exit 1
    fi
    print_info "已复用运行时密钥文件: $RUNTIME_SECRETS_FILE"
else
    MYSQL_ROOT_PASSWORD=$(openssl rand -base64 32)
    MYSQL_APP_PASSWORD=$(openssl rand -base64 32)
    JWT_SECRET=$(openssl rand -base64 48)
    cat > "$RUNTIME_SECRETS_FILE" <<EOF
MYSQL_ROOT_PASSWORD=$MYSQL_ROOT_PASSWORD
MYSQL_PASSWORD=$MYSQL_APP_PASSWORD
ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=hailong_consulting;User=hailong_app;Password=$MYSQL_APP_PASSWORD;CharSet=utf8mb4;
Jwt__Key=$JWT_SECRET
EOF
    chmod 600 "$RUNTIME_SECRETS_FILE"
    SECRETS_CREATED=true
    print_warn "已首次生成运行时密钥，请立即安全保存以下内容；此信息不会在后续部署时再次打印。"
    echo "  MySQL Root密码: $MYSQL_ROOT_PASSWORD"
    echo "  MySQL应用密码: $MYSQL_APP_PASSWORD"
    echo "  JWT密钥: $JWT_SECRET"
    echo "  密钥文件: $RUNTIME_SECRETS_FILE"
fi

if [ "$SECRETS_CREATED" = true ] && command -v mysql &> /dev/null && systemctl is-active --quiet mysql; then
    GENERATED_SECRETS_BACKUP="$RUNTIME_SECRETS_FILE.generated.$(date +%Y%m%d_%H%M%S)"
    mv "$RUNTIME_SECRETS_FILE" "$GENERATED_SECRETS_BACKUP"
    print_error "检测到已有 MySQL 服务，但未找到可复用的运行时密钥文件。"
    print_error "已停止部署，避免新生成的密码与已有数据库不匹配。"
    print_info "请从现有安全记录恢复真实凭据到 $RUNTIME_SECRETS_FILE 后重试。"
    print_info "本次未使用的随机密钥已移动到: $GENERATED_SECRETS_BACKUP"
    exit 1
fi

###############################################################################
# 第一步：更新系统并安装基础软件
###############################################################################
print_step "第一步：更新系统并安装基础软件"

print_info "更新系统..."
apt update
apt upgrade -y

print_info "安装基础工具..."
apt install -y wget curl git unzip vim net-tools software-properties-common

###############################################################################
# 第二步：安装.NET 8.0运行时
###############################################################################
print_step "第二步：安装.NET 8.0运行时"

if ! command -v dotnet &> /dev/null; then
    print_info "添加Microsoft软件源..."
    wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    apt update
    
    print_info "安装.NET运行时和SDK..."
    apt install -y aspnetcore-runtime-8.0 dotnet-sdk-8.0
    
    print_info ".NET版本: $(dotnet --version)"
else
    print_info ".NET已安装，版本: $(dotnet --version)"
fi

###############################################################################
# 第三步：安装MySQL 8.0
###############################################################################
print_step "第三步：安装MySQL 8.0"

if ! command -v mysql &> /dev/null; then
    print_info "安装MySQL..."
    apt install -y mysql-server
    
    print_info "启动MySQL..."
    systemctl start mysql
    systemctl enable mysql
    
    # 设置root密码
    print_info "配置MySQL root密码..."
    mysql <<EOF
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '$MYSQL_ROOT_PASSWORD';
FLUSH PRIVILEGES;
EOF
else
    print_info "MySQL已安装"
fi

###############################################################################
# 第四步：创建数据库和用户
###############################################################################
print_step "第四步：创建数据库和用户"

print_info "创建数据库和应用用户..."
mysql -u root -p"$MYSQL_ROOT_PASSWORD" <<EOF
CREATE DATABASE IF NOT EXISTS hailong_consulting CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER IF NOT EXISTS 'hailong_app'@'localhost' IDENTIFIED BY '$MYSQL_APP_PASSWORD';
GRANT ALL PRIVILEGES ON hailong_consulting.* TO 'hailong_app'@'localhost';
FLUSH PRIVILEGES;
EOF

print_info "数据库创建成功"

###############################################################################
# 第五步：安装Nginx
###############################################################################
print_step "第五步：安装Nginx"

if ! command -v nginx &> /dev/null; then
    print_info "安装Nginx..."
    apt install -y nginx
    
    print_info "启动Nginx..."
    systemctl start nginx
    systemctl enable nginx
else
    print_info "Nginx已安装"
fi

###############################################################################
# 第六步：安装Node.js
###############################################################################
print_step "第六步：安装Node.js"

if ! command -v node &> /dev/null; then
    print_info "安装Node.js 18..."
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
    apt install -y nodejs
    
    print_info "Node.js版本: $(node --version)"
    print_info "npm版本: $(npm --version)"
else
    print_info "Node.js已安装，版本: $(node --version)"
fi

###############################################################################
# 第七步：部署后端API
###############################################################################
print_step "第七步：部署后端API"

if [ ! -d "$PROJECT_PATH" ]; then
    print_error "项目路径不存在: $PROJECT_PATH"
    print_info "请先将项目文件上传到服务器"
    exit 1
fi

print_info "创建部署目录..."
mkdir -p /var/www/hailong-api

print_info "发布后端应用..."
cd "$PROJECT_PATH/BackEnd/HailongConsulting.API"
dotnet publish -c Release -o /var/www/hailong-api

print_info "配置应用..."
cat > /var/www/hailong-api/appsettings.json <<EOF
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=hailong_consulting;User=hailong_app;Password=$MYSQL_APP_PASSWORD;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Key": "$JWT_SECRET",
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
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
EOF

print_info "创建systemd服务..."
cat > /etc/systemd/system/hailong-api.service <<EOF
[Unit]
Description=Hailong Consulting API
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/hailong-api
ExecStart=/usr/bin/dotnet /var/www/hailong-api/HailongConsulting.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=hailong-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

print_info "启动后端服务..."
systemctl daemon-reload
systemctl start hailong-api
systemctl enable hailong-api

sleep 3
if systemctl is-active --quiet hailong-api; then
    print_info "后端API启动成功"
else
    print_error "后端API启动失败，请查看日志: journalctl -u hailong-api -n 50"
fi

###############################################################################
# 第八步：部署前端
###############################################################################
print_step "第八步：部署前端"

# 部署后台管理系统
if [ -d "$PROJECT_PATH/hailong-admin" ]; then
    print_info "构建后台管理系统..."
    cd "$PROJECT_PATH/hailong-admin"
    
    # 使用相对路径配置，无需修改.env.production
    print_info "使用相对路径配置（/api）..."
    
    print_info "安装依赖..."
    npm install --registry=https://registry.npmmirror.com
    
    print_info "构建项目..."
    npm run build
    
    print_info "部署到Nginx..."
    mkdir -p /var/www/hailong-admin
    cp -r dist/* /var/www/hailong-admin/
    
    print_info "后台管理系统部署成功"
else
    print_warn "未找到后台管理系统目录"
fi

# 部署前端门户
if [ -d "$PROJECT_PATH/hailong-protral" ]; then
    print_info "构建前端门户..."
    cd "$PROJECT_PATH/hailong-protral"
    
    # 使用相对路径配置，无需修改.env.production
    print_info "使用相对路径配置（/api）..."
    
    npm install --registry=https://registry.npmmirror.com
    npm run build
    
    mkdir -p /var/www/hailong-protral
    cp -r dist/* /var/www/hailong-protral/
    
    print_info "前端门户部署成功"
fi

###############################################################################
# 第九步：配置Nginx
###############################################################################
print_step "第九步：配置Nginx"

print_info "配置后端API代理..."
cat > /etc/nginx/sites-available/hailong-api <<'EOF'
server {
    listen 5001;
    server_name _;

    client_max_body_size 100M;

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
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    location /uploads/ {
        alias /var/www/hailong-api/wwwroot/uploads/;
        expires 30d;
    }
}
EOF

print_info "配置后台管理系统..."
cat > /etc/nginx/sites-available/hailong-admin <<'EOF'
server {
    listen 8080;
    server_name _;

    root /var/www/hailong-admin;
    index index.html;

    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/javascript application/json;

    # API代理 - 使用相对路径访问
    location /api/ {
        proxy_pass http://localhost:5000/;
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

    # 上传文件访问
    location ^~ /uploads/ {
        alias /var/www/hailong-api/wwwroot/uploads/;
        charset utf-8;
        expires 30d;
        add_header Cache-Control "public, immutable";
        add_header Access-Control-Allow-Origin *;
    }

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
EOF

print_info "配置前端门户..."
cat > /etc/nginx/sites-available/hailong-protral <<'EOF'
server {
    listen 80;
    server_name _;

    root /var/www/hailong-protral;
    index index.html;

    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/javascript application/json;

    # API代理 - 使用相对路径访问
    location /api/ {
        proxy_pass http://localhost:5000/;
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

    # 上传文件访问
    location ^~ /uploads/ {
        alias /var/www/hailong-api/wwwroot/uploads/;
        charset utf-8;
        expires 30d;
        add_header Cache-Control "public, immutable";
        add_header Access-Control-Allow-Origin *;
    }

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
EOF

# 启用站点
ln -sf /etc/nginx/sites-available/hailong-api /etc/nginx/sites-enabled/
ln -sf /etc/nginx/sites-available/hailong-admin /etc/nginx/sites-enabled/
ln -sf /etc/nginx/sites-available/hailong-protral /etc/nginx/sites-enabled/

# 删除默认站点
rm -f /etc/nginx/sites-enabled/default

print_info "测试Nginx配置..."
nginx -t

print_info "重启Nginx..."
systemctl restart nginx

###############################################################################
# 第十步：配置防火墙
###############################################################################
print_step "第十步：配置防火墙"

if command -v ufw &> /dev/null; then
    print_info "配置UFW防火墙..."
    ufw allow 80/tcp
    ufw allow 8080/tcp
    ufw allow 5001/tcp
    ufw allow 22/tcp
    ufw --force enable
    
    print_info "防火墙配置完成"
else
    print_warn "UFW未安装，跳过防火墙配置"
fi

###############################################################################
# 部署完成
###############################################################################
print_step "部署完成！"

echo ""
echo "=========================================="
echo "  部署信息"
echo "=========================================="
echo ""
echo "服务器IP: $SERVER_IP"
echo ""
echo "访问地址："
echo "  - 前端门户:     http://$SERVER_IP"
echo "  - 后台管理:     http://$SERVER_IP:8080"
echo "  - API接口:      http://$SERVER_IP:5001"
echo ""
echo "初始管理员信息："
echo "  - 文件: /var/www/hailong-api/logs/bootstrap/initial-admin-credentials.txt"
echo ""
echo "数据库信息："
echo "  - 数据库名: hailong_consulting"
echo "  - 用户名: hailong_app"
echo "  - 密钥文件: $RUNTIME_SECRETS_FILE"
echo ""
echo "常用命令："
echo "  - 查看API状态:  systemctl status hailong-api"
echo "  - 查看API日志:  journalctl -u hailong-api -f"
echo "  - 重启API:      systemctl restart hailong-api"
echo "  - 重启Nginx:    systemctl restart nginx"
echo ""
echo "=========================================="
echo ""

print_info "请在浏览器中访问上述地址进行测试"
print_warn "首次登录后请立即修改系统自动生成的管理员密码。"

# 保存配置信息
cat > /root/hailong-deploy-info.txt <<EOF
海隆咨询官网部署信息 (Ubuntu 22.04)
部署时间: $(date)
服务器IP: $SERVER_IP

访问地址:
- 前端门户: http://$SERVER_IP
- 后台管理: http://$SERVER_IP:8080
- API接口: http://$SERVER_IP:5001

数据库信息:
- 数据库名: hailong_consulting
- 用户名: hailong_app
- 密钥文件: $RUNTIME_SECRETS_FILE

初始管理员凭据:
- /var/www/hailong-api/logs/bootstrap/initial-admin-credentials.txt

项目路径: $PROJECT_PATH
部署路径: /var/www/hailong-api
EOF

print_info "部署信息已保存到: /root/hailong-deploy-info.txt"

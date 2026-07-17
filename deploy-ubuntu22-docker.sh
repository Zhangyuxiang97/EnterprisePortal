#!/bin/bash

###############################################################################
# 海隆咨询官网 - Ubuntu 22.04 Docker一键部署脚本
# 使用方法: chmod +x deploy-ubuntu22-docker.sh && sudo ./deploy-ubuntu22-docker.sh
###############################################################################

set -e  # 遇到错误立即退出

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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
    echo -e "\n${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}\n"
}

# 检查是否为root用户
if [ "$EUID" -ne 0 ]; then 
    print_error "请使用root用户运行此脚本"
    exit 1
fi

# 获取服务器IP
SERVER_IP=$(ip addr | grep 'inet ' | grep -v '127.0.0.1' | awk '{print $2}' | cut -d/ -f1 | head -n1)

print_step "欢迎使用海隆咨询官网Docker一键部署脚本 (Ubuntu 22.04)"
echo "服务器IP: $SERVER_IP"
echo ""
echo "本脚本将使用Docker容器化部署，包括："
echo "  - MySQL 8.0 容器"
echo "  - .NET 8.0 API 容器"
echo "  - Nginx 容器（前端+反向代理）"
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

###############################################################################
# 第一步：检查项目文件
###############################################################################
print_step "第一步：检查项目文件"

if [ ! -d "$PROJECT_PATH" ]; then
    print_error "项目路径不存在: $PROJECT_PATH"
    print_info "请先将项目文件上传到服务器"
    exit 1
fi

print_info "检查必需文件..."
REQUIRED_FILES=(
    "$PROJECT_PATH/docker-compose.yml"
    "$PROJECT_PATH/BackEnd/HailongConsulting.API/Dockerfile"
    "$PROJECT_PATH/nginx/nginx.conf"
    "$PROJECT_PATH/nginx/conf.d/default.conf"
)

for file in "${REQUIRED_FILES[@]}"; do
    if [ ! -f "$file" ]; then
        print_error "缺少必需文件: $file"
        exit 1
    fi
done

print_info "所有必需文件检查通过"

# 密钥仅在首次部署生成；以后重跑脚本会复用同一份文件，避免与已有 MySQL 数据卷失配。
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
ConnectionStrings__DefaultConnection=Server=mysql;Port=3306;Database=hailong_consulting;User=hailong_app;Password=$MYSQL_APP_PASSWORD;CharSet=utf8mb4;
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

###############################################################################
# 第二步：安装Docker（最新版本）
###############################################################################
print_step "第二步：安装Docker"

if command -v docker &> /dev/null; then
    DOCKER_VERSION=$(docker version --format '{{.Server.Version}}' 2>/dev/null || echo "unknown")
    print_info "Docker已安装，版本: $DOCKER_VERSION"
    
    # 检查是否需要升级
    print_warn "检查Docker版本兼容性..."
    if docker version &> /dev/null; then
        print_info "Docker版本正常"
    else
        print_warn "Docker版本可能过旧，建议升级"
        read -p "是否重新安装最新版Docker? (y/n): " REINSTALL
        if [ "$REINSTALL" == "y" ]; then
            print_info "卸载旧版Docker..."
            apt remove -y docker docker-engine docker.io containerd runc docker-ce docker-ce-cli containerd.io 2>/dev/null || true
            apt autoremove -y
        else
            print_warn "继续使用当前版本"
        fi
    fi
fi

if ! command -v docker &> /dev/null; then
    print_info "安装最新版Docker..."
    
    # 更新包索引
    apt update
    
    # 安装依赖
    apt install -y ca-certificates curl gnupg lsb-release
    
    # 添加Docker官方GPG密钥
    mkdir -p /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    
    # 设置Docker仓库
    echo \
      "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
      $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
    
    # 安装Docker Engine
    apt update
    apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    
    # 启动Docker
    systemctl start docker
    systemctl enable docker
    
    print_info "Docker安装成功: $(docker --version)"
    
    # 配置Docker镜像加速器（中国大陆）
    print_info "配置Docker镜像加速器..."
    mkdir -p /etc/docker
    cat > /etc/docker/daemon.json <<DOCKER_CONFIG
{
  "registry-mirrors": [
    "https://dockerproxy.com",
    "https://docker.nju.edu.cn",
    "https://docker.m.daocloud.io"
  ],
  "dns": ["8.8.8.8", "114.114.114.114"],
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "100m",
    "max-file": "3"
  }
}
DOCKER_CONFIG
    
    # 重启Docker使配置生效
    systemctl daemon-reload
    systemctl restart docker
    
    print_info "Docker镜像加速器配置完成"
fi

# 如果Docker已安装但未配置镜像加速器，则配置
if [ ! -f /etc/docker/daemon.json ]; then
    print_info "配置Docker镜像加速器..."
    mkdir -p /etc/docker
    cat > /etc/docker/daemon.json <<DOCKER_CONFIG
{
  "registry-mirrors": [
    "https://dockerproxy.com",
    "https://docker.nju.edu.cn",
    "https://docker.m.daocloud.io"
  ],
  "dns": ["8.8.8.8", "114.114.114.114"],
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "100m",
    "max-file": "3"
  }
}
DOCKER_CONFIG
    
    systemctl daemon-reload
    systemctl restart docker
    print_info "Docker镜像加速器配置完成"
fi

if [ "$SECRETS_CREATED" = true ] && docker ps -a --format '{{.Names}}' | grep -qx 'hailong-mysql'; then
    GENERATED_SECRETS_BACKUP="$RUNTIME_SECRETS_FILE.generated.$(date +%Y%m%d_%H%M%S)"
    mv "$RUNTIME_SECRETS_FILE" "$GENERATED_SECRETS_BACKUP"
    print_error "检测到已有 hailong-mysql 容器，但未找到可复用的运行时密钥文件。"
    print_error "已停止部署，避免新生成的密码与已有数据库不匹配。"
    print_info "请从现有安全记录恢复真实凭据到 $RUNTIME_SECRETS_FILE 后重试。"
    print_info "本次未使用的随机密钥已移动到: $GENERATED_SECRETS_BACKUP"
    exit 1
fi

###############################################################################
# 第三步：检测Docker Compose命令
###############################################################################
print_step "第三步：检测Docker Compose"

# 优先使用 docker compose (新版插件)
if docker compose version &> /dev/null; then
    COMPOSE_CMD="docker compose"
    print_info "使用Docker Compose插件: $(docker compose version)"
# 回退到 docker-compose (独立版本)
elif command -v docker-compose &> /dev/null; then
    COMPOSE_CMD="docker-compose"
    print_info "使用独立Docker Compose: $(docker-compose --version)"
else
    print_warn "未检测到Docker Compose，开始安装..."
    
    # 下载最新版Docker Compose
    COMPOSE_VERSION=$(curl -s https://api.github.com/repos/docker/compose/releases/latest | grep 'tag_name' | cut -d\" -f4)
    if [ -z "$COMPOSE_VERSION" ]; then
        COMPOSE_VERSION="v2.24.0"
    fi
    
    print_info "安装Docker Compose $COMPOSE_VERSION..."
    curl -L "https://github.com/docker/compose/releases/download/${COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    
    chmod +x /usr/local/bin/docker-compose
    ln -sf /usr/local/bin/docker-compose /usr/bin/docker-compose
    
    COMPOSE_CMD="docker-compose"
    print_info "Docker Compose安装成功: $(docker-compose --version)"
fi

print_info "将使用命令: $COMPOSE_CMD"

###############################################################################
# 第四步：安装Node.js（用于构建前端）
###############################################################################
print_step "第四步：安装Node.js"

if command -v node &> /dev/null; then
    print_info "Node.js已安装，版本: $(node --version)"
else
    print_info "安装Node.js 18..."
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
    apt install -y nodejs
    
    print_info "Node.js安装成功: $(node --version)"
    print_info "npm版本: $(npm --version)"
fi

###############################################################################
# 第五步：更新docker-compose.yml配置
###############################################################################
print_step "第五步：更新docker-compose.yml配置"

print_info "生成配置文件..."

# 备份原配置
if [ -f "$PROJECT_PATH/docker-compose.yml" ]; then
    cp "$PROJECT_PATH/docker-compose.yml" "$PROJECT_PATH/docker-compose.yml.bak.$(date +%Y%m%d_%H%M%S)"
fi

# 生成新的docker-compose.yml
cat > "$PROJECT_PATH/docker-compose.yml" <<EOF
services:
  # MySQL数据库服务
  mysql:
    image: mysql:8.0
    container_name: hailong-mysql
    restart: always
    command: --character-set-server=utf8mb4 --collation-server=utf8mb4_unicode_ci --default-authentication-plugin=mysql_native_password
    env_file:
      - .runtime/secrets.env
    environment:
      MYSQL_DATABASE: hailong_consulting
      MYSQL_USER: hailong_app
      TZ: Asia/Shanghai
    volumes:
      - mysql-data:/var/lib/mysql
      - ./SQL:/docker-entrypoint-initdb.d:ro
    ports:
      - "3306:3306"
    networks:
      - hailong-network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      timeout: 5s
      retries: 5

  # 后端API服务
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
      - ASPNETCORE_URLS=http://+:5000
      - Jwt__Issuer=HailongConsulting.API
      - Jwt__Audience=HailongConsulting.Client
      - Jwt__ExpireHours=24
      - TZ=Asia/Shanghai
    volumes:
      - api-uploads:/app/wwwroot/uploads
      - api-logs:/app/logs
    depends_on:
      mysql:
        condition: service_healthy
    networks:
      - hailong-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/api/home/statistics"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Nginx服务（前端+反向代理）
  nginx:
    image: nginx:alpine
    container_name: hailong-nginx
    restart: always
    ports:
      - "80:80"        # 前端门户
      - "8080:8080"    # 后台管理
      - "5001:5001"    # API代理
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/conf.d:/etc/nginx/conf.d:ro
      - ./hailong-admin/dist:/usr/share/nginx/html/admin:ro
      - ./hailong-protral/dist:/usr/share/nginx/html/portal:ro
      - api-uploads:/usr/share/nginx/html/uploads:ro
    depends_on:
      - api
    networks:
      - hailong-network
    healthcheck:
      test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost:80"]
      interval: 30s
      timeout: 10s
      retries: 3

networks:
  hailong-network:
    driver: bridge

volumes:
  mysql-data:
    driver: local
  api-uploads:
    driver: local
  api-logs:
    driver: local
EOF

print_info "docker-compose.yml配置完成"

###############################################################################
# 第六步：构建前端
###############################################################################
print_step "第六步：构建前端"

# 构建后台管理系统
if [ -d "$PROJECT_PATH/hailong-admin" ]; then
    print_info "构建后台管理系统..."
    cd "$PROJECT_PATH/hailong-admin"
    
    # 使用相对路径配置，无需修改.env.production
    print_info "使用相对路径配置（/api）..."
    
    print_info "安装依赖..."
    npm install --registry=https://registry.npmmirror.com
    
    print_info "构建项目..."
    npm run build
    
    if [ -d "dist" ]; then
        print_info "后台管理系统构建成功"
    else
        print_error "后台管理系统构建失败"
        exit 1
    fi
else
    print_warn "未找到后台管理系统目录"
fi

# 构建前端门户
if [ -d "$PROJECT_PATH/hailong-protral" ]; then
    print_info "构建前端门户..."
    cd "$PROJECT_PATH/hailong-protral"
    
    # 使用相对路径配置，无需修改.env.production
    print_info "使用相对路径配置（/api）..."
    
    npm install --registry=https://registry.npmmirror.com
    npm run build
    
    if [ -d "dist" ]; then
        print_info "前端门户构建成功"
    else
        print_error "前端门户构建失败"
        exit 1
    fi
else
    print_warn "未找到前端门户目录"
fi

###############################################################################
# 第七步：启动Docker容器
###############################################################################
print_step "第七步：启动Docker容器"

cd "$PROJECT_PATH"

print_info "停止并删除旧容器（如果存在）..."
$COMPOSE_CMD down 2>/dev/null || true

print_info "启动所有服务..."
print_warn "首次启动需要构建镜像，可能需要5-10分钟，请耐心等待..."

$COMPOSE_CMD up -d --build

print_info "等待服务启动..."
sleep 10

###############################################################################
# 第八步：验证部署
###############################################################################
print_step "第八步：验证部署"

print_info "检查容器状态..."
$COMPOSE_CMD ps

# 等待MySQL初始化完成
print_info "等待MySQL初始化完成..."
for i in {1..30}; do
    if docker exec hailong-mysql mysqladmin ping -h localhost -p"$MYSQL_ROOT_PASSWORD" &> /dev/null; then
        print_info "MySQL已就绪"
        break
    fi
    echo -n "."
    sleep 2
done
echo ""

# 等待API启动
print_info "等待API服务启动..."
for i in {1..30}; do
    if curl -s http://localhost:5001/api/home/statistics > /dev/null 2>&1; then
        print_info "API服务已就绪"
        break
    fi
    echo -n "."
    sleep 2
done
echo ""

# 检查数据库表
print_info "验证数据库初始化..."
TABLE_COUNT=$(docker exec hailong-mysql mysql -u root -p"$MYSQL_ROOT_PASSWORD" -e "USE hailong_consulting; SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='hailong_consulting';" -s -N 2>/dev/null || echo "0")

if [ "$TABLE_COUNT" -gt "0" ]; then
    print_info "数据库初始化成功，共 $TABLE_COUNT 张表"
else
    print_warn "数据库表数量为0，可能初始化失败"
    print_info "查看MySQL日志："
    $COMPOSE_CMD logs mysql | tail -20
fi

###############################################################################
# 第九步：配置防火墙
###############################################################################
print_step "第九步：配置防火墙"

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
echo "  Docker部署信息"
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
echo "  - 首次启动后请查看 API 容器日志，或容器内 logs/bootstrap/initial-admin-credentials.txt"
echo ""
echo "Docker容器："
echo "  - hailong-mysql  (MySQL 8.0)"
echo "  - hailong-api    (.NET 8.0 API)"
echo "  - hailong-nginx  (Nginx)"
echo ""
echo "常用Docker命令："
echo "  - 查看容器状态:  $COMPOSE_CMD ps"
echo "  - 查看日志:      $COMPOSE_CMD logs -f"
echo "  - 重启服务:      $COMPOSE_CMD restart"
echo "  - 停止服务:      $COMPOSE_CMD down"
echo "  - 启动服务:      $COMPOSE_CMD up -d"
echo ""
echo "数据库信息："
echo "  - 数据库名: hailong_consulting"
echo "  - 应用用户: hailong_app"
echo "  - 密钥文件: $RUNTIME_SECRETS_FILE"
echo ""
echo "项目路径: $PROJECT_PATH"
echo ""
echo "=========================================="
echo ""

print_info "请在浏览器中访问上述地址进行测试"
print_warn "首次登录后请立即修改系统自动生成的管理员密码。"

# 保存配置信息
cat > /root/hailong-docker-deploy-info.txt <<EOF
海隆咨询官网Docker部署信息 (Ubuntu 22.04)
部署时间: $(date)
服务器IP: $SERVER_IP
使用命令: $COMPOSE_CMD

访问地址:
- 前端门户: http://$SERVER_IP
- 后台管理: http://$SERVER_IP:8080
- API接口: http://$SERVER_IP:5001

Docker容器:
- hailong-mysql  (MySQL 8.0)
- hailong-api    (.NET 8.0 API)
- hailong-nginx  (Nginx)

数据库信息:
- 数据库名: hailong_consulting
- 用户名: hailong_app
- 密钥文件: $RUNTIME_SECRETS_FILE

初始管理员凭据:
- 查看 API 容器日志，或容器内 logs/bootstrap/initial-admin-credentials.txt

项目路径: $PROJECT_PATH

常用命令:
cd $PROJECT_PATH
$COMPOSE_CMD ps              # 查看容器状态
$COMPOSE_CMD logs -f         # 查看日志
$COMPOSE_CMD restart api     # 重启API
$COMPOSE_CMD restart nginx   # 重启Nginx
$COMPOSE_CMD down            # 停止所有服务
$COMPOSE_CMD up -d           # 启动所有服务

备份命令:
source $RUNTIME_SECRETS_FILE && docker exec hailong-mysql mysqldump -u root -p\$MYSQL_ROOT_PASSWORD hailong_consulting > backup.sql
EOF

print_info "部署信息已保存到: /root/hailong-docker-deploy-info.txt"

# 显示容器状态
echo ""
print_info "当前容器状态："
$COMPOSE_CMD ps

echo ""
print_info "如需查看详细日志，请执行："
echo "  cd $PROJECT_PATH && $COMPOSE_CMD logs -f"

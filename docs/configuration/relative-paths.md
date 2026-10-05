# 相对路径配置说明

## 概述

本文档说明了前后端配置路径改为相对路径后的配置方案，使系统更加灵活，便于在不同环境下部署。

## 配置修改内容

### 1. 前端门户 (hailong-protral)

#### 环境配置文件

**生产环境** ([`.env.production`](../../hailong-protral/.env.production))
```env
# 生产环境配置
# 使用相对路径，通过nginx反向代理访问API
VITE_API_BASE_URL=/api
```

**开发环境** ([`.env.development`](../../hailong-protral/.env.development))
```env
# 开发环境配置
VITE_APP_TITLE=海隆咨询门户
# 开发环境使用相对路径，需要配置vite代理
VITE_API_BASE_URL=/api
VITE_APP_PORT=3000
```

#### Vite配置 ([`vite.config.js`](../../hailong-protral/vite.config.js))

添加了开发环境的API代理配置：
```javascript
server: {
  port: 3000,
  open: true,
  proxy: {
    // 开发环境API代理
    '/api': {
      target: 'https://localhost:49522',
      changeOrigin: true,
      secure: false,
      rewrite: (path) => path.replace(/^\/api/, '')
    }
  }
}
```

### 2. 后台管理系统 (hailong-admin)

#### 环境配置文件

**生产环境** ([`.env.production`](../../hailong-admin/.env.production))
```env
# 生产环境配置
VITE_APP_TITLE=海隆咨询后台管理系统
# 使用相对路径，通过nginx反向代理访问API
VITE_API_BASE_URL=/api
```

**开发环境** ([`.env.development`](../../hailong-admin/.env.development))
```env
# 开发环境配置
VITE_APP_TITLE=海隆咨询后台管理系统
# 开发环境使用相对路径，需要配置vite代理
VITE_API_BASE_URL=/api
VITE_APP_PORT=3000
```

#### Vite配置 ([`vite.config.js`](../../hailong-admin/vite.config.js))

更新了开发环境的API代理配置：
```javascript
server: {
  port: 3000,
  proxy: {
    // 开发环境API代理
    '/api': {
      target: 'https://localhost:49522',
      changeOrigin: true,
      secure: false,
      rewrite: (path) => path.replace(/^\/api/, '')
    }
  }
}
```

### 3. 后端API (BackEnd/HailongConsulting.API)

#### 文件上传配置

后端的文件上传路径已经使用相对路径配置（[`appsettings.json`](../../BackEnd/HailongConsulting.API/appsettings.json)）：
```json
"FileUpload": {
  "RootPath": "wwwroot",
  "UploadPath": "uploads/attachments",
  "MaxFileSize": 10485760,
  ...
}
```

这个配置无需修改，已经是相对路径。

### 4. Nginx配置 ([`nginx/conf.d/default.conf`](../../nginx/conf.d/default.conf))

#### 前端门户 (端口 80)

添加了API代理配置：
```nginx
# API代理 - 使用相对路径访问
location /api/ {
    proxy_pass http://api:5000/;
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
```

#### 后台管理系统 (端口 8080)

添加了API代理配置：
```nginx
# API代理 - 使用相对路径访问
location /api/ {
    proxy_pass http://api:5000/;
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
```

## 工作原理

### 开发环境

1. 前端应用运行在 `localhost:3000`
2. 前端代码中使用相对路径 `/api` 访问API
3. Vite开发服务器的代理配置将 `/api` 请求转发到 `https://localhost:49522`
4. 后端API运行在 `https://localhost:49522`

**请求流程：**
```
前端 (localhost:3000) 
  → 请求 /api/xxx 
  → Vite代理转发到 https://localhost:49522/xxx 
  → 后端API处理
```

### 生产环境 (Docker部署)

1. 前端门户通过Nginx提供服务（端口80）
2. 后台管理通过Nginx提供服务（端口8080）
3. 后端API运行在Docker容器内（端口5000）
4. Nginx作为反向代理，将 `/api` 请求转发到后端API容器

**请求流程：**
```
浏览器 
  → 请求 http://yourdomain.com/api/xxx 
  → Nginx接收请求 
  → 转发到 http://api:5000/xxx (Docker内部网络) 
  → 后端API处理
```

## 优势

1. **环境无关性**：不需要在配置文件中硬编码域名或IP地址
2. **部署灵活性**：可以在任何域名下部署，无需修改配置
3. **开发便利性**：开发环境和生产环境使用相同的相对路径配置
4. **安全性提升**：后端API不直接暴露给外部，通过Nginx统一管理
5. **维护简便**：修改后端API地址只需要修改Nginx配置，前端代码无需改动

## 注意事项

1. **开发环境启动**：
   - 确保后端API先启动在 `https://localhost:49522`
   - 然后启动前端开发服务器
   - Vite代理会自动将 `/api` 请求转发到后端

2. **生产环境部署**：
   - 使用 `docker-compose up -d` 启动所有服务
   - Nginx会自动配置反向代理
   - 前端构建时会使用 `.env.production` 中的配置

3. **跨域问题**：
   - 开发环境：通过Vite代理解决
   - 生产环境：通过Nginx反向代理解决
   - 后端API无需额外配置CORS（因为请求来自同源）

4. **文件上传**：
   - 上传的文件存储在后端的 `wwwroot/uploads` 目录
   - 通过Nginx的 `/uploads/` 路径访问
   - Docker环境中使用volume持久化存储

## 测试验证

### 开发环境测试

```bash
# 1. 启动后端API
cd BackEnd/HailongConsulting.API
dotnet run

# 2. 启动前端门户
cd hailong-protral
npm run dev

# 3. 启动后台管理
cd hailong-admin
npm run dev
```

访问 `http://localhost:3000`，检查API请求是否正常。

### 生产环境测试

```bash
# 1. 构建前端
cd hailong-protral
npm run build

cd ../hailong-admin
npm run build

# 2. 启动Docker服务
cd ..
docker-compose up -d

# 3. 检查服务状态
docker-compose ps
```

访问对应端口，检查服务是否正常：
- 前端门户：`http://localhost:80`
- 后台管理：`http://localhost:8080`
- API服务：`http://localhost:5001`

## 部署脚本调整

部署脚本已经更新以支持相对路径配置：

### Ubuntu 22.04 部署脚本 ([`deploy-ubuntu22.sh`](../../scripts/deployment/legacy/deploy-ubuntu22.sh))

**修改内容：**
1. 移除了构建前端时覆盖 `.env.production` 的代码
2. 在Nginx配置中添加了 `/api` 反向代理配置
3. 前端直接使用项目中的相对路径配置

**关键变更：**
```bash
# 旧版本（已移除）
cat > .env.production <<EOF
VITE_API_BASE_URL=http://$SERVER_IP:5001
EOF

# 新版本
# 使用相对路径配置，无需修改.env.production
print_info "使用相对路径配置（/api）..."
```

### Docker 部署脚本 ([`deploy-ubuntu22-docker.sh`](../../deploy-ubuntu22-docker.sh))

**修改内容：**
1. 移除了构建前端时覆盖 `.env.production` 的代码
2. Docker环境中Nginx配置已包含 `/api` 反向代理
3. 前端直接使用项目中的相对路径配置

**部署流程：**
- 前端构建时使用 `.env.production` 中的 `/api` 配置
- Nginx容器将 `/api` 请求转发到后端API容器
- 无需根据服务器IP修改配置文件

## 相关文件

- [`hailong-protral/.env.production`](../../hailong-protral/.env.production)
- [`hailong-protral/.env.development`](../../hailong-protral/.env.development)
- [`hailong-protral/vite.config.js`](../../hailong-protral/vite.config.js)
- [`hailong-admin/.env.production`](../../hailong-admin/.env.production)
- [`hailong-admin/.env.development`](../../hailong-admin/.env.development)
- [`hailong-admin/vite.config.js`](../../hailong-admin/vite.config.js)
- [`BackEnd/HailongConsulting.API/appsettings.json`](../../BackEnd/HailongConsulting.API/appsettings.json)
- [`nginx/conf.d/default.conf`](../../nginx/conf.d/default.conf)
- [`docker-compose.yml`](../../docker-compose.yml)
- [`deploy-ubuntu22.sh`](../../scripts/deployment/legacy/deploy-ubuntu22.sh)
- [`deploy-ubuntu22-docker.sh`](../../deploy-ubuntu22-docker.sh)

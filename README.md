# 海隆咨询官网项目

河南海隆工程咨询有限公司官方网站 - 前后端分离全栈项目

## 📋 项目概述

**项目名称**: 海隆咨询官网 (Hailong Consulting Official Website)

**项目定位**: 企业形象展示、业务资质宣传、公告信息发布、实用工具提供

**开发状态**: ✅ 已完成并投入使用

**技术架构**: 前后端分离 + 微服务架构

## 🎯 项目目标

- 📢 **信息发布平台** - 发布政府采购、建设工程公告信息
- 🏢 **企业形象展示** - 展示企业资质、荣誉、业绩
- 🛠 **实用工具提供** - 提供招标代理费、造价咨询费等计算工具
- 📊 **数据统计分析** - 统计访问量、公告数据等
- 🔐 **后台管理系统** - 内容管理、用户管理、系统配置

## 🏗 项目架构

```
Protral/
├── BackEnd/                          # 后端项目
│   └── HailongConsulting.API/       # .NET 8 Web API
│       ├── Controllers/             # API控制器
│       ├── Services/                # 业务逻辑层
│       ├── Repositories/            # 数据访问层
│       ├── Models/                  # 数据模型
│       ├── Common/                  # 公共组件
│       ├── Middleware/              # 中间件
│       └── wwwroot/                 # 静态文件
├── hailong-admin/                   # 后台管理系统
│   ├── src/
│   │   ├── api/                     # API接口
│   │   ├── components/              # 公共组件
│   │   ├── views/                   # 页面组件
│   │   ├── router/                  # 路由配置
│   │   └── stores/                  # 状态管理
│   └── package.json
├── hailong-protral/                 # 前端门户网站
│   ├── src/
│   │   ├── api/                     # API接口
│   │   ├── components/              # 公共组件
│   │   ├── views/                   # 页面组件
│   │   ├── router/                  # 路由配置
│   │   └── utils/                   # 工具函数
│   └── package.json
├── SQL/                             # 数据库脚本
│   ├── hailong_consulting_schema.sql      # 数据库结构
│   ├── hailong_consulting_init_data.sql   # 初始数据
│   └── README.md                    # 数据库文档
└── README.md                        # 项目说明（本文件）
```

## 🛠 技术栈

### 后端技术

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 | Web API框架 |
| Entity Framework Core | 8.0.0 | ORM框架 |
| MySQL | 8.0+ | 关系型数据库 |
| JWT | 8.0.0 | 身份认证 |
| AutoMapper | 13.0.1 | 对象映射 |
| Serilog | 8.0.0 | 日志记录 |
| Swagger | 6.5.0 | API文档 |

### 前端技术

| 技术 | 版本 | 用途 |
|------|------|------|
| Vue | 3.4.0 | 前端框架 |
| Vite | 5.0.0 | 构建工具 |
| Element Plus | 2.5.0 | UI组件库（后台） |
| Tailwind CSS | 3.4.0 | CSS框架（门户） |
| Vue Router | 4.2.5 | 路由管理 |
| Pinia | 2.1.7 | 状态管理 |
| Axios | 1.6.2 | HTTP客户端 |
| WangEditor | 5.1.23 | 富文本编辑器 |
| ECharts | 6.0.0 | 数据可视化 |

## 📦 项目模块

### 1. 后端API (HailongConsulting.API)

**技术**: .NET 8 Web API + Entity Framework Core + MySQL

**主要功能**:
- ✅ JWT身份认证和授权
- ✅ RESTful API设计
- ✅ 统一响应格式
- ✅ 全局异常处理
- ✅ 系统日志记录
- ✅ 文件上传管理
- ✅ 分页查询支持
- ✅ 全文搜索功能

**核心模块**:
- 认证授权模块
- 公告管理模块
- 信息发布模块
- 附件管理模块
- 系统配置模块
- 统计分析模块
- 用户管理模块
- 系统日志模块

📖 [查看详细文档](BackEnd/HailongConsulting.API/README.md)

### 2. 后台管理系统 (hailong-admin)

**技术**: Vue 3 + Element Plus + Vite

**主要功能**:
- ✅ 用户登录认证
- ✅ 公告管理（政府采购、建设工程）
- ✅ 信息发布管理（公司新闻、政策法规）
- ✅ 附件管理
- ✅ 系统配置（轮播图、企业信息、友情链接等）
- ✅ 用户管理
- ✅ 系统日志查询
- ✅ 数据统计分析
- ✅ 富文本编辑
- ✅ 文件上传

**页面模块**:
- 登录页
- 首页仪表盘
- 公告管理
- 信息发布
- 附件管理
- 系统配置
- 用户管理
- 系统日志

📖 [查看详细文档](hailong-admin/README.md)

### 3. 前端门户网站 (hailong-protral)

**技术**: Vue 3 + Tailwind CSS + Vite

**主要功能**:
- ✅ 响应式设计（支持PC、平板、手机）
- ✅ 首页展示（轮播图、企业简介、业务范围、数据统计）
- ✅ 关于我们（企业简介、资质、荣誉、业绩）
- ✅ 公告信息（政府采购、建设工程）
- ✅ 信息发布（公司新闻、政策法规）
- ✅ 全局搜索
- ✅ 实用工具（招标代理费、造价咨询费、司法鉴定费计算器）
- ✅ 联系我们
- ✅ SEO优化

**页面模块**:
- 首页
- 关于我们
- 公告信息
- 信息发布
- 搜索结果
- 实用工具
- 联系我们

📖 [查看详细文档](hailong-protral/README.md)

### 4. 数据库 (MySQL)

**数据库名**: hailong_consulting

**主要表**:
- `users` - 用户表
- `announcements` - 统一公告表
- `info_publications` - 统一信息发布表
- `attachments` - 附件表
- `region_dictionary` - 区域字典表
- `system_logs` - 系统日志表
- `carousel_banners` - 轮播图表
- `company_profile` - 企业简介表
- `business_scope` - 业务范围表
- `company_qualifications` - 企业资质表
- `company_honors` - 企业荣誉表
- `major_achievements` - 重要业绩表
- `friendly_links` - 友情链接表
- `visit_statistics` - 访问统计表

📖 [查看详细文档](SQL/README.md)

## 🚀 快速开始

### 环境要求

- **Node.js** >= 18.0
- **.NET SDK** >= 8.0
- **MySQL** >= 8.0
- **Git**

### 1. 克隆项目

```bash
git clone <repository-url>
cd Protral
```

### 2. 数据库初始化

```bash
# 进入SQL目录
cd SQL

# 创建数据库结构
mysql -u root -p < hailong_consulting_schema.sql

# 导入初始数据
mysql -u root -p < hailong_consulting_init_data.sql
```

### 3. 后端API启动

```bash
# 进入后端目录
cd BackEnd/HailongConsulting.API

# 配置数据库连接（编辑appsettings.json）
# 修改ConnectionStrings.DefaultConnection

# 安装依赖
dotnet restore

# 运行项目
dotnet run

# 访问Swagger文档
# http://localhost:5000
```

### 4. 后台管理系统启动

```bash
# 进入后台管理目录
cd hailong-admin

# 安装依赖
npm install

# 配置API地址（编辑.env.development）
# VITE_API_BASE_URL=http://localhost:5000

# 启动开发服务器
npm run dev

# 访问后台管理系统
# http://localhost:3000
# 默认账号: admin / Admin@123456
```

### 5. 前端门户启动

```bash
# 进入前端门户目录
cd hailong-protral

# 安装依赖
npm install

# 配置API地址（编辑.env.development）
# VITE_API_BASE_URL=http://localhost:5000

# 启动开发服务器
npm run dev

# 访问前端门户
# http://localhost:5173
```

## 📱 核心功能

### 1. 公告信息管理

#### 政府采购公告
- 公告类型：采购公告、更正公告、结果公告
- 支持富文本编辑
- 支持附件上传
- 支持区域筛选（省市区三级）
- 支持关键词搜索
- 自动统计访问量

#### 建设工程公告
- 公告类型：招标公告、更正公告、中标公告
- 功能同政府采购公告

### 2. 信息发布管理

#### 公司新闻
- 分类：公司新闻、行业动态、通知公告
- 支持封面图片
- 支持富文本内容
- 支持附件上传

#### 政策法规
- 分类：法律法规、行政法规、地方政策
- 支持文件下载
- 支持富文本内容

### 3. 系统配置管理

- 轮播图管理
- 企业简介管理
- 业务范围管理
- 企业资质管理
- 企业荣誉管理
- 重要业绩管理
- 友情链接管理

### 4. 实用工具

#### 招标代理费计算器
- 依据：豫招协〔2023〕002号
- 支持工程、货物、服务三种类型
- 分档累进计费
- 支持优惠比例设置

#### 造价咨询费计算器
- 依据：豫价协〔2022〕6号
- 支持多种工程类型
- 分段计费

#### 司法鉴定费计算器
- 依据：《司法鉴定收费管理办法》
- 分段累进计费

### 5. 全局搜索

- 支持关键词搜索
- 支持业务类型筛选
- 支持时间范围筛选
- 支持区域筛选
- 关键词高亮显示

### 6. 统计分析

- 访问量统计
- 公告数据统计
- 用户行为分析
- 数据可视化展示

## 🔐 安全特性

- ✅ JWT Token认证
- ✅ 密码SHA256加密
- ✅ SQL注入防护
- ✅ XSS攻击防护
- ✅ CSRF防护
- ✅ 文件上传类型验证
- ✅ 操作日志记录
- ✅ 敏感信息加密

## 📊 性能优化

- ✅ 数据库索引优化
- ✅ 全文搜索索引
- ✅ 分页查询
- ✅ 路由懒加载
- ✅ 图片懒加载
- ✅ 组件按需引入
- ✅ Gzip压缩
- ✅ 浏览器缓存策略

## 🚀 部署指南

项目采用 Ubuntu 22.04 + Docker Compose 部署，后端为 .NET 8。

- 部署入口：根目录 `deploy-ubuntu22-docker.sh`。
- [Ubuntu22 Docker 部署指南](docs/deployment/ubuntu22-docker.md)：安装、8082 默认端口、更新、验证与回退。
- [配置存储位置](docs/configuration/storage.md)：部署参数、展示默认值与运行密钥。
- [全部文档](docs/README.md)。

## 📝 开发规范

### 代码规范

- **后端**: 遵循C#编码规范
- **前端**: 遵循Vue 3 + ESLint规范
- **数据库**: 遵循MySQL命名规范

### Git提交规范

```
feat: 新功能
fix: 修复bug
docs: 文档更新
style: 代码格式调整
refactor: 代码重构
test: 测试相关
chore: 构建/工具链相关
```

### 分支管理

- `main` - 主分支（生产环境）
- `develop` - 开发分支
- `feature/*` - 功能分支
- `hotfix/*` - 紧急修复分支

## 🐛 故障排查

### 常见问题

**1. 数据库连接失败**
- 检查MySQL服务状态
- 验证连接字符串
- 检查防火墙设置

**2. API请求失败**
- 检查后端服务状态
- 验证CORS配置
- 检查网络连接

**3. 前端页面空白**
- 检查控制台错误
- 验证API地址配置
- 清除浏览器缓存

**4. 文件上传失败**
- 检查文件大小限制
- 验证文件类型
- 检查目录权限

## 📚 相关文档

- [后端API文档](BackEnd/HailongConsulting.API/README.md)
- [后台管理文档](hailong-admin/README.md)
- [前端门户文档](hailong-protral/README.md)
- [门户站点配置说明](docs/portal-settings.md)
- [数据库文档](SQL/README.md)

## 👥 团队成员

- **项目经理**: 负责项目整体规划和协调
- **后端开发**: .NET开发团队
- **前端开发**: Vue开发团队
- **数据库设计**: DBA团队
- **UI/UX设计**: 设计团队
- **测试团队**: QA团队

## 📞 联系方式

**公司名称**: 河南海隆工程咨询有限公司

**联系电话**: 0371-55894666

**公司地址**: 河南省郑州市郑东新区金水东路雅宝·东方国际广场1号楼8层

**技术支持**: support@hailongzixun.com

## 📄 许可证

Copyright © 2025 河南海隆工程咨询有限公司

本项目为海隆咨询官网专有项目，未经授权不得用于商业用途。

---

**项目版本**: v1.0.0  
**最后更新**: 2025年12月16日  
**维护团队**: 海隆咨询技术部

## 🎯 未来规划

- [ ] 移动端APP开发
- [ ] 微信小程序开发
- [ ] 在线支付功能
- [ ] 智能客服系统
- [ ] 数据分析平台
- [ ] API开放平台

---

**⭐ 如果这个项目对您有帮助，请给我们一个Star！**

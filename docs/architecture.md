# 🏗️ 海龙咨询官网 - 系统架构设计文档

## 📋 目录

- [系统概述](#系统概述)
- [架构设计](#架构设计)
- [技术选型](#技术选型)
- [数据库设计](#数据库设计)
- [接口设计](#接口设计)
- [安全架构](#安全架构)
- [性能优化](#性能优化)
- [扩展性设计](#扩展性设计)

---

## 🎯 系统概述

### 项目定位
海龙咨询官网是一个面向政府采购和工程建设领域的企业门户网站，提供信息发布、公告展示、数据统计等功能。

### 核心目标
- ✅ 高性能：支持高并发访问
- ✅ 高可用：7x24小时稳定运行
- ✅ 易维护：模块化设计，便于扩展
- ✅ 安全性：数据安全和访问控制

### 系统特点
- 前后端分离架构
- RESTful API设计
- 响应式Web设计
- 模块化组件开发

---

## 🏛️ 架构设计

### 整体架构

```
┌─────────────────────────────────────────────────────────────┐
│                         用户层                               │
├─────────────────────────────────────────────────────────────┤
│  前台门户 (Vue3)          │      后台管理 (Vue3)            │
│  - 信息浏览               │      - 内容管理                 │
│  - 公告查询               │      - 用户管理                 │
│  - 数据展示               │      - 系统配置                 │
└─────────────────────────────────────────────────────────────┘
                              ↓ HTTPS
┌─────────────────────────────────────────────────────────────┐
│                      Nginx 反向代理                          │
│  - 负载均衡                                                  │
│  - SSL终止                                                   │
│  - 静态资源服务                                              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    应用层 (ASP.NET Core 8)                   │
├─────────────────────────────────────────────────────────────┤
│  Controllers (控制器层)                                      │
│  ├─ AuthController          - 认证授权                      │
│  ├─ AnnouncementController  - 公告管理                      │
│  ├─ InfoPublicationController - 信息公开                    │
│  ├─ ConfigController        - 系统配置                      │
│  └─ StatisticsController    - 数据统计                      │
├─────────────────────────────────────────────────────────────┤
│  Services (业务逻辑层)                                       │
│  ├─ AuthService             - 认证服务                      │
│  ├─ AnnouncementService     - 公告服务                      │
│  ├─ InfoPublicationService  - 信息服务                      │
│  ├─ ConfigService           - 配置服务                      │
│  └─ StatisticsService       - 统计服务                      │
├─────────────────────────────────────────────────────────────┤
│  Repositories (数据访问层)                                   │
│  ├─ IRepository<T>          - 通用仓储接口                  │
│  ├─ Repository<T>           - 通用仓储实现                  │
│  └─ IUnitOfWork             - 工作单元                      │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    数据层 (MySQL 8.0)                        │
│  - 业务数据存储                                              │
│  - 事务管理                                                  │
│  - 数据备份                                                  │
└─────────────────────────────────────────────────────────────┘
```

### 分层架构说明

#### 1. 表现层 (Presentation Layer)
- **前台门户**: Vue3 + Element Plus
- **后台管理**: Vue3 + Element Plus
- **职责**: 用户交互、数据展示、表单验证

#### 2. 网关层 (Gateway Layer)
- **Nginx**: 反向代理、负载均衡、SSL终止
- **职责**: 请求路由、静态资源服务、安全防护

#### 3. 应用层 (Application Layer)
- **Controllers**: 接收HTTP请求，调用服务层
- **Middleware**: 异常处理、日志记录、认证授权
- **职责**: 请求处理、响应封装、权限验证

#### 4. 业务逻辑层 (Business Logic Layer)
- **Services**: 实现业务逻辑
- **DTOs**: 数据传输对象
- **职责**: 业务规则、数据转换、业务流程

#### 5. 数据访问层 (Data Access Layer)
- **Repositories**: 数据访问抽象
- **Entity Framework Core**: ORM框架
- **职责**: 数据CRUD、查询优化、事务管理

#### 6. 数据层 (Data Layer)
- **MySQL**: 关系型数据库
- **职责**: 数据持久化、数据完整性、备份恢复

---

## 🔧 技术选型

### 后端技术栈

| 技术 | 版本 | 用途 | 选型理由 |
|------|------|------|----------|
| ASP.NET Core | 7.0 | Web框架 | 高性能、跨平台、成熟稳定 |
| Entity Framework Core | 7.0 | ORM框架 | 强大的数据访问能力 |
| MySQL | 8.0 | 数据库 | 开源、高性能、社区活跃 |
| JWT | - | 认证授权 | 无状态、易扩展 |
| AutoMapper | 12.0 | 对象映射 | 简化DTO转换 |
| Serilog | 3.0 | 日志框架 | 结构化日志、多种输出 |
| Swagger | 6.5 | API文档 | 自动生成、在线测试 |

### 前端技术栈

| 技术 | 版本 | 用途 | 选型理由 |
|------|------|------|----------|
| Vue.js | 3.3 | 前端框架 | 响应式、组件化、生态丰富 |
| Vue Router | 4.2 | 路由管理 | 官方路由方案 |
| Pinia | 2.1 | 状态管理 | Vue3官方推荐 |
| Element Plus | 2.4 | UI组件库 | 企业级、组件丰富 |
| Axios | 1.6 | HTTP客户端 | 拦截器、Promise支持 |
| ECharts | 5.4 | 数据可视化 | 功能强大、图表丰富 |
| Vite | 5.0 | 构建工具 | 快速、现代化 |

### 基础设施

| 技术 | 版本 | 用途 |
|------|------|------|
| Nginx | 1.24 | Web服务器、反向代理 |
| Let's Encrypt | - | SSL证书 |
| Linux | Ubuntu 22.04 | 服务器操作系统 |
| Git | 2.40 | 版本控制 |

---

## 💾 数据库设计

### ER图

```
┌─────────────────┐
│     Users       │
│─────────────────│
│ Id (PK)         │
│ Username        │
│ PasswordHash    │
│ Email           │
│ Role            │
└─────────────────┘
         │
         │ 1:N
         ↓
┌─────────────────┐       ┌─────────────────┐
│  Announcements  │       │ InfoPublications│
│─────────────────│       │─────────────────│
│ Id (PK)         │       │ Id (PK)         │
│ Title           │       │ Title           │
│ Content         │       │ Content         │
│ Type            │       │ Category        │
│ PublishDate     │       │ PublishDate     │
│ CreatedBy (FK)  │       │ CreatedBy (FK)  │
└─────────────────┘       └─────────────────┘
         │                         │
         │ 1:N                     │ 1:N
         ↓                         ↓
┌─────────────────────────────────────┐
│           Attachments               │
│─────────────────────────────────────│
│ Id (PK)                             │
│ FileName                            │
│ FilePath                            │
│ FileType                            │
│ RelatedId (FK)                      │
│ RelatedType                         │
└─────────────────────────────────────┘
```

### 核心表设计

#### 1. Users (用户表)
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100),
    Role VARCHAR(20) NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_username (Username),
    INDEX idx_role (Role)
);
```

#### 2. Announcements (公告表)
```sql
CREATE TABLE Announcements (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Title VARCHAR(200) NOT NULL,
    Content TEXT NOT NULL,
    Type VARCHAR(50) NOT NULL,
    Region VARCHAR(100),
    PublishDate DATETIME NOT NULL,
    Source VARCHAR(100),
    ViewCount INT DEFAULT 0,
    IsPublished BOOLEAN DEFAULT TRUE,
    CreatedBy INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    INDEX idx_type (Type),
    INDEX idx_publish_date (PublishDate),
    INDEX idx_region (Region),
    FULLTEXT INDEX ft_title_content (Title, Content)
);
```

### 索引策略

1. **主键索引**: 所有表的Id字段
2. **唯一索引**: Username、Email等唯一字段
3. **普通索引**: 常用查询字段（Type、Region、PublishDate）
4. **全文索引**: Title、Content字段（支持搜索）
5. **复合索引**: 多字段组合查询

---

## 🔌 接口设计

### RESTful API设计原则

1. **资源导向**: URL表示资源，HTTP方法表示操作
2. **统一响应**: 使用ApiResponse统一封装
3. **版本控制**: 通过URL路径管理版本
4. **状态码**: 正确使用HTTP状态码

### API响应格式

```json
{
  "success": true,
  "message": "操作成功",
  "data": {
    // 业务数据
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 分页响应格式

```json
{
  "success": true,
  "data": {
    "items": [],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 10
  }
}
```

### API端点设计

#### 认证相关
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/logout` - 用户登出
- `POST /api/auth/refresh` - 刷新Token

#### 公告管理
- `GET /api/announcements` - 获取公告列表
- `GET /api/announcements/{id}` - 获取公告详情
- `POST /api/announcements` - 创建公告
- `PUT /api/announcements/{id}` - 更新公告
- `DELETE /api/announcements/{id}` - 删除公告

#### 信息公开
- `GET /api/info-publications` - 获取信息列表
- `GET /api/info-publications/{id}` - 获取信息详情
- `POST /api/info-publications` - 创建信息
- `PUT /api/info-publications/{id}` - 更新信息
- `DELETE /api/info-publications/{id}` - 删除信息

---

## 🔒 安全架构

### 认证授权

#### JWT认证流程
```
1. 用户登录 → 验证用户名密码
2. 生成JWT Token (包含用户信息、角色、过期时间)
3. 返回Token给客户端
4. 客户端在请求头中携带Token
5. 服务端验证Token有效性
6. 授权访问资源
```

#### Token结构
```json
{
  "sub": "user_id",
  "username": "admin",
  "role": "Admin",
  "exp": 1704067200,
  "iat": 1704063600
}
```

### 数据安全

1. **密码加密**: BCrypt哈希算法
2. **SQL注入防护**: 参数化查询
3. **XSS防护**: 输入验证和输出编码
4. **CSRF防护**: Token验证
5. **HTTPS**: 全站SSL加密

### 权限控制

```csharp
// 基于角色的访问控制 (RBAC)
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // 只有Admin角色可以访问
}

[Authorize(Roles = "Admin,Editor")]
public class ContentController : ControllerBase
{
    // Admin和Editor角色可以访问
}
```

### 日志审计

- **操作日志**: 记录所有增删改操作
- **访问日志**: 记录API访问情况
- **错误日志**: 记录系统异常
- **安全日志**: 记录登录、权限变更

---

## ⚡ 性能优化

### 数据库优化

1. **索引优化**
   - 为常用查询字段添加索引
   - 使用复合索引优化多字段查询
   - 定期分析和优化索引

2. **查询优化**
   - 使用分页查询避免大数据量
   - 避免N+1查询问题
   - 使用Include预加载关联数据

3. **连接池**
   - 配置合理的连接池大小
   - 复用数据库连接

### 应用层优化

1. **缓存策略**
   - 内存缓存热点数据
   - 响应缓存静态内容
   - 分布式缓存（Redis）

2. **异步处理**
   - 使用async/await异步编程
   - 后台任务处理耗时操作

3. **资源压缩**
   - Gzip压缩响应内容
   - 静态资源压缩和合并

### 前端优化

1. **代码分割**
   - 路由懒加载
   - 组件按需加载

2. **资源优化**
   - 图片压缩和懒加载
   - CDN加速静态资源

3. **缓存策略**
   - 浏览器缓存
   - Service Worker

### 服务器优化

1. **Nginx配置**
   - 启用Gzip压缩
   - 配置缓存策略
   - 负载均衡

2. **系统调优**
   - 调整文件描述符限制
   - 优化TCP参数

---

## 🔄 扩展性设计

### 水平扩展

```
┌─────────────┐
│   Nginx     │ (负载均衡)
└─────────────┘
       │
   ┌───┴───┬───────┬───────┐
   │       │       │       │
┌──▼──┐ ┌──▼──┐ ┌──▼──┐ ┌──▼──┐
│API-1│ │API-2│ │API-3│ │API-N│
└─────┘ └─────┘ └─────┘ └─────┘
   │       │       │       │
   └───┬───┴───────┴───────┘
       │
┌──────▼──────┐
│   MySQL     │ (主从复制)
└─────────────┘
```

### 模块化设计

1. **业务模块独立**
   - 公告模块
   - 信息公开模块
   - 配置模块
   - 统计模块

2. **服务解耦**
   - 通过接口定义服务契约
   - 依赖注入实现松耦合

3. **插件化架构**
   - 支持功能模块动态加载
   - 扩展点设计

### 微服务演进

未来可以拆分为微服务架构：

```
API Gateway
    │
    ├─ 认证服务
    ├─ 公告服务
    ├─ 信息服务
    ├─ 配置服务
    └─ 统计服务
```

---

## 📊 监控和运维

### 监控指标

1. **系统指标**
   - CPU使用率
   - 内存使用率
   - 磁盘IO
   - 网络流量

2. **应用指标**
   - API响应时间
   - 请求成功率
   - 错误率
   - 并发用户数

3. **业务指标**
   - 访问量统计
   - 用户活跃度
   - 内容发布量

### 日志管理

```
应用日志 → Serilog → 文件/数据库
                    ↓
              日志分析工具
                    ↓
              告警和报表
```

---

## 🎯 设计模式应用

### 1. 仓储模式 (Repository Pattern)
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 2. 工作单元模式 (Unit of Work)
```csharp
public interface IUnitOfWork : IDisposable
{
    IAnnouncementRepository Announcements { get; }
    IInfoPublicationRepository InfoPublications { get; }
    Task<int> SaveChangesAsync();
}
```

### 3. 依赖注入 (Dependency Injection)
```csharp
services.AddScoped<IAnnouncementService, AnnouncementService>();
services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
```

### 4. 中间件模式 (Middleware Pattern)
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SystemLogMiddleware>();
```

---

## 📚 相关文档

- [README.md](../README.md) - 项目总览
- [Docker 部署指南](deployment/ubuntu22-docker.md) - 部署指南
- [TROUBLESHOOTING.md](operations/troubleshooting.md) - 故障排查
- [MAINTENANCE.md](operations/maintenance.md) - 运维手册
- [USER_MANUAL.md](user-manual.md) - 用户手册

---

## 📝 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| 1.0 | 2024-01-01 | 初始版本 |

---

**文档维护**: 开发团队  
**最后更新**: 2024-01-01
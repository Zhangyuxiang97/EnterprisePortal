# 海隆咨询官网后台管理系统

基于 Vue 3 + Element Plus 开发的现代化后台管理系统。

## 📋 项目概述

**项目名称**: 海隆咨询官网后台管理系统 (hailong-admin)

**技术架构**: Vue 3 + Vite + Element Plus

**开发状态**: 本地开发与预览；正式部署按根目录 Ubuntu 22 Docker 部署指南执行。

## 后台维护与验证

- 菜单按公告管理、信息发布、附件管理、门户设置、企业资料、统计分析、系统管理组织；轮播图与友情链接独立编辑页暂未开放入口。
- `components/RichEditor.vue` 统一处理历史 HTML 载入、图片上传和隔离正文预览；历史正文只有实际编辑后才转为编辑器格式。
- `composables/useEditorForm.js` 统一处理关闭、路由离开、刷新提醒及上传期间的保存限制；`utils/form.js` 提供本地时间、有效正文和地区树筛选。
- 上传格式和体积限制读取 `/api/attachments/upload-options`，默认后端上限为 10 MB。附件“引用”可查看已保存内容的使用位置，有引用的文件不能直接删除。
- 资质展示开关与证书有效期分别显示。日期、编号和荣誉级别没有明确依据时留空，长期证书在描述中注明。
- 公告数量按记录计数，不代表去重后的项目数；金额是已记录的结果公告中标 / 成交金额，可能包含同项目的多条结果记录。发布时间趋势按启用记录的 `PublishTime` 统计，“今日入库”按创建时间统计。

```bash
npm ci
npm test
npm run build
```

浏览器回归使用本机已初始化的测试数据库和 Chrome，需先启动 API 和管理端。设置环境变量 `ADMIN_TEST_USERNAME`、`ADMIN_TEST_PASSWORD` 后运行 `npm run test:browser`；可用 `ADMIN_TEST_URL` / `ADMIN_TEST_API` 调整本机地址，默认分别为 `http://127.0.0.1:3002` 和 `http://127.0.0.1:5000/api`。脚本拒绝连接非本机域名，不输出凭据。

该回归会验证现有初始化公告与扫描通知，按原值保存资质/荣誉，并创建临时政策、地区、普通用户及文本附件，最后清理本轮创建的记录。服务端附件使用软删除，因此上传的测试文件仍按既有存储保留规则处理。不要针对业务库运行；测试代码位于 `tests/admin.smoke.mjs`。

## 🛠 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| Vue | 3.4.0 | 前端框架 |
| Vite | 5.0.0 | 构建工具 |
| Element Plus | 2.5.0 | UI组件库 |
| Pinia | 2.1.7 | 状态管理 |
| Vue Router | 4.2.5 | 路由管理 |
| Axios | 1.6.2 | HTTP客户端 |
| WangEditor | 5.1.23 | 富文本编辑器 |
| ECharts | 6.0.0 | 数据可视化 |

## 📁 项目结构

```
hailong-admin/
├── public/                          # 静态资源
├── src/
│   ├── api/                        # API接口封装
│   │   ├── request.js              # Axios封装（拦截器、错误处理）
│   │   ├── index.js                # API统一导出
│   │   ├── auth.js                 # 认证相关API
│   │   ├── announcement.js         # 公告管理API
│   │   ├── infoPublication.js      # 信息发布API
│   │   ├── attachment.js           # 附件管理API
│   │   ├── systemConfig.js         # 系统配置API
│   │   ├── statistics.js           # 统计分析API
│   │   ├── system.js               # 系统管理API
│   │   └── user.js                 # 用户管理API
│   ├── assets/                     # 资源文件
│   │   ├── logo.png                # Logo图片
│   │   └── hailong.ico             # 网站图标
│   ├── components/                 # 公共组件
│   │   ├── Header.vue              # 顶部导航栏
│   │   ├── Sidebar.vue             # 侧边栏导航
│   │   ├── RichEditor.vue          # 富文本编辑器
│   │   ├── FileUpload.vue          # 文件上传组件
│   │   ├── RegionSelector.vue      # 区域选择器
│   │   └── RegionCascader.vue      # 区域级联选择器
│   ├── config/                     # 配置文件
│   │   └── api.config.js           # API配置
│   ├── router/                     # 路由配置
│   │   └── index.js                # 路由定义 + 权限守卫
│   ├── stores/                     # Pinia状态管理
│   │   └── user.js                 # 用户状态
│   ├── utils/                      # 工具函数
│   │   ├── auth.js                 # Token存储工具
│   │   ├── date.js                 # 日期格式化工具
│   │   └── chartOptions.js         # 图表配置工具
│   ├── views/                      # 页面组件
│   │   ├── Login.vue               # 登录页
│   │   ├── Layout.vue              # 主框架布局
│   │   ├── Dashboard.vue           # 首页仪表盘
│   │   ├── announcements/          # 公告管理
│   │   │   ├── GovProcurement.vue  # 政府采购公告
│   │   │   └── Construction.vue    # 建设工程公告
│   │   ├── infoPublications/       # 信息发布
│   │   │   ├── CompanyNews.vue     # 公司新闻
│   │   │   └── PolicyRegulation.vue # 政策法规
│   │   ├── attachments/            # 附件管理
│   │   │   └── AttachmentList.vue  # 附件列表
│   │   ├── config/                 # 系统配置
│   │   │   ├── Banners.vue         # 轮播图管理
│   │   │   ├── CompanyProfile.vue  # 企业简介
│   │   │   ├── BusinessScope.vue   # 业务范围
│   │   │   ├── Qualifications.vue  # 企业资质
│   │   │   ├── Honors.vue          # 企业荣誉
│   │   │   ├── Achievements.vue    # 重要业绩
│   │   │   └── FriendlyLinks.vue   # 友情链接
│   │   ├── system/                 # 系统管理
│   │   │   ├── Users.vue           # 用户管理
│   │   │   ├── SystemLogs.vue      # 系统日志
│   │   │   └── Profile.vue         # 个人资料
│   │   └── statistics/             # 统计分析
│   │       └── Dashboard.vue       # 数据统计
│   ├── App.vue                     # 根组件
│   ├── main.js                     # 入口文件
│   └── style.css                   # 全局样式
├── .env.development                # 开发环境配置
├── .env.production                 # 生产环境配置
├── .gitignore                      # Git忽略文件
├── index.html                      # HTML模板
├── package.json                    # 项目依赖
├── vite.config.js                  # Vite配置
└── README.md                       # 项目说明
```

## 🚀 快速开始

### 1. 环境要求

- **Node.js** >= 18.0
- **npm** >= 9.0 或 **pnpm** >= 8.0

### 2. 安装依赖

```bash
cd hailong-admin
npm install
```

或使用 pnpm:

```bash
pnpm install
```

### 3. 配置后端API地址

编辑 `.env.development` 文件：

```env
# 开发环境API地址
VITE_API_BASE_URL=/api
```

编辑 `.env.production` 文件：

```env
# 生产环境API地址
VITE_API_BASE_URL=https://api.yourdomain.com
```

### 4. 启动开发服务器

```bash
npm run dev
```

访问 http://localhost:3000

### 5. 构建生产版本

```bash
npm run build
```

构建产物位于 `dist/` 目录。

### 6. 预览生产构建

```bash
npm run preview
```

## 🔑 默认登录账号

```
用户名: admin
密码: Admin@123456
```

⚠️ **安全提示**: 首次登录后请立即修改密码！

## 📱 功能模块

### 1. 用户认证

- ✅ 登录（用户名/密码）
- ✅ Token自动管理（localStorage）
- ✅ 请求拦截器自动添加Authorization头
- ✅ 401自动跳转登录页
- ✅ 修改密码
- ✅ 退出登录

### 2. 首页仪表盘

- ✅ 数据统计卡片（总项目数、总用户数、今日访问量等）
- ✅ 访问趋势图表（ECharts）
- ✅ 公告统计图表
- ✅ 最新公告列表
- ✅ 快捷操作入口

### 3. 公告管理

#### 政府采购公告
- ✅ 列表展示（分页、搜索、筛选）
- ✅ 新增公告（富文本编辑器）
- ✅ 编辑公告
- ✅ 删除公告（软删除）
- ✅ 批量操作
- ✅ 附件上传管理
- ✅ 区域选择（省市区三级联动）
- ✅ 公告类型选择
- ✅ 预览功能

#### 建设工程公告
- ✅ 功能同政府采购公告
- ✅ 独立的公告类型配置

### 4. 信息发布管理

#### 公司新闻
- ✅ 新闻列表（分页、搜索）
- ✅ 新增/编辑新闻
- ✅ 富文本内容编辑
- ✅ 封面图片上传
- ✅ 附件管理
- ✅ 置顶功能
- ✅ 发布/下架

#### 政策法规
- ✅ 法规列表管理
- ✅ 分类管理
- ✅ 富文本编辑
- ✅ 附件上传
- ✅ 发布时间设置

### 5. 附件管理

- ✅ 附件列表展示
- ✅ 按类型筛选（图片/文档/其他）
- ✅ 按关联类型筛选
- ✅ 附件预览
- ✅ 附件下载
- ✅ 批量删除
- ✅ 存储空间统计

### 6. 系统配置

#### 轮播图管理
- ✅ 轮播图列表
- ✅ 新增/编辑轮播图
- ✅ 图片上传（推荐尺寸：1920x600）
- ✅ 标题、描述、链接设置
- ✅ 排序调整（拖拽排序）
- ✅ 启用/禁用

#### 企业简介
- ✅ 富文本编辑
- ✅ 图片上传
- ✅ 企业特色标签管理
- ✅ 实时预览

#### 业务范围
- ✅ 业务列表管理
- ✅ 业务图标上传
- ✅ 业务特点编辑
- ✅ 排序管理

#### 企业资质
- ✅ 资质证书管理
- ✅ 证书图片上传
- ✅ 证书信息编辑
- ✅ 有效期管理

#### 企业荣誉
- ✅ 荣誉列表管理
- ✅ 荣誉证书上传
- ✅ 荣誉级别设置
- ✅ 获奖日期管理

#### 重要业绩
- ✅ 业绩项目管理
- ✅ 项目图片上传
- ✅ 项目金额设置
- ✅ 完成日期管理

#### 友情链接
- ✅ 链接列表管理
- ✅ 链接分类
- ✅ Logo上传
- ✅ 排序管理

### 7. 系统管理

#### 用户管理
- ✅ 用户列表
- ✅ 新增/编辑用户
- ✅ 角色分配
- ✅ 启用/禁用用户
- ✅ 重置密码

#### 系统日志
- ✅ 操作日志查询
- ✅ 按用户筛选
- ✅ 按操作类型筛选
- ✅ 按时间范围筛选
- ✅ 日志详情查看
- ✅ 日志导出

#### 个人资料
- ✅ 查看个人信息
- ✅ 修改密码
- ✅ 修改邮箱
- ✅ 修改手机号

### 8. 统计分析

- ✅ 访问统计（日/周/月）
- ✅ 公告统计
- ✅ 用户行为分析
- ✅ 数据可视化图表
- ✅ 数据导出

## 🔌 API调用示例

### 登录

```javascript
import { authApi } from '@/api'

const login = async () => {
  try {
    const res = await authApi.login({
      username: 'admin',
      password: 'Admin@123456'
    })
    
    if (res.success) {
      console.log('登录成功', res.data)
      // Token已自动存储到localStorage
    }
  } catch (error) {
    console.error('登录失败', error)
  }
}
```

### 获取公告列表

```javascript
import { announcementApi } from '@/api'

const getAnnouncements = async () => {
  try {
    const res = await announcementApi.getList({
      businessType: 'GOV_PROCUREMENT',
      keyword: '招标',
      pageIndex: 1,
      pageSize: 10
    })
    
    if (res.success) {
      console.log('公告列表', res.data.items)
      console.log('总数', res.data.totalCount)
    }
  } catch (error) {
    console.error('获取失败', error)
  }
}
```

### 上传附件

```javascript
import { attachmentApi } from '@/api'

const uploadFile = async (file) => {
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('category', 'image')
    formData.append('relatedType', 'announcement')
    
    const res = await attachmentApi.upload(formData)
    
    if (res.success) {
      console.log('上传成功', res.data)
      return res.data.id
    }
  } catch (error) {
    console.error('上传失败', error)
  }
}
```

### 创建公告

```javascript
import { announcementApi } from '@/api'

const createAnnouncement = async () => {
  try {
    const res = await announcementApi.create({
      title: '招标公告标题',
      businessType: 'GOV_PROCUREMENT',
      noticeType: 'bidding',
      content: '<p>公告内容...</p>',
      province: '河南省',
      city: '郑州市',
      district: '金水区',
      bidder: '招标单位',
      publishTime: '2025-12-16 12:00:00',
      attachmentIds: [1, 2, 3]
    })
    
    if (res.success) {
      console.log('创建成功', res.data)
    }
  } catch (error) {
    console.error('创建失败', error)
  }
}
```

## 🎨 组件使用示例

### 富文本编辑器

```vue
<template>
  <RichEditor v-model="content" :height="400" />
</template>

<script setup>
import { ref } from 'vue'
import RichEditor from '@/components/RichEditor.vue'

const content = ref('<p>初始内容</p>')
</script>
```

### 文件上传

```vue
<template>
  <FileUpload
    :file-list="fileList"
    :max-count="5"
    accept="image/*"
    @success="handleUploadSuccess"
    @remove="handleRemove"
  />
</template>

<script setup>
import { ref } from 'vue'
import FileUpload from '@/components/FileUpload.vue'

const fileList = ref([])

const handleUploadSuccess = (file) => {
  fileList.value.push(file)
}

const handleRemove = (file) => {
  const index = fileList.value.findIndex(f => f.id === file.id)
  if (index > -1) {
    fileList.value.splice(index, 1)
  }
}
</script>
```

### 区域选择器

```vue
<template>
  <RegionCascader
    v-model:province="province"
    v-model:city="city"
    v-model:district="district"
  />
</template>

<script setup>
import { ref } from 'vue'
import RegionCascader from '@/components/RegionCascader.vue'

const province = ref('')
const city = ref('')
const district = ref('')
</script>
```

## 🔧 开发指南

### 添加新页面

1. **创建页面组件**:

```vue
<!-- src/views/example/NewPage.vue -->
<template>
  <div class="new-page">
    <h1>新页面</h1>
  </div>
</template>

<script setup>
// 页面逻辑
</script>

<style scoped>
.new-page {
  padding: 20px;
}
</style>
```

2. **添加路由**:

```javascript
// src/router/index.js
{
  path: '/new-page',
  name: 'NewPage',
  component: () => import('@/views/example/NewPage.vue'),
  meta: { requiresAuth: true, title: '新页面' }
}
```

3. **添加菜单**:

```vue
<!-- src/components/Sidebar.vue -->
<el-menu-item index="/new-page">
  <el-icon><Document /></el-icon>
  <span>新页面</span>
</el-menu-item>
```

### 添加新API

```javascript
// src/api/example.js
import request from './request'

export const exampleApi = {
  // 获取列表
  getList(params) {
    return request.get('/api/example', { params })
  },
  
  // 获取详情
  getById(id) {
    return request.get(`/api/example/${id}`)
  },
  
  // 创建
  create(data) {
    return request.post('/api/example', data)
  },
  
  // 更新
  update(id, data) {
    return request.put(`/api/example/${id}`, data)
  },
  
  // 删除
  delete(id) {
    return request.delete(`/api/example/${id}`)
  }
}
```

### 状态管理

```javascript
// src/stores/example.js
import { defineStore } from 'pinia'

export const useExampleStore = defineStore('example', {
  state: () => ({
    data: [],
    loading: false
  }),
  
  getters: {
    count: (state) => state.data.length
  },
  
  actions: {
    async fetchData() {
      this.loading = true
      try {
        // 调用API
        const res = await exampleApi.getList()
        this.data = res.data
      } finally {
        this.loading = false
      }
    }
  }
})
```

## 🎯 最佳实践

### 1. 代码规范

- 使用 Vue 3 Composition API
- 组件命名使用 PascalCase
- 文件命名使用 kebab-case
- 使用 ESLint 进行代码检查

### 2. 性能优化

- 路由懒加载
- 组件按需引入
- 图片懒加载
- 合理使用 computed 和 watch
- 避免不必要的响应式数据

### 3. 安全建议

- Token 存储在 localStorage
- 敏感操作二次确认
- 文件上传类型验证
- XSS 防护（富文本内容过滤）
- CSRF 防护

### 4. 用户体验

- 加载状态提示
- 操作成功/失败提示
- 表单验证提示
- 空状态提示
- 错误边界处理

## 🚀 部署

### 构建生产版本

```bash
npm run build
```

### Nginx配置示例

```nginx
server {
    listen 80;
    server_name admin.yourdomain.com;
    
    root /var/www/hailong-admin/dist;
    index index.html;
    
    # Gzip压缩
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    # SPA路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # API代理
    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_cache_bypass $http_upgrade;
    }
    
    # 静态资源缓存
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|woff|woff2|ttf|svg)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
```

### Docker部署

创建 `Dockerfile`:

```dockerfile
FROM node:18-alpine as build

WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

构建和运行：

```bash
docker build -t hailong-admin .
docker run -d -p 3000:80 --name hailong-admin hailong-admin
```

## 🐛 故障排查

### 常见问题

**1. 登录后立即退出**

```
检查项：
- Token是否正确存储
- API地址是否正确
- 后端JWT配置是否正确
```

**2. 文件上传失败**

```
检查项：
- 文件大小是否超限
- 文件类型是否允许
- 后端上传接口是否正常
- 网络连接是否正常
```

**3. 富文本编辑器不显示**

```
检查项：
- WangEditor是否正确安装
- 组件是否正确引入
- 样式是否正确加载
```

**4. 图表不显示**

```
检查项：
- ECharts是否正确安装
- 数据格式是否正确
- 容器尺寸是否正确
```

## 后台接口与编辑约定（2026-10-05）

- 公告、资讯和企业资料的后台读取使用 `/api/announcements/manage`、`/api/info-publications/manage`、`/api/config/manage/...`，需要登录。原公开读取接口只返回启用内容；管理详情不增加门户浏览量。
- 列表只返回摘要及表格字段，编辑时读取详情。保存公告、资讯和企业资料必须带读取时的 `version`；过期版本返回 HTTP 409，页面保留本地修改。
- `useContentSubmit` 统一公告/资讯的保存过程；`useEditorForm`、`useEditorLeaveGuard` 保护未保存编辑和上传；`useLatestRequest` 防止迟到请求更新页面或已销毁的图表。
- 普通请求与附件上传共用单次令牌刷新。会话彻底失效时保留当前表单，可在提示的新窗口重新登录后返回继续操作。退出登录前先确认未保存内容，再调用服务端注销。
- 用户列表的 `isLastActiveAdmin` 用于禁用危险操作，后端同时通过事务和行锁保护最后一名启用管理员。
- Element Plus 组件及字符串图标在 `src/plugins-ui.js` 按实际使用注册；新增组件需要补充注册，局部图标直接导入。统计图表通过 `src/utils/echarts.js` 注册需要的图表能力。

### 回归验证

- `npm test`：本地时间、富文本有效性、地区筛选、证书展示和查询时序。
- `npm run test:browser`：历史公告和企业资料保存、扫描件预览、文件上传删除、权限、内容冲突、并发刷新、上传重试、登录失效恢复及服务端注销。
- 浏览器测试只连接 localhost / 127.0.0.1，需要预先启动 API 和后台，并通过 `ADMIN_TEST_USERNAME`、`ADMIN_TEST_PASSWORD` 提供本机测试管理员。端口可用 `ADMIN_TEST_URL`、`ADMIN_TEST_API` 调整；默认使用本机 Chrome，设置 `ADMIN_TEST_CHANNEL=chromium` 可使用 Playwright 自带 Chromium。
- 后端 `dotnet test BackEnd/Protral.sln` 从仓库根目录执行。MySQL 集成用例需显式设置 `LOCAL_MYSQL_TEST_CONNECTION`，仅接受本机连接，创建随机 `hailong_cleanup_test_*` 数据库并在结束时清理；不会使用该连接指定的业务库作为测试库。
- CI 的 `initial-data` 作业会启动独立 MySQL，导入初始化 SQL，再通过 `scripts/run-ci-browser.mjs` 启动隔离的 API/后台服务并执行浏览器用例。测试凭据通过进程环境传递，不写入工作流输出。

## 📚 相关文档

- [项目总体说明](../README.md)
- [后端API文档](../BackEnd/HailongConsulting.API/README.md)
- [前端门户文档](../hailong-protral/README.md)
- [数据库文档](../SQL/README.md)

## 📄 许可证

Copyright © 2025 河南海隆工程咨询有限公司

---

**最后更新**: 2025年12月16日  
**维护团队**: 海隆咨询技术部

# 海隆咨询官网前端门户

基于 Vue 3 + Tailwind CSS 开发的企业官网前端门户系统。

## 📋 项目概述

**项目名称**: 海隆咨询官网前端门户 (hailong-protral)

**技术架构**: Vue 3 + Vite + Tailwind CSS

**开发状态**: 本地开发与验证，部署状态以实际环境为准

## 🛠 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| Vue | 3.4.0 | 前端框架 |
| Vite | 5.0.0 | 构建工具 |
| Vue Router | 4.2.5 | 路由管理 |
| Tailwind CSS | 3.4.0 | CSS框架 |
| Axios | (内置) | HTTP客户端 |

## 📁 项目结构

```text
hailong-protral/
├─ config/site-config.json      导航、首页模块启用与排序
├─ src/
│  ├─ layouts/PortalLayout.vue  公共页头页脚与页面元信息
│  ├─ views/
│  │  ├─ home/                 首页
│  │  ├─ announcements/        公告列表与详情
│  │  ├─ news/                 新闻列表与详情
│  │  ├─ policies/             政策列表与详情
│  │  ├─ company/              关于、联系、业务/资质/业绩详情
│  │  ├─ services/             工具与专家库
│  │  └─ NotFound.vue          未知地址页面
│  ├─ components/
│  │  ├─ common/               加载状态、分页、富文本与图片预览
│  │  ├─ announcements/        公告筛选与卡片
│  │  ├─ company/              企业简介与业务集合
│  │  └─ home/                 首页模块、标语与联系弹层
│  ├─ composables/             可取消查询、详情加载、URL 筛选、元信息
│  ├─ api/                     HTTP 接口
│  ├─ utils/                   日期、分类、配置、查询辅助
│  ├─ router/                  公开 URL、栏目归属、默认页面标题
│  └─ assets/                  Logo、二维码等资源
└─ tests/                      请求竞态、日期、详情、元信息及配置回归
```

页面通过 `PortalLayout` 统一显示页头、页脚，不在单个页面重复引入。公开 URL 与原系统保持一致。
首页按品牌、公告、业务、简介、资质、业绩、数据、联系展示；资质和业绩成功加载后为空时隐藏整块，失败显示重试入口。屏外模块采用异步组件和可视区域触发。
首页背景由 `views/home/Home.vue` 统一管理，普通模块使用 `home-section` 与共享浅灰蓝底色，首屏和联系区分别标记为 `hero`、`closing`。调整模块顺序时，无需再拼接各模块独立的背景渐变；加载占位沿用相同底色。
列表筛选保存在 URL 中，重复搜索支持重新读取，旧请求不覆盖当前结果；新闻和政策分类与后台选项一致。关于页栏目通过 `?tab=` 链接定位，按访问的栏目加载内容。
正文使用 `RichTextContent`，后端继续负责 HTML 清洗；前端仅统一排版，不更改上传文件路径。
页面标题、描述、canonical 和分享信息由路由及详情数据生成，域名来自实际访问地址。当前为客户端 SPA 元信息，不等同于服务端渲染；需要无 JavaScript 的完整搜索引擎/分享抓取时另行增加预渲染。

回归命令：`node --test tests/*.test.mjs`；构建：`npm run build`。


## 🚀 快速开始

### 1. 环境要求

- **Node.js** >= 18.0
- **npm** >= 9.0 或 **pnpm** >= 8.0

### 2. 安装依赖

```bash
cd hailong-protral
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
VITE_API_BASE_URL=http://localhost:5000
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

访问 http://localhost:5173

### 5. 构建生产版本

```bash
npm run build
```

构建产物位于 `dist/` 目录。

### 6. 预览生产构建

```bash
npm run preview
```

## 📱 功能模块

### 1. 首页

#### 轮播Banner
- ✅ 自动轮播
- ✅ 手动切换
- ✅ 响应式设计
- ✅ 支持跳转链接

#### 企业简介
- ✅ 图文展示
- ✅ 企业特色标签
- ✅ 响应式布局

#### 业务范围
- ✅ 卡片式展示
- ✅ Hover效果
- ✅ 图标展示
- ✅ 详情跳转

#### 数据统计
- ✅ 动态数字展示
- ✅ 数据可视化
- ✅ 实时更新

#### 业绩展示
- ✅ 图片轮播
- ✅ 无限滚动
- ✅ 点击放大

#### 最新公告
- ✅ 分类展示
- ✅ 时间排序
- ✅ 快速跳转

### 2. 关于我们

#### 企业简介
- ✅ 公司历史
- ✅ 企业文化
- ✅ 组织架构
- ✅ 富文本展示

#### 业务范围
- ✅ 业务分类
- ✅ 详细介绍
- ✅ 服务流程

#### 企业资质
- ✅ 资质证书展示
- ✅ 证书详情
- ✅ 图片预览

#### 企业荣誉
- ✅ 荣誉墙展示
- ✅ 时间线布局
- ✅ 荣誉详情

#### 重要业绩
- ✅ 项目列表
- ✅ 项目详情
- ✅ 图片展示

### 3. 公告信息

#### 政府采购公告
- ✅ 列表展示（分页）
- ✅ 多条件筛选
  - 公告类型（采购公告/更正公告/结果公告）
  - 项目区域（省市区三级）
  - 时间范围
  - 关键词搜索
- ✅ 公告详情
- ✅ 附件下载
- ✅ 相关公告推荐
- ✅ 访问量统计

#### 建设工程公告
- ✅ 列表展示（分页）
- ✅ 多条件筛选
  - 公告类型（招标公告/中标公告/变更公告）
  - 项目区域
  - 时间范围
  - 关键词搜索
- ✅ 公告详情
- ✅ 附件下载
- ✅ 相关公告推荐

### 4. 信息发布

#### 公司新闻
- ✅ 新闻列表
- ✅ 分类筛选
- ✅ 新闻详情
- ✅ 富文本展示
- ✅ 附件下载

#### 政策法规
- ✅ 法规列表
- ✅ 分类筛选
- ✅ 法规详情
- ✅ 文件下载

### 5. 全局搜索

- ✅ 关键词搜索
- ✅ 高级筛选
  - 业务类别
  - 公告类型
  - 时间范围
  - 项目区域
- ✅ 搜索结果列表
- ✅ 关键词高亮
- ✅ 分页展示
- ✅ 结果统计

### 6. 实用工具

#### 招标代理费计算器
- ✅ 项目金额输入
- ✅ 项目类型选择（工程/货物/服务）
- ✅ 优惠比例设置
- ✅ 实时计算
- ✅ 计算过程展示
- ✅ 收费标准表格
- ✅ 依据文件查看

#### 造价咨询费计算器
- ✅ 工程造价输入
- ✅ 工程类型选择
- ✅ 计费方式选择
- ✅ 实时计算
- ✅ 收费标准说明
- ✅ 标准文件下载

#### 司法鉴定费计算器
- ✅ 鉴定标的输入
- ✅ 鉴定类型选择
- ✅ 分段累进计费
- ✅ 计算明细展示
- ✅ 计算示例

### 7. 专家库

#### 专家信息录入
- ✅ 电脑端填写
  - 居中弹窗表单
  - 在线填写专家信息
  - 表单验证
- ✅ 手机端填写
  - 二维码扫码填写
  - 可切换显示/隐藏二维码
  - 适合不方便使用电脑的用户
- ✅ 响应式布局
  - 移动端：二维码居中显示
  - 电脑端：二维码在公告框内右侧
- ✅ 联系方式展示
- ✅ 温馨提示说明

### 8. 联系我们

- ✅ 联系方式展示
  - 固定电话：0371-55894666
  - 公司地址：河南省郑州市郑东新区金水东路雅宝·东方国际广场1号楼8层
  - 电子邮箱
  - 工作时间
- ✅ 地图定位（百度/高德地图）
- ✅ 在线留言表单
- ✅ 二维码展示

### 9. 底部信息

#### 友情链接
- ✅ 分类展示
  - 省级单位
  - 地市级单位
  - 国家级单位
- ✅ 新窗口打开
- ✅ 响应式布局

#### 访问统计
- ✅ 总访问量
- ✅ 今日访问量
- ✅ 在线人数

## 🎨 响应式设计

### 断点设置

基于 Tailwind CSS 断点：

| 断点 | 最小宽度 | 设备类型 |
|------|---------|---------|
| `sm` | 640px | 手机横屏 |
| `md` | 768px | 平板 |
| `lg` | 1024px | 小屏电脑 |
| `xl` | 1280px | 标准电脑 |
| `2xl` | 1536px | 大屏 |

### 适配策略

- ✅ 移动端优先（Mobile First）
- ✅ 弹性布局（Flexbox/Grid）
- ✅ 图片响应式
- ✅ 导航自适应（汉堡菜单）
- ✅ 字体大小自适应
- ✅ 触摸友好

## 🔌 API调用示例

### 获取公告列表

```javascript
import { getAnnouncements } from '@/api/announcement'

const fetchAnnouncements = async () => {
  try {
    const response = await getAnnouncements({
      businessType: 'GOV_PROCUREMENT',
      pageIndex: 1,
      pageSize: 10
    })
    
    if (response.success) {
      console.log('公告列表', response.data.items)
    }
  } catch (error) {
    console.error('获取失败', error)
  }
}
```

### 获取公告详情

```javascript
import { getAnnouncementById } from '@/api/announcement'

const fetchDetail = async (id) => {
  try {
    const response = await getAnnouncementById(id)
    
    if (response.success) {
      console.log('公告详情', response.data)
    }
  } catch (error) {
    console.error('获取失败', error)
  }
}
```

### 全局搜索

```javascript
import { globalSearch } from '@/api/search'

const search = async () => {
  try {
    const response = await globalSearch({
      keyword: '招标',
      businessType: 'GOV_PROCUREMENT',
      startDate: '2025-01-01',
      endDate: '2025-12-31',
      province: '河南省',
      pageIndex: 1,
      pageSize: 10
    })
    
    if (response.success) {
      console.log('搜索结果', response.data.items)
    }
  } catch (error) {
    console.error('搜索失败', error)
  }
}
```

### 获取首页统计数据

```javascript
import { getHomeStatistics } from '@/api/config'

const fetchStatistics = async () => {
  try {
    const response = await getHomeStatistics()
    
    if (response.success) {
      console.log('统计数据', response.data)
    }
  } catch (error) {
    console.error('获取失败', error)
  }
}
```

## 🎯 性能优化

### 已实现的优化

- ✅ 路由懒加载
- ✅ 图片懒加载
- ✅ 组件按需引入
- ✅ 代码分割
- ✅ Gzip压缩
- ✅ 浏览器缓存策略
- ✅ CDN加速（可选）

### 优化建议

```javascript
// 路由懒加载
const Home = () => import('@/views/Home.vue')

// 图片懒加载
<img loading="lazy" src="image.jpg" alt="description">

// 组件按需引入
import { ref, computed } from 'vue'
```

## 🔧 开发指南

### 添加新页面

1. **创建页面组件**:

```vue
<!-- src/views/example/NewPage.vue -->
<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-3xl font-bold mb-6">新页面</h1>
    <p>页面内容...</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

// 页面逻辑
onMounted(() => {
  console.log('页面已加载')
})
</script>
```

2. **添加路由**:

```javascript
// src/router/index.js
{
  path: '/new-page',
  name: 'NewPage',
  component: () => import('@/views/example/NewPage.vue'),
  meta: { title: '新页面' }
}
```

3. **添加导航链接**:

在 `config/site-config.json` 的 `navigation.header` 中添加链接；需要下拉分组时使用 `children`。

```json
{ "name": "新页面", "path": "/new-page", "order": 7 }
```

### 使用Tailwind CSS

```vue
<template>
  <!-- 响应式布局 -->
  <div class="container mx-auto px-4">
    <!-- 网格布局 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <!-- 卡片 -->
      <div class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
        <h3 class="text-xl font-semibold mb-2">标题</h3>
        <p class="text-gray-600">内容...</p>
      </div>
    </div>
  </div>
</template>
```

### 自定义Tailwind配置

```javascript
// tailwind.config.js
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#1e40af',
        secondary: '#64748b',
      },
      fontFamily: {
        sans: ['Microsoft YaHei', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
```

## 🚀 部署

### 构建生产版本

```bash
npm run build
```

### Nginx配置示例

```nginx
server {
    listen 80;
    server_name www.hailongzixun.com;
    
    root /var/www/hailong-protral/dist;
    index index.html;
    
    # Gzip压缩
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/x-javascript application/xml+rss application/json;
    
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
docker build -t hailong-protral .
docker run -d -p 80:80 --name hailong-protral hailong-protral
```

## 📊 SEO优化

### Meta标签

```html
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>海隆咨询 - 专业招标代理、造价咨询、司法鉴定服务</title>
  <meta name="description" content="河南海隆工程咨询有限公司，提供专业的招标代理、造价咨询、司法鉴定服务">
  <meta name="keywords" content="招标代理,造价咨询,司法鉴定,河南,郑州,海隆咨询">
  <link rel="canonical" href="https://www.hailongzixun.com">
</head>
```

### 结构化数据

```javascript
// 在页面中添加结构化数据
const structuredData = {
  "@context": "https://schema.org",
  "@type": "Organization",
  "name": "河南海隆工程咨询有限公司",
  "url": "https://www.hailongzixun.com",
  "logo": "https://www.hailongzixun.com/logo.png",
  "contactPoint": {
    "@type": "ContactPoint",
    "telephone": "+86-371-55894666",
    "contactType": "customer service"
  }
}
```

### Sitemap生成

在 `public/` 目录创建 `sitemap.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url>
    <loc>https://www.hailongzixun.com/</loc>
    <lastmod>2025-12-16</lastmod>
    <changefreq>daily</changefreq>
    <priority>1.0</priority>
  </url>
  <url>
    <loc>https://www.hailongzixun.com/about</loc>
    <changefreq>monthly</changefreq>
    <priority>0.8</priority>
  </url>
  <!-- 更多URL -->
</urlset>
```

## 🐛 故障排查

### 常见问题

**1. 页面空白**

```
检查项：
- 控制台是否有错误
- API地址是否正确
- 路由配置是否正确
- 组件是否正确导入
```

**2. 样式不生效**

```
检查项：
- Tailwind CSS是否正确配置
- PostCSS是否正确配置
- 样式文件是否正确引入
- 浏览器缓存
```

**3. 图片不显示**

```
检查项：
- 图片路径是否正确
- 图片是否存在
- 网络请求是否成功
- CORS配置
```

**4. API请求失败**

```
检查项：
- API地址是否正确
- 后端服务是否运行
- CORS配置
- 网络连接
```

## 📚 相关文档

- [项目总体说明](../README.md)
- [后端API文档](../BackEnd/HailongConsulting.API/README.md)
- [后台管理文档](../hailong-admin/README.md)
- [数据库文档](../SQL/README.md)

## 📄 许可证

Copyright © 2025 河南海隆工程咨询有限公司

---

**最后更新**: 2025年12月16日  
**维护团队**: 海隆咨询技术部

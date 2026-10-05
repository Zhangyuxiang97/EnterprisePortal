# 海隆咨询前端门户

Vue 3 + Vite + Tailwind CSS 门户，使用 Vue Router 和 Axios。依赖版本以 `package.json` 及锁文件为准。

## 启动

先按 [根 README](../README.md) 初始化数据库并启动 API，然后在本目录执行：

```bash
npm ci
npm run dev
```

默认地址为 `http://127.0.0.1:3000`。`VITE_API_BASE_URL=/api`，开发服务器将 `/api` 和 `/uploads` 代理到 `http://127.0.0.1:5000`；需要其他 API 地址时，在启动终端设置 `API_PROXY_TARGET`。

## 页面与结构

门户包含首页、公告列表与详情、新闻中心、政策法规、关于海隆、联系我们、费用计算工具和专家库页面。

```text
config/site-config.json          导航、页脚链接和首页模块
src/
├─ layouts/PortalLayout.vue      公共页头页脚与页面元信息
├─ views/
│  ├─ home/                     首页
│  ├─ announcements/            公告列表与详情
│  ├─ news/                     新闻列表与详情
│  ├─ policies/                 政策列表与详情
│  ├─ company/                  关于、联系、业务 / 资质 / 业绩详情
│  ├─ services/                 工具与专家库
│  └─ NotFound.vue              未知地址页面
├─ components/
│  ├─ common/                   加载、分页、面包屑、富文本和图片预览
│  ├─ announcements/            公告筛选与卡片
│  ├─ company/                  企业简介与集合
│  └─ home/                     首页模块、标语和联系弹层
├─ composables/                 可取消查询、详情、URL 筛选及元信息
├─ api/                         HTTP 接口
├─ utils/                       日期、分类、配置和查询辅助
└─ router/                      公开 URL、栏目归属及默认标题
```

## 展示与配置约定

- 页面通过 `PortalLayout` 共用页头页脚；首页背景在 `views/home/Home.vue` 统一管理，普通模块共用 `home-section`，首屏和联系区分别使用 `hero`、`closing`。
- 首页按品牌、公告、业务、简介、资质、业绩、数据、联系组织。首次打开优先加载首屏与简介，屏外模块接近可视区域再加载；返回首页恢复滚动时立即挂载模块。
- 标语读取门户设置，首尾字先显示，再平滑展开并逐字浮现；不在组件中写死公司标语。
- 资质和业绩成功加载后为空时隐藏整块；加载失败显示重试入口。初始化业绩为空，待后台补充真实内容。
- 列表筛选保存在 URL 中；迟到请求不会覆盖当前结果。关于页通过 `?tab=` 定位企业简介、业务、资质、业绩及荣誉栏目。
- 公告详情分别展示预算与中标 / 成交金额；列表摘要不直接输出历史富文本标签或样式。
- 正文统一通过 `RichTextContent` 排版，后端负责 HTML 清洗。上传资源地址由接口提供，不在页面重复拼接 `/uploads`。
- 公司、标语、联系方式、地图、交通及 FAQ 从后台“门户设置”读取，默认值与 API 共用 `SiteSettingsDefaults.json`。读取失败时可使用七天内最近一次有效缓存或默认内容。
- 导航、页脚链接和首页模块的启用 / 顺序在 `config/site-config.json` 中维护，修改后重新构建；企业正文及资质、荣誉等在后台“企业资料”维护。

详细字段和生效方式见 [门户设置](../docs/portal-settings.md) 与 [配置存储位置](../docs/configuration/storage.md)。

## 元信息与资源

页面标题、描述、canonical 和分享信息由路由及详情数据生成，域名来自实际访问地址。当前采用客户端 SPA 元信息，需要无 JavaScript 的完整抓取时需另行实现预渲染。

旧站已恢复的图片和附件由 API 发布资源补入 `/uploads/legacy/`，部署后不依赖旧站提供这些文件。历史缺失文件、外链及来源限制见 [数据库说明](../SQL/README.md)。

## 测试与构建

```bash
node --test tests/*.test.mjs
npm run build
```

测试覆盖请求时序、列表与详情状态、日期、分类、元信息及默认配置。`npm run preview` 用于在本机查看构建产物，当前 Vite 预览会继承配置中的 `/api` 和 `/uploads` 代理，仍需启动 API。正式部署由 Nginx 提供这些路由。

Docker 门户默认入口为 `http://服务器IP:8082`，可通过 `PORTAL_HTTP_PORT` 调整。统一部署步骤见 [Ubuntu22 Docker 部署指南](../docs/deployment/ubuntu22-docker.md)。

最后更新：2026-10-05。

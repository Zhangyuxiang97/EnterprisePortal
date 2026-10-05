# 海隆咨询后台管理系统

Vue 3 + Vite + Element Plus 管理端，使用 Pinia、Axios、WangEditor 和 ECharts。依赖版本以 `package.json` 及锁文件为准。

## 启动

先按 [根 README](../README.md) 初始化数据库并启动 API，然后在本目录执行：

```bash
npm ci
npm run dev
```

默认地址为 `http://127.0.0.1:3001`。`.env.development` 的 `VITE_API_BASE_URL=/api`，Vite 将 `/api`、`/uploads` 代理到 `http://127.0.0.1:5000`。API 使用其他地址时，在启动终端设置 `API_PROXY_TARGET`。

## 初始账号

API 在用户表为空时生成用户名 `admin` 和 12 位随机密码。按 API 启动提示读取工作目录下的 `logs/bootstrap/initial-admin-credentials.txt`；Docker 获取方法见 [初始管理员说明](../README.md#初始管理员)。

首次登录后修改密码并删除凭据文件。登录表单只验证非空，创建、重置和修改新密码至少 6 位。

## 管理入口

| 菜单 | 内容 |
| --- | --- |
| 数据看板 | 公告和资讯数量、金额与趋势 |
| 公告管理 | 政府采购、建设工程，预算与中标 / 成交金额分别维护 |
| 信息发布 | 新闻中心、政策法规，包含知识资讯等现有分类 |
| 附件管理 | 上传、预览、引用查询和删除 |
| 门户设置 | 公司标语、联系方式、地图、交通及 FAQ |
| 企业资料 | 企业简介、业务范围、资质、重要业绩、荣誉 |
| 统计分析 | 访问、公告与信息发布统计 |
| 系统管理 | 用户、日志、区域字典和修改密码 |

菜单按角色显示。轮播图与友情链接的独立编辑页暂未开放菜单入口；门户静态导航和首页模块配置需修改源文件并重新构建，详见 [配置存储位置](../docs/configuration/storage.md)。

公告数量按记录计数，不代表去重后的项目数。金额统计汇总已记录的结果公告中标 / 成交金额，同一项目可能有多条结果记录；发布时间趋势按启用记录的 `PublishTime` 统计，“今日入库”按创建时间统计。

## 目录与维护约定

```text
src/
├─ api/             HTTP 请求、刷新与接口封装
├─ components/      布局、编辑器、上传和地区选择
├─ composables/     表单、提交、离开确认与查询时序
├─ views/           看板、公告、资讯、企业资料和系统页面
├─ stores/          用户状态
├─ router/          路由和角色守卫
├─ plugins-ui.js    实际使用的 Element Plus 组件及字符串图标
└─ utils/           会话、错误、表单、上传和图表辅助
```

- 后台读取使用 `/api/announcements/manage`、`/api/info-publications/manage`、`/api/config/manage/...`，需要授权；管理详情不计门户浏览量。
- 列表只返回表格或摘要字段，编辑时读取详情。保存公告、资讯及企业资料携带读取时的 `version`；过期返回 409，保留本地修改。
- `useContentSubmit` 统一公告 / 资讯保存；`useEditorForm`、`useEditorLeaveGuard` 保护未保存内容及上传；`useLatestRequest` 防止迟到响应覆盖页面。
- 普通请求与上传共用单次刷新及重试。会话失效时保留编辑内容，可在新窗口登录后返回。退出先确认未保存内容，再调用服务端注销接口。
- `RichEditor.vue` 处理历史 HTML、图片上传和隔离正文预览；历史正文在实际编辑后才转成编辑器格式。
- 上传格式和体积限制读取 `/api/attachments/upload-options`，默认后端上限 10 MB。已保存内容仍引用的附件不能直接删除。
- 用户列表的 `isLastActiveAdmin` 用于限制操作，后端事务和行锁保护最后一名启用管理员。
- 资质启用状态与证书有效期分别展示；日期、编号和荣誉级别缺少依据时留空。
- 新增 Element Plus 组件需检查 `plugins-ui.js` 注册；图表能力统一在 `utils/echarts.js` 导入。

## 测试与构建

```bash
npm test
npm run build
```

浏览器回归需启动本机独立测试数据库、API 和管理端，并通过环境变量提供凭据：

| 环境变量 | 用途 / 默认值 |
| --- | --- |
| `ADMIN_TEST_USERNAME` / `ADMIN_TEST_PASSWORD` | 本机测试管理员，必须设置 |
| `ADMIN_TEST_URL` | `http://127.0.0.1:3002` |
| `ADMIN_TEST_API` | `http://127.0.0.1:5000/api` |
| `ADMIN_TEST_CHANNEL` | 默认 `chrome`；`chromium` 使用 Playwright 自带浏览器 |

```bash
# 使用回归默认端口启动管理端
npm run dev -- --host 127.0.0.1 --port 3002 --strictPort
# 在另一个已设置测试环境变量的终端运行
npm run test:browser
```

该命令执行 `tests/admin.smoke.mjs` 与 `tests/backend-cleanup.smoke.mjs`，覆盖历史公告 / 企业资料保存、扫描件预览、上传删除、权限、版本冲突、刷新、登录恢复及注销。

脚本只连接本机地址，会修改测试库中的现有记录、创建和清理临时内容。附件采用软删除，上传文件按存储保留规则处理；应使用独立测试库。CI 通过 [验证工作流](../.github/workflows/verify.yml) 启动隔离服务执行回归。

## 部署与文档

Docker 后台默认入口为 `http://服务器IP:8080`，使用根目录的统一构建与代理配置。

- [Ubuntu22 Docker 部署](../docs/deployment/ubuntu22-docker.md)
- [门户设置字段](../docs/portal-settings.md)
- [后端 API](../BackEnd/HailongConsulting.API/README.md)
- [全部文档](../docs/README.md)

最后更新：2026-10-05。

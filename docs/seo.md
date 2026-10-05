# 门户页面元信息

## 实现位置

- [HTML 入口](../hailong-protral/index.html)：基础标题、描述、语言及分享标签。
- [PortalLayout](../hailong-protral/src/layouts/PortalLayout.vue)：结合路由、公司设置和详情状态更新页面元信息。
- [usePageMeta](../hailong-protral/src/composables/usePageMeta.js)：提供详情标题、描述和不可用状态。
- [metadata.js](../hailong-protral/src/utils/metadata.js)：生成并写入标题、description、canonical、robots、Open Graph、Twitter 及 Organization JSON-LD。

公司名称和默认描述读取门户设置；canonical 和分享 URL 使用实际访问域名及页面路径，分享图片使用站点 Logo。页面不可用时通过 `noindex` 标记。

## 渲染方式

门户是客户端 SPA，详情内容和动态元信息在浏览器执行 JavaScript 后生成。需要不执行 JavaScript 的完整正文或分享抓取时，应配置预渲染或服务端渲染。

## 发布配置

- [robots.txt](../hailong-protral/public/robots.txt) 和 [sitemap.xml](../hailong-protral/public/sitemap.xml) 是构建时复制的静态文件，发布时需与实际域名及公开路由一致。
- 静态 sitemap 不会自动枚举数据库中的全部公告和资讯。内容持续增加时，需要按实际公开内容生成并更新。
- 搜索平台验证标签按实际申请结果配置；没有有效验证码时不添加占位标签。
- HTTPS、域名和外部反向代理按实际部署环境配置，部署步骤见 [Ubuntu22 Docker 指南](deployment/ubuntu22-docker.md)。

## 验证

1. 打开列表和详情，检查标题、描述、canonical 和分享标签。
2. 检查停用或不存在内容的页面状态及 robots 元信息。
3. 检查实际站点的 `/robots.txt`、`/sitemap.xml` 地址和其中的公开 URL。
4. 从门户目录执行 `node --test tests/*.test.mjs`，核验元信息生成与页面状态。

部署排查见 [故障排查](operations/troubleshooting.md)。

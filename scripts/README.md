# 项目工具

在本目录执行 `npm ci` 安装工具依赖。

| 目录 | 内容 |
| --- | --- |
| `initialization/` | 编号 SQL 生成、公告字段复核、地区字典、公开媒体来源及企业资料 |
| `migration/` | Access 只读导出、旧站抓取、数据转换与预览 |
| `checks/` | 配置来源、Docker 构建引用及部署入口一致性检查 |
| `tests/` | 数据转换、部署就绪和浏览器回归验证 |
| `data/` | 本机中间数据，不提交 Git |

从仓库根目录执行 `node scripts/checks/check-project-config.mjs`。从本目录执行 `npm test` 运行数据处理测试。

`tests/run-ci-browser.mjs` 在 CI 的已初始化隔离数据库上启动 API 和后台，执行后台回归及门户 Chromium / WebKit 多尺寸验收。门户独立执行命令及报告位置见 [门户测试说明](../hailong-protral/README.md#浏览器与手机端验收)。

初始化维护命令为 `npm run generate-initial-sql`、`npm run generate-access-initial-sql`、`npm run generate-regions`。输入和执行顺序见 [数据库说明](../SQL/README.md)。这些命令修改 `SQL/init/`，复核报告写入 `SQL/reports/`；服务器已有数据由 API 版本迁移更新。

迁移工具通过 `npm run scrape`、`npm run transform` 调用；Access 导出入口为 `migration/export-access-public.ps1`。完整原始备份和中间数据保留在本机，部署直接使用仓库中已生成的 SQL 和 SeedAssets。

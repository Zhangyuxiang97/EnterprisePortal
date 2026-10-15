# 海隆咨询数据库初始化说明

## 目录与执行文件

`init/` 保存八份按 01–08 顺序执行的首次初始化 SQL；`reports/` 保存资料来源和字段复核证据。MySQL 数据目录为空时自动初始化，已有数据目录由 API 版本化迁移升级。

| 执行文件 | 内容 |
| --- | --- |
| [01_schema.sql](init/01_schema.sql) | 字符集、数据库结构、索引和视图 |
| [02_base_data.sql](init/02_base_data.sql) | 真实企业简介、业务范围及全部 7 条友情链接；不导入管理员或演示业绩 |
| [03_regions.sql](init/03_regions.sql) | 2463 个唯一区域编码，保留省市区县、父编码和排序字段 |
| [04_announcements_bidding.sql](init/04_announcements_bidding.sql) | 旧站招标栏目 1169 条公告 |
| [05_announcements_result.sql](init/05_announcements_result.sql) | 旧站中标栏目 1131 条公告 |
| [06_announcements_correction.sql](init/06_announcements_correction.sql) | 旧站变更栏目 226 条公告 |
| [07_info_publications.sql](init/07_info_publications.sql) | 43 篇资讯：政策 26、新闻中心 17；包含 13 篇知识资讯 |
| [08_company_media.sql](init/08_company_media.sql) | 4 项证照、6 组荣誉及 29 个公开媒体文件的附件关联 |

媒体脚本查询企业简介和资讯的实际 ID，因此在资讯导入后执行。初始管理员由 API 在用户表为空时生成。重要业绩缺少真实来源，保持空白，由后台补充。

每份数据 SQL 声明 `utf8mb4`，结构文件明确设置数据库和表的字符集与排序规则。三个公告文件按旧站栏目保留；实际公告类型还根据标题判断，数据库按类型统计的数量可能与栏目文件行数不同。

## 初始化数据维护

仓库已包含生成结果，部署直接使用 `init/` 和 API `SeedAssets/legacy`，无需提供原始 MDB 或再次抓取旧站。原 MDB 和本机中间数据不提交。

从完整公告数据 `scripts/data/access-initial-announcements.json` 生成公告 SQL：

```bash
cd scripts
npm run generate-initial-sql
```

地区数据在 `init/03_regions.sql` 维护；公告生成、离线清洗及地区格式化共用 `scripts/initialization/region-dictionary.js` 读取。修改地区后执行 `npm run generate-regions` 校验编码唯一性并统一格式，保留原插入顺序。省级默认按六位行政区划代码升序排列，顶层父编码为 NULL；门户与后台公告筛选按当前条件下的地区数量降序排列，相同数量沿用字典顺序。

数据来自 henanhailong.com 抓取结果及海隆数据库.mdb：保留 2375 条已清洗公告，补入 151 条备份公告，共 **2526 条**，最新公告日期为 2026-09-24。


## 从 Access 备份重新生成

Windows 需要已安装的 Microsoft ACE OLEDB Provider。导出工具复制原文件后只读访问副本，仅导出公开内容，不读取管理员密码、留言或订单。

```powershell
powershell -File scripts/migration/export-access-public.ps1 -DatabasePath "D:/备份/海隆数据库.mdb" -OutputDirectory ".runtime/access-export"
cd scripts
npm ci
npm run generate-access-initial-sql -- --source ../.runtime/access-export --baseline data/final-data.json
```

`final-data.json` 是原 2375 条已清洗公告，不要将生成后的完整公告文件当作基线；该中间文件保留在本地 `scripts/data/`，不随仓库分发。没有原始输入时直接使用已生成 SQL 即可。

生成器更新 `init/02_base_data.sql`、`init/04–08` 及 `reports/` 下的两份 JSON 报告，并核验初始化资源 SHA-256；重复执行输出一致。只有旧 2375 条数据时，普通公告生成命令会拒绝覆盖当前完整初始化数据。

## 分类与金额字段

2526 条公告的分类、金额和地区依据标题、正文及表格核验，43 篇资讯按内容归入现有页面分类。字段证据和推断依据保存在复核报告中。

| 项目 | 复核结果 |
| --- | ---: |
| 政府采购 / 建设工程 | 1787 / 739 |
| 招标采购 / 结果 / 更正 | 1177 / 1138 / 211 |
| 政府采购货物 / 服务 / 工程 | 608 / 846 / 333 |
| 政府采购子类未填写 | 0 |
| 明确预算金额 | 620 |
| 明确中标或成交金额 | 497 |
| 已确认省市 / 已确认区县 | 2524 / 1195 |
| 至少存在一项待核查原因的公告 | 13 |

以下 5 条公告的采购子类采用内容推断：旧 ID 2165 人居环境集中治理、799 电商进农村示范归服务；509/510 路演大厅采购归货物；2956 医共体整项目终止公告按预算占比更大的医疗设备包归货物（包 2 为 7960 万元，包 1 信息化服务为 5800 万元）。字段复核报告以 `inferred: true` 和 `rationale` 明示依据，保留混合包说明；正文和金额按来源保留。

### 金额字段及接口

- `budget_amount` / `budgetAmount`：只存明确预算。最高限价、控制价和项目投资额在证据报告中分别记录，不写入预算。
- `award_amount` / `awardAmount`：独立存最终中标或成交金额；首次部署表结构及公告 INSERT 已加入该字段。
- 两个字段沿用当前金额口径：万元、`DECIMAL(15,2)`。元除以 10000、亿元乘以 10000，原始数字、单位和出处另存于复核报告，避免丢失原文精度。
- 候选人报价、终止或流标结果、折扣费率、单价、代理费不填写中标金额。多标段无法确认单一总额时留空，不只取第一个标段。
- 已逐条确认完整范围的 40 条分包结果，按每包原始数值及单位精确汇总后统一舍入，报告附 `parts` 和 `scope`；相同金额的不同包各计一次，表格与正文重复表达只计一次。复核范围或金额项数发生变化时生成器报错。
- 明确预算变更采用“变更为”后的现行金额；例如旧 ID 1345 从 63.82 改为 51.68 万元。结果更正的新金额保留在报告 `notes`，更正公告不再计入结果总额；原公告需要改标段的旧 ID 384 同时关联更正来源 387。
- HTML 表格独立单位列优先于表头，“中标金额（元）”表头下的 `%` 不得作为元金额。最终获奖人报价不会越过正文小节读取其他投标人的报价。
- API 创建、列表、详情和更新均支持预算与中标金额；更新时字段省略保留原值，显式 `null` 清空金额，0 是有效值。管理端输入框清空时发送 `null`。
- 管理端政府采购和建设工程列表显示两类金额，结果公告编辑提供中标金额输入；门户详情分别显示预算和中标/成交金额。
- 首页总额和地区金额排行汇总结果公告的 `award_amount`，采用公告口径；同一项目的重复发布与未确认总额不构成完整财务统计。

### 地区与可追溯性

地区优先使用正文项目地点，其次标题，再取采购人/招标人块；截断代理机构、财政部门和监督部门信息。完整省市约束区县，长区县名称优先，避免“北京大道”“中心城区”“太康县/康县”“清丰县/丰县”误识别。

补充现有字典遗漏的巩义市；编码依据[巩义市政府 2025 年“一卡通”清单](https://public.gongyishi.gov.cn/)，归属关系参考[巩义市政府历史沿革](https://www.gongyishi.gov.cn/lsyg.jhtml)。中原区 410102 和二七区 410103 根据[郑州市生态环境局重点排污单位名录](https://sthjj.zhengzhou.gov.cn/u/cms/sthjj/202005/04032833jv2m.pdf)补齐。其他地区继续使用项目现有字典，不把没有明确区县证据的公告默认写成市区。

[initialization-field-review.json](reports/initialization-field-review.json) 包含每条公告的旧值、新值、是否变更、规则、证据片段、金额候选及待核查原因，并保留旧站 ID；顶部 `pendingAnnouncements` 为完整剩余清单。分类/地区在同名项目一致时补缺；不同包的对应关系经逐条确认后引用明确来源，不复制其他包的标的类型。金额不会跨公告复制。正常候选报价和明确跨区县范围留在 `notes`，不列为未解决冲突。

### 缺少可靠证据的字段

13 条公告的部分金额或地区字段缺少可靠证据，对应字段留空，正文正常初始化。具体原因如下，中标 / 成交金额共覆盖 497 条结果记录。

| 剩余原因 | 条数 | 旧站 ID / 依据 |
| --- | ---: | --- |
| 缺少可靠项目城市 | 2 | 738、767 管理配套物资；仅有代理/开标地点及联系方式，不能当作交货或项目地点 |
| 单价/费率无法转成固定总额，或标段不完整 | 9 | 2180、1865、1723、1466、1444、641、595、574、384；每条报告说明不能合计的具体依据 |
| 原文金额单位冲突 | 1 | 1849 唐河矿山修复，表头写万元但原数字与元控制价数量级冲突；对应候选公示也没标报价单位，不能擅改 |
| 最终报价缺少单位 | 1 | 1193 都里镇农田，中标价仅写 2933422.62，保留数值证据而不猜元或万元 |

已确认完整两标段的旧 ID 618 合计 25299.23 万元；旧 ID 2869 合计 51.07 万元；旧 ID 2705 合计 45.35 万元。预算与中标金额分别存储。

复核脚本为 `scripts/initialization/review-announcement-fields.js`，逐条确认结论位于 `scripts/initialization/confirmed-announcement-reviews.js`，由 Access 初始化生成器统一调用。后续重新生成会复用复核规则。

## 图片与附件

MDB 未包含旧站 `uploadfile` 文件。原有 61 个文件引用中，已从原站恢复企业资料及新闻、政策的 28 张图片和 1 个 Word 附件；其余 32 个公告媒体文件仍列入待补清单，见 [initialization-source-report.json](reports/initialization-source-report.json)。

- 不保留没有原始文件的本地图片引用，缺失文档保留链接文字并标注“附件待补充”。
- 清除编辑器本机 `file:///` 图片、旧编辑器图标、脚本及事件属性。
- 保留正文中的 6 处 PNG 内嵌图片；外部图片和文档保留原外链，未核验外站可用性。
- 后端正文过滤器允许 `img.src` 中限定栅格图片的 base64 数据；其他 data URI、SVG、脚本及事件属性被过滤。
- 已恢复文件保存在 `BackEnd/HailongConsulting.API/SeedAssets/legacy`，来源 URL、大小和 SHA-256 见 `scripts/initialization/legacy-media-manifest.json`，企业证书人工核对字段见 `scripts/initialization/legacy-company-data.json`。原站标记 `.doc` 的附件实为 OpenXML Word 文档，本站按 `.docx` 提供。
- API 每次启动仅把缺失的初始化资源补入上传卷，不覆盖已存在文件。Docker 与本地部署均使用 `/uploads/legacy/`；发布与备份时保留初始化资源及整个上传目录。招聘页面尚未导入。
- 当前门户默认地址按用户确认设置为“雅宝·东方国际广场1号楼8层”。

## 执行顺序

Docker Compose 将 `SQL/init/` 挂载到 MySQL 的 `/docker-entrypoint-initdb.d`。空数据目录首次启动时按文件名执行，来源报告不进入初始化目录。

手动初始化时，在仓库根目录执行下面的 Bash 命令。凭据使用本机权限受限的 MySQL 客户端配置文件，文件位置通过 `MYSQL_CLIENT_CONFIG` 提供：

```bash
: "${MYSQL_CLIENT_CONFIG:?请提供本机 MySQL 客户端配置文件路径}"
for sql in SQL/init/[0-9][0-9]_*.sql; do
  mysql --defaults-extra-file="$MYSQL_CLIENT_CONFIG" --default-character-set=utf8mb4 < "$sql" || exit 1
done
mysql --defaults-extra-file="$MYSQL_CLIENT_CONFIG" --default-character-set=utf8mb4 < scripts/tests/verify-initial-data.sql
```

初始化断言检查公告、分类、金额、地区层级、资讯、友情链接、真实企业资料和媒体关联，任一断言不符即返回 SQL 错误。


## 数据覆盖范围

地区 SQL 包含 2463 条唯一区域编码，数量按仓库初始化文件统计：

| 字典层级 | 条数 |
| --- | ---: |
| 省级 | 34 |
| 市级 | 361 |
| 区县级及市区汇总项 | 2068 |

字典包含市区汇总项，具体区县覆盖以 SQL 中实际编码为准。维护时按现有父子关系写入，地区编码不得重复。

### 公告与资讯
- 旧站招标信息栏目：1169条
- 旧站中标公示栏目：1131条
- 旧站变更公告栏目：226条
- 公告总计：2526条；真实资讯：43条

## 数据统计

执行完成后，可以运行以下查询查看统计信息：

```sql
-- 查看各层级数据量
SELECT
    CASE region_level
        WHEN 1 THEN '省级'
        WHEN 2 THEN '市级'
        WHEN 3 THEN '区县级'
    END AS '行政层级',
    COUNT(*) AS '数量'
FROM region_dictionary
WHERE is_deleted = 0
GROUP BY region_level
ORDER BY region_level;

-- 查看公告数据统计
SELECT
    notice_type AS '公告类型',
    COUNT(*) AS '数量'
FROM announcements
WHERE is_deleted = 0
GROUP BY notice_type;
```

## 注意事项

1. **执行前确认**：确保已经创建了数据库表结构
2. **字符编码**：确保数据库和表使用UTF-8编码
3. **数据冲突**：如果已有数据，可能会出现主键冲突，建议在空数据库中执行
4. **区县数据**：地区文件按统一编码写入，河南、安徽为完整市县级数据
5. **数据更新**：行政区划可能会有调整，使用时请注意数据的时效性
6. **历史数据**：04、05、06 三个公告脚本包含从旧网站抓取的历史公告，数据来源为 henanhailong.com；公告 SQL 已直接写入区域编码，无需额外后处理脚本

## 数据格式说明

### 区县数据格式
本项目采用以下格式处理区县数据：

1. **市辖区表示**：字典使用"XX市区"汇总项，并包含中原区、二七区等独立区县编码；正文明确且字典支持具体区名时存对应区编码。
   ```sql
   ('320101', '南京市区', 3, '320100', 1),  -- 包含玄武区、秦淮区等所有市辖区
   ```

2. **县单独列出**：县级行政区单独列出
   ```sql
   ('320118', '溧水区', 3, '320100', 2),     -- 原为县，现为区，但单独列出
   ('320117', '高淳区', 3, '320100', 3),     -- 原为县，现为区，但单独列出
   ```

3. **县级市**：县级市也单独列出
   ```sql
   ('320581', '常熟市', 3, '320500', 2),     -- 县级市
   ```

## 相关文档

- [项目说明](../README.md)
- [系统架构](../docs/architecture.md)
- [部署指南](../docs/deployment/ubuntu22-docker.md)

---

**最后更新时间**：2026-10-06

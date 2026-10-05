# 海隆咨询数据库初始化说明

## 文件说明

**重要提示：** 所有 SQL 文件已按执行顺序编号（00-10），MySQL Docker 容器在数据目录为空的首次启动时按文件名顺序执行。现有数据目录不会再次自动导入。

### 0. 00_set_charset.sql
**字符集设置文件**，统一使用 `utf8mb4`，应在导入其他文件前执行。

### 1. 01_hailong_consulting_schema.sql
**数据库结构文件**，包含：
- 数据库创建语句
- 所有表的创建语句（CREATE TABLE）
- 索引和视图定义

### 2. 02_hailong_consulting_init_data.sql
**主初始化数据文件**，包含：
- 不包含管理员或测试账号，初始管理员由 API 首次启动生成
- 备份中的真实企业简介，以及业务范围和友情链接基础数据
- 未经证实的演示资质、荣誉和业绩不再初始化；核对原图恢复的真实证照与荣誉由 10 导入
- 真实公告和资讯分别由 06–09 导入，不再包含演示资讯
- 友情链接数据
- 区域字典由 03、04、05 三批初始化文件统一导入，本文件不重复导入

### 3. 03_region_dictionary_batch1_henan.sql
**区域字典初始化第一批**：河南省现有省市县字典，共 143 条唯一区域编码，补充原先缺失的巩义市、中原区和二七区；保留现有市区汇总项。

### 4. 04_region_dictionary_batch2_north_east_east.sql
**区域字典初始化第二批**：华北、东北、华东区域数据（含安徽省），共 998 条唯一区域编码。

### 5. 05_region_dictionary_batch3_central_south_west.sql
**区域字典初始化第三批**：华中、华南、西南、西北及港澳台区域数据，共 1322 条唯一区域编码。

### 6. 06_announcements_bidding.sql
**旧站招标信息栏目数据**，1169条

### 7. 07_announcements_result.sql
**旧站中标公示栏目数据**，1131条（包含 1 条备份中没有的既有公告）

### 8. 08_announcements_correction.sql
**旧站变更公告栏目数据**，226条

### 9. 09_access_info_publications.sql

备份中的 43 篇真实资讯按正文重新分类：政策 26 篇（国家政策 13、地方政策 8、行业法规 5），新闻中心 17 篇（公司新闻 1、行业动态 1、通知公告 2、知识资讯 13）；另补充 2 条未重复的友情链接。

知识文章归入新闻中心的“知识资讯”分类，后台支持筛选和编辑；门户新闻列表和详情直接展示。

### 10. 10_legacy_company_media.sql

根据旧站公开原图恢复 4 项证照资料、6 组企业荣誉及其附件关联。过往造价资质、信用等级、管理体系认证标明历史资料，日期按原证书记载；重要业绩没有真实来源，保持空白。对应图片由 API 发布包中的 `SeedAssets/legacy` 补入 `/uploads/legacy/`，不依赖旧站在线服务。

`10_legacy_company_media.sql` 同时登记正文中的已恢复媒体，覆盖 29 个文件。关联类型使用后台支持的 `company_qualification`、`company_honor`、`company_profile` 和 `info_publication`；实际跨内容引用可通过后台附件管理查询。政策文号由 `info_publications.document_number` 可空列保存，既有正文不自动推断文号。

公告 SQL 已在生成阶段直接写入区域编码，首次导入后无需再执行区域转换脚本。

如需从本次生成的完整公告数据 `scripts/data/access-initial-announcements.json` 重新生成公告 SQL，可执行：

```bash
cd scripts
npm run generate-initial-sql
```

> 数据来源：henanhailong.com 的既有抓取数据与海隆数据库.mdb。按旧站 ID 保留 2375 条已清洗公告，补入 151 条备份公告，合计 **2526 条**。最新公告日期为 2026-09-24。
> 文件按旧站栏目拆分；实际 `notice_type` 还会依据标题识别结果/更正，因此数据库按公告类型统计的数量可能与文件行数不同。
> 原 MDB、公开内容中间 JSON 和完整公告生成数据均不提交；部署使用仓库中的 SQL 和 API 的 `SeedAssets/legacy` 初始化资源。

## 从 Access 备份重新生成

Windows 需要已安装的 Microsoft ACE OLEDB Provider。导出工具复制原文件后只读访问副本，仅导出公开内容，不读取管理员密码、留言或订单。

```powershell
powershell -File scripts/migration/export-access-public.ps1 -DatabasePath "D:/备份/海隆数据库.mdb" -OutputDirectory ".runtime/access-export"
cd scripts
npm ci
npm run generate-access-initial-sql -- --source ../.runtime/access-export --baseline data/final-data.json
```

`final-data.json` 是原 2375 条已清洗公告，不要将生成后的完整公告文件当作基线；该中间文件保留在本地 `scripts/data/`，不随仓库分发。没有原始输入时直接使用已生成 SQL 即可。

生成器同时更新 02、06–10、`initialization-source-report.json` 和 `initialization-field-review.json`，并核验初始化资源 SHA-256；重复执行输出一致。只有旧 2375 条数据时，普通公告生成命令会拒绝覆盖当前完整初始化数据。

## 分类和金额全量复核（2026-10-04）

本轮对全部 2526 条公告执行标题、正文及表格规则复核，对 43 篇资讯逐条确认现有页面分类，并针对金额异常、跨省和识别冲突抽查原文。不是仅补新增公告。

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

用户已授权对原来未分类的 5 条公告合理推断：旧 ID 2165 人居环境集中治理、799 电商进农村示范归服务；509/510 路演大厅采购归货物；2956 医共体整项目终止公告按预算占比更大的医疗设备包归货物（包 2 为 7960 万元，包 1 信息化服务为 5800 万元）。字段复核报告以 `inferred: true` 和 `rationale` 明示推断依据，保留混合包说明，不把推断的具体标的补写进原正文，也不修改金额。

### 金额字段及接口

- `budget_amount` / `budgetAmount`：只存明确预算。最高限价、控制价和项目投资额在证据报告中分别记录，不写入预算。
- `award_amount` / `awardAmount`：独立存最终中标或成交金额；首次部署表结构及公告 INSERT 已加入该字段。
- 两个字段沿用当前金额口径：万元、`DECIMAL(15,2)`。元除以 10000、亿元乘以 10000，原始数字、单位和出处另存于复核报告，避免丢失原文精度。
- 候选人报价、终止或流标结果、折扣费率、单价、代理费不填写中标金额。多标段无法确认单一总额时留空，不只取第一个标段。
- 已逐条确认完整范围的 40 条分包结果，按每包原始数值及单位精确汇总后统一舍入，报告附 `parts` 和 `scope`；相同金额的不同包各计一次，表格与正文重复表达只计一次。复核范围或金额项数发生变化时生成器报错。
- 明确预算变更采用“变更为”后的现行金额；例如旧 ID 1345 从 63.82 改为 51.68 万元。结果更正的新金额保留在报告 `notes`，更正公告不再计入结果总额；原公告需要改标段的旧 ID 384 同时关联更正来源 387。
- HTML 表格独立单位列优先于表头，“中标金额（元）”表头下的 `%` 不得作为元金额。最终获奖人报价不会越过正文小节读取其他投标人的报价。
- API 创建、列表、详情和更新均支持新字段；更新时字段省略保留原值，显式 `null` 清空金额，0 是有效值。管理端输入框清空时发送 `null`。
- 管理端政府采购和建设工程列表显示两类金额，结果公告编辑提供中标金额输入；门户详情分别显示预算和中标/成交金额。
- 首页总额和地区金额排行改为汇总结果公告的 `award_amount`，不再累计预算。这仍是公告口径，同一项目的重复发布与未确认总额不构成完整财务统计。

### 地区与可追溯性

地区优先使用正文项目地点，其次标题，再取采购人/招标人块；截断代理机构、财政部门和监督部门信息。完整省市约束区县，长区县名称优先，避免“北京大道”“中心城区”“太康县/康县”“清丰县/丰县”误识别。

补充现有字典遗漏的巩义市；编码依据[巩义市政府 2025 年“一卡通”清单](https://public.gongyishi.gov.cn/)，归属关系参考[巩义市政府历史沿革](https://www.gongyishi.gov.cn/lsyg.jhtml)。中原区 410102 和二七区 410103 根据[郑州市生态环境局重点排污单位名录](https://sthjj.zhengzhou.gov.cn/u/cms/sthjj/202005/04032833jv2m.pdf)补齐。其他地区继续使用项目现有字典，不把没有明确区县证据的公告默认写成市区。

[initialization-field-review.json](initialization-field-review.json) 包含每条公告的旧值、新值、是否变更、规则、证据片段、金额候选及待核查原因，并保留旧站 ID；顶部 `pendingAnnouncements` 为完整剩余清单。分类/地区在同名项目一致时补缺；不同包的对应关系经逐条确认后引用明确来源，不复制其他包的标的类型。金额不会跨公告复制。正常候选报价和明确跨区县范围留在 `notes`，不列为未解决冲突。

### 88 条待核查项的后续处理

用户授权修复可确认项后，原 88 条中 71 条已解决，17 条继续保留。金额复核另发现旧 ID 2180 校服单价被误读为总额，已清空错误金额并记录实际总额待确认，当时待核查共 18 条。随后用户授权合理推断归类，5 条采购子类已补齐，当前剩余 13 条涉及金额或地区；全部可正常初始化和查看正文。另补齐原先因标题仅写“结果公告”或金额标签写“中标（成交）金额”而未识别的最终成交金额；补齐 76 条、清除 1 处单价误填后，中标字段从 422 条增至 497 条。

| 剩余原因 | 条数 | 旧站 ID / 依据 |
| --- | ---: | --- |
| 缺少可靠项目城市 | 2 | 738、767 管理配套物资；仅有代理/开标地点及联系方式，不能当作交货或项目地点 |
| 单价/费率无法转成固定总额，或标段不完整 | 9 | 2180、1865、1723、1466、1444、641、595、574、384；每条报告说明不能合计的具体依据 |
| 原文金额单位冲突 | 1 | 1849 唐河矿山修复，表头写万元但原数字与元控制价数量级冲突；对应候选公示也没标报价单位，不能擅改 |
| 最终报价缺少单位 | 1 | 1193 都里镇农田，中标价仅写 2933422.62，保留数值证据而不猜元或万元 |

已确认完整两标段的旧 ID 618 合计 25299.23 万元；旧 ID 2869 合计 51.07 万元；旧 ID 2705 合计 45.35 万元。预算与中标金额分别存储。

复核脚本为 `scripts/review-announcement-fields.js`，逐条确认结论位于 `scripts/confirmed-announcement-reviews.js`，由 Access 初始化生成器统一调用。后续重新生成会复用复核规则。

## 图片与附件

MDB 未包含旧站 `uploadfile` 文件。原有 61 个文件引用中，已从原站恢复企业资料及新闻、政策的 28 张图片和 1 个 Word 附件；其余 32 个公告媒体文件仍列入待补清单，见 [initialization-source-report.json](initialization-source-report.json)。

- 无原始文件的本地图片引用已移除，缺失文档保留链接文字并标注“附件待补充”。
- 清除编辑器本机 `file:///` 图片、旧编辑器图标、脚本及事件属性。
- 保留正文中的 6 处 PNG 内嵌图片；外部图片和文档保留原外链，未核验外站可用性。
- 后端共用正文过滤器同步支持 `img.src` 中的限定栅格图片 base64；PNG 不再因 data URI 过滤而丢失。其他 data URI、SVG、脚本及事件属性保持过滤。
- 已恢复文件保存在 `BackEnd/HailongConsulting.API/SeedAssets/legacy`，来源 URL、大小和 SHA-256 见 `scripts/legacy-media-manifest.json`，企业证书人工核对字段见 `scripts/legacy-company-data.json`。原站标记 `.doc` 的附件实为 OpenXML Word 文档，本站按 `.docx` 提供。
- API 每次启动仅把缺失的初始化资源补入上传卷，不覆盖已存在文件。Docker 与本地部署均使用 `/uploads/legacy/`；发布与备份时保留初始化资源及整个上传目录。招聘页面尚未导入。
- 当前门户默认地址按用户确认设置为“雅宝·东方国际广场1号楼8层”。

## 执行顺序

**Docker 部署：** 文件已按执行顺序编号（00-10），SQL 目录整体挂载到 MySQL 初始化目录，空数据目录首次启动时会自动按顺序执行，无需额外导入。

**手动执行顺序：**

```bash
# 0. 设置字符集
mysql -u root -p < 00_set_charset.sql

# 1. 首先执行数据库结构文件
mysql -u root -p < 01_hailong_consulting_schema.sql

# 2. 执行主初始化数据文件
mysql -u root -p hailong_consulting < 02_hailong_consulting_init_data.sql

# 3. 执行河南、安徽区域初始化
mysql -u root -p hailong_consulting < 03_region_dictionary_batch1_henan.sql

# 4. 执行第二批区域初始化
mysql -u root -p hailong_consulting < 04_region_dictionary_batch2_north_east_east.sql

# 5. 执行第三批区域初始化
mysql -u root -p hailong_consulting < 05_region_dictionary_batch3_central_south_west.sql

# 6. 导入招标公告
mysql -u root -p hailong_consulting < 06_announcements_bidding.sql

# 7. 导入中标公示
mysql -u root -p hailong_consulting < 07_announcements_result.sql

# 8. 导入变更公告
mysql -u root -p hailong_consulting < 08_announcements_correction.sql

# 9. 导入真实资讯和补充友情链接
mysql -u root -p hailong_consulting < 09_access_info_publications.sql

# 10. 导入经原图核对的企业证照、荣誉与附件关联
mysql -u root -p hailong_consulting < 10_legacy_company_media.sql

```

或者在MySQL客户端中执行：

```sql
SOURCE /path/to/00_set_charset.sql;
SOURCE /path/to/01_hailong_consulting_schema.sql;
SOURCE /path/to/02_hailong_consulting_init_data.sql;
SOURCE /path/to/03_region_dictionary_batch1_henan.sql;
SOURCE /path/to/04_region_dictionary_batch2_north_east_east.sql;
SOURCE /path/to/05_region_dictionary_batch3_central_south_west.sql;
SOURCE /path/to/06_announcements_bidding.sql;
SOURCE /path/to/07_announcements_result.sql;
SOURCE /path/to/08_announcements_correction.sql;
SOURCE /path/to/09_access_info_publications.sql;
SOURCE /path/to/10_legacy_company_media.sql;
```

## 数据覆盖范围

执行完所有SQL文件后，将包含：

### 省级行政区（34个）
1. 河南省（已含详细区县数据）
2. 安徽省（16个地级市及现行区县）
3. 北京市、天津市、上海市、重庆市（4个直辖市）
4. 河北、山西、内蒙古（华北地区）
5. 辽宁、吉林、黑龙江（东北地区）
6. 江苏、浙江、福建、江西、山东（华东地区）
7. 湖北、湖南、广东、广西、海南（华中、华南地区）
8. 四川、贵州、云南、西藏（西南地区）
9. 陕西、甘肃、青海、宁夏、新疆（西北地区）
10. 香港、澳门、台湾（特别行政区）

### 市级行政区
- 全国所有地级市、自治州、地区
- 共计约340+个市级行政区

### 区县级行政区
- **河南省**：所有区县（完整数据）
- **全国各批次覆盖省级区域**：按国家行政区划编码整理，批次之间无重复编码
- **数据特点**：市辖区合并为"市区"，县单独列出
- 共计约1800+个区县数据

### 历史公告数据
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
4. **区县数据**：三批区域文件均按统一编码写入，河南、安徽为完整市县级数据
5. **数据更新**：行政区划可能会有调整，使用时请注意数据的时效性
6. **历史数据**：06、07、08 三个公告脚本包含从旧网站抓取的历史公告，数据来源为 henanhailong.com；公告 SQL 已直接写入区域编码，无需额外后处理脚本

## 数据格式说明

### 区县数据格式
本项目采用以下格式处理区县数据：

1. **市辖区表示**：现有字典大多使用"XX市区"汇总项；本次明确需要的中原区、二七区独立补充，正文明确区名时存具体区编码，不再映射成市区汇总项。
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

## 技术支持

如有问题，请参考：
- 数据库设计文档
- 项目README.md
- 或联系技术支持团队

---

**最后更新时间**：2026-10-04

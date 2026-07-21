-- ============================================
-- 公告区域字段统一为行政区划编码
--
-- 规则：
--   announcements.province -> 省级 region_code
--   announcements.city     -> 市级 region_code
--   announcements.district -> 区县级 region_code
--
-- project_region 保留为展示文本，不参与筛选和关联。
-- 无法映射到区域字典或层级关系不正确的值会被清空，避免继续混存名称。
-- 本脚本可重复执行。
-- ============================================

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

USE `hailong_consulting`;

START TRANSACTION;

-- 旧站清洗程序曾把所有公告的省份默认成河南省。
-- 这里只按已经由标题及项目建设地点确认的项目系列覆盖区域，不能仅凭正文中抽出的
-- 区县名称反推省份，否则供应商/中标候选人地址会把河南项目错误迁移到外省。
DROP TEMPORARY TABLE IF EXISTS `announcement_region_overrides`;
CREATE TEMPORARY TABLE `announcement_region_overrides` (
    `title_prefix` VARCHAR(255) NOT NULL PRIMARY KEY,
    `province_code` VARCHAR(50) NOT NULL,
    `city_code` VARCHAR(50) NOT NULL,
    `district_code` VARCHAR(50) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `announcement_region_overrides`
    (`title_prefix`, `province_code`, `city_code`, `district_code`)
VALUES
    ('中国烟草总公司郑州烟草研究院北京实验室', '110000', '110100', '110101'),
    ('赞皇县龙门岗45MW分散式风力发电项目', '130000', '130100', '130129'),
    ('内蒙古清源环保有限公司年产20000吨纯三氯化铁净水剂建设项目', '150000', '150600', '150624'),
    ('临河IT创业园（众城新都汇中央公馆）工程建设项目', '150000', '150800', '150801'),
    ('辽宁营口盖州市32.61MWp地面分布式光伏发电项目', '210000', '210800', '210881'),
    ('中垭（浙江）智能科技有限公司钢结构配件及系统窗生产基地基本建设项目', '330000', '331000', NULL),
    ('合肥新质产业总部中心项目', '340000', '340100', '340101'),
    ('湖北黄冈黄梅平北46MW分散式风电项目', '420000', '421100', '421127'),
    ('岚皋县茶溪山庄康养项目', '610000', '610900', '610925'),
    ('武山杨河45MW分散式风电项目', '620000', '620500', '620524');

UPDATE `announcements` AS a
INNER JOIN `announcement_region_overrides` AS region_override
    ON a.`title` LIKE CONCAT(region_override.`title_prefix`, '%')
INNER JOIN `region_dictionary` AS override_province
    ON override_province.`region_code` = region_override.`province_code`
   AND override_province.`region_level` = 1
   AND override_province.`is_deleted` = 0
INNER JOIN `region_dictionary` AS override_city
    ON override_city.`region_code` = region_override.`city_code`
   AND override_city.`parent_code` = override_province.`region_code`
   AND override_city.`region_level` = 2
   AND override_city.`is_deleted` = 0
LEFT JOIN `region_dictionary` AS override_district
    ON override_district.`region_code` = region_override.`district_code`
   AND override_district.`parent_code` = override_city.`region_code`
   AND override_district.`region_level` = 3
   AND override_district.`is_deleted` = 0
SET a.`province` = override_province.`region_code`,
    a.`city` = override_city.`region_code`,
    a.`district` = override_district.`region_code`,
    a.`project_region` = CONCAT_WS(
        ' ',
        override_province.`region_name`,
        override_city.`region_name`,
        override_district.`region_name`
    );

DROP TEMPORARY TABLE `announcement_region_overrides`;

-- 先将可识别的省份名称转换为省级编码。
UPDATE `announcements` AS a
INNER JOIN `region_dictionary` AS province
    ON province.`region_name` = a.`province`
   AND province.`region_level` = 1
   AND province.`is_deleted` = 0
SET a.`province` = province.`region_code`
WHERE a.`province` IS NOT NULL
  AND a.`province` <> '';

-- 同一省份内，标题明确包含区县全名时，可用区县安全地纠正旧清洗程序误判的城市。
-- 例如“固始县……”曾因正文含郑州代理机构地址而被标记为“郑州市 + 固始县”。
-- 不跨省、不使用正文地址，避免将供应商或候选人所在地当成项目所在地。
UPDATE `announcements` AS a
INNER JOIN (
    SELECT
        province.`region_code` AS `province_code`,
        district.`region_name` AS `district_name`,
        MIN(city.`region_code`) AS `city_code`,
        MIN(district.`region_code`) AS `district_code`
    FROM `region_dictionary` AS district
    INNER JOIN `region_dictionary` AS city
        ON city.`region_code` = district.`parent_code`
       AND city.`region_level` = 2
       AND city.`is_deleted` = 0
    INNER JOIN `region_dictionary` AS province
        ON province.`region_code` = city.`parent_code`
       AND province.`region_level` = 1
       AND province.`is_deleted` = 0
    WHERE district.`region_level` = 3
      AND district.`is_deleted` = 0
    GROUP BY province.`region_code`, district.`region_name`
    HAVING COUNT(*) = 1
) AS resolved_district
    ON resolved_district.`province_code` = a.`province`
   AND resolved_district.`district_name` = a.`district`
SET a.`city` = resolved_district.`city_code`,
    a.`district` = resolved_district.`district_code`
WHERE a.`title` LIKE CONCAT('%', resolved_district.`district_name`, '%');

-- 城市名称必须在公告所属省份下匹配，避免同名区域误转换。
UPDATE `announcements` AS a
INNER JOIN `region_dictionary` AS province
    ON province.`region_code` = a.`province`
   AND province.`region_level` = 1
   AND province.`is_deleted` = 0
INNER JOIN `region_dictionary` AS city
    ON city.`region_name` = a.`city`
   AND city.`parent_code` = province.`region_code`
   AND city.`region_level` = 2
   AND city.`is_deleted` = 0
SET a.`city` = city.`region_code`
WHERE a.`city` IS NOT NULL
  AND a.`city` <> '';

-- 区县名称必须在公告所属城市下匹配。
UPDATE `announcements` AS a
INNER JOIN `region_dictionary` AS city
    ON city.`region_code` = a.`city`
   AND city.`region_level` = 2
   AND city.`is_deleted` = 0
INNER JOIN `region_dictionary` AS district
    ON district.`region_name` = a.`district`
   AND district.`parent_code` = city.`region_code`
   AND district.`region_level` = 3
   AND district.`is_deleted` = 0
SET a.`district` = district.`region_code`
WHERE a.`district` IS NOT NULL
  AND a.`district` <> '';

-- 当区域字典将城市所辖城区合并为“XX市区”节点时（编码通常以 01 结尾），
-- 历史公告使用的“市区”或字典中没有单独节点的城区/开发区名称统一归入该节点。
-- project_region 条件用于恢复上一版脚本已经清空 district、但仍保留完整展示地址的数据。
UPDATE `announcements` AS a
INNER JOIN `region_dictionary` AS city
    ON city.`region_code` = a.`city`
   AND city.`region_level` = 2
   AND city.`is_deleted` = 0
INNER JOIN `region_dictionary` AS urban_district
    ON urban_district.`region_code` = CONCAT(LEFT(city.`region_code`, 4), '01')
   AND urban_district.`parent_code` = city.`region_code`
   AND urban_district.`region_level` = 3
   AND urban_district.`is_deleted` = 0
SET a.`district` = urban_district.`region_code`
WHERE a.`district` = '市区'
   OR a.`district` LIKE '%区'
   OR (a.`district` IS NULL AND TRIM(a.`project_region`) REGEXP '区$');

-- 空字符串统一为 NULL。
UPDATE `announcements`
SET `province` = NULLIF(TRIM(`province`), ''),
    `city` = NULLIF(TRIM(`city`), ''),
    `district` = NULLIF(TRIM(`district`), '');

-- 清理无法映射或层级不正确的数据，确保三个字段只保留合法编码。
UPDATE `announcements` AS a
LEFT JOIN `region_dictionary` AS province
    ON province.`region_code` = a.`province`
   AND province.`region_level` = 1
   AND province.`is_deleted` = 0
SET a.`province` = NULL,
    a.`city` = NULL,
    a.`district` = NULL
WHERE a.`province` IS NOT NULL
  AND province.`id` IS NULL;

UPDATE `announcements` AS a
LEFT JOIN `region_dictionary` AS city
    ON city.`region_code` = a.`city`
   AND city.`parent_code` = a.`province`
   AND city.`region_level` = 2
   AND city.`is_deleted` = 0
SET a.`city` = NULL,
    a.`district` = NULL
WHERE a.`city` IS NOT NULL
  AND city.`id` IS NULL;

UPDATE `announcements` AS a
LEFT JOIN `region_dictionary` AS district
    ON district.`region_code` = a.`district`
   AND district.`parent_code` = a.`city`
   AND district.`region_level` = 3
   AND district.`is_deleted` = 0
SET a.`district` = NULL
WHERE a.`district` IS NOT NULL
  AND district.`id` IS NULL;

COMMIT;

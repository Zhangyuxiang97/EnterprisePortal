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

-- 先将可识别的省份名称转换为省级编码。
UPDATE `announcements` AS a
INNER JOIN `region_dictionary` AS province
    ON province.`region_name` = a.`province`
   AND province.`region_level` = 1
   AND province.`is_deleted` = 0
SET a.`province` = province.`region_code`
WHERE a.`province` IS NOT NULL
  AND a.`province` <> '';

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

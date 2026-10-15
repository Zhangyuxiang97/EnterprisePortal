-- 省级字典默认采用六位行政区划代码顺序；保留区域编码、记录 ID 和下级排序。
-- 窗口函数物化排名，避免直接更新目标表的自查询限制。
UPDATE region_dictionary AS region
JOIN (
    SELECT id, ROW_NUMBER() OVER (ORDER BY region_code) AS default_order
    FROM region_dictionary
    WHERE region_level = 1 AND is_deleted = 0
) AS ranked ON ranked.id = region.id
SET region.sort_order = ranked.default_order
WHERE region.sort_order <> ranked.default_order;

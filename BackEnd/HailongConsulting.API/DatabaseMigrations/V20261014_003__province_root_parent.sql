-- 重庆、四川等旧省级数据使用字符串 '0' 表示顶层，统一为 NULL。
UPDATE region_dictionary
SET parent_code = NULL
WHERE region_level = 1 AND parent_code = '0';

-- 高德 GCJ-02：雅宝东方国际广场 1 号楼（河南自贸大厦），位于心怡路东侧。
-- 修正前一版误落东方国际花园的默认标记；仅匹配该默认坐标及广场 1 号楼地址。
-- 保留管理员自定义坐标、其他地址及其余门户配置。
UPDATE portal_site_settings
SET content_json = JSON_SET(content_json,
        '$.contact.map.longitude', 113.772030,
        '$.contact.map.latitude', 34.768478),
    version = REPLACE(UUID(), '-', '')
WHERE JSON_VALID(content_json)
  AND ABS(CAST(JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.map.longitude')) AS DECIMAL(12,6)) - 113.774980) < 0.000001
  AND ABS(CAST(JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.map.latitude')) AS DECIMAL(12,6)) - 34.768861) < 0.000001
  AND JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.address.fullAddress')) LIKE '%雅宝%东方国际广场%1号楼%';

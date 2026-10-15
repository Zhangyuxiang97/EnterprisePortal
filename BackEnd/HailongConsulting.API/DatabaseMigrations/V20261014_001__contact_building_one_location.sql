-- 高德 GCJ-02：修正雅宝·东方国际广场 1 号楼的旧默认定位。
-- 仅修正仍使用旧默认坐标且地址为 1 号楼的设置；保留管理员自定义坐标。
UPDATE portal_site_settings
SET content_json = JSON_SET(content_json,
        '$.contact.map.longitude', 113.774980,
        '$.contact.map.latitude', 34.768861),
    version = REPLACE(UUID(), '-', '')
WHERE JSON_VALID(content_json)
  AND ABS(CAST(JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.map.longitude')) AS DECIMAL(12,6)) - 113.773301) < 0.000001
  AND ABS(CAST(JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.map.latitude')) AS DECIMAL(12,6)) - 34.768953) < 0.000001
  AND JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.address.fullAddress')) LIKE '%雅宝%'
  AND JSON_UNQUOTE(JSON_EXTRACT(content_json, '$.contact.address.fullAddress')) LIKE '%1号楼%';

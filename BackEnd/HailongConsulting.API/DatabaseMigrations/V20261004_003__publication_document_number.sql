-- 新库已含此列；旧库只补充可空文号，不修改既有正文。
SET @column_exists = (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = DATABASE() AND table_name = 'info_publications' AND column_name = 'document_number'
);
SET @sql = IF(@column_exists = 0,
    'ALTER TABLE info_publications ADD COLUMN document_number VARCHAR(100) NULL', 'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @version_exists = (SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='portal_site_settings' AND column_name='version');
SET @version_sql = IF(@version_exists=0, 'ALTER TABLE portal_site_settings ADD COLUMN version VARCHAR(32) NOT NULL DEFAULT ''''', 'SELECT 1');
PREPARE version_statement FROM @version_sql;
EXECUTE version_statement;
DEALLOCATE PREPARE version_statement;
UPDATE portal_site_settings SET version = REPLACE(UUID(), '-', '') WHERE version = '';

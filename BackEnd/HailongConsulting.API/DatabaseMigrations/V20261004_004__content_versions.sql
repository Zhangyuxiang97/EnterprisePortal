-- 内容版本独立于访问统计，兼容已存在与首次初始化的数据库。

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='announcements' AND column_name='version') = 0, 'ALTER TABLE `announcements` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='info_publications' AND column_name='version') = 0, 'ALTER TABLE `info_publications` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='company_profile' AND column_name='version') = 0, 'ALTER TABLE `company_profile` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='business_scope' AND column_name='version') = 0, 'ALTER TABLE `business_scope` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='company_qualifications' AND column_name='version') = 0, 'ALTER TABLE `company_qualifications` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='company_honors' AND column_name='version') = 0, 'ALTER TABLE `company_honors` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='major_achievements' AND column_name='version') = 0, 'ALTER TABLE `major_achievements` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='carousel_banners' AND column_name='version') = 0, 'ALTER TABLE `carousel_banners` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

SET @content_version_sql = IF((SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='friendly_links' AND column_name='version') = 0, 'ALTER TABLE `friendly_links` ADD COLUMN `version` VARCHAR(32) NOT NULL DEFAULT ''initial''', 'SELECT 1');
PREPARE content_version_statement FROM @content_version_sql;
EXECUTE content_version_statement;
DEALLOCATE PREPARE content_version_statement;

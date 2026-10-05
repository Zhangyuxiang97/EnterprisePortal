-- 在 00–10 全部导入后执行，任一核心初始化断言失败即返回 SQL 错误。
USE hailong_consulting;
DELIMITER //
CREATE PROCEDURE verify_initial_data()
BEGIN
  IF (SELECT COUNT(*) FROM announcements) <> 2526 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '公告数量不符';
  END IF;
  IF (SELECT COUNT(*) FROM info_publications) <> 43 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '资讯数量不符';
  END IF;
  IF EXISTS (SELECT 1 FROM announcements WHERE business_type = 'GOV_PROCUREMENT'
      AND (procurement_type IS NULL OR procurement_type NOT IN ('goods','service','project'))) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '政府采购子类未完成归档';
  END IF;
  IF (SELECT COUNT(*) FROM info_publications WHERE type = 'COMPANY_NEWS' AND category = '知识资讯') <> 13 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '知识资讯分类不符';
  END IF;
  IF (SELECT COUNT(*) FROM info_publications WHERE type = 'POLICY_REGULATION') <> 26 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '政策法规数量不符';
  END IF;
  IF EXISTS (SELECT 1 FROM info_publications WHERE
    (type = 'COMPANY_NEWS' AND category NOT IN ('公司新闻','行业动态','通知公告','知识资讯')) OR
    (type = 'POLICY_REGULATION' AND category NOT IN ('国家政策','地方政策','行业法规'))) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '资讯分类不在当前页面选项中';
  END IF;
  IF EXISTS (SELECT 1 FROM announcements WHERE award_amount IS NOT NULL AND
    (notice_type <> 'result' OR title REGEXP '候选人|流标|废标|终止|失败')) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '非最终结果不应填中标金额';
  END IF;
  IF (SELECT COUNT(*) FROM announcements WHERE award_amount IS NOT NULL) = 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '独立中标金额未初始化';
  END IF;
  IF NOT EXISTS (SELECT 1 FROM announcements
    WHERE title = '息县2022年乡村振兴示范村人居环境改善提升项目规划设计变更公告' AND budget_amount = 51.68) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '变更后的预算未采用新值';
  END IF;
  IF NOT EXISTS (SELECT 1 FROM announcements
    WHERE title = '漯河市市场监督管理局城乡一体化示范区分局2026年食品安全抽检项目成交公告' AND award_amount = 45.35) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '完整分包结果合计不符';
  END IF;
  IF EXISTS (SELECT 1 FROM announcements
    WHERE title = '固始县人民医院新院信息化采购项目中标公示' AND award_amount IS NOT NULL) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '混合费率结果不应填写固定总额';
  END IF;
  IF (SELECT COUNT(*) FROM announcements WHERE title IN
    ('中原区柿园村拆迁安置地块围挡搭建项目竞争性磋商公告','中原区柿园村拆迁安置地块围挡搭建项目中标公告')
    AND province = '410000' AND city = '410100' AND district = '410102') <> 2 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '中原区项目地区补全不符';
  END IF;
  IF EXISTS (SELECT 1 FROM announcements a LEFT JOIN region_dictionary city ON a.city = city.region_code
    WHERE a.city IS NOT NULL AND (city.region_code IS NULL OR city.parent_code <> a.province)) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '公告省市层级不符';
  END IF;
  IF EXISTS (SELECT 1 FROM announcements a LEFT JOIN region_dictionary district ON a.district = district.region_code
    WHERE a.district IS NOT NULL AND (district.region_code IS NULL OR district.parent_code <> a.city)) THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '公告市区县层级不符';
  END IF;
  IF (SELECT COUNT(*) FROM friendly_links) <> 7 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '友情链接数量不符';
  END IF;
  IF (SELECT COUNT(*) FROM company_profile WHERE content LIKE '%原名河南海隆%') <> 1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '真实企业简介缺失';
  END IF;
  IF (SELECT COUNT(*) FROM users) <> 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '不应导入旧账户';
  END IF;
  IF (SELECT COUNT(*) FROM company_qualifications) <> 4 OR (SELECT COUNT(*) FROM company_honors) <> 6 OR (SELECT COUNT(*) FROM major_achievements) <> 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '真实证照荣誉数量不符，不应发布演示业绩';
  END IF;
  IF EXISTS (SELECT 1 FROM info_publications WHERE summary REGEXP '@font-face|mso-style|<style|<p>') THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '资讯摘要不得含 Word 样式或 HTML';
  END IF;
  IF NOT EXISTS (SELECT 1 FROM info_publications WHERE title = '河南招标采购综合网关于停止发布招标公告及公示信息的通知'
      AND content LIKE '%/uploads/legacy/20190801163902403.jpg%' AND content LIKE '%/uploads/legacy/20190801163918971.jpg%') THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '停止发布公告通知扫描件未恢复';
  END IF;
  IF (SELECT COUNT(DISTINCT file_url) FROM attachments WHERE file_url LIKE '/uploads/legacy/%') <> 29 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = '企业证书附件关联不完整';
  END IF;
END//
DELIMITER ;
CALL verify_initial_data();
DROP PROCEDURE verify_initial_data;
SELECT '初始化数据核验通过' AS result;

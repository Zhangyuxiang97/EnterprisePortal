-- 旧站公开证书原图核对恢复，仅用于空库初始化；历史证书不推断续期。
SET NAMES utf8mb4;
USE `hailong_consulting`;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20230804135209174.jpg', 'uploads/legacy/20230804135209174.jpg', '/uploads/legacy/20230804135209174.jpg', 138129, 'image/jpeg', '.jpg', 'image', 'company_qualification');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_qualifications (name, description, image_id, certificate_no, issuing_authority, issue_date, expiry_date, sort_order, status) VALUES ('工程监理资质证书（乙级）', '原证书记载市政公用工程、机电安装工程、房屋建筑工程、化工石油工程、电力工程监理乙级；证载有效期至 2028 年 1 月 17 日。', @legacy_image_id, 'E341039884', '河南省住房和城乡建设厅', '2023-02-17', '2028-01-17', 1, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20230804135140469.jpg', 'uploads/legacy/20230804135140469.jpg', '/uploads/legacy/20230804135140469.jpg', 135524, 'image/jpeg', '.jpg', 'image', 'company_qualification');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_qualifications (name, description, image_id, certificate_no, issuing_authority, issue_date, expiry_date, sort_order, status) VALUES ('营业执照', '海隆工程咨询有限公司营业执照存档，原件记载成立日期为 2018 年 9 月 12 日，营业期限长期。图片为旧站留存版本。', @legacy_image_id, NULL, NULL, NULL, NULL, 2, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20230804135156185.jpg', 'uploads/legacy/20230804135156185.jpg', '/uploads/legacy/20230804135156185.jpg', 138311, 'image/jpeg', '.jpg', 'image', 'company_qualification');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_qualifications (name, description, image_id, certificate_no, issuing_authority, issue_date, expiry_date, sort_order, status) VALUES ('工程造价咨询企业乙级资质证书（历史资料）', '旧站留存的暂定乙级资质证书，有效期为 2020 年 12 月 11 日至 2021 年 12 月 10 日。作为历史资料展示。', @legacy_image_id, '暂乙002041013123', '河南省住房和城乡建设厅', '2020-12-11', '2021-12-10', 3, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20250327101407320.jpg', 'uploads/legacy/20250327101407320.jpg', '/uploads/legacy/20250327101407320.jpg', 137902, 'image/jpeg', '.jpg', 'image', 'company_qualification');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_qualifications (name, description, image_id, certificate_no, issuing_authority, issue_date, expiry_date, sort_order, status) VALUES ('质量、环境、职业健康安全管理体系认证（历史资料）', '旧站三项管理体系认证证书原图，包含质量管理、环境管理和职业健康安全管理体系。原图为历史版本，续期信息待补充。', @legacy_image_id, NULL, NULL, NULL, NULL, 4, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20230804104937173.jpg', 'uploads/legacy/20230804104937173.jpg', '/uploads/legacy/20230804104937173.jpg', 174484, 'image/jpeg', '.jpg', 'image', 'company_honor');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_honors (name, description, image_id, award_organization, award_date, certificate_no, sort_order, status) VALUES ('河南省建设工程招标投标协会副会长单位', '旧站留存的副会长单位证书，颁发日期为 2020 年 9 月 28 日。', @legacy_image_id, '河南省建设工程招标投标协会', '2020-09-28', NULL, 1, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20250327100101368.jpg', 'uploads/legacy/20250327100101368.jpg', '/uploads/legacy/20250327100101368.jpg', 197911, 'image/jpeg', '.jpg', 'image', 'company_honor');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_honors (name, description, image_id, award_organization, award_date, certificate_no, sort_order, status) VALUES ('2018—2024 年度招标投标行业诚实守信单位', '连续七年诚实守信单位荣誉证书汇编，涵盖 2018 至 2024 年度。', @legacy_image_id, '河南省建设工程招标投标协会', NULL, NULL, 2, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20250327100127543.jpg', 'uploads/legacy/20250327100127543.jpg', '/uploads/legacy/20250327100127543.jpg', 190187, 'image/jpeg', '.jpg', 'image', 'company_honor');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_honors (name, description, image_id, award_organization, award_date, certificate_no, sort_order, status) VALUES ('2020—2023 年度优秀会员单位', '连续四年优秀会员单位荣誉证书汇编，涵盖 2020 至 2023 年度。', @legacy_image_id, '河南省建设工程招标投标协会', NULL, NULL, 3, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20250327100856480.jpg', 'uploads/legacy/20250327100856480.jpg', '/uploads/legacy/20250327100856480.jpg', 168205, 'image/jpeg', '.jpg', 'image', 'company_honor');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_honors (name, description, image_id, award_organization, award_date, certificate_no, sort_order, status) VALUES ('2023 年度优秀工程监理企业', '2023 年度河南省建设监理行业优秀工程监理企业；同页留存优秀总监理工程师荣誉证书。', @legacy_image_id, '河南省建设监理协会', '2024-07-18', NULL, 4, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20250327101200712.jpg', 'uploads/legacy/20250327101200712.jpg', '/uploads/legacy/20250327101200712.jpg', 185685, 'image/jpeg', '.jpg', 'image', 'company_honor');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_honors (name, description, image_id, award_organization, award_date, certificate_no, sort_order, status) VALUES ('AAA 级信用企业（2024—2025 年证书）', '企业信用等级证书存档，原图记载有效期为 2024 年 3 月 4 日至 2025 年 3 月 4 日。作为历史荣誉展示。', @legacy_image_id, NULL, NULL, NULL, 5, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES ('20250327101540383.jpg', 'uploads/legacy/20250327101540383.jpg', '/uploads/legacy/20250327101540383.jpg', 197862, 'image/jpeg', '.jpg', 'image', 'company_honor');
SET @legacy_image_id = LAST_INSERT_ID();
INSERT INTO company_honors (name, description, image_id, award_organization, award_date, certificate_no, sort_order, status) VALUES ('爱心企业与慈善捐赠证书', '旧站留存的爱心企业荣誉及慈善捐赠证书原图。', @legacy_image_id, NULL, NULL, NULL, 6, 1);
UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143531807.jpg', 'uploads/legacy/20190514143531807.jpg', '/uploads/legacy/20190514143531807.jpg', 88966, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143531807.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143531807.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143531807.jpg', 'uploads/legacy/20190514143531807.jpg', '/uploads/legacy/20190514143531807.jpg', 88966, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143531807.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143531807.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143542760.jpg', 'uploads/legacy/20190514143542760.jpg', '/uploads/legacy/20190514143542760.jpg', 71096, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143542760.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143542760.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143542760.jpg', 'uploads/legacy/20190514143542760.jpg', '/uploads/legacy/20190514143542760.jpg', 71096, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143542760.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143542760.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143550674.jpg', 'uploads/legacy/20190514143550674.jpg', '/uploads/legacy/20190514143550674.jpg', 60470, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143550674.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143550674.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143550674.jpg', 'uploads/legacy/20190514143550674.jpg', '/uploads/legacy/20190514143550674.jpg', 60470, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143550674.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143550674.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143559600.docx', 'uploads/legacy/20190514143559600.docx', '/uploads/legacy/20190514143559600.docx', 15866, 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', '.docx', 'document', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143559600.docx', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143559600.docx' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190514143559600.docx', 'uploads/legacy/20190514143559600.docx', '/uploads/legacy/20190514143559600.docx', 15866, 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', '.docx', 'document', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190514143559600.docx', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190514143559600.docx' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155655558.jpg', 'uploads/legacy/20190516155655558.jpg', '/uploads/legacy/20190516155655558.jpg', 45459, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155655558.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155655558.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155655558.jpg', 'uploads/legacy/20190516155655558.jpg', '/uploads/legacy/20190516155655558.jpg', 45459, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155655558.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155655558.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155704881.jpg', 'uploads/legacy/20190516155704881.jpg', '/uploads/legacy/20190516155704881.jpg', 12951, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155704881.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155704881.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155704881.jpg', 'uploads/legacy/20190516155704881.jpg', '/uploads/legacy/20190516155704881.jpg', 12951, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155704881.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155704881.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155712340.jpg', 'uploads/legacy/20190516155712340.jpg', '/uploads/legacy/20190516155712340.jpg', 66166, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155712340.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155712340.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155712340.jpg', 'uploads/legacy/20190516155712340.jpg', '/uploads/legacy/20190516155712340.jpg', 66166, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155712340.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155712340.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155720371.jpg', 'uploads/legacy/20190516155720371.jpg', '/uploads/legacy/20190516155720371.jpg', 68055, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155720371.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155720371.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155720371.jpg', 'uploads/legacy/20190516155720371.jpg', '/uploads/legacy/20190516155720371.jpg', 68055, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155720371.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155720371.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155728564.jpg', 'uploads/legacy/20190516155728564.jpg', '/uploads/legacy/20190516155728564.jpg', 62665, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155728564.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155728564.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155728564.jpg', 'uploads/legacy/20190516155728564.jpg', '/uploads/legacy/20190516155728564.jpg', 62665, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155728564.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155728564.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155736600.jpg', 'uploads/legacy/20190516155736600.jpg', '/uploads/legacy/20190516155736600.jpg', 67476, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155736600.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155736600.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155736600.jpg', 'uploads/legacy/20190516155736600.jpg', '/uploads/legacy/20190516155736600.jpg', 67476, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155736600.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155736600.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155800960.jpg', 'uploads/legacy/20190516155800960.jpg', '/uploads/legacy/20190516155800960.jpg', 66705, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155800960.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155800960.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155800960.jpg', 'uploads/legacy/20190516155800960.jpg', '/uploads/legacy/20190516155800960.jpg', 66705, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155800960.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155800960.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155903615.jpg', 'uploads/legacy/20190516155903615.jpg', '/uploads/legacy/20190516155903615.jpg', 60020, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155903615.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155903615.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155903615.jpg', 'uploads/legacy/20190516155903615.jpg', '/uploads/legacy/20190516155903615.jpg', 60020, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155903615.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155903615.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155911638.jpg', 'uploads/legacy/20190516155911638.jpg', '/uploads/legacy/20190516155911638.jpg', 66402, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155911638.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155911638.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155911638.jpg', 'uploads/legacy/20190516155911638.jpg', '/uploads/legacy/20190516155911638.jpg', 66402, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155911638.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155911638.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155919102.jpg', 'uploads/legacy/20190516155919102.jpg', '/uploads/legacy/20190516155919102.jpg', 57610, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155919102.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155919102.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190516155919102.jpg', 'uploads/legacy/20190516155919102.jpg', '/uploads/legacy/20190516155919102.jpg', 57610, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190516155919102.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190516155919102.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190801163902403.jpg', 'uploads/legacy/20190801163902403.jpg', '/uploads/legacy/20190801163902403.jpg', 79635, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190801163902403.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190801163902403.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190801163902403.jpg', 'uploads/legacy/20190801163902403.jpg', '/uploads/legacy/20190801163902403.jpg', 79635, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190801163902403.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190801163902403.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190801163918971.jpg', 'uploads/legacy/20190801163918971.jpg', '/uploads/legacy/20190801163918971.jpg', 24649, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190801163918971.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190801163918971.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20190801163918971.jpg', 'uploads/legacy/20190801163918971.jpg', '/uploads/legacy/20190801163918971.jpg', 24649, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20190801163918971.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20190801163918971.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20240808173501766.jpg', 'uploads/legacy/20240808173501766.jpg', '/uploads/legacy/20240808173501766.jpg', 182930, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20240808173501766.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20240808173501766.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20240808173501766.jpg', 'uploads/legacy/20240808173501766.jpg', '/uploads/legacy/20240808173501766.jpg', 182930, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20240808173501766.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20240808173501766.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20240808173623235.jpg', 'uploads/legacy/20240808173623235.jpg', '/uploads/legacy/20240808173623235.jpg', 194332, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20240808173623235.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20240808173623235.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20240808173623235.jpg', 'uploads/legacy/20240808173623235.jpg', '/uploads/legacy/20240808173623235.jpg', 194332, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20240808173623235.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20240808173623235.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20250711095148141.jpg', 'uploads/legacy/20250711095148141.jpg', '/uploads/legacy/20250711095148141.jpg', 194796, 'image/jpeg', '.jpg', 'image', 'company_profile', source.id FROM company_profile source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20250711095148141.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20250711095148141.jpg' AND a.related_type = 'company_profile' AND a.related_id = source.id AND a.is_deleted = 0);

INSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)
SELECT '20250711095148141.jpg', 'uploads/legacy/20250711095148141.jpg', '/uploads/legacy/20250711095148141.jpg', 194796, 'image/jpeg', '.jpg', 'image', 'info_publication', source.id FROM info_publications source
WHERE source.is_deleted = 0 AND LOCATE('/uploads/legacy/20250711095148141.jpg', source.content) > 0
AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = '/uploads/legacy/20250711095148141.jpg' AND a.related_type = 'info_publication' AND a.related_id = source.id AND a.is_deleted = 0);

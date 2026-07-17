-- ============================================================
-- 辽宁省各区县数据
-- region_level = 3 (区县级)
-- 规则：市区统一标记为"XX市区"，开发功能区归入市区
-- ============================================================

-- 沈阳市 (210100) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210101', '沈阳市区', 3, '210100', 1),
('210112', '浑南区', 3, '210100', 2),
('210113', '沈北新区', 3, '210100', 3),
('210114', '于洪区', 3, '210100', 4),
('210115', '辽中区', 3, '210100', 5),
('210123', '康平县', 3, '210100', 6),
('210124', '法库县', 3, '210100', 7),
('210181', '新民市', 3, '210100', 8);

-- 大连市 (210200) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210201', '大连市区', 3, '210200', 1),
('210212', '旅顺口区', 3, '210200', 2),
('210213', '金州区', 3, '210200', 3),
('210224', '长海县', 3, '210200', 4),
('210281', '瓦房店市', 3, '210200', 5),
('210282', '普兰店区', 3, '210200', 6),
('210283', '庄河市', 3, '210200', 7);

-- 鞍山市 (210300) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210301', '鞍山市区', 3, '210300', 1),
('210321', '台安县', 3, '210300', 2),
('210323', '岫岩满族自治县', 3, '210300', 3),
('210381', '海城市', 3, '210300', 4);

-- 抚顺市 (210400) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210401', '抚顺市区', 3, '210400', 1),
('210421', '抚顺县', 3, '210400', 2),
('210422', '新宾满族自治县', 3, '210400', 3),
('210423', '清原满族自治县', 3, '210400', 4);

-- 本溪市 (210500) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210501', '本溪市区', 3, '210500', 1),
('210521', '本溪满族自治县', 3, '210500', 2),
('210522', '桓仁满族自治县', 3, '210500', 3);

-- 丹东市 (210600) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210601', '丹东市区', 3, '210600', 1),
('210624', '宽甸满族自治县', 3, '210600', 2),
('210681', '东港市', 3, '210600', 3),
('210682', '凤城市', 3, '210600', 4);

-- 锦州市 (210700) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210701', '锦州市区', 3, '210700', 1),
('210726', '黑山县', 3, '210700', 2),
('210727', '义县', 3, '210700', 3),
('210781', '凌海市', 3, '210700', 4),
('210782', '北镇市', 3, '210700', 5);

-- 营口市 (210800) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210801', '营口市区', 3, '210800', 1),
('210881', '盖州市', 3, '210800', 2),
('210882', '大石桥市', 3, '210800', 3);

-- 阜新市 (210900) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210901', '阜新市区', 3, '210900', 1),
('210921', '阜新蒙古族自治县', 3, '210900', 2),
('210922', '彰武县', 3, '210900', 3);

-- 辽阳市 (211000) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211001', '辽阳市区', 3, '211000', 1),
('211021', '辽阳县', 3, '211000', 2),
('211081', '灯塔市', 3, '211000', 3);

-- 盘锦市 (211100) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211101', '盘锦市区', 3, '211100', 1),
('211122', '盘山县', 3, '211100', 2);

-- 铁岭市 (211200) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211201', '铁岭市区', 3, '211200', 1),
('211221', '铁岭县', 3, '211200', 2),
('211223', '西丰县', 3, '211200', 3),
('211224', '昌图县', 3, '211200', 4),
('211281', '调兵山市', 3, '211200', 5),
('211282', '开原市', 3, '211200', 6);

-- 朝阳市 (211300) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211301', '朝阳市区', 3, '211300', 1),
('211321', '朝阳县', 3, '211300', 2),
('211322', '建平县', 3, '211300', 3),
('211324', '喀喇沁左翼蒙古族自治县', 3, '211300', 4),
('211381', '北票市', 3, '211300', 5),
('211382', '凌源市', 3, '211300', 6);

-- 葫芦岛市 (211400) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211401', '葫芦岛市区', 3, '211400', 1),
('211421', '绥中县', 3, '211400', 2),
('211422', '建昌县', 3, '211400', 3),
('211481', '兴城市', 3, '211400', 4);

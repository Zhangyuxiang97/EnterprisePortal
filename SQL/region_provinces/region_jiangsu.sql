-- ============================================================
-- 江苏省各区县数据
-- region_level = 3 (区县级)
-- 规则：市区统一标记为"XX市区"，开发功能区归入市区
-- ============================================================

-- 南京市 (320100) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320101', '南京市区', 3, '320100', 1);

-- 无锡市 (320200) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320201', '无锡市区', 3, '320200', 1),
('320281', '江阴市', 3, '320200', 2),
('320282', '宜兴市', 3, '320200', 3);

-- 徐州市 (320300) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320301', '徐州市区', 3, '320300', 1),
('320321', '丰县', 3, '320300', 2),
('320322', '沛县', 3, '320300', 3),
('320324', '睢宁县', 3, '320300', 4),
('320381', '新沂市', 3, '320300', 5),
('320382', '邳州市', 3, '320300', 6);

-- 常州市 (320400) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320401', '常州市区', 3, '320400', 1),
('320481', '溧阳市', 3, '320400', 2);

-- 苏州市 (320500) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320501', '苏州市区', 3, '320500', 1),
('320581', '常熟市', 3, '320500', 2),
('320582', '张家港市', 3, '320500', 3),
('320583', '昆山市', 3, '320500', 4),
('320585', '太仓市', 3, '320500', 5);

-- 南通市 (320600) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320601', '南通市区', 3, '320600', 1),
('320623', '如东县', 3, '320600', 2),
('320681', '启东市', 3, '320600', 3),
('320682', '如皋市', 3, '320600', 4),
('320685', '海门市', 3, '320600', 5);

-- 连云港市 (320700) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320701', '连云港市区', 3, '320700', 1),
('320722', '东海县', 3, '320700', 2),
('320723', '灌云县', 3, '320700', 3),
('320724', '灌南县', 3, '320700', 4);

-- 淮安市 (320800) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320801', '淮安市区', 3, '320800', 1),
('320826', '涟水县', 3, '320800', 2),
('320830', '盱眙县', 3, '320800', 3),
('320831', '金湖县', 3, '320800', 4);

-- 盐城市 (320900) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320901', '盐城市区', 3, '320900', 1),
('320921', '响水县', 3, '320900', 2),
('320922', '滨海县', 3, '320900', 3),
('320923', '阜宁县', 3, '320900', 4),
('320924', '射阳县', 3, '320900', 5),
('320925', '建湖县', 3, '320900', 6),
('320981', '东台市', 3, '320900', 7);

-- 扬州市 (321000) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321001', '扬州市区', 3, '321000', 1),
('321023', '宝应县', 3, '321000', 2),
('321081', '仪征市', 3, '321000', 3),
('321084', '高邮市', 3, '321000', 4);

-- 镇江市 (321100) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321101', '镇江市区', 3, '321100', 1),
('321181', '丹阳市', 3, '321100', 2),
('321182', '扬中市', 3, '321100', 3),
('321183', '句容市', 3, '321100', 4);

-- 泰州市 (321200) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321201', '泰州市区', 3, '321200', 1),
('321281', '兴化市', 3, '321200', 2),
('321282', '靖江市', 3, '321200', 3),
('321283', '泰兴市', 3, '321200', 4);

-- 宿迁市 (321300) - 区县
INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321301', '宿迁市区', 3, '321300', 1),
('321322', '沭阳县', 3, '321300', 2),
('321323', '泗阳县', 3, '321300', 3),
('321324', '泗洪县', 3, '321300', 4);

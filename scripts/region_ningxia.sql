-- 宁夏回族自治区各区县 INSERT 语句
-- region_level: 3 表示区县级

-- 银川市 (640100)
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('640102', '兴庆区', 3, '640100', 1),
('640103', '西夏区', 3, '640100', 2),
('640104', '金凤区', 3, '640100', 3),
('640105', '永宁县', 3, '640100', 4),
('640106', '贺兰县', 3, '640100', 5),
('640181', '灵武市', 3, '640100', 6);

-- 石嘴山市 (640200)
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('640202', '大武口区', 3, '640200', 1),
('640205', '惠农区', 3, '640200', 2),
('640221', '平罗县', 3, '640200', 3);

-- 吴忠市 (640300)
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('640302', '利通区', 3, '640300', 1),
('640303', '红寺堡区', 3, '640300', 2),
('640323', '盐池县', 3, '640300', 3),
('640324', '同心县', 3, '640300', 4),
('640381', '青铜峡市', 3, '640300', 5);

-- 固原市 (640400)
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('640402', '原州区', 3, '640400', 1),
('640422', '西吉县', 3, '640400', 2),
('640423', '隆德县', 3, '640400', 3),
('640424', '泾源县', 3, '640400', 4),
('640425', '彭阳县', 3, '640400', 5);

-- 中卫市 (640500)
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('640502', '沙坡头区', 3, '640500', 1),
('640521', '中宁县', 3, '640500', 2),
('640522', '海原县', 3, '640500', 3);

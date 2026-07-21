-- ============================================================
-- 河北省各区县数据
-- region_level = 3 (区县级)
-- 规则：市区统一标记为"XX市区"，开发功能区归入市区
-- ============================================================

-- 石家庄市 (130100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130101', '石家庄市区', 3, '130100', 1),
('130121', '井陉县', 3, '130100', 2),
('130123', '正定县', 3, '130100', 3),
('130125', '行唐县', 3, '130100', 4),
('130126', '灵寿县', 3, '130100', 5),
('130127', '高邑县', 3, '130100', 6),
('130128', '深泽县', 3, '130100', 7),
('130129', '赞皇县', 3, '130100', 8),
('130130', '无极县', 3, '130100', 9),
('130131', '平山县', 3, '130100', 10),
('130132', '元氏县', 3, '130100', 11),
('130133', '赵县', 3, '130100', 12),
('130181', '辛集市', 3, '130100', 13),
('130183', '晋州市', 3, '130100', 14),
('130184', '新乐市', 3, '130100', 15);

-- 唐山市 (130200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130201', '唐山市区', 3, '130200', 1),
('130224', '滦南县', 3, '130200', 2),
('130225', '乐亭县', 3, '130200', 3),
('130227', '迁西县', 3, '130200', 4),
('130229', '玉田县', 3, '130200', 5),
('130281', '遵化市', 3, '130200', 6),
('130283', '迁安市', 3, '130200', 7),
('130284', '滦州市', 3, '130200', 8);

-- 秦皇岛市 (130300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130301', '秦皇岛市区', 3, '130300', 1),
('130321', '青龙满族自治县', 3, '130300', 2),
('130322', '昌黎县', 3, '130300', 3),
('130324', '卢龙县', 3, '130300', 4);

-- 邯郸市 (130400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130401', '邯郸市区', 3, '130400', 1),
('130423', '临漳县', 3, '130400', 2),
('130424', '成安县', 3, '130400', 3),
('130425', '大名县', 3, '130400', 4),
('130426', '涉县', 3, '130400', 5),
('130427', '磁县', 3, '130400', 6),
('130430', '邱县', 3, '130400', 7),
('130431', '鸡泽县', 3, '130400', 8),
('130432', '广平县', 3, '130400', 9),
('130433', '馆陶县', 3, '130400', 10),
('130434', '魏县', 3, '130400', 11),
('130435', '曲周县', 3, '130400', 12),
('130481', '武安市', 3, '130400', 13);

-- 邢台市 (130500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130501', '邢台市区', 3, '130500', 1),
('130522', '临城县', 3, '130500', 2),
('130523', '内丘县', 3, '130500', 3),
('130524', '柏乡县', 3, '130500', 4),
('130525', '隆尧县', 3, '130500', 5),
('130528', '宁晋县', 3, '130500', 6),
('130529', '巨鹿县', 3, '130500', 7),
('130530', '新河县', 3, '130500', 8),
('130531', '广宗县', 3, '130500', 9),
('130532', '平乡县', 3, '130500', 10),
('130533', '威县', 3, '130500', 11),
('130534', '清河县', 3, '130500', 12),
('130535', '临西县', 3, '130500', 13),
('130581', '南宫市', 3, '130500', 14),
('130582', '沙河市', 3, '130500', 15);

-- 保定市 (130600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130601', '保定市区', 3, '130600', 1),
('130623', '涞水县', 3, '130600', 2),
('130624', '阜平县', 3, '130600', 3),
('130626', '定兴县', 3, '130600', 4),
('130627', '唐县', 3, '130600', 5),
('130628', '高阳县', 3, '130600', 6),
('130629', '容城县', 3, '130600', 7),
('130630', '涞源县', 3, '130600', 8),
('130631', '望都县', 3, '130600', 9),
('130632', '安新县', 3, '130600', 10),
('130633', '易县', 3, '130600', 11),
('130634', '曲阳县', 3, '130600', 12),
('130635', '蠡县', 3, '130600', 13),
('130636', '顺平县', 3, '130600', 14),
('130637', '博野县', 3, '130600', 15),
('130681', '涿州市', 3, '130600', 16),
('130682', '定州市', 3, '130600', 17),
('130683', '安国市', 3, '130600', 18),
('130684', '高碑店市', 3, '130600', 19);

-- 张家口市 (130700) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130701', '张家口市区', 3, '130700', 1),
('130722', '张北县', 3, '130700', 2),
('130723', '康保县', 3, '130700', 3),
('130724', '沽源县', 3, '130700', 4),
('130725', '尚义县', 3, '130700', 5),
('130726', '蔚县', 3, '130700', 6),
('130727', '阳原县', 3, '130700', 7),
('130728', '怀安县', 3, '130700', 8),
('130730', '怀来县', 3, '130700', 9),
('130731', '涿鹿县', 3, '130700', 10),
('130732', '赤城县', 3, '130700', 11);

-- 承德市 (130800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130801', '承德市区', 3, '130800', 1),
('130821', '承德县', 3, '130800', 2),
('130822', '兴隆县', 3, '130800', 3),
('130824', '滦平县', 3, '130800', 4),
('130825', '隆化县', 3, '130800', 5),
('130826', '丰宁满族自治县', 3, '130800', 6),
('130827', '宽城满族自治县', 3, '130800', 7),
('130828', '围场满族蒙古族自治县', 3, '130800', 8),
('130881', '平泉市', 3, '130800', 9);

-- 沧州市 (130900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('130901', '沧州市区', 3, '130900', 1),
('130921', '沧县', 3, '130900', 2),
('130922', '青县', 3, '130900', 3),
('130923', '东光县', 3, '130900', 4),
('130924', '海兴县', 3, '130900', 5),
('130925', '盐山县', 3, '130900', 6),
('130926', '肃宁县', 3, '130900', 7),
('130927', '南皮县', 3, '130900', 8),
('130928', '吴桥县', 3, '130900', 9),
('130929', '献县', 3, '130900', 10),
('130930', '孟村回族自治县', 3, '130900', 11),
('130981', '泊头市', 3, '130900', 12),
('130982', '任丘市', 3, '130900', 13),
('130983', '黄骅市', 3, '130900', 14),
('130984', '河间市', 3, '130900', 15);

-- 廊坊市 (131000) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('131001', '廊坊市区', 3, '131000', 1),
('131022', '固安县', 3, '131000', 2),
('131023', '永清县', 3, '131000', 3),
('131024', '香河县', 3, '131000', 4),
('131025', '大城县', 3, '131000', 5),
('131026', '文安县', 3, '131000', 6),
('131028', '大厂回族自治县', 3, '131000', 7),
('131081', '三河市', 3, '131000', 8),
('131082', '霸州市', 3, '131000', 9);

-- 衡水市 (131100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('131101', '衡水市区', 3, '131100', 1),
('131121', '枣强县', 3, '131100', 2),
('131122', '武邑县', 3, '131100', 3),
('131123', '武强县', 3, '131100', 4),
('131124', '饶阳县', 3, '131100', 5),
('131125', '安平县', 3, '131100', 6),
('131126', '故城县', 3, '131100', 7),
('131127', '景县', 3, '131100', 8),
('131128', '阜城县', 3, '131100', 9);
-- ============================================================
-- 山西省区县级行政区划数据
-- region_level = 3 (区县级)
-- 生成日期: 2026-07-16
-- 说明: 市区统一标记为"XX市区"，区级行政区划合并处理
-- ============================================================

-- 太原市 (140100) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140101', '太原市区', 3, '140100', 1),
('140121', '清徐县', 3, '140100', 2),
('140122', '阳曲县', 3, '140100', 3),
('140123', '娄烦县', 3, '140100', 4),
('140181', '古交市', 3, '140100', 5);

-- 大同市 (140200) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140201', '大同市区', 3, '140200', 1),
('140221', '阳高县', 3, '140200', 2),
('140222', '天镇县', 3, '140200', 3),
('140223', '广灵县', 3, '140200', 4),
('140224', '灵丘县', 3, '140200', 5),
('140225', '浑源县', 3, '140200', 6),
('140226', '左云县', 3, '140200', 7);

-- 阳泉市 (140300) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140301', '阳泉市区', 3, '140300', 1),
('140321', '平定县', 3, '140300', 2),
('140322', '盂县', 3, '140300', 3);

-- 长治市 (140400) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140401', '长治市区', 3, '140400', 1),
('140423', '襄垣县', 3, '140400', 2),
('140425', '平顺县', 3, '140400', 3),
('140426', '黎城县', 3, '140400', 4),
('140427', '壶关县', 3, '140400', 5),
('140428', '长子县', 3, '140400', 6),
('140429', '武乡县', 3, '140400', 7),
('140430', '沁县', 3, '140400', 8),
('140431', '沁源县', 3, '140400', 9);

-- 晋城市 (140500) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140501', '晋城市区', 3, '140500', 1),
('140521', '沁水县', 3, '140500', 2),
('140522', '阳城县', 3, '140500', 3),
('140524', '陵川县', 3, '140500', 4),
('140525', '泽州县', 3, '140500', 5),
('140581', '高平市', 3, '140500', 6);

-- 朔州市 (140600) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140601', '朔州市区', 3, '140600', 1),
('140621', '山阴县', 3, '140600', 2),
('140622', '应县', 3, '140600', 3),
('140623', '右玉县', 3, '140600', 4),
('140681', '怀仁市', 3, '140600', 5);

-- 晋中市 (140700) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140701', '晋中市区', 3, '140700', 1),
('140721', '榆社县', 3, '140700', 2),
('140722', '左权县', 3, '140700', 3),
('140723', '和顺县', 3, '140700', 4),
('140724', '昔阳县', 3, '140700', 5),
('140725', '寿阳县', 3, '140700', 6),
('140726', '祁县', 3, '140700', 7),
('140727', '平遥县', 3, '140700', 8),
('140728', '灵石县', 3, '140700', 9),
('140781', '介休市', 3, '140700', 10);

-- 运城市 (140800) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140801', '运城市区', 3, '140800', 1),
('140821', '临猗县', 3, '140800', 2),
('140822', '万荣县', 3, '140800', 3),
('140823', '闻喜县', 3, '140800', 4),
('140824', '稷山县', 3, '140800', 5),
('140825', '新绛县', 3, '140800', 6),
('140826', '绛县', 3, '140800', 7),
('140827', '垣曲县', 3, '140800', 8),
('140828', '夏县', 3, '140800', 9),
('140829', '平陆县', 3, '140800', 10),
('140830', '芮城县', 3, '140800', 11),
('140881', '永济市', 3, '140800', 12),
('140882', '河津市', 3, '140800', 13);

-- 忻州市 (140900) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('140901', '忻州市区', 3, '140900', 1),
('140921', '定襄县', 3, '140900', 2),
('140922', '五台县', 3, '140900', 3),
('140923', '代县', 3, '140900', 4),
('140924', '繁峙县', 3, '140900', 5),
('140925', '宁武县', 3, '140900', 6),
('140926', '静乐县', 3, '140900', 7),
('140927', '神池县', 3, '140900', 8),
('140928', '五寨县', 3, '140900', 9),
('140929', '岢岚县', 3, '140900', 10),
('140930', '河曲县', 3, '140900', 11),
('140931', '保德县', 3, '140900', 12),
('140932', '偏关县', 3, '140900', 13),
('140981', '原平市', 3, '140900', 14);

-- 临汾市 (141000) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('141001', '临汾市区', 3, '141000', 1),
('141021', '曲沃县', 3, '141000', 2),
('141022', '翼城县', 3, '141000', 3),
('141023', '襄汾县', 3, '141000', 4),
('141024', '洪洞县', 3, '141000', 5),
('141025', '古县', 3, '141000', 6),
('141026', '安泽县', 3, '141000', 7),
('141027', '浮山县', 3, '141000', 8),
('141028', '吉县', 3, '141000', 9),
('141029', '乡宁县', 3, '141000', 10),
('141030', '大宁县', 3, '141000', 11),
('141031', '隰县', 3, '141000', 12),
('141032', '永和县', 3, '141000', 13),
('141033', '蒲县', 3, '141000', 14),
('141034', '汾西县', 3, '141000', 15),
('141081', '侯马市', 3, '141000', 16),
('141082', '霍州市', 3, '141000', 17);

-- 吕梁市 (141100) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('141101', '吕梁市区', 3, '141100', 1),
('141121', '文水县', 3, '141100', 2),
('141122', '交城县', 3, '141100', 3),
('141123', '兴县', 3, '141100', 4),
('141124', '临县', 3, '141100', 5),
('141125', '柳林县', 3, '141100', 6),
('141126', '石楼县', 3, '141100', 7),
('141127', '岚县', 3, '141100', 8),
('141128', '方山县', 3, '141100', 9),
('141129', '中阳县', 3, '141100', 10),
('141130', '交口县', 3, '141100', 11),
('141181', '孝义市', 3, '141100', 12),
('141182', '汾阳市', 3, '141100', 13);
-- ============================================
-- 内蒙古自治区区县数据
-- 数据库名：hailong_consulting
-- 创建时间：2026-07-16
-- 说明：内蒙古自治区所有区县级数据
-- 规则：市区统一标记为"XX市区"，县级市、县、旗单独列出
-- ============================================

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
USE `hailong_consulting`;

-- ============================================
-- 内蒙古自治区区县
-- ============================================
INSERT INTO `region_dictionary` (`region_code`, `region_name`, `region_level`, `parent_code`, `sort_order`) VALUES
-- 呼和浩特市（新城区、回民区、玉泉区、赛罕区 -> 市区）
('150101', '呼和浩特市区', 3, '150100', 1),
('150121', '土默特左旗', 3, '150100', 2),
('150122', '托克托县', 3, '150100', 3),
('150123', '和林格尔县', 3, '150100', 4),
('150124', '清水河县', 3, '150100', 5),
('150125', '武川县', 3, '150100', 6),
-- 包头市（昆都仑区、青山区、东河区、九原区、石拐区、白云鄂博矿区 -> 市区）
('150201', '包头市区', 3, '150200', 1),
('150221', '土默特右旗', 3, '150200', 2),
('150222', '固阳县', 3, '150200', 3),
('150223', '达尔罕茂明安联合旗', 3, '150200', 4),
-- 乌海市（海勃湾区、海南区、乌达区 -> 市区）
('150301', '乌海市区', 3, '150300', 1),
-- 赤峰市（红山区、元宝山区、松山区 -> 市区）
('150401', '赤峰市区', 3, '150400', 1),
('150421', '阿鲁科尔沁旗', 3, '150400', 2),
('150422', '巴林左旗', 3, '150400', 3),
('150423', '巴林右旗', 3, '150400', 4),
('150424', '林西县', 3, '150400', 5),
('150425', '克什克腾旗', 3, '150400', 6),
('150426', '翁牛特旗', 3, '150400', 7),
('150428', '喀喇沁旗', 3, '150400', 8),
('150429', '宁城县', 3, '150400', 9),
('150430', '敖汉旗', 3, '150400', 10),
-- 通辽市（科尔沁区 -> 市区）
('150501', '通辽市区', 3, '150500', 1),
('150521', '科尔沁左翼中旗', 3, '150500', 2),
('150522', '科尔沁左翼后旗', 3, '150500', 3),
('150523', '开鲁县', 3, '150500', 4),
('150524', '库伦旗', 3, '150500', 5),
('150525', '奈曼旗', 3, '150500', 6),
('150526', '扎鲁特旗', 3, '150500', 7),
('150581', '霍林郭勒市', 3, '150500', 8),
-- 鄂尔多斯市（东胜区、康巴什区 -> 市区）
('150601', '鄂尔多斯市区', 3, '150600', 1),
('150621', '达拉特旗', 3, '150600', 2),
('150622', '准格尔旗', 3, '150600', 3),
('150623', '鄂托克前旗', 3, '150600', 4),
('150624', '鄂托克旗', 3, '150600', 5),
('150625', '杭锦旗', 3, '150600', 6),
('150626', '乌审旗', 3, '150600', 7),
('150627', '伊金霍洛旗', 3, '150600', 8),
-- 呼伦贝尔市（海拉尔区 -> 市区）
('150701', '呼伦贝尔市区', 3, '150700', 1),
('150721', '阿荣旗', 3, '150700', 2),
('150722', '莫力达瓦达斡尔族自治旗', 3, '150700', 3),
('150723', '鄂伦春自治旗', 3, '150700', 4),
('150724', '鄂温克族自治旗', 3, '150700', 5),
('150725', '陈巴尔虎旗', 3, '150700', 6),
('150726', '新巴尔虎左旗', 3, '150700', 7),
('150727', '新巴尔虎右旗', 3, '150700', 8),
('150781', '满洲里市', 3, '150700', 9),
('150782', '牙克石市', 3, '150700', 10),
('150783', '扎兰屯市', 3, '150700', 11),
('150784', '额尔古纳市', 3, '150700', 12),
('150785', '根河市', 3, '150700', 13),
-- 巴彦淖尔市（临河区 -> 市区）
('150801', '巴彦淖尔市区', 3, '150800', 1),
('150821', '五原县', 3, '150800', 2),
('150822', '磴口县', 3, '150800', 3),
('150823', '乌拉特前旗', 3, '150800', 4),
('150824', '乌拉特中旗', 3, '150800', 5),
('150825', '乌拉特后旗', 3, '150800', 6),
('150826', '杭锦后旗', 3, '150800', 7),
-- 乌兰察布市（集宁区 -> 市区）
('150901', '乌兰察布市区', 3, '150900', 1),
('150921', '卓资县', 3, '150900', 2),
('150922', '化德县', 3, '150900', 3),
('150923', '商都县', 3, '150900', 4),
('150924', '兴和县', 3, '150900', 5),
('150925', '凉城县', 3, '150900', 6),
('150926', '察哈尔右翼前旗', 3, '150900', 7),
('150927', '察哈尔右翼中旗', 3, '150900', 8),
('150928', '察哈尔右翼后旗', 3, '150900', 9),
('150929', '四子王旗', 3, '150900', 10),
('150981', '丰镇市', 3, '150900', 11),
-- 兴安盟（乌兰浩特市为盟府所在地）
('152201', '乌兰浩特市', 3, '152200', 1),
('152202', '阿尔山市', 3, '152200', 2),
('152221', '科尔沁右翼前旗', 3, '152200', 3),
('152222', '科尔沁右翼中旗', 3, '152200', 4),
('152223', '扎赉特旗', 3, '152200', 5),
('152224', '突泉县', 3, '152200', 6),
-- 锡林郭勒盟（锡林浩特市为盟府所在地）
('152501', '锡林浩特市', 3, '152500', 1),
('152502', '二连浩特市', 3, '152500', 2),
('152522', '阿巴嘎旗', 3, '152500', 3),
('152523', '苏尼特左旗', 3, '152500', 4),
('152524', '苏尼特右旗', 3, '152500', 5),
('152525', '东乌珠穆沁旗', 3, '152500', 6),
('152526', '西乌珠穆沁旗', 3, '152500', 7),
('152527', '太仆寺旗', 3, '152500', 8),
('152528', '镶黄旗', 3, '152500', 9),
('152529', '正镶白旗', 3, '152500', 10),
('152530', '正蓝旗', 3, '152500', 11),
('152531', '多伦县', 3, '152500', 12),
-- 阿拉善盟（阿拉善左旗为盟府所在地）
('152921', '阿拉善左旗', 3, '152900', 1),
('152922', '阿拉善右旗', 3, '152900', 2),
('152923', '额济纳旗', 3, '152900', 3);
-- ============================================================
-- 辽宁省各区县数据
-- region_level = 3 (区县级)
-- 规则：市区统一标记为"XX市区"，开发功能区归入市区
-- ============================================================

-- 沈阳市 (210100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210101', '沈阳市区', 3, '210100', 1),
('210112', '浑南区', 3, '210100', 2),
('210113', '沈北新区', 3, '210100', 3),
('210114', '于洪区', 3, '210100', 4),
('210115', '辽中区', 3, '210100', 5),
('210123', '康平县', 3, '210100', 6),
('210124', '法库县', 3, '210100', 7),
('210181', '新民市', 3, '210100', 8);

-- 大连市 (210200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210201', '大连市区', 3, '210200', 1),
('210212', '旅顺口区', 3, '210200', 2),
('210213', '金州区', 3, '210200', 3),
('210224', '长海县', 3, '210200', 4),
('210281', '瓦房店市', 3, '210200', 5),
('210282', '普兰店区', 3, '210200', 6),
('210283', '庄河市', 3, '210200', 7);

-- 鞍山市 (210300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210301', '鞍山市区', 3, '210300', 1),
('210321', '台安县', 3, '210300', 2),
('210323', '岫岩满族自治县', 3, '210300', 3),
('210381', '海城市', 3, '210300', 4);

-- 抚顺市 (210400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210401', '抚顺市区', 3, '210400', 1),
('210421', '抚顺县', 3, '210400', 2),
('210422', '新宾满族自治县', 3, '210400', 3),
('210423', '清原满族自治县', 3, '210400', 4);

-- 本溪市 (210500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210501', '本溪市区', 3, '210500', 1),
('210521', '本溪满族自治县', 3, '210500', 2),
('210522', '桓仁满族自治县', 3, '210500', 3);

-- 丹东市 (210600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210601', '丹东市区', 3, '210600', 1),
('210624', '宽甸满族自治县', 3, '210600', 2),
('210681', '东港市', 3, '210600', 3),
('210682', '凤城市', 3, '210600', 4);

-- 锦州市 (210700) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210701', '锦州市区', 3, '210700', 1),
('210726', '黑山县', 3, '210700', 2),
('210727', '义县', 3, '210700', 3),
('210781', '凌海市', 3, '210700', 4),
('210782', '北镇市', 3, '210700', 5);

-- 营口市 (210800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210801', '营口市区', 3, '210800', 1),
('210881', '盖州市', 3, '210800', 2),
('210882', '大石桥市', 3, '210800', 3);

-- 阜新市 (210900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('210901', '阜新市区', 3, '210900', 1),
('210921', '阜新蒙古族自治县', 3, '210900', 2),
('210922', '彰武县', 3, '210900', 3);

-- 辽阳市 (211000) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211001', '辽阳市区', 3, '211000', 1),
('211021', '辽阳县', 3, '211000', 2),
('211081', '灯塔市', 3, '211000', 3);

-- 盘锦市 (211100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211101', '盘锦市区', 3, '211100', 1),
('211122', '盘山县', 3, '211100', 2);

-- 铁岭市 (211200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211201', '铁岭市区', 3, '211200', 1),
('211221', '铁岭县', 3, '211200', 2),
('211223', '西丰县', 3, '211200', 3),
('211224', '昌图县', 3, '211200', 4),
('211281', '调兵山市', 3, '211200', 5),
('211282', '开原市', 3, '211200', 6);

-- 朝阳市 (211300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211301', '朝阳市区', 3, '211300', 1),
('211321', '朝阳县', 3, '211300', 2),
('211322', '建平县', 3, '211300', 3),
('211324', '喀喇沁左翼蒙古族自治县', 3, '211300', 4),
('211381', '北票市', 3, '211300', 5),
('211382', '凌源市', 3, '211300', 6);

-- 葫芦岛市 (211400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('211401', '葫芦岛市区', 3, '211400', 1),
('211421', '绥中县', 3, '211400', 2),
('211422', '建昌县', 3, '211400', 3),
('211481', '兴城市', 3, '211400', 4);
-- =============================================
-- 吉林省区县数据
-- region_level: 3 (区县级)
-- 生成日期: 2026-07-16
-- 说明: 市区统一标记为"XX市区"，县级市和县单独列出
-- =============================================

-- 长春市 (220100) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220101', '长春市区', 3, '220100', 1),
('220122', '农安县', 3, '220100', 2),
('220182', '榆树市', 3, '220100', 3),
('220183', '德惠市', 3, '220100', 4),
('220184', '公主岭市', 3, '220100', 5);

-- 吉林市 (220200) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220201', '吉林市区', 3, '220200', 1),
('220221', '永吉县', 3, '220200', 2),
('220281', '蛟河市', 3, '220200', 3),
('220282', '桦甸市', 3, '220200', 4),
('220283', '舒兰市', 3, '220200', 5),
('220284', '磐石市', 3, '220200', 6);

-- 四平市 (220300) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220301', '四平市区', 3, '220300', 1),
('220322', '梨树县', 3, '220300', 2),
('220323', '伊通满族自治县', 3, '220300', 3),
('220382', '双辽市', 3, '220300', 4);

-- 辽源市 (220400) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220401', '辽源市区', 3, '220400', 1),
('220421', '东丰县', 3, '220400', 2),
('220422', '东辽县', 3, '220400', 3);

-- 通化市 (220500) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220501', '通化市区', 3, '220500', 1),
('220521', '通化县', 3, '220500', 2),
('220523', '辉南县', 3, '220500', 3),
('220524', '柳河县', 3, '220500', 4),
('220581', '梅河口市', 3, '220500', 5),
('220582', '集安市', 3, '220500', 6);

-- 白山市 (220600) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220601', '白山市区', 3, '220600', 1),
('220621', '抚松县', 3, '220600', 2),
('220622', '靖宇县', 3, '220600', 3),
('220623', '长白朝鲜族自治县', 3, '220600', 4),
('220681', '临江市', 3, '220600', 5);

-- 松原市 (220700) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220701', '松原市区', 3, '220700', 1),
('220721', '前郭尔罗斯蒙古族自治县', 3, '220700', 2),
('220722', '长岭县', 3, '220700', 3),
('220723', '乾安县', 3, '220700', 4),
('220781', '扶余市', 3, '220700', 5);

-- 白城市 (220800) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('220801', '白城市区', 3, '220800', 1),
('220821', '镇赉县', 3, '220800', 2),
('220822', '通榆县', 3, '220800', 3),
('220881', '洮南市', 3, '220800', 4),
('220882', '大安市', 3, '220800', 5);

-- 延边朝鲜族自治州 (222400) 下辖县市
-- 注: 自治州以州府延吉市作为市区
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('222401', '延边州市区', 3, '222400', 1),
('222402', '图们市', 3, '222400', 2),
('222403', '敦化市', 3, '222400', 3),
('222404', '珲春市', 3, '222400', 4),
('222405', '龙井市', 3, '222400', 5),
('222406', '和龙市', 3, '222400', 6),
('222424', '汪清县', 3, '222400', 7),
('222426', '安图县', 3, '222400', 8);
-- ============================================================
-- 黑龙江省各区县数据
-- region_level = 3 (区县级)
-- 规则：市区统一标记为"XX市区"，开发功能区归入市区
-- ============================================================

-- 哈尔滨市 (230100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230101', '哈尔滨市区', 3, '230100', 1),
('230123', '依兰县', 3, '230100', 2),
('230124', '方正县', 3, '230100', 3),
('230125', '宾县', 3, '230100', 4),
('230126', '巴彦县', 3, '230100', 5),
('230127', '木兰县', 3, '230100', 6),
('230128', '通河县', 3, '230100', 7),
('230129', '延寿县', 3, '230100', 8),
('230183', '尚志市', 3, '230100', 9),
('230184', '五常市', 3, '230100', 10);

-- 齐齐哈尔市 (230200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230201', '齐齐哈尔市区', 3, '230200', 1),
('230221', '龙江县', 3, '230200', 2),
('230223', '依安县', 3, '230200', 3),
('230224', '泰来县', 3, '230200', 4),
('230225', '甘南县', 3, '230200', 5),
('230227', '富裕县', 3, '230200', 6),
('230229', '克山县', 3, '230200', 7),
('230230', '克东县', 3, '230200', 8),
('230231', '拜泉县', 3, '230200', 9),
('230281', '讷河市', 3, '230200', 10);

-- 鸡西市 (230300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230301', '鸡西市区', 3, '230300', 1),
('230321', '鸡东县', 3, '230300', 2),
('230381', '虎林市', 3, '230300', 3),
('230382', '密山市', 3, '230300', 4);

-- 鹤岗市 (230400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230401', '鹤岗市区', 3, '230400', 1),
('230421', '萝北县', 3, '230400', 2),
('230422', '绥滨县', 3, '230400', 3);

-- 双鸭山市 (230500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230501', '双鸭山市区', 3, '230500', 1),
('230521', '集贤县', 3, '230500', 2),
('230522', '友谊县', 3, '230500', 3),
('230523', '宝清县', 3, '230500', 4),
('230524', '饶河县', 3, '230500', 5);

-- 大庆市 (230600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230601', '大庆市区', 3, '230600', 1),
('230621', '肇州县', 3, '230600', 2),
('230622', '肇源县', 3, '230600', 3),
('230623', '林甸县', 3, '230600', 4),
('230624', '杜尔伯特蒙古族自治县', 3, '230600', 5);

-- 伊春市 (230700) - 区县
-- 注：伊春市2019年行政区划调整，原15个市辖区合并为4区4县1市
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230701', '伊春市区', 3, '230700', 1),
('230717', '汤旺县', 3, '230700', 2),
('230718', '丰林县', 3, '230700', 3),
('230719', '南岔县', 3, '230700', 4),
('230722', '嘉荫县', 3, '230700', 5),
('230723', '铁力市', 3, '230700', 6);

-- 佳木斯市 (230800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230801', '佳木斯市区', 3, '230800', 1),
('230822', '桦南县', 3, '230800', 2),
('230826', '桦川县', 3, '230800', 3),
('230828', '汤原县', 3, '230800', 4),
('230881', '同江市', 3, '230800', 5),
('230882', '富锦市', 3, '230800', 6),
('230883', '抚远市', 3, '230800', 7);

-- 七台河市 (230900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('230901', '七台河市区', 3, '230900', 1),
('230921', '勃利县', 3, '230900', 2);

-- 牡丹江市 (231000) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('231001', '牡丹江市区', 3, '231000', 1),
('231025', '林口县', 3, '231000', 2),
('231081', '绥芬河市', 3, '231000', 3),
('231083', '海林市', 3, '231000', 4),
('231084', '宁安市', 3, '231000', 5),
('231085', '穆棱市', 3, '231000', 6),
('231086', '东宁市', 3, '231000', 7);

-- 黑河市 (231100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('231101', '黑河市区', 3, '231100', 1),
('231123', '逊克县', 3, '231100', 2),
('231124', '孙吴县', 3, '231100', 3),
('231181', '北安市', 3, '231100', 4),
('231182', '五大连池市', 3, '231100', 5),
('231183', '嫩江市', 3, '231100', 6);

-- 绥化市 (231200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('231201', '绥化市区', 3, '231200', 1),
('231221', '望奎县', 3, '231200', 2),
('231222', '兰西县', 3, '231200', 3),
('231223', '青冈县', 3, '231200', 4),
('231224', '庆安县', 3, '231200', 5),
('231225', '明水县', 3, '231200', 6),
('231226', '绥棱县', 3, '231200', 7),
('231281', '安达市', 3, '231200', 8),
('231282', '肇东市', 3, '231200', 9),
('231283', '海伦市', 3, '231200', 10);

-- 大兴安岭地区 (232700) - 区县
-- 注：加格达奇区、松岭区、新林区、呼中区为地区管辖的县级行政区
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('232701', '大兴安岭地区市区', 3, '232700', 1),
('232721', '呼玛县', 3, '232700', 2),
('232722', '塔河县', 3, '232700', 3),
('232723', '漠河市', 3, '232700', 4);
-- ============================================================
-- 江苏省各区县数据
-- region_level = 3 (区县级)
-- 规则：市区统一标记为"XX市区"，开发功能区归入市区
-- ============================================================

-- 南京市 (320100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320101', '南京市区', 3, '320100', 1);

-- 无锡市 (320200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320201', '无锡市区', 3, '320200', 1),
('320281', '江阴市', 3, '320200', 2),
('320282', '宜兴市', 3, '320200', 3);

-- 徐州市 (320300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320301', '徐州市区', 3, '320300', 1),
('320321', '丰县', 3, '320300', 2),
('320322', '沛县', 3, '320300', 3),
('320324', '睢宁县', 3, '320300', 4),
('320381', '新沂市', 3, '320300', 5),
('320382', '邳州市', 3, '320300', 6);

-- 常州市 (320400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320401', '常州市区', 3, '320400', 1),
('320481', '溧阳市', 3, '320400', 2);

-- 苏州市 (320500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320501', '苏州市区', 3, '320500', 1),
('320581', '常熟市', 3, '320500', 2),
('320582', '张家港市', 3, '320500', 3),
('320583', '昆山市', 3, '320500', 4),
('320585', '太仓市', 3, '320500', 5);

-- 南通市 (320600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320601', '南通市区', 3, '320600', 1),
('320623', '如东县', 3, '320600', 2),
('320681', '启东市', 3, '320600', 3),
('320682', '如皋市', 3, '320600', 4),
('320685', '海门市', 3, '320600', 5);

-- 连云港市 (320700) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320701', '连云港市区', 3, '320700', 1),
('320722', '东海县', 3, '320700', 2),
('320723', '灌云县', 3, '320700', 3),
('320724', '灌南县', 3, '320700', 4);

-- 淮安市 (320800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320801', '淮安市区', 3, '320800', 1),
('320826', '涟水县', 3, '320800', 2),
('320830', '盱眙县', 3, '320800', 3),
('320831', '金湖县', 3, '320800', 4);

-- 盐城市 (320900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('320901', '盐城市区', 3, '320900', 1),
('320921', '响水县', 3, '320900', 2),
('320922', '滨海县', 3, '320900', 3),
('320923', '阜宁县', 3, '320900', 4),
('320924', '射阳县', 3, '320900', 5),
('320925', '建湖县', 3, '320900', 6),
('320981', '东台市', 3, '320900', 7);

-- 扬州市 (321000) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321001', '扬州市区', 3, '321000', 1),
('321023', '宝应县', 3, '321000', 2),
('321081', '仪征市', 3, '321000', 3),
('321084', '高邮市', 3, '321000', 4);

-- 镇江市 (321100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321101', '镇江市区', 3, '321100', 1),
('321181', '丹阳市', 3, '321100', 2),
('321182', '扬中市', 3, '321100', 3),
('321183', '句容市', 3, '321100', 4);

-- 泰州市 (321200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321201', '泰州市区', 3, '321200', 1),
('321281', '兴化市', 3, '321200', 2),
('321282', '靖江市', 3, '321200', 3),
('321283', '泰兴市', 3, '321200', 4);

-- 宿迁市 (321300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('321301', '宿迁市区', 3, '321300', 1),
('321322', '沭阳县', 3, '321300', 2),
('321323', '泗阳县', 3, '321300', 3),
('321324', '泗洪县', 3, '321300', 4);
-- ============================================================
-- 浙江省所有区县 SQL INSERT 语句
-- 表名: region_dictionary
-- 生成日期: 2026-07-16
-- ============================================================

-- 杭州市 (330100) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330102', '上城区', 3, '330100', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330105', '拱墅区', 3, '330100', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330106', '西湖区', 3, '330100', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330108', '滨江区', 3, '330100', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330109', '萧山区', 3, '330100', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330110', '余杭区', 3, '330100', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330111', '富阳区', 3, '330100', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330112', '临安区', 3, '330100', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330113', '临平区', 3, '330100', 9);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330114', '钱塘区', 3, '330100', 10);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330122', '桐庐县', 3, '330100', 11);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330127', '淳安县', 3, '330100', 12);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330182', '建德市', 3, '330100', 13);

-- 宁波市 (330200) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330203', '海曙区', 3, '330200', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330205', '江北区', 3, '330200', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330206', '北仑区', 3, '330200', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330211', '镇海区', 3, '330200', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330212', '鄞州区', 3, '330200', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330213', '奉化区', 3, '330200', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330225', '象山县', 3, '330200', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330226', '宁海县', 3, '330200', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330281', '余姚市', 3, '330200', 9);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330282', '慈溪市', 3, '330200', 10);

-- 温州市 (330300) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330302', '鹿城区', 3, '330300', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330303', '龙湾区', 3, '330300', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330304', '瓯海区', 3, '330300', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330305', '洞头区', 3, '330300', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330324', '永嘉县', 3, '330300', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330326', '平阳县', 3, '330300', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330327', '苍南县', 3, '330300', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330328', '文成县', 3, '330300', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330329', '泰顺县', 3, '330300', 9);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330381', '瑞安市', 3, '330300', 10);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330382', '乐清市', 3, '330300', 11);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330383', '龙港市', 3, '330300', 12);

-- 嘉兴市 (330400) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330402', '南湖区', 3, '330400', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330411', '秀洲区', 3, '330400', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330421', '嘉善县', 3, '330400', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330424', '海盐县', 3, '330400', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330481', '海宁市', 3, '330400', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330482', '平湖市', 3, '330400', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330483', '桐乡市', 3, '330400', 7);

-- 湖州市 (330500) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330502', '吴兴区', 3, '330500', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330503', '南浔区', 3, '330500', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330521', '德清县', 3, '330500', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330522', '长兴县', 3, '330500', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330523', '安吉县', 3, '330500', 5);

-- 绍兴市 (330600) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330602', '越城区', 3, '330600', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330603', '柯桥区', 3, '330600', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330604', '上虞区', 3, '330600', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330624', '新昌县', 3, '330600', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330681', '诸暨市', 3, '330600', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330683', '嵊州市', 3, '330600', 6);

-- 金华市 (330700) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330702', '婺城区', 3, '330700', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330703', '金东区', 3, '330700', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330723', '武义县', 3, '330700', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330726', '浦江县', 3, '330700', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330727', '磐安县', 3, '330700', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330781', '兰溪市', 3, '330700', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330782', '义乌市', 3, '330700', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330783', '东阳市', 3, '330700', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330784', '永康市', 3, '330700', 9);

-- 衢州市 (330800) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330802', '柯城区', 3, '330800', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330803', '衢江区', 3, '330800', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330822', '常山县', 3, '330800', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330824', '开化县', 3, '330800', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330825', '龙游县', 3, '330800', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330881', '江山市', 3, '330800', 6);

-- 舟山市 (330900) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330902', '定海区', 3, '330900', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330903', '普陀区', 3, '330900', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330921', '岱山县', 3, '330900', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('330922', '嵊泗县', 3, '330900', 4);

-- 台州市 (331000) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331002', '椒江区', 3, '331000', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331003', '黄岩区', 3, '331000', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331004', '路桥区', 3, '331000', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331022', '三门县', 3, '331000', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331023', '天台县', 3, '331000', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331024', '仙居县', 3, '331000', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331081', '温岭市', 3, '331000', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331082', '临海市', 3, '331000', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331083', '玉环市', 3, '331000', 9);

-- 丽水市 (331100) 下辖区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331102', '莲都区', 3, '331100', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331121', '青田县', 3, '331100', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331122', '缙云县', 3, '331100', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331123', '遂昌县', 3, '331100', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331124', '松阳县', 3, '331100', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331125', '云和县', 3, '331100', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331126', '庆元县', 3, '331100', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331127', '景宁畲族自治县', 3, '331100', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('331181', '龙泉市', 3, '331100', 9);
-- =============================================
-- 安徽省区县级行政区划 INSERT 语句
-- region_level: 3 (区县级)
-- 生成日期: 2026-07-16
-- 共 16 个地级市，104 个区县（现行行政区划）
-- =============================================

-- 合肥市 (340100)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340102', '瑶海区', 3, '340100', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340103', '庐阳区', 3, '340100', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340104', '蜀山区', 3, '340100', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340111', '包河区', 3, '340100', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340121', '长丰县', 3, '340100', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340122', '肥东县', 3, '340100', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340123', '肥西县', 3, '340100', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340124', '庐江县', 3, '340100', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340181', '巢湖市', 3, '340100', 9);

-- 芜湖市 (340200)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340202', '镜湖区', 3, '340200', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340203', '弋江区', 3, '340200', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340207', '鸠江区', 3, '340200', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340208', '湾沚区', 3, '340200', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340209', '繁昌区', 3, '340200', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340223', '南陵县', 3, '340200', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340281', '无为市', 3, '340200', 7);

-- 蚌埠市 (340300)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340302', '龙子湖区', 3, '340300', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340303', '蚌山区', 3, '340300', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340304', '禹会区', 3, '340300', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340311', '淮上区', 3, '340300', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340321', '怀远县', 3, '340300', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340322', '五河县', 3, '340300', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340323', '固镇县', 3, '340300', 7);

-- 淮南市 (340400)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340402', '大通区', 3, '340400', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340403', '田家庵区', 3, '340400', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340404', '谢家集区', 3, '340400', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340405', '八公山区', 3, '340400', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340406', '潘集区', 3, '340400', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340421', '凤台县', 3, '340400', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340422', '寿县', 3, '340400', 7);

-- 马鞍山市 (340500)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340503', '花山区', 3, '340500', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340504', '雨山区', 3, '340500', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340506', '博望区', 3, '340500', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340521', '当涂县', 3, '340500', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340522', '含山县', 3, '340500', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340523', '和县', 3, '340500', 6);

-- 淮北市 (340600)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340602', '杜集区', 3, '340600', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340603', '相山区', 3, '340600', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340604', '烈山区', 3, '340600', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340621', '濉溪县', 3, '340600', 4);

-- 铜陵市 (340700)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340705', '铜官区', 3, '340700', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340706', '义安区', 3, '340700', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340711', '郊区', 3, '340700', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340722', '枞阳县', 3, '340700', 4);

-- 安庆市 (340800)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340802', '迎江区', 3, '340800', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340803', '大观区', 3, '340800', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340811', '宜秀区', 3, '340800', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340822', '怀宁县', 3, '340800', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340825', '太湖县', 3, '340800', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340826', '宿松县', 3, '340800', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340827', '望江县', 3, '340800', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340828', '岳西县', 3, '340800', 8);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340881', '桐城市', 3, '340800', 9);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('340882', '潜山市', 3, '340800', 10);

-- 黄山市 (341000)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341002', '屯溪区', 3, '341000', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341003', '黄山区', 3, '341000', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341004', '徽州区', 3, '341000', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341021', '歙县', 3, '341000', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341022', '休宁县', 3, '341000', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341023', '黟县', 3, '341000', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341024', '祁门县', 3, '341000', 7);

-- 滁州市 (341100)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341102', '琅琊区', 3, '341100', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341103', '南谯区', 3, '341100', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341122', '来安县', 3, '341100', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341124', '全椒县', 3, '341100', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341125', '定远县', 3, '341100', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341126', '凤阳县', 3, '341100', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341181', '天长市', 3, '341100', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341182', '明光市', 3, '341100', 8);

-- 阜阳市 (341200)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341202', '颍州区', 3, '341200', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341203', '颍东区', 3, '341200', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341204', '颍泉区', 3, '341200', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341221', '临泉县', 3, '341200', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341222', '太和县', 3, '341200', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341225', '阜南县', 3, '341200', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341226', '颍上县', 3, '341200', 7);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341282', '界首市', 3, '341200', 8);

-- 宿州市 (341300)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341302', '埇桥区', 3, '341300', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341321', '砀山县', 3, '341300', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341322', '萧县', 3, '341300', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341323', '灵璧县', 3, '341300', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341324', '泗县', 3, '341300', 5);

-- 六安市 (341500)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341502', '金安区', 3, '341500', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341503', '裕安区', 3, '341500', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341504', '叶集区', 3, '341500', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341522', '霍邱县', 3, '341500', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341523', '舒城县', 3, '341500', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341524', '金寨县', 3, '341500', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341525', '霍山县', 3, '341500', 7);

-- 亳州市 (341600)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341602', '谯城区', 3, '341600', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341621', '涡阳县', 3, '341600', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341622', '蒙城县', 3, '341600', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341623', '利辛县', 3, '341600', 4);

-- 池州市 (341700)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341702', '贵池区', 3, '341700', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341721', '东至县', 3, '341700', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341722', '石台县', 3, '341700', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341723', '青阳县', 3, '341700', 4);

-- 宣城市 (341800)
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341802', '宣州区', 3, '341800', 1);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341821', '郎溪县', 3, '341800', 2);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341822', '广德市', 3, '341800', 3);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341823', '泾县', 3, '341800', 4);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341824', '绩溪县', 3, '341800', 5);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341825', '旌德县', 3, '341800', 6);
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES ('341881', '宁国市', 3, '341800', 7);
-- ============================================================
-- 湖北省各区县 SQL INSERT 语句
-- 表名: region_dictionary
-- 生成日期: 2026-07-16
-- 规则：市区统一标记为"XX市区"，县单独列出，县级市单独列出
-- ============================================================

-- 武汉市 (420100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420101', '武汉市区', 3, '420100', 1);

-- 黄石市 (420200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420201', '黄石市区', 3, '420200', 1),
('420222', '阳新县', 3, '420200', 2),
('420281', '大冶市', 3, '420200', 3);

-- 十堰市 (420300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420301', '十堰市区', 3, '420300', 1),
('420322', '郧西县', 3, '420300', 2),
('420323', '竹山县', 3, '420300', 3),
('420324', '竹溪县', 3, '420300', 4),
('420325', '房县', 3, '420300', 5),
('420381', '丹江口市', 3, '420300', 6);

-- 宜昌市 (420500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420501', '宜昌市区', 3, '420500', 1),
('420525', '远安县', 3, '420500', 2),
('420526', '兴山县', 3, '420500', 3),
('420527', '秭归县', 3, '420500', 4),
('420528', '长阳土家族自治县', 3, '420500', 5),
('420529', '五峰土家族自治县', 3, '420500', 6),
('420581', '宜都市', 3, '420500', 7),
('420582', '当阳市', 3, '420500', 8),
('420583', '枝江市', 3, '420500', 9);

-- 襄阳市 (420600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420601', '襄阳市区', 3, '420600', 1),
('420624', '南漳县', 3, '420600', 2),
('420625', '谷城县', 3, '420600', 3),
('420626', '保康县', 3, '420600', 4),
('420682', '老河口市', 3, '420600', 5),
('420683', '枣阳市', 3, '420600', 6),
('420684', '宜城市', 3, '420600', 7);

-- 鄂州市 (420700) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420701', '鄂州市区', 3, '420700', 1);

-- 荆门市 (420800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420801', '荆门市区', 3, '420800', 1),
('420822', '沙洋县', 3, '420800', 2),
('420881', '钟祥市', 3, '420800', 3),
('420882', '京山市', 3, '420800', 4);

-- 孝感市 (420900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('420901', '孝感市区', 3, '420900', 1),
('420921', '孝昌县', 3, '420900', 2),
('420922', '大悟县', 3, '420900', 3),
('420923', '云梦县', 3, '420900', 4),
('420981', '应城市', 3, '420900', 5),
('420982', '安陆市', 3, '420900', 6),
('420984', '汉川市', 3, '420900', 7);

-- 荆州市 (421000) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('421001', '荆州市区', 3, '421000', 1),
('421022', '公安县', 3, '421000', 2),
('421024', '江陵县', 3, '421000', 3),
('421081', '石首市', 3, '421000', 4),
('421083', '洪湖市', 3, '421000', 5),
('421087', '松滋市', 3, '421000', 6),
('421088', '监利市', 3, '421000', 7);

-- 黄冈市 (421100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('421101', '黄冈市区', 3, '421100', 1),
('421121', '团风县', 3, '421100', 2),
('421122', '红安县', 3, '421100', 3),
('421123', '罗田县', 3, '421100', 4),
('421124', '英山县', 3, '421100', 5),
('421125', '浠水县', 3, '421100', 6),
('421126', '蕲春县', 3, '421100', 7),
('421127', '黄梅县', 3, '421100', 8),
('421181', '麻城市', 3, '421100', 9),
('421182', '武穴市', 3, '421100', 10);

-- 咸宁市 (421200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('421201', '咸宁市区', 3, '421200', 1),
('421221', '嘉鱼县', 3, '421200', 2),
('421222', '通城县', 3, '421200', 3),
('421223', '崇阳县', 3, '421200', 4),
('421224', '通山县', 3, '421200', 5),
('421281', '赤壁市', 3, '421200', 6);

-- 随州市 (421300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('421301', '随州市区', 3, '421300', 1),
('421321', '随县', 3, '421300', 2),
('421381', '广水市', 3, '421300', 3);

-- 恩施土家族苗族自治州 (422800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('422801', '恩施市', 3, '422800', 1),
('422802', '利川市', 3, '422800', 2),
('422822', '建始县', 3, '422800', 3),
('422823', '巴东县', 3, '422800', 4),
('422825', '宣恩县', 3, '422800', 5),
('422826', '咸丰县', 3, '422800', 6),
('422827', '来凤县', 3, '422800', 7),
('422828', '鹤峰县', 3, '422800', 8);
-- 湖南省各区县数据
-- region_level: 3 表示区县级
-- 市区统一标记为"XX市区"，县和县级市单独列出

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- 长沙市 (430100)
(430101, '长沙市区', 3, 430100, 1),
(430121, '长沙县', 3, 430100, 2),
(430181, '浏阳市', 3, 430100, 3),
(430182, '宁乡市', 3, 430100, 4),

-- 株洲市 (430200)
(430201, '株洲市区', 3, 430200, 1),
(430223, '攸县', 3, 430200, 2),
(430224, '茶陵县', 3, 430200, 3),
(430225, '炎陵县', 3, 430200, 4),
(430281, '醴陵市', 3, 430200, 5),

-- 湘潭市 (430300)
(430301, '湘潭市区', 3, 430300, 1),
(430321, '湘潭县', 3, 430300, 2),
(430381, '湘乡市', 3, 430300, 3),
(430382, '韶山市', 3, 430300, 4),

-- 衡阳市 (430400)
(430401, '衡阳市区', 3, 430400, 1),
(430421, '衡阳县', 3, 430400, 2),
(430422, '衡南县', 3, 430400, 3),
(430423, '衡山县', 3, 430400, 4),
(430424, '衡东县', 3, 430400, 5),
(430426, '祁东县', 3, 430400, 6),
(430481, '耒阳市', 3, 430400, 7),
(430482, '常宁市', 3, 430400, 8),

-- 邵阳市 (430500)
(430501, '邵阳市区', 3, 430500, 1),
(430522, '新邵县', 3, 430500, 2),
(430523, '邵阳县', 3, 430500, 3),
(430524, '隆回县', 3, 430500, 4),
(430525, '洞口县', 3, 430500, 5),
(430527, '绥宁县', 3, 430500, 6),
(430528, '新宁县', 3, 430500, 7),
(430529, '城步苗族自治县', 3, 430500, 8),
(430581, '武冈市', 3, 430500, 9),
(430582, '邵东市', 3, 430500, 10),

-- 岳阳市 (430600)
(430601, '岳阳市区', 3, 430600, 1),
(430621, '岳阳县', 3, 430600, 2),
(430623, '华容县', 3, 430600, 3),
(430624, '湘阴县', 3, 430600, 4),
(430626, '平江县', 3, 430600, 5),
(430681, '汨罗市', 3, 430600, 6),
(430682, '临湘市', 3, 430600, 7),

-- 常德市 (430700)
(430701, '常德市区', 3, 430700, 1),
(430721, '安乡县', 3, 430700, 2),
(430722, '汉寿县', 3, 430700, 3),
(430723, '澧县', 3, 430700, 4),
(430724, '临澧县', 3, 430700, 5),
(430725, '桃源县', 3, 430700, 6),
(430726, '石门县', 3, 430700, 7),
(430781, '津市市', 3, 430700, 8),

-- 张家界市 (430800)
(430801, '张家界市区', 3, 430800, 1),
(430821, '慈利县', 3, 430800, 2),
(430822, '桑植县', 3, 430800, 3),

-- 益阳市 (430900)
(430901, '益阳市区', 3, 430900, 1),
(430921, '南县', 3, 430900, 2),
(430922, '桃江县', 3, 430900, 3),
(430923, '安化县', 3, 430900, 4),
(430981, '沅江市', 3, 430900, 5),

-- 郴州市 (431000)
(431001, '郴州市区', 3, 431000, 1),
(431021, '桂阳县', 3, 431000, 2),
(431022, '宜章县', 3, 431000, 3),
(431023, '永兴县', 3, 431000, 4),
(431024, '嘉禾县', 3, 431000, 5),
(431025, '临武县', 3, 431000, 6),
(431026, '汝城县', 3, 431000, 7),
(431027, '桂东县', 3, 431000, 8),
(431028, '安仁县', 3, 431000, 9),
(431081, '资兴市', 3, 431000, 10),

-- 永州市 (431100)
(431101, '永州市区', 3, 431100, 1),
(431121, '祁阳市', 3, 431100, 2),
(431122, '东安县', 3, 431100, 3),
(431123, '双牌县', 3, 431100, 4),
(431124, '道县', 3, 431100, 5),
(431125, '江永县', 3, 431100, 6),
(431126, '宁远县', 3, 431100, 7),
(431127, '蓝山县', 3, 431100, 8),
(431128, '新田县', 3, 431100, 9),
(431129, '江华瑶族自治县', 3, 431100, 10),

-- 怀化市 (431200)
(431201, '怀化市区', 3, 431200, 1),
(431221, '中方县', 3, 431200, 2),
(431222, '沅陵县', 3, 431200, 3),
(431223, '辰溪县', 3, 431200, 4),
(431224, '溆浦县', 3, 431200, 5),
(431225, '会同县', 3, 431200, 6),
(431226, '麻阳苗族自治县', 3, 431200, 7),
(431227, '新晃侗族自治县', 3, 431200, 8),
(431228, '芷江侗族自治县', 3, 431200, 9),
(431229, '靖州苗族侗族自治县', 3, 431200, 10),
(431230, '通道侗族自治县', 3, 431200, 11),
(431281, '洪江市', 3, 431200, 12),

-- 娄底市 (431300)
(431301, '娄底市区', 3, 431300, 1),
(431321, '双峰县', 3, 431300, 2),
(431322, '新化县', 3, 431300, 3),
(431381, '涟源市', 3, 431300, 4),
(431382, '冷水江市', 3, 431300, 5),

-- 湘西土家族苗族自治州 (433100)
(433101, '吉首市', 3, 433100, 1),
(433122, '泸溪县', 3, 433100, 2),
(433123, '凤凰县', 3, 433100, 3),
(433124, '花垣县', 3, 433100, 4),
(433125, '保靖县', 3, 433100, 5),
(433126, '古丈县', 3, 433100, 6),
(433127, '永顺县', 3, 433100, 7),
(433130, '龙山县', 3, 433100, 8);
-- 广东省区县数据
-- region_level = 3 (区县级)

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- ============ 广州市 (440100) ============
('440101', '广州市市区', 3, '440100', 1),
('440103', '荔湾区', 3, '440100', 2),
('440104', '越秀区', 3, '440100', 3),
('440105', '海珠区', 3, '440100', 4),
('440106', '天河区', 3, '440100', 5),
('440111', '白云区', 3, '440100', 6),
('440112', '黄埔区', 3, '440100', 7),
('440113', '番禺区', 3, '440100', 8),
('440114', '花都区', 3, '440100', 9),
('440115', '南沙区', 3, '440100', 10),
('440117', '从化区', 3, '440100', 11),
('440118', '增城区', 3, '440100', 12),

-- ============ 韶关市 (440200) ============
('440201', '韶关市市区', 3, '440200', 1),
('440203', '武江区', 3, '440200', 2),
('440204', '浈江区', 3, '440200', 3),
('440205', '曲江区', 3, '440200', 4),
('440222', '始兴县', 3, '440200', 5),
('440224', '仁化县', 3, '440200', 6),
('440229', '翁源县', 3, '440200', 7),
('440232', '乳源瑶族自治县', 3, '440200', 8),
('440233', '新丰县', 3, '440200', 9),
('440281', '乐昌市', 3, '440200', 10),
('440282', '南雄市', 3, '440200', 11),

-- ============ 深圳市 (440300) ============
('440301', '深圳市市区', 3, '440300', 1),
('440303', '罗湖区', 3, '440300', 2),
('440304', '福田区', 3, '440300', 3),
('440305', '南山区', 3, '440300', 4),
('440306', '宝安区', 3, '440300', 5),
('440307', '龙岗区', 3, '440300', 6),
('440308', '盐田区', 3, '440300', 7),
('440309', '龙华区', 3, '440300', 8),
('440310', '坪山区', 3, '440300', 9),
('440311', '光明区', 3, '440300', 10),

-- ============ 珠海市 (440400) ============
('440401', '珠海市市区', 3, '440400', 1),
('440402', '香洲区', 3, '440400', 2),
('440403', '斗门区', 3, '440400', 3),
('440404', '金湾区', 3, '440400', 4),

-- ============ 汕头市 (440500) ============
('440501', '汕头市市区', 3, '440500', 1),
('440507', '龙湖区', 3, '440500', 2),
('440511', '金平区', 3, '440500', 3),
('440512', '濠江区', 3, '440500', 4),
('440513', '潮阳区', 3, '440500', 5),
('440514', '潮南区', 3, '440500', 6),
('440515', '澄海区', 3, '440500', 7),
('440523', '南澳县', 3, '440500', 8),

-- ============ 佛山市 (440600) ============
('440601', '佛山市市区', 3, '440600', 1),
('440604', '禅城区', 3, '440600', 2),
('440605', '南海区', 3, '440600', 3),
('440606', '顺德区', 3, '440600', 4),
('440607', '三水区', 3, '440600', 5),
('440608', '高明区', 3, '440600', 6),

-- ============ 江门市 (440700) ============
('440701', '江门市市区', 3, '440700', 1),
('440703', '蓬江区', 3, '440700', 2),
('440704', '江海区', 3, '440700', 3),
('440705', '新会区', 3, '440700', 4),
('440781', '台山市', 3, '440700', 5),
('440783', '开平市', 3, '440700', 6),
('440784', '鹤山市', 3, '440700', 7),
('440785', '恩平市', 3, '440700', 8),

-- ============ 湛江市 (440800) ============
('440801', '湛江市市区', 3, '440800', 1),
('440802', '赤坎区', 3, '440800', 2),
('440803', '霞山区', 3, '440800', 3),
('440804', '坡头区', 3, '440800', 4),
('440811', '麻章区', 3, '440800', 5),
('440823', '遂溪县', 3, '440800', 6),
('440825', '徐闻县', 3, '440800', 7),
('440881', '廉江市', 3, '440800', 8),
('440882', '雷州市', 3, '440800', 9),
('440883', '吴川市', 3, '440800', 10),

-- ============ 茂名市 (440900) ============
('440901', '茂名市市区', 3, '440900', 1),
('440902', '茂南区', 3, '440900', 2),
('440904', '电白区', 3, '440900', 3),
('440981', '高州市', 3, '440900', 4),
('440982', '化州市', 3, '440900', 5),
('440983', '信宜市', 3, '440900', 6),

-- ============ 肇庆市 (441200) ============
('441201', '肇庆市市区', 3, '441200', 1),
('441202', '端州区', 3, '441200', 2),
('441203', '鼎湖区', 3, '441200', 3),
('441204', '高要区', 3, '441200', 4),
('441223', '广宁县', 3, '441200', 5),
('441224', '怀集县', 3, '441200', 6),
('441225', '封开县', 3, '441200', 7),
('441226', '德庆县', 3, '441200', 8),
('441284', '四会市', 3, '441200', 9),

-- ============ 惠州市 (441300) ============
('441301', '惠州市市区', 3, '441300', 1),
('441302', '惠城区', 3, '441300', 2),
('441303', '惠阳区', 3, '441300', 3),
('441322', '博罗县', 3, '441300', 4),
('441323', '惠东县', 3, '441300', 5),
('441324', '龙门县', 3, '441300', 6),

-- ============ 梅州市 (441400) ============
('441401', '梅州市市区', 3, '441400', 1),
('441402', '梅江区', 3, '441400', 2),
('441403', '梅县区', 3, '441400', 3),
('441422', '大埔县', 3, '441400', 4),
('441423', '丰顺县', 3, '441400', 5),
('441424', '五华县', 3, '441400', 6),
('441426', '平远县', 3, '441400', 7),
('441427', '蕉岭县', 3, '441400', 8),
('441481', '兴宁市', 3, '441400', 9),

-- ============ 汕尾市 (441500) ============
('441501', '汕尾市市区', 3, '441500', 1),
('441502', '城区', 3, '441500', 2),
('441521', '海丰县', 3, '441500', 3),
('441523', '陆河县', 3, '441500', 4),
('441581', '陆丰市', 3, '441500', 5),

-- ============ 河源市 (441600) ============
('441601', '河源市市区', 3, '441600', 1),
('441602', '源城区', 3, '441600', 2),
('441621', '紫金县', 3, '441600', 3),
('441622', '龙川县', 3, '441600', 4),
('441623', '连平县', 3, '441600', 5),
('441624', '和平县', 3, '441600', 6),
('441625', '东源县', 3, '441600', 7),

-- ============ 阳江市 (441700) ============
('441701', '阳江市市区', 3, '441700', 1),
('441702', '江城区', 3, '441700', 2),
('441704', '阳东区', 3, '441700', 3),
('441721', '阳西县', 3, '441700', 4),
('441781', '阳春市', 3, '441700', 5),

-- ============ 清远市 (441800) ============
('441801', '清远市市区', 3, '441800', 1),
('441802', '清城区', 3, '441800', 2),
('441803', '清新区', 3, '441800', 3),
('441821', '佛冈县', 3, '441800', 4),
('441823', '阳山县', 3, '441800', 5),
('441825', '连山壮族瑶族自治县', 3, '441800', 6),
('441826', '连南瑶族自治县', 3, '441800', 7),
('441881', '英德市', 3, '441800', 8),
('441882', '连州市', 3, '441800', 9),

-- ============ 东莞市 (441900) ============
('441901', '东莞市市区', 3, '441900', 1),

-- ============ 中山市 (442000) ============
('442001', '中山市市区', 3, '442000', 1),

-- ============ 潮州市 (445100) ============
('445101', '潮州市市区', 3, '445100', 1),
('445102', '湘桥区', 3, '445100', 2),
('445103', '潮安区', 3, '445100', 3),
('445122', '饶平县', 3, '445100', 4),

-- ============ 揭阳市 (445200) ============
('445201', '揭阳市市区', 3, '445200', 1),
('445202', '榕城区', 3, '445200', 2),
('445203', '揭东区', 3, '445200', 3),
('445222', '揭西县', 3, '445200', 4),
('445224', '惠来县', 3, '445200', 5),
('445281', '普宁市', 3, '445200', 6),

-- ============ 云浮市 (445300) ============
('445301', '云浮市市区', 3, '445300', 1),
('445302', '云城区', 3, '445300', 2),
('445303', '云安区', 3, '445300', 3),
('445321', '新兴县', 3, '445300', 4),
('445322', '郁南县', 3, '445300', 5),
('445381', '罗定市', 3, '445300', 6);
-- 四川省和重庆市行政区划 SQL
-- 包含所有地级市/区县的 INSERT 语句
-- region_level: 1=省/直辖市, 2=地级市/区, 3=区/县/县级市

-- ============================================================
-- 省级行政区
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510000', '四川省', 1, '0', 1),
('500000', '重庆市', 1, '0', 2);

-- ============================================================
-- 四川省 - 地级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510100', '成都市', 2, '510000', 1),
('510300', '自贡市', 2, '510000', 2),
('510400', '攀枝花市', 2, '510000', 3),
('510500', '泸州市', 2, '510000', 4),
('510600', '德阳市', 2, '510000', 5),
('510700', '绵阳市', 2, '510000', 6),
('510800', '广元市', 2, '510000', 7),
('510900', '遂宁市', 2, '510000', 8),
('511000', '内江市', 2, '510000', 9),
('511100', '乐山市', 2, '510000', 10),
('511300', '南充市', 2, '510000', 11),
('511400', '眉山市', 2, '510000', 12),
('511500', '宜宾市', 2, '510000', 13),
('511600', '广安市', 2, '510000', 14),
('511700', '达州市', 2, '510000', 15),
('511800', '雅安市', 2, '510000', 16),
('511900', '巴中市', 2, '510000', 17),
('512000', '资阳市', 2, '510000', 18),
('513200', '阿坝藏族羌族自治州', 2, '510000', 19),
('513300', '甘孜藏族自治州', 2, '510000', 20),
('513400', '凉山彝族自治州', 2, '510000', 21);

-- ============================================================
-- 510100 成都市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510104', '锦江区', 3, '510100', 1),
('510105', '青羊区', 3, '510100', 2),
('510106', '金牛区', 3, '510100', 3),
('510107', '武侯区', 3, '510100', 4),
('510108', '成华区', 3, '510100', 5),
('510112', '龙泉驿区', 3, '510100', 6),
('510113', '青白江区', 3, '510100', 7),
('510114', '新都区', 3, '510100', 8),
('510115', '温江区', 3, '510100', 9),
('510116', '双流区', 3, '510100', 10),
('510117', '郫都区', 3, '510100', 11),
('510118', '新津区', 3, '510100', 12),
('510121', '金堂县', 3, '510100', 13),
('510129', '大邑县', 3, '510100', 14),
('510131', '蒲江县', 3, '510100', 15),
('510181', '都江堰市', 3, '510100', 16),
('510182', '彭州市', 3, '510100', 17),
('510183', '邛崃市', 3, '510100', 18),
('510184', '崇州市', 3, '510100', 19),
('510185', '简阳市', 3, '510100', 20);

-- ============================================================
-- 510300 自贡市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510302', '自流井区', 3, '510300', 1),
('510303', '贡井区', 3, '510300', 2),
('510304', '大安区', 3, '510300', 3),
('510311', '沿滩区', 3, '510300', 4),
('510321', '荣县', 3, '510300', 5),
('510322', '富顺县', 3, '510300', 6);

-- ============================================================
-- 510400 攀枝花市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510402', '东区', 3, '510400', 1),
('510403', '西区', 3, '510400', 2),
('510411', '仁和区', 3, '510400', 3),
('510421', '米易县', 3, '510400', 4),
('510422', '盐边县', 3, '510400', 5);

-- ============================================================
-- 510500 泸州市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510502', '江阳区', 3, '510500', 1),
('510503', '纳溪区', 3, '510500', 2),
('510504', '龙马潭区', 3, '510500', 3),
('510521', '泸县', 3, '510500', 4),
('510522', '合江县', 3, '510500', 5),
('510524', '叙永县', 3, '510500', 6),
('510525', '古蔺县', 3, '510500', 7);

-- ============================================================
-- 510600 德阳市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510603', '旌阳区', 3, '510600', 1),
('510604', '罗江区', 3, '510600', 2),
('510623', '中江县', 3, '510600', 3),
('510681', '广汉市', 3, '510600', 4),
('510682', '什邡市', 3, '510600', 5),
('510683', '绵竹市', 3, '510600', 6);

-- ============================================================
-- 510700 绵阳市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510703', '涪城区', 3, '510700', 1),
('510704', '游仙区', 3, '510700', 2),
('510705', '安州区', 3, '510700', 3),
('510722', '三台县', 3, '510700', 4),
('510723', '盐亭县', 3, '510700', 5),
('510725', '梓潼县', 3, '510700', 6),
('510726', '北川羌族自治县', 3, '510700', 7),
('510727', '平武县', 3, '510700', 8),
('510781', '江油市', 3, '510700', 9);

-- ============================================================
-- 510800 广元市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510802', '利州区', 3, '510800', 1),
('510811', '昭化区', 3, '510800', 2),
('510812', '朝天区', 3, '510800', 3),
('510821', '旺苍县', 3, '510800', 4),
('510822', '青川县', 3, '510800', 5),
('510823', '剑阁县', 3, '510800', 6),
('510824', '苍溪县', 3, '510800', 7);

-- ============================================================
-- 510900 遂宁市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('510903', '船山区', 3, '510900', 1),
('510904', '安居区', 3, '510900', 2),
('510921', '蓬溪县', 3, '510900', 3),
('510923', '大英县', 3, '510900', 4),
('510981', '射洪市', 3, '510900', 5);

-- ============================================================
-- 511000 内江市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511002', '市中区', 3, '511000', 1),
('511011', '东兴区', 3, '511000', 2),
('511024', '威远县', 3, '511000', 3),
('511025', '资中县', 3, '511000', 4),
('511083', '隆昌市', 3, '511000', 5);

-- ============================================================
-- 511100 乐山市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511102', '市中区', 3, '511100', 1),
('511111', '沙湾区', 3, '511100', 2),
('511112', '五通桥区', 3, '511100', 3),
('511113', '金口河区', 3, '511100', 4),
('511123', '犍为县', 3, '511100', 5),
('511124', '井研县', 3, '511100', 6),
('511126', '夹江县', 3, '511100', 7),
('511129', '沐川县', 3, '511100', 8),
('511132', '峨边彝族自治县', 3, '511100', 9),
('511133', '马边彝族自治县', 3, '511100', 10),
('511181', '峨眉山市', 3, '511100', 11);

-- ============================================================
-- 511300 南充市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511302', '顺庆区', 3, '511300', 1),
('511303', '高坪区', 3, '511300', 2),
('511304', '嘉陵区', 3, '511300', 3),
('511321', '南部县', 3, '511300', 4),
('511322', '营山县', 3, '511300', 5),
('511323', '蓬安县', 3, '511300', 6),
('511324', '仪陇县', 3, '511300', 7),
('511325', '西充县', 3, '511300', 8),
('511381', '阆中市', 3, '511300', 9);

-- ============================================================
-- 511400 眉山市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511402', '东坡区', 3, '511400', 1),
('511403', '彭山区', 3, '511400', 2),
('511421', '仁寿县', 3, '511400', 3),
('511423', '洪雅县', 3, '511400', 4),
('511424', '丹棱县', 3, '511400', 5),
('511425', '青神县', 3, '511400', 6);

-- ============================================================
-- 511500 宜宾市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511502', '翠屏区', 3, '511500', 1),
('511503', '南溪区', 3, '511500', 2),
('511504', '叙州区', 3, '511500', 3),
('511523', '江安县', 3, '511500', 4),
('511524', '长宁县', 3, '511500', 5),
('511525', '高县', 3, '511500', 6),
('511526', '珙县', 3, '511500', 7),
('511527', '筠连县', 3, '511500', 8),
('511528', '兴文县', 3, '511500', 9),
('511529', '屏山县', 3, '511500', 10);

-- ============================================================
-- 511600 广安市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511602', '广安区', 3, '511600', 1),
('511603', '前锋区', 3, '511600', 2),
('511621', '岳池县', 3, '511600', 3),
('511622', '武胜县', 3, '511600', 4),
('511623', '邻水县', 3, '511600', 5),
('511681', '华蓥市', 3, '511600', 6);

-- ============================================================
-- 511700 达州市 - 区/县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511702', '通川区', 3, '511700', 1),
('511703', '达川区', 3, '511700', 2),
('511722', '宣汉县', 3, '511700', 3),
('511723', '开江县', 3, '511700', 4),
('511724', '大竹县', 3, '511700', 5),
('511725', '渠县', 3, '511700', 6),
('511781', '万源市', 3, '511700', 7);

-- ============================================================
-- 511800 雅安市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511802', '雨城区', 3, '511800', 1),
('511803', '名山区', 3, '511800', 2),
('511822', '荥经县', 3, '511800', 3),
('511823', '汉源县', 3, '511800', 4),
('511824', '石棉县', 3, '511800', 5),
('511825', '天全县', 3, '511800', 6),
('511826', '芦山县', 3, '511800', 7),
('511827', '宝兴县', 3, '511800', 8);

-- ============================================================
-- 511900 巴中市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('511902', '巴州区', 3, '511900', 1),
('511903', '恩阳区', 3, '511900', 2),
('511921', '通江县', 3, '511900', 3),
('511922', '南江县', 3, '511900', 4),
('511923', '平昌县', 3, '511900', 5);

-- ============================================================
-- 512000 资阳市 - 区/县
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('512002', '雁江区', 3, '512000', 1),
('512021', '安岳县', 3, '512000', 2),
('512022', '乐至县', 3, '512000', 3);

-- ============================================================
-- 513200 阿坝藏族羌族自治州 - 县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('513201', '马尔康市', 3, '513200', 1),
('513221', '汶川县', 3, '513200', 2),
('513222', '理县', 3, '513200', 3),
('513223', '茂县', 3, '513200', 4),
('513224', '松潘县', 3, '513200', 5),
('513225', '九寨沟县', 3, '513200', 6),
('513226', '金川县', 3, '513200', 7),
('513227', '小金县', 3, '513200', 8),
('513228', '黑水县', 3, '513200', 9),
('513230', '壤塘县', 3, '513200', 10),
('513231', '阿坝县', 3, '513200', 11),
('513232', '若尔盖县', 3, '513200', 12),
('513233', '红原县', 3, '513200', 13);

-- ============================================================
-- 513300 甘孜藏族自治州 - 县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('513301', '康定市', 3, '513300', 1),
('513322', '泸定县', 3, '513300', 2),
('513323', '丹巴县', 3, '513300', 3),
('513324', '九龙县', 3, '513300', 4),
('513325', '雅江县', 3, '513300', 5),
('513326', '道孚县', 3, '513300', 6),
('513327', '炉霍县', 3, '513300', 7),
('513328', '甘孜县', 3, '513300', 8),
('513329', '新龙县', 3, '513300', 9),
('513330', '德格县', 3, '513300', 10),
('513331', '白玉县', 3, '513300', 11),
('513332', '石渠县', 3, '513300', 12),
('513333', '色达县', 3, '513300', 13),
('513334', '理塘县', 3, '513300', 14),
('513335', '巴塘县', 3, '513300', 15),
('513336', '乡城县', 3, '513300', 16),
('513337', '稻城县', 3, '513300', 17),
('513338', '得荣县', 3, '513300', 18);

-- ============================================================
-- 513400 凉山彝族自治州 - 县/县级市
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('513401', '西昌市', 3, '513400', 1),
('513402', '会理市', 3, '513400', 2),
('513422', '木里藏族自治县', 3, '513400', 3),
('513423', '盐源县', 3, '513400', 4),
('513424', '德昌县', 3, '513400', 5),
('513425', '会东县', 3, '513400', 6),
('513426', '宁南县', 3, '513400', 7),
('513427', '普格县', 3, '513400', 8),
('513428', '布拖县', 3, '513400', 9),
('513429', '金阳县', 3, '513400', 10),
('513430', '昭觉县', 3, '513400', 11),
('513431', '喜德县', 3, '513400', 12),
('513432', '冕宁县', 3, '513400', 13),
('513433', '越西县', 3, '513400', 14),
('513434', '甘洛县', 3, '513400', 15),
('513435', '美姑县', 3, '513400', 16),
('513436', '雷波县', 3, '513400', 17);

-- ============================================================
-- 重庆市 - 区/县（直辖市下直接为区县级）
-- ============================================================
INSERT INTO region (region_code, region_name, region_level, parent_code, sort_order) VALUES
('500101', '万州区', 3, '500000', 1),
('500102', '涪陵区', 3, '500000', 2),
('500103', '渝中区', 3, '500000', 3),
('500104', '大渡口区', 3, '500000', 4),
('500105', '江北区', 3, '500000', 5),
('500106', '沙坪坝区', 3, '500000', 6),
('500107', '九龙坡区', 3, '500000', 7),
('500108', '南岸区', 3, '500000', 8),
('500109', '北碚区', 3, '500000', 9),
('500110', '綦江区', 3, '500000', 10),
('500111', '大足区', 3, '500000', 11),
('500112', '渝北区', 3, '500000', 12),
('500113', '巴南区', 3, '500000', 13),
('500114', '黔江区', 3, '500000', 14),
('500115', '长寿区', 3, '500000', 15),
('500116', '江津区', 3, '500000', 16),
('500117', '合川区', 3, '500000', 17),
('500118', '永川区', 3, '500000', 18),
('500119', '南川区', 3, '500000', 19),
('500120', '璧山区', 3, '500000', 20),
('500151', '铜梁区', 3, '500000', 21),
('500152', '潼南区', 3, '500000', 22),
('500153', '荣昌区', 3, '500000', 23),
('500154', '开州区', 3, '500000', 24),
('500155', '梁平区', 3, '500000', 25),
('500156', '武隆区', 3, '500000', 26),
('500229', '城口县', 3, '500000', 27),
('500230', '丰都县', 3, '500000', 28),
('500231', '垫江县', 3, '500000', 29),
('500233', '忠县', 3, '500000', 30),
('500235', '云阳县', 3, '500000', 31),
('500236', '奉节县', 3, '500000', 32),
('500237', '巫山县', 3, '500000', 33),
('500238', '巫溪县', 3, '500000', 34),
('500240', '石柱土家族自治县', 3, '500000', 35),
('500241', '秀山土家族苗族自治县', 3, '500000', 36),
('500242', '酉阳土家族苗族自治县', 3, '500000', 37),
('500243', '彭水苗族土家族自治县', 3, '500000', 38);
-- ============================================================
-- 贵州省各区县 SQL INSERT 语句
-- 表名: region_dictionary
-- 生成日期: 2026-07-16
-- 规则：市区统一标记为"XX市区"，县单独列出，县级市单独列出
-- ============================================================

-- 贵阳市 (520100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('520101', '贵阳市区', 3, '520100', 1),
('520121', '开阳县', 3, '520100', 2),
('520122', '息烽县', 3, '520100', 3),
('520123', '修文县', 3, '520100', 4),
('520181', '清镇市', 3, '520100', 5);

-- 六盘水市 (520200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('520201', '六盘水市区', 3, '520200', 1),
('520281', '盘州市', 3, '520200', 2);

-- 遵义市 (520300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('520301', '遵义市区', 3, '520300', 1),
('520322', '桐梓县', 3, '520300', 2),
('520323', '绥阳县', 3, '520300', 3),
('520324', '正安县', 3, '520300', 4),
('520325', '道真仡佬族苗族自治县', 3, '520300', 5),
('520326', '务川仡佬族苗族自治县', 3, '520300', 6),
('520327', '凤冈县', 3, '520300', 7),
('520328', '湄潭县', 3, '520300', 8),
('520329', '余庆县', 3, '520300', 9),
('520330', '习水县', 3, '520300', 10),
('520381', '赤水市', 3, '520300', 11),
('520382', '仁怀市', 3, '520300', 12);

-- 安顺市 (520400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('520401', '安顺市区', 3, '520400', 1),
('520422', '普定县', 3, '520400', 2),
('520423', '镇宁布依族苗族自治县', 3, '520400', 3),
('520424', '关岭布依族苗族自治县', 3, '520400', 4),
('520425', '紫云苗族布依族自治县', 3, '520400', 5);

-- 毕节市 (520500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('520501', '毕节市区', 3, '520500', 1),
('520521', '大方县', 3, '520500', 2),
('520522', '黔西市', 3, '520500', 3),
('520523', '金沙县', 3, '520500', 4),
('520524', '织金县', 3, '520500', 5),
('520525', '纳雍县', 3, '520500', 6),
('520526', '威宁彝族回族苗族自治县', 3, '520500', 7),
('520527', '赫章县', 3, '520500', 8);

-- 铜仁市 (520600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('520601', '铜仁市区', 3, '520600', 1),
('520621', '江口县', 3, '520600', 2),
('520622', '玉屏侗族自治县', 3, '520600', 3),
('520623', '石阡县', 3, '520600', 4),
('520624', '思南县', 3, '520600', 5),
('520625', '印江土家族苗族自治县', 3, '520600', 6),
('520626', '德江县', 3, '520600', 7),
('520627', '沿河土家族自治县', 3, '520600', 8),
('520628', '松桃苗族自治县', 3, '520600', 9);

-- 黔西南布依族苗族自治州 (522300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('522301', '兴义市', 3, '522300', 1),
('522322', '兴仁市', 3, '522300', 2),
('522323', '普安县', 3, '522300', 3),
('522324', '晴隆县', 3, '522300', 4),
('522325', '贞丰县', 3, '522300', 5),
('522326', '望谟县', 3, '522300', 6),
('522327', '册亨县', 3, '522300', 7),
('522328', '安龙县', 3, '522300', 8);

-- 黔东南苗族侗族自治州 (522600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('522601', '凯里市', 3, '522600', 1),
('522622', '黄平县', 3, '522600', 2),
('522623', '施秉县', 3, '522600', 3),
('522624', '三穗县', 3, '522600', 4),
('522625', '镇远县', 3, '522600', 5),
('522626', '岑巩县', 3, '522600', 6),
('522627', '天柱县', 3, '522600', 7),
('522628', '锦屏县', 3, '522600', 8),
('522629', '剑河县', 3, '522600', 9),
('522630', '台江县', 3, '522600', 10),
('522631', '黎平县', 3, '522600', 11),
('522632', '榕江县', 3, '522600', 12),
('522633', '从江县', 3, '522600', 13),
('522634', '雷山县', 3, '522600', 14),
('522635', '麻江县', 3, '522600', 15),
('522636', '丹寨县', 3, '522600', 16);

-- 黔南布依族苗族自治州 (522700) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('522701', '都匀市', 3, '522700', 1),
('522702', '福泉市', 3, '522700', 2),
('522722', '荔波县', 3, '522700', 3),
('522723', '贵定县', 3, '522700', 4),
('522724', '瓮安县', 3, '522700', 5),
('522725', '独山县', 3, '522700', 6),
('522726', '平塘县', 3, '522700', 7),
('522727', '罗甸县', 3, '522700', 8),
('522728', '长顺县', 3, '522700', 9),
('522729', '龙里县', 3, '522700', 10),
('522730', '惠水县', 3, '522700', 11),
('522731', '三都水族自治县', 3, '522700', 12);
-- ============================================================
-- 云南省各区县 SQL INSERT 语句
-- 表名: region_dictionary
-- 生成日期: 2026-07-16
-- 规则：市区统一标记为"XX市区"，县单独列出，县级市单独列出
-- ============================================================

-- 昆明市 (530100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530101', '昆明市区', 3, '530100', 1),
('530113', '东川区', 3, '530100', 2),
('530124', '富民县', 3, '530100', 3),
('530125', '宜良县', 3, '530100', 4),
('530126', '石林彝族自治县', 3, '530100', 5),
('530127', '嵩明县', 3, '530100', 6),
('530128', '禄劝彝族苗族自治县', 3, '530100', 7),
('530129', '寻甸回族彝族自治县', 3, '530100', 8),
('530181', '安宁市', 3, '530100', 9);

-- 曲靖市 (530300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530301', '曲靖市区', 3, '530300', 1),
('530322', '陆良县', 3, '530300', 2),
('530323', '师宗县', 3, '530300', 3),
('530324', '罗平县', 3, '530300', 4),
('530325', '富源县', 3, '530300', 5),
('530326', '会泽县', 3, '530300', 6),
('530381', '宣威市', 3, '530300', 7);

-- 玉溪市 (530400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530401', '玉溪市区', 3, '530400', 1),
('530422', '通海县', 3, '530400', 2),
('530423', '华宁县', 3, '530400', 3),
('530424', '易门县', 3, '530400', 4),
('530425', '峨山彝族自治县', 3, '530400', 5),
('530426', '新平彝族傣族自治县', 3, '530400', 6),
('530427', '元江哈尼族彝族傣族自治县', 3, '530400', 7),
('530481', '澄江市', 3, '530400', 8);

-- 保山市 (530500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530501', '保山市区', 3, '530500', 1),
('530522', '施甸县', 3, '530500', 2),
('530523', '龙陵县', 3, '530500', 3),
('530524', '昌宁县', 3, '530500', 4),
('530581', '腾冲市', 3, '530500', 5);

-- 昭通市 (530600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530601', '昭通市区', 3, '530600', 1),
('530622', '鲁甸县', 3, '530600', 2),
('530623', '巧家县', 3, '530600', 3),
('530624', '盐津县', 3, '530600', 4),
('530625', '大关县', 3, '530600', 5),
('530626', '永善县', 3, '530600', 6),
('530627', '绥江县', 3, '530600', 7),
('530628', '镇雄县', 3, '530600', 8),
('530629', '彝良县', 3, '530600', 9),
('530630', '威信县', 3, '530600', 10),
('530681', '水富市', 3, '530600', 11);

-- 丽江市 (530700) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530701', '丽江市区', 3, '530700', 1),
('530721', '玉龙纳西族自治县', 3, '530700', 2),
('530722', '永胜县', 3, '530700', 3),
('530723', '华坪县', 3, '530700', 4),
('530724', '宁蒗彝族自治县', 3, '530700', 5);

-- 普洱市 (530800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530801', '普洱市区', 3, '530800', 1),
('530822', '宁洱哈尼族彝族自治县', 3, '530800', 2),
('530823', '景东彝族自治县', 3, '530800', 3),
('530824', '景谷傣族彝族自治县', 3, '530800', 4),
('530825', '镇沅彝族哈尼族拉祜族自治县', 3, '530800', 5),
('530826', '江城哈尼族彝族自治县', 3, '530800', 6),
('530827', '孟连傣族拉祜族佤族自治县', 3, '530800', 7),
('530828', '澜沧拉祜族自治县', 3, '530800', 8),
('530829', '西盟佤族自治县', 3, '530800', 9);

-- 临沧市 (530900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('530901', '临沧市区', 3, '530900', 1),
('530922', '凤庆县', 3, '530900', 2),
('530923', '云县', 3, '530900', 3),
('530924', '永德县', 3, '530900', 4),
('530925', '镇康县', 3, '530900', 5),
('530926', '双江拉祜族佤族布朗族傣族自治县', 3, '530900', 6),
('530927', '耿马傣族佤族自治县', 3, '530900', 7),
('530928', '沧源佤族自治县', 3, '530900', 8);

-- 楚雄彝族自治州 (532300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('532301', '楚雄市', 3, '532300', 1),
('532322', '双柏县', 3, '532300', 2),
('532323', '牟定县', 3, '532300', 3),
('532324', '南华县', 3, '532300', 4),
('532325', '姚安县', 3, '532300', 5),
('532326', '大姚县', 3, '532300', 6),
('532327', '永仁县', 3, '532300', 7),
('532328', '元谋县', 3, '532300', 8),
('532329', '武定县', 3, '532300', 9),
('532331', '禄丰市', 3, '532300', 10);

-- 红河哈尼族彝族自治州 (532500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('532501', '个旧市', 3, '532500', 1),
('532502', '开远市', 3, '532500', 2),
('532503', '蒙自市', 3, '532500', 3),
('532504', '弥勒市', 3, '532500', 4),
('532523', '屏边苗族自治县', 3, '532500', 5),
('532524', '建水县', 3, '532500', 6),
('532525', '石屏县', 3, '532500', 7),
('532527', '泸西县', 3, '532500', 8),
('532528', '元阳县', 3, '532500', 9),
('532529', '红河县', 3, '532500', 10),
('532530', '金平苗族瑶族傣族自治县', 3, '532500', 11),
('532531', '绿春县', 3, '532500', 12),
('532532', '河口瑶族自治县', 3, '532500', 13);

-- 文山壮族苗族自治州 (532600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('532601', '文山市', 3, '532600', 1),
('532622', '砚山县', 3, '532600', 2),
('532623', '西畴县', 3, '532600', 3),
('532624', '麻栗坡县', 3, '532600', 4),
('532625', '马关县', 3, '532600', 5),
('532626', '丘北县', 3, '532600', 6),
('532627', '广南县', 3, '532600', 7),
('532628', '富宁县', 3, '532600', 8);

-- 西双版纳傣族自治州 (532800) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('532801', '景洪市', 3, '532800', 1),
('532822', '勐海县', 3, '532800', 2),
('532823', '勐腊县', 3, '532800', 3);

-- 大理白族自治州 (532900) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('532901', '大理市', 3, '532900', 1),
('532922', '漾濞彝族自治县', 3, '532900', 2),
('532923', '祥云县', 3, '532900', 3),
('532924', '宾川县', 3, '532900', 4),
('532925', '弥渡县', 3, '532900', 5),
('532926', '南涧彝族自治县', 3, '532900', 6),
('532927', '巍山彝族回族自治县', 3, '532900', 7),
('532928', '永平县', 3, '532900', 8),
('532929', '云龙县', 3, '532900', 9),
('532930', '洱源县', 3, '532900', 10),
('532931', '剑川县', 3, '532900', 11),
('532932', '鹤庆县', 3, '532900', 12);

-- 德宏傣族景颇族自治州 (533100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('533101', '芒市', 3, '533100', 1),
('533102', '瑞丽市', 3, '533100', 2),
('533122', '梁河县', 3, '533100', 3),
('533123', '盈江县', 3, '533100', 4),
('533124', '陇川县', 3, '533100', 5);

-- 怒江傈僳族自治州 (533300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('533301', '泸水市', 3, '533300', 1),
('533323', '福贡县', 3, '533300', 2),
('533324', '贡山独龙族怒族自治县', 3, '533300', 3),
('533325', '兰坪白族普米族自治县', 3, '533300', 4);

-- 迪庆藏族自治州 (533400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('533401', '香格里拉市', 3, '533400', 1),
('533422', '德钦县', 3, '533400', 2),
('533423', '维西傈僳族自治县', 3, '533400', 3);
-- ============================================================
-- 西藏自治区各区县 SQL INSERT 语句
-- 表名: region_dictionary
-- 生成日期: 2026-07-16
-- 规则：市区统一标记为"XX市区"，县单独列出
-- ============================================================

-- 拉萨市 (540100) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('540101', '拉萨市区', 3, '540100', 1),
('540121', '林周县', 3, '540100', 2),
('540122', '当雄县', 3, '540100', 3),
('540123', '尼木县', 3, '540100', 4),
('540124', '曲水县', 3, '540100', 5),
('540126', '堆龙德庆区', 3, '540100', 6),
('540127', '达孜区', 3, '540100', 7),
('540128', '墨竹工卡县', 3, '540100', 8);

-- 日喀则市 (540200) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('540201', '日喀则市区', 3, '540200', 1),
('540222', '南木林县', 3, '540200', 2),
('540223', '江孜县', 3, '540200', 3),
('540224', '定日县', 3, '540200', 4),
('540225', '萨迦县', 3, '540200', 5),
('540226', '拉孜县', 3, '540200', 6),
('540227', '昂仁县', 3, '540200', 7),
('540228', '谢通门县', 3, '540200', 8),
('540229', '白朗县', 3, '540200', 9),
('540230', '仁布县', 3, '540200', 10),
('540231', '康马县', 3, '540200', 11),
('540232', '定结县', 3, '540200', 12),
('540233', '仲巴县', 3, '540200', 13),
('540234', '亚东县', 3, '540200', 14),
('540235', '吉隆县', 3, '540200', 15),
('540236', '聂拉木县', 3, '540200', 16),
('540237', '萨嘎县', 3, '540200', 17),
('540238', '岗巴县', 3, '540200', 18);

-- 昌都市 (540300) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('540301', '昌都市区', 3, '540300', 1),
('540321', '江达县', 3, '540300', 2),
('540322', '贡觉县', 3, '540300', 3),
('540323', '类乌齐县', 3, '540300', 4),
('540324', '丁青县', 3, '540300', 5),
('540325', '察雅县', 3, '540300', 6),
('540326', '八宿县', 3, '540300', 7),
('540327', '左贡县', 3, '540300', 8),
('540328', '芒康县', 3, '540300', 9),
('540329', '洛隆县', 3, '540300', 10),
('540330', '边坝县', 3, '540300', 11);

-- 林芝市 (540400) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('540401', '林芝市区', 3, '540400', 1),
('540421', '工布江达县', 3, '540400', 2),
('540422', '米林县', 3, '540400', 3),
('540423', '墨脱县', 3, '540400', 4),
('540424', '波密县', 3, '540400', 5),
('540425', '察隅县', 3, '540400', 6),
('540426', '朗县', 3, '540400', 7);

-- 山南市 (540500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('540501', '山南市区', 3, '540500', 1),
('540521', '扎囊县', 3, '540500', 2),
('540522', '贡嘎县', 3, '540500', 3),
('540523', '桑日县', 3, '540500', 4),
('540524', '琼结县', 3, '540500', 5),
('540525', '曲松县', 3, '540500', 6),
('540526', '措美县', 3, '540500', 7),
('540527', '洛扎县', 3, '540500', 8),
('540528', '加查县', 3, '540500', 9),
('540529', '隆子县', 3, '540500', 10),
('540530', '错那县', 3, '540500', 11),
('540531', '浪卡子县', 3, '540500', 12);

-- 那曲市 (540600) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('540601', '那曲市区', 3, '540600', 1),
('540621', '嘉黎县', 3, '540600', 2),
('540622', '比如县', 3, '540600', 3),
('540623', '聂荣县', 3, '540600', 4),
('540624', '安多县', 3, '540600', 5),
('540625', '申扎县', 3, '540600', 6),
('540626', '索县', 3, '540600', 7),
('540627', '班戈县', 3, '540600', 8),
('540628', '巴青县', 3, '540600', 9),
('540629', '尼玛县', 3, '540600', 10),
('540630', '双湖县', 3, '540600', 11);

-- 阿里地区 (542500) - 区县
INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES
('542521', '普兰县', 3, '542500', 1),
('542522', '札达县', 3, '542500', 2),
('542523', '噶尔县', 3, '542500', 3),
('542524', '日土县', 3, '542500', 4),
('542525', '革吉县', 3, '542500', 5),
('542526', '改则县', 3, '542500', 6),
('542527', '措勤县', 3, '542500', 7);
-- 陕西省区县数据
-- region_level = 3 (区县级)

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- ============ 西安市 (610100) ============
('610101', '西安市市区', 3, '610100', 1),
('610102', '新城区', 3, '610100', 2),
('610103', '碑林区', 3, '610100', 3),
('610104', '莲湖区', 3, '610100', 4),
('610111', '灞桥区', 3, '610100', 5),
('610112', '未央区', 3, '610100', 6),
('610113', '雁塔区', 3, '610100', 7),
('610114', '阎良区', 3, '610100', 8),
('610115', '临潼区', 3, '610100', 9),
('610116', '长安区', 3, '610100', 10),
('610117', '高陵区', 3, '610100', 11),
('610118', '鄠邑区', 3, '610100', 12),
('610122', '蓝田县', 3, '610100', 13),
('610124', '周至县', 3, '610100', 14),

-- ============ 铜川市 (610200) ============
('610201', '铜川市市区', 3, '610200', 1),
('610202', '王益区', 3, '610200', 2),
('610203', '印台区', 3, '610200', 3),
('610204', '耀州区', 3, '610200', 4),
('610222', '宜君县', 3, '610200', 5),

-- ============ 宝鸡市 (610300) ============
('610301', '宝鸡市市区', 3, '610300', 1),
('610302', '渭滨区', 3, '610300', 2),
('610303', '金台区', 3, '610300', 3),
('610304', '陈仓区', 3, '610300', 4),
('610322', '凤翔县', 3, '610300', 5),
('610323', '岐山县', 3, '610300', 6),
('610324', '扶风县', 3, '610300', 7),
('610326', '眉县', 3, '610300', 8),
('610327', '陇县', 3, '610300', 9),
('610328', '千阳县', 3, '610300', 10),
('610329', '麟游县', 3, '610300', 11),
('610330', '凤县', 3, '610300', 12),
('610331', '太白县', 3, '610300', 13),

-- ============ 咸阳市 (610400) ============
('610401', '咸阳市市区', 3, '610400', 1),
('610402', '秦都区', 3, '610400', 2),
('610403', '杨陵区', 3, '610400', 3),
('610404', '渭城区', 3, '610400', 4),
('610422', '三原县', 3, '610400', 5),
('610423', '泾阳县', 3, '610400', 6),
('610424', '乾县', 3, '610400', 7),
('610425', '礼泉县', 3, '610400', 8),
('610426', '永寿县', 3, '610400', 9),
('610428', '长武县', 3, '610400', 10),
('610429', '旬邑县', 3, '610400', 11),
('610430', '淳化县', 3, '610400', 12),
('610431', '武功县', 3, '610400', 13),
('610481', '兴平市', 3, '610400', 14),
('610482', '彬州市', 3, '610400', 15),

-- ============ 渭南市 (610500) ============
('610501', '渭南市市区', 3, '610500', 1),
('610502', '临渭区', 3, '610500', 2),
('610503', '华州区', 3, '610500', 3),
('610522', '潼关县', 3, '610500', 4),
('610523', '大荔县', 3, '610500', 5),
('610524', '合阳县', 3, '610500', 6),
('610525', '澄城县', 3, '610500', 7),
('610526', '蒲城县', 3, '610500', 8),
('610527', '白水县', 3, '610500', 9),
('610528', '富平县', 3, '610500', 10),
('610581', '韩城市', 3, '610500', 11),
('610582', '华阴市', 3, '610500', 12),

-- ============ 延安市 (610600) ============
('610601', '延安市市区', 3, '610600', 1),
('610602', '宝塔区', 3, '610600', 2),
('610603', '安塞区', 3, '610600', 3),
('610621', '延长县', 3, '610600', 4),
('610622', '延川县', 3, '610600', 5),
('610623', '志丹县', 3, '610600', 6),
('610624', '吴起县', 3, '610600', 7),
('610625', '甘泉县', 3, '610600', 8),
('610626', '富县', 3, '610600', 9),
('610627', '洛川县', 3, '610600', 10),
('610628', '宜川县', 3, '610600', 11),
('610629', '黄龙县', 3, '610600', 12),
('610630', '黄陵县', 3, '610600', 13),
('610631', '子长市', 3, '610600', 14),

-- ============ 汉中市 (610700) ============
('610701', '汉中市市区', 3, '610700', 1),
('610702', '汉台区', 3, '610700', 2),
('610703', '南郑区', 3, '610700', 3),
('610722', '城固县', 3, '610700', 4),
('610723', '洋县', 3, '610700', 5),
('610724', '西乡县', 3, '610700', 6),
('610725', '勉县', 3, '610700', 7),
('610726', '宁强县', 3, '610700', 8),
('610727', '略阳县', 3, '610700', 9),
('610728', '镇巴县', 3, '610700', 10),
('610729', '留坝县', 3, '610700', 11),
('610730', '佛坪县', 3, '610700', 12),

-- ============ 榆林市 (610800) ============
('610801', '榆林市市区', 3, '610800', 1),
('610802', '榆阳区', 3, '610800', 2),
('610803', '横山区', 3, '610800', 3),
('610822', '府谷县', 3, '610800', 4),
('610824', '靖边县', 3, '610800', 5),
('610825', '定边县', 3, '610800', 6),
('610826', '绥德县', 3, '610800', 7),
('610827', '米脂县', 3, '610800', 8),
('610828', '佳县', 3, '610800', 9),
('610829', '吴堡县', 3, '610800', 10),
('610830', '清涧县', 3, '610800', 11),
('610831', '子洲县', 3, '610800', 12),
('610881', '神木市', 3, '610800', 13),

-- ============ 安康市 (610900) ============
('610901', '安康市市区', 3, '610900', 1),
('610902', '汉滨区', 3, '610900', 2),
('610921', '汉阴县', 3, '610900', 3),
('610922', '石泉县', 3, '610900', 4),
('610923', '宁陕县', 3, '610900', 5),
('610924', '紫阳县', 3, '610900', 6),
('610925', '岚皋县', 3, '610900', 7),
('610926', '平利县', 3, '610900', 8),
('610927', '镇坪县', 3, '610900', 9),
('610928', '旬阳县', 3, '610900', 10),
('610929', '白河县', 3, '610900', 11),

-- ============ 商洛市 (611000) ============
('611001', '商洛市市区', 3, '611000', 1),
('611002', '商州区', 3, '611000', 2),
('611021', '洛南县', 3, '611000', 3),
('611022', '丹凤县', 3, '611000', 4),
('611023', '商南县', 3, '611000', 5),
('611024', '山阳县', 3, '611000', 6),
('611025', '镇安县', 3, '611000', 7),
('611026', '柞水县', 3, '611000', 8);
-- 甘肃省区县数据
-- region_level = 3 (区县级)

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- ============ 兰州市 (620100) ============
('620101', '兰州市市区', 3, '620100', 1),
('620102', '城关区', 3, '620100', 2),
('620103', '七里河区', 3, '620100', 3),
('620104', '西固区', 3, '620100', 4),
('620105', '安宁区', 3, '620100', 5),
('620111', '红古区', 3, '620100', 6),
('620121', '永登县', 3, '620100', 7),
('620122', '皋兰县', 3, '620100', 8),
('620123', '榆中县', 3, '620100', 9),

-- ============ 嘉峪关市 (620200) ============
('620201', '嘉峪关市市区', 3, '620200', 1),

-- ============ 金昌市 (620300) ============
('620301', '金昌市市区', 3, '620300', 1),
('620302', '金川区', 3, '620300', 2),
('620321', '永昌县', 3, '620300', 3),

-- ============ 白银市 (620400) ============
('620401', '白银市市区', 3, '620400', 1),
('620402', '白银区', 3, '620400', 2),
('620403', '平川区', 3, '620400', 3),
('620421', '靖远县', 3, '620400', 4),
('620422', '会宁县', 3, '620400', 5),
('620423', '景泰县', 3, '620400', 6),

-- ============ 天水市 (620500) ============
('620501', '天水市市区', 3, '620500', 1),
('620502', '秦州区', 3, '620500', 2),
('620503', '麦积区', 3, '620500', 3),
('620521', '清水县', 3, '620500', 4),
('620522', '秦安县', 3, '620500', 5),
('620523', '甘谷县', 3, '620500', 6),
('620524', '武山县', 3, '620500', 7),
('620525', '张家川回族自治县', 3, '620500', 8),

-- ============ 武威市 (620600) ============
('620601', '武威市市区', 3, '620600', 1),
('620602', '凉州区', 3, '620600', 2),
('620621', '民勤县', 3, '620600', 3),
('620622', '古浪县', 3, '620600', 4),
('620623', '天祝藏族自治县', 3, '620600', 5),

-- ============ 张掖市 (620700) ============
('620701', '张掖市市区', 3, '620700', 1),
('620702', '甘州区', 3, '620700', 2),
('620721', '肃南裕固族自治县', 3, '620700', 3),
('620722', '民乐县', 3, '620700', 4),
('620723', '临泽县', 3, '620700', 5),
('620724', '高台县', 3, '620700', 6),
('620725', '山丹县', 3, '620700', 7),

-- ============ 平凉市 (620800) ============
('620801', '平凉市市区', 3, '620800', 1),
('620802', '崆峒区', 3, '620800', 2),
('620821', '泾川县', 3, '620800', 3),
('620822', '灵台县', 3, '620800', 4),
('620823', '崇信县', 3, '620800', 5),
('620825', '庄浪县', 3, '620800', 6),
('620826', '静宁县', 3, '620800', 7),
('620881', '华亭市', 3, '620800', 8),

-- ============ 酒泉市 (620900) ============
('620901', '酒泉市市区', 3, '620900', 1),
('620902', '肃州区', 3, '620900', 2),
('620921', '金塔县', 3, '620900', 3),
('620922', '瓜州县', 3, '620900', 4),
('620923', '肃北蒙古族自治县', 3, '620900', 5),
('620924', '阿克塞哈萨克族自治县', 3, '620900', 6),
('620981', '玉门市', 3, '620900', 7),
('620982', '敦煌市', 3, '620900', 8),

-- ============ 庆阳市 (621000) ============
('621001', '庆阳市市区', 3, '621000', 1),
('621002', '西峰区', 3, '621000', 2),
('621021', '庆城县', 3, '621000', 3),
('621022', '环县', 3, '621000', 4),
('621023', '华池县', 3, '621000', 5),
('621024', '合水县', 3, '621000', 6),
('621025', '正宁县', 3, '621000', 7),
('621026', '宁县', 3, '621000', 8),
('621027', '镇原县', 3, '621000', 9),

-- ============ 定西市 (621100) ============
('621101', '定西市市区', 3, '621100', 1),
('621102', '安定区', 3, '621100', 2),
('621121', '通渭县', 3, '621100', 3),
('621122', '陇西县', 3, '621100', 4),
('621123', '渭源县', 3, '621100', 5),
('621124', '临洮县', 3, '621100', 6),
('621125', '漳县', 3, '621100', 7),
('621126', '岷县', 3, '621100', 8),

-- ============ 陇南市 (621200) ============
('621201', '陇南市市区', 3, '621200', 1),
('621202', '武都区', 3, '621200', 2),
('621221', '成县', 3, '621200', 3),
('621222', '文县', 3, '621200', 4),
('621223', '宕昌县', 3, '621200', 5),
('621224', '康县', 3, '621200', 6),
('621225', '西和县', 3, '621200', 7),
('621226', '礼县', 3, '621200', 8),
('621227', '徽县', 3, '621200', 9),
('621228', '两当县', 3, '621200', 10),

-- ============ 临夏回族自治州 (622900) ============
('622901', '临夏州市区', 3, '622900', 1),
('622901', '临夏市', 3, '622900', 2),
('622921', '临夏县', 3, '622900', 3),
('622922', '康乐县', 3, '622900', 4),
('622923', '永靖县', 3, '622900', 5),
('622924', '广河县', 3, '622900', 6),
('622925', '和政县', 3, '622900', 7),
('622926', '东乡族自治县', 3, '622900', 8),
('622927', '积石山保安族东乡族撒拉族自治县', 3, '622900', 9),

-- ============ 甘南藏族自治州 (623000) ============
('623001', '甘南州市区', 3, '623000', 1),
('623001', '合作市', 3, '623000', 2),
('623021', '临潭县', 3, '623000', 3),
('623022', '卓尼县', 3, '623000', 4),
('623023', '舟曲县', 3, '623000', 5),
('623024', '迭部县', 3, '623000', 6),
('623025', '玛曲县', 3, '623000', 7),
('623026', '碌曲县', 3, '623000', 8),
('623027', '夏河县', 3, '623000', 9);
-- 青海省区县数据
-- region_level = 3 (区县级)

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- ============ 西宁市 (630100) ============
('630101', '西宁市市区', 3, '630100', 1),
('630102', '城东区', 3, '630100', 2),
('630103', '城中区', 3, '630100', 3),
('630104', '城西区', 3, '630100', 4),
('630105', '城北区', 3, '630100', 5),
('630121', '大通回族土族自治县', 3, '630100', 6),
('630122', '湟中区', 3, '630100', 7),
('630123', '湟源县', 3, '630100', 8),

-- ============ 海东市 (630200) ============
('630201', '海东市市区', 3, '630200', 1),
('630202', '乐都区', 3, '630200', 2),
('630203', '平安区', 3, '630200', 3),
('630222', '民和回族土族自治县', 3, '630200', 4),
('630223', '互助土族自治县', 3, '630200', 5),
('630224', '化隆回族自治县', 3, '630200', 6),
('630225', '循化撒拉族自治县', 3, '630200', 7),

-- ============ 海北藏族自治州 (632200) ============
('632221', '门源回族自治县', 3, '632200', 1),
('632222', '祁连县', 3, '632200', 2),
('632223', '海晏县', 3, '632200', 3),
('632224', '刚察县', 3, '632200', 4),

-- ============ 黄南藏族自治州 (632300) ============
('632321', '同仁市', 3, '632300', 1),
('632322', '尖扎县', 3, '632300', 2),
('632323', '泽库县', 3, '632300', 3),
('632324', '河南蒙古族自治县', 3, '632300', 4),

-- ============ 海南藏族自治州 (632500) ============
('632521', '共和县', 3, '632500', 1),
('632522', '同德县', 3, '632500', 2),
('632523', '贵德县', 3, '632500', 3),
('632524', '兴海县', 3, '632500', 4),
('632525', '贵南县', 3, '632500', 5),

-- ============ 果洛藏族自治州 (632600) ============
('632621', '玛沁县', 3, '632600', 1),
('632622', '班玛县', 3, '632600', 2),
('632623', '甘德县', 3, '632600', 3),
('632624', '达日县', 3, '632600', 4),
('632625', '久治县', 3, '632600', 5),
('632626', '玛多县', 3, '632600', 6),

-- ============ 玉树藏族自治州 (632700) ============
('632701', '玉树市', 3, '632700', 1),
('632722', '杂多县', 3, '632700', 2),
('632723', '称多县', 3, '632700', 3),
('632724', '治多县', 3, '632700', 4),
('632725', '囊谦县', 3, '632700', 5),
('632726', '曲麻莱县', 3, '632700', 6),

-- ============ 海西蒙古族藏族自治州 (632800) ============
('632801', '格尔木市', 3, '632800', 1),
('632802', '德令哈市', 3, '632800', 2),
('632803', '茫崖市', 3, '632800', 3),
('632821', '乌兰县', 3, '632800', 4),
('632822', '都兰县', 3, '632800', 5),
('632823', '天峻县', 3, '632800', 6);
-- 宁夏回族自治区区县数据
-- region_level = 3 (区县级)

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- ============ 银川市 (640100) ============
('640101', '银川市市区', 3, '640100', 1),
('640104', '兴庆区', 3, '640100', 2),
('640105', '西夏区', 3, '640100', 3),
('640106', '金凤区', 3, '640100', 4),
('640121', '永宁县', 3, '640100', 5),
('640122', '贺兰县', 3, '640100', 6),
('640181', '灵武市', 3, '640100', 7),

-- ============ 石嘴山市 (640200) ============
('640201', '石嘴山市市区', 3, '640200', 1),
('640202', '大武口区', 3, '640200', 2),
('640205', '惠农区', 3, '640200', 3),
('640221', '平罗县', 3, '640200', 4),

-- ============ 吴忠市 (640300) ============
('640301', '吴忠市市区', 3, '640300', 1),
('640302', '利通区', 3, '640300', 2),
('640303', '红寺堡区', 3, '640300', 3),
('640323', '盐池县', 3, '640300', 4),
('640324', '同心县', 3, '640300', 5),
('640381', '青铜峡市', 3, '640300', 6),

-- ============ 固原市 (640400) ============
('640401', '固原市市区', 3, '640400', 1),
('640402', '原州区', 3, '640400', 2),
('640422', '西吉县', 3, '640400', 3),
('640423', '隆德县', 3, '640400', 4),
('640424', '泾源县', 3, '640400', 5),
('640425', '彭阳县', 3, '640400', 6),

-- ============ 中卫市 (640500) ============
('640501', '中卫市市区', 3, '640500', 1),
('640502', '沙坡头区', 3, '640500', 2),
('640521', '中宁县', 3, '640500', 3),
('640522', '海原县', 3, '640500', 4);
-- 新疆维吾尔自治区区县数据
-- region_level = 3 (区县级)

INSERT IGNORE INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES

-- ============ 乌鲁木齐市 (650100) ============
('650101', '乌鲁木齐市市区', 3, '650100', 1),
('650102', '天山区', 3, '650100', 2),
('650103', '沙依巴克区', 3, '650100', 3),
('650104', '新市区', 3, '650100', 4),
('650105', '水磨沟区', 3, '650100', 5),
('650106', '头屯河区', 3, '650100', 6),
('650107', '达坂城区', 3, '650100', 7),
('650109', '米东区', 3, '650100', 8),
('650121', '乌鲁木齐县', 3, '650100', 9),

-- ============ 克拉玛依市 (650200) ============
('650201', '克拉玛依市市区', 3, '650200', 1),
('650202', '独山子区', 3, '650200', 2),
('650203', '克拉玛依区', 3, '650200', 3),
('650204', '白碱滩区', 3, '650200', 4),
('650205', '乌尔禾区', 3, '650200', 5),

-- ============ 吐鲁番市 (650400) ============
('650401', '吐鲁番市市区', 3, '650400', 1),
('650402', '高昌区', 3, '650400', 2),
('650421', '鄯善县', 3, '650400', 3),
('650422', '托克逊县', 3, '650400', 4),

-- ============ 哈密市 (650500) ============
('650501', '哈密市市区', 3, '650500', 1),
('650502', '伊州区', 3, '650500', 2),
('650521', '巴里坤哈萨克自治县', 3, '650500', 3),
('650522', '伊吾县', 3, '650500', 4),

-- ============ 昌吉回族自治州 (652300) ============
('652301', '昌吉市', 3, '652300', 1),
('652302', '阜康市', 3, '652300', 2),
('652323', '呼图壁县', 3, '652300', 3),
('652324', '玛纳斯县', 3, '652300', 4),
('652325', '奇台县', 3, '652300', 5),
('652327', '吉木萨尔县', 3, '652300', 6),
('652328', '木垒哈萨克自治县', 3, '652300', 7),

-- ============ 博尔塔拉蒙古自治州 (652700) ============
('652701', '博乐市', 3, '652700', 1),
('652722', '精河县', 3, '652700', 2),
('652723', '温泉县', 3, '652700', 3),

-- ============ 巴音郭楞蒙古自治州 (652800) ============
('652801', '库尔勒市', 3, '652800', 1),
('652822', '轮台县', 3, '652800', 2),
('652823', '尉犁县', 3, '652800', 3),
('652824', '若羌县', 3, '652800', 4),
('652825', '且末县', 3, '652800', 5),
('652826', '焉耆回族自治县', 3, '652800', 6),
('652827', '和静县', 3, '652800', 7),
('652828', '和硕县', 3, '652800', 8),
('652829', '博湖县', 3, '652800', 9),

-- ============ 阿克苏地区 (652900) ============
('652901', '阿克苏市', 3, '652900', 1),
('652922', '温宿县', 3, '652900', 2),
('652923', '库车市', 3, '652900', 3),
('652924', '沙雅县', 3, '652900', 4),
('652925', '新和县', 3, '652900', 5),
('652926', '拜城县', 3, '652900', 6),
('652927', '乌什县', 3, '652900', 7),
('652928', '阿瓦提县', 3, '652900', 8),
('652929', '柯坪县', 3, '652900', 9),

-- ============ 克孜勒苏柯尔克孜自治州 (653000) ============
('653001', '阿图什市', 3, '653000', 1),
('653022', '阿克陶县', 3, '653000', 2),
('653023', '阿合奇县', 3, '653000', 3),
('653024', '乌恰县', 3, '653000', 4),

-- ============ 喀什地区 (653100) ============
('653101', '喀什市', 3, '653100', 1),
('653121', '疏附县', 3, '653100', 2),
('653122', '疏勒县', 3, '653100', 3),
('653123', '英吉沙县', 3, '653100', 4),
('653124', '泽普县', 3, '653100', 5),
('653125', '莎车县', 3, '653100', 6),
('653126', '叶城县', 3, '653100', 7),
('653127', '麦盖提县', 3, '653100', 8),
('653128', '岳普湖县', 3, '653100', 9),
('653129', '伽师县', 3, '653100', 10),
('653130', '巴楚县', 3, '653100', 11),
('653131', '塔什库尔干塔吉克自治县', 3, '653100', 12),

-- ============ 和田地区 (653200) ============
('653201', '和田市', 3, '653200', 1),
('653221', '和田县', 3, '653200', 2),
('653222', '墨玉县', 3, '653200', 3),
('653223', '皮山县', 3, '653200', 4),
('653224', '洛浦县', 3, '653200', 5),
('653225', '策勒县', 3, '653200', 6),
('653226', '于田县', 3, '653200', 7),
('653227', '民丰县', 3, '653200', 8),

-- ============ 伊犁哈萨克自治州 (654000) ============
('654002', '伊宁市', 3, '654000', 1),
('654003', '奎屯市', 3, '654000', 2),
('654004', '霍尔果斯市', 3, '654000', 3),
('654021', '伊宁县', 3, '654000', 4),
('654022', '察布查尔锡伯自治县', 3, '654000', 5),
('654023', '霍城县', 3, '654000', 6),
('654024', '巩留县', 3, '654000', 7),
('654025', '新源县', 3, '654000', 8),
('654026', '昭苏县', 3, '654000', 9),
('654027', '特克斯县', 3, '654000', 10),
('654028', '尼勒克县', 3, '654000', 11),

-- ============ 塔城地区 (654200) ============
('654201', '塔城市', 3, '654200', 1),
('654202', '乌苏市', 3, '654200', 2),
('654221', '额敏县', 3, '654200', 3),
('654223', '沙湾市', 3, '654200', 4),
('654224', '托里县', 3, '654200', 5),
('654225', '裕民县', 3, '654200', 6),
('654226', '和布克赛尔蒙古自治县', 3, '654200', 7),

-- ============ 阿勒泰地区 (654300) ============
('654301', '阿勒泰市', 3, '654300', 1),
('654321', '布尔津县', 3, '654300', 2),
('654322', '富蕴县', 3, '654300', 3),
('654323', '福海县', 3, '654300', 4),
('654324', '哈巴河县', 3, '654300', 5),
('654325', '青河县', 3, '654300', 6),
('654326', '吉木乃县', 3, '654300', 7);
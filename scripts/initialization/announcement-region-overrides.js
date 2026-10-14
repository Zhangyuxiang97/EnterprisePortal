/**
 * 旧站数据中已通过标题和项目建设地点确认的跨省项目。
 *
 * 旧清洗程序默认所有公告都属于河南省；这里按项目系列覆盖完整区域，
 * 避免根据正文中的供应商或中标候选人地址跨省反推项目所在地。
 */
const ANNOUNCEMENT_REGION_OVERRIDES = [
  {
    titlePrefix: '中国烟草总公司郑州烟草研究院北京实验室',
    province: '北京市', city: '北京市', district: '市区'
  },
  {
    titlePrefix: '赞皇县龙门岗45MW分散式风力发电项目',
    province: '河北省', city: '石家庄市', district: '赞皇县'
  },
  {
    titlePrefix: '内蒙古清源环保有限公司年产20000吨纯三氯化铁净水剂建设项目',
    province: '内蒙古自治区', city: '鄂尔多斯市', district: '鄂托克旗'
  },
  {
    titlePrefix: '临河IT创业园（众城新都汇中央公馆）工程建设项目',
    province: '内蒙古自治区', city: '巴彦淖尔市', district: '市区'
  },
  {
    titlePrefix: '辽宁营口盖州市32.61MWp地面分布式光伏发电项目',
    province: '辽宁省', city: '营口市', district: '盖州市'
  },
  {
    titlePrefix: '中垭（浙江）智能科技有限公司钢结构配件及系统窗生产基地基本建设项目',
    province: '浙江省', city: '台州市', district: ''
  },
  {
    titlePrefix: '合肥新质产业总部中心项目',
    province: '安徽省', city: '合肥市', district: '包河区'
  },
  {
    titlePrefix: '湖北黄冈黄梅平北46MW分散式风电项目',
    province: '湖北省', city: '黄冈市', district: '黄梅县'
  },
  {
    titlePrefix: '岚皋县茶溪山庄康养项目',
    province: '陕西省', city: '安康市', district: '岚皋县'
  },
  {
    titlePrefix: '武山杨河45MW分散式风电项目',
    province: '甘肃省', city: '天水市', district: '武山县'
  }
];

function findAnnouncementRegionOverride(title) {
  if (!title) return null;
  return ANNOUNCEMENT_REGION_OVERRIDES.find(rule => title.startsWith(rule.titlePrefix)) || null;
}

module.exports = {
  ANNOUNCEMENT_REGION_OVERRIDES,
  findAnnouncementRegionOverride
};

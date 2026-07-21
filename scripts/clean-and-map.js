/**
 * 清理HTML内容 + 映射区域字典
 */

const fs = require('fs');
const path = require('path');
const { findAnnouncementRegionOverride } = require('./announcement-region-overrides');
const { HENAN_DISTRICT_TO_CITY } = require('./region-dictionary');

const INPUT_FILE = path.join(__dirname, 'data', 'transformed-data.json');
const OUTPUT_FILE = path.join(__dirname, 'data', 'final-data.json');

// 区域字典映射
const PROVINCE_MAP = {
  '河南': '河南省', '安徽': '安徽省', '北京': '北京市', '天津': '天津市',
  '河北': '河北省', '山西': '山西省', '内蒙古': '内蒙古自治区',
  '辽宁': '辽宁省', '吉林': '吉林省', '黑龙江': '黑龙江省',
  '上海': '上海市', '江苏': '江苏省', '浙江': '浙江省',
  '福建': '福建省', '江西': '江西省', '山东': '山东省',
  '湖北': '湖北省', '湖南': '湖南省', '广东': '广东省',
  '广西': '广西壮族自治区', '海南': '海南省', '重庆': '重庆市',
  '四川': '四川省', '贵州': '贵州省', '云南': '云南省',
  '西藏': '西藏自治区', '陕西': '陕西省', '甘肃': '甘肃省',
  '青海': '青海省', '宁夏': '宁夏回族自治区', '新疆': '新疆维吾尔自治区'
};

/**
 * 清理Word样式标签
 */
function cleanHtml(html) {
  if (!html) return '';
  return html
    // 移除Word命名空间样式
    .replace(/mso-[^:]+:[^;"]+;?/gi, '')
    // 移除Word条件注释
    .replace(/<!--\[if[\s\S]*?endif\]-->/gi, '')
    .replace(/<!--\[endif\]-->/gi, '')
    // 移除空的span标签
    .replace(/<span[^>]*>\s*<\/span>/gi, '')
    // 移除Word特有的XML标签
    .replace(/<o:p[\s\S]*?<\/o:p>/gi, '')
    .replace(/<w:[^>]+>[\s\S]*?<\/w:[^>]+>/gi, '')
    // 移除style标签中的Word样式
    .replace(/<style[^>]*>[\s\S]*?<\/style>/gi, '')
    // 清理多余的空格和换行
    .replace(/\s+/g, ' ')
    .replace(/>\s+</g, '><')
    .trim();
}

// 城市简称映射
const CITY_MAP = {
  '信阳': '信阳市', '周口': '周口市', '安阳': '安阳市', '南阳': '南阳市',
  '濮阳': '濮阳市', '漯河': '漯河市', '开封': '开封市', '鹤壁': '鹤壁市',
  '驻马店': '驻马店市', '商丘': '商丘市', '焦作': '焦作市', '新乡': '新乡市',
  '许昌': '许昌市', '三门峡': '三门峡市', '平顶山': '平顶山市', '济源': '济源市',
  '洛阳': '洛阳市', '郑州': '郑州市'
};

/**
 * 映射区域到字典格式
 */
function mapRegion(item) {
  const regionOverride = findAnnouncementRegionOverride(item.title);
  let province = regionOverride?.province || item.province || '';
  let city = regionOverride?.city || item.city || '';
  let district = regionOverride ? regionOverride.district : (item.district || '');

  // 优先从县名反推城市
  if (!regionOverride && district && HENAN_DISTRICT_TO_CITY[district]) {
    city = HENAN_DISTRICT_TO_CITY[district];
  }
  // 如果还是没有城市，从标题提取
  if (!city || city === '郑州市') {
    const titleCityMatch = item.title.match(/(信阳|周口|安阳|南阳|濮阳|漯河|开封|鹤壁|驻马店|商丘|焦作|新乡|许昌|三门峡|平顶山|济源|洛阳)/);
    if (titleCityMatch && CITY_MAP[titleCityMatch[1]]) {
      city = CITY_MAP[titleCityMatch[1]];
    }
  }

  // 省份：补"省"后缀
  if (province && PROVINCE_MAP[province]) {
    province = PROVINCE_MAP[province];
  } else if (province && !province.endsWith('省') && !province.endsWith('市') && !province.endsWith('自治区')) {
    province = province + '省';
  }

  // 城市：确保有"市"后缀
  if (city && !city.endsWith('市') && !city.endsWith('地区') && !city.endsWith('州')) {
    city = city + '市';
  }

  // 区县：清理前缀，保留纯县名
  if (district && district !== '市区') {
    // 去掉省/市前缀
    let cleanDistrict = district
      .replace(/^河南省/, '')
      .replace(/^.*市/, '')
      .replace(/^.*地区/, '');
    // 提取县名
    const countyMatch = cleanDistrict.match(/^([一-龥]{2,}县)$/);
    if (countyMatch) {
      district = countyMatch[1];
    } else if (cleanDistrict && cleanDistrict !== district) {
      district = cleanDistrict;
    }
    // 去掉"年"开头的错误提取
    if (district.startsWith('年')) {
      district = district.substring(1);
    }
  }

  // 项目区域：拼接完整地址
  const regionParts = [province, city, district].filter(Boolean);
  const project_region = regionParts.join(' ');

  return { province, city, district, project_region };
}

/**
 * 主函数
 */
function main() {
  const data = JSON.parse(fs.readFileSync(INPUT_FILE, 'utf-8'));

  console.log('原始数据:', data.length, '条');

  let cleanedCount = 0;
  let regionMappedCount = 0;

  const result = data.map(item => {
    // 清理HTML
    const originalContent = item.content;
    const cleanedContent = cleanHtml(item.content);
    if (cleanedContent !== originalContent) cleanedCount++;

    // 映射区域
    const originalProvince = item.province;
    const originalCity = item.city;
    const originalDistrict = item.district;
    const region = mapRegion(item);
    if (region.province !== originalProvince || region.city !== originalCity || region.district !== originalDistrict) {
      regionMappedCount++;
    }

    // 重新计算fieldStatus
    const fieldStatus = {};
    fieldStatus.title = 'direct';
    fieldStatus.content = 'direct';
    fieldStatus.publisher = item.publisher ? 'direct' : 'missing';
    fieldStatus.publish_time = item.publish_time ? 'direct' : 'missing';
    fieldStatus.view_count = 'direct';
    fieldStatus.business_type = 'inferred';
    fieldStatus.notice_type = 'inferred';
    fieldStatus.procurement_type = item.procurement_type ? 'inferred' : 'missing';
    fieldStatus.bidder = item.bidder ? 'inferred' : 'missing';
    fieldStatus.budget_amount = item.budget_amount ? 'inferred' : 'missing';
    fieldStatus.deadline = item.deadline ? 'inferred' : 'missing';
    fieldStatus.province = region.province ? 'inferred' : 'missing';
    fieldStatus.city = region.city ? 'inferred' : 'missing';
    fieldStatus.district = region.district ? 'inferred' : 'missing';
    fieldStatus.project_region = region.project_region ? 'inferred' : 'missing';
    fieldStatus.winner = item.winner ? 'inferred' : (item.notice_type === 'result' ? 'missing' : 'inferred');
    fieldStatus.attachment_ids = 'inferred';
    fieldStatus.is_top = 'inferred';
    fieldStatus.status = 'inferred';

    return {
      // 基本信息
      title: item.title,
      content: cleanedContent,
      publisher: item.publisher,
      publish_time: item.publish_time,
      view_count: item.view_count,

      // 业务信息
      business_type: item.business_type,
      notice_type: item.notice_type,
      procurement_type: item.procurement_type,

      // 招标信息
      bidder: item.bidder,
      budget_amount: item.budget_amount,
      deadline: item.deadline,
      winner: item.winner,

      // 区域信息（映射后）
      ...region,

      // 其他
      is_top: 0,
      status: 1,
      is_deleted: 0,

      // 字段状态
      _fieldStatus: fieldStatus,

      // 原始数据
      _raw: item._raw
    };
  });

  // 保存结果
  fs.writeFileSync(OUTPUT_FILE, JSON.stringify(result, null, 2), 'utf-8');

  console.log('HTML清理:', cleanedCount, '条');
  console.log('区域映射:', regionMappedCount, '条');
  console.log('输出文件:', OUTPUT_FILE);

  // 统计区域分布
  const provinces = {};
  const cities = {};
  const districts = {};
  result.forEach(item => {
    if (item.province) provinces[item.province] = (provinces[item.province] || 0) + 1;
    if (item.city) cities[item.city] = (cities[item.city] || 0) + 1;
    if (item.district) districts[item.district] = (districts[item.district] || 0) + 1;
  });

  console.log('\n=== 省份分布 ===');
  Object.entries(provinces).sort((a, b) => b[1] - a[1]).forEach(([k, v]) => console.log(k + ': ' + v));

  console.log('\n=== 城市分布(前10) ===');
  Object.entries(cities).sort((a, b) => b[1] - a[1]).slice(0, 10).forEach(([k, v]) => console.log(k + ': ' + v));

  console.log('\n=== 区县分布(前10) ===');
  Object.entries(districts).sort((a, b) => b[1] - a[1]).slice(0, 10).forEach(([k, v]) => console.log(k + ': ' + v));
}

if (require.main === module) {
  main();
}

module.exports = { cleanHtml, mapRegion };

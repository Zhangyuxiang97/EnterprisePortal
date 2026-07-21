/**
 * 生成首次部署用的公告 SQL。
 *
 * 公告文件直接写入区域编码，避免首次部署后再执行区域转换 UPDATE。
 */
const fs = require('fs');
const path = require('path');
const { findAnnouncementRegionOverride } = require('./announcement-region-overrides');

const ROOT = path.join(__dirname, '..');
const DATA_FILE = path.join(__dirname, 'data', 'final-data.json');
const REGION_FILES = [
  'SQL/02_hailong_consulting_init_data.sql',
  'SQL/03_region_dictionary_batch1_henan.sql',
  'SQL/04_region_dictionary_batch2_north_east_east.sql',
  'SQL/05_region_dictionary_batch3_central_south_west.sql'
].map(file => path.join(ROOT, file));

const REGION_TUPLE_PATTERN = /\(\s*'([^']+)'\s*,\s*'([^']*)'\s*,\s*(\d+)\s*,\s*(?:'([^']*)'|NULL)\s*,\s*\d+\s*\)/g;

function loadRegions() {
  const byCode = new Map();
  for (const file of REGION_FILES) {
    const sql = fs.readFileSync(file, 'utf8');
    let match;
    while ((match = REGION_TUPLE_PATTERN.exec(sql)) !== null) {
      if (!/^\d{6}$/.test(match[1])) continue;
      const level = Number(match[3]);
      if (![1, 2, 3].includes(level)) continue;
      byCode.set(match[1], {
        code: match[1], name: match[2], level, parentCode: match[4] || null
      });
    }
  }

  const byLevelName = new Map();
  const byName = new Map();
  for (const region of byCode.values()) {
    const key = `${region.level}:${region.parentCode || ''}:${region.name}`;
    byLevelName.set(key, region);
    const nameKey = `${region.level}:${region.name}`;
    if (!byName.has(nameKey)) byName.set(nameKey, []);
    byName.get(nameKey).push(region);
  }
  return { byCode, byLevelName, byName };
}

function escapeSql(value) {
  if (value === null || value === undefined || value === '') return 'NULL';
  return `'${String(value).replace(/\\/g, '\\\\').replace(/'/g, "''")}'`;
}

function formatDate(value) {
  if (!value) return 'NULL';
  return `'${String(value).replace('T', ' ').substring(0, 19)}'`;
}

function formatNumber(value) {
  return value === null || value === undefined || value === '' ? 'NULL' : String(value);
}

function normalizeProvince(value) {
  if (!value) return '';
  const map = {
    河南: '河南省', 安徽: '安徽省', 北京: '北京市', 天津: '天津市',
    河北: '河北省', 山西: '山西省', 内蒙古: '内蒙古自治区', 辽宁: '辽宁省',
    吉林: '吉林省', 黑龙江: '黑龙江省', 上海: '上海市', 江苏: '江苏省',
    浙江: '浙江省', 福建: '福建省', 江西: '江西省', 山东: '山东省',
    湖北: '湖北省', 湖南: '湖南省', 广东: '广东省', 广西: '广西壮族自治区',
    海南: '海南省', 重庆: '重庆市', 四川: '四川省', 贵州: '贵州省',
    云南: '云南省', 西藏: '西藏自治区', 陕西: '陕西省', 甘肃: '甘肃省',
    青海: '青海省', 宁夏: '宁夏回族自治区', 新疆: '新疆维吾尔自治区'
  };
  return map[value] || value;
}

function normalizeCity(value) {
  if (!value) return '';
  if (value.endsWith('市') || value.endsWith('地区') || value.endsWith('州')) return value;
  return `${value}市`;
}

function resolveRegion(item, regions) {
  const override = findAnnouncementRegionOverride(item.title);
  const provinceName = normalizeProvince(override?.province || item.province || '');
  const cityName = normalizeCity(override?.city || item.city || '');
  const districtName = override ? (override.district || '') : (item.district || '');

  const province = regions.byLevelName.get(`1::${provinceName}`);
  const city = province && regions.byLevelName.get(`2:${province.code}:${cityName}`);
  const dictionaryDistrictName = districtName === '市区' && cityName
    ? `${cityName.replace(/市$/, '')}市区`
    : districtName;
  let district = city && dictionaryDistrictName
    ? regions.byLevelName.get(`3:${city.code}:${dictionaryDistrictName}`)
    : null;

  // 清洗数据中个别河南公告的 city 仍是默认的郑州市；用区县字典反查真实地市。
  if (!district && dictionaryDistrictName) {
    const candidates = regions.byName.get(`3:${dictionaryDistrictName}`) || [];
    const candidate = candidates.find(x => {
      const candidateCity = regions.byCode.get(x.parentCode);
      return candidateCity && candidateCity.parentCode === province?.code;
    });
    if (candidate) {
      district = candidate;
      const resolvedCity = regions.byCode.get(candidate.parentCode);
      return {
        provinceCode: province?.code || null,
        cityCode: resolvedCity?.code || null,
        districtCode: district.code,
        provinceName,
        cityName: resolvedCity?.name || cityName,
        districtName
      };
    }
  }

  return {
    provinceCode: province?.code || null,
    cityCode: city?.code || null,
    districtCode: district?.code || null,
    provinceName,
    cityName,
    districtName
  };
}

function categoryOf(item) {
  const category = item._raw?.category;
  if (category === '招标信息') return 'bidding';
  if (category === '中标公示') return 'result';
  if (category === '变更公告') return 'correction';
  return item.notice_type || 'bidding';
}

function renderSql(type, items, regions) {
  const label = type === 'bidding' ? '招标公告' : type === 'result' ? '中标公示' : '变更公告';
  let sql = `-- ${label}（首次部署时直接写入区域编码）\n`;
  sql += `-- 记录数：${items.length}\n\nSET NAMES utf8mb4;\nSET CHARACTER SET utf8mb4;\n\nUSE \`hailong_consulting\`;\n\n`;
  sql += `INSERT INTO announcements (\n`;
  sql += `  title, business_type, notice_type, procurement_type,\n`;
  sql += `  bidder, winner, budget_amount, deadline,\n`;
  sql += `  province, city, district, project_region,\n`;
  sql += `  content, publisher, publish_time, view_count,\n`;
  sql += `  is_top, status, is_deleted, created_at, updated_at\n`;
  sql += `) VALUES\n`;

  const values = items.map(item => {
    const region = resolveRegion(item, regions);
    const row = [
      escapeSql(item.title), escapeSql(item.business_type), escapeSql(item.notice_type), escapeSql(item.procurement_type),
      escapeSql(item.bidder), escapeSql(item.winner), formatNumber(item.budget_amount), formatDate(item.deadline),
      escapeSql(region.provinceCode), escapeSql(region.cityCode), escapeSql(region.districtCode),
      escapeSql([region.provinceName, region.cityName, region.districtName].filter(Boolean).join(' ')),
      escapeSql(item.content), escapeSql(item.publisher), formatDate(item.publish_time), formatNumber(item.view_count),
      '0', '1', '0', 'NOW()', 'NOW()'
    ];
    return `(${row.join(', ')})`;
  });
  sql += `${values.join(',\n')};\n`;
  return sql;
}

function main() {
  const data = JSON.parse(fs.readFileSync(DATA_FILE, 'utf8'));
  const regions = loadRegions();
  const grouped = { bidding: [], result: [], correction: [] };
  const unresolved = [];

  for (const item of data) {
    const type = categoryOf(item);
    if (!grouped[type]) grouped[type] = [];
    grouped[type].push(item);
    const region = resolveRegion(item, regions);
    // 与原初始化清洗规则一致：无法确认的市/区县写 NULL，不阻断整批公告导入。
    if (!region.provinceCode) {
      unresolved.push({ title: item.title, region });
    }
  }

  if (unresolved.length) {
    throw new Error(`存在 ${unresolved.length} 条公告无法解析区域：\n${unresolved.slice(0, 20).map(x => x.title).join('\n')}`);
  }

  const files = {
    bidding: 'SQL/06_announcements_bidding.sql',
    result: 'SQL/07_announcements_result.sql',
    correction: 'SQL/08_announcements_correction.sql'
  };
  for (const [type, file] of Object.entries(files)) {
    fs.writeFileSync(path.join(ROOT, file), renderSql(type, grouped[type], regions), 'utf8');
    console.log(`${file}: ${grouped[type].length} 条`);
  }
}

if (require.main === module) main();

module.exports = { main, loadRegions, resolveRegion };

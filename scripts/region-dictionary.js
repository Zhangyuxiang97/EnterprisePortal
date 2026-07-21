/**
 * 为离线清洗脚本读取项目自带的行政区划字典。
 *
 * 河南省的基础省/市/区县数据集中维护在 02 初始化脚本中。清洗程序从该文件构建
 * 区县到城市的映射，避免再维护一份容易遗漏或过期的手工列表。
 */
const fs = require('fs');
const path = require('path');

const REGION_SQL_FILE = path.join(__dirname, '..', 'SQL', '02_hailong_consulting_init_data.sql');
const REGION_TUPLE_PATTERN = /\(\s*'([^']+)'\s*,\s*'([^']+)'\s*,\s*(\d+)\s*,\s*(?:'([^']*)'|NULL)\s*,\s*\d+\s*\)/g;

function loadRegionDictionary() {
  const sql = fs.readFileSync(REGION_SQL_FILE, 'utf-8');
  const regions = new Map();
  let match;

  while ((match = REGION_TUPLE_PATTERN.exec(sql)) !== null) {
    regions.set(match[1], {
      code: match[1],
      name: match[2],
      level: Number(match[3]),
      parentCode: match[4] || null
    });
  }

  return regions;
}

function buildHenanDistrictToCity(regions) {
  const result = {};

  for (const district of regions.values()) {
    if (district.level !== 3) continue;

    const city = regions.get(district.parentCode);
    if (!city || city.level !== 2 || city.parentCode !== '410000') continue;

    result[district.name] = city.name;
  }

  return Object.freeze(result);
}

const REGION_DICTIONARY = loadRegionDictionary();
const HENAN_DISTRICT_TO_CITY = buildHenanDistrictToCity(REGION_DICTIONARY);

module.exports = { HENAN_DISTRICT_TO_CITY };

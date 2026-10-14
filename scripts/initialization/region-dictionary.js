/** 统一读取首次初始化的地区字典，并为公告生成和离线清洗提供索引。 */
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const REGION_SQL_FILE = path.join(__dirname, '../../SQL/init/03_regions.sql');
const REGION_TUPLE_PATTERN = /\(\s*'([^']+)'\s*,\s*'([^']*)'\s*,\s*(\d+)\s*,\s*(?:'([^']*)'|NULL)\s*,\s*(\d+)\s*\)/g;

function loadRegionDictionary(sql = fs.readFileSync(REGION_SQL_FILE, 'utf8')) {
  const regions = new Map();
  for (const match of sql.matchAll(REGION_TUPLE_PATTERN)) {
    assert.match(match[1], /^\d{6}$/, '地区编码必须为六位数字');
    assert.ok([1, 2, 3].includes(Number(match[3])), '地区层级无效');
    assert.ok(!regions.has(match[1]), `地区编码重复：${match[1]}`);
    regions.set(match[1], { code: match[1], name: match[2], level: Number(match[3]), parentCode: match[4] || null, sortOrder: Number(match[5]) });
  }
  assert.ok(regions.size, '初始化地区字典不能为空');
  return regions;
}

function loadRegions() {
  const byCode = loadRegionDictionary();
  const byLevelName = new Map();
  const byName = new Map();
  for (const region of byCode.values()) {
    byLevelName.set(`${region.level}:${region.parentCode || ''}:${region.name}`, region);
    const key = `${region.level}:${region.name}`;
    if (!byName.has(key)) byName.set(key, []);
    byName.get(key).push(region);
  }
  return { byCode, byLevelName, byName };
}

function buildHenanDistrictToCity(regions) {
  const result = {};
  for (const district of regions.values()) {
    const city = regions.get(district.parentCode);
    if (district.level === 3 && city?.level === 2 && city.parentCode === '410000') result[district.name] = city.name;
  }
  return Object.freeze(result);
}

const HENAN_DISTRICT_TO_CITY = buildHenanDistrictToCity(loadRegionDictionary());
module.exports = { REGION_SQL_FILE, loadRegionDictionary, loadRegions, buildHenanDistrictToCity, HENAN_DISTRICT_TO_CITY };

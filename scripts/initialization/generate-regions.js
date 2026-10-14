/** 格式化唯一地区数据源，保留编码、父级、排序字段及原插入顺序。 */
const fs = require('node:fs');
const { REGION_SQL_FILE, loadRegionDictionary } = require('./region-dictionary');

const escapeSql = value => value === null ? 'NULL' : `'${String(value).replace(/\\/g, '\\\\').replace(/'/g, "''")}'`;

function renderRegions(regions) {
  const rows = [...regions.values()];
  const header = `-- 行政区划字典：${rows.length} 个唯一区域编码。\n` +
    '-- 巩义市编码依据巩义市政府 2025 年“一卡通”政策清单及历史沿革（gongyishi.gov.cn）。\n' +
    '-- 中原区、二七区编码依据郑州市生态环境局重点排污单位名录（sthjj.zhengzhou.gov.cn）。\n';
  return header + '\nSET NAMES utf8mb4;\nSET CHARACTER SET utf8mb4;\nUSE `hailong_consulting`;\n\n' +
    'INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES\n' +
    rows.map(row => `(${escapeSql(row.code)}, ${escapeSql(row.name)}, ${row.level}, ${escapeSql(row.parentCode)}, ${row.sortOrder})`).join(',\n') + ';\n';
}

function main() {
  const regions = loadRegionDictionary();
  fs.writeFileSync(REGION_SQL_FILE, renderRegions(regions), 'utf8');
  console.log(`地区字典：${regions.size} 条`);
}

if (require.main === module) main();
module.exports = { renderRegions, main };

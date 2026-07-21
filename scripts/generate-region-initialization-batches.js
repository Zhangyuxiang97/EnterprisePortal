/**
 * 将现有区域字典整理为三批唯一编码的首次初始化 SQL。
 * 第一批：河南；第二批：华北/东北/华东（含安徽）；第三批：华中/华南/西南/西北及港澳台。
 */
const fs = require('fs');
const path = require('path');

const ROOT = path.join(__dirname, '..');
const sourceFiles = [
  'SQL/02_hailong_consulting_init_data.sql',
  'SQL/03_region_dictionary_batch1_henan.sql',
  'SQL/04_region_dictionary_batch2_north_east_east.sql',
  'SQL/05_region_dictionary_batch3_central_south_west.sql'
].map(file => path.join(ROOT, file));
const tuplePattern = /\(\s*'([^']+)'\s*,\s*'([^']*)'\s*,\s*(\d+)\s*,\s*(?:'([^']*)'|NULL)\s*,\s*(\d+)\s*\)/g;

const batches = [
  { file: 'SQL/03_region_dictionary_batch1_henan.sql', title: '第一批：河南省', prefixes: new Set(['41']) },
  { file: 'SQL/04_region_dictionary_batch2_north_east_east.sql', title: '第二批：华北、东北、华东（含安徽省）', prefixes: new Set(['11', '12', '13', '14', '15', '21', '22', '23', '31', '32', '33', '34', '35', '36', '37']) },
  { file: 'SQL/05_region_dictionary_batch3_central_south_west.sql', title: '第三批：华中、华南、西南、西北及港澳台', prefixes: new Set(['42', '43', '44', '45', '46', '50', '51', '52', '53', '54', '61', '62', '63', '64', '65', '71', '81', '82']) }
];

function escapeSql(value) {
  return `'${String(value).replace(/\\/g, '\\\\').replace(/'/g, "''")}'`;
}

function loadRegions() {
  const regions = new Map();
  for (const file of sourceFiles) {
    const sql = fs.readFileSync(file, 'utf8');
    let match;
    while ((match = tuplePattern.exec(sql)) !== null) {
      if (!/^\d{6}$/.test(match[1])) continue;
      const level = Number(match[3]);
      if (![1, 2, 3].includes(level)) continue;
      regions.set(match[1], {
        code: match[1], name: match[2], level,
        parentCode: match[4] || null, sortOrder: Number(match[5])
      });
    }
  }
  return [...regions.values()].sort((a, b) => a.code.localeCompare(b.code));
}

function render(batch, regions) {
  const rows = regions.filter(region => batch.prefixes.has(region.code.slice(0, 2)));
  let sql = `-- ${batch.title}\n-- 区域编码唯一，首次部署直接执行，无需后续补丁。\n-- 区域数量：${rows.length}\n\n`;
  sql += 'SET NAMES utf8mb4;\nSET CHARACTER SET utf8mb4;\n\nUSE `hailong_consulting`;\n\n';
  sql += 'INSERT INTO region_dictionary (region_code, region_name, region_level, parent_code, sort_order) VALUES\n';
  sql += rows.map((region, index) => {
    const parent = region.parentCode ? escapeSql(region.parentCode) : 'NULL';
    const suffix = index === rows.length - 1 ? ';' : ',';
    return `(${escapeSql(region.code)}, ${escapeSql(region.name)}, ${region.level}, ${parent}, ${region.sortOrder})${suffix}`;
  }).join('\n');
  sql += '\n';
  return { sql, count: rows.length };
}

function main() {
  const regions = loadRegions();
  const covered = new Set();
  for (const batch of batches) {
    const result = render(batch, regions);
    fs.writeFileSync(path.join(ROOT, batch.file), result.sql, 'utf8');
    for (const region of regions) if (batch.prefixes.has(region.code.slice(0, 2))) covered.add(region.code);
    console.log(`${batch.file}: ${result.count} 条`);
  }
  if (covered.size !== regions.length) {
    const missing = regions.filter(region => !covered.has(region.code));
    throw new Error(`存在未分配区域编码：${missing.map(region => region.code).join(', ')}`);
  }
  console.log(`区域编码总数：${regions.length}`);
}

if (require.main === module) main();

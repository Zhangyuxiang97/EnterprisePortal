/**
 * 生成SQL导入脚本
 * 将final-data.json转为SQL INSERT语句
 */

const fs = require('fs');
const path = require('path');

const data = JSON.parse(fs.readFileSync(path.join(__dirname, 'data', 'final-data.json'), 'utf-8'));
const outputFile = path.join(__dirname, '..', 'SQL', '07_announcements_import.sql');

// 转义SQL字符串
function escapeSql(str) {
  if (!str) return 'NULL';
  return "'" + str.replace(/'/g, "''").replace(/\\/g, '\\\\') + "'";
}

// 格式化日期
function formatDate(dateStr) {
  if (!dateStr) return 'NULL';
  return "'" + dateStr.replace('T', ' ').substring(0, 19) + "'";
}

// 格式化数字
function formatNumber(num) {
  if (num === null || num === undefined) return 'NULL';
  return num.toString();
}

// 生成SQL
let sql = `-- ============================================
-- 招标公告数据导入
-- 数据来源: henanhailong.com
-- 生成时间: ${new Date().toISOString()}
-- 总记录数: ${data.length}
-- ============================================

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

USE \`hailong_consulting\`;

`;

// 按原始分类分组（不用推断的notice_type）
const byType = {};
data.forEach(item => {
  // 用原始分类名称映射
  const category = item._raw?.category;
  let type;
  if (category === '招标信息') type = 'bidding';
  else if (category === '中标公示') type = 'result';
  else if (category === '变更公告') type = 'correction';
  else type = item.notice_type || 'bidding'; // 备用

  if (!byType[type]) byType[type] = [];
  byType[type].push(item);
});

// 生成INSERT语句
for (const [type, items] of Object.entries(byType)) {
  sql += `-- ${type === 'bidding' ? '招标公告' : type === 'result' ? '中标公示' : '变更公告'} (${items.length}条)\n`;
  sql += `INSERT INTO announcements (\n`;
  sql += `  title, business_type, notice_type, procurement_type,\n`;
  sql += `  bidder, winner, budget_amount, deadline,\n`;
  sql += `  province, city, district, project_region,\n`;
  sql += `  content, publisher, publish_time, view_count,\n`;
  sql += `  is_top, status, is_deleted, created_at, updated_at\n`;
  sql += `) VALUES\n`;

  const values = items.map((item, idx) => {
    const row = [
      escapeSql(item.title),
      escapeSql(item.business_type),
      escapeSql(item.notice_type),
      escapeSql(item.procurement_type),
      escapeSql(item.bidder),
      escapeSql(item.winner),
      formatNumber(item.budget_amount),
      formatDate(item.deadline),
      escapeSql(item.province),
      escapeSql(item.city),
      escapeSql(item.district),
      escapeSql(item.project_region),
      escapeSql(item.content),
      escapeSql(item.publisher),
      formatDate(item.publish_time),
      formatNumber(item.view_count),
      '0', '1', '0',
      'NOW()', 'NOW()'
    ];
    return `(${row.join(', ')})`;
  });

  sql += values.join(',\n');
  sql += ';\n\n';
}

// 写入文件
fs.writeFileSync(outputFile, sql, 'utf-8');

console.log('SQL文件已生成:', outputFile);
console.log('总记录数:', data.length);
console.log('按类型:', Object.entries(byType).map(([t, i]) => `${t}: ${i.length}`).join(', '));
console.log('文件大小:', (fs.statSync(outputFile).size / 1024 / 1024).toFixed(2), 'MB');

/**
 * AI智能分析缺失字段
 * 将文章内容丢给AI分析，判断能否推理出缺失字段
 */

const fs = require('fs');
const path = require('path');

const data = require('./data/transformed-data.json');

// 找出有缺失字段的条目
const missingBidder = data.filter(d => d._fieldStatus.bidder === 'missing');
const missingBudget = data.filter(d => d._fieldStatus.budget_amount === 'missing');
const missingDeadline = data.filter(d => d._fieldStatus.deadline === 'missing');
const missingWinner = data.filter(d => d._fieldStatus.winner === 'missing');

console.log('=== 缺失字段统计 ===');
console.log('bidder缺失:', missingBidder.length);
console.log('budget缺失:', missingBudget.length);
console.log('deadline缺失:', missingDeadline.length);
console.log('winner缺失:', missingWinner.length);

// 生成分析任务
const tasks = [];

// 分析bidder缺失的
missingBidder.forEach(item => {
  const text = (item.content || '').replace(/<[^>]+>/g, '').substring(0, 2000);
  tasks.push({
    id: item._raw?.id,
    title: item.title,
    field: 'bidder',
    content: text,
    question: '这篇文章的招标人/采购人是谁？如果文中明确提到了招标人、采购人、建设单位的名称，请直接给出。如果没有提到，回答"无法推理"。'
  });
});

// 分析budget缺失的
missingBudget.forEach(item => {
  const text = (item.content || '').replace(/<[^>]+>/g, '').substring(0, 2000);
  tasks.push({
    id: item._raw?.id,
    title: item.title,
    field: 'budget_amount',
    content: text,
    question: '这篇文章的预算金额是多少？如果文中明确提到了预算金额、项目概算、控制价、最高限价、总投资、中标金额、成交金额、报价等，请给出具体数字和单位（元或万元）。如果没有提到，回答"无法推理"。'
  });
});

// 分析deadline缺失的
missingDeadline.forEach(item => {
  const text = (item.content || '').replace(/<[^>]+>/g, '').substring(0, 2000);
  tasks.push({
    id: item._raw?.id,
    title: item.title,
    field: 'deadline',
    content: text,
    question: '这篇文章的投标截止时间是什么？如果文中明确提到了投标截止时间、响应文件递交截止时间、开标时间等，请给出具体日期和时间。如果没有提到，回答"无法推理"。'
  });
});

// 分析winner缺失的（仅中标公示类）
missingWinner.filter(d => d.notice_type === 'result').forEach(item => {
  const text = (item.content || '').replace(/<[^>]+>/g, '').substring(0, 2000);
  tasks.push({
    id: item._raw?.id,
    title: item.title,
    field: 'winner',
    content: text,
    question: '这篇文章的中标人/成交供应商是谁？如果文中明确提到了中标人、中标单位、成交供应商、成交人等，请直接给出名称。如果没有提到，回答"无法推理"。'
  });
});

console.log('\n总分析任务:', tasks.length);

// 保存任务列表供AI分析
fs.writeFileSync('./data/ai-tasks.json', JSON.stringify(tasks.slice(0, 50), null, 2), 'utf-8');
console.log('已保存前50条任务到 data/ai-tasks.json');

// 输出示例
console.log('\n=== 示例任务 ===');
tasks.slice(0, 3).forEach(t => {
  console.log(`\n[${t.field}] ${t.title.substring(0, 50)}`);
  console.log(`问题: ${t.question}`);
  console.log(`内容片段: ${t.content.substring(0, 100)}...`);
});

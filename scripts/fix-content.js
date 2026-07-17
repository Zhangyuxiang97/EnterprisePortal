/**
 * 批量修复内容提取 - 只重抓有缺失字段的条目
 * 从主内容区域获取完整HTML，而不是仅StartFragment到EndFragment
 */

const http = require('http');
const cheerio = require('cheerio');
const iconv = require('iconv-lite');
const fs = require('fs');
const path = require('path');

const DATA_FILE = path.join(__dirname, 'data', 'scraped-data.json');
const CONCURRENCY = 10;
const TIMEOUT = 15000;

function fetchUrl(url) {
  return new Promise((resolve, reject) => {
    const req = http.get(url, { timeout: TIMEOUT }, (res) => {
      const chunks = [];
      res.on('data', chunk => chunks.push(chunk));
      res.on('end', () => resolve(iconv.decode(Buffer.concat(chunks), 'gb2312')));
    });
    req.on('error', reject);
    req.on('timeout', () => { req.destroy(); reject(new Error('timeout')); });
  });
}

function delay(ms) { return new Promise(r => setTimeout(r, ms)); }

async function fetchWithRetry(url, retries = 3) {
  for (let i = 0; i < retries; i++) {
    try { return await fetchUrl(url); }
    catch (e) { if (i === retries - 1) throw e; await delay(1000 * (i + 1)); }
  }
}

function extractFullContent(html) {
  const $ = cheerio.load(html);
  // 取主内容区域
  const $mainTable = $('table').filter(function() {
    const h = $(this).attr('height');
    return h === '450' || h === '630';
  }).first();
  if ($mainTable.length) return $mainTable.html() || '';
  // 备用：取StartFragment到EndFragment
  const bodyHtml = $.html();
  const s = bodyHtml.indexOf('<!--StartFragment-->');
  const e = bodyHtml.indexOf('<!--EndFragment-->');
  if (s !== -1 && e !== -1) return bodyHtml.substring(s + 20, e).trim();
  return '';
}

async function main() {
  const data = JSON.parse(fs.readFileSync(DATA_FILE, 'utf-8'));

  // 找出需要修复的条目（有缺失字段且内容可能不完整）
  const needFix = [];
  data.forEach((item, idx) => {
    const text = item.contentText || '';
    // 如果contentText很短或缺少关键信息，需要重新获取
    if (text.length < 500 ||
        (!text.includes('采购人') && !text.includes('招标人') && !text.includes('中标人') && !text.includes('预算金额'))) {
      needFix.push(idx);
    }
  });

  console.log(`总条目: ${data.length}`);
  console.log(`需要修复: ${needFix.length} 条`);
  console.log(`并发数: ${CONCURRENCY}`);
  console.log('');

  let fixed = 0;
  let failed = 0;

  // 分批并发处理
  for (let i = 0; i < needFix.length; i += CONCURRENCY) {
    const batch = needFix.slice(i, i + CONCURRENCY);
    const promises = batch.map(async (idx) => {
      const item = data[idx];
      try {
        const html = await fetchWithRetry(item.url);
        const contentHtml = extractFullContent(html);
        if (contentHtml && contentHtml.length > (item.contentHtml || '').length) {
          const $content = cheerio.load(contentHtml);
          data[idx].contentHtml = contentHtml;
          data[idx].contentText = $content.text().trim();
          fixed++;
        }
      } catch (e) {
        failed++;
      }
      process.stdout.write(`\r进度: ${fixed + failed}/${needFix.length} (修复${fixed} 失败${failed})`);
    });
    await Promise.all(promises);
    if (i + CONCURRENCY < needFix.length) await delay(200);
  }

  console.log('\n');
  console.log(`修复完成: ${fixed} 条, 失败: ${failed} 条`);

  // 保存
  fs.writeFileSync(DATA_FILE, JSON.stringify(data, null, 2), 'utf-8');
  console.log('已保存:', DATA_FILE);
}

main().catch(console.error);

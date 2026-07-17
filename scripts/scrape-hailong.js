/**
 * 河南海隆旧网站招标信息抓取脚本
 * 数据源: http://www.henanhailong.com/newslist.asp?bid=1
 * 编码: GB2312
 */

const http = require('http');
const https = require('https');
const cheerio = require('cheerio');
const iconv = require('iconv-lite');
const fs = require('fs');
const path = require('path');

// 配置
const CONFIG = {
  baseUrl: 'http://www.henanhailong.com',
  listUrl: 'http://www.henanhailong.com/newslist.asp?bid=1',
  detailUrl: 'http://www.henanhailong.com/news.asp',
  encoding: 'gb2312',
  requestDelay: 200, // 请求间隔(ms)
  concurrency: 20, // 并发数
  maxRetries: 3,
  outputFile: path.join(__dirname, 'data', 'scraped-data.json'),
  // 也抓取中标公示和变更公告
  categories: [
    { bid: 1, name: '招标信息' },
    { bid: 26, name: '中标公示' },
    { bid: 27, name: '变更公告' }
  ]
};

/**
 * 发送HTTP请求
 */
function fetchUrl(url) {
  return new Promise((resolve, reject) => {
    const protocol = url.startsWith('https') ? https : http;
    const req = protocol.get(url, { timeout: 15000 }, (res) => {
      const chunks = [];
      res.on('data', chunk => chunks.push(chunk));
      res.on('end', () => {
        const buffer = Buffer.concat(chunks);
        const html = iconv.decode(buffer, CONFIG.encoding);
        resolve(html);
      });
    });
    req.on('error', reject);
    req.on('timeout', () => {
      req.destroy();
      reject(new Error(`Request timeout: ${url}`));
    });
  });
}

/**
 * 延迟
 */
function delay(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * 带重试的请求
 */
async function fetchWithRetry(url, retries = CONFIG.maxRetries) {
  for (let i = 0; i < retries; i++) {
    try {
      return await fetchUrl(url);
    } catch (err) {
      console.error(`  [重试 ${i + 1}/${retries}] ${url}: ${err.message}`);
      if (i === retries - 1) throw err;
      await delay(1000 * (i + 1));
    }
  }
}

/**
 * 解析列表页，提取条目列表
 */
function parseListPage(html) {
  const $ = cheerio.load(html);
  const items = [];

  // 查找列表中的链接
  $('a[href*="news.asp?id="]').each((i, el) => {
    const $a = $(el);
    const href = $a.attr('href');
    const title = $a.attr('title') || $a.text().trim();

    // 提取ID
    const idMatch = href.match(/id=(\d+)/);
    if (!idMatch) return;

    // 提取日期 - 在同一行的右侧td中
    const $row = $a.closest('tr');
    const dateText = $row.find('td:last font').text().trim();
    const dateMatch = dateText.match(/(\d{4}\/\d{1,2}\/\d{1,2})/);

    items.push({
      id: parseInt(idMatch[1]),
      title: title.replace(/^\s*·\s*/, '').trim(),
      date: dateMatch ? dateMatch[1] : dateText,
      url: `${CONFIG.baseUrl}/${href}`,
      sid: href.match(/sid=(\d+)/)?.[1] || '1',
      bid: href.match(/bid=(\d+)/)?.[1] || '1'
    });
  });

  return items;
}

/**
 * 获取列表总页数
 */
function parseTotalPages(html) {
  // 精确匹配 "页次：1/56" 或 "页次:1/56" 格式，避免误匹配日期
  const match = html.match(/页次[：:]\s*(\d+)\s*\/\s*(\d+)/);
  if (match) {
    return parseInt(match[2]);
  }
  // 备用：匹配 "页码:1101条记录" 格式计算总页数
  const countMatch = html.match(/(\d+)\s*条记录/);
  if (countMatch) {
    return Math.ceil(parseInt(countMatch[1]) / 20);
  }
  return 1;
}

/**
 * 解析详情页
 */
function parseDetailPage(html, item) {
  const $ = cheerio.load(html);
  const detail = { ...item };

  // 标题 - 在 <strong> 标签中
  const $title = $('td strong').first();
  detail.fullTitle = $title.text().trim() || item.title;

  // 发布信息 - "发布者：XXX  浏览次数：XXX  发布时间：XXX"
  const infoText = $('font').filter(function() {
    return $(this).text().includes('发布者');
  }).text();

  const publisherMatch = infoText.match(/发布者[：:]\s*(\S+)/);
  const viewCountMatch = infoText.match(/浏览次数[：:]\s*(\d+)/);
  const publishTimeMatch = infoText.match(/发布时间[：:]\s*([\d\/]+)/);

  detail.publisher = publisherMatch ? publisherMatch[1] : '';
  detail.viewCount = viewCountMatch ? parseInt(viewCountMatch[1]) : 0;
  detail.publishTime = publishTimeMatch ? publishTimeMatch[1] : item.date;

  // 正文HTML - 取完整公告内容（包含采购人信息、预算金额等）
  // 策略：先找StartFragment到EndFragment，如果内容太短则取更大的区域
  const bodyHtml = $.html();
  const startFrag = bodyHtml.indexOf('<!--StartFragment-->');
  const endFrag = bodyHtml.indexOf('<!--EndFragment-->');

  let contentHtml = '';
  if (startFrag !== -1 && endFrag !== -1) {
    contentHtml = bodyHtml.substring(startFrag + 20, endFrag).trim();
  }

  // 如果片段内容太短或缺少关键字段，取更大的内容区域
  if (contentHtml.length < 500 || !contentHtml.includes('采购人')) {
    // 取height=450或height=630的td内容（主内容区域）
    const $mainTable = $('table').filter(function() {
      const h = $(this).attr('height');
      return h === '450' || h === '630';
    }).first();
    if ($mainTable.length) {
      const fullHtml = $mainTable.html() || '';
      if (fullHtml.length > contentHtml.length) {
        contentHtml = fullHtml;
      }
    }
  }
  detail.contentHtml = contentHtml;

  // 从正文提取纯文本
  const $content = cheerio.load(detail.contentHtml);
  detail.contentText = $content.text().trim();

  // 从正文提取结构化字段
  extractStructuredFields(detail);

  return detail;
}

/**
 * 从正文中提取结构化字段
 */
function extractStructuredFields(detail) {
  const text = detail.contentText;

  // 项目编号
  const projectNoMatch = text.match(/(?:项目编号|交易项目编号|采购项目编号)[：:]\s*([^\n]+)/);
  detail.projectNumber = projectNoMatch ? projectNoMatch[1].trim() : '';

  // 预算金额
  const budgetMatch = text.match(/(?:预算金额|项目预算金额)[：:]\s*([\d,.]+)\s*(?:元|万元)/);
  if (budgetMatch) {
    const amount = parseFloat(budgetMatch[1].replace(/,/g, ''));
    // 判断是元还是万元
    const unitMatch = text.match(/预算金额[：:][\s\S]*?([\d,.]+)\s*(元|万元)/);
    detail.budgetAmount = amount;
    detail.budgetUnit = unitMatch ? unitMatch[2] : '元';
  } else {
    detail.budgetAmount = null;
    detail.budgetUnit = '';
  }

  // 最高控制价
  const maxPriceMatch = text.match(/最高控制价[：:]\s*([\d,.]+)\s*(?:元|万元)/);
  detail.maxControlPrice = maxPriceMatch ? parseFloat(maxPriceMatch[1].replace(/,/g, '')) : null;

  // 采购人/招标人
  const purchaserMatch = text.match(/(?:采购人|招标人)[：:]\s*([^\n,，。]+)/);
  detail.purchaser = purchaserMatch ? purchaserMatch[1].trim() : '';

  // 采购人地址
  const purchaserAddrMatch = text.match(/(?:采购人|招标人)[\s\S]*?联系地址[：:]\s*([^\n]+)/);
  detail.purchaserAddress = purchaserAddrMatch ? purchaserAddrMatch[1].trim() : '';

  // 采购人联系人
  const purchaserContactMatch = text.match(/(?:采购人|招标人)[\s\S]*?联系人[：:]\s*([^\n]+)/);
  detail.purchaserContact = purchaserContactMatch ? purchaserContactMatch[1].trim() : '';

  // 采购人电话
  const purchaserPhoneMatch = text.match(/(?:采购人|招标人)[\s\S]*?(?:联系电话|电话)[：:]\s*([\d\-]+)/);
  detail.purchaserPhone = purchaserPhoneMatch ? purchaserPhoneMatch[1].trim() : '';

  // 代理机构
  const agencyMatch = text.match(/(?:代理机构|集中采购机构)[：:]\s*([^\n,，。]+)/);
  detail.agency = agencyMatch ? agencyMatch[1].trim() : '';

  // 代理机构联系人
  const agencyContactMatch = text.match(/(?:代理机构|集中采购机构)[\s\S]*?联系人[：:]\s*([^\n]+)/);
  detail.agencyContact = agencyContactMatch ? agencyContactMatch[1].trim() : '';

  // 代理机构电话
  const agencyPhoneMatch = text.match(/(?:代理机构|集中采购机构)[\s\S]*?(?:联系电话|电话)[：:]\s*([\d\-]+)/);
  detail.agencyPhone = agencyPhoneMatch ? agencyPhoneMatch[1].trim() : '';

  // 项目地点
  const locationMatch = text.match(/项目地点[：:]\s*([^\n]+)/);
  detail.projectLocation = locationMatch ? locationMatch[1].trim() : '';

  // 工期
  const periodMatch = text.match(/工期[：:]\s*([^\n]+)/);
  detail.workPeriod = periodMatch ? periodMatch[1].trim() : '';

  // 质量要求
  const qualityMatch = text.match(/质量[：:]\s*([^\n]+)/);
  detail.qualityRequirement = qualityMatch ? qualityMatch[1].trim() : '';

  // 投标截止时间
  const deadlineMatch = text.match(/(?:投标截止时间|响应性文件递交截止时间|递交响应文件截止时间)[：:]\s*([\d\s年月日时分秒:：]+)/);
  detail.bidDeadline = deadlineMatch ? deadlineMatch[1].trim() : '';

  // 开标时间
  const openTimeMatch = text.match(/(?:开标时间|谈判时间)[：:]\s*([\d\s年月日时分秒:：]+)/);
  detail.bidOpenTime = openTimeMatch ? openTimeMatch[1].trim() : '';

  // 开标地点
  const openPlaceMatch = text.match(/(?:开标地点|谈判地点|递交及谈判地点)[：:]\s*([^\n]+)/);
  detail.bidOpenPlace = openPlaceMatch ? openPlaceMatch[1].trim() : '';

  // 招标文件售价
  const filePriceMatch = text.match(/(?:文件售价|招标文件售价|谈判文件售价)[：:]\s*([\d,.]+)\s*元/);
  detail.filePrice = filePriceMatch ? parseFloat(filePriceMatch[1]) : null;

  // 资金来源
  const fundMatch = text.match(/资金来源[：:]\s*([^\n]+)/);
  detail.fundSource = fundMatch ? fundMatch[1].trim() : '';

  // 招标方式（从标题推断）
  const title = detail.fullTitle || detail.title;
  if (title.includes('公开招标')) {
    detail.bidMethod = '公开招标';
  } else if (title.includes('竞争性谈判')) {
    detail.bidMethod = '竞争性谈判';
  } else if (title.includes('竞争性磋商')) {
    detail.bidMethod = '竞争性磋商';
  } else if (title.includes('询价')) {
    detail.bidMethod = '询价';
  } else if (title.includes('单一来源')) {
    detail.bidMethod = '单一来源';
  } else {
    detail.bidMethod = '';
  }
}

/**
 * 并发抓取详情页
 */
async function fetchDetailsConcurrent(items, category) {
  const details = [];
  let completed = 0;
  const total = items.length;

  // 分批并发
  for (let i = 0; i < items.length; i += CONFIG.concurrency) {
    const batch = items.slice(i, i + CONFIG.concurrency);
    const promises = batch.map(async (item) => {
      try {
        const detailHtml = await fetchWithRetry(item.url);
        const detail = parseDetailPage(detailHtml, item);
        detail.category = category.name;
        detail.categoryBid = category.bid;
        completed++;
        process.stdout.write(`\r  进度: ${completed}/${total} (${(completed/total*100).toFixed(1)}%)`);
        return detail;
      } catch (err) {
        completed++;
        process.stdout.write(`\r  进度: ${completed}/${total} (${(completed/total*100).toFixed(1)}%)`);
        return {
          ...item,
          category: category.name,
          categoryBid: category.bid,
          error: err.message
        };
      }
    });

    const batchResults = await Promise.all(promises);
    details.push(...batchResults);

    // 批次间短暂延迟，避免被封
    if (i + CONFIG.concurrency < items.length) {
      await delay(CONFIG.requestDelay);
    }
  }
  console.log(''); // 换行
  return details;
}

/**
 * 抓取指定分类的所有数据
 */
async function scrapeCategory(category) {
  console.log(`\n========== 抓取: ${category.name} (bid=${category.bid}) ==========`);

  const allItems = [];

  // 1. 获取第一页，确定总页数
  const firstPageUrl = `${CONFIG.baseUrl}/newslist.asp?bid=${category.bid}`;
  console.log(`获取列表首页: ${firstPageUrl}`);
  const firstPageHtml = await fetchWithRetry(firstPageUrl);
  const totalPages = parseTotalPages(firstPageHtml);
  console.log(`总页数: ${totalPages}`);

  // 2. 解析第一页
  const firstPageItems = parseListPage(firstPageHtml);
  console.log(`第1页: ${firstPageItems.length} 条`);
  allItems.push(...firstPageItems);

  // 3. 并发获取剩余页面
  const pagePromises = [];
  for (let page = 2; page <= totalPages; page++) {
    pagePromises.push(
      (async () => {
        const pageUrl = `${CONFIG.baseUrl}/newslist.asp?bid=${category.bid}&Page=${page}`;
        const pageHtml = await fetchWithRetry(pageUrl);
        return parseListPage(pageHtml);
      })()
    );
  }
  const pageResults = await Promise.all(pagePromises);
  pageResults.forEach(items => allItems.push(...items));
  console.log(`列表总计: ${allItems.length} 条`);

  // 4. 并发获取详情
  console.log(`开始并发抓取详情 (并发数: ${CONFIG.concurrency})...`);
  const details = await fetchDetailsConcurrent(allItems, category);

  return details;
}

/**
 * 主函数
 */
async function main() {
  console.log('========================================');
  console.log('河南海隆旧网站数据抓取工具');
  console.log('========================================');
  console.log(`目标: ${CONFIG.baseUrl}`);
  console.log(`输出: ${CONFIG.outputFile}`);
  console.log('');

  const allData = [];

  for (const category of CONFIG.categories) {
    try {
      const data = await scrapeCategory(category);
      allData.push(...data);
    } catch (err) {
      console.error(`分类 ${category.name} 抓取失败: ${err.message}`);
    }
  }

  // 保存结果
  fs.mkdirSync(path.dirname(CONFIG.outputFile), { recursive: true });
  fs.writeFileSync(CONFIG.outputFile, JSON.stringify(allData, null, 2), 'utf-8');

  console.log('\n========================================');
  console.log('抓取完成!');
  console.log(`总计: ${allData.length} 条`);
  console.log(`输出: ${CONFIG.outputFile}`);

  // 统计
  const stats = {
    total: allData.length,
    withError: allData.filter(d => d.error).length,
    byCategory: {}
  };
  for (const cat of CONFIG.categories) {
    stats.byCategory[cat.name] = allData.filter(d => d.categoryBid === cat.bid).length;
  }
  console.log('统计:', JSON.stringify(stats, null, 2));
}

main().catch(console.error);

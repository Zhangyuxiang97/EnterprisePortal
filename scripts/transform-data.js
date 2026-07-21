/**
 * 数据转换脚本
 * 将抓取的原始数据转换为新系统 Announcement 格式
 */

const fs = require('fs');
const path = require('path');
const { findAnnouncementRegionOverride } = require('./announcement-region-overrides');

const INPUT_FILE = path.join(__dirname, 'data', 'scraped-data.json');
const OUTPUT_FILE = path.join(__dirname, 'data', 'transformed-data.json');
const REPORT_FILE = path.join(__dirname, 'data', 'transform-report.json');

/**
 * 从标题推断 notice_type
 */
function inferNoticeType(title, categoryBid) {
  if (categoryBid === 26 || title.includes('中标') || title.includes('成交')) {
    return 'result';
  }
  if (categoryBid === 27 || title.includes('变更') || title.includes('更正') || title.includes('澄清')) {
    return 'correction';
  }
  return 'bidding';
}

/**
 * 从标题和内容推断 business_type
 */
function inferBusinessType(title, contentText) {
  const text = title + ' ' + (contentText || '').substring(0, 500);

  // 建设工程特征
  const constructionPatterns = [
    /施工(?:招标|公告|采购)/,
    /建设工程/,
    /道路工程/,
    /桥梁工程/,
    /水利工程/,
    /建筑工程/,
    /装修工程/,
    /改造工程/,
    /管网工程/,
    /绿化工程/,
    /硬化工程/,
    /EPC总承包/,
    /施工标段/,
    /施工一标段|施工二标段|施工三标段/,
    /监理(?:招标|公告|采购)/,
    /勘察(?:招标|公告|采购)/,
    /设计(?:招标|公告|采购)/,
    /造价咨询(?:招标|公告|采购)/,
    /全过程咨询(?:招标|公告|采购)/,
  ];

  // 政府采购特征
  const procurementPatterns = [
    /采购(?:项目|公告|招标)/,
    /服务(?:项目|公告|招标)/,
    /货物(?:项目|公告|招标)/,
    /设备(?:采购|招标)/,
    /物资(?:采购|招标)/,
    /耗材(?:采购|招标)/,
    /办公用品/,
    /物业服务/,
    /保安服务/,
    /保洁服务/,
    /餐饮服务/,
    /维修服务/,
    /维保服务/,
    /检测服务/,
    /咨询服务/,
    /培训服务/,
    /保险服务/,
    /租赁(?:项目|招标)/,
  ];

  // 先检查建设工程
  for (const pattern of constructionPatterns) {
    if (pattern.test(text)) return 'CONSTRUCTION';
  }

  // 再检查政府采购
  for (const pattern of procurementPatterns) {
    if (pattern.test(text)) return 'GOV_PROCUREMENT';
  }

  // 默认政府采购
  return 'GOV_PROCUREMENT';
}

/**
 * 从标题推断 procurement_type
 */
function inferProcurementType(title, contentText) {
  const text = title + ' ' + (contentText || '');
  // 货物类
  if (text.match(/设备|采购|货物|产品|器具|器材|仪器/)) {
    return 'goods';
  }
  // 服务类
  if (text.match(/监理|咨询|服务|勘察|设计|审计|评估|检测|维保|代理/)) {
    return 'service';
  }
  // 工程类
  if (text.match(/施工|装修|改造|维修|建设|工程|道路|桥梁|管网/)) {
    return 'project';
  }
  return null;
}

/**
 * 从标题和正文提取地区信息
 */
function extractRegion(title, contentText, projectLocation) {
  const regionOverride = findAnnouncementRegionOverride(title);
  if (regionOverride) {
    return {
      province: regionOverride.province.replace(/省$|市$|自治区$/, ''),
      city: regionOverride.city,
      district: regionOverride.district
    };
  }

  // 只从标题、结构化项目地点和正文中的“项目/建设地点”片段识别区域。
  // 不能扫描整篇正文，否则代理机构地址、供应商地址和候选人注册地址会污染项目区域。
  const locationParts = [];
  const locationPattern = /(?:项目建设地点|建设地点|项目地点|项目地址|项目所在地|供货地点)\s*[：:]?\s*([^。；;\n]{0,120})/g;
  let locationMatch;
  while ((locationMatch = locationPattern.exec(contentText || '')) !== null && locationParts.length < 5) {
    locationParts.push(locationMatch[1]);
  }

  const locationText = [title, projectLocation, ...locationParts].filter(Boolean).join(' ');

  // 旧站主体数据默认属于河南；已核实的跨省项目已在上方按项目系列覆盖。
  const province = '河南';

  // 城市
  const cityMap = {
    '郑州': '郑州市', '商丘': '商丘市', '开封': '开封市',
    '洛阳': '洛阳市', '南阳': '南阳市', '新乡': '新乡市',
    '信阳': '信阳市', '周口': '周口市', '驻马店': '驻马店市',
    '安阳': '安阳市', '焦作': '焦作市', '濮阳': '濮阳市',
    '鹤壁': '鹤壁市', '漯河': '漯河市', '许昌': '许昌市',
    '三门峡': '三门峡市', '平顶山': '平顶山市', '济源': '济源市',
    '封丘': '新乡市', '中牟': '郑州市', '巩义': '郑州市',
    '荥阳': '郑州市', '新密': '郑州市', '登封': '郑州市',
    '新郑': '郑州市'
  };

  let city = '';
  let district = '';

  for (const [key, value] of Object.entries(cityMap)) {
    if (locationText.includes(key)) {
      city = value;
      break;
    }
  }

  // 区县逻辑：县单独标记，其他（区、开发区、功能区等）统一标"市区"
  // 按标题、结构化项目地点、正文地点片段的优先级提取，避免取到正文末尾的企业注册地址。
  const countySources = [title, projectLocation, ...locationParts].filter(Boolean);
  const countyMatches = countySources
    .map(source => source.match(/[一-龥]{2,}县/g))
    .find(matches => matches && matches.length > 0);
  if (countyMatches && countyMatches.length > 0) {
    // 同一地点片段有多个县名时取最后一个，通常是最具体的项目所在地。
    district = countyMatches[countyMatches.length - 1];
  } else if (city) {
    // 有城市但没有县，就是市区
    district = '市区';
  }

  return { province, city, district };
}

/**
 * 解析金额（元转万元）
 */
function parseBudgetAmount(amount, unit) {
  if (!amount) return null;
  if (unit === '万元') return amount;
  return Math.round(amount / 10000 * 100) / 100; // 保留2位小数
}

/**
 * 解析日期字符串
 */
function parseDate(dateStr) {
  if (!dateStr) return null;
  // 处理 "2026/7/15" 格式
  const match = dateStr.match(/(\d{4})[\/\-](\d{1,2})[\/\-](\d{1,2})/);
  if (match) {
    return `${match[1]}-${match[2].padStart(2, '0')}-${match[3].padStart(2, '0')}T00:00:00`;
  }
  // 处理 "2026年7月15日" 格式
  const match2 = dateStr.match(/(\d{4})\s*年\s*(\d{1,2})\s*月\s*(\d{1,2})\s*日/);
  if (match2) {
    return `${match2[1]}-${match2[2].padStart(2, '0')}-${match2[3].padStart(2, '0')}T00:00:00`;
  }
  return null;
}

/**
 * 清理HTML内容
 */
function cleanContent(html) {
  if (!html) return '';
  // 只移除Word特有的标签，保留内容
  let cleaned = html
    .replace(/<style[\s\S]*?<\/style>/gi, '')
    .replace(/<!--\[if[\s\S]*?endif\]-->/gi, '')
    .replace(/<o:p[\s\S]*?<\/o:p>/gi, '')
    .replace(/<w:[^>]+>[\s\S]*?<\/w:[^>]+>/gi, '')
    // 保留内容，只清理多余空格
    .replace(/\s{2,}/g, ' ')
    .trim();
  return cleaned;
}

/**
 * 从正文中提取金额（支持多种模式）
 */
function extractBudgetFromText(text, noticeType) {
  if (!text) return null;

  // 统一冒号（全角→半角）
  const t = text.replace(/：/g, ':').replace(/（/g, '(').replace(/）/g, ')');

  // 招标类：优先取预算金额、控制价
  const bidPatterns = [
    /预算金额[:]\s*([\d,.]+)\s*(万元|元)/,
    /项目预算[:]\s*([\d,.]+)\s*(万元|元)/,
    /采购预算[:]\s*([\d,.]+)\s*(万元|元)/,
    /招标控制价[:]\s*([\d,.]+)\s*(万元|元)/,
    /最高限价[:]\s*([\d,.]+)\s*(万元|元)/,
    /最高控制价[:]\s*([\d,.]+)\s*(万元|元)/,
    /项目总?投资[:]\s*约?\s*([\d,.]+)\s*(万元|元)/,
    /项目概算[:]\s*([\d,.]+)\s*(万元|元)/,
    /总\s*投资[:]\s*约?\s*([\d,.]+)\s*(万元|元)/,
    /本次招标\s*([\d,.]+)\s*元/,
    /预算金额[:]\s*([\d,.]+)\s*元/,
  ];

  // 中标/成交类：取中标金额、成交价
  const resultPatterns = [
    /中标金额[:]\s*([\d,.]+)\s*(万元|元)/,
    /成交金额[:]\s*([\d,.]+)\s*(万元|元)/,
    /中标价[:]\s*([\d,.]+)\s*(万元|元)/,
    /成交价[:]\s*([\d,.]+)\s*(万元|元)/,
    /报价[:]\s*([\d,.]+)\s*(万元|元)/,
    /合同金额[:]\s*([\d,.]+)\s*(万元|元)/,
  ];

  const patterns = noticeType === 'result' ? [...resultPatterns, ...bidPatterns] : [...bidPatterns, ...resultPatterns];

  for (const pattern of patterns) {
    const match = t.match(pattern);
    if (match) {
      const amount = parseFloat(match[1].replace(/,/g, ''));
      // 如果没有万元/元单位，判断大小
      if (match[2]) {
        return parseBudgetAmount(amount, match[2]);
      } else {
        // "本次招标 228665859.89元" 这种没有单位的
        return parseBudgetAmount(amount, '元');
      }
    }
  }
  return null;
}

/**
 * 从中标公示中提取中标人
 */
function extractWinner(text) {
  if (!text) return null;

  // 统一冒号和空格
  const t = text.replace(/：/g, ':').replace(/\r/g, '');

  // 带冒号的格式
  const colonPatterns = [
    /中标人[:]\s*([^\s,，。；;、\n]{2,50})/,
    /中标单位[:]\s*([^\s,，。；;、\n]{2,50})/,
    /中标供应商名称?[:]\s*([^\s,，。；;、\n]{2,50})/,
    /成交供应商名称?[:]\s*([^\s,，。；;、\n]{2,50})/,
    /成交人[:]\s*([^\s,，。；;、\n]{2,50})/,
    /成交单位[:]\s*([^\s,，。；;、\n]{2,50})/,
    /第一中标候选人[:]\s*([^\s,，。；;、\n]{2,50})/,
    /第一成交候选人[:]\s*([^\s,，。；;、\n]{2,50})/,
    /中标候选人第一名[:]\s*([^\s,，。；;、\n]{2,50})/,
  ];

  for (const pattern of colonPatterns) {
    const match = t.match(pattern);
    if (match) {
      let winner = match[1].trim().replace(/[,，。；;、].*$/, '');
      if (winner.length >= 2 && winner.length <= 50) return winner;
    }
  }

  // 表格格式：中标供应商名称 后面几行是公司名
  const lines = text.split(/[\n]+/).map(l => l.trim()).filter(l => l);
  const headerIdx = lines.findIndex(l => l.includes('中标供应商名称') || l.includes('成交供应商名称'));
  if (headerIdx !== -1) {
    // 往后找公司名（跳过其他表头列）
    for (let i = headerIdx + 1; i < Math.min(headerIdx + 15, lines.length); i++) {
      const line = lines[i];
      // 匹配公司名格式
      const companyMatch = line.match(/^([一-龥]{4,}(?:有限公司|集团|股份|研究院|学院|大学|医院|中心|公司))/);
      if (companyMatch) {
        return companyMatch[1];
      }
    }
  }

  return null;
}

/**
 * 从正文中提取截止时间
 */
function extractDeadline(text) {
  if (!text) return null;

  // 日期正则（宽松匹配）
  const dateRe = /(\d{4})\s*年?\s*(\d{1,2})\s*月?\s*(\d{1,2})\s*日?\s*(\d{1,2})\s*[时:：]\s*(\d{1,2})\s*分?/;

  function parseDateMatch(m) {
    if (!m) return null;
    const y = m[1], mo = m[2].padStart(2, '0'), d = m[3].padStart(2, '0');
    const h = (m[4] || '0').padStart(2, '0'), mi = (m[5] || '0').padStart(2, '0');
    return `${y}-${mo}-${d}T${h}:${mi}:00`;
  }

  // 模式1: "投标文件递交截止时间及开标时间2026年7月27日15时30分"（无冒号）
  const noColonPatterns = [
    /(?:投标|响应性?文件)?(?:递交)?截止时间[^。；\n]*?(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)/,
    /于\s*(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)[^。]*前递/,
    /截止[^。；\n]*?(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)/,
  ];

  for (const pattern of noColonPatterns) {
    const match = text.match(pattern);
    if (match) {
      const result = parseDateMatch(dateRe.exec(match[1] ? match[0] : match[0]));
      if (result) return result;
      // 直接尝试从match[1]解析
      const m2 = (match[1] || match[0]).match(dateRe);
      if (m2) return parseDateMatch(m2);
    }
  }

  // 模式2: 有冒号的格式
  const colonPatterns = [
    /投标截止时间[：:]\s*(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)/,
    /响应性?文件递交截止时间[：:]\s*(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)/,
    /递交截止时间[：:]\s*(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)/,
    /截止时间[：:]\s*(\d{4}\s*年?\s*\d{1,2}\s*月?\s*\d{1,2}\s*日?\s*\d{1,2}\s*[时:：]\s*\d{1,2}\s*分?)/,
  ];

  for (const pattern of colonPatterns) {
    const match = text.match(pattern);
    if (match) return parseDateMatch(dateRe.exec(match[1]));
  }

  return null;
}

/**
 * 从标题中推理招标人（最后手段）
 */
function inferBidderFromTitle(title) {
  if (!title) return '';
  const patterns = [
    /^([一-龥]{2,}(?:市|县|区|镇)[一-龥]{2,}(?:局|委|办|中心|学校|医院|集团|公司))/,
    /^((?:河南|郑州|开封|洛阳|南阳|新乡|信阳|周口|驻马店|安阳|焦作|濮阳|鹤壁|漯河|许昌|三门峡|平顶山|济源)[一-龥]*(?:大学|学院|医院|研究院|实验室|集团|公司|学校|中心|局|委))/,
    /([一-龥]{2,}(?:大学|学院|医院|研究院|实验室))/,
  ];
  for (const pattern of patterns) {
    const match = title.match(pattern);
    if (match && match[1].length >= 4) return match[1];
  }
  return '';
}

/**
 * 从正文中提取招标人/采购人（增强版）
 */
function extractBidder(text, rawBidder, title) {
  if (rawBidder) return rawBidder;
  if (!text) return inferBidderFromTitle(title);

  // 统一冒号和空格
  const t = text.replace(/：/g, ':').replace(/\s+/g, ' ');

  // 模式1: "采购人信息" 或 "招标人信息" 或 "八、采购人:" 后面的 "名 称:XXX"
  // 先尝试带"名称:"的格式
  const infoPattern1 = /(?:采购人|招标人|建设单位)(?:信息|:)[\s\S]*?名\s*称[:]\s*([^\s,，。；;、\n]{2,50}?)(?:\s*(?:地址|联系人|电话|邮编|网址)[:]|$)/;
  const infoMatch1 = t.match(infoPattern1);
  if (infoMatch1) {
    let bidder = infoMatch1[1].trim().replace(/[,，。；;、].*$/, '');
    if (bidder.length >= 2 && bidder.length <= 50 && !bidder.includes('海隆')) return bidder;
  }

  // 模式1b: "采购人:" 或 "招标人:" 直接跟公司名（没有"名称:"）
  const infoPattern1b = /(?:八、|九、|1、|2、)?(?:采购人|招标人|建设单位)[:]\s*([一-龥]{4,}(?:公司|集团|医院|学校|中心|局|委|研究院|学院))/;
  const infoMatch1b = t.match(infoPattern1b);
  if (infoMatch1b) {
    let bidder = infoMatch1b[1].trim();
    if (bidder.length >= 2 && bidder.length <= 50 && !bidder.includes('海隆')) return bidder;
  }

  // 模式1c: 从"采购人"或"招标人"后面的"名称:"提取，精确匹配公司名格式
  const infoPattern1c = /(?:采购人|招标人|建设单位)[^一-龥]*?名\s*称[:]\s*([一-龥]{2,20}(?:有限公司|集团|股份|医院|学校|中心|局|委|研究院|学院))/;
  const infoMatch1c = t.match(infoPattern1c);
  if (infoMatch1c) {
    let bidder = infoMatch1c[1].trim();
    if (bidder.length >= 2 && bidder.length <= 50 && !bidder.includes('海隆')) return bidder;
  }

  // 模式2: "招标人名称:XXX" / "招标人（异议受理单位）:XXX" / "招 标 人:XXX"（允许空格）
  const spacedPatterns = [
    /招\s*标\s*人\s*名\s*称[:]\s*([^\s,，。；;、\n]{2,50})/,
    /招\s*标\s*人\s*[(（]异议受理单位[)）][:]\s*([^\s,，。；;、\n]{2,50})/,
    /采\s*购\s*人\s*名\s*称[:]\s*([^\s,，。；;、\n]{2,50})/,
    /招\s*标\s*人[:\s]\s*([^\s,，。；;、\n]{2,50})/,
    /采\s*购\s*人[:\s]\s*([^\s,，。；;、\n]{2,50})/,
    /建\s*设\s*单\s*位[:]\s*([^\s,，。；;、\n]{2,50})/,
  ];
  for (const pattern of spacedPatterns) {
    const match = t.match(pattern);
    if (match) {
      let bidder = match[1].trim().replace(/[,，。；;、].*$/, '');
      if (bidder.length >= 2 && bidder.length <= 50 && !bidder.includes('海隆')) return bidder;
    }
  }

  // 模式3: "招标人为XXX"（正文中的叙述）
  const narrativePattern = /招标人为\s*([^\s,，。；;、\n]{2,50}?)(?:\s|，|,|。|；|;)/;
  const narrativeMatch = t.match(narrativePattern);
  if (narrativeMatch) {
    let bidder = narrativeMatch[1].trim();
    if (bidder.length >= 2 && bidder.length <= 50 && !bidder.includes('海隆')) return bidder;
  }

  // 模式4: 从标题推理
  return inferBidderFromTitle(title);
}

/**
 * 转换单条记录（增强版）
 */
function transformItem(raw) {
  const title = raw.fullTitle || raw.title;
  const contentText = raw.contentText || '';
  const noticeType = inferNoticeType(title, raw.categoryBid);

  // 字段映射状态追踪
  const fieldStatus = {};

  // 增强提取
  const bidder = extractBidder(contentText, raw.purchaser, title);
  const budgetAmount = parseBudgetAmount(raw.budgetAmount, raw.budgetUnit) || extractBudgetFromText(contentText, noticeType);
  const deadline = raw.bidDeadline ? parseDate(raw.bidDeadline) : extractDeadline(contentText);
  const winner = noticeType === 'result' ? extractWinner(contentText) : null;

  // 直接映射字段
  const result = {
    // 直接映射 ✅
    title: title,
    content: cleanContent(raw.contentHtml),
    publisher: raw.publisher || '',
    publish_time: parseDate(raw.publishTime),
    view_count: raw.viewCount || 0,

    // 推理字段 ⚠️
    business_type: inferBusinessType(title, contentText),
    notice_type: noticeType,
    procurement_type: inferProcurementType(title, contentText),
    bidder: bidder,
    budget_amount: budgetAmount,
    deadline: deadline,

    // 地区推理
    ...extractRegion(title, contentText, raw.projectLocation),
    project_region: '',

    // 中标人（仅中标公示）
    winner: winner,

    // 缺失字段 ❌
    attachment_ids: null,

    // 默认值
    is_top: 0,
    status: 1,
    is_deleted: 0,

    // 原始数据保留（用于预览对比）
    _raw: {
      id: raw.id,
      url: raw.url,
      category: raw.category,
      projectNumber: raw.projectNumber,
      maxControlPrice: raw.maxControlPrice,
      purchaser: raw.purchaser,
      purchaserAddress: raw.purchaserAddress,
      purchaserContact: raw.purchaserContact,
      purchaserPhone: raw.purchaserPhone,
      agency: raw.agency,
      agencyContact: raw.agencyContact,
      agencyPhone: raw.agencyPhone,
      projectLocation: raw.projectLocation,
      workPeriod: raw.workPeriod,
      qualityRequirement: raw.qualityRequirement,
      bidDeadline: raw.bidDeadline,
      bidOpenTime: raw.bidOpenTime,
      bidOpenPlace: raw.bidOpenPlace,
      filePrice: raw.filePrice,
      fundSource: raw.fundSource,
      bidMethod: raw.bidMethod,
      error: raw.error
    }
  };

  // 拼接 project_region
  const regionParts = [result.province, result.city, result.district].filter(Boolean);
  result.project_region = regionParts.join(' ');

  // 字段状态标记
  fieldStatus.title = 'direct';
  fieldStatus.content = 'direct';
  fieldStatus.publisher = raw.publisher ? 'direct' : 'missing';
  fieldStatus.publish_time = result.publish_time ? 'direct' : 'missing';
  fieldStatus.view_count = 'direct';
  fieldStatus.business_type = 'inferred';
  fieldStatus.notice_type = 'inferred';
  fieldStatus.procurement_type = result.procurement_type ? 'inferred' : 'missing';
  fieldStatus.bidder = result.bidder ? 'inferred' : 'missing';
  fieldStatus.budget_amount = result.budget_amount ? 'inferred' : 'missing';
  fieldStatus.deadline = result.deadline ? 'inferred' : 'missing';
  fieldStatus.province = 'inferred';
  fieldStatus.city = result.city ? 'inferred' : 'missing';
  fieldStatus.district = result.district ? 'inferred' : 'missing';
  fieldStatus.project_region = result.project_region ? 'inferred' : 'missing';
  fieldStatus.winner = result.winner ? 'inferred' : (noticeType === 'result' ? 'missing' : 'inferred');
  fieldStatus.attachment_ids = 'inferred'; // 不考虑附件缺失
  fieldStatus.is_top = 'inferred';
  fieldStatus.status = 'inferred';

  result._fieldStatus = fieldStatus;

  return result;
}

/**
 * 生成转换报告
 */
function generateReport(transformed, rawData) {
  const report = {
    summary: {
      total: transformed.length,
      successful: transformed.filter(t => !t._raw.error).length,
      failed: transformed.filter(t => t._raw.error).length,
      byCategory: {},
      byNoticeType: {}
    },
    fieldCoverage: {},
    sampleData: transformed.slice(0, 5).map(t => ({
      title: t.title,
      notice_type: t.notice_type,
      budget_amount: t.budget_amount,
      bidder: t.bidder,
      city: t.city,
      fieldStatus: t._fieldStatus
    }))
  };

  // 按分类统计
  for (const item of transformed) {
    const cat = item._raw.category;
    report.summary.byCategory[cat] = (report.summary.byCategory[cat] || 0) + 1;
    report.summary.byNoticeType[item.notice_type] = (report.summary.byNoticeType[item.notice_type] || 0) + 1;
  }

  // 字段覆盖率
  const fields = ['title', 'content', 'publisher', 'publish_time', 'view_count',
    'business_type', 'notice_type', 'procurement_type', 'bidder', 'budget_amount',
    'deadline', 'province', 'city', 'district', 'project_region', 'winner', 'attachment_ids'];

  for (const field of fields) {
    const statuses = transformed.map(t => t._fieldStatus[field]);
    report.fieldCoverage[field] = {
      direct: statuses.filter(s => s === 'direct').length,
      inferred: statuses.filter(s => s === 'inferred').length,
      default: statuses.filter(s => s === 'default').length,
      missing: statuses.filter(s => s === 'missing').length,
      coverage: Math.round((statuses.filter(s => s !== 'missing').length / statuses.length) * 100) + '%'
    };
  }

  return report;
}

/**
 * 主函数
 */
function main() {
  console.log('========================================');
  console.log('数据转换工具');
  console.log('========================================');

  // 读取原始数据
  if (!fs.existsSync(INPUT_FILE)) {
    console.error(`输入文件不存在: ${INPUT_FILE}`);
    console.error('请先运行 scrape-hailong.js');
    process.exit(1);
  }

  const rawData = JSON.parse(fs.readFileSync(INPUT_FILE, 'utf-8'));
  console.log(`读取原始数据: ${rawData.length} 条`);

  // 转换
  const transformed = rawData
    .filter(item => !item.error) // 过滤失败的
    .map(transformItem);

  console.log(`转换成功: ${transformed.length} 条`);

  // 生成报告
  const report = generateReport(transformed, rawData);

  // 保存结果
  fs.writeFileSync(OUTPUT_FILE, JSON.stringify(transformed, null, 2), 'utf-8');
  fs.writeFileSync(REPORT_FILE, JSON.stringify(report, null, 2), 'utf-8');

  console.log(`\n输出文件:`);
  console.log(`  转换数据: ${OUTPUT_FILE}`);
  console.log(`  转换报告: ${REPORT_FILE}`);

  // 打印摘要
  console.log('\n---------- 转换摘要 ----------');
  console.log(`总计: ${report.summary.total} 条`);
  console.log(`成功: ${report.summary.successful} 条`);
  console.log(`失败: ${report.summary.failed} 条`);

  console.log('\n按分类:');
  for (const [cat, count] of Object.entries(report.summary.byCategory)) {
    console.log(`  ${cat}: ${count} 条`);
  }

  console.log('\n按公告类型:');
  for (const [type, count] of Object.entries(report.summary.byNoticeType)) {
    console.log(`  ${type}: ${count} 条`);
  }

  console.log('\n字段覆盖率:');
  for (const [field, coverage] of Object.entries(report.fieldCoverage)) {
    console.log(`  ${field}: ${coverage.coverage} (直接:${coverage.direct} 推理:${coverage.inferred} 缺失:${coverage.missing})`);
  }
}

if (require.main === module) {
  main();
}

module.exports = { extractRegion };

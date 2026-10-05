/** 按标题与正文复核初始化字段；每个结论带证据，缺失/冲突不猜数值。 */
const cheerio = require('cheerio');
const { findAnnouncementRegionOverride } = require('./announcement-region-overrides');
const {awardScopes,subjects,incompleteAmounts,inferredSubjects}=require('./confirmed-announcement-reviews');

function extractDocument(html) {
  const $ = cheerio.load(html || '');
  $('script,style,iframe,object,embed').remove();
  const cellText = el => $(el).text().replace(/[\u00a0\u3000]/g, ' ').replace(/\s+/g, ' ').trim();
  const tables = $('table').toArray().map(el => $(el).find('tr').toArray().filter(tr => $(tr).closest('table')[0] === el)
    .map(tr => $(tr).children('td,th').toArray().map(cellText))).filter(t => t.length);
  $('br').replaceWith('\n');
  $('td,th').append('\t');
  $('p,div,li,h1,h2,h3,h4,h5,h6,tr').append('\n');
  const text = $('body').text().replace(/\r/g, '').replace(/[\u00a0\u3000]/g, ' ')
    .replace(/[ \t]+/g, ' ').replace(/ *\n */g, '\n').replace(/\n{2,}/g, '\n').trim();
  return { text, tables };
}

function noticeType(title, oldCategory) {
  if (/更正|变更|澄清|延期|延迟|恢复|暂停|补充公告/.test(title)) return { value: 'correction', rule: '标题更正或变更', evidence: title };
  if (/专家论证|单一来源采购公示|招标文件公示/.test(title)) return { value:'bidding',rule:'采购前公示',evidence:title };
  if (/中标|成交|结果|候选人|流标|废标|终止|失败/.test(title)) return { value: 'result', rule: '标题结果或终止', evidence: title };
  if (/招标|采购|谈判|磋商|询价|征集|遴选|比选|资格预审/.test(title)) return { value: 'bidding', rule: '标题招标采购', evidence: title };
  return { value: oldCategory === 27 ? 'correction' : oldCategory === 26 ? 'result' : 'bidding', rule: '旧站栏目', evidence: title };
}

const SERVICE = /托管|劳务|物业|维保|运维|保洁|保安|洗涤|印刷|维修保养|设备维修|硬盘维修|外检|保险|监理|勘察|设计(?:服务|项目|招标|评审)|造价|咨询|审计|检测(?:服务|抽查|项目)|运营|租赁|租用|餐饮服务|驾驶服务|保养|软件(?:开发|建设|服务)|信息化(?:服务|运维|建设)|污泥处置|编制.{0,12}报告|评估|评价报告|抽检|报废处置|垃圾清运|草坪修剪|标准优化|智能化升级|检测抽查|飞防/;
const GOODS = /设备(?:采购|购置|更新|遴选)|(?:采购|购置).{0,12}(?:设备|客车|车辆|仪器|器材|物资|材料|图书|服装)|医疗设备|教学设备|办证设备|机电设备|药品|耗材|物资|食材|原料|滤饼|粮油|慰问品|复印机(?:购置|采购)|打印机|电子屏|计算机|柴油|加油|营运客车|配件|钢材|家具|有机肥|彩色多普勒|光学|CT机|磁共振|电缆|吊具|抓斗|试卷|材料|制剂|校服|纸张|油墨|显微镜|检测仪|生物安全柜|灭火器/;
const PROJECT = /施工|EPC|总承包|改造|装修|修缮|加固|道路|管网|桥梁|防水|工程|绿化|硬化|校舍|教学楼|地坪|整修|维修|刷漆|工程量清单|施工图|围挡搭建|街面提升|恢复重建|塑钢窗更换/;
const GENERAL_SERVICE = /服务|体检|鉴定|检验|培训|广告|宣传|普查|回收处置|软件|云胶片|智慧课程|智慧管理|项目建议书|可研报告|设计|决算.{0,8}(?:编制|审核)|计划编制|方案编制|规划(?:编制|项目|竞争)|综合提升规划|测量|测绘|测评|登记提质|云存储.{0,8}(?:升级|扩容)|系统(?:升级|开发|运行维护)|申报系统|经营权|招租|耕地轮作|一喷三防|项目区位调整|遥感|住院救助|委托加工|外包|演练|网络维护|高素质农民培育|上图入库|承保企业|融资主体|抽查检查/;
const EXTRA_GOODS = /设备|电梯|储存柜|储物柜|直饮机|护士鞋|劳保用品|桌椅|屏蔽器|客车|防磨垫|档木|方木|电刀|治疗机|吸脂机|营销品|照明灯|校园监控|录播室|课桌|空调|锅炉|电器|服务器|终端|电池|门禁|电动自行车|放射源|制备仪|商砼|采暖设施|用品|种子|肥料|专用肥|药剂|呼吸机|窗帘|防尘布|服装|西服|衬衣|黑板|钢琴|胃镜|放大镜|诊断仪|治疗仪|螺旋CT|透析机|开水器|注射器|变压器|发电机组|烧结炉|伸缩门|电源|除冰车|果苗|鱼苗|苗木|天然气/;

function classify(title, text) {
  const demands = [...text.matchAll(/(?:采购内容|采购需求|服务内容|服务范围|招标范围|采购范围(?:及内容)?|招标内容|施工内容|建设内容|项目内容|拟提供货物或服务项目基本情况)(?:[（(][^）)]{0,70}[）)])?\s*[:：]?\s*([^。；]{0,360})/g)].map(m=>m[1]);
  const demand = demands.find(s=>SERVICE.test(s)||GOODS.test(s)||PROJECT.test(s)||GENERAL_SERVICE.test(s)||EXTRA_GOODS.test(s)) || '';
  let kind, kindEvidence, kindRule;
  if (SERVICE.test(title)) { kind = 'service'; kindEvidence = title; kindRule = '标题服务标的'; }
  else if (GOODS.test(title) || EXTRA_GOODS.test(title) || /(?:设备|图书|车辆|器材|仪器|产品).{0,8}(?:采购|购置|供应商)/.test(title)) { kind = 'goods'; kindEvidence = title; kindRule = '标题货物标的'; }
  else if (PROJECT.test(title)) { kind = 'project'; kindEvidence = title; kindRule = '标题工程标的'; }
  else if (GENERAL_SERVICE.test(title)) { kind = 'service'; kindEvidence = title; kindRule = '标题服务业务'; }
  else if (SERVICE.test(demand)) { kind = 'service'; kindEvidence = demand; kindRule = '采购需求服务标的'; }
  else if (GOODS.test(demand) || /货物/.test(demand)) { kind = 'goods'; kindEvidence = demand; kindRule = '采购需求货物标的'; }
  else if (PROJECT.test(demand)) { kind = 'project'; kindEvidence = demand; kindRule = '采购需求工程标的'; }
  else if (GENERAL_SERVICE.test(demand)) { kind = 'service'; kindEvidence = demand; kindRule = '采购需求服务业务'; }
  else if (EXTRA_GOODS.test(demand)) { kind = 'goods'; kindEvidence = demand; kindRule = '采购需求具体设备'; }
  else if (/工程量清单|施工图|施工总承包|建筑工程施工|市政公用工程|工程项目经理/.test(text)) { kind='project';kindEvidence=text.match(/.{0,25}(?:工程量清单|施工图|施工总承包|建筑工程施工|市政公用工程|工程项目经理).{0,70}/)?.[0];kindRule='正文明确施工内容或资质'; }
  else if (/建设项目|综合楼|教学楼|楼项目|整改|修复|基础设施|农村公益事业|财政奖补|配套设施|改建|扩建|光伏项目|农贸市场|环境集中治理|产业发展|文化建设|更换提升|环境整治|停车场|车间|高标准农田|土方清运|厂房拆除|井篦安装|真石漆/.test(title) && /工程量清单|施工图|施工资质|项目经理|建造师|施工验收|工期|工GK|工程F/.test(text)) {
    kind='project';kindEvidence=text.match(/.{0,30}(?:工程量清单|施工图|施工资质|项目经理|建造师).{0,50}/)?.[0];kindRule='项目标题及施工需求';
  }
  else if (/(?:供货期限|交货期|交货时间|供货期|供货(?:及)?安装期(?:限)?)\s*[:：]/.test(text)) { kind='goods';kindEvidence=text.match(/(?:供货期限|交货期|交货时间|供货期|供货(?:及)?安装期(?:限)?)\s*[:：].{0,80}/)?.[0];kindRule='明确交付货物要求'; }
  else if (/(?:服务期限|服务期)[：:]\s*(?:自|合同|签订|\d|[一二三四五六七八九十])/.test(text)) { kind='service';kindEvidence=text.match(/(?:服务期限|服务期)[：:][^\n]{0,100}/)?.[0];kindRule='明确服务履约期限'; }
  else { kind = null; kindEvidence = demand || title; kindRule = '标的证据不足'; }
  const gov = text.match(/(?:中华人民共和国)?政府采购法(?:实施条例)?|竞争性磋商|竞争性谈判|单一来源|询价采购|政府采购(?:项目|预算)/);
  const construction = text.match(/(?:中华人民共和国)?招标投标法(?:实施条例)?|工程建设项目|施工招标|工程总承包|监理招标|商工程〔|GCJS/);
  let business, evidence, rule;
  if (gov && !/商工程〔|GCJS/.test(text)) { business = 'GOV_PROCUREMENT'; evidence = gov[0]; rule = '正文采购依据或采购方式'; }
  else if ((kind === 'project' || /监理|勘察|设计|造价/.test(title)) && (construction || /招标/.test(title))) { business = 'CONSTRUCTION'; evidence = construction?.[0] || title; rule = '工程标的与工程招标依据'; }
  else if (kind === 'project') { business = 'CONSTRUCTION'; evidence = title; rule = '工程建设标的'; }
  else { business = 'GOV_PROCUREMENT'; evidence = title; rule = '按现有采购栏目容纳货物或服务'; }
  return {
    business_type: { value: business, rule, evidence },
    procurement_type: { value: business === 'CONSTRUCTION' ? null : kind, rule: business === 'CONSTRUCTION' ? '建设工程不使用采购子类' : kindRule, evidence: kindEvidence },
    subject: kind,
    needsReview: !kind ? ['采购标的无法明确识别'] : []
  };
}

const LABEL = /预算总金额|预算总价|(?:项目|采购)?预算金额|采购预算|项目预算|预算价|预算为|中标[（(]成交[）)]金额|中标总金额|成交总金额|中标总价|成交总价|中标金额|成交金额|成交价|中标价|最高投标限价|招标控制价|最高控制价|最高限价|采购控制价|采购限价|项目投资额|总投资|项目概算/g;
function amountValue(number, unit) {
  const n = Number(number.replace(/[,，\s]/g, ''));
  if (!Number.isFinite(n) || n < 0) return null;
  return Math.round((unit === '亿元' ? n * 10000 : unit === '万元' ? n : n / 10000) * 100) / 100;
}
function monetaryCandidates(title, text, tables) {
  text=text.replace(/中\s*标\s*人/g,'中标人');
  const candidates = [];
  const put = (kind, number, unit, evidence, rule, label) => {
    const value = amountValue(number, unit);
    if (value === null) return;
    candidates.push({ kind, value, sourceNumber: number, sourceUnit: unit, evidence: evidence.slice(0, 260), rule, label });
  };
  for (const match of text.matchAll(LABEL)) {
    const label = match[0];
    const kind = /预算/.test(label) ? 'budget' : /中标|成交|合同/.test(label) ? 'award' : /投资|概算/.test(label) ? 'investment' : 'ceiling';
    const tail = text.slice(match.index + label.length, match.index + label.length + 180);
    const direct = tail.match(/^[\s：:（()）人民币￥¥为是约]*([\d,，]+(?:\.\d+)?)\s*(亿元|万元|元)(?!\s*[/／每])/);
    // 表头后的下一行通常是包号/年份；无冒号时仅允许同一行数值，且不能读取数字前缀。
    const headerUnit = tail.match(/^[ \t]*[（(]\s*(亿元|万元|元)\s*[）)](?:[ \t]*[:：]\s*|[ \t]+)([\d,，]+(?:\.\d+)?)(?=$|[\s，,；;。])/);
    const resultTail=tail.split(/\n(?:序号|[一二三四五六七八九十]+、)/)[0];
    const afterName = /单位|供应商名称|包号|地址|中标内容|品牌/.test(tail.slice(0, 70)) && /中标.*金额|成交金额/.test(label)
      ? resultTail.match(/(?:^|\n)\s*([\d,，]+(?:\.\d+)?)\s*\n?\s*(亿元|万元|元)(?:\s|$)/) : null;
    if (direct) put(kind, direct[1], direct[2], label + tail.slice(0, direct[0].length), '金额标签与单位', label);
    else if (headerUnit) put(kind, headerUnit[2], headerUnit[1], label + tail.slice(0, headerUnit[0].length), '金额标签表头单位', label);
    else if (afterName) put(kind, afterName[1], afterName[2], label + tail.slice(0, tail.indexOf(afterName[0]) + afterName[0].length), '结果表格文字与单位', label);
  }
  // HTML 表格按表头列取值，不能把数量、单价或代理费作为总价。
  for (const rows of tables) {
    for (let hi = 0; hi < Math.min(rows.length, 4); hi++) {
      const headers = rows[hi];
      for (let ci = 0; ci < headers.length; ci++) {
        const header = headers[ci];
        if (!/预算金额|中标金额|成交金额|中标价|成交价|中标总价|成交总价/.test(header)) continue;
        const kind = /预算/.test(header) ? 'budget' : 'award';
        for (const row of rows.slice(hi + 1)) {
          if (row.length !== headers.length) continue;
          const cell = row[ci];
          const explicit = cell?.match(/^\s*([\d,，]+(?:\.\d+)?)\s*(亿元|万元|元)(?!\s*[/／每])/);
          // 独立单位列优先于表头；“金额（元）”下的监理费率不能当成元。
          const unitColumn=headers.findIndex(h => /^单位$/.test(h));
          const unit = unitColumn>=0 ? row[unitColumn] : header.match(/亿元|万元|元/)?.[0];
          if (/[%％/／每]|费率|单价/.test(unit || '')) continue;
          if (explicit) put(kind, explicit[1], explicit[2], `${headers.join(' | ')} => ${row.join(' | ')}`, 'HTML 表格金额列', header);
          else if (/^(?:亿元|万元|元)$/.test(unit || '') && /^[\d,，]+(?:\.\d+)?$/.test(cell || '')) put(kind, cell, unit, `${headers.join(' | ')} => ${row.join(' | ')}`, 'HTML 表格单位列', header);
        }
      }
    }
  }
  // 最终结果中，获奖主体紧邻的“报价”可作为成交金额；候选公示不适用。
  if (/中标|成交|结果/.test(title) && !/候选人|流标|废标|终止|失败/.test(title)) {
    const markers=[...text.matchAll(/(?:中标人(?:名称)?[：:]|成交人(?:名称)?[：:]|成交供应商(?:名称)?[：:]|中标单位[：:])/g)];
    for(let i=0;i<markers.length;i++) {
      const section=text.slice(markers[i].index,Math.min(markers[i].index+500,markers[i+1]?.index??text.length))
        .split(/\n[一二三四五六七八九十]+、|\n(?:供应商|投标单位|响应单位)报价情况/)[0];
      if (/中标价|成交价|中标金额|成交金额/.test(section)) continue;
      const quote = section.match(/(?:投标(?:总)?报价|最终报价|报价)[：:]\s*([\d,，]+(?:\.\d+)?)\s*(亿元|万元|元)(?!\s*[/／每])/);
      if (quote && !/候选人|业绩/.test(section.slice(0, section.indexOf(quote[0])))) put('award', quote[1], quote[2], section.slice(0, 260), '最终中标人上下文报价', '报价');
    }
  }
  return candidates.filter((c,i,a) => a.findIndex(x => x.kind === c.kind && x.value === c.value && x.evidence === c.evidence) === i);
}
function chooseAmount(candidates, kind, title) {
  let xs = candidates.filter(c => c.kind === kind);
  if (kind === 'award' && (!/中标|成交|结果/.test(title) || /候选人|流标|废标|终止|失败|更正|变更|澄清/.test(title))) return { value: null, rule: '未确认最终成交结果', evidence: title };
  const totals = xs.filter(c => /总金额|总价/.test(c.label));
  if (totals.length) xs = totals;
  const values = [...new Set(xs.map(c => c.value))];
  if (values.length === 1) return { value: values[0], rule: xs[0].rule, evidence: xs[0].evidence };
  return { value: null, rule: values.length ? '多金额或多标段无法确认单一总额' : '未发现带单位的明确金额', evidence: xs.slice(0, 3).map(x => x.evidence).join(' / ') };
}

function regionIndex(regions) {
  const list=[...regions.byCode.values()];
  const parentProvince = r => r.level === 1 ? r : r.level === 2 ? regions.byCode.get(r.parentCode) : regions.byCode.get(regions.byCode.get(r.parentCode)?.parentCode);
  return { list, parentProvince, regions };
}
function matchLocation(text, index, constrainedProvince = null) {
  const compact = text.replace(/\s/g, '');
  const allowed = r => !constrainedProvince || index.parentProvince(r)?.code === constrainedProvince;
  const namedProvinces = index.list.filter(r => r.level === 1 && compact.includes(r.name) && allowed(r));
  const namedCities = index.list.filter(r => r.level === 2 && compact.includes(r.name) && allowed(r));
  // 先用完整省市约束区县，防止“中心城区”命中山西的“城区”、“北京大道”命中北京市。
  const cityCodes = new Set(namedCities.map(r => r.code));
  const provinceCodes = new Set(namedProvinces.map(r => r.code));
  const matchedDistricts = index.list.filter(r => r.level === 3 && compact.includes(r.name) && allowed(r) &&
    !/^(?:市区|城区|郊区|矿区)$/.test(r.name) &&
    (!cityCodes.size || cityCodes.has(r.parentCode)) &&
    (!provinceCodes.size || provinceCodes.has(index.parentProvince(r)?.code)));
  // 太康县不能同时匹配甘肃康县，清丰县不能同时匹配江苏丰县。
  const districtHits=matchedDistricts.filter(r=>!matchedDistricts.some(other=>other.name!==r.name && other.name.includes(r.name)));
  let hits = [...namedProvinces, ...namedCities, ...districtHits];
  if (!namedCities.length && !districtHits.length) {
    const shortCities=index.list.filter(r => r.level === 2 && r.name.endsWith('市') && r.name.length >= 3 && compact.includes(r.name.slice(0,-1)) &&
      !compact.includes(r.name.slice(0,-1)+'大道') && !compact.includes(r.name.slice(0,-1)+'路') && allowed(r) &&
      (!provinceCodes.size || provinceCodes.has(index.parentProvince(r)?.code)));
    hits.push(...shortCities);
  }
  const provinces = [...new Set(hits.map(r => index.parentProvince(r)?.code).filter(Boolean))];
  if (provinces.length !== 1) return null;
  const province = index.regions.byCode.get(provinces[0]);
  const districts=hits.filter(r=>r.level===3);
  const cities=[...new Set(hits.filter(r=>r.level>=2).map(r=>r.level===2?r.code:r.parentCode))];
  if (cities.length>1) return { province, city:null, district:null, ambiguous:true };
  const city=index.regions.byCode.get(cities[0])||null;
  const district=districts.length===1?districts[0]:null;
  return {province,city,district,ambiguous:districts.length>1};
}
function resolveLocation(title, text, index) {
  text=text.replace(/采\s*购\s*人/g,'采购人').replace(/招\s*标\s*人/g,'招标人');
  const override = findAnnouncementRegionOverride(title);
  if (override) {
    const found=matchLocation(`${override.province} ${override.city} ${override.district==='市区'?'':override.district}`,index);
    if (found) return { ...found, rule:'已核实跨省项目覆盖', evidence: `${override.province} ${override.city} ${override.district}` };
  }
  const locations=[...text.matchAll(/(?:项目建设地点|建设地点|项目地点|项目所在地|项目实施地点|服务实施地|交货地点|施工地点|建设地址|供货地点|服务地点|项目地址)\s*[:：]?\s*([^\n。；;]{2,180})/g)].map(m=>m[0]);
  let fallback = null;
  for (const source of locations) {
    const found=matchLocation(source,index);
    if (found?.city) return {...found,rule:'正文项目地点',evidence:source};
    if (found) fallback={...found,rule:'正文项目地点',evidence:source};
  }
  const fromTitle=matchLocation(title,index);
  if (fromTitle?.city) return {...fromTitle,rule:'标题行政区名称',evidence:title};
  if (!fallback && fromTitle) fallback={...fromTitle,rule:'标题行政区名称',evidence:title};
  // 只取采购人/招标人块，截断在代理机构信息前，绝不扫描所有地址。
  const buyers=[...text.matchAll(/(?:采购人\s*信息|招标人\s*信息|招标人或招标代理机构\s*[（(]异议受理[）)]\s*单位名称[：:]|招标人(?:名称|（异议受理单位）)?[：:]|采购人(?:名称)?[：:]|招标人为|采购人为|遴选单位[：:]|招标单位[：:]|采购单位[：:]|建设单位[：:]|项目业主[：:])([\s\S]{0,350})/g)];
  for (const m of buyers.reverse()) {
    const block=m[1].split(/(?:采购代理机构|招标代理机构|代理机构|代理公司|财政部门信息|监督部门)/)[0];
    if(/^海隆工程咨询有限公司/.test(block.trim())) continue;
    const found=matchLocation(block,index,fallback?.province?.code);
    if(found?.city) return {...found,rule:'采购人或招标人明确地点',evidence:block.slice(0,220)};
    if(found && !fallback) fallback={...found,rule:'采购人或招标人明确地点',evidence:block.slice(0,220)};
  }
  if (fallback) return fallback;
  return {province:null,city:null,district:null,rule:'没有可靠项目地区证据',evidence:''};
}

function review(record, regions) {
  const doc=record.text!==undefined?record:extractDocument(record.content);
  const classification=classify(record.title,doc.text);
  const confirmedSubjects = {
    1232: ['goods','造成部分野外监测设施设备毁坏','从原供应商处添购'],
    1735: ['goods','鼻动力系统','交货期'],
    1717: ['goods','院内模拟导航系统','交货期']
  };
  const confirmed=confirmedSubjects[record.id];
  if(confirmed && confirmed.slice(1).every(s=>doc.text.includes(s))) {
    classification.procurement_type={value:confirmed[0],rule:'逐条复核采购标的及交付要求',evidence:confirmed.slice(1).join('；')};
    classification.needsReview=[];
  }
  const money=monetaryCandidates(record.title,doc.text,doc.tables);
  const award=chooseAmount(money,'award',record.title);
  let budget=chooseAmount(money,'budget',record.title);
  const notes=[];
  // 仅有明确“现变更为/变更为”分界时采用新预算，不依赖出现顺序猜测。
  if (noticeType(record.title,record.category).value==='correction') {
    const marker=[...doc.text.matchAll(/(?:现(?:更正|变更)为|变更为)\s*[:：]?/g)].at(-1);
    if(marker) {
      const updated=monetaryCandidates(record.title,doc.text.slice(marker.index+marker[0].length),[]);
      const current=chooseAmount(updated,'budget',record.title);
      if(current.value!==null) budget={...current,rule:'明确变更分界后的现行预算'};
      const currentAward=chooseAmount(updated,'award',record.title.replace(/更正|变更|澄清/g,''));
      if(currentAward.value!==null) notes.push({type:'resultCorrection',rule:'更正公告不重复计入结果金额',currentAwardAmount:currentAward.value,evidence:currentAward.evidence});
    }
  }
  const ceilings=money.filter(c=>c.kind==='ceiling' && c.value>0);
  const unitConflict=award.value!==null && ceilings.length && award.value>Math.max(...ceilings.map(c=>c.value))*20;
  if(unitConflict) { award.value=null;award.rule='原文金额单位与控制价数量级冲突';award.evidence=money.filter(c=>['award','ceiling'].includes(c.kind)).map(c=>c.evidence).join(' / ').slice(0,600); }
  const location=resolveLocation(record.title,doc.text,regionIndex(regions));
  const winnerQuote=doc.text.replace(/中\s*标\s*人/g,'中标人').match(/(?:中标人(?:名称)?|中标单位|成交供应商(?:名称)?)[：:][^\n]+\n(?:[^\n]*\n){0,4}(?:投标(?:总)?报价|最终报价|报价)[：:]\s*([\d,，]+(?:\.\d+)?)\s*(?=\n|$)/);
  const missingAwardUnit=award.rule==='未发现带单位的明确金额' && noticeType(record.title,record.category).value==='result' && !!winnerQuote;
  if(missingAwardUnit) {award.rule='原文最终报价未注明单位';award.evidence=winnerQuote[0];}
  if(location.ambiguous && location.city) notes.push({type:'multipleDistricts',rule:'项目跨多个区县，保留明确省市，区县留空',evidence:location.evidence});
  if(award.rule==='未确认最终成交结果' && /候选人/.test(record.title)) notes.push({type:'candidateNotice',rule:'候选报价正常保留在正文，不填最终中标金额',evidence:record.title});
  const region={ province:location.province?.name||'',city:location.city?.name||'',district:location.district?.name||'' };
  return {
    fields: {
      notice_type:noticeType(record.title,record.category),business_type:classification.business_type,procurement_type:classification.procurement_type,
      budget_amount:budget,award_amount:award,
      province:{value:region.province,rule:location.rule,evidence:location.evidence},
      city:{value:region.city,rule:location.rule,evidence:location.evidence},
      district:{value:region.district,rule:location.rule,evidence:location.evidence}
    }, moneyCandidates:money,notes,
    needsReview:[...classification.needsReview,...(!location.city?['城市未确认']:[]),...(location.ambiguous && !location.city?['涉及多个地区']:[]),...(unitConflict?['中标金额原文单位冲突']:[]),...(missingAwardUnit?['最终中标报价缺少金额单位']:[]),
      ...(['budget','award'].filter(kind=>(kind==='budget'?budget:award).rule==='多金额或多标段无法确认单一总额').map(kind=>`${kind} 存在多个金额`))]
  };
}

function projectStem(title) {
  return title.replace(/\s/g,'').replace(/^关于/,'').replace(/[（(](?:二|三|四|五|第[一二三四五六七八九十])次[）)]/g,'')
    .replace(/(?:竞争性(?:磋商|谈判)(?:结果)?|公开招标|招标|采购|中标候选人|定标候选人|中标结果|中标|成交结果|成交|流标|终止|变更|更正|暂停|补充|澄清|结果|专家论证结果|定标时间|招标文件|控制价|最高投标限价)(?:公告|公示).*$/,'')
    .replace(/[-—、]+$/,'');
}

function reviewAll(records, regions) {
  const reviewed=records.map(r=>({legacyId:r.id,title:r.title,...review(r,regions)}));
  const documents=new Map(records.map(r=>[r.id,r.text!==undefined?r:extractDocument(r.content)]));
  const byId=new Map(reviewed.map(r=>[r.legacyId,r]));
  for(const [id,count,scope,sourceId,anchor] of awardScopes) {
    const row=byId.get(id);
    if(!row) continue;
    if(sourceId && !documents.get(sourceId)?.text.includes(anchor)) throw new Error(`公告 ${id} 的标段范围来源 ${sourceId} 已变化`);
    let parts=row.moneyCandidates.filter(c=>c.kind==='award');
    // 同一结果常同时以表格、文字重复发布，只选一种表达；不同包的同价不能去重。
    const tableParts=parts.filter(c=>/^HTML 表格/.test(c.rule));
    if(tableParts.length) parts=tableParts;
    else {
      const labeled=parts.filter(c=>c.rule==='金额标签与单位');
      if(labeled.length) parts=labeled;
    }
    if(parts.length!==count || row.fields.notice_type.value!=='result' || row.needsReview.includes('中标金额原文单位冲突')) throw new Error(`公告 ${id} 的最终金额项数或单位与已复核范围不符：${parts.length}/${count}`);
    row.fields.award_amount={value:sumAmounts(parts),rule:'逐条复核完整标段范围后合计',scope,sourceId:sourceId||id,
      evidence:parts.map(c=>c.evidence).join(' / '),parts:parts.map(c=>({sourceNumber:c.sourceNumber,sourceUnit:c.sourceUnit,evidence:c.evidence}))};
    row.needsReview=row.needsReview.filter(s=>s!=='award 存在多个金额');
  }
  for(const [id,reason] of Object.entries(incompleteAmounts)) {
    const row=byId.get(Number(id));
    if(!row) continue;
    row.fields.award_amount={value:null,rule:'公告未提供完整固定总额',evidence:reason};
    row.needsReview=row.needsReview.filter(s=>s!=='award 存在多个金额');
    row.needsReview.push('中标结果含单价、费率或缺失标段，无法确认总额');
  }
  const historical=byId.get(384),correction=byId.get(387);
  const correctedPart=correction?.notes.find(n=>n.type==='resultCorrection');
  if(historical && correctedPart) historical.notes.push({...correctedPart,type:'supersededResultPart',sourceId:387,
    rule:'施工十六标由后续更正公告387替换；本公告原文保留，旧报价不计入总额'});
  const groups=new Map();
  records.forEach((r,i)=>{
    const stem=projectStem(r.title);
    if(!groups.has(stem)) groups.set(stem,[]);
    groups.get(stem).push(i);
  });
  for(const indexes of groups.values()) {
    // 只补同名项目中缺失的分类/地区，要求所有可识别公告结论一致；金额不跨公告复制。
    for(const i of indexes) {
      const row=reviewed[i];
      if(row.fields.procurement_type.value===null && row.fields.business_type.value==='GOV_PROCUREMENT') {
        const sources=indexes.map(j=>reviewed[j]).filter(r=>r.legacyId!==row.legacyId && (r.fields.procurement_type.value || r.fields.business_type.value==='CONSTRUCTION'));
        const kinds=new Set(sources.map(r=>r.fields.business_type.value+':'+r.fields.procurement_type.value));
        if(kinds.size===1) {
          const source=sources[0];
          for(const key of ['business_type','procurement_type']) row.fields[key]={...source.fields[key],rule:`同名项目公告 ${source.legacyId} 的明确标的`,sourceId:source.legacyId};
          row.needsReview=row.needsReview.filter(x=>x!=='采购标的无法明确识别');
        }
      }
      if(!row.fields.city.value) {
        const sources=indexes.map(j=>reviewed[j]).filter(r=>r.legacyId!==row.legacyId && r.fields.city.value && (!row.fields.province.value || row.fields.province.value===r.fields.province.value));
        const cities=new Set(sources.map(r=>r.fields.province.value+':'+r.fields.city.value));
        if(cities.size===1) {
          const source=sources[0];
          for(const key of ['province','city']) row.fields[key]={...source.fields[key],rule:`同名项目公告 ${source.legacyId} 的明确地区`,sourceId:source.legacyId};
          // 区县只有同名项目的明确区县全部一致时补充。
          const districts=new Set(sources.map(r=>r.fields.district.value).filter(Boolean));
          if(districts.size===1) row.fields.district={...sources.find(r=>r.fields.district.value).fields.district,sourceId:source.legacyId,rule:`同名项目公告 ${source.legacyId} 的明确区县`};
          row.needsReview=row.needsReview.filter(x=>!['城市未确认','涉及多个地区'].includes(x));
        }
      }
    }
  }
  for(const [id,sourceId,business,kind,anchor,titleAnchor] of subjects) {
    const row=byId.get(id);
    if(!row) continue;
    if(!row.title.includes(titleAnchor) || !documents.get(sourceId)?.text.includes(anchor)) throw new Error(`公告 ${id} 的采购标的复核依据已变化`);
    row.fields.business_type={value:business,rule:'逐条复核对应项目及标段的采购依据',sourceId,evidence:anchor};
    row.fields.procurement_type={value:kind,rule:'对应公告明确本标段的标的，未复制其他包类型',sourceId,evidence:anchor};
    row.needsReview=row.needsReview.filter(s=>s!=='采购标的无法明确识别');
  }
  const mixed=byId.get(2956);
  if(mixed && byId.get(2912)?.fields.procurement_type.value==='service' && byId.get(2913)?.fields.procurement_type.value==='goods') {
    mixed.needsReview=mixed.needsReview.filter(s=>s!=='采购标的无法明确识别');
    mixed.needsReview.push('项目多个包采购类型不同，单一子类字段无法表示');
    mixed.notes.push({type:'mixedProcurement',rule:'终止公告覆盖整个项目，保留不同包的实际采购类型',sourceIds:[2912,2913],evidence:'包1智慧医院信息化服务；包2医疗设备货物'});
  }
  for(const inference of inferredSubjects) {
    const row=byId.get(inference.id);
    if(!row) continue;
    const checks=[inference,...(inference.sources||[])];
    if(row.fields.business_type.value!=='GOV_PROCUREMENT' || checks.some(source=>
      byId.get(source.id)?.title!==source.title || !source.anchors.every(anchor=>documents.get(source.id)?.text.includes(anchor)))) {
      throw new Error(`公告 ${inference.id} 的采购分类推断依据已变化`);
    }
    row.fields.procurement_type={value:inference.kind,rule:'用户授权按业务含义及主要采购包合理推断',inferred:true,
      evidence:checks.map(source=>`${source.id}：${source.anchors.join('；')}`).join(' / '),rationale:inference.rationale,
      sourceIds:checks.map(source=>source.id)};
    row.notes.push({type:'classificationInference',value:inference.kind,rule:'2026-10-04 用户授权合理推断归类',rationale:inference.rationale});
    row.needsReview=row.needsReview.filter(reason=>!['采购标的无法明确识别','项目多个包采购类型不同，单一子类字段无法表示'].includes(reason));
  }
  return reviewed;
}

function sumAmounts(parts) {
  // 先用整数精确汇总原文金额，最后统一舍入万元；避免每包先舍入导致合计误差。
  const scale=Math.max(...parts.map(c=>(c.sourceNumber.replace(/[,，\s]/g,'').split('.')[1]||'').length));
  const denominator=10n**BigInt(scale);
  const yuanScaled=parts.reduce((sum,c)=>{
    const [whole,decimal='']=c.sourceNumber.replace(/[,，\s]/g,'').split('.');
    const multiplier=c.sourceUnit==='亿元'?100000000n:c.sourceUnit==='万元'?10000n:1n;
    return sum+(BigInt(whole)*denominator+BigInt(decimal.padEnd(scale,'0')||'0'))*multiplier;
  },0n);
  const perHundredYuan=denominator*100n;
  return Number((yuanScaled+perHundredYuan/2n)/perHundredYuan)/100;
}

function reviewInfo(record) {
  // 43 篇历史文章逐条复核；分类名必须来自当前管理页面的选项。
  const groups = [
    ['POLICY_REGULATION','国家政策',[317,320,329,330,331,332,333,334,335,399,401,413,730],'正文国家机关政策、条例或部门管理办法'],
    ['POLICY_REGULATION','地方政策',[321,322,323,324,325,326,403,414],'正文河南省主管部门通知或管理办法'],
    ['POLICY_REGULATION','行业法规',[316,327,400,482,925],'正文建筑、工程招标及资质管理规定'],
    ['COMPANY_NEWS','知识资讯',[280,281,282,283,284,318,328,394,395,398,444,718,719],'正文业务知识、法律分析或操作说明'],
    ['COMPANY_NEWS','通知公告',[393,443],'培训或网站公告发布通知'],
    ['COMPANY_NEWS','行业动态',[319],'正文对行业政策工作要点的新闻解读'],
    ['COMPANY_NEWS','公司新闻',[1975],'正文海隆公司走访调研活动']
  ];
  const group=groups.find(g=>g[2].includes(record.id));
  if(!group) throw new Error(`资讯 ${record.id} 尚未逐条确认分类`);
  const doc=extractDocument(record.news_content);
  return {legacyId:record.id,title:record.news_title,type:group[0],category:group[1],rule:group[3],evidence:doc.text.slice(0,220)||record.news_title,
    needsReview:doc.text ? [] : ['正文只有图片，分类依据标题，原图片待补充']};
}

module.exports={extractDocument,noticeType,classify,monetaryCandidates,chooseAmount,resolveLocation,regionIndex,review,reviewAll,projectStem,reviewInfo,sumAmounts};

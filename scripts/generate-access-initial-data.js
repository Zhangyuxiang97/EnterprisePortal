/**
 * 合并只读导出的 Access 公开内容与已清洗公告，生成首次部署 SQL。
 * 用法：node generate-access-initial-data.js --source <导出目录> --baseline <final-data.json>
 * 不连接数据库。原 MDB 和包含个人信息的旧表不加入初始化数据。
 */
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const assert = require('node:assert/strict');
const cheerio = require('cheerio');
const { transformItem } = require('./transform-data');
const { loadRegions, renderSql, escapeSql, resolveRegion } = require('./generate-initial-announcement-sql');
const { reviewAll, reviewInfo, extractDocument } = require('./review-announcement-fields');
const ROOT = path.resolve(__dirname, '..');
const ANNOUNCEMENT_CATEGORIES = new Map([[1, '招标信息'], [26, '中标公示'], [27, '变更公告']]);
const FILES = { 招标信息: '06_announcements_bidding.sql', 中标公示: '07_announcements_result.sql', 变更公告: '08_announcements_correction.sql' };
const TYPES = { 招标信息: 'bidding', 中标公示: 'result', 变更公告: 'correction' };
const ALLOWED_TAGS = new Set('p div span br hr h1 h2 h3 h4 h5 h6 strong b em i u s sub sup ul ol li blockquote pre code table thead tbody tfoot tr td th caption colgroup col a img'.split(' '));
const MEDIA_MANIFEST = require('./legacy-media-manifest.json');
const MEDIA_MAP = new Map(MEDIA_MANIFEST.assets.flatMap(asset =>
  [asset.legacyPath, asset.sourceUrl, asset.publicPath].map(url => [url.toLowerCase(), asset.publicPath])));

function plain(html) {
  const $ = cheerio.load(html || '');
  $('script,style,noscript,template,iframe,object').remove();
  $('p,div,br,li,h1,h2,h3,h4,h5,h6,tr,td,th,blockquote').append(' ');
  return $.root().text().replace(/\s+/g, ' ').trim();
}

function hasContent(html) {
  return Boolean(plain(html) || cheerio.load(html || '')('img[src],a[href]').length);
}

function summaryFor(row) {
  const explicit = plain(row.jianjie);
  const text = explicit || plain(row.cleanedContent);
  return Array.from(text || (hasContent(row.cleanedContent) ? '原文为扫描件，点击查看完整内容。' : '')).slice(0, 500).join('');
}

function cleanHtml(html, missing = [], mediaMap = MEDIA_MAP) {
  const $ = cheerio.load(html || '', null, false);
  $('script,style,iframe,object,embed,link,meta,form,input,button,textarea,select').remove();
  $('*').each((_, node) => {
    const el = $(node);
    if (!ALLOWED_TAGS.has(node.name)) {
      el.replaceWith(el.contents());
      return;
    }
    for (const [key, value] of Object.entries(node.attribs || {})) {
      if (/^on/i.test(key) || !['href', 'src', 'alt', 'title', 'colspan', 'rowspan', 'style'].includes(key)) {
        el.removeAttr(key);
        continue;
      }
      if (key === 'style') {
        const style = value.split(';').filter(rule =>
          /^(?:text-align|font-weight|font-style|text-decoration|color|background-color|vertical-align|border(?:-[a-z]+)?|padding(?:-[a-z]+)?)\s*:/i.test(rule.trim()) &&
          !/url\s*\(|expression|[<>\\]/i.test(rule)
        ).join(';');
        if (style) el.attr(key, style); else el.removeAttr(key);
      }
    }
    if (node.name !== 'img' && node.name !== 'a') return;
    const attr = node.name === 'img' ? 'src' : 'href';
    const url = (el.attr(attr) || '').trim();
    const recovered = mediaMap.get(url.toLowerCase());
    if (recovered) {
      el.attr(attr, recovered);
      return;
    }
    const inlineImage = node.name === 'img' && /^data:image\/(?:png|jpeg|gif|webp);base64,[a-z0-9+/=\s]+$/i.test(url);
    const external = /^(?:https?:\/\/|\/\/)/i.test(url);
    const safeAnchor = node.name === 'a' && /^(?:mailto:|tel:|#)/i.test(url);
    if (inlineImage || external || safeAnchor) return;
    if (url) missing.push({ tag: node.name, url });
    if (node.name === 'img') el.remove();
    else {
      const text = el.text().trim();
      if (/\.(?:pdf|docx?|xlsx?|zip|rar|7z|pptx?)(?:[?#]|$)/i.test(url)) {
        el.replaceWith($('<span>').text(text ? `${text}（附件待补充）` : '附件待补充'));
      } else el.replaceWith(el.contents());
    }
  });
  return $.html().trim();
}

function dateSql(value) {
  assert.match(value || '', /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/);
  return escapeSql(value.replace('T', ' ').slice(0, 19));
}

function renderInfo(rows) {
  return '-- Access 旧站真实资讯：43 篇，按当前系统分类及正文重新复核\n' +
    'INSERT INTO info_publications (type, category, title, summary, content, author, publisher, publish_time, view_count, is_top) VALUES\n' +
    rows.map(r => '(' + [
      escapeSql(r.reviewed.type), escapeSql(r.reviewed.category),
      escapeSql(plain(r.news_title)), escapeSql(summaryFor(r)),
      escapeSql(hasContent(r.cleanedContent) ? r.cleanedContent : '<p>原站正文资料暂缺，待补充。</p>'),
      escapeSql(r.news_author), escapeSql(r.news_author || '海隆工程咨询有限公司'),
      dateSql(r.news_addtime), String(Math.max(0, Number(r.news_hits) || 0)), r.news_stickies ? '1' : '0'
    ].join(', ') + ')').join(',\n') + ';\n';
}

function main(sourceDirectory, baselineFile) {
  assert.ok(sourceDirectory && baselineFile, '需要 --source 导出目录和 --baseline 已清洗公告文件');
  for (const asset of MEDIA_MANIFEST.assets) {
    const bytes = fs.readFileSync(path.join(ROOT, 'BackEnd/HailongConsulting.API/SeedAssets/legacy', path.basename(asset.publicPath)));
    assert.equal(bytes.length, asset.size, `初始化资源大小不符：${asset.publicPath}`);
    assert.equal(crypto.createHash('sha256').update(bytes).digest('hex'), asset.sha256, `初始化资源内容不符：${asset.publicPath}`);
  }
  const read = name => JSON.parse(fs.readFileSync(path.join(sourceDirectory, `${name}.json`), 'utf8').replace(/^\uFEFF/, ''));
  const news = read('news');
  const baseline = JSON.parse(fs.readFileSync(baselineFile, 'utf8').replace(/^\uFEFF/, ''));
  const ids = new Set(baseline.map(r => Number(r._raw?.id)));
  assert.equal(ids.size, baseline.length, '基线公告旧站 ID 必须唯一');
  assert.ok(!ids.has(NaN), '基线公告缺少旧站 ID');
  assert.equal(new Set(news.map(r => r.id)).size, news.length, '备份旧站 ID 必须唯一');
  const missingRecords = [];
  // 所有公开源内容生成缺失文件清单，即使该条已包含在基线中。
  for (const r of news) {
    const missing = [];
    r.cleanedContent = cleanHtml(r.news_content, missing);
    if (r.news_image && !/^(?:https?:\/\/|\/\/|data:image\/)/i.test(r.news_image)) missing.push({ tag: 'cover', url: r.news_image });
    if (missing.length) missingRecords.push({ table: 'news', id: r.id, title: plain(r.news_title), references: missing });
  }
  const mediaPattern = /(?:uploadfile\/|\/admin\/edit\/|file:\/\/|<script\b|\son\w+\s*=|(?:src|href)\s*=\s*["']\s*javascript:)/i;
  for (const r of baseline) {
    if (mediaPattern.test(r.content || '')) r.content = cleanHtml(r.content);
  }
  const added = news.filter(r => ANNOUNCEMENT_CATEGORIES.has(r.news_bigcid) && !ids.has(r.id)).map(r => {
    const item = transformItem({
      id: r.id, fullTitle: plain(r.news_title), contentHtml: r.cleanedContent,
      contentText: plain(r.news_content), categoryBid: r.news_bigcid, category: ANNOUNCEMENT_CATEGORIES.get(r.news_bigcid),
      publishTime: r.news_addtime.slice(0, 19), viewCount: r.news_hits,
      publisher: r.news_author || '海隆工程咨询有限公司',
      url: `http://www.henanhailong.com/news.asp?id=${r.id}`
    });
    item.is_top = r.news_stickies ? 1 : 0;
    return item;
  });
  const regions = loadRegions();
  const all=[...baseline,...added];
  const sourceById=new Map(news.map(r=>[r.id,r]));
  const fieldReview=reviewAll(all.map(item=>{
    const source=sourceById.get(Number(item._raw.id));
    return {id:Number(item._raw.id),title:item.title,category:source?.news_bigcid||26,...extractDocument(source?.news_content||item.content)};
  }),regions);
  all.forEach((item,i)=>{
    assert.ok(item.title.length <= 255 && item.content, `公告字段异常：${item._raw.id}`);
    const audit=fieldReview[i];
    for(const [key,field] of Object.entries(audit.fields)) {
      field.previousValue=item[key]??null;
      field.changed=(item[key]??null)!==field.value;
      item[key]=field.value;
    }
    item.project_region=[item.province,item.city,item.district].filter(Boolean).join(' ');
    const resolved=resolveRegion(item,regions);
    assert.ok(!item.province || resolved.provinceCode,`复核省份无法映射：${item._raw.id}`);
    assert.ok(!item.city || resolved.cityCode,`复核城市无法映射：${item._raw.id}`);
    assert.ok(!item.district || resolved.districtCode,`复核区县无法映射：${item._raw.id}`);
  });
  fs.mkdirSync(path.join(ROOT, 'scripts/data'), { recursive: true });
  fs.writeFileSync(path.join(ROOT, 'scripts/data/access-initial-announcements.json'), JSON.stringify([...baseline, ...added]));
  const grouped = {};
  for (const [category, file] of Object.entries(FILES)) {
    const items = [...baseline.filter(r => r._raw.category === category), ...added.filter(r => r._raw.category === category)];
    grouped[category] = items.length;
    fs.writeFileSync(path.join(ROOT, 'SQL', file), renderSql(TYPES[category], items, regions));
  }
  const info = news.filter(r => [28,29,30].includes(r.news_bigcid));
  assert.equal(info.length, 43, '公开资讯数量与核查结果不符');
  const infoReview=info.map(r=>{
    r.reviewed=reviewInfo(r);
    return {...r.reviewed,previousType:r.news_bigcid===29?'POLICY_REGULATION':'COMPANY_NEWS',previousCategory:r.news_bigcid===29?'法律法规':r.news_bigcid===30?'知识资讯':'公司新闻'};
  });
  const countBy=(rows,key)=>rows.reduce((counts,r)=>{counts[r[key]]=(counts[r[key]]||0)+1;return counts;},{});
  const reviewSummary={announcementCount:all.length,noticeTypeCounts:countBy(all,'notice_type'),businessTypeCounts:countBy(all,'business_type'),
    procurementTypeCounts:countBy(all,'procurement_type'),budgetAmountCount:all.filter(r=>r.budget_amount!==null).length,
    awardAmountCount:all.filter(r=>r.award_amount!==null).length,provinceCount:all.filter(r=>r.province).length,cityCount:all.filter(r=>r.city).length,districtCount:all.filter(r=>r.district).length,
    changedFields:Object.fromEntries(Object.keys(fieldReview[0].fields).map(key=>[key,fieldReview.filter(r=>r.fields[key].changed).length])),
    announcementsNeedingReview:fieldReview.filter(r=>r.needsReview.length).length,
    pendingReasonCounts:fieldReview.reduce((counts,r)=>{for(const reason of r.needsReview) counts[reason]=(counts[reason]||0)+1;return counts;},{}),
    infoTypeCounts:countBy(infoReview,'type'),infoCategoryCounts:countBy(infoReview,'category')};
  fs.writeFileSync(path.join(ROOT,'SQL/initialization-field-review.json'),JSON.stringify({
    methodology:'全量规则复核，43 篇资讯逐条分类；字段附原值、结论、证据及变更标识。同名公告补一致分类和地区；逐条确认同标段类型及完整结果范围见 scripts/confirmed-announcement-reviews.js。用户于2026-10-04授权对剩余5条采购子类合理推断，结论标注inferred、rationale和来源；混合包按预算占比最大的采购类型归档。分包先汇总原始数值再舍入万元，附 parts 和 scope；候选报价不填最终金额，更正后金额在 notes 中留证、不重复统计。单价、费率、缺单位或缺标段留空并列待核查；明确多区县范围在 notes 中记录。',
    summary:reviewSummary,pendingAnnouncements:fieldReview.filter(r=>r.needsReview.length).map(r=>({legacyId:r.legacyId,title:r.title,needsReview:r.needsReview})),
    announcements:fieldReview,infoPublications:infoReview
  },null,2)+'\n');
  let init = fs.readFileSync(path.join(ROOT, 'SQL/02_hailong_consulting_init_data.sql'), 'utf8');
  init = init.replace(/-- 更新时间：\d{4}-\d{2}-\d{2}/, '-- 更新时间：2026-10-04');
  const profile = read('about').find(r => r.ID === 1);
  assert.ok(profile?.content, '缺少真实企业简介');
  init = init.replace(/INSERT INTO `company_profile`[\s\S]*?(?=-- ============================================)/,
    `INSERT INTO \`company_profile\` (\`title\`, \`content\`, \`highlights\`) VALUES\n(${escapeSql('关于海隆咨询')}, ${escapeSql(cleanHtml(profile.content))}, '["专业咨询","合作共赢"]');\n\n`);
  // 未提供证书/荣誉文件和真实业绩依据，首次部署不发布原演示项目。
  for (const table of ['company_qualifications', 'company_honors', 'major_achievements']) {
    init = init.replace(new RegExp('INSERT INTO `' + table + '`[\\s\\S]*?(?=-- ============================================)'), '-- 原演示数据不作为真实业务数据发布，待后台补充。\n\n');
  }
  init = init.replace(/-- 公司公告[\s\S]*?(?=-- ============================================\r?\n-- 数据初始化完成)/,
    '-- 真实新闻、政策和知识资讯由 09_access_info_publications.sql 导入。\n\n');
  init = init.replace(/真实公告数据由 06、07、08 三个公告初始化文件导入（\d+条）/, `真实公告数据由 06、07、08 三个公告初始化文件导入（${baseline.length + added.length}条）`)
    .replace(/-- 公告文件在 MySQL 初始化后通过 init-announcements.sh 脚本执行/, '-- MySQL 空数据目录首次启动时自动按文件名顺序执行。');
  const links = read('LINK').filter(r => !init.includes(escapeSql(r.link_url)));
  // 新增链接使用独立文件，重新生成时不与 02 中的条目互相影响。
  let sql = '-- Access 旧站资讯及补充友情链接（仅用于空库首次初始化）\nSET NAMES utf8mb4;\nUSE `hailong_consulting`;\n\n' + renderInfo(info);
  if (links.length) sql += '\nINSERT INTO friendly_links (name, url, sort_order, status) VALUES\n' + links.map(r =>
    `(${escapeSql(r.link_title)}, ${escapeSql(r.link_url)}, ${10 + Number(r.px_id || 0)}, 1)`
  ).join(',\n') + ';\n';
  fs.writeFileSync(path.join(ROOT, 'SQL/02_hailong_consulting_init_data.sql'), init);
  fs.writeFileSync(path.join(ROOT, 'SQL/09_access_info_publications.sql'), sql);
  fs.writeFileSync(path.join(ROOT, 'SQL/10_legacy_company_media.sql'), renderCompanyMedia());
  for (const r of read('about')) {
    const missing=[]; cleanHtml(r.content, missing);
    if (missing.length) missingRecords.push({table:'about',id:r.ID,title:r.title,references:missing});
  }
  const uploadPaths = [...new Set(missingRecords.flatMap(r => r.references.map(x => x.url)).filter(u => /\/uploadfile\//i.test(u)))].sort();
  const report = {
    source: '海隆数据库.mdb', sourceDate: '2026-09-24',
    sourceSha256: fs.existsSync(path.join(sourceDirectory, 'source.mdb')) ? crypto.createHash('sha256').update(fs.readFileSync(path.join(sourceDirectory, 'source.mdb'))).digest('hex') : null,
    baselineCount: baseline.length, addedAnnouncementCount: added.length,
    announcementCount: baseline.length + added.length, announcementCounts: grouped,
    noticeTypeCounts: [...baseline, ...added].reduce((counts,r) => { counts[r.notice_type] = (counts[r.notice_type] || 0) + 1; return counts; }, {}),
    infoCounts: reviewSummary.infoCategoryCounts,fieldReviewSummary:reviewSummary,
    addedAnnouncements: added.map(r => ({ legacyId: r._raw.id, title: r.title, category: r._raw.category, province: r.province, city: r.city, district: r.district })),
    addedFriendlyLinkCount: links.length, uploadPaths, missingRecords,
    recoveredMedia: MEDIA_MANIFEST.assets,
    announcementsWithoutCity: all.filter(r => !resolveRegion(r, regions).cityCode).map(r => ({ legacyId: r._raw.id, title: r.title })),
    omittedPages: ['人才招聘（当前无独立模块）'],
    notes: ['保留 1 条不在备份中的既有公告', '企业与资讯共 28 张图片、1 个 Word 附件恢复为本地初始化资源，按清单校验 SHA-256', '4 项企业证照资料、6 组荣誉由 10_legacy_company_media.sql 初始化，历史证书不推断续期', '其余缺失本地图片已移除，附件保留名称并标注待补充，详见 missingRecords', '外链按原地址保留，未核验外站可用性', '6 处 PNG 内嵌图片保留在正文']
  };
  fs.writeFileSync(path.join(ROOT, 'SQL/initialization-source-report.json'), JSON.stringify(report, null, 2) + '\n');
  console.log(JSON.stringify({ announcementCount: report.announcementCount, grouped, infoCount: info.length, addedLinks: links.length, missingUploadFiles: uploadPaths.length }));
}

function renderCompanyMedia() {
  const company = require('./legacy-company-data.json');
  const values = row => row.map(escapeSql).join(', ');
  let sql = '-- 旧站公开证书原图核对恢复，仅用于空库初始化；历史证书不推断续期。\nSET NAMES utf8mb4;\nUSE `hailong_consulting`;\n';
  for (const [key, table, fields] of [
    ['qualifications', 'company_qualifications', ['certificate_no','issuing_authority','issue_date','expiry_date']],
    ['honors', 'company_honors', ['award_organization','award_date','certificate_no']]
  ]) {
    company[key].forEach((row, index) => {
      const asset = MEDIA_MANIFEST.assets.find(a => path.basename(a.publicPath) === row.image);
      assert.ok(asset, `企业证书缺少已核验原图：${row.image}`);
      sql += `\nINSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type) VALUES (${values([
        row.image, asset.publicPath.slice(1), asset.publicPath])}, ${asset.size}, 'image/jpeg', '.jpg', 'image', '${table === 'company_qualifications' ? 'company_qualification' : 'company_honor'}');\n`;
      sql += `SET @legacy_image_id = LAST_INSERT_ID();\n`;
      sql += `INSERT INTO ${table} (name, description, image_id, ${fields.join(', ')}, sort_order, status) VALUES (${values([row.name,row.description])}, @legacy_image_id, ${values(fields.map(f => row[f] || null))}, ${index + 1}, 1);\n`;
      sql += `UPDATE attachments SET related_id = LAST_INSERT_ID() WHERE id = @legacy_image_id;\n`;
    });
  }
  // 正文媒体也注册到附件管理；按实际引用记录关联，不依赖旧站 ID。
  const certificateFiles = new Set([...company.qualifications, ...company.honors].map(row => row.image));
  for (const asset of MEDIA_MANIFEST.assets.filter(asset => !certificateFiles.has(path.basename(asset.publicPath)))) {
    const extension = path.extname(asset.publicPath);
    const type = extension === '.docx' ? 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' : 'image/jpeg';
    const category = extension === '.docx' ? 'document' : 'image';
    for (const [table, related] of [['company_profile', 'company_profile'], ['info_publications', 'info_publication']]) {
      sql += `\nINSERT INTO attachments (file_name, file_path, file_url, file_size, file_type, file_extension, category, related_type, related_id)\n` +
        `SELECT ${values([path.basename(asset.publicPath), asset.publicPath.slice(1), asset.publicPath])}, ${asset.size}, ${values([type, extension, category, related])}, source.id FROM ${table} source\n` +
        `WHERE source.is_deleted = 0 AND LOCATE(${escapeSql(asset.publicPath)}, source.content) > 0\n` +
        `AND NOT EXISTS (SELECT 1 FROM attachments a WHERE a.file_url = ${escapeSql(asset.publicPath)} AND a.related_type = '${related}' AND a.related_id = source.id AND a.is_deleted = 0);\n`;
    }
  }
  return sql;
}

if (require.main === module) {
  const args=process.argv.slice(2);
  const value = flag => args.includes(flag) ? args[args.indexOf(flag) + 1] : undefined;
  main(value('--source'), value('--baseline'));
}
module.exports = { cleanHtml, plain, hasContent, summaryFor, renderInfo, renderCompanyMedia, main };

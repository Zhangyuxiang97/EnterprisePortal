const { test } = require('node:test');
const assert = require('node:assert/strict');
const { cleanHtml, plain, hasContent, summaryFor } = require('../generate-access-initial-data');
const { renderSql, escapeSql } = require('../generate-initial-announcement-sql');

test('保留正文、表格与 PNG 内嵌图片，清除旧站失效引用和执行内容', () => {
  const missing=[];
  const html=cleanHtml('<p onclick="evil()">正文<strong>内容</strong></p><script>evil()</script>' +
    '<table><tr><td colspan="2">表格</td></tr></table>' +
    '<img src="/uploadfile/a.jpg"><img src="file:///C:/temp/a.png">' +
    '<img src="data:image/png;base64,aGVsbG8=">' +
    '<a href="/uploadfile/a.pdf">下载文件</a><a href="javascript:evil()">链接</a>' +
    '<span style="color:red;background-image:url(file:///a.png)">说明</span>', missing);
  assert.match(html, /正文<strong>内容/);
  assert.match(html, /<table>/);
  assert.match(html, /colspan="2"/);
  assert.match(html, /data:image\/png;base64/);
  assert.match(html, /下载文件（附件待补充）/);
  assert.doesNotMatch(html, /<script|onclick|javascript:|file:\/\/|url\(|src="\/uploadfile/);
  assert.equal(missing.length, 4);
});

test('SQL 按 UTF-8 字节分批，保留中文、单引号、反斜线与置顶', () => {
  const regions={byCode:new Map(),byLevelName:new Map(),byName:new Map()};
  const item={title:"中文'标题",content:'中'.repeat(600000),business_type:'GOV_PROCUREMENT',notice_type:'bidding',is_top:1};
  const sql=renderSql('bidding',[item,item,item],regions);
  const statements=sql.split(/;\n/).filter(s=>s.includes('INSERT INTO announcements'));
  assert.equal(statements.length,2);
  assert.ok(statements.every(s=>Buffer.byteLength(s)<4*1024*1024));
  assert.match(sql,/中文''标题/);
  assert.match(sql,/, 1, 1, 0, NOW\(\), NOW\(\)/);
  assert.equal(escapeSql("a\\b'c"),"'a\\\\b''c'");
});

test('Word 样式和脚本不进入摘要，段落、表格和实体转为可读文本', () => {
  const word = '<style>@font-face {font-family:宋体} p.MsoNormal{mso-style-name:正文}</style>' +
    '<script>bad()</script><p>第一段&nbsp;内容</p><p>第二段</p><table><tr><td>采购</td><td>服务</td></tr></table>';
  assert.equal(plain(word), '第一段 内容 第二段 采购 服务');
  assert.equal(summaryFor({ jianjie: '<b>明确摘要</b>', cleanedContent: word }), '明确摘要');
  assert.equal(summaryFor({ cleanedContent: word }), '第一段 内容 第二段 采购 服务');
});

test('仅已恢复媒体可以映射至本站路径，扫描件保留且空标签可识别', () => {
  const missing = [];
  const html = cleanHtml('<p><img src="/uploadfile/20190801163902403.jpg"><img src="/uploadfile/missing.jpg"></p>', missing);
  assert.match(html, /src="\/uploads\/legacy\/20190801163902403.jpg"/);
  assert.equal(missing.length, 1);
  assert.ok(hasContent(html));
  assert.equal(hasContent('<p>&nbsp;<br></p>'), false);
  assert.equal(summaryFor({ cleanedContent: html }), '原文为扫描件，点击查看完整内容。');
  assert.match(cleanHtml('<a href="/uploadfile/20190514143559600.doc">报名表</a>'), /\/uploads\/legacy\/20190514143559600.docx/);
});

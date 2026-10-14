const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { loadRegions, loadRegionDictionary, HENAN_DISTRICT_TO_CITY } = require('../initialization/region-dictionary');
const { renderRegions } = require('../initialization/generate-regions');
const { mergeFriendlyLinks } = require('../initialization/friendly-links');

test('共用地区源完整，河南区县能映射城市且格式化保留原始插入顺序', () => {
  const regions = loadRegionDictionary();
  assert.equal(regions.size, 2463);
  assert.equal(HENAN_DISTRICT_TO_CITY['中原区'], '郑州市');
  assert.equal(HENAN_DISTRICT_TO_CITY['二七区'], '郑州市');
  assert.equal(HENAN_DISTRICT_TO_CITY['巩义市'], '郑州市');
  assert.equal(loadRegions().byCode.get('410102').parentCode, '410100');
  assert.deepEqual([...loadRegionDictionary(renderRegions(regions))], [...regions]);
  assert.throws(() => loadRegionDictionary(''), /不能为空/);
  const duplicated = "('410000', '河南省', 1, NULL, 1),\n('410000', '河南省', 1, NULL, 1);";
  assert.throws(() => loadRegionDictionary(duplicated), /地区编码重复/);
});

test('公开友情链接集中到基础数据，重生成不会丢失基础链接或重复追加', () => {
  const base = "INSERT INTO friendly_links (name, url) VALUES ('基础链接', 'https://base.example');\n";
  const source = [
    { link_title: '基础链接', link_url: 'https://base.example' },
    { link_title: '公开链接', link_url: 'https://source.example', px_id: 2 },
    { link_title: '重复链接', link_url: 'https://source.example' }
  ];
  const first = mergeFriendlyLinks(base, source);
  assert.equal(first.addedCount, 1);
  assert.deepEqual(mergeFriendlyLinks(first.sql, source), first);
  assert.match(first.sql, /'公开链接', 'https:\/\/source.example', 12, 1/);
  const cleared = mergeFriendlyLinks(first.sql, []);
  assert.equal(cleared.addedCount, 0);
  assert.ok(cleared.sql.includes('https://base.example'));
  assert.ok(!cleared.sql.includes('https://source.example'));
});

test('八份首次初始化文件职责及执行顺序完整，附件关联在资讯之后执行', () => {
  const directory = path.join(__dirname, '../../SQL/init');
  assert.deepEqual(fs.readdirSync(directory).filter(f => f.endsWith('.sql')).sort(), [
    '01_schema.sql', '02_base_data.sql', '03_regions.sql', '04_announcements_bidding.sql',
    '05_announcements_result.sql', '06_announcements_correction.sql', '07_info_publications.sql', '08_company_media.sql'
  ]);
  const info = fs.readFileSync(path.join(directory, '07_info_publications.sql'), 'utf8');
  assert.doesNotMatch(info, /INSERT INTO\s+`?friendly_links/i);
  const media = fs.readFileSync(path.join(directory, '08_company_media.sql'), 'utf8');
  assert.match(media, /FROM info_publications source/);
});

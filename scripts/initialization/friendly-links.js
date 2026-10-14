const { escapeSql } = require('./generate-initial-announcement-sql');

/** 重新生成公开来源链接段，基础链接保持原样；重复生成不会追加重复记录。 */
function mergeFriendlyLinks(baseSql, sourceLinks) {
  const marker = /-- 公开来源友情链接\r?\n[\s\S]*?-- 公开来源友情链接结束\r?\n?/;
  const base = baseSql.replace(marker, '').trimEnd();
  const urls = new Set();
  const links = sourceLinks.filter(link => {
    if (!link.link_url || urls.has(link.link_url) || base.includes(escapeSql(link.link_url))) return false;
    urls.add(link.link_url);
    return true;
  });
  const block = links.length ? '-- 公开来源友情链接\nINSERT INTO friendly_links (name, url, sort_order, status) VALUES\n' +
    links.map(link => `(${escapeSql(link.link_title)}, ${escapeSql(link.link_url)}, ${10 + Number(link.px_id || 0)}, 1)`).join(',\n') +
    ';\n-- 公开来源友情链接结束\n' : '';
  return { sql: base + '\n\n' + block, addedCount: links.length };
}

module.exports = { mergeFriendlyLinks };

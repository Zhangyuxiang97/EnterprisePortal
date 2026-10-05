export const newsCategories = ['公司新闻', '行业动态', '通知公告', '知识资讯']
export const policyCategories = ['国家政策', '地方政策', '行业法规']
export function getCategoryStyle(category) {
  const styles = {
    '公司新闻': 'bg-blue-50 text-blue-700 border-blue-200',
    '行业动态': 'bg-orange-50 text-orange-700 border-orange-200',
    '通知公告': 'bg-emerald-50 text-emerald-700 border-emerald-200',
    '知识资讯': 'bg-indigo-50 text-indigo-700 border-indigo-200'
  }
  return styles[category] || 'bg-slate-50 text-slate-600 border-slate-200'
}

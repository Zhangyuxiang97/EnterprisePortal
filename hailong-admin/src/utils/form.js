import { formatDateTime } from './date.js'

export const localDateTime = (date = new Date()) => formatDateTime(date)
export const localDate = (date = new Date()) => localDateTime(date).slice(0, 10)

export function hasRichContent(html) {
  const content = String(html || '').replace(/<(script|style)\b[^>]*>[\s\S]*?<\/\1>/gi, '')
  if (/<(?:img|video|audio)\b[^>]*\bsrc\s*=\s*["']?[^\s"'>]+/i.test(content)) return true
  return !!content.replace(/<[^>]*>/g, '').replace(/&(?:nbsp|#160|#x0*a0);/gi, '').replace(/[\s\u200B-\u200D\uFEFF]/g, '')
}
export const richContentRule = {
  validator: (_rule, value, done) => done(hasRichContent(value) ? undefined : new Error('请输入正文或上传正文图片')),
  trigger: 'change'
}

// 保留匹配地区及祖先路径；父级匹配时展示其全部下级。
export function filterRegionTree(nodes, { name = '', code = '', level } = {}) {
  if (!name.trim() && !code.trim() && !level) return nodes
  return nodes.flatMap(node => {
    const matches = node.name.includes(name.trim()) && node.code.includes(code.trim()) && (!level || node.level === level)
    const children = filterRegionTree(node.children || [], { name, code, level })
    return matches || children.length ? [{ ...node, children: matches ? node.children : children }] : []
  })
}

export function certificateValidity(row, today = localDate()) {
  if (!row.expiryDate) return '未注明到期日'
  if (row.expiryDate.slice(0, 10) < today) return '已到期 · 历史资料'
  if (row.issueDate && row.issueDate.slice(0, 10) > today) return '尚未生效'
  return '有效期内'
}

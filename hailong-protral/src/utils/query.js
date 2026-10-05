export const queryText = value => typeof value === 'string' ? value : ''
export const queryPage = value => !Array.isArray(value) && /^\d+$/.test(String(value)) && Number.isSafeInteger(Number(value)) && Number(value) > 0 ? Number(value) : 1

export function readSavedQuery(key) {
  try {
    const value = JSON.parse(sessionStorage.getItem(key) || '{}')
    return value && !Array.isArray(value) && typeof value === 'object' ? value : {}
  } catch { return {} }
}

export function saveQuery(key, query) {
  try { sessionStorage.setItem(key, JSON.stringify(query)) } catch { /* 存储被禁用时仍可查询 */ }
}

export function sameQuery(left, right) {
  const normalize = value => JSON.stringify(Object.entries(value).map(([key, item]) => [key, String(item)]).sort())
  return normalize(left) === normalize(right)
}

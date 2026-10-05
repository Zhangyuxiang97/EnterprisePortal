const CACHE_KEY = 'hailong.portal-settings.v1'
const MAX_CACHE_AGE = 7 * 24 * 60 * 60 * 1000
const GROUPS = ['company', 'contact', 'transportation', 'faqs']

export const isSiteSettings = data => Boolean(
  data?.company && typeof data.company.fullName === 'string' && typeof data.company.slogan === 'string' &&
  typeof data.company.description === 'string' && typeof data.contact?.phone === 'string' &&
  typeof data.contact?.email === 'string' && typeof data.contact?.address?.fullAddress === 'string' &&
  data.contact?.workingHours && data.contact?.map && data.transportation?.metro &&
  Array.isArray(data.transportation.metro.lines) && data.transportation?.bus &&
  Array.isArray(data.transportation.bus.routes) && data.transportation?.driving &&
  Array.isArray(data.transportation.landmarks) && Array.isArray(data.faqs)
)

const displayOnly = data => Object.fromEntries(GROUPS.map(key => [key, data[key]]))

// 保持与 Vue、浏览器及请求库解耦，便于验证失败恢复和缓存行为。
export const createSiteSettingsLoader = ({ fetchSettings, applySettings, storage, schedule = setTimeout, now = Date.now }) => {
  let pending
  let loaded = false
  let attempts = 0
  let retryScheduled = false
  let nextAttemptAt = 0

  try {
    const cached = JSON.parse(storage?.getItem(CACHE_KEY) || 'null')
    if (cached && now() >= cached.savedAt && now() - cached.savedAt <= MAX_CACHE_AGE && isSiteSettings(cached.data))
      applySettings(displayOnly(cached.data))
  } catch {
    // 缓存损坏、禁用存储等情况使用构建默认内容。
  }

  const load = () => {
    if (pending) return pending
    if (loaded || attempts >= 3 || now() < nextAttemptAt) return Promise.resolve()
    attempts++
    pending = Promise.resolve().then(fetchSettings).then(response => {
      if (!response?.success || !isSiteSettings(response.data)) throw new Error('无效的门户配置')
      const data = displayOnly(response.data)
      applySettings(data)
      loaded = true
      try { storage?.setItem(CACHE_KEY, JSON.stringify({ savedAt: now(), data })) } catch { /* 存储失败不影响展示 */ }
    }).catch(() => {
      if (attempts < 3 && !retryScheduled) {
        const delay = attempts === 1 ? 500 : 1500
        nextAttemptAt = now() + delay
        retryScheduled = true
        schedule(() => {
          retryScheduled = false
          nextAttemptAt = 0
          load()
        }, delay)
      }
    }).finally(() => { pending = undefined })
    return pending
  }
  return { load }
}

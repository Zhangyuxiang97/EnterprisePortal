import axios from 'axios'
import { API_CONFIG } from '@/config/api.config'
import { tokenUtils } from '@/utils/auth'
import { session, saveSession, clearSession } from '@/utils/session'
import { notifyError } from '@/utils/errors'

const request = axios.create({ baseURL: API_CONFIG.baseURL, timeout: API_CONFIG.timeout })
const authTransport = axios.create({ baseURL: API_CONFIG.baseURL, timeout: API_CONFIG.timeout })
let refreshing = null

async function refreshSession(failedToken) {
  if (!refreshing) {
    const run = async () => {
      const latest = tokenUtils.getToken()
      if (latest && latest !== failedToken) { session.token = latest; return latest }
      const refreshToken = tokenUtils.getRefreshToken()
      if (!refreshToken) throw new Error('登录已过期，请重新登录')
      const generation = session.generation
      const { data } = await authTransport.post('/auth/refresh', { refreshToken })
      if (!data.success || generation !== session.generation) throw new Error('登录状态已改变，请重新登录')
      saveSession(data.data)
      return data.data.token
    }
    // 同页只刷新一次；支持 Web Locks 时，多标签页也按顺序轮换刷新令牌。
    refreshing = (navigator.locks ? navigator.locks.request('hailong-admin-refresh', run) : run())
      .finally(() => { refreshing = null })
  }
  return refreshing
}

request.interceptors.request.use(config => {
  const token = tokenUtils.getToken()
  if (token) config.headers.Authorization = 'Bearer ' + token
  else delete config.headers.Authorization
  return config
})

request.interceptors.response.use(response => {
  if (response.config.responseType === 'blob') return response.data
  const body = response.data
  if (body.success === false) {
    const error = new Error(body.message || '请求失败')
    error.response = response
    if (!response.config.silent) notifyError(error)
    return Promise.reject(error)
  }
  return body
}, async error => {
  const config = error.config || {}
  if (axios.isCancel(error)) return Promise.reject(error)
  const isLogin = config.url?.includes('/auth/login')
  if (error.response?.status === 401 && !isLogin && !config._retried) {
    config._retried = true
    try {
      const failedToken = config.headers?.Authorization?.replace(/^Bearer /, '')
      await refreshSession(failedToken)
      return request(config)
    } catch (refreshError) {
      // 网络不可达时保留编辑和认证信息，让用户重试；仅明确无效的会话清除认证。
      if (refreshError.response?.status === 401 || !tokenUtils.getRefreshToken()) {
        clearSession()
        error.message = '登录已过期，请重新登录；当前未保存的内容仍保留在页面中。'
        if (error.response?.data) error.response.data.message = error.message
      } else error = refreshError
    }
  } else if (error.response?.status === 401 && !isLogin) clearSession()
  if (!config.silent) notifyError(error)
  return Promise.reject(error)
})

export default request

import { ElMessage } from 'element-plus'

export function notifyError(error, fallback = '操作失败，请稍后重试') {
  if (error === 'cancel' || error === 'close' || error?.code === 'ERR_CANCELED' || error?._notified) return
  if (error && typeof error === 'object') error._notified = true
  const status = error?.response?.status
  const messages = { 401: '登录已过期，请重新登录', 403: '当前账号没有操作权限', 404: '内容不存在或已被删除', 409: '内容已发生变化，请重新读取后再试', 429: '操作过于频繁，请稍后重试', 500: '服务器处理失败，请稍后重试' }
  const message = error?.response?.data?.message || messages[status] ||
    (error?.isAxiosError ? (error.code === 'ECONNABORTED' ? '请求超时，请重试' : '网络连接失败，请检查网络后重试') : error?.message) || fallback
  ElMessage.error(message)
}

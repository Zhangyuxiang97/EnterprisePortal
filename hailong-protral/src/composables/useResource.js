import { ref, onScopeDispose } from 'vue'
import { createLatestRequest } from '../utils/latestRequest.js'

export function useResource(initialValue = null) {
  const data = ref(initialValue)
  const loading = ref(false)
  const error = ref('')
  const requests = createLatestRequest()
  onScopeDispose(requests.cancel)

  async function execute(loader) {
    const request = requests.begin()
    loading.value = true
    error.value = ''
    data.value = initialValue
    try {
      const value = await loader(request.signal)
      if (request.isCurrent()) data.value = value
      return request.isCurrent() ? value : undefined
    } catch (cause) {
      if (request.isCurrent()) {
        data.value = initialValue
        error.value = cause?.isAxiosError
          ? (cause.response?.status === 404 ? '内容不存在或已下架' : '暂时无法连接服务，请稍后重试')
          : cause?.message || '加载失败，请稍后重试'
      }
    } finally {
      if (request.isCurrent()) loading.value = false
    }
  }
  return { data, loading, error, execute, cancel: requests.cancel }
}

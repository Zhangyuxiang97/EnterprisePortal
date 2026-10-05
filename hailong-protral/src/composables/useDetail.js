import { watch } from 'vue'
import { useResource } from './useResource.js'

export function useDetail(id, fetchDetail, transform = value => value) {
  const resource = useResource()
  const reload = () => resource.execute(async signal => {
    const value = id()
    if (!value) throw new Error('内容不存在')
    const response = await fetchDetail(value, { signal })
    if (!response.success || !response.data) throw new Error(response.message || '内容不存在或暂时无法读取')
    return transform(response.data)
  })
  watch(id, reload, { immediate: true })
  return { ...resource, reload }
}

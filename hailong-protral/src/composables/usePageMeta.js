import { reactive, watchEffect } from 'vue'
import { useRoute } from 'vue-router'

export const pageMeta = reactive({ path: '', title: '', description: '', unavailable: false })

export function usePageMeta(source, enabled = () => true) {
  const route = useRoute()
  watchEffect(() => {
    if (enabled()) Object.assign(pageMeta, { path: route.path, title: '', description: '', unavailable: false, ...source() })
  })
}

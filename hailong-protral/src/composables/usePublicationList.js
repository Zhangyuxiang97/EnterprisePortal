import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter, onBeforeRouteLeave } from 'vue-router'
import { useResource } from './useResource.js'
import { queryText, queryPage, readSavedQuery, saveQuery, sameQuery } from '../utils/query.js'

export function usePublicationList(fetchList, routeName, cacheKey) {
  const route = useRoute()
  const router = useRouter()
  const keyword = ref('')
  const category = ref('')
  const currentPage = ref(1)
  const pageSize = ref(10)
  const resource = useResource({ items: [], totalCount: 0 })
  const items = computed(() => resource.data.value.items || [])
  const total = computed(() => resource.data.value.totalCount || 0)

  const loadItems = () => {
    // 重试以 URL 中已提交的条件为准，输入框中未提交的文字不影响当前结果。
    const params = { keyword: queryText(route.query.keyword), category: queryText(route.query.category), pageNumber: queryPage(route.query.page), pageSize: pageSize.value }
    return resource.execute(async signal => {
      const response = await fetchList(params, { signal })
      if (!response.success || !response.data) throw new Error('列表加载失败，请重试')
      return response.data
    })
  }
  const restore = () => {
    if (route.name !== routeName) return
    keyword.value = queryText(route.query.keyword)
    category.value = queryText(route.query.category)
    currentPage.value = queryPage(route.query.page)
    return loadItems()
  }
  const updateUrlQuery = () => {
    const query = {}
    if (keyword.value.trim()) query.keyword = keyword.value.trim()
    if (category.value) query.category = category.value
    if (currentPage.value > 1) query.page = String(currentPage.value)
    resource.cancel()
    if (sameQuery(query, route.query)) return loadItems()
    return router.replace({ query })
  }
  const handleSearch = () => { currentPage.value = 1; return updateUrlQuery() }
  const clearKeyword = () => { keyword.value = ''; return handleSearch() }
  const handlePageChange = page => {
    currentPage.value = page
    updateUrlQuery()
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
  watch(() => route.query, restore)
  onBeforeRouteLeave(() => { resource.cancel(); saveQuery(cacheKey, route.query) })
  onMounted(() => {
    const saved = readSavedQuery(cacheKey)
    if (!Object.keys(route.query).length && Object.keys(saved).length) router.replace({ query: saved })
    else restore()
  })
  return { keyword, category, currentPage, pageSize, items, total, loading: resource.loading, error: resource.error, loadItems, handleSearch, clearKeyword, handlePageChange }
}

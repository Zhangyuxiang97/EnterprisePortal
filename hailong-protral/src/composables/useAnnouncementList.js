import { ref, watch, onMounted, onScopeDispose } from 'vue'
import { useRoute, useRouter, onBeforeRouteLeave } from 'vue-router'
import { getAnnouncementList, getAnnouncementRegionOptions } from '@/api/announcement'
import { createLatestRequest } from '../utils/latestRequest.js'
import { queryText, queryPage, readSavedQuery, saveQuery, sameQuery } from '../utils/query.js'
import { getDateRange } from '../utils/date.js'

const emptyFilters = () => ({ businessType: '', noticeType: '', procurementType: '', province: '', city: '', district: '', timeRange: '', startDate: '', endDate: '', keyword: '' })
const emptyCounts = () => ({ provinceTotal: 0, cityTotal: 0, districtTotal: 0, provinceUnlocated: 0, cityUnlocated: 0, districtUnlocated: 0 })
const businessTypes = [{ label: '全部', value: '' }, { label: '政府采购', value: 'GOV_PROCUREMENT' }, { label: '建设工程', value: 'CONSTRUCTION' }]
const currentAnnouncementTypes = [{ label: '全部', value: '' }, { label: '招标/采购公告', value: 'bidding' }, { label: '更正公告', value: 'correction' }, { label: '结果公告', value: 'result' }]
const procurementTypes = [{ label: '全部', value: '' }, { label: '货物', value: 'goods' }, { label: '服务', value: 'service' }, { label: '工程', value: 'project' }]
const timeRanges = [{ label: '全部', value: '' }, { label: '当天', value: 'today' }, { label: '近三天', value: '3days' }, { label: '近一周', value: 'week' }, { label: '近一月', value: 'month' }]

export function useAnnouncementList() {
  const route = useRoute()
  const router = useRouter()
  const requests = createLatestRequest()
  const searchParams = ref(emptyFilters())
  const showCustomDatePicker = ref(false)
  const loading = ref(false)
  const error = ref('')
  const regionError = ref('')
  const validationError = ref('')
  const announcements = ref([])
  const provinces = ref([])
  const cities = ref([])
  const districts = ref([])
  const regionCounts = ref(emptyCounts())
  const total = ref(0)
  const currentPage = ref(1)
  const pageSize = ref(10)
  onScopeDispose(requests.cancel)

  const filtersFromQuery = query => {
    const filters = emptyFilters()
    for (const key of Object.keys(filters)) filters[key] = queryText(query[key === 'businessType' ? 'tab' : key])
    if (filters.businessType !== 'GOV_PROCUREMENT') filters.procurementType = ''
    return filters
  }
  const loadPageData = async () => {
    const request = requests.begin()
    const filters = filtersFromQuery(route.query)
    const params = { ...filters, pageNumber: queryPage(route.query.page), pageSize: pageSize.value }
    loading.value = true
    error.value = ''
    regionError.value = ''
    announcements.value = []
    total.value = 0
    provinces.value = []; cities.value = []; districts.value = []; regionCounts.value = emptyCounts()
    // 两个响应属于同一份已提交条件；旧请求不得回写地区、URL 或列表。
    const [regions, list] = await Promise.allSettled([
      getAnnouncementRegionOptions(params, { signal: request.signal }),
      getAnnouncementList(params, { signal: request.signal })
    ])
    if (!request.isCurrent()) return
    if (regions.status === 'fulfilled' && regions.value.success && regions.value.data) {
      const data = regions.value.data
      provinces.value = data.provinces || []; cities.value = data.cities || []; districts.value = data.districts || []
      for (const key of Object.keys(regionCounts.value)) regionCounts.value[key] = data[`${key}Count`] || 0
      const resolved = { province: data.selectedProvinceCode || '', city: data.selectedCityCode || '', district: data.selectedDistrictCode || '' }
      if (Object.keys(resolved).some(key => filters[key] !== resolved[key])) {
        const query = { ...route.query }
        for (const [key, value] of Object.entries(resolved)) { if (value) query[key] = value; else delete query[key] }
        delete query.page
        await router.replace({ query })
        return
      }
    } else regionError.value = '地区选项加载失败，当前列表仍按已选条件查询。'
    if (list.status === 'fulfilled' && list.value.success && list.value.data) {
      announcements.value = list.value.data.items || []
      total.value = list.value.data.totalCount || 0
    } else error.value = '公告加载失败，请检查网络后重试'
    loading.value = false
  }
  const updateUrlQuery = () => {
    validationError.value = ''
    const filters = searchParams.value
    if (filters.startDate && filters.endDate && filters.startDate > filters.endDate) {
      validationError.value = '开始日期不能晚于结束日期'
      return
    }
    const query = {}
    for (const [key, value] of Object.entries(filters)) {
      if (value) query[key === 'businessType' ? 'tab' : key] = key === 'keyword' ? value.trim() : value
    }
    if (currentPage.value > 1) query.page = String(currentPage.value)
    requests.cancel()
    if (sameQuery(query, route.query)) return loadPageData()
    return router.replace({ query })
  }
  const handleSearch = () => { currentPage.value = 1; return updateUrlQuery() }
  const clearKeyword = () => { searchParams.value.keyword = ''; return handleSearch() }
  const handleReset = () => { searchParams.value = emptyFilters(); showCustomDatePicker.value = false; return handleSearch() }
  const handleBusinessTypeChange = value => {
    searchParams.value.businessType = value
    if (value !== 'GOV_PROCUREMENT') searchParams.value.procurementType = ''
    return handleSearch()
  }
  const handleNoticeTypeChange = value => { searchParams.value.noticeType = value; return handleSearch() }
  const handleProcurementTypeChange = value => { searchParams.value.procurementType = value; return handleSearch() }
  const onProvinceChange = () => { searchParams.value.city = ''; searchParams.value.district = ''; return handleSearch() }
  const onCityChange = () => { searchParams.value.district = ''; return handleSearch() }
  const onDistrictChange = handleSearch
  const selectTimeRange = value => {
    Object.assign(searchParams.value, { timeRange: value }, getDateRange(value))
    showCustomDatePicker.value = false
    return handleSearch()
  }
  const onCustomDateChange = () => { searchParams.value.timeRange = 'custom'; return handleSearch() }
  const handlePageChange = page => { currentPage.value = page; updateUrlQuery(); window.scrollTo({ top: 0, behavior: 'smooth' }) }
  const restore = () => {
    if (route.name !== 'Announcements') return
    searchParams.value = filtersFromQuery(route.query)
    showCustomDatePicker.value = searchParams.value.timeRange === 'custom'
    currentPage.value = queryPage(route.query.page)
    return loadPageData()
  }
  watch(() => route.query, restore)
  onBeforeRouteLeave(() => { requests.cancel(); saveQuery('announcements_query', route.query) })
  onMounted(() => {
    const saved = readSavedQuery('announcements_query')
    if (!Object.keys(route.query).length && Object.keys(saved).length) router.replace({ query: saved })
    else restore()
  })
  return { searchParams, showCustomDatePicker, loading, error, regionError, validationError, announcements, provinces, cities, districts, regionCounts, total, currentPage, pageSize, businessTypes, currentAnnouncementTypes, procurementTypes, timeRanges, loadPageData, handleSearch, clearKeyword, handleReset, handleBusinessTypeChange, handleNoticeTypeChange, handleProcurementTypeChange, onProvinceChange, onCityChange, onDistrictChange, selectTimeRange, onCustomDateChange, handlePageChange }
}

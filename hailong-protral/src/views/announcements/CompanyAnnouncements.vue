<template>
  <div class="min-h-screen bg-slate-50/50">
    <Header />
    
    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <!-- 几何网格与光晕背景 -->
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          新闻中心
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">
          关注海隆最新动态，获取重要通知公告与企业人文发展资讯
        </p>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-10">
      <div class="container-wide">
        <div class="animate-fade-in">
          <!-- 搜索区域 -->
          <div class="bg-white rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.02)] border border-slate-100 p-6 mb-6">
            <div class="flex gap-3">
              <div class="relative flex-1">
                <svg class="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
                <input
                  v-model="keyword"
                  type="text"
                  placeholder="请输入您想检索的新闻标题关键字"
                  class="w-full pl-12 pr-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-sm hover:border-slate-300 bg-slate-50/50 focus:bg-white"
                  @keyup.enter="handleSearch"
                />
              </div>
              <button
                @click="handleSearch"
                class="px-8 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-xl hover:shadow-lg transition-all font-bold text-sm hover:-translate-y-0.5"
              >
                搜索
              </button>
              <button
                @click="handleReset"
                class="px-6 py-3 bg-slate-100 hover:bg-slate-200 text-slate-600 rounded-xl transition-all font-bold text-sm"
              >
                重置
              </button>
            </div>
          </div>

          <!-- 结果统计 -->
          <div class="mb-4 text-xs md:text-sm text-slate-500 font-medium">
            共找到 <span class="text-hailong-primary font-bold">{{ total }}</span> 条新闻动态
          </div>

          <!-- 状态：加载中 -->
          <div v-if="loading" class="text-center py-32 bg-white rounded-2xl shadow-sm border border-slate-100">
            <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
            <p class="mt-4 text-slate-400 text-sm">正在获取新闻列表数据...</p>
          </div>

          <!-- 状态：无结果 -->
          <div v-else-if="items.length === 0" class="text-center py-24 bg-white rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.02)] border border-slate-100">
            <div class="w-16 h-16 bg-slate-50 text-slate-300 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <p class="text-slate-500 font-bold mb-1">暂无相关新闻资讯</p>
            <p class="text-slate-400 text-xs">可以尝试更换其他词汇进行检索</p>
          </div>

          <!-- 新闻列表 -->
          <div v-else class="space-y-4">
            <div
              v-for="item in items"
              :key="item.id"
              @click="handleViewDetail(item.id)"
              class="group bg-white rounded-2xl p-5 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 cursor-pointer flex flex-col md:flex-row gap-5"
            >
              <!-- 封面缩略图 (仅在有封面时渲染) -->
              <div
                v-if="item.coverImage && item.coverImage.fileUrl"
                class="w-full md:w-44 h-28 flex-shrink-0 overflow-hidden rounded-xl bg-slate-50 border border-slate-100/50"
              >
                <img
                  :src="item.coverImage.fileUrl"
                  :alt="item.title"
                  class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                />
              </div>

              <!-- 新闻文本主体 -->
              <div class="flex-1 flex flex-col justify-between space-y-3 min-w-0">
                <div>
                  <div class="flex items-start justify-between gap-3 mb-2">
                    <h3 class="text-base md:text-lg font-bold text-slate-800 leading-snug group-hover:text-hailong-primary transition-colors flex-1 min-w-0 truncate md:whitespace-normal md:line-clamp-2">
                      <span v-if="item.isTop" class="inline-flex items-center bg-red-50 text-red-500 border border-red-100 text-[10px] font-extrabold px-1.5 py-0.5 rounded shadow-sm mr-2 align-middle">
                        置顶
                      </span>
                      {{ item.title }}
                    </h3>
                    
                    <span
                      v-if="item.category"
                      :class="[
                        'px-2.5 py-0.5 rounded-md text-[10px] font-bold whitespace-nowrap border self-start shadow-sm',
                        getTypeStyle(item.category)
                      ]"
                    >
                      {{ item.category }}
                    </span>
                  </div>

                  <!-- 摘要说明 -->
                  <p v-if="item.summary" class="text-slate-500 text-xs md:text-sm line-clamp-2 leading-relaxed">
                    {{ item.summary }}
                  </p>
                </div>

                <!-- 底部元数据栏 -->
                <div class="flex items-center justify-between pt-3 border-t border-slate-100/60 text-xs text-slate-400">
                  <div class="flex items-center gap-4">
                    <span class="flex items-center gap-1">
                      <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                      {{ formatDate(item.publishTime) }}
                    </span>
                    <span class="flex items-center gap-1">
                      <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                      </svg>
                      {{ item.viewCount || 0 }} 次浏览
                    </span>
                  </div>
                  
                  <span class="text-hailong-primary font-bold text-xs flex items-center gap-0.5 group-hover:translate-x-1 transition-all">
                    阅读全文
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                    </svg>
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- 精细分页器 -->
          <div v-if="total > 0" class="mt-12 flex items-center justify-center gap-2">
            <button
              @click="handlePageChange(currentPage - 1)"
              :disabled="currentPage === 1"
              :class="[
                'px-4 py-2 rounded-xl border text-sm font-semibold transition-all duration-200',
                currentPage === 1
                  ? 'border-slate-100 text-slate-300 cursor-not-allowed bg-slate-50/50'
                  : 'border-slate-200 text-slate-600 bg-white hover:bg-slate-50 hover:border-slate-300'
              ]"
            >
              上一页
            </button>

            <div class="flex gap-1.5">
              <button
                v-for="page in displayPages"
                :key="page"
                @click="page !== '...' && handlePageChange(page)"
                :class="[
                  'w-10 h-10 rounded-xl text-sm font-bold transition-all duration-200 flex items-center justify-center',
                  page === currentPage
                    ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-md shadow-blue-500/15'
                    : page === '...'
                    ? 'text-slate-400 cursor-default'
                    : 'text-slate-600 bg-white border border-slate-200 hover:bg-slate-50 hover:border-slate-300'
                ]"
              >
                {{ page }}
              </button>
            </div>

            <button
              @click="handlePageChange(currentPage + 1)"
              :disabled="currentPage === totalPages"
              :class="[
                'px-4 py-2 rounded-xl border text-sm font-semibold transition-all duration-200',
                currentPage === totalPages
                  ? 'border-slate-100 text-slate-300 cursor-not-allowed bg-slate-50/50'
                  : 'border-slate-200 text-slate-600 bg-white hover:bg-slate-50 hover:border-slate-300'
              ]"
            >
              下一页
            </button>
          </div>
        </div>
      </div>
    </div>

    <Footer />
  </div>
</template>
<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import { getCompanyNewsList } from '@/api/infoPublication'
import { formatDate } from '@/utils/date'

const router = useRouter()
const route = useRoute()

// 搜索关键字
const keyword = ref('')

// 列表数据
const items = ref([])

// 加载状态
const loading = ref(false)

// 分页参数
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)

// 总页数
const totalPages = computed(() => Math.ceil(total.value / pageSize.value))

// 显示的页码
const displayPages = computed(() => {
  const pages = []
  const maxDisplay = 7
  
  if (totalPages.value <= maxDisplay) {
    for (let i = 1; i <= totalPages.value; i++) {
      pages.push(i)
    }
  } else {
    if (currentPage.value <= 4) {
      for (let i = 1; i <= 5; i++) {
        pages.push(i)
      }
      pages.push('...')
      pages.push(totalPages.value)
    } else if (currentPage.value >= totalPages.value - 3) {
      pages.push(1)
      pages.push('...')
      for (let i = totalPages.value - 4; i <= totalPages.value; i++) {
        pages.push(i)
      }
    } else {
      pages.push(1)
      pages.push('...')
      for (let i = currentPage.value - 1; i <= currentPage.value + 1; i++) {
        pages.push(i)
      }
      pages.push('...')
      pages.push(totalPages.value)
    }
  }
  
  return pages
})

// 加载列表
const loadItems = async () => {
  loading.value = true
  try {
    const response = await getCompanyNewsList({
      keyword: keyword.value,
      pageNumber: currentPage.value,
      pageSize: pageSize.value
    })
    
    if (response.success) {
      items.value = response.data.items
      total.value = response.data.totalCount
    } else {
      console.error('加载列表失败:', response.message)
      items.value = []
      total.value = 0
    }
  } catch (error) {
    console.error('加载列表失败:', error)
    items.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

// 动态同步筛选条件至 URL Query
const updateUrlQuery = () => {
  const query = {}
  if (keyword.value) query.keyword = keyword.value
  if (currentPage.value > 1) query.page = currentPage.value

  router.replace({ query }).catch(err => {
    if (err.name !== 'NavigationDuplicated') console.error(err)
  })
}

// 搜索
const handleSearch = () => {
  currentPage.value = 1
  updateUrlQuery()
}

// 重置
const handleReset = () => {
  keyword.value = ''
  currentPage.value = 1
  updateUrlQuery()
}

// 页码变化
const handlePageChange = (page) => {
  currentPage.value = page
  updateUrlQuery()
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// 获取类型样式
const getTypeStyle = (category) => {
  const styles = {
    '企业动态': 'bg-blue-50/50 text-blue-600 border-blue-100/50',
    '通知公告': 'bg-emerald-50/50 text-emerald-600 border-emerald-100/50',
    '人事任免': 'bg-purple-50/50 text-purple-600 border-purple-100/50',
    '制度文件': 'bg-orange-50/50 text-orange-600 border-orange-100/50'
  }
  return styles[category] || 'bg-slate-50 text-slate-600 border-slate-100'
}

// 查看详情
const handleViewDetail = (id) => {
  router.push(`/detail/company-announcement/${id}`)
}

// 监听路由 Query 变化并重新加载数据
watch(
  () => route.query,
  async (newQuery) => {
    if (route.name !== 'CompanyAnnouncements') return
    keyword.value = newQuery.keyword || ''
    currentPage.value = parseInt(newQuery.page) || 1
    await loadItems()
  },
  { deep: true }
)

// 离开路由前将 query 暂存至 sessionStorage
import { onBeforeRouteLeave } from 'vue-router'
onBeforeRouteLeave((to, from) => {
  sessionStorage.setItem('company_announcements_query', JSON.stringify(route.query))
})

// 组件挂载时加载数据
onMounted(async () => {
  let query = route.query
  // 若 URL query 为空，尝试从 sessionStorage 恢复缓存的条件
  if (Object.keys(query).length === 0) {
    const savedQueryStr = sessionStorage.getItem('company_announcements_query')
    if (savedQueryStr) {
      try {
        const savedQuery = JSON.parse(savedQueryStr)
        if (Object.keys(savedQuery).length > 0) {
          router.replace({ query: savedQuery })
          return // 让 watch(route.query) 去接管数据加载流程
        }
      } catch (e) {
        console.error('解析新闻筛选缓存失败:', e)
      }
    }
  }

  keyword.value = query.keyword || ''
  currentPage.value = parseInt(query.page) || 1
  await loadItems()
})
</script>

<style scoped>
.container-wide {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>
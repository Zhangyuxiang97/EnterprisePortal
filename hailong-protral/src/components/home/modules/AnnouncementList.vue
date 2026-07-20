<template>
  <div class="py-24 bg-gradient-to-b from-white via-slate-50/50 to-blue-50/20">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-4xl md:text-5xl font-extrabold text-slate-800 mb-4 font-tech tracking-tight">最新公告信息</h2>
        <div class="w-16 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full"></div>
      </div>
      
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- 政府采购 -->
        <div class="space-y-6">
          <div class="flex items-center justify-between pb-3 border-b border-slate-100">
            <h3 class="text-xl font-bold text-slate-800 flex items-center gap-2">
              <span class="w-1.5 h-5 bg-hailong-primary rounded-full"></span>
              政府采购
            </h3>
            <router-link
              to="/announcements?tab=GOV_PROCUREMENT"
              class="text-hailong-primary hover:text-hailong-secondary transition-colors text-xs font-bold flex items-center gap-0.5"
            >
              查看全部
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </router-link>
          </div>
          
          <div v-if="loading" class="text-center py-12 text-slate-400 text-sm">正在加载公告数据...</div>
          <div v-else-if="govProcurementList.length === 0" class="text-center py-12 text-slate-400 text-sm bg-white rounded-2xl border border-slate-100">暂无政府采购公告</div>
          <div v-else class="space-y-4">
            <div
              v-for="announcement in govProcurementList"
              :key="announcement.id"
              @click="$emit('announcement-click', announcement.id)"
              class="group p-6 bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.01)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.05)] hover:-translate-y-0.5 transition-all duration-300 cursor-pointer border-l-4 border-l-hailong-primary flex flex-col justify-between"
            >
              <div class="flex justify-between items-center mb-3">
                <span :class="[
                  'px-2 py-0.5 rounded text-[10px] font-bold border',
                  announcement.noticeType === 'bidding' ? 'bg-blue-50 text-hailong-primary border-blue-100/30' :
                  announcement.noticeType === 'result' ? 'bg-emerald-50 text-emerald-600 border-emerald-100/30' :
                  announcement.noticeType === 'correction' ? 'bg-amber-50 text-amber-600 border-amber-100/30' :
                  'bg-slate-50 text-slate-600 border-slate-100'
                ]">
                  {{ announcement.noticeTypeName }}
                </span>
                <span class="text-xs text-slate-400 font-semibold">{{ formatDate(announcement.publishTime) }}</span>
              </div>
              
              <h4 class="text-base font-bold text-slate-800 group-hover:text-hailong-primary transition-colors mb-4 line-clamp-2 leading-snug">
                {{ announcement.title }}
              </h4>
              
              <div class="flex justify-between items-center text-xs text-slate-500 pt-3 border-t border-slate-50">
                <span>执行区域: <strong class="text-slate-700 font-bold">{{ announcement.projectRegion }}</strong></span>
                <span class="text-hailong-primary font-bold flex items-center gap-0.5 group-hover:translate-x-1 transition-all">
                  查看详情
                  <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- 建设工程 -->
        <div class="space-y-6">
          <div class="flex items-center justify-between pb-3 border-b border-slate-100">
            <h3 class="text-xl font-bold text-slate-800 flex items-center gap-2">
              <span class="w-1.5 h-5 bg-hailong-secondary rounded-full"></span>
              建设工程
            </h3>
            <router-link
              to="/announcements?tab=CONSTRUCTION"
              class="text-hailong-primary hover:text-hailong-secondary transition-colors text-xs font-bold flex items-center gap-0.5"
            >
              查看全部
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </router-link>
          </div>
          
          <div v-if="loading" class="text-center py-12 text-slate-400 text-sm">正在加载公告数据...</div>
          <div v-else-if="constructionList.length === 0" class="text-center py-12 text-slate-400 text-sm bg-white rounded-2xl border border-slate-100">暂无建设工程公告</div>
          <div v-else class="space-y-4">
            <div
              v-for="announcement in constructionList"
              :key="announcement.id"
              @click="$emit('announcement-click', announcement.id)"
              class="group p-6 bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.01)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.05)] hover:-translate-y-0.5 transition-all duration-300 cursor-pointer border-l-4 border-l-hailong-secondary flex flex-col justify-between"
            >
              <div class="flex justify-between items-center mb-3">
                <span :class="[
                  'px-2 py-0.5 rounded text-[10px] font-bold border',
                  announcement.noticeType === 'bidding' ? 'bg-blue-50 text-hailong-primary border-blue-100/30' :
                  announcement.noticeType === 'result' ? 'bg-emerald-50 text-emerald-600 border-emerald-100/30' :
                  announcement.noticeType === 'correction' ? 'bg-amber-50 text-amber-600 border-amber-100/30' :
                  'bg-slate-50 text-slate-600 border-slate-100'
                ]">
                  {{ announcement.noticeTypeName }}
                </span>
                <span class="text-xs text-slate-400 font-semibold">{{ formatDate(announcement.publishTime) }}</span>
              </div>
              
              <h4 class="text-base font-bold text-slate-800 group-hover:text-hailong-primary transition-colors mb-4 line-clamp-2 leading-snug">
                {{ announcement.title }}
              </h4>
              
              <div class="flex justify-between items-center text-xs text-slate-500 pt-3 border-t border-slate-50">
                <span>执行区域: <strong class="text-slate-700 font-bold">{{ announcement.projectRegion }}</strong></span>
                <span class="text-hailong-primary font-bold flex items-center gap-0.5 group-hover:translate-x-1 transition-all">
                  查看详情
                  <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getRecentAnnouncements } from '@/api/home'

defineEmits(['announcement-click'])

const loading = ref(false)
const govProcurementList = ref([])
const constructionList = ref([])

const loadRecentAnnouncements = async () => {
  loading.value = true
  try {
    const response = await getRecentAnnouncements()
    if (response.success && response.data) {
      govProcurementList.value = response.data
        .filter(item => item.sourceType === '政府采购')
        .slice(0, 3)
      constructionList.value = response.data
        .filter(item => item.sourceType === '建设工程')
        .slice(0, 3)
    }
  } catch (error) {
    console.error('加载最新公告失败:', error)
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', { year: 'numeric', month: '2-digit', day: '2-digit' })
}

onMounted(() => {
  loadRecentAnnouncements()
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden
}
</style>

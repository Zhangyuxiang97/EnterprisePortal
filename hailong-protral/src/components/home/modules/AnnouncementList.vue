<template>
  <div class="py-24 bg-gradient-to-b from-white to-blue-50/30">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-5xl font-bold text-hailong-dark mb-4 font-tech">公告信息</h2>
        <div class="w-24 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto"></div>
      </div>
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- 政府采购 -->
        <div>
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-2xl font-bold text-hailong-dark">政府采购</h3>
            <router-link to="/announcements?tab=GOV_PROCUREMENT"
              class="text-hailong-primary hover:underline text-sm">查看全部 →</router-link>
          </div>
          <div v-if="loading" class="text-center py-8 text-gray-500">加载中...</div>
          <div v-else-if="govProcurementList.length === 0" class="text-center py-8 text-gray-500">暂无公告</div>
          <div v-else class="space-y-4">
            <div v-for="announcement in govProcurementList" :key="announcement.id"
              @click="$emit('announcement-click', announcement.hashId || announcement.id)"
              class="p-6 bg-white rounded-xl hover:shadow-lg transition-all cursor-pointer border-l-4 border-hailong-primary">
              <div class="flex justify-between items-start mb-3">
                <span class="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-xs">{{ announcement.noticeTypeName }}</span>
                <span class="text-xs text-gray-500">{{ formatDate(announcement.publishTime) }}</span>
              </div>
              <h4 class="text-lg font-bold text-gray-900 mb-2 line-clamp-2">{{ announcement.title }}</h4>
              <div class="flex justify-between items-center text-sm">
                <span class="text-gray-600">区域: <strong class="text-hailong-primary">{{ announcement.projectRegion }}</strong></span>
              </div>
            </div>
          </div>
        </div>

        <!-- 建设工程 -->
        <div>
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-2xl font-bold text-hailong-dark">建设工程</h3>
            <router-link to="/announcements?tab=CONSTRUCTION"
              class="text-hailong-primary hover:underline text-sm">查看全部 →</router-link>
          </div>
          <div v-if="loading" class="text-center py-8 text-gray-500">加载中...</div>
          <div v-else-if="constructionList.length === 0" class="text-center py-8 text-gray-500">暂无公告</div>
          <div v-else class="space-y-4">
            <div v-for="announcement in constructionList" :key="announcement.id"
              @click="$emit('announcement-click', announcement.hashId || announcement.id)"
              class="p-6 bg-white rounded-xl hover:shadow-lg transition-all cursor-pointer border-l-4 border-hailong-secondary">
              <div class="flex justify-between items-start mb-3">
                <span class="px-3 py-1 bg-green-100 text-green-800 rounded-full text-xs">{{ announcement.noticeTypeName }}</span>
                <span class="text-xs text-gray-500">{{ formatDate(announcement.publishTime) }}</span>
              </div>
              <h4 class="text-lg font-bold text-gray-900 mb-2 line-clamp-2">{{ announcement.title }}</h4>
              <div class="flex justify-between items-center text-sm">
                <span class="text-gray-600">区域: <strong class="text-hailong-secondary">{{ announcement.projectRegion }}</strong></span>
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

<template>
  <div class="min-h-screen bg-slate-50/50">
    
    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <!-- 几何网格与光晕背景 -->
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>

      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          公告信息
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">
          招标采购信息
        </p>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-16 bg-white">
      <div class="container-wide">
        <div class="animate-fade-in">
      <!-- 搜索筛选区域 -->
      <AnnouncementFilters :controller="controller" />
      <p v-if="validationError" role="alert" class="mb-4 text-sm text-red-600">{{ validationError }}</p>
      <div v-if="regionError" role="alert" class="mb-4 flex flex-wrap items-center gap-3 rounded-lg bg-amber-50 p-3 text-sm text-amber-800">
        {{ regionError }} <button @click="loadPageData" class="underline">重试地区选项</button>
      </div>

      <!-- 结果统计 -->
      <div v-if="!loading && !error" class="mb-4 text-xs md:text-sm text-slate-500 font-medium">
        共找到 <span class="text-hailong-primary font-bold">{{ total }}</span> 条招投标公告
      </div>

      <!-- 公告列表 -->
      <AsyncState v-if="loading || error || !announcements.length" :loading="loading" :error="error" empty-text="暂无相关公告，可以尝试调整筛选条件。" @retry="loadPageData" />

      <div v-else class="space-y-4">
        <AnnouncementCard v-for="announcement in announcements" :key="announcement.id" :announcement="announcement" @open="handleViewDetail" />
      </div>

      <!-- 精细圆角分页器 -->
      <Pagination v-if="!loading && !error" :total="total" :page="currentPage" :page-size="pageSize" @change="handlePageChange" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, toRefs } from 'vue'
import { useRouter } from 'vue-router'
import { useAnnouncementList } from '@/composables/useAnnouncementList'
import AnnouncementFilters from '@/components/announcements/AnnouncementFilters.vue'
import AnnouncementCard from '@/components/announcements/AnnouncementCard.vue'
import AsyncState from '@/components/common/AsyncState.vue'
import Pagination from '@/components/common/Pagination.vue'
const router = useRouter()
const controller = reactive(useAnnouncementList())
const { loading, error, regionError, validationError, announcements, total, currentPage, pageSize, loadPageData, handlePageChange } = toRefs(controller)
const handleViewDetail = id => router.push(`/announcement/${id}`)
</script>

<style scoped>
.container-wide {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
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

<template>
  <div class="min-h-screen bg-slate-50/50">
    
    <!-- 页面头部 Hero Banner -->
    <PageBanner title="公告信息" description="招标采购信息" />

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
import PageBanner from '@/components/common/PageBanner.vue'
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

</style>

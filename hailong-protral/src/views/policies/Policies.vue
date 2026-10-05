<template>
  <div class="min-h-screen bg-slate-50/50">
    
    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <!-- 几何网格与光晕背景 -->
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          政策法规
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">
          汇聚国家、地方关于招投标与工程咨询的最新政策规章与法律条文
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
              <div class="relative flex-1 group">
                <svg class="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
                <input
                  v-model="keyword"
                  type="text"
                  placeholder="请输入您想检索的政策法规标题关键字"
                  class="w-full pl-12 pr-10 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-sm hover:border-slate-300 bg-slate-50/50 focus:bg-white"
                  @keyup.enter="handleSearch"
                />
                <!-- 一键清除按钮 -->
                <button
                  v-if="keyword"
                  @click="clearKeyword"
                  class="absolute right-3.5 top-1/2 transform -translate-y-1/2 text-slate-400 hover:text-slate-600 transition-colors focus:outline-none"
                  title="清除关键字"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
              <button
                @click="handleSearch"
                class="portal-button portal-button--primary px-8 py-3 text-sm"
              >
                搜索
              </button>
            </div>
          </div>

          <div class="mb-6 flex flex-wrap gap-2" aria-label="内容分类">
            <button v-for="name in ['', ...categories]" :key="name" @click="category = name; handleSearch()" :aria-pressed="category === name" class="portal-choice px-4 py-2 text-sm" :class="{ 'portal-choice--selected': category === name }">{{ name || '全部' }}</button>
          </div>
          <!-- 结果统计 -->
          <div v-if="!loading && !error" class="mb-4 text-xs md:text-sm text-slate-500 font-medium">
            共找到 <span class="text-hailong-primary font-bold">{{ total }}</span> 条政策法规
          </div>

          <!-- 状态：加载中 -->
          <AsyncState v-if="loading || error || !items.length" :loading="loading" :error="error" empty-text="暂无相关政策法规" @retry="loadItems" />

          <div v-else class="space-y-4">
            <div
              v-for="item in items"
              :key="item.id"
              @click="handleViewDetail(item.id)" @keydown.enter="handleViewDetail(item.id)" role="link" tabindex="0"
              class="group bg-white rounded-2xl p-5 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 cursor-pointer flex flex-col gap-4"
            >
              <!-- 标题和类别 -->
              <div class="flex items-start justify-between gap-3">
                <h3 class="text-base md:text-lg font-bold text-slate-800 leading-snug group-hover:text-hailong-primary transition-colors flex-1 min-w-0 line-clamp-2">
                  <span v-if="item.isTop" class="inline-flex items-center bg-red-50 text-red-500 border border-red-100 text-[10px] font-extrabold px-1.5 py-0.5 rounded shadow-sm mr-2 align-middle">
                    置顶
                  </span>
                  {{ item.title }}
                </h3>
                
                <span
                  v-if="item.category"
                  class="px-2.5 py-0.5 rounded-md text-[10px] font-bold whitespace-nowrap bg-blue-50/50 text-blue-600 border border-blue-100/50 shadow-sm self-start"
                >
                  {{ item.category }}
                </span>
              </div>

              <!-- 摘要说明 -->
              <p v-if="item.summary" class="text-slate-500 text-xs md:text-sm line-clamp-2 leading-relaxed">
                {{ item.summary }}
              </p>

              <!-- 底部元数据信息 -->
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
                  <span
                    v-if="item.attachments && item.attachments.length > 0"
                    class="flex items-center gap-1 px-1.5 py-0.5 rounded bg-slate-50 text-slate-500 border border-slate-100 text-[10px] font-semibold"
                  >
                    <svg class="w-3.5 h-3.5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                    </svg>
                    含 {{ item.attachments.length }} 个附件
                  </span>
                </div>
                
                <span class="text-hailong-primary font-bold text-xs flex items-center gap-0.5 group-hover:translate-x-1 transition-all">
                  查看详情
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </span>
              </div>
            </div>
          </div>

          <!-- 精细圆角分页器 -->
          <Pagination v-if="!loading && !error" :total="total" :page="currentPage" :page-size="pageSize" @change="handlePageChange" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { getPolicyRegulationsList } from '@/api/infoPublication'
import { usePublicationList } from '@/composables/usePublicationList'
import { formatDate } from '@/utils/date'
import { policyCategories as categories, getCategoryStyle as getTypeStyle } from '@/utils/categories'
import AsyncState from '@/components/common/AsyncState.vue'
import Pagination from '@/components/common/Pagination.vue'
const router = useRouter()
const { keyword, category, items, loading, error, currentPage, pageSize, total, loadItems, handleSearch, clearKeyword, handlePageChange } = usePublicationList(getPolicyRegulationsList, 'Policies', 'policies_query')
const handleViewDetail = id => router.push(`/policy/${id}`)
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
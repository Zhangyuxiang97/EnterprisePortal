<template>
  <div class="min-h-screen bg-gray-50" :class="{ 'pt-20': !embedded }">
    
    <!-- 内容区域 -->
    <div class="pt-8 pb-12 bg-white">
      <div class="container-wide">
        <DetailBreadcrumb
          class="max-w-5xl mx-auto mb-4"
          section-label="公告信息"
          section-to="/announcements"
          current-label="公告详情"
          :embedded="embedded"
          @back="emit('back')"
        />
        <div v-if="loading" class="text-center py-20">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
          <p class="mt-4 text-gray-500">加载中...</p>
        </div>

        <div v-else-if="error" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-red-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-gray-500 mb-4">{{ error }}</p>
          <button @click="loadAnnouncementDetail" class="mt-4 rounded-lg bg-hailong-primary px-5 py-2 text-white">重新加载</button>
          <router-link v-if="!embedded" to="/announcements" class="text-hailong-primary hover:underline">返回列表</router-link>
          <button v-else @click="emit('back')" class="text-hailong-primary hover:underline">返回列表</button>
        </div>

        <div v-else-if="!announcement" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p class="text-gray-500 mb-4">公告不存在或已被删除</p>
          <router-link v-if="!embedded" to="/announcements" class="text-hailong-primary hover:underline">返回列表</router-link>
          <button v-else @click="emit('back')" class="text-hailong-primary hover:underline">返回列表</button>
        </div>

        <div v-else class="max-w-5xl mx-auto">
          <!-- 公告头部 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <!-- 标题 -->
            <h1 class="text-3xl font-bold text-gray-900 mb-6 leading-tight">
              {{ announcement.title }}
            </h1>

            <!-- 元信息 -->
            <div class="flex flex-wrap items-center gap-6 pb-6 border-b border-gray-200">
              <div class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                </svg>
                <span class="font-medium">类型：</span>
                <span :class="getTypeStyle(announcement.noticeType)" class="px-3 py-1 rounded-full text-xs font-medium">
                  {{ getNoticeTypeLabel(announcement.noticeType) }}
                </span>
              </div>

              <div v-if="announcement.businessType" class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                </svg>
                <span class="font-medium">业务类型：</span>
                <span>{{ getBusinessTypeLabel(announcement.businessType) }}</span>
              </div>

              <div class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <span class="font-medium">发布时间：</span>
                <span>{{ formatDate(announcement.publishTime) }}</span>
              </div>

              <div class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                <span class="font-medium">浏览：</span>
                <span>{{ announcement.viewCount || 0 }} 次</span>
              </div>
            </div>

            <!-- 基本信息 -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">
              <div v-if="announcement.bidder" class="flex items-start gap-3">
                <div class="flex-shrink-0 w-10 h-10 bg-hailong-primary/10 rounded-lg flex items-center justify-center">
                  <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                  </svg>
                </div>
                <div class="flex-1">
                  <div class="text-sm text-gray-500 mb-1">招标人</div>
                  <div class="text-gray-900 font-medium">{{ announcement.bidder }}</div>
                </div>
              </div>

              <div v-if="announcement.winner" class="flex items-start gap-3">
                <div class="flex-shrink-0 w-10 h-10 bg-hailong-secondary/10 rounded-lg flex items-center justify-center">
                  <svg class="w-5 h-5 text-hailong-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                  </svg>
                </div>
                <div class="flex-1">
                  <div class="text-sm text-gray-500 mb-1">中标人</div>
                  <div class="text-gray-900 font-medium">{{ announcement.winner }}</div>
                </div>
              </div>

              <div v-if="announcement.province || announcement.city || announcement.district" class="flex items-start gap-3">
                <div class="flex-shrink-0 w-10 h-10 bg-hailong-cyan/10 rounded-lg flex items-center justify-center">
                  <svg class="w-5 h-5 text-hailong-cyan" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                </div>
                <div class="flex-1">
                  <div class="text-sm text-gray-500 mb-1">项目区域</div>
                  <div class="text-gray-900 font-medium">{{ getRegionText(announcement) }}</div>
                </div>
              </div>

              <div v-if="announcement.budgetAmount != null" class="flex items-start gap-3">
                <div class="flex-shrink-0 w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center">
                  <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div class="flex-1">
                  <div class="text-sm text-gray-500 mb-1">预算金额</div>
                  <div class="text-gray-900 font-medium">¥{{ announcement.budgetAmount }}万元</div>
                </div>
              </div>
              <div v-if="announcement.awardAmount != null" class="flex items-start gap-3">
                <div class="flex-shrink-0 w-10 h-10 bg-amber-100 rounded-lg flex items-center justify-center">
                  <svg class="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div class="flex-1">
                  <div class="text-sm text-gray-500 mb-1">中标 / 成交金额</div>
                  <div class="text-gray-900 font-medium">¥{{ announcement.awardAmount }}万元</div>
                </div>
              </div>
            </div>
          </div>

          <!-- 公告内容 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <h2 class="text-xl font-bold text-gray-900 mb-6 pb-4 border-b border-gray-200">
              公告内容
            </h2>
            <RichTextContent class="prose prose-lg max-w-none text-gray-700 leading-relaxed" :content="announcement.content" />
          </div>

          <!-- 附件下载 -->
          <AttachmentList :attachments="announcement.attachments" @preview="handlePreview" />

          <!-- 操作按钮 -->
          <div class="no-print flex flex-wrap items-center justify-between gap-3">
            <button
              @click="goBack"
              class="whitespace-nowrap px-4 sm:px-6 py-3 portal-button portal-button--secondary gap-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
              返回
            </button>

            <button
              @click="handlePrint"
              class="portal-button portal-button--primary no-print whitespace-nowrap px-4 sm:px-6 py-3 flex items-center gap-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" />
              </svg>
              打印公告
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- 附件预览组件 -->
    <AttachmentPreview
      :visible="previewVisible"
      :attachment="currentAttachment"
      @close="previewVisible = false"
    />
  </div>
</template>

<script setup>
import DetailBreadcrumb from '@/components/common/DetailBreadcrumb.vue'
import AttachmentList from "@/components/common/AttachmentList.vue"
import { useDetail } from "@/composables/useDetail"
import { usePageMeta } from "@/composables/usePageMeta"
import RichTextContent from "@/components/common/RichTextContent.vue"
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAnnouncementByHash } from '@/api/announcement'
import AttachmentPreview from '@/components/AttachmentPreview.vue'

const props = defineProps({
  hashId: { type: String, default: null },
  embedded: { type: Boolean, default: false }
})
const emit = defineEmits(['back'])

const route = useRoute()
const router = useRouter()

// 公告数据

// 附件预览
const previewVisible = ref(false)
const currentAttachment = ref({})

// 返回上一页
const goBack = () => {
  if (props.hashId && props.embedded) {
    emit('back')
  } else {
    if (window.history.length <= 1) {
      router.push('/announcements')
    } else {
      router.back()
    }
  }
}

// 获取公告类型标签
const getNoticeTypeLabel = (type) => {
  const typeMap = {
    'bidding': '招标公告',
    'correction': '更正公告',
    'result': '结果公告'
  }
  return typeMap[type] || type
}

// 获取业务类型标签
const getBusinessTypeLabel = (type) => {
  const typeMap = {
    'GOV_PROCUREMENT': '政府采购',
    'CONSTRUCTION': '建设工程'
  }
  return typeMap[type] || type
}

// 获取类型样式
const getTypeStyle = (type) => {
  switch (type) {
    case 'bidding':
      return 'bg-hailong-primary/10 text-hailong-primary border border-hailong-primary/20'
    case 'result':
      return 'bg-hailong-secondary/10 text-hailong-secondary border border-hailong-secondary/20'
    case 'correction':
      return 'bg-hailong-cyan/10 text-hailong-cyan border border-hailong-cyan/20'
    default:
      return 'bg-gray-100 text-gray-800 border border-gray-200'
  }
}

// 获取区域文本
const getRegionText = (announcement) => {
  const parts = []
  if (announcement.province) parts.push(announcement.province)
  if (announcement.city) parts.push(announcement.city)
  if (announcement.district) parts.push(announcement.district)
  return parts.join(' ')
}

import { formatDate } from '@/utils/date'


// 预览附件
const handlePreview = (attachment) => {
  currentAttachment.value = attachment
  previewVisible.value = true
}

// 打印公告
const handlePrint = () => {
  window.print()
}

// 加载公告详情
const { data: announcement, loading, error, reload: loadAnnouncementDetail } = useDetail(
  () => props.hashId || route.params.id, getAnnouncementByHash, value => {
  return value
})
usePageMeta(() => ({ title: announcement.value?.title || announcement.value?.name, description: announcement.value?.summary || announcement.value?.description, unavailable: Boolean(error.value) }), () => !props.embedded)
watch(() => props.hashId || route.params.id, () => { previewVisible.value = false; currentAttachment.value = {} })
</script>

<style scoped>
/* 表格样式 */
@media print {
  header, footer, .no-print {
    display: none !important;
  }
}
</style>

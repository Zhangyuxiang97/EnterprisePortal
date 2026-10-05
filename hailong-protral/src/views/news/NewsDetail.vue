<template>
  <div class="min-h-screen bg-gray-50 pt-20">
    
    <!-- 内容区域 -->
    <div class="pt-8 pb-12 bg-white">
      <div class="container-wide">
        <DetailBreadcrumb
          class="max-w-5xl mx-auto mb-4"
          section-label="新闻中心"
          section-to="/news"
          current-label="新闻详情"
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
          <button @click="loadItemDetail" class="mt-4 rounded-lg bg-hailong-primary px-5 py-2 text-white">重新加载</button>
          <router-link to="/news" class="text-hailong-primary hover:underline">返回列表</router-link>
        </div>

        <div v-else-if="!item" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p class="text-gray-500 mb-4">新闻不存在或已被删除</p>
          <router-link to="/news" class="text-hailong-primary hover:underline">返回列表</router-link>
        </div>

        <div v-else class="max-w-5xl mx-auto">
          <!-- 文章头部 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <!-- 置顶标签 -->
            <div v-if="item.isTop" class="mb-4">
              <span class="inline-flex items-center gap-1 px-3 py-1 bg-red-500 text-white text-sm rounded-full font-medium">
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M2 6a2 2 0 012-2h6a2 2 0 012 2v8a2 2 0 01-2 2H4a2 2 0 01-2-2V6zM14.553 7.106A1 1 0 0014 8v4a1 1 0 00.553.894l2 1A1 1 0 0018 13V7a1 1 0 00-1.447-.894l-2 1z" />
                </svg>
                置顶
              </span>
            </div>

            <!-- 标题 -->
            <h1 class="text-3xl font-bold text-gray-900 mb-6 leading-tight">
              {{ item.title }}
            </h1>

            <!-- 元信息 -->
            <div class="flex flex-wrap items-center gap-6 pb-6 border-b border-gray-200">
              <div class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                </svg>
                <span class="font-medium">分类：</span>
                <span :class="getTypeStyle(item.type)" class="px-3 py-1 rounded-full text-xs font-medium">
                  {{ item.type }}
                </span>
              </div>

              <div class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <span class="font-medium">发布时间：</span>
                <span>{{ item.publishDate }}</span>
              </div>

              <div v-if="item.author" class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
                <span class="font-medium">作者：</span>
                <span>{{ item.author }}</span>
              </div>

              <div class="flex items-center gap-2 text-sm text-gray-600">
                <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                <span class="font-medium">浏览：</span>
                <span>{{ item.views || 0 }} 次</span>
              </div>
            </div>

            <!-- 摘要 -->
            <div v-if="item.summary" class="mt-6 p-4 bg-gradient-to-r from-hailong-primary/5 to-hailong-secondary/5 rounded-lg border-l-4 border-hailong-primary">
              <div class="flex items-start gap-3">
                <svg class="w-5 h-5 text-hailong-primary flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p class="text-gray-700 leading-relaxed">{{ item.summary }}</p>
              </div>
            </div>
          </div>

          <!-- 封面图片 -->
          <div v-if="coverImageUrl" class="bg-white rounded-xl shadow-sm overflow-hidden mb-6">
            <img :src="coverImageUrl" :alt="item.title" class="w-full h-96 object-cover" />
          </div>

          <!-- 文章内容 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <RichTextContent class="prose prose-lg max-w-none text-gray-700 leading-relaxed" :content="item.content" />
          </div>

          <!-- 标签 -->
          <div v-if="item.tags && item.tags.length > 0" class="bg-white rounded-xl shadow-sm p-6 mb-6">
            <div class="flex items-center gap-3 flex-wrap">
              <span class="text-sm font-medium text-gray-600">标签：</span>
              <span
                v-for="tag in item.tags"
                :key="tag"
                class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm hover:bg-hailong-primary/10 hover:text-hailong-primary transition-colors cursor-pointer"
              >
                # {{ tag }}
              </span>
            </div>
          </div>

          <!-- 附件列表 -->
          <AttachmentList :attachments="item.attachments" @preview="handlePreview" />

          <!-- 相关新闻 -->
          <div v-if="relatedItems.length > 0" class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <h2 class="text-xl font-bold text-gray-900 mb-6 pb-4 border-b border-gray-200">
              相关新闻
            </h2>
            <div class="space-y-4">
              <router-link
                v-for="related in relatedItems"
                :key="related.id"
                :to="`/news/${related.id}`"
                class="flex items-start gap-4 p-4 rounded-lg hover:bg-gray-50 transition-colors group"
              >
                <div class="flex-shrink-0 w-2 h-2 bg-hailong-primary rounded-full mt-2"></div>
                <div class="flex-1">
                  <h3 class="text-gray-900 font-medium group-hover:text-hailong-primary transition-colors line-clamp-2">
                    {{ related.title }}
                  </h3>
                  <div class="flex items-center gap-4 mt-2 text-xs text-gray-500">
                    <span>{{ related.publishDate }}</span>
                    <span>{{ related.views }} 次浏览</span>
                  </div>
                </div>
              </router-link>
            </div>
          </div>

          <!-- 操作按钮 -->
          <div class="no-print flex flex-wrap items-center justify-between gap-3">
            <router-link
              to="/news"
              class="whitespace-nowrap px-4 sm:px-6 py-3 portal-button portal-button--secondary gap-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
              返回列表
            </router-link>

            <div class="flex items-center gap-3">
              <button
                @click="handleShare"
                class="whitespace-nowrap px-4 sm:px-6 py-3 portal-button portal-button--secondary gap-2"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                </svg>
                分享
              </button>

              <button
                @click="handlePrint"
                class="portal-button portal-button--primary no-print whitespace-nowrap px-4 sm:px-6 py-3 flex items-center gap-2"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" />
                </svg>
                打印
              </button>
            </div>
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
import { getCategoryStyle as getTypeStyle } from "@/utils/categories"
import { useDetail } from "@/composables/useDetail"
import { usePageMeta } from "@/composables/usePageMeta"
import RichTextContent from "@/components/common/RichTextContent.vue"
import { ref, computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { getInfoPublicationDetail } from '@/api/infoPublication'
import AttachmentPreview from '@/components/AttachmentPreview.vue'

const route = useRoute()

// 新闻数据
const relatedItems = ref([])

// 附件预览
const previewVisible = ref(false)
const currentAttachment = ref({})

// 格式化日期
const formatDate = (date) => {
  if (!date) return '-'
  try {
    const dateObj = new Date(date)
    return dateObj.toLocaleDateString('zh-CN')
  } catch (e) {
    return date
  }
}

// 获取封面图片URL
const coverImageUrl = computed(() => {
  if (item.value?.coverImage?.fileUrl) {
    return item.value.coverImage.fileUrl
  }
  return null
})

// 获取类型样式


// 分享
const handleShare = () => {
  if (navigator.share) {
    navigator.share({
      title: item.value.title,
      text: item.value.summary,
      url: window.location.href
    }).catch(err => console.log('分享失败:', err))
  } else {
    // 复制链接到剪贴板
    navigator.clipboard.writeText(window.location.href).then(() => {
      alert('链接已复制到剪贴板')
    })
  }
}


// 预览附件
const handlePreview = (attachment) => {
  currentAttachment.value = attachment
  previewVisible.value = true
}

// 打印
const handlePrint = () => {
  window.print()
}

// 加载新闻详情
const { data: item, loading, error, reload: loadItemDetail } = useDetail(
  () => route.params.id, getInfoPublicationDetail, value => {
  if (value.type && value.type !== 'COMPANY_NEWS') throw new Error('该内容不属于当前栏目')
  return {
        id: value.id,
        title: value.title,
        type: value.category || '公司新闻', // 使用category作为type
        summary: value.summary,
        author: value.author,
        publishDate: formatDate(value.publishTime),
        views: value.viewCount || 0,
        isTop: value.isTop || false,
        coverImage: value.coverImage,
        content: value.content || '',
        attachments: value.attachments || [],
        tags: [] // 后端暂无tags字段，可以后续扩展
      }
})
usePageMeta(() => ({ title: item.value?.title || item.value?.name, description: item.value?.summary || item.value?.description, unavailable: Boolean(error.value) }))
watch(() => route.params.id, () => { previewVisible.value = false; currentAttachment.value = {} })
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

@media print {
  header, footer, .no-print {
    display: none !important;
  }
}
</style>

<template>
  <div class="min-h-screen bg-gray-50">
    <Header />
    
    <!-- 面包屑导航 -->
    <div class="bg-white border-b">
      <div class="container-wide py-4">
        <div class="flex items-center text-sm text-gray-600">
          <router-link to="/" class="hover:text-hailong-primary transition-colors">首页</router-link>
          <svg class="w-4 h-4 mx-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
          <router-link to="/company-announcements" class="hover:text-hailong-primary transition-colors">新闻中心</router-link>
          <svg class="w-4 h-4 mx-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
          <span class="text-gray-900">新闻详情</span>
        </div>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-12 bg-white">
      <div class="container-wide">
        <div v-if="loading" class="text-center py-20">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
          <p class="mt-4 text-gray-500">加载中...</p>
        </div>

        <div v-else-if="error" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-red-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-gray-500 mb-4">{{ error }}</p>
          <router-link to="/company-announcements" class="text-hailong-primary hover:underline">返回列表</router-link>
        </div>

        <div v-else-if="!item" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p class="text-gray-500 mb-4">新闻不存在或已被删除</p>
          <router-link to="/company-announcements" class="text-hailong-primary hover:underline">返回列表</router-link>
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
            <div class="prose prose-lg max-w-none text-gray-700 leading-relaxed" v-html="item.content"></div>
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
          <div v-if="item.attachments && item.attachments.length > 0" class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <h2 class="text-xl font-bold text-gray-900 mb-6 pb-4 border-b border-gray-200 flex items-center gap-2">
              <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
              </svg>
              附件列表
            </h2>
            <div class="space-y-3">
              <div
                v-for="attachment in item.attachments"
                :key="attachment.id"
                class="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors group"
              >
                <div class="flex items-center gap-3 flex-1">
                  <div class="w-10 h-10 bg-hailong-primary/10 rounded-lg flex items-center justify-center">
                    <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                    </svg>
                  </div>
                  <div class="flex-1">
                    <div class="text-gray-900 font-medium group-hover:text-hailong-primary transition-colors">
                      {{ attachment.fileName }}
                    </div>
                    <div class="text-sm text-gray-500">
                      {{ formatFileSize(attachment.fileSize) }}
                    </div>
                  </div>
                </div>
                <div class="flex items-center gap-2">
                  <button
                    @click="handlePreview(attachment)"
                    class="px-4 py-2 bg-white border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-all font-medium flex items-center gap-2"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                    预览
                  </button>
                  <a
                    :href="attachment.fileUrl"
                    download
                    class="px-4 py-2 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-lg hover:shadow-lg transition-all font-medium flex items-center gap-2"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                    </svg>
                    下载
                  </a>
                </div>
              </div>
            </div>
          </div>

          <!-- 相关新闻 -->
          <div v-if="relatedItems.length > 0" class="bg-white rounded-xl shadow-sm p-8 mb-6">
            <h2 class="text-xl font-bold text-gray-900 mb-6 pb-4 border-b border-gray-200">
              相关新闻
            </h2>
            <div class="space-y-4">
              <router-link
                v-for="related in relatedItems"
                :key="related.id"
                :to="`/company-announcement/${related.id}`"
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
          <div class="flex items-center justify-between">
            <router-link
              to="/company-announcements"
              class="px-6 py-3 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-all font-medium flex items-center gap-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
              返回列表
            </router-link>

            <div class="flex items-center gap-3">
              <button
                @click="handleShare"
                class="px-6 py-3 bg-white border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-all font-medium flex items-center gap-2"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                </svg>
                分享
              </button>

              <button
                @click="handlePrint"
                class="px-6 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-lg hover:shadow-lg transition-all font-medium flex items-center gap-2"
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

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { getInfoPublicationDetail } from '@/api/infoPublication'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import AttachmentPreview from '@/components/AttachmentPreview.vue'

const route = useRoute()

// 新闻数据
const item = ref(null)
const loading = ref(true)
const error = ref(null)
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
const getTypeStyle = (type) => {
  switch (type) {
    case '企业动态':
      return 'bg-blue-100 text-blue-700 border border-blue-200'
    case '通知公告':
      return 'bg-green-100 text-green-700 border border-green-200'
    case '人事任免':
      return 'bg-purple-100 text-purple-700 border border-purple-200'
    case '制度文件':
      return 'bg-orange-100 text-orange-700 border border-orange-200'
    default:
      return 'bg-gray-100 text-gray-800 border border-gray-200'
  }
}

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

// 格式化文件大小
const formatFileSize = (bytes) => {
  if (!bytes || bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
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
const loadItemDetail = async () => {
  loading.value = true
  error.value = null
  
  try {
    const id = route.params.id
    
    if (!id) {
      error.value = '新闻ID不存在'
      return
    }
    
    // 调用API获取新闻详情
    const response = await getInfoPublicationDetail(id)
    
    if (response.success && response.data) {
      // 映射后端字段到前端显示
      item.value = {
        id: response.data.id,
        title: response.data.title,
        type: response.data.category || '企业动态', // 使用category作为type
        summary: response.data.summary,
        author: response.data.author,
        publishDate: formatDate(response.data.publishTime),
        views: response.data.viewCount || 0,
        isTop: response.data.isTop || false,
        coverImage: response.data.coverImage,
        content: response.data.content || '',
        attachments: response.data.attachments || [],
        tags: [] // 后端暂无tags字段，可以后续扩展
      }
      
      // TODO: 加载相关新闻（可以后续实现）
      relatedItems.value = []
    } else {
      error.value = response.message || '获取新闻详情失败'
      item.value = null
    }
  } catch (err) {
    console.error('加载新闻详情失败:', err)
    error.value = err.message || '加载新闻详情失败，请稍后重试'
    item.value = null
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadItemDetail()
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.prose h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
  margin-top: 2rem;
  margin-bottom: 1rem;
}

.prose p {
  margin-bottom: 1rem;
  line-height: 1.75;
}

@media print {
  header, footer, .no-print {
    display: none !important;
  }
}
</style>
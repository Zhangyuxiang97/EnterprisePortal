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
          <router-link to="/policies" class="hover:text-hailong-primary transition-colors">政策法规</router-link>
          <svg class="w-4 h-4 mx-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
          <span class="text-gray-900">法规详情</span>
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
          <router-link to="/policies" class="text-hailong-primary hover:underline">返回列表</router-link>
        </div>

        <div v-else-if="!policy" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p class="text-gray-500 mb-4">政策法规不存在或已被删除</p>
          <router-link to="/policies" class="text-hailong-primary hover:underline">返回列表</router-link>
        </div>

        <div v-else class="lg:grid lg:grid-cols-3 lg:gap-8 items-start">
          <!-- 左侧：主要内容 -->
          <div class="lg:col-span-2 space-y-6">
            <!-- 法规主内容卡片 -->
            <div class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6 md:p-10">
              <!-- 置顶标识 -->
              <div v-if="policy.isTop" class="mb-4">
                <span class="inline-flex items-center gap-1 px-3 py-1 bg-red-500 text-white text-sm rounded-full font-medium">
                  <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M2 6a2 2 0 012-2h6a2 2 0 012 2v8a2 2 0 01-2 2H4a2 2 0 01-2-2V6zM14.553 7.106A1 1 0 0014 8v4a1 1 0 00.553.894l2 1A1 1 0 0018 13V7a1 1 0 00-1.447-.894l-2 1z" />
                  </svg>
                  置顶
                </span>
              </div>

              <!-- 标题 -->
              <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800 mb-6 leading-tight">
                {{ policy.title }}
              </h1>

              <!-- 元信息 -->
              <div class="flex flex-wrap items-center gap-6 pb-6 border-b border-slate-100">
                <div class="flex items-center gap-2 text-sm text-slate-500">
                  <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>
                  <span class="font-bold">分类：</span>
                  <span class="px-2.5 py-0.5 rounded text-[11px] font-bold border bg-blue-50 text-hailong-primary border-blue-100/30">
                    {{ policy.category }}
                  </span>
                </div>

                <div class="flex items-center gap-2 text-sm text-slate-500">
                  <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                  <span class="font-bold">发布时间：</span>
                  <span class="font-medium text-slate-700">{{ policy.publishDate }}</span>
                </div>

                <div class="flex items-center gap-2 text-sm text-slate-500">
                  <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  <span class="font-bold">浏览量：</span>
                  <span class="font-medium text-slate-700">{{ policy.views || 0 }} 次</span>
                </div>
              </div>

              <!-- 摘要 -->
              <div v-if="policy.summary" class="mt-6 p-5 bg-slate-50 border-l-4 border-l-hailong-primary rounded-xl">
                <div class="flex items-start gap-3">
                  <svg class="w-5 h-5 text-hailong-primary flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <div>
                    <h3 class="font-bold text-slate-800 mb-1.5 text-sm">内容摘要</h3>
                    <p class="text-slate-600 text-xs leading-relaxed">{{ policy.summary }}</p>
                  </div>
                </div>
              </div>

              <!-- 法规内容 -->
              <div class="mt-8">
                <div class="prose prose-lg max-w-none text-slate-700 leading-relaxed" v-html="policy.content"></div>
              </div>
            </div>

            <!-- 标签 -->
            <div v-if="policy.tags && policy.tags.length > 0" class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6">
              <div class="flex items-center gap-3 flex-wrap">
                <span class="text-sm font-bold text-slate-500">相关标签：</span>
                <span
                  v-for="tag in policy.tags"
                  :key="tag"
                  class="px-3 py-1 bg-slate-50 text-slate-600 rounded-full text-xs font-bold hover:bg-hailong-primary/10 hover:text-hailong-primary transition-colors cursor-pointer border border-slate-100"
                >
                  # {{ tag }}
                </span>
              </div>
            </div>

            <!-- 附件列表 -->
            <div v-if="policy.attachments && policy.attachments.length > 0" class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6 md:p-8">
              <h2 class="text-lg md:text-xl font-extrabold text-slate-900 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
                <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
                附件下载
              </h2>
              <div class="space-y-3">
                <div
                  v-for="attachment in policy.attachments"
                  :key="attachment.id"
                  class="flex items-center justify-between p-4 bg-slate-50 rounded-xl hover:bg-slate-100/70 border border-slate-100 transition-colors group"
                >
                  <div class="flex items-center gap-3 flex-1 min-w-0">
                    <div class="w-10 h-10 bg-hailong-primary/10 rounded-lg flex items-center justify-center flex-shrink-0">
                      <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                      </svg>
                    </div>
                    <div class="flex-1 min-w-0">
                      <div class="text-slate-800 font-bold text-sm truncate group-hover:text-hailong-primary transition-colors">
                        {{ attachment.fileName }}
                      </div>
                      <div class="text-[11px] text-slate-400 font-semibold mt-0.5">
                        {{ formatFileSize(attachment.fileSize) }}
                      </div>
                    </div>
                  </div>
                  <div class="flex items-center gap-2 ml-4">
                    <button
                      @click="handlePreview(attachment)"
                      class="px-4 py-2 bg-white border border-slate-200 text-slate-600 rounded-xl hover:bg-slate-50 hover:text-slate-800 transition-all text-xs font-bold flex items-center gap-1.5 shadow-sm"
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
                      class="px-4 py-2 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-xl hover:shadow-lg transition-all text-xs font-bold flex items-center gap-1.5"
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
          </div>

          <!-- 右侧：推荐与操作 -->
          <div class="lg:col-span-1 space-y-6 lg:sticky lg:top-24 h-fit">
            <!-- 快捷操作面板 -->
            <div class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6">
              <h3 class="text-sm font-bold text-slate-400 tracking-wider mb-4 uppercase">快捷操作</h3>
              <div class="grid grid-cols-1 gap-3">
                <button
                  @click="handleShare"
                  class="w-full px-5 py-3 bg-slate-50 hover:bg-slate-100 text-slate-700 rounded-xl transition-all text-sm font-bold flex items-center justify-center gap-2 border border-slate-100"
                >
                  <svg class="w-4 h-4 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                  </svg>
                  分享链接
                </button>
                
                <button
                  @click="handlePrint"
                  class="w-full px-5 py-3 bg-slate-50 hover:bg-slate-100 text-slate-700 rounded-xl transition-all text-sm font-bold flex items-center justify-center gap-2 border border-slate-100"
                >
                  <svg class="w-4 h-4 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" />
                  </svg>
                  打印本页
                </button>
                
                <router-link
                  to="/policies"
                  class="w-full px-5 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-xl hover:shadow-lg transition-all text-sm font-bold flex items-center justify-center gap-2 text-center"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                  </svg>
                  返回列表
                </router-link>
              </div>
            </div>

            <!-- 相关法规推荐卡片 -->
            <div v-if="relatedPolicies.length > 0" class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6">
              <h3 class="text-sm font-bold text-slate-400 tracking-wider mb-4 uppercase">相关法规推荐</h3>
              <div class="space-y-4">
                <router-link
                  v-for="related in relatedPolicies"
                  :key="related.id"
                  :to="`/detail/policy/${related.id}`"
                  class="block p-3.5 bg-slate-50 hover:bg-blue-50/20 rounded-xl border border-slate-100/50 transition-all group"
                >
                  <h4 class="text-xs font-bold text-slate-700 group-hover:text-hailong-primary transition-colors line-clamp-2 leading-relaxed mb-2">
                    {{ related.title }}
                  </h4>
                  <div class="flex items-center justify-between text-[10px] text-slate-400 font-semibold">
                    <span>{{ related.publishDate }}</span>
                    <span>{{ related.views }}次浏览</span>
                  </div>
                </router-link>
              </div>
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
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { getInfoPublicationDetail } from '@/api/infoPublication'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import AttachmentPreview from '@/components/AttachmentPreview.vue'

const route = useRoute()

// 政策法规数据
const policy = ref(null)
const loading = ref(true)
const error = ref(null)
const relatedPolicies = ref([])

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

// 分享
const handleShare = () => {
  if (navigator.share) {
    navigator.share({
      title: policy.value.title,
      text: policy.value.summary,
      url: window.location.href
    }).catch(err => console.log('分享失败:', err))
  } else {
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

// 加载政策法规详情
const loadPolicyDetail = async () => {
  loading.value = true
  error.value = null
  
  try {
    const id = route.params.id
    
    if (!id) {
      error.value = '政策法规ID不存在'
      return
    }
    
    // 调用API获取政策法规详情
    const response = await getInfoPublicationDetail(id)
    
    if (response.success && response.data) {
      // 映射后端字段到前端显示
      policy.value = {
        id: response.data.id,
        title: response.data.title,
        category: response.data.category || '政策法规',
        publishDate: formatDate(response.data.publishTime),
        views: response.data.viewCount || 0,
        isTop: response.data.isTop || false,
        summary: response.data.summary,
        content: response.data.content || '',
        attachments: response.data.attachments || [],
        tags: [] // 后端暂无tags字段，可以后续扩展
      }
      
      // TODO: 加载相关法规（可以后续实现）
      relatedPolicies.value = []
    } else {
      error.value = response.message || '获取政策法规详情失败'
      policy.value = null
    }
  } catch (err) {
    console.error('加载政策法规详情失败:', err)
    error.value = err.message || '加载政策法规详情失败，请稍后重试'
    policy.value = null
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadPolicyDetail()
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

.prose ul {
  list-style-type: none;
  padding-left: 0;
  margin-bottom: 1rem;
}

.prose li {
  margin-bottom: 0.5rem;
  line-height: 1.75;
  padding-left: 1.5rem;
  position: relative;
}

.prose li::before {
  content: "•";
  position: absolute;
  left: 0.5rem;
  color: #3b82f6;
}

.prose strong {
  color: #1f2937;
  font-weight: 600;
}

@media print {
  header, footer, .no-print {
    display: none !important;
  }
}
</style>
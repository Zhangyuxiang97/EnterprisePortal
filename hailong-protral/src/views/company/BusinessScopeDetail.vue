<template>
  <div class="min-h-screen bg-gray-50 pt-20">
    
    <!-- 内容区域 -->
    <div class="pt-8 pb-12 bg-white">
      <div class="container-wide">
        <DetailBreadcrumb
          class="max-w-6xl mx-auto mb-4"
          section-label="业务范围"
          section-to="/about?tab=business"
          current-label="业务详情"
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
          <button @click="loadBusinessDetail" class="mt-4 rounded-lg bg-hailong-primary px-5 py-2 text-white">重新加载</button>
          <router-link to="/about" class="text-hailong-primary hover:underline">返回关于我们</router-link>
        </div>

        <div v-else-if="!business" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
          <p class="text-gray-500 mb-4">业务信息不存在</p>
          <router-link to="/about" class="text-hailong-primary hover:underline">返回关于我们</router-link>
        </div>

        <div v-else class="max-w-6xl mx-auto">
          <!-- 业务头部 -->
          <div class="bg-gradient-to-br from-hailong-primary via-hailong-secondary to-hailong-dark rounded-2xl shadow-xl p-12 mb-8 text-white">
            <div class="flex items-center gap-4 mb-6">
              <div class="w-16 h-16 bg-white/20 backdrop-blur-sm rounded-xl flex items-center justify-center">
                <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
              </div>
              <div>
                <h1 class="text-4xl font-bold mb-2">{{ business.name }}</h1>
                <p class="text-xl text-white/90">{{ business.description }}</p>
              </div>
            </div>
          </div>

          <!-- 业务配图 -->
          <div v-if="business.image" class="bg-white rounded-xl shadow-sm overflow-hidden mb-8">
            <img :src="business.image" :alt="business.name" class="w-full h-96 object-cover" />
          </div>

          <!-- 业务特点 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-8">
            <h2 class="text-lg md:text-xl font-extrabold text-slate-800 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
              <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
              业务特点
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div
                v-for="(feature, index) in business.features"
                :key="index"
                class="flex items-start gap-4 p-5 bg-gradient-to-br from-gray-50 to-white rounded-xl border border-gray-100 hover:shadow-md transition-all"
              >
                <div class="flex-shrink-0 w-8 h-8 bg-gradient-to-br from-hailong-primary to-hailong-secondary rounded-lg flex items-center justify-center text-white font-bold text-sm">
                  {{ index + 1 }}
                </div>
                <div class="flex-1">
                  <p class="text-gray-700 leading-relaxed">{{ feature }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- 详细内容 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-8">
            <h2 class="text-lg md:text-xl font-extrabold text-slate-800 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
              <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
              详细介绍
            </h2>
            <RichTextContent class="prose prose-lg max-w-none text-gray-700 leading-relaxed" :content="business.detailContent" />
          </div>

          <!-- 联系咨询 -->
          <div class="bg-gradient-to-r from-hailong-primary/10 via-hailong-secondary/10 to-hailong-cyan/10 rounded-xl p-8 mb-8">
            <div class="text-center">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">需要此项服务？</h2>
              <p class="text-gray-600 mb-6">我们的专业团队随时为您提供咨询服务</p>
              <router-link
                to="/contact"
                class="portal-button portal-button--primary inline-flex items-center gap-2 px-8 py-3"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
                立即咨询
              </router-link>
            </div>
          </div>

          <!-- 返回按钮 -->
          <div class="flex justify-center">
            <button
              @click="goBack"
              class="px-6 py-3 portal-button portal-button--secondary gap-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
              返回
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import DetailBreadcrumb from '@/components/common/DetailBreadcrumb.vue'
import { useDetail } from "@/composables/useDetail"
import { usePageMeta } from "@/composables/usePageMeta"
import RichTextContent from "@/components/common/RichTextContent.vue"
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getBusinessScopeById } from '@/api/config'

const route = useRoute()
const router = useRouter()

// 业务数据

// 返回上一页
const goBack = () => {
  const from = route.query.from
  const tab = route.query.tab
  
  if (from === 'about' && tab) {
    router.push({ path: '/about', query: { tab } })
  } else {
    router.back()
  }
}

// 加载业务详情
const { data: business, loading, error, reload: loadBusinessDetail } = useDetail(
  () => route.params.id, getBusinessScopeById, value => {
  return {
        id: value.id,
        name: value.name,
        description: value.description,
        image: value.imageUrl, // 后端返回的完整图片URL
        features: value.features || [], // features 数组
        detailContent: value.content || '' // 富文本内容
      }
})
usePageMeta(() => ({ title: business.value?.title || business.value?.name, description: business.value?.summary || business.value?.description, unavailable: Boolean(error.value) }))
</script>

<style scoped>
</style>
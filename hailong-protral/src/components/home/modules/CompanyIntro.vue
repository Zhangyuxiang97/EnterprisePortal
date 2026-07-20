<template>
  <div class="py-24 bg-gradient-to-b from-gray-50 to-white">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold text-slate-800 mb-4 font-tech tracking-tight">企业简介</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
      </div>
      <div v-if="profileLoading" class="text-center py-8 text-gray-500">加载中...</div>
      <div v-else-if="!profileContent" class="text-center py-8 text-gray-500">暂无企业简介</div>
      <div v-else class="bg-white rounded-2xl p-8 md:p-12 shadow-lg border border-gray-200">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8 items-center">
          <div class="lg:col-span-2">
            <div class="text-gray-700 text-base leading-relaxed prose prose-lg max-w-none" v-html="profileContent"></div>
            <div class="mt-6">
              <router-link to="/about"
                class="inline-flex items-center text-hailong-primary hover:text-hailong-secondary font-medium transition-colors group">
                <span>查看完整介绍</span>
                <svg class="w-5 h-5 ml-2 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4-4H3"></path>
                </svg>
              </router-link>
            </div>
          </div>
          <div v-if="profileHighlights.length > 0" class="grid grid-cols-2 gap-4">
            <div v-for="highlight in profileHighlights" :key="highlight"
              class="text-center p-6 bg-gradient-to-br from-hailong-primary/5 to-hailong-secondary/5 rounded-xl hover:shadow-md hover:scale-105 transition-all duration-300 cursor-pointer group">
              <div
                class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mx-auto mb-3 group-hover:bg-hailong-primary/20 transition-colors">
                <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                </svg>
              </div>
              <div
                class="text-hailong-primary font-semibold text-sm group-hover:text-hailong-secondary transition-colors">
                {{ highlight }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getCompanyProfile } from '@/api/config'

const profileLoading = ref(false)
const profileContent = ref('')
const profileHighlights = ref([])

const extractFirstParagraphs = (htmlContent, count = 2) => {
  if (!htmlContent) return ''
  
  const tempDiv = document.createElement('div')
  tempDiv.innerHTML = htmlContent
  
  const paragraphs = tempDiv.querySelectorAll('p')
  
  if (paragraphs.length === 0) {
    const text = tempDiv.textContent || tempDiv.innerText || ''
    return text.length <= 300 ? text : text.substring(0, 300).trim() + '...'
  }
  
  let result = ''
  for (let i = 0; i < Math.min(count, paragraphs.length); i++) {
    result += paragraphs[i].outerHTML
  }
  
  return result
}

const loadCompanyProfile = async () => {
  profileLoading.value = true
  try {
    const response = await getCompanyProfile()
    if (response.success && response.data) {
      const fullContent = response.data.content || ''
      profileContent.value = extractFirstParagraphs(fullContent, 2)
      profileHighlights.value = response.data.highlights || []
    }
  } catch (error) {
    console.error('加载企业简介失败:', error)
  } finally {
    profileLoading.value = false
  }
}

onMounted(() => {
  loadCompanyProfile()
})
</script>

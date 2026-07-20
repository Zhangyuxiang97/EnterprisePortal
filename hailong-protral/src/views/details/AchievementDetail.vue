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
          <span class="text-gray-900">业绩详情</span>
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
          <router-link to="/" class="text-hailong-primary hover:underline">返回首页</router-link>
        </div>

        <div v-else-if="!achievement" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
          </svg>
          <p class="text-gray-500 mb-4">业绩信息不存在</p>
          <router-link to="/" class="text-hailong-primary hover:underline">返回首页</router-link>
        </div>

        <div v-else class="max-w-4xl mx-auto">
          <!-- 业绩头部 -->
          <div class="bg-gradient-to-br from-hailong-primary via-hailong-secondary to-hailong-dark rounded-2xl shadow-xl p-8 mb-8 text-white relative overflow-hidden">
            <!-- 背景装饰 -->
            <div class="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/2"></div>
            <div class="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/2"></div>
            
            <div class="relative z-10">
              <h1 class="text-3xl font-bold mb-4">{{ achievement.projectName }}</h1>
              
              <div class="flex flex-wrap items-center gap-6 text-white/90">
                <div class="flex items-center gap-2">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>
                  <span>{{ achievement.projectType }}</span>
                </div>
                <div v-if="achievement.projectAmount" class="flex items-center gap-2">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <span class="text-xl font-bold">{{ formatAmount(achievement.projectAmount) }}</span>
                </div>
                <div v-if="achievement.completionDate" class="flex items-center gap-2">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                  <span>{{ formatDate(achievement.completionDate) }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- 项目图片 -->
          <div v-if="achievement.imageUrls && achievement.imageUrls.length > 0" class="bg-white rounded-xl shadow-sm overflow-hidden mb-8">
            <img :src="achievement.imageUrls[0]" :alt="achievement.projectName" class="w-full h-80 object-cover" />
          </div>

          <!-- 项目信息卡片 -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
            <!-- 客户信息 -->
            <div v-if="achievement.clientName" class="bg-white rounded-xl shadow-sm p-6">
              <div class="flex items-start gap-3">
                <div class="w-10 h-10 bg-hailong-primary/10 rounded-lg flex items-center justify-center flex-shrink-0">
                  <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                  </svg>
                </div>
                <div>
                  <div class="text-sm text-gray-500 mb-1">客户单位</div>
                  <div class="text-base font-semibold text-gray-900">{{ achievement.clientName }}</div>
                </div>
              </div>
            </div>

            <!-- 项目金额 -->
            <div v-if="achievement.projectAmount" class="bg-white rounded-xl shadow-sm p-6">
              <div class="flex items-start gap-3">
                <div class="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div>
                  <div class="text-sm text-gray-500 mb-1">项目金额</div>
                  <div class="text-base font-semibold text-gray-900">{{ formatAmount(achievement.projectAmount) }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- 项目描述 -->
          <div v-if="achievement.description" class="bg-white rounded-xl shadow-sm p-8 mb-8">
            <h2 class="text-lg md:text-xl font-extrabold text-slate-800 mb-4 pb-3 border-b border-slate-100 flex items-center gap-2.5">
              <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
              项目描述
            </h2>
            <div class="text-gray-700 leading-relaxed whitespace-pre-wrap">{{ achievement.description }}</div>
          </div>

          <!-- 返回按钮 -->
          <div class="flex justify-center">
            <button
              @click="goBack"
              class="px-6 py-3 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-all font-medium flex items-center gap-2"
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

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAchievementById } from '@/api/config'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'

const route = useRoute()
const router = useRouter()

// 业绩数据
const achievement = ref(null)
const loading = ref(true)
const error = ref(null)

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

// 格式化金额（与首页保持一致）
const formatAmount = (amount) => {
  if (!amount) return '¥0'
  const numAmount = Number(amount)
  if (numAmount >= 10000) {
    return `¥${(numAmount / 10000).toFixed(2)}亿`
  }
  return `¥${numAmount.toLocaleString()}万`
}

// 格式化日期
const formatDate = (date) => {
  if (!date) return '-'
  // 如果是Date对象，转换为字符串
  if (date instanceof Date) {
    return date.toLocaleDateString('zh-CN')
  }
  // 如果是字符串，尝试格式化
  try {
    const dateObj = new Date(date)
    return dateObj.toLocaleDateString('zh-CN')
  } catch (e) {
    return date
  }
}

// 加载业绩详情
const loadAchievementDetail = async () => {
  loading.value = true
  error.value = null
  
  try {
    const id = route.params.id
    
    if (!id) {
      error.value = '业绩ID不存在'
      return
    }
    
    // 调用API获取业绩详情
    const response = await getAchievementById(id)
    
    if (response.success && response.data) {
      achievement.value = response.data
    } else {
      error.value = response.message || '获取业绩详情失败'
      achievement.value = null
    }
  } catch (err) {
    console.error('加载业绩详情失败:', err)
    error.value = err.message || '加载业绩详情失败，请稍后重试'
    achievement.value = null
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadAchievementDetail()
})
</script>

<style scoped>
.prose p {
  margin-bottom: 1rem;
  line-height: 1.75;
}
</style>
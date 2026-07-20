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
          <router-link to="/about" class="hover:text-hailong-primary transition-colors">关于我们</router-link>
          <svg class="w-4 h-4 mx-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
          <span class="text-gray-900">业务详情</span>
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
            <div class="prose prose-lg max-w-none text-gray-700 leading-relaxed" v-html="business.detailContent"></div>
          </div>

          <!-- 联系咨询 -->
          <div class="bg-gradient-to-r from-hailong-primary/10 via-hailong-secondary/10 to-hailong-cyan/10 rounded-xl p-8 mb-8">
            <div class="text-center">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">需要此项服务？</h2>
              <p class="text-gray-600 mb-6">我们的专业团队随时为您提供咨询服务</p>
              <router-link
                to="/contact"
                class="inline-flex items-center gap-2 px-8 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-lg hover:shadow-lg transition-all font-medium"
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
import { getBusinessScopeById } from '@/api/config'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'

const route = useRoute()
const router = useRouter()

// 业务数据
const business = ref(null)
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

// 加载业务详情
const loadBusinessDetail = async () => {
  loading.value = true
  error.value = null
  
  try {
    const id = route.params.id
    
    if (!id) {
      error.value = '业务ID不存在'
      return
    }
    
    // 调用API获取业务详情
    const response = await getBusinessScopeById(id)
    
    if (response.success && response.data) {
      // 映射后端字段到前端显示
      business.value = {
        id: response.data.id,
        name: response.data.name,
        description: response.data.description,
        image: response.data.imageUrl, // 后端返回的完整图片URL
        features: response.data.features || [], // features 数组
        detailContent: response.data.content || '' // 富文本内容
      }
    } else {
      error.value = response.message || '获取业务详情失败'
      business.value = null
    }
  } catch (err) {
    console.error('加载业务详情失败:', err)
    error.value = err.message || '加载业务详情失败，请稍后重试'
    business.value = null
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadBusinessDetail()
})
</script>

<style scoped>
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
  list-style-type: disc;
  padding-left: 2rem;
  margin-bottom: 1rem;
}

.prose li {
  margin-bottom: 0.5rem;
  line-height: 1.75;
}
</style>
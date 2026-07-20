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
          <span class="text-gray-900">资质详情</span>
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

        <div v-else-if="!qualification" class="text-center py-20">
          <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-gray-500 mb-4">资质信息不存在</p>
          <router-link to="/about" class="text-hailong-primary hover:underline">返回关于我们</router-link>
        </div>

        <div v-else class="max-w-5xl mx-auto">
          <!-- 资质头部 -->
          <div class="bg-gradient-to-br from-hailong-primary via-hailong-secondary to-hailong-dark rounded-2xl shadow-xl p-12 mb-8 text-white relative overflow-hidden">
            <!-- 背景装饰 -->
            <div class="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/2"></div>
            <div class="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/2"></div>
            
            <div class="relative z-10">
              <div class="flex items-center gap-4 mb-6">
                <div class="w-16 h-16 bg-white/20 backdrop-blur-sm rounded-xl flex items-center justify-center">
                  <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div>
                  <h1 class="text-4xl font-bold mb-2">{{ qualification.name }}</h1>
                  <p class="text-xl text-white/90">{{ qualification.description }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- 资质证书图片 -->
          <div class="bg-white rounded-xl shadow-sm overflow-hidden mb-8">
            <img :src="qualification.image" :alt="qualification.name" class="w-full h-auto object-contain max-h-[600px]" />
          </div>

          <!-- 资质信息卡片 -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <!-- 证书编号 -->
            <div v-if="qualification.certificateNo" class="bg-white rounded-xl shadow-sm p-6">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-12 h-12 bg-hailong-primary/10 rounded-lg flex items-center justify-center">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14" />
                  </svg>
                </div>
                <div>
                  <div class="text-sm text-gray-500">证书编号</div>
                  <div class="text-lg font-bold text-gray-900">{{ qualification.certificateNo }}</div>
                </div>
              </div>
            </div>

            <!-- 颁发日期 -->
            <div v-if="qualification.issueDate" class="bg-white rounded-xl shadow-sm p-6">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                  <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </div>
                <div>
                  <div class="text-sm text-gray-500">颁发日期</div>
                  <div class="text-lg font-bold text-gray-900">{{ qualification.issueDate }}</div>
                </div>
              </div>
            </div>

            <!-- 有效期至 -->
            <div v-if="qualification.validUntil" class="bg-white rounded-xl shadow-sm p-6">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-12 h-12 bg-hailong-secondary/10 rounded-lg flex items-center justify-center">
                  <svg class="w-6 h-6 text-hailong-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div>
                  <div class="text-sm text-gray-500">有效期至</div>
                  <div class="text-lg font-bold text-gray-900">{{ qualification.validUntil }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- 资质详情 -->
          <div class="bg-white rounded-xl shadow-sm p-8 mb-8">
            <h2 class="text-lg md:text-xl font-extrabold text-slate-800 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
              <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
              资质说明
            </h2>
            <div class="prose prose-lg max-w-none text-gray-700 leading-relaxed">
              <p>{{ qualification.description }}</p>
              
              <h3>资质意义</h3>
              <p>该资质证明了我公司在相关领域具备专业的服务能力和资格，能够为客户提供高质量、规范化的专业服务。</p>
              
              <h3>业务范围</h3>
              <p>持有该资质，我公司可以依法开展相关业务活动，为政府机关、企事业单位提供专业的咨询和服务。</p>
              
              <h3>质量保证</h3>
              <p>我公司严格按照资质要求和行业规范开展业务，建立了完善的质量管理体系，确保服务质量和客户满意度。</p>
            </div>
          </div>

          <!-- 相关资质 -->
          <div v-if="relatedQualifications.length > 0" class="bg-white rounded-xl shadow-sm p-8 mb-8">
            <h2 class="text-lg md:text-xl font-extrabold text-slate-800 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
              <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
              相关资质
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <router-link
                v-for="related in relatedQualifications"
                :key="related.id"
                :to="`/detail/qualification/${related.id}`"
                class="group bg-gradient-to-br from-gray-50 to-white rounded-xl p-6 border border-gray-100 hover:border-hailong-primary hover:shadow-lg transition-all"
              >
                <div class="flex items-center gap-3 mb-3">
                  <div class="w-10 h-10 bg-hailong-primary/10 rounded-lg flex items-center justify-center group-hover:bg-hailong-primary/20 transition-colors">
                    <svg class="w-5 h-5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <h3 class="text-base font-bold text-gray-900 group-hover:text-hailong-primary transition-colors line-clamp-2 flex-1">
                    {{ related.name }}
                  </h3>
                </div>
                <p class="text-sm text-gray-600 line-clamp-2">{{ related.description }}</p>
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
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getQualificationById } from '@/api/config'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'

const route = useRoute()
const router = useRouter()

// 资质数据
const qualification = ref(null)
const loading = ref(true)
const error = ref(null)
const relatedQualifications = ref([])

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

// 获取证书图片URL
const certificateImageUrl = computed(() => {
  return qualification.value?.certificateImageUrl || null
})

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

// 加载资质详情
const loadQualificationDetail = async () => {
  loading.value = true
  error.value = null
  
  try {
    const id = route.params.id
    
    if (!id) {
      error.value = '资质ID不存在'
      return
    }
    
    // 调用API获取资质详情
    const response = await getQualificationById(id)
    
    if (response.success && response.data) {
      // 映射后端字段到前端显示
      qualification.value = {
        id: response.data.id,
        name: response.data.name,
        image: response.data.certificateImageUrl, // 证书图片URL
        description: response.data.description || '',
        certificateNo: response.data.certificateNumber,
        issueDate: formatDate(response.data.issueDate),
        validUntil: formatDate(response.data.expiryDate)
      }
      
      // TODO: 加载相关资质（可以后续实现）
      relatedQualifications.value = []
    } else {
      error.value = response.message || '获取资质详情失败'
      qualification.value = null
    }
  } catch (err) {
    console.error('加载资质详情失败:', err)
    error.value = err.message || '加载资质详情失败，请稍后重试'
    qualification.value = null
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadQualificationDetail()
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
</style>
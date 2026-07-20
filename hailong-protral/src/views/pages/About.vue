<template>
  <div class="min-h-screen bg-slate-50/50">
    <!-- 导航栏 -->
    <Header />

    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <!-- 几何网格与光晕背景 -->
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          关于海隆
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">{{ companyInfo.slogan }}</p>
      </div>
    </div>

    <!-- Tab导航 -->
    <div class="bg-white/80 backdrop-blur-md sticky top-20 z-40 border-b border-slate-100/80 shadow-sm">
      <div class="container-wide">
        <div class="flex space-x-8 overflow-x-auto">
          <button v-for="tab in tabs" :key="tab.id"
            @click="activeTab = tab.id"
            class="px-6 py-4 text-gray-700 font-medium whitespace-nowrap transition-all border-b-2"
            :class="activeTab === tab.id ? 'border-hailong-primary text-hailong-primary' : 'border-transparent hover:text-hailong-primary'">
            {{ tab.name }}
          </button>
        </div>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-12">
      <div class="container-wide">
        <!-- 企业简介 -->
        <div v-show="activeTab === 'intro'" class="animate-fade-in">
          <div class="max-w-5xl mx-auto">
            <div class="bg-white rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.02)] overflow-hidden border border-slate-100">
              <div v-if="loading" class="p-12 text-center">
                <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-hailong-primary"></div>
                <p class="mt-4 text-gray-600">加载中...</p>
              </div>
              <div v-else-if="error" class="p-12 text-center">
                <p class="text-red-600">{{ error }}</p>
                <button @click="fetchCompanyProfile" class="mt-4 px-6 py-2 bg-hailong-primary text-white rounded-lg hover:bg-hailong-secondary transition-colors">
                  重新加载
                </button>
              </div>
              <div v-else>
                <div v-if="companyProfile.imageUrl" class="h-96 overflow-hidden">
                  <img :src="companyProfile.imageUrl" alt="关于海隆"
                    @click="openImagePreview(companyProfile.imageUrl)"
                    class="w-full h-full object-cover cursor-pointer hover:scale-105 transition-transform duration-500" />
                </div>
                <div class="p-12">
                  <div class="mb-8">
                    <h2 class="text-lg md:text-xl font-extrabold text-slate-800 mb-4 pb-3 border-b border-slate-100 flex items-center gap-2.5">
                      <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
                      关于海隆
                    </h2>
                    <h3 v-if="companyProfile.title" class="text-xl md:text-2xl font-medium text-gray-700 mb-4">
                      {{ companyProfile.title }}
                    </h3>
                  </div>
                  <div class="prose prose-lg max-w-none text-gray-700 leading-relaxed" v-html="companyProfile.content"></div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 业务范围 -->
        <div v-show="activeTab === 'business'" class="animate-fade-in">
          <h2 class="text-4xl font-bold text-hailong-dark mb-8 text-center font-tech">业务范围</h2>
          <div v-if="loadingBusiness" class="text-center py-12">
            <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-hailong-primary"></div>
            <p class="mt-4 text-gray-600">加载中...</p>
          </div>
          <div v-else-if="businessError" class="text-center py-12">
            <p class="text-red-600">{{ businessError }}</p>
            <button @click="fetchBusinessScope" class="mt-4 px-6 py-2 bg-hailong-primary text-white rounded-lg hover:bg-hailong-secondary transition-colors">
              重新加载
            </button>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <div v-for="business in businessScope" :key="business.id"
              @click="handleBusinessClick(business.id)"
              class="group bg-white rounded-2xl overflow-hidden shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 transition-all duration-300 border border-slate-100 transition-all duration-300 cursor-pointer">
              <div class="h-48 overflow-hidden">
                <img :src="business.image" :alt="business.name"
                  class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              </div>
              <div class="p-6">
                <h3 class="text-2xl font-bold text-gray-900 mb-3 group-hover:text-hailong-primary transition-colors">
                  {{ business.name }}
                </h3>
                <p class="text-gray-600 mb-4">{{ business.description }}</p>
                <ul class="space-y-2">
                  <li v-for="feature in business.features" :key="feature" class="text-sm text-gray-500 flex items-center">
                    <span class="w-1.5 h-1.5 bg-hailong-secondary rounded-full mr-2"></span>
                    {{ feature }}
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <!-- 企业资质 -->
        <div v-show="activeTab === 'qualifications'" class="animate-fade-in">
          <h2 class="text-4xl font-bold text-hailong-dark mb-12 text-center font-tech">企业资质</h2>
          <div v-if="loadingQualifications" class="text-center py-12">
            <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-hailong-primary"></div>
            <p class="mt-4 text-gray-600">加载中...</p>
          </div>
          <div v-else-if="qualificationsError" class="text-center py-12">
            <p class="text-red-600">{{ qualificationsError }}</p>
            <button @click="fetchQualifications" class="mt-4 px-6 py-2 bg-hailong-primary text-white rounded-lg hover:bg-hailong-secondary transition-colors">
              重新加载
            </button>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div v-for="qualification in qualifications" :key="qualification.id"
              @click="handleQualificationClick(qualification.id)"
              class="group bg-white rounded-2xl overflow-hidden shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 cursor-pointer">
              <div class="relative h-40 overflow-hidden bg-gradient-to-br from-hailong-primary/10 to-hailong-secondary/10">
                <img :src="qualification.image" :alt="qualification.name"
                  class="w-full h-full object-cover opacity-80 group-hover:opacity-100 group-hover:scale-110 transition-all duration-500" />
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                <div class="absolute bottom-0 left-0 right-0 p-4">
                  <div class="w-10 h-10 bg-white/90 rounded-full flex items-center justify-center mb-2">
                    <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                  </div>
                </div>
              </div>
              <div class="p-5">
                <h3 class="text-base font-bold text-gray-900 mb-2 group-hover:text-hailong-primary transition-colors line-clamp-2">
                  {{ qualification.name }}
                </h3>
                <p class="text-sm text-gray-600 line-clamp-2">{{ qualification.description }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- 重要业绩 -->
        <div v-show="activeTab === 'achievements'" class="animate-fade-in">
          <h2 class="text-4xl font-bold text-hailong-dark mb-12 text-center font-tech">重要业绩</h2>
          
          <div v-if="loadingMajorAchievements" class="text-center py-12">
            <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-hailong-primary"></div>
            <p class="mt-4 text-gray-600">加载中...</p>
          </div>
          <div v-else-if="majorAchievementsError" class="text-center py-12">
            <p class="text-red-600">{{ majorAchievementsError }}</p>
            <button @click="fetchMajorAchievements" class="mt-4 px-6 py-2 bg-hailong-primary text-white rounded-lg hover:bg-hailong-secondary transition-colors">
              重新加载
            </button>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <div v-for="achievement in majorAchievements" :key="achievement.id"
              @click="handleAchievementClick(achievement.id)"
              class="group bg-white rounded-2xl overflow-hidden shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 cursor-pointer">
              <div class="h-48 overflow-hidden bg-gradient-to-br from-hailong-primary/5 to-hailong-secondary/5">
                <img
                  :src="achievement.imageUrls && achievement.imageUrls.length > 0 ? achievement.imageUrls[0] : 'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?w=800&h=600&fit=crop'"
                  :alt="achievement.projectName"
                  class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              </div>
              <div class="p-6">
                <h3 class="text-xl font-bold text-gray-900 mb-3 group-hover:text-hailong-primary transition-colors line-clamp-2">
                  {{ achievement.projectName }}
                </h3>
                <div class="space-y-2 text-sm text-gray-600">
                  <div v-if="achievement.projectType" class="flex items-center gap-2">
                    <svg class="w-4 h-4 text-hailong-secondary flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                    </svg>
                    <span>{{ achievement.projectType }}</span>
                  </div>
                  <div v-if="achievement.clientName" class="flex items-center gap-2">
                    <svg class="w-4 h-4 text-hailong-secondary flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                    </svg>
                    <span class="line-clamp-1">{{ achievement.clientName }}</span>
                  </div>
                  <div v-if="achievement.projectAmount" class="flex items-center gap-2">
                    <svg class="w-4 h-4 text-hailong-secondary flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <span class="font-semibold text-hailong-primary">{{ formatAmount(achievement.projectAmount) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 企业荣誉 -->
        <div v-show="activeTab === 'honors'" class="animate-fade-in">
          <div class="max-w-6xl mx-auto">
            <h2 class="text-4xl font-bold text-hailong-dark mb-12 text-center font-tech">企业荣誉</h2>
            
            <div v-if="loadingHonors" class="text-center py-12">
              <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-hailong-primary"></div>
              <p class="mt-4 text-gray-600">加载中...</p>
            </div>
            <div v-else-if="honorsError" class="text-center py-12">
              <p class="text-red-600">{{ honorsError }}</p>
              <button @click="fetchHonors" class="mt-4 px-6 py-2 bg-hailong-primary text-white rounded-lg hover:bg-hailong-secondary transition-colors">
                重新加载
              </button>
            </div>
            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <div v-for="honor in honors" :key="honor.id"
                @click="openImagePreview(honor.imageUrl)"
                class="group bg-white rounded-2xl overflow-hidden shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 cursor-pointer">
                <div class="h-48 overflow-hidden bg-gradient-to-br from-hailong-primary/5 to-hailong-secondary/5">
                  <img :src="honor.imageUrl" :alt="honor.name"
                    class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                </div>
                <div class="p-4">
                  <h3 class="text-sm font-bold text-gray-900 text-center group-hover:text-hailong-primary transition-colors line-clamp-2">
                    {{ honor.name }}
                  </h3>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 页脚 -->
    <Footer />

    <!-- 图片预览模态框 -->
    <div v-if="showImagePreview"
      @click="closeImagePreview"
      class="fixed inset-0 z-[100] bg-black/90 flex items-center justify-center p-4 animate-fade-in">
      <button @click="closeImagePreview"
        class="absolute top-4 right-4 text-white hover:text-hailong-cyan transition-colors z-10">
        <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
        </svg>
      </button>
      <img :src="previewImageUrl"
        @click.stop
        class="max-w-full max-h-full object-contain rounded-lg shadow-2xl"
        alt="预览图片" />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import { getCompanyInfo } from '@/utils/config'
import {
  getCompanyProfile,
  getBusinessScope,
  getCompanyQualifications,
  getCompanyHonors,
  getMajorAchievements
} from '@/api/config'

const router = useRouter()

// 获取公司信息
const companyInfo = computed(() => getCompanyInfo())

// Tab配置
const tabs = [
  { id: 'intro', name: '企业简介' },
  { id: 'business', name: '业务范围' },
  { id: 'qualifications', name: '企业资质' },
  { id: 'achievements', name: '重要业绩' },
  { id: 'honors', name: '企业荣誉' }
]

const activeTab = ref('intro')

// 企业简介数据
const companyProfile = ref({
  id: 0,
  title: '',
  content: '',
  imageUrl: ''
})
const loading = ref(false)
const error = ref('')

// 业务范围数据
const businessScope = ref([])
const loadingBusiness = ref(false)
const businessError = ref('')

// 企业资质数据
const qualifications = ref([])
const loadingQualifications = ref(false)
const qualificationsError = ref('')

// 企业荣誉数据
const honors = ref([])
const loadingHonors = ref(false)
const honorsError = ref('')

// 重要业绩数据
const majorAchievements = ref([])
const loadingMajorAchievements = ref(false)
const majorAchievementsError = ref('')

// API基础URL
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

// 获取企业简介
const fetchCompanyProfile = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await getCompanyProfile()
    if (response.success && response.data) {
      const data = response.data
      // 处理图片URL - 如果imageUrls存在且有值，使用第一张图片
      let imageUrl = ''
      if (data.imageUrls && data.imageUrls.length > 0) {
        // 检查是否已经是完整URL（包含http或https）
        const firstImage = data.imageUrls[0]
        imageUrl = firstImage.startsWith('http') ? firstImage : `${API_BASE_URL}${firstImage}`
      } else if (data.imageIds && data.imageIds.length > 0) {
        // 如果只有imageIds，使用附件API
        imageUrl = `${API_BASE_URL}/api/attachment/${data.imageIds[0]}`
      }
      
      companyProfile.value = {
        id: data.id,
        title: data.title || '企业简介',
        content: data.content || '',
        imageUrl: imageUrl
      }
    } else {
      error.value = response.message || '获取企业简介失败'
    }
  } catch (err) {
    console.error('获取企业简介失败:', err)
    error.value = '获取企业简介失败，请稍后重试'
  } finally {
    loading.value = false
  }
}

// 获取业务范围
const fetchBusinessScope = async () => {
  loadingBusiness.value = true
  businessError.value = ''
  try {
    const response = await getBusinessScope()
    if (response.success && response.data) {
      businessScope.value = response.data
        .filter(item => item.status)
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .map(item => ({
          id: item.id,
          name: item.name,
          description: item.description || '',
          content: item.content || '',
          features: item.features || [],
          // 后端返回的 imageUrl 已经是完整URL，直接使用
          image: item.imageUrl || 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=800&h=600&fit=crop'
        }))
    } else {
      businessError.value = response.message || '获取业务范围失败'
    }
  } catch (err) {
    console.error('获取业务范围失败:', err)
    businessError.value = '获取业务范围失败，请稍后重试'
  } finally {
    loadingBusiness.value = false
  }
}

// 获取企业资质
const fetchQualifications = async () => {
  loadingQualifications.value = true
  qualificationsError.value = ''
  try {
    const response = await getCompanyQualifications()
    if (response.success && response.data) {
      qualifications.value = response.data
        .filter(item => item.status)
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .map(item => ({
          id: item.id,
          name: item.name,
          description: item.description || '',
          certificateNumber: item.certificateNumber || '',
          issuingAuthority: item.issuingAuthority || '',
          issueDate: item.issueDate || '',
          expiryDate: item.expiryDate || '',
          // 后端返回的 certificateImageUrl 已经是完整URL，直接使用
          image: item.certificateImageUrl || 'https://images.unsplash.com/photo-1450101499163-c8848c66ca85?w=800&h=600&fit=crop'
        }))
    } else {
      qualificationsError.value = response.message || '获取企业资质失败'
    }
  } catch (err) {
    console.error('获取企业资质失败:', err)
    qualificationsError.value = '获取企业资质失败，请稍后重试'
  } finally {
    loadingQualifications.value = false
  }
}

// 获取企业荣誉
const fetchHonors = async () => {
  loadingHonors.value = true
  honorsError.value = ''
  try {
    const response = await getCompanyHonors()
    if (response.success && response.data) {
      honors.value = response.data
        .filter(item => item.status)
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .map(item => ({
          id: item.id,
          name: item.name,
          description: item.description || '',
          awardOrganization: item.awardOrganization || '',
          awardDate: item.awardDate || '',
          certificateNo: item.certificateNo || '',
          honorLevel: item.honorLevel || '',
          // 后端返回的 imageUrl 已经是完整URL，直接使用（与业务范围保持一致）
          imageUrl: item.imageUrl || 'https://images.unsplash.com/photo-1589829545856-d10d557cf95f?w=400&h=300&fit=crop'
        }))
    } else {
      honorsError.value = response.message || '获取企业荣誉失败'
    }
  } catch (err) {
    console.error('获取企业荣誉失败:', err)
    honorsError.value = '获取企业荣誉失败，请稍后重试'
  } finally {
    loadingHonors.value = false
  }
}

// 获取重要业绩
const fetchMajorAchievements = async () => {
  loadingMajorAchievements.value = true
  majorAchievementsError.value = ''
  try {
    const response = await getMajorAchievements()
    if (response.success && response.data) {
      majorAchievements.value = response.data
        .filter(item => item.status)
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .map(item => ({
          id: item.id,
          projectName: item.projectName,
          projectType: item.projectType || '',
          clientName: item.clientName || '',
          projectAmount: item.projectAmount || 0,
          completionDate: item.completionDate || '',
          description: item.description || '',
          imageUrls: item.imageUrls || []
        }))
    } else {
      majorAchievementsError.value = response.message || '获取重要业绩失败'
    }
  } catch (err) {
    console.error('获取重要业绩失败:', err)
    majorAchievementsError.value = '获取重要业绩失败，请稍后重试'
  } finally {
    loadingMajorAchievements.value = false
  }
}

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}年${d.getMonth() + 1}月`
}

// 格式化金额
const formatAmount = (amount) => {
  if (!amount) return ''
  return `${amount.toLocaleString()}万元`
}

// 图片预览
const showImagePreview = ref(false)
const previewImageUrl = ref('')

const openImagePreview = (imageUrl) => {
  previewImageUrl.value = imageUrl
  showImagePreview.value = true
  document.body.style.overflow = 'hidden'
}

const closeImagePreview = () => {
  showImagePreview.value = false
  previewImageUrl.value = ''
  document.body.style.overflow = ''
}

// 业务范围点击处理
const handleBusinessClick = (id) => {
  router.push({
    path: `/business-scope/${id}`,
    query: { from: 'about', tab: 'business' }
  })
}

// 重要业绩点击处理
const handleAchievementClick = (id) => {
  router.push({
    path: `/achievement/${id}`,
    query: { from: 'about', tab: 'achievements' }
  })
}

// 企业资质点击处理
const handleQualificationClick = (id) => {
  router.push({
    path: `/qualification/${id}`,
    query: { from: 'about', tab: 'qualifications' }
  })
}

onMounted(() => {
  // 检查是否有 tab 参数，如果有则切换到对应的 tab
  const tab = router.currentRoute.value.query.tab
  if (tab && tabs.some(t => t.id === tab)) {
    activeTab.value = tab
  }
  
  fetchCompanyProfile()
  fetchBusinessScope()
  fetchQualifications()
  fetchMajorAchievements()
  fetchHonors()
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.animate-fade-in {
  animation: fadeIn 0.5s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.prose {
  color: #374151;
}

.prose :deep(h1),
.prose :deep(h2),
.prose :deep(h3),
.prose :deep(h4) {
  color: #1f2937;
  font-weight: 700;
  margin-top: 1.5em;
  margin-bottom: 0.75em;
}

.prose :deep(p) {
  margin-bottom: 1em;
  line-height: 1.75;
}

.prose :deep(ul),
.prose :deep(ol) {
  margin-left: 1.5em;
  margin-bottom: 1em;
}

.prose :deep(li) {
  margin-bottom: 0.5em;
}

.prose :deep(strong) {
  color: #1f2937;
  font-weight: 600;
}

.prose :deep(a) {
  color: #3b82f6;
  text-decoration: underline;
}

.prose :deep(img) {
  border-radius: 0.5rem;
  margin: 1.5em 0;
}

.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>
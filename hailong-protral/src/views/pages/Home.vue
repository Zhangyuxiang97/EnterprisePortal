<template>
  <div class="min-h-screen bg-gradient-to-br from-hailong-dark via-hailong-primary to-hailong-secondary overflow-hidden">
    <Header />

    <!-- 动态渲染首页模块 -->
    <component
      v-for="module in enabledModules"
      :key="module.name"
      :is="moduleComponents[module.component]"
      @show-contact="showContactModal = true"
      @announcement-click="handleAnnouncementClick"
      @business-click="handleBusinessClick"
      @qualification-click="handleQualificationClick"
      @achievement-click="handleAchievementClick"
    />

    <!-- 联系信息模态框 -->
    <div v-if="showContactModal" @click="showContactModal = false"
      class="fixed inset-0 z-[100] bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 animate-fade-in">
      <div @click.stop class="bg-white rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <!-- 模态框头部 -->
        <div class="bg-gradient-to-r from-hailong-primary to-hailong-secondary p-6 text-white relative">
          <button @click="showContactModal = false"
            class="absolute top-4 right-4 text-white hover:text-gray-200 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
          <h3 class="text-2xl font-bold">联系我们</h3>
          <p class="text-white/80 mt-2">期待与您的合作，共创美好未来</p>
        </div>

        <!-- 模态框内容 -->
        <div class="p-8">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 items-stretch">
            <!-- 联系方式列表 -->
            <div class="space-y-6 flex flex-col justify-center">
              <div class="flex items-start">
                <div
                  class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z">
                    </path>
                  </svg>
                </div>
                <div>
                  <h4 class="font-bold text-gray-900 mb-1">联系电话</h4>
                  <p class="text-gray-600 text-sm">{{ contactInfo.phone }}</p>
                </div>
              </div>

              <div class="flex items-start">
                <div
                  class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z">
                    </path>
                  </svg>
                </div>
                <div>
                  <h4 class="font-bold text-gray-900 mb-1">邮箱地址</h4>
                  <p class="text-gray-600 text-sm break-all">{{ contactInfo.email }}</p>
                </div>
              </div>

              <div class="flex items-start">
                <div
                  class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
                  </svg>
                </div>
                <div>
                  <h4 class="font-bold text-gray-900 mb-1">公司地址</h4>
                  <p class="text-gray-600 text-sm">{{ contactInfo.address.fullAddress }}</p>
                </div>
              </div>
            </div>

            <!-- 工作时间 -->
            <div
              class="flex flex-col items-center justify-center bg-gradient-to-br from-hailong-primary/10 to-hailong-secondary/10 rounded-xl p-6 h-full">
              <div class="w-20 h-20 bg-hailong-primary/20 rounded-full flex items-center justify-center mb-4">
                <svg class="w-10 h-10 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <h4 class="font-bold text-gray-900 text-lg mb-2">工作时间</h4>
              <p class="text-gray-700 font-medium mb-1">{{ contactInfo.workingHours.weekdays.split(' ')[0] }}</p>
              <p class="text-hailong-primary text-xl font-bold mb-3">{{ contactInfo.workingHours.weekdays.split(' ')[1] }}</p>
              <p class="text-gray-500 text-sm text-center">节假日{{ contactInfo.workingHours.weekend }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <Footer />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getSiteConfig, getContactInfo } from '@/utils/config'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'

// 导入所有模块组件
import HeroBanner from '@/components/home/modules/HeroBanner.vue'
import CompanyIntro from '@/components/home/modules/CompanyIntro.vue'
import AnnouncementList from '@/components/home/modules/AnnouncementList.vue'
import BusinessScope from '@/components/home/modules/BusinessScope.vue'
import QualificationList from '@/components/home/modules/QualificationList.vue'
import AchievementList from '@/components/home/modules/AchievementList.vue'
import DataVisualization from '@/components/home/modules/DataVisualization.vue'
import ContactInfo from '@/components/home/modules/ContactInfo.vue'

const router = useRouter()
const showContactModal = ref(false)
const contactInfo = computed(() => getContactInfo())

// 组件映射
const moduleComponents = {
  HeroBanner,
  CompanyIntro,
  AnnouncementList,
  BusinessScope,
  QualificationList,
  AchievementList,
  DataVisualization,
  ContactInfo
}

// 获取启用的模块并按 order 排序
const enabledModules = computed(() => {
  const config = getSiteConfig()
  const modules = config.homeModules || []
  return modules
    .filter(m => m.enabled)
    .sort((a, b) => a.order - b.order)
})

// 业务范围点击处理
const handleBusinessClick = (id) => {
  const savedScrollPosition = window.scrollY
  sessionStorage.setItem('homeScrollPosition', savedScrollPosition.toString())
  router.push(`/business-scope/${id}`)
}

// 重要业绩点击处理
const handleAchievementClick = (id) => {
  const savedScrollPosition = window.scrollY
  sessionStorage.setItem('homeScrollPosition', savedScrollPosition.toString())
  router.push(`/achievement/${id}`)
}

// 公告信息点击处理
const handleAnnouncementClick = (id) => {
  const savedScrollPosition = window.scrollY
  sessionStorage.setItem('homeScrollPosition', savedScrollPosition.toString())
  router.push(`/announcement/${id}`)
}

// 企业资质点击处理
const handleQualificationClick = (id) => {
  const savedScrollPosition = window.scrollY
  sessionStorage.setItem('homeScrollPosition', savedScrollPosition.toString())
  router.push(`/qualification/${id}`)
}

// 恢复滚动条位置
onMounted(() => {
  const savedScrollPosition = sessionStorage.getItem('homeScrollPosition')
  if (savedScrollPosition) {
    setTimeout(() => {
      window.scrollTo({
        top: parseInt(savedScrollPosition),
        behavior: 'instant'
      })
      sessionStorage.removeItem('homeScrollPosition')
    }, 100)
  }
})
</script>

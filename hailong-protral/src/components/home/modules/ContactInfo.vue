<template>
  <!-- 联系我们区块 -->
  <div
    class="relative py-24 bg-gradient-to-br from-hailong-dark via-hailong-primary to-hailong-secondary text-white overflow-hidden"
  >
    <!-- 几何网格与悬浮光晕 -->
    <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
    <div class="absolute inset-0">
      <div
        class="absolute top-1/4 left-1/4 w-96 h-96 bg-hailong-primary rounded-full filter blur-3xl opacity-15 animate-float"
      ></div>
      <div
        class="absolute bottom-1/4 right-1/4 w-96 h-96 bg-hailong-secondary rounded-full filter blur-3xl opacity-15 animate-float"
        style="animation-delay:1s"
      ></div>
      
      <!-- 修正 Math.random() 引发渲染/水合不一致的星光微粒 -->
      <div
        v-for="star in starParticles"
        :key="star.id"
        class="absolute w-1 h-1 bg-hailong-cyan rounded-full animate-ping"
        :style="{ top: star.top, left: star.left, animationDelay: star.delay }"
      ></div>
    </div>

    <div class="container-wide relative z-10">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold mb-4 font-tech tracking-tight">联系我们</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
        <p class="mt-4 text-base md:text-lg text-white/80 font-medium">期待与您的合作，共创美好未来</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <!-- 联系电话 -->
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl p-8 hover:bg-white/20 transition-all group">
          <div
            class="w-16 h-16 bg-white/20 rounded-full flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
          >
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"
              ></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold mb-3">联系电话</h3>
          <p class="text-white/95 text-lg font-bold mb-1">{{ contactInfo.phone }}</p>
          <p class="text-white/70 text-xs font-semibold">{{ contactInfo.workingHours.weekdays }}</p>
        </div>

        <!-- 邮箱地址 -->
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl p-8 hover:bg-white/20 transition-all group">
          <div
            class="w-16 h-16 bg-white/20 rounded-full flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
          >
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              ></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold mb-3">邮箱地址</h3>
          <p class="text-white/90 text-base font-medium break-all">{{ contactInfo.email }}</p>
        </div>

        <!-- 公司地址 -->
        <div class="bg-white/10 backdrop-blur-lg rounded-2xl p-8 hover:bg-white/20 transition-all group">
          <div
            class="w-16 h-16 bg-white/20 rounded-full flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
          >
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
              ></path>
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
              ></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold mb-3">公司地址</h3>
          <p class="text-white/90 text-base font-medium leading-relaxed">{{ contactInfo.address.fullAddress }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { getContactInfo } from '@/utils/config'

const contactInfo = computed(() => getContactInfo())

// 在组件挂载时生成固定的随机微粒坐标，避开在 Template 中随机渲染
const starParticles = ref([])

onMounted(() => {
  starParticles.value = Array.from({ length: 20 }, (_, i) => ({
    id: i,
    top: `${Math.random() * 100}%`,
    left: `${Math.random() * 100}%`,
    delay: `${Math.random() * 2}s`
  }))
})
</script>

<style scoped>
.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>

<template>
  <div class="relative h-screen flex items-center justify-center pt-20 bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950 overflow-hidden">
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

    <!-- 主体标语 -->
    <div class="relative z-10 text-center text-white px-6 max-w-5xl mx-auto">
      <h1
        class="text-4xl md:text-7xl font-extrabold mb-6 font-tech tracking-tight leading-tight"
        :aria-label="slogan"
      >
        <SloganReveal :text="slogan" :ready="configReady" />
      </h1>
      <p class="text-lg md:text-2xl text-slate-300 mb-12 max-w-4xl mx-auto font-medium leading-relaxed">
        {{ companyInfo.description }}
      </p>
      
      <div class="mx-auto flex max-w-xs flex-col gap-4 sm:max-w-none sm:flex-row sm:justify-center">
        <router-link to="/announcements" class="portal-button portal-button--primary px-9 py-3 text-base">查询招标与中标公告</router-link>
        <button @click="$emit('show-contact')" class="portal-button portal-button--on-dark px-9 py-3 text-base">联系业务咨询</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { getCompanyInfo, loadSiteConfig } from '@/utils/config'
import SloganReveal from './SloganReveal.vue'

defineEmits(['show-contact'])

const companyInfo = computed(() => getCompanyInfo())
const slogan = computed(() => (companyInfo.value.slogan || '').trim())
const configReady = ref(false)
let mounted = false

// 在组件挂载时生成固定的随机微粒坐标，避开在 Template 中随机渲染
const starParticles = ref([])

onMounted(() => {
  mounted = true
  // 配置读取结束后再展开，避免默认标语与后台标语各播放一次。
  loadSiteConfig().then(() => {
    if (mounted) configReady.value = true
  })
  starParticles.value = Array.from({ length: 20 }, (_, i) => ({
    id: i,
    top: `${Math.random() * 100}%`,
    left: `${Math.random() * 100}%`,
    delay: `${Math.random() * 2}s`
  }))
})

onUnmounted(() => {
  mounted = false
})
</script>

<style scoped>
.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>

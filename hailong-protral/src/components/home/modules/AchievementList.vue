<template>
  <div class="py-24 bg-hailong-dark text-white">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold mb-4 font-tech tracking-tight">重要业绩展示</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
      </div>
      <div v-if="loading" class="text-center py-8 text-white/70">加载中...</div>
      <div v-else-if="achievementsList.length === 0" class="text-center py-8 text-white/70">暂无业绩数据</div>
      <div v-else class="relative overflow-hidden">
        <div class="flex gap-6 animate-scroll">
          <div v-for="achievement in [...achievementsList, ...achievementsList]"
            :key="achievement.id + Math.random()"
            @click="$emit('achievement-click', achievement.id)"
            class="flex-shrink-0 w-80 bg-white/10 backdrop-blur-lg rounded-2xl overflow-hidden hover:bg-white/20 transition-all cursor-pointer group">
            <div class="h-48 overflow-hidden bg-gray-700">
              <img v-if="achievement.imageUrls && achievement.imageUrls.length > 0" :src="achievement.imageUrls[0]" :alt="achievement.projectName"
                class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              <div v-else class="w-full h-full flex items-center justify-center text-gray-500">
                <svg class="w-16 h-16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                </svg>
              </div>
            </div>
            <div class="p-6">
              <div v-if="achievement.projectType" class="flex items-center justify-between mb-3">
                <span :class="[
                  'px-3 py-1 rounded-full text-xs font-semibold',
                  achievement.projectType === '工程' ? 'bg-hailong-primary/20 text-hailong-primary' :
                    achievement.projectType === '服务' ? 'bg-hailong-secondary/20 text-hailong-secondary' :
                      'bg-hailong-cyan/20 text-hailong-cyan'
                ]">
                  {{ achievement.projectType }}
                </span>
              </div>
              <h3 class="text-lg font-bold mb-3 line-clamp-2">{{ achievement.projectName }}</h3>
              <div v-if="achievement.projectAmount" class="text-2xl font-bold text-hailong-secondary">
                {{ formatAmount(achievement.projectAmount) }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getMajorAchievements } from '@/api/config'

defineEmits(['achievement-click'])

const loading = ref(false)
const achievementsList = ref([])

const loadAchievements = async () => {
  loading.value = true
  try {
    const response = await getMajorAchievements()
    if (response.success && response.data) {
      achievementsList.value = response.data
    }
  } catch (error) {
    console.error('加载重要业绩失败:', error)
  } finally {
    loading.value = false
  }
}

const formatAmount = (amount) => {
  if (!amount) return '0'
  if (amount >= 10000) {
    return (amount / 10000).toFixed(2) + '亿'
  }
  return amount.toLocaleString() + '万'
}

onMounted(() => {
  loadAchievements()
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden
}

@keyframes scroll {
  0% {
    transform: translateX(0);
  }

  100% {
    transform: translateX(-50%);
  }
}

.animate-scroll {
  animation: scroll 30s linear infinite;
}

.animate-scroll:hover {
  animation-play-state: paused;
}
</style>

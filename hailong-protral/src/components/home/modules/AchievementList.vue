<template>
  <div v-if="loading || error || achievementsList.length" class="home-section">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold text-slate-800 mb-4 font-tech tracking-tight">重要业绩展示</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
      </div>
      <AsyncState v-if="loading || error" :loading="loading" :error="error" @retry="loadAchievements" />
      <div v-else class="relative overflow-hidden">
        <div class="flex gap-6 animate-scroll">
          <div v-for="(achievement, index) in [...achievementsList, ...achievementsList]"
            :key="`${achievement.id}-${index}`"
            @click="$emit('achievement-click', achievement.id)" @keydown.enter="$emit('achievement-click', achievement.id)" tabindex="0" role="link"
            class="flex-shrink-0 w-80 max-w-full bg-white border border-slate-200/80 rounded-2xl overflow-hidden hover:shadow-lg transition-shadow cursor-pointer group">
            <div class="h-48 overflow-hidden bg-slate-100">
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
                  achievement.projectType === '工程' ? 'bg-blue-50 text-blue-700' :
                    achievement.projectType === '服务' ? 'bg-violet-50 text-violet-700' :
                      'bg-cyan-50 text-cyan-700'
                ]">
                  {{ achievement.projectType }}
                </span>
              </div>
              <h3 class="text-lg font-bold mb-3 line-clamp-2">{{ achievement.projectName }}</h3>
              <div v-if="achievement.projectAmount" class="text-2xl font-bold text-hailong-primary">
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
import { onMounted } from 'vue'
import { useResource } from '@/composables/useResource'
import AsyncState from '@/components/common/AsyncState.vue'
import { getMajorAchievements } from '@/api/config'

defineEmits(['achievement-click'])

const { data: achievementsList, loading, error, execute } = useResource([])
const loadAchievements = () => execute(async signal => {
  const response = await getMajorAchievements({ signal })
  if (!response.success || !response.data) throw new Error('内容加载失败，请重试')
  return response.data.filter(item => item.status !== false)
})

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

.animate-scroll:hover, .animate-scroll:focus-within {
  animation-play-state: paused;
}
@media (prefers-reduced-motion: reduce) { .animate-scroll { animation: none; flex-wrap: wrap; } }
</style>

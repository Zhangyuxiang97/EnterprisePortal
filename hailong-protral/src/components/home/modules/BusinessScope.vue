<template>
  <div class="py-24 bg-gradient-to-b from-blue-50/30 to-gray-50">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold text-slate-800 mb-4 font-tech tracking-tight">业务范围</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
      </div>
      <div v-if="loading" class="text-center py-8 text-gray-500">加载中...</div>
      <div v-else-if="businessList.length === 0" class="text-center py-8 text-gray-500">暂无业务范围数据</div>
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        <div v-for="business in businessList" :key="business.id"
          @click="$emit('business-click', business.id)"
          class="group relative bg-white rounded-2xl overflow-hidden shadow-lg hover:shadow-2xl transition-all duration-300 border border-gray-200 hover:border-hailong-primary cursor-pointer">
          <div class="h-48 overflow-hidden bg-gray-200">
            <img v-if="business.imageUrl" :src="business.imageUrl" :alt="business.name"
              class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
            <div v-else class="w-full h-full flex items-center justify-center text-gray-400">
              <svg class="w-16 h-16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
              </svg>
            </div>
          </div>
          <div class="p-6">
            <h3 class="text-2xl font-bold text-gray-900 mb-3 group-hover:text-hailong-primary transition-colors">{{
              business.name }}</h3>
            <p class="text-gray-600 mb-4">{{ business.description }}</p>
            <div v-if="business.features && business.features.length > 0">
              <ul class="space-y-2">
                <li v-for="(feature, index) in business.features.slice(0, 4)" :key="index" class="text-sm text-gray-500 flex items-center">
                  <span class="w-1.5 h-1.5 bg-hailong-secondary rounded-full mr-2"></span>{{ feature }}
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getBusinessScope } from '@/api/config'

defineEmits(['business-click'])

const loading = ref(false)
const businessList = ref([])

const loadBusinessScope = async () => {
  loading.value = true
  try {
    const response = await getBusinessScope()
    if (response.success && response.data) {
      businessList.value = response.data
    }
  } catch (error) {
    console.error('加载业务范围失败:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadBusinessScope()
})
</script>

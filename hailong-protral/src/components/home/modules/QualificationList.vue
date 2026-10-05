<template>
  <div v-if="loading || error || qualificationsList.length" class="home-section">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold text-slate-800 mb-4 font-tech tracking-tight">企业资质</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
      </div>
      <div class="bg-white rounded-2xl p-8 md:p-12 shadow-lg border border-gray-200">
        <AsyncState v-if="loading || error" :loading="loading" :error="error" @retry="loadQualifications" />
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 md:gap-6">
          <div v-for="qualification in qualificationsList" :key="qualification.id"
            @click="$emit('qualification-click', qualification.id)"
            class="flex items-center p-4 bg-gradient-to-br from-hailong-primary/5 to-hailong-secondary/5 rounded-xl hover:shadow-md hover:scale-105 transition-all duration-300 cursor-pointer group">
            <div
              class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0 group-hover:bg-hailong-primary/20 transition-colors">
              <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <span class="text-gray-800 font-medium text-sm group-hover:text-hailong-primary transition-colors">{{
              qualification.name }}</span>
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
import { getCompanyQualifications } from '@/api/config'

defineEmits(['qualification-click'])

const { data: qualificationsList, loading, error, execute } = useResource([])
const loadQualifications = () => execute(async signal => {
  const response = await getCompanyQualifications({ signal })
  if (!response.success || !response.data) throw new Error('内容加载失败，请重试')
  return response.data.filter(item => item.status !== false)
})

onMounted(() => {
  loadQualifications()
})
</script>

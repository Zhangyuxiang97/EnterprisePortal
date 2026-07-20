<template>
  <!-- 交易数据可视化 -->
  <div class="py-24 bg-gradient-to-b from-white via-slate-50/50 to-gray-50/50">
    <div class="container-wide">
      <div class="text-center mb-16">
        <h2 class="text-3xl md:text-4xl font-extrabold text-slate-800 mb-4 font-tech tracking-tight">交易数据可视化</h2>
        <div class="w-12 h-1 bg-gradient-to-r from-hailong-primary to-hailong-secondary mx-auto rounded-full mt-3"></div>
      </div>
      
      <div v-if="statisticsLoading" class="text-center py-20 bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.01)]">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
        <p class="mt-4 text-slate-400 text-sm">正在加载统计分析数据...</p>
      </div>
      
      <div v-else-if="!statistics.totalProjects" class="text-center py-20 bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.01)]">
        <div class="w-16 h-16 bg-slate-50 text-slate-300 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 002 2h2a2 2 0 002-2z" />
          </svg>
        </div>
        <p class="text-slate-400 text-sm">暂无交易统计数据可用</p>
      </div>
      
      <div v-else>
        <!-- 4大指标数据看板 (横向分布，包含高端图表指示图标) -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
          <!-- 1. 项目总数 -->
          <div class="group bg-white rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 flex items-center justify-between">
            <div class="min-w-0">
              <div class="text-xs font-bold text-slate-400 tracking-wider mb-1.5 uppercase">项目总数</div>
              <div class="text-2xl md:text-3xl font-extrabold text-slate-800 font-tech truncate">
                {{ statistics.totalProjects || 0 }}
              </div>
            </div>
            <div class="w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 bg-blue-50 text-hailong-primary group-hover:scale-105 transition-all">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
              </svg>
            </div>
          </div>

          <!-- 2. 交易总额 -->
          <div class="group bg-white rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 flex items-center justify-between">
            <div class="min-w-0">
              <div class="text-xs font-bold text-slate-400 tracking-wider mb-1.5 uppercase">交易总额</div>
              <div class="text-2xl md:text-3xl font-extrabold text-slate-800 font-tech truncate">
                {{ formatTotalAmount(statistics.totalAmount) }}
              </div>
            </div>
            <div class="w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 bg-purple-50 text-hailong-secondary group-hover:scale-105 transition-all">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>

          <!-- 3. 政府采购项目数 -->
          <div class="group bg-white rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 flex items-center justify-between">
            <div class="min-w-0">
              <div class="text-xs font-bold text-slate-400 tracking-wider mb-1.5 uppercase">政府采购项目</div>
              <div class="text-2xl md:text-3xl font-extrabold text-slate-800 font-tech truncate">
                {{ getProjectTypeCount('政府采购') }}
              </div>
            </div>
            <div class="w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 bg-blue-50 text-hailong-primary group-hover:scale-105 transition-all">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
            </div>
          </div>

          <!-- 4. 建设工程项目数 -->
          <div class="group bg-white rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 flex items-center justify-between">
            <div class="min-w-0">
              <div class="text-xs font-bold text-slate-400 tracking-wider mb-1.5 uppercase">建设工程项目</div>
              <div class="text-2xl md:text-3xl font-extrabold text-slate-800 font-tech truncate">
                {{ getProjectTypeCount('建设工程') }}
              </div>
            </div>
            <div class="w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 bg-purple-50 text-hailong-secondary group-hover:scale-105 transition-all">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 3.055A9.001 9.001 0 1020.945 13H11V3.055z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.488 9H15V3.512A9.025 9.025 0 0120.488 9z" />
              </svg>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <!-- 交易类型占比 -->
          <div class="bg-white rounded-2xl p-8 shadow-[0_8px_30px_rgb(0,0,0,0.02)] border border-slate-100">
            <h3 class="text-lg font-bold text-slate-800 mb-6 pb-3.5 border-b border-slate-100 flex items-center gap-2">
              <span class="w-1.5 h-5 bg-hailong-primary rounded-full"></span>
              交易类型占比
            </h3>
            
            <div v-if="typeDistribution.length === 0" class="text-center py-8 text-slate-400 text-sm">暂无类型占比数据</div>
            <div v-else class="flex flex-col sm:flex-row items-center justify-center gap-8 py-4">
              <!-- 环形图 -->
              <div class="relative w-48 h-48 flex-shrink-0">
                <svg class="w-full h-full transform -rotate-90" viewBox="0 0 100 100">
                  <circle cx="50" cy="50" r="40" fill="none" stroke="#f8fafc" stroke-width="16" />
                  <circle v-for="(item, index) in typeDistribution" :key="item.type"
                    cx="50" cy="50" r="40" fill="none" :stroke="item.color"
                    stroke-width="16" 
                    :stroke-dasharray="`${item.percentage * 2.51} 251`"
                    :stroke-dashoffset="getStrokeDashOffset(index)"
                    class="transition-all duration-500 hover:stroke-width-[18]" />
                </svg>
                <div class="absolute inset-0 flex items-center justify-center">
                  <div class="text-center">
                    <div class="text-3xl font-extrabold text-slate-800 font-tech leading-none mb-1">
                      {{ statistics.totalProjects || 0 }}
                    </div>
                    <div class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">总项目数</div>
                  </div>
                </div>
              </div>
              
              <!-- 图例 -->
              <div class="grid grid-cols-1 gap-2.5 flex-1 w-full">
                <div v-for="item in typeDistribution" :key="item.type"
                  class="flex items-center justify-between p-3 bg-slate-50/50 hover:bg-slate-50 rounded-xl border border-slate-100/50 transition-colors">
                  <div class="flex items-center min-w-0 mr-4">
                    <div class="w-2.5 h-2.5 rounded-full mr-2.5 flex-shrink-0" :style="{ backgroundColor: item.color }"></div>
                    <span class="text-slate-600 font-bold text-xs truncate">{{ item.type }}</span>
                  </div>
                  <div class="text-right flex items-center gap-4">
                    <span class="text-sm font-extrabold font-tech" :style="{ color: item.color }">{{ item.count }}个</span>
                    <span class="text-[10px] font-bold text-slate-400 font-tech w-10 text-right">{{ item.percentage }}%</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 地区排行 -->
          <div class="bg-white rounded-2xl p-8 shadow-[0_8px_30px_rgb(0,0,0,0.02)] border border-slate-100">
            <h3 class="text-lg font-bold text-slate-800 mb-6 pb-3.5 border-b border-slate-100 flex items-center gap-2">
              <span class="w-1.5 h-5 bg-hailong-secondary rounded-full"></span>
              地区项目排行
            </h3>
            
            <div v-if="regionRanking.length === 0" class="text-center py-8 text-slate-400 text-sm">暂无地区排行数据</div>
            <div v-else class="space-y-4">
              <div v-for="(region, index) in regionRanking" :key="region.region" class="group relative">
                <div class="flex items-center justify-between mb-1.5">
                  <div class="flex items-center flex-1 min-w-0">
                    <div class="w-6 h-6 rounded-lg flex items-center justify-center mr-2.5 font-extrabold text-xs flex-shrink-0 transition-transform duration-300 group-hover:scale-105" :class="[
                      index === 0 ? 'bg-amber-500/10 text-amber-600' :
                      index === 1 ? 'bg-slate-400/10 text-slate-500' :
                      index === 2 ? 'bg-orange-500/10 text-orange-600' :
                      'bg-slate-100 text-slate-500'
                    ]">
                      {{ index + 1 }}
                    </div>
                    <span class="text-slate-700 font-bold text-sm truncate">{{ region.region }}</span>
                  </div>
                  <div class="text-right ml-4 flex items-baseline gap-2">
                    <span class="text-sm font-extrabold text-slate-700 font-tech">{{ region.projectCount }} 个项目</span>
                    <span class="text-[10px] font-semibold text-slate-400 font-tech">金额 {{ formatRegionAmount(region.amount) }}</span>
                  </div>
                </div>
                <!-- 进度条 -->
                <div class="relative h-1.5 bg-slate-100 rounded-full overflow-hidden">
                  <div
                    class="absolute inset-y-0 left-0 bg-gradient-to-r from-hailong-primary to-hailong-secondary rounded-full transition-all duration-500 group-hover:opacity-85"
                    :style="{ width: (region.projectCount / regionRanking[0].projectCount * 100) + '%' }">
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { getStatisticsOverview } from '@/api/home'

// 统计数据
const statisticsLoading = ref(false)
const statistics = ref({
  totalProjects: 0,
  totalAmount: 0,
  projectTypes: [],
  regionRanking: []
})

// 交易类型占比 - 过滤掉"政府采购"主类型，只显示4个细分类型
const typeDistribution = computed(() => {
  if (!statistics.value.projectTypes || statistics.value.projectTypes.length === 0) {
    return []
  }
  
  // 过滤掉"政府采购"主类型，只保留细分类型
  const filteredTypes = statistics.value.projectTypes.filter(
    item => item.type !== '政府采购'
  )
  
  // 为4个类型分配不同的颜色
  const colorMap = {
    '建设工程': '#10B981',      // 绿色
    '政府采购-货物': '#3B82F6',  // 蓝色
    '政府采购-服务': '#F59E0B',  // 橙色
    '政府采购-工程': '#8B5CF6'   // 紫色
  }
  
  return filteredTypes.map(item => ({
    type: item.type,
    count: item.count,
    percentage: Number(item.percentage.toFixed(1)),
    color: colorMap[item.type] || '#6B7280'
  }))
})

// 计算环形图的偏移量
const getStrokeDashOffset = (index) => {
  if (index === 0) return 0
  
  let offset = 0
  for (let i = 0; i < index; i++) {
    offset -= typeDistribution.value[i].percentage * 2.51
  }
  return offset
}

// 地区排行
const regionRanking = computed(() => {
  return statistics.value.regionRanking || []
})

// 获取指定类型的项目数量
const getProjectTypeCount = (type) => {
  const projectType = statistics.value.projectTypes?.find(item => item.type === type)
  return projectType ? projectType.count : 0
}

// 加载统计数据
const loadStatistics = async () => {
  statisticsLoading.value = true
  try {
    const response = await getStatisticsOverview()
    if (response.success && response.data) {
      statistics.value = {
        totalProjects: response.data.totalProjects || 0,
        totalAmount: response.data.totalAmount || 0,
        projectTypes: response.data.projectTypes || [],
        regionRanking: response.data.regionRanking || []
      }
    }
  } catch (error) {
    console.error('加载统计数据失败:', error)
  } finally {
    statisticsLoading.value = false
  }
}

// 格式化金额（项目金额）
const formatAmount = (amount) => {
  if (!amount) return '0'
  if (amount >= 10000) {
    return (amount / 10000).toFixed(2) + '亿'
  }
  return amount.toLocaleString() + '万'
}

// 格式化总金额
const formatTotalAmount = (amount) => {
  if (!amount || amount === 0) return '0'
  if (amount >= 10000) {
    return (amount / 10000).toFixed(2) + '亿'
  }
  return amount.toLocaleString() + '万'
}

// 格式化地区金额
const formatRegionAmount = (amount) => {
  if (!amount || amount === 0) return '0万'
  if (amount >= 10000) {
    return (amount / 10000).toFixed(2) + '亿'
  }
  return amount.toLocaleString() + '万'
}


onMounted(() => {
  loadStatistics()
})
</script>

<style scoped>
.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>

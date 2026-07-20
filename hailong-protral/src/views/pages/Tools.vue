<template>
  <div class="min-h-screen bg-slate-50/50">
    <Header />
    
    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <!-- 几何网格与光晕背景 -->
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          实用工具
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">
          为招投标与工程造价提供专业费用测算，助力项目快捷准确计算
        </p>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-12">
      <div class="container-wide">
        <div class="animate-fade-in">
          <!-- 工具列表 -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-12">
            <div
              v-for="tool in tools"
              :key="tool.id"
              class="group bg-white rounded-2xl overflow-hidden shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border border-slate-100 transition-all duration-300 flex flex-col justify-between"
            >
              <div class="relative h-48 overflow-hidden bg-gradient-to-br from-hailong-primary/5 to-hailong-secondary/5 border-b border-slate-100/50">
                <div class="absolute inset-0 flex items-center justify-center">
                  <svg class="w-24 h-24 text-hailong-primary/20 group-hover:text-hailong-primary/40 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" :d="tool.icon" />
                  </svg>
                </div>
                <div class="absolute top-4 right-4">
                  <span :class="[
                    'px-3 py-1 rounded-full text-xs font-bold shadow-sm',
                    tool.status === 'available' ? 'bg-emerald-50 text-emerald-700 border border-emerald-100/50' :
                    tool.status === 'coming' ? 'bg-amber-50 text-amber-700 border border-amber-100/50' :
                    'bg-slate-100 text-slate-600 border border-slate-200'
                  ]">
                    {{ tool.status === 'available' ? '可使用' : tool.status === 'coming' ? '即将上线' : '维护中' }}
                  </span>
                </div>
              </div>
              
              <div class="p-6 flex-1 flex flex-col justify-between">
                <div>
                  <h3 class="text-lg font-bold text-slate-800 mb-2 group-hover:text-hailong-primary transition-colors">
                    {{ tool.name }}
                  </h3>
                  <p class="text-slate-500 text-sm mb-6 leading-relaxed line-clamp-2">
                    {{ tool.description }}
                  </p>
                </div>
                
                <div class="flex items-center justify-end pt-4 border-t border-slate-100/60">
                  <button
                    v-if="tool.status === 'available'"
                    @click="handleToolClick(tool)"
                    class="px-5 py-2 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-xl hover:shadow-lg transition-all text-xs font-bold hover:-translate-y-0.5"
                  >
                    立即使用
                  </button>
                  <button
                    v-else
                    disabled
                    class="px-5 py-2 bg-slate-100 text-slate-400 rounded-xl cursor-not-allowed text-xs font-bold"
                  >
                    {{ tool.status === 'coming' ? '敬请期待' : '维护中' }}
                  </button>
                </div>
              </div>
            </div>
          </div>
          
          <!-- 招标代理服务费计算工具 -->
          <BiddingCalculator v-if="showBiddingCalculator" @close="showBiddingCalculator = false" class="mb-12" />

          <!-- 造价费用计算工具 -->
          <CostCalculator v-if="showCostCalculator" @close="showCostCalculator = false" class="mb-12" />

          <!-- 司法鉴定费用计算工具 -->
          <JudicialAppraisalCalculator v-if="showJudicialCalculator" @close="showJudicialCalculator = false" class="mb-12" />

          <!-- 使用说明 -->
          <div class="bg-white rounded-2xl p-8 border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.01)]">
            <h2 class="text-xl font-extrabold text-slate-800 mb-6 flex items-center gap-2">
              <span class="w-1 h-5 bg-hailong-primary rounded-full"></span>
              测算工具使用说明
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 text-slate-700">
              <div class="flex gap-3">
                <div class="flex-shrink-0 w-8 h-8 bg-blue-50 text-hailong-primary rounded-xl flex items-center justify-center font-bold text-sm">1</div>
                <div>
                  <h3 class="font-bold text-slate-800 text-sm mb-1">选择工具</h3>
                  <p class="text-xs text-slate-400 leading-normal">根据您的业务测算需求选择合适的计算工具</p>
                </div>
              </div>
              <div class="flex gap-3">
                <div class="flex-shrink-0 w-8 h-8 bg-blue-50 text-hailong-primary rounded-xl flex items-center justify-center font-bold text-sm">2</div>
                <div>
                  <h3 class="font-bold text-slate-800 text-sm mb-1">点击开启</h3>
                  <p class="text-xs text-slate-400 leading-normal">点击“立即使用”按钮，在页面下方展开测算面板</p>
                </div>
              </div>
              <div class="flex gap-3">
                <div class="flex-shrink-0 w-8 h-8 bg-blue-50 text-hailong-primary rounded-xl flex items-center justify-center font-bold text-sm">3</div>
                <div>
                  <h3 class="font-bold text-slate-800 text-sm mb-1">输入参数</h3>
                  <p class="text-xs text-slate-400 leading-normal">按照业务提示输入计算基数、行业类别等参数</p>
                </div>
              </div>
              <div class="flex gap-3">
                <div class="flex-shrink-0 w-8 h-8 bg-blue-50 text-hailong-primary rounded-xl flex items-center justify-center font-bold text-sm">4</div>
                <div>
                  <h3 class="font-bold text-slate-800 text-sm mb-1">计算结果</h3>
                  <p class="text-xs text-slate-400 leading-normal">系统实时演算得出各项费用明细，支持一键复制</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import BiddingCalculator from '@/components/BiddingCalculator.vue'
import CostCalculator from '@/components/CostCalculator.vue'
import JudicialAppraisalCalculator from '@/components/JudicialAppraisalCalculator.vue'

// 页面加载时滚动到顶部
onMounted(() => {
  window.scrollTo(0, 0)
})

// 工具列表
const tools = [
  {
    id: 1,
    name: '招标代理服务费计算工具',
    description: '根据河南省招标代理服务收费指导意见，快速计算工程、货物、服务类项目的代理服务费用',
    icon: 'M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z',
    status: 'available'
  },
  {
    id: 2,
    name: '造价费用计算工具',
    description: '依据河南省建设工程造价咨询行业服务收费市场参考价格，提供专业的造价咨询费用计算',
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    status: 'available'
  },
  {
    id: 3,
    name: '司法鉴定费用计算工具',
    description: '基于建设工程造价咨询收费基准价，采用差额定率分档累进方法，精确计算工程造价纠纷鉴定等各类咨询项目费用',
    icon: 'M3 6l3 1m0 0l-3 9a5.002 5.002 0 006.001 0M6 7l3 9M6 7l6-2m6 2l3-1m-3 1l-3 9a5.002 5.002 0 006.001 0M18 7l3 9m-3-9l-6-2m0-2v2m0 16V5m0 16H9m3 0h3',
    status: 'coming'
  }
]

// 显示计算器
const showBiddingCalculator = ref(false)
const showCostCalculator = ref(false)
const showJudicialCalculator = ref(false)

// 处理工具点击
const handleToolClick = (tool) => {
  // 先关闭所有计算器
  showBiddingCalculator.value = false
  showCostCalculator.value = false
  showJudicialCalculator.value = false
  
  // 打开对应的计算器
  if (tool.id === 1) {
    showBiddingCalculator.value = true
  } else if (tool.id === 2) {
    showCostCalculator.value = true
  } else if (tool.id === 3) {
    showJudicialCalculator.value = true
  }
  
  scrollToCalculator()
}

// 滚动到计算器位置
const scrollToCalculator = () => {
  setTimeout(() => {
    const calculator = document.querySelector('.bg-white.rounded-xl.shadow-2xl')
    if (calculator) {
      const headerHeight = 80 // 考虑固定头部的高度
      const calculatorTop = calculator.getBoundingClientRect().top + window.pageYOffset - headerHeight - 20
      window.scrollTo({ top: calculatorTop, behavior: 'smooth' })
    }
  }, 100)
}
</script>

<style scoped>
.container-wide {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>
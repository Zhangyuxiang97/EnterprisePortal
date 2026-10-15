<template>
  <div class="min-h-screen bg-slate-50/50">
    
    <!-- 页面头部 Hero Banner -->
    <PageBanner title="实用工具" description="为招投标与工程造价提供专业费用测算，助力项目快捷准确计算" />

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
              <div class="relative h-48 overflow-hidden bg-slate-100 border-b border-slate-100/50">
                <img
                  :src="tool.image"
                  :alt="tool.imageAlt"
                  width="960"
                  height="540"
                  loading="lazy"
                  decoding="async"
                  class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500 motion-reduce:transition-none motion-reduce:transform-none"
                />
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
                    class="portal-button portal-button--primary px-5 py-2 text-xs"
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
  </div>
</template>

<script setup>
import PageBanner from '@/components/common/PageBanner.vue'
import { ref, onMounted } from 'vue'
import BiddingCalculator from '@/components/BiddingCalculator.vue'
import CostCalculator from '@/components/CostCalculator.vue'
import JudicialAppraisalCalculator from '@/components/JudicialAppraisalCalculator.vue'
import biddingFeeImage from '@/assets/tool-defaults/bidding-fee.webp'
import costFeeImage from '@/assets/tool-defaults/cost-fee.webp'
import judicialFeeImage from '@/assets/tool-defaults/judicial-fee.webp'

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
    image: biddingFeeImage,
    imageAlt: '招标文件、费用测算表与计算器',
    status: 'available'
  },
  {
    id: 2,
    name: '造价费用计算工具',
    description: '依据河南省建设工程造价咨询行业服务收费市场参考价格，提供专业的造价咨询费用计算',
    image: costFeeImage,
    imageAlt: '工程图纸、比例尺、建筑模型与计算器',
    status: 'available'
  },
  {
    id: 3,
    name: '司法鉴定费用计算工具',
    description: '基于建设工程造价咨询收费基准价，采用差额定率分档累进方法，精确计算工程造价纠纷鉴定等各类咨询项目费用',
    image: judicialFeeImage,
    imageAlt: '平衡天平、工程鉴定资料与费用计算器',
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

</style>

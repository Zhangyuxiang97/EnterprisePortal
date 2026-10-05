<template>
  <div class="bg-white rounded-xl shadow-2xl p-8 border border-hailong-primary/20">
    <div class="flex items-center justify-between mb-6">
      <h2 class="text-3xl font-bold text-hailong-dark flex items-center gap-3">
        <svg class="w-8 h-8 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        造价费用计算工具
      </h2>
      <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 transition-colors">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- 输入区域 -->
      <div class="space-y-6">
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-3">咨询项目类型</label>
          <select 
            v-model="selectedProject" 
            @change="calculate"
            class="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-hailong-primary focus:outline-none transition-colors text-lg"
          >
            <option value="">请选择项目类型</option>
            <option v-for="project in projects" :key="project.id" :value="project.id">
              {{ project.name }}
            </option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-3">工程类别</label>
          <select 
            v-model="selectedEngineering" 
            @change="calculate"
            class="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-hailong-primary focus:outline-none transition-colors text-lg"
          >
            <option value="">请选择工程类别</option>
            <option v-for="eng in engineeringTypes" :key="eng.id" :value="eng.id">
              {{ eng.name }} (系数: {{ eng.coefficient }})
            </option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-3">{{ feeBaseLabel }}（万元）</label>
          <input 
            v-model.number="feeBase" 
            type="number" 
            step="0.01"
            min="0"
            placeholder="请输入金额(万元)"
            @input="calculate"
            class="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-hailong-primary focus:outline-none transition-colors text-lg"
          />
          <p class="text-xs text-gray-500 mt-2">请输入正数，支持小数</p>
        </div>

        <div v-if="showSteelOption">
          <label class="flex items-center space-x-2 mb-3">
            <input 
              v-model="needSteelCalculation" 
              type="checkbox"
              @change="calculate"
              class="w-5 h-5 text-hailong-primary border-gray-300 rounded focus:ring-hailong-primary"
            />
            <span class="text-sm font-semibold text-gray-700">需要钢筋工程精细计量</span>
          </label>
          
          <div v-if="needSteelCalculation">
            <input 
              v-model.number="steelWeight" 
              type="number" 
              step="0.01"
              min="0"
              placeholder="请输入钢筋重量（吨）"
              @input="calculate"
              class="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-hailong-primary focus:outline-none transition-colors text-lg"
            />
            <p class="text-xs text-gray-500 mt-2">钢筋重量（吨），{{ steelRate }}元/吨</p>
          </div>
        </div>

        <button
          @click="calculate"
          :disabled="!canCalculate"
          class="portal-button portal-button--primary w-full py-4 text-lg"
        >
          开始计算
        </button>
      </div>

      <!-- 输出区域 -->
      <div class="space-y-6">
        <div class="bg-gradient-to-br from-hailong-primary/5 to-hailong-secondary/5 rounded-xl p-6 border border-hailong-primary/20">
          <h3 class="text-lg font-bold text-hailong-dark mb-4">计算结果</h3>
          
          <div class="space-y-4">
            <div v-if="result" class="space-y-2 text-sm mb-4">
              <div class="flex justify-between items-center">
                <span class="text-gray-600">项目类型：</span>
                <span class="font-medium text-gray-800">{{ result.projectName }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-gray-600">工程类别：</span>
                <span class="font-medium text-gray-800">{{ result.engineeringName }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-gray-600">收费基数：</span>
                <span class="font-medium text-gray-800">{{ result.feeBase.toFixed(2) }} 万元</span>
              </div>
            </div>

            <div v-if="result && result.breakdown.length > 0" class="border-t border-hailong-primary/20 pt-3 mb-3">
              <div class="text-xs text-gray-600 mb-2 font-semibold">分档计算明细：</div>
              <div class="space-y-1">
                <div v-for="(item, index) in result.breakdown" :key="index" class="flex justify-between items-center text-sm">
                  <span class="text-gray-600">{{ item.range }} ({{ item.rate }}‰)</span>
                  <span class="text-gray-800 font-medium">{{ item.amount.toFixed(2) }} 元</span>
                </div>
              </div>
            </div>

            <div class="flex justify-between items-center py-3 border-t border-gray-200">
              <span class="text-gray-700 font-medium">基准价小计：</span>
              <span class="text-xl font-bold text-gray-800">{{ formatCurrency(result ? result.baseTotal : 0) }}</span>
            </div>

            <div v-if="result" class="space-y-2 text-sm">
              <div class="flex justify-between items-center">
                <span class="text-gray-600">专业调整系数：</span>
                <span class="font-medium text-gray-800">{{ result.engineeringCoefficient }}</span>
              </div>
              <div v-if="result.steelFee > 0" class="flex justify-between items-center">
                <span class="text-gray-600">钢筋精细计量附加费：</span>
                <span class="font-medium text-gray-800">{{ formatCurrency(result.steelFee) }}</span>
              </div>
            </div>
            
            <div class="flex justify-between items-center py-3 bg-green-50 rounded-lg px-3">
              <span class="text-gray-700 font-medium">总费用：</span>
              <span class="text-2xl font-bold text-green-600">{{ formatCurrency(result ? result.totalFee : 0) }}</span>
            </div>
          </div>
        </div>

        <div v-if="result && result.report" class="bg-gray-50 rounded-xl p-6 border border-gray-200">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-bold text-hailong-dark">计算报告</h3>
            <button
              @click="copyReport"
              class="flex items-center gap-2 px-4 py-2 bg-hailong-primary text-white rounded-lg hover:bg-hailong-secondary transition-colors text-sm font-medium"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
              </svg>
              一键复制
            </button>
          </div>
          <div class="text-sm text-gray-700 whitespace-pre-wrap leading-relaxed" style="font-family: 'Courier New', Courier, monospace;">{{ result.report }}</div>
        </div>
      </div>
    </div>

    <!-- 页面底部说明 -->
    <div class="mt-8 pt-6 border-t border-gray-200">
      <div class="text-sm text-gray-600 leading-relaxed space-y-2">
        <p><span class="font-semibold text-gray-700">说明：</span></p>
        <ul class="list-disc list-inside space-y-1 ml-4">
          <li>本计算器提供的费用测算结果仅供参考估价，不能作为实际收费依据，实际费用应以双方签订的正式咨询服务合同为准。</li>
          <li>工程造价咨询收费基准价采取差额定率分档累进方法计算</li>
          <li>不同专业工程有相应的调整系数</li>
          <li>钢筋精细计量需额外计算附加费用（工程量清单编制12元/吨，结算审查18元/吨）</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

defineEmits(['close'])

const selectedProject = ref('')
const selectedEngineering = ref('')
const feeBase = ref(null)
const needSteelCalculation = ref(false)
const steelWeight = ref(null)
const result = ref(null)

const projects = [
  { id: 1, name: '工程概算编制', rates: [3, 2.5, 2, 1.8, 1.6, 1.5], baseType: '建安工程费用' },
  { id: 2, name: '工程量清单编制', rates: [5, 4, 3, 2.2, 1.8, 1.5], baseType: '建安工程费用', supportSteel: true },
  { id: 3, name: '招标控制价编制', rates: [2.0, 1.8, 1.6, 1.4, 1.2, 1.0], baseType: '建安工程费用' },
  { id: 4, name: '工程预算编制', rates: [4, 3.5, 3, 2.5, 2, 1.5], baseType: '建安工程费用' },
  { id: 5, name: '工程结算审查', rates: [8, 7, 6, 5.0, 4.0, 3.0], baseType: '建安工程费用', supportSteel: true },
  { id: 6, name: '全过程造价咨询', rates: [0, 0, 0, 12, 10, 8], baseType: '建安工程费用', minBase: 2000 },
  { id: 7, name: '竣工决算编制', rates: [2, 1.5, 1.2, 1.0, 0.8, 0.6], baseType: '建安工程费用' }
]

const engineeringTypes = [
  { id: 1, name: '房屋建筑工程', coefficient: 1.0 },
  { id: 2, name: '市政工程', coefficient: 0.8 },
  { id: 3, name: '水利水电工程', coefficient: 0.9 },
  { id: 4, name: '机场场道工程', coefficient: 0.7 },
  { id: 5, name: '公路、道路工程', coefficient: 0.8 },
  { id: 6, name: '城市轨道工程', coefficient: 0.8 },
  { id: 7, name: '桥梁、隧道工程', coefficient: 0.7 },
  { id: 8, name: '港口工程', coefficient: 0.8 },
  { id: 9, name: '井巷矿山工程', coefficient: 1.1 },
  { id: 10, name: '园林绿化工程', coefficient: 1.1 },
  { id: 11, name: '装饰装修工程', coefficient: 1.2 },
  { id: 12, name: '仿古建筑工程', coefficient: 1.2 },
  { id: 13, name: '安装工程', coefficient: 1.2 },
  { id: 14, name: '其他工程', coefficient: 1.0 }
]

const brackets = [
  { max: 200, label: 'X≤200万元' },
  { min: 200, max: 500, label: '200＜X≤500万元' },
  { min: 500, max: 2000, label: '500＜X≤2000万元' },
  { min: 2000, max: 10000, label: '2000＜X≤10000万元' },
  { min: 10000, max: 50000, label: '10000＜X≤50000万元' },
  { min: 50000, max: Infinity, label: 'X＞50000万元' }
]

const currentProject = computed(() => {
  return projects.find(p => p.id === selectedProject.value)
})

const currentEngineering = computed(() => {
  return engineeringTypes.find(e => e.id === selectedEngineering.value)
})

const feeBaseLabel = computed(() => {
  return currentProject.value ? currentProject.value.baseType : '收费基数'
})

const showSteelOption = computed(() => {
  return currentProject.value && currentProject.value.supportSteel
})

const steelRate = computed(() => {
  return currentProject.value && currentProject.value.id === 2 ? 12 : 18
})

const canCalculate = computed(() => {
  if (!selectedProject.value || !selectedEngineering.value || !feeBase.value || feeBase.value <= 0) {
    return false
  }
  
  if (currentProject.value.minBase && feeBase.value < currentProject.value.minBase) {
    return false
  }
  
  if (needSteelCalculation.value && (!steelWeight.value || steelWeight.value <= 0)) {
    return false
  }
  
  return true
})

const calculate = () => {
  if (!canCalculate.value) {
    result.value = null
    return
  }
  
  const project = currentProject.value
  const engineering = currentEngineering.value
  const breakdown = []
  let baseTotal = 0
  
  let remainingAmount = feeBase.value
  
  for (let i = 0; i < brackets.length; i++) {
    const bracket = brackets[i]
    const rate = project.rates[i]
    
    if (rate === 0) continue
    
    let amountInBracket = 0
    
    if (i === 0) {
      amountInBracket = Math.min(remainingAmount, bracket.max)
    } else if (bracket.max === Infinity) {
      if (remainingAmount > bracket.min) {
        amountInBracket = remainingAmount - bracket.min
      }
    } else {
      if (remainingAmount > bracket.min) {
        amountInBracket = Math.min(remainingAmount, bracket.max) - bracket.min
      }
    }
    
    if (amountInBracket > 0) {
      const fee = amountInBracket * 10000 * (rate / 1000)
      breakdown.push({
        range: bracket.label,
        amount: fee,
        rate: rate
      })
      baseTotal += fee
    }
    
    if (remainingAmount <= (bracket.max || Infinity)) {
      break
    }
  }
  
  const adjustedTotal = baseTotal * engineering.coefficient
  
  let steelFee = 0
  if (needSteelCalculation.value && steelWeight.value > 0) {
    steelFee = steelWeight.value * steelRate.value
  }
  
  const totalFee = adjustedTotal + steelFee
  
  let report = `项目类型：${project.name}\n`
  report += `工程类别：${engineering.name}\n`
  report += `收费基数：${feeBase.value}万元\n`
  report += `------------------------------\n`
  report += `分档计算明细：\n`
  
  breakdown.forEach(item => {
    report += `${item.range}：${item.rate}‰ = ${item.amount.toFixed(2)}元\n`
  })
  
  report += `------------------------------\n`
  report += `基准价小计：${baseTotal.toFixed(2)}元\n`
  report += `专业调整系数：${engineering.coefficient}\n`
  
  if (steelFee > 0) {
    report += `钢筋精细计量附加费：${steelWeight.value}吨 × ${steelRate.value}元/吨 = ${steelFee.toFixed(2)}元\n`
  }
  
  report += `------------------------------\n`
  report += `总费用：${totalFee.toFixed(2)}元`
  
  result.value = {
    projectName: project.name,
    engineeringName: engineering.name,
    feeBase: feeBase.value,
    breakdown: breakdown,
    baseTotal: baseTotal,
    engineeringCoefficient: engineering.coefficient,
    steelFee: steelFee,
    totalFee: totalFee,
    report: report
  }
}

const formatCurrency = (value) => {
  if (!value) return '¥0.00'
  return `¥${value.toFixed(2)}`
}

const copyReport = async () => {
  try {
    await navigator.clipboard.writeText(result.value.report)
    alert('计算报告已复制到剪贴板！')
  } catch (err) {
    const textarea = document.createElement('textarea')
    textarea.value = result.value.report
    textarea.style.position = 'fixed'
    textarea.style.opacity = '0'
    document.body.appendChild(textarea)
    textarea.select()
    try {
      document.execCommand('copy')
      alert('计算报告已复制到剪贴板！')
    } catch (e) {
      alert('复制失败，请手动复制')
    }
    document.body.removeChild(textarea)
  }
}
</script>

<style scoped>
input[type="number"]::-webkit-inner-spin-button,
input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type="number"] {
  -moz-appearance: textfield;
}
</style>
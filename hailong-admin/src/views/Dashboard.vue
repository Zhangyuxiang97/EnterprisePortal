<template>
  <div class="dashboard-container">
    <el-row :gutter="20">
      <!-- 统计卡片区域 -->
      <el-col :xs="12" :sm="12" :md="6" :lg="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon total-projects">
              <el-icon :size="32"><Document /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-label">公告总数</div>
              <div class="stat-value">{{ statistics.totalProjects || 0 }}</div>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="12" :sm="12" :md="6" :lg="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon total-amount">
              <el-icon :size="32"><Money /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-label">已记录中标金额</div>
              <div class="stat-value" :title="`${statistics.totalAmount?.toLocaleString()} 万元`">{{ formatAmount(statistics.totalAmount) }}</div>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="12" :sm="12" :md="6" :lg="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon gov-procurement">
              <el-icon :size="32"><ShoppingCart /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-label">政府采购公告</div>
              <div class="stat-value">{{ getProjectTypeCount('政府采购') }}</div>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="12" :sm="12" :md="6" :lg="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon construction">
              <el-icon :size="32"><OfficeBuilding /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-label">建设工程公告</div>
              <div class="stat-value">{{ getProjectTypeCount('建设工程') }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <p class="statistics-note">按公告记录统计，同一项目可能包含多条公告。中标 / 成交金额覆盖 {{ statistics.knownAwardAmountCount || 0 }} 条结果公告，按已记录金额汇总。</p>
    <!-- 图表区域 -->
    <el-row :gutter="20" style="margin-top: 20px">
      <!-- 交易类型饼图 -->
      <el-col :xs="24" :sm="24" :md="12" :lg="12">
        <el-card shadow="hover" class="chart-card">
          <v-chart
            class="chart"
            :option="pieChartOption"
            :autoresize="true"
          />
        </el-card>
      </el-col>

      <!-- 地区排行柱状图 -->
      <el-col :xs="24" :sm="24" :md="12" :lg="12">
        <el-card shadow="hover" class="chart-card">
          <v-chart
            class="chart"
            :option="barChartOption"
            :autoresize="true"
          />
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { Document, Money, ShoppingCart, OfficeBuilding } from '@element-plus/icons-vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { PieChart, BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import { statisticsApi } from '@/api'
import { getPieChartOption, getBarChartOption } from '@/utils/chartOptions'
import { ElMessage } from 'element-plus'

// 注册 ECharts 组件
use([
  CanvasRenderer,
  PieChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

// 统计数据
const statistics = ref({
  totalProjects: 0,
  totalAmount: 0,
  projectTypes: [],
  regionRanking: []
})

// 定时器
let refreshTimer = null

// 饼图配置 - 过滤掉"政府采购"主类型，只显示细分类型
const pieChartOption = computed(() => {
  // 过滤掉"政府采购"主类型，只保留细分类型和建设工程
  const filteredTypes = statistics.value.projectTypes.filter(
    item => item.type !== '政府采购'
  )
  const gov = statistics.value.projectTypes.find(item => item.type === '政府采购')?.count || 0
  const classified = filteredTypes.filter(item => item.type.startsWith('政府采购-')).reduce((sum, item) => sum + item.count, 0)
  if (gov > classified) filteredTypes.push({ type: '政府采购-待确认', count: gov - classified, percentage: (gov - classified) / statistics.value.totalProjects * 100 })
  return getPieChartOption(filteredTypes)
})

// 柱状图配置
const barChartOption = computed(() => {
  return getBarChartOption(statistics.value.regionRanking)
})

/**
 * 格式化金额显示
 */
const formatAmount = (amount) => {
  if (!amount || amount === 0) {
    return '暂无数据'
  }
  return amount >= 10000 ? `${(amount / 10000).toFixed(2)}亿元` : `${amount.toLocaleString()}万元`
}

/**
 * 获取指定类型的项目数量
 */
const getProjectTypeCount = (type) => {
  const projectType = statistics.value.projectTypes.find(item => item.type === type)
  return projectType ? projectType.count : 0
}

/**
 * 加载统计数据
 */
const loadStatistics = async () => {
  try {
    const response = await statisticsApi.system.getHomeStatistics()
    if (response.success) {
      statistics.value = response.data
    } else {
      ElMessage.error(response.message || '获取统计数据失败')
    }
  } catch (error) {

    notifyError(error, '加载统计数据失败，请稍后重试')
  }
}

/**
 * 启动自动刷新
 */
const startAutoRefresh = () => {
  // 每30秒刷新一次数据
  refreshTimer = setInterval(() => {
    loadStatistics()
  }, 30000)
}

/**
 * 停止自动刷新
 */
const stopAutoRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
}

onMounted(() => {
  loadStatistics()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})
</script>

<style scoped>
.statistics-note { color: #64748b; font-size: 13px; line-height: 1.8; }
.dashboard-container {
  padding: 20px;
  background-color: #f0f2f5;
  min-height: calc(100vh - 100px);
}

/* 统计卡片样式 */
.stat-card {
  min-height: 106px;
  margin-bottom: 20px;
}

.stat-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 14px;
}

.stat-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  flex-shrink: 0;
  height: 48px;
  border-radius: 12px;
  color: white;
}

.stat-icon.total-projects {
  background: #edf3f8;
  color: #245b85;
}

.stat-icon.total-amount {
  background: #edf3f8;
  color: #245b85;
}

.stat-icon.gov-procurement {
  background: #edf3f8;
  color: #245b85;
}

.stat-icon.construction {
  background: #edf3f8;
  color: #245b85;
}

.stat-info {
  min-width: 0;
  flex: 1;
}

.stat-label {
  font-size: 14px;
  color: #909399;
  margin-bottom: 8px;
}

.stat-value {
  white-space: nowrap;
  font-variant-numeric: tabular-nums;
  font-size: 24px;
  font-weight: bold;
  color: #303133;
}

/* 图表卡片样式 */
.chart-card {
  margin-bottom: 20px;
}

.chart {
  height: 400px;
  width: 100%;
}

/* 响应式调整 */
@media (max-width: 768px) {
  .dashboard-container {
    padding: 10px;
  }

  .stat-content {
    gap: 15px;
  }

  .stat-icon {
    width: 50px;
    height: 50px;
  }

  .stat-icon :deep(.el-icon) {
    font-size: 24px !important;
  }

  .stat-label {
    font-size: 12px;
  }

  .stat-value {
  white-space: nowrap;
  font-variant-numeric: tabular-nums;
    font-size: 20px;
  }

  .chart {
    height: 300px;
  }
}
</style>

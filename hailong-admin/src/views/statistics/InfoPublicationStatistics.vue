<template>
  <div class="page-container">
    <!-- 概览卡片 -->
    <el-row :gutter="20" class="overview-cards">
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #409eff;">
              <el-icon><DocumentCopy /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.totalPublications || 0 }}</div>
              <div class="stat-label">信息总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #67c23a;">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.todayAdded || 0 }}</div>
              <div class="stat-label">今日入库</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #e6a23c;">
              <el-icon><View /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.totalViews || 0 }}</div>
              <div class="stat-label">总浏览量</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #f56c6c;">
              <el-icon><TrendCharts /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.averageViews?.toFixed(1) || 0 }}</div>
              <div class="stat-label">平均浏览量</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20">
      <!-- 发布趋势 -->
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <div class="card-header">
              <span>发布趋势（最近30天 · 按启用记录的发布时间）</span>
            </div>
          </template>
          <div ref="trendChartRef" style="height: 350px;"></div>
        </el-card>
      </el-col>

      <!-- 信息类型分布 -->
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <span>信息类型分布</span>
          </template>
          <div ref="typeChartRef" style="height: 350px;"></div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" style="margin-top: 20px;">
      <!-- 发布单位统计 -->
      <el-col :span="24">
        <el-card>
          <template #header>
            <span>发布单位统计 TOP 10</span>
          </template>
          <div ref="authorChartRef" style="height: 400px;"></div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 热门信息 -->
    <el-card style="margin-top: 20px;">
      <template #header>
        <div class="card-header">
          <span>热门信息 TOP 10</span>
        </div>
      </template>
      <el-table :data="hotInfo" v-loading="loading" border stripe>
        <el-table-column type="index" label="排名" width="60" align="center" />
        <el-table-column prop="title" label="信息标题" min-width="300" show-overflow-tooltip />
        <el-table-column prop="type" label="信息类型" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getTypeColor(row.type)">
              {{ getTypeName(row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="viewCount" label="浏览量" width="100" align="center" sortable />
        <el-table-column prop="publishDate" label="发布时间" width="180" align="center">
          <template #default="{ row }">
            {{ formatDateTime(row.publishDate) }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup>
import { useLatestRequest } from '@/composables/useLatestRequest'
const loadOverviewRequest = useLatestRequest()
const loadTrendDataRequest = useLatestRequest()
const loadTypeDistributionRequest = useLatestRequest()
const loadAuthorStatisticsRequest = useLatestRequest()
const loadHotInfoRequest = useLatestRequest()

import { notifyError } from '@/utils/errors'
import { localDate } from '@/utils/form'
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import { DocumentCopy, Calendar, View, TrendCharts } from '@element-plus/icons-vue'
import { statisticsApi } from '@/api'
import * as echarts from '@/utils/echarts'
import { getHorizontalBarChartOption, getDoughnutChartOption } from '@/utils/chartOptions'

// 概览数据
const overview = reactive({
  totalPublications: 0,
  todayAdded: 0,
  weekAdded: 0,
  monthAdded: 0,
  newsCenterCount: 0,
  policyRegulationCount: 0,
  totalViews: 0,
  averageViews: 0
})

// 图表实例
let trendChart = null
let typeChart = null
let authorChart = null

const trendChartRef = ref(null)
const typeChartRef = ref(null)
const authorChartRef = ref(null)

// 热门信息
const hotInfo = ref([])
const loading = ref(false)

// 类型映射
const typeNameMap = {
  'COMPANY_NEWS': '新闻中心',
  'POLICY_REGULATION': '政策法规'
}

const typeColorMap = {
  'COMPANY_NEWS': 'primary',
  'POLICY_REGULATION': 'success'
}

/**
 * 获取类型名称
 */
const getTypeName = (type) => {
  return typeNameMap[type] || type || '-'
}

/**
 * 获取类型颜色
 */
const getTypeColor = (type) => {
  return typeColorMap[type] || 'info'
}

/**
 * 格式化日期时间
 */
const formatDateTime = (dateStr) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

/**
 * 加载概览数据
 */
const loadOverview = async () => {
  const requestId = loadOverviewRequest.begin()
  try {
    const res = await statisticsApi.infoPublication.getOverview()
    if (!loadOverviewRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      Object.assign(overview, res.data)
    }
  } catch (error) {
    if (!loadOverviewRequest.isCurrent(requestId)) return

    notifyError(error, '加载概览数据失败')
  }
}

/**
 * 加载发布趋势
 */
const loadTrendData = async () => {
  const requestId = loadTrendDataRequest.begin()
  try {
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - 30)

    const params = {
      startDate: localDate(startDate),
      endDate: localDate(endDate),
      groupBy: 'day'
    }
    const res = await statisticsApi.infoPublication.getTrend(params)
    if (!loadTrendDataRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderTrendChart(res.data)
    }
  } catch (error) {
    if (!loadTrendDataRequest.isCurrent(requestId)) return

    notifyError(error, '加载趋势数据失败')
  }
}

/**
 * 渲染发布趋势图表
 */
const renderTrendChart = (data) => {
  if (!trendChart) {
    trendChart = echarts.init(trendChartRef.value)
  }

  const dates = data.map(item => item.date)
  const newsData = data.map(item => item.newsCenterCount)
  const policyData = data.map(item => item.policyRegulationCount)

  const option = {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      }
    },
    legend: {
      data: ['新闻中心', '政策法规']
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: '15%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: dates
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        name: '新闻中心',
        type: 'line',
        data: newsData,
        smooth: true,
        itemStyle: { color: '#409eff' },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(64, 158, 255, 0.3)' },
            { offset: 1, color: 'rgba(64, 158, 255, 0.05)' }
          ])
        }
      },
      {
        name: '政策法规',
        type: 'line',
        data: policyData,
        smooth: true,
        itemStyle: { color: '#67c23a' },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(103, 194, 58, 0.3)' },
            { offset: 1, color: 'rgba(103, 194, 58, 0.05)' }
          ])
        }
      }
    ]
  }

  trendChart.setOption(option)
}

/**
 * 加载类型分布
 */
const loadTypeDistribution = async () => {
  const requestId = loadTypeDistributionRequest.begin()
  try {
    const res = await statisticsApi.infoPublication.getTypeDistribution()
    if (!loadTypeDistributionRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      const typeData = res.data.map(item => ({
        name: item.typeName || getTypeName(item.type),
        value: item.count
      }))
      renderTypeChart(typeData)
    }
  } catch (error) {
    if (!loadTypeDistributionRequest.isCurrent(requestId)) return

    notifyError(error, '加载类型分布失败')
  }
}

/**
 * 渲染类型分布图表
 */
const renderTypeChart = (data) => {
  if (!typeChart) {
    typeChart = echarts.init(typeChartRef.value)
  }

  const option = getDoughnutChartOption(data, {
    radius: ['40%', '70%'],
    center: ['50%', '50%']
  })

  typeChart.setOption(option)
}

/**
 * 加载作者统计
 */
const loadAuthorStatistics = async () => {
  const requestId = loadAuthorStatisticsRequest.begin()
  try {
    const res = await statisticsApi.infoPublication.getAuthorStatistics()
    if (!loadAuthorStatisticsRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderAuthorChart(res.data)
    }
  } catch (error) {
    if (!loadAuthorStatisticsRequest.isCurrent(requestId)) return

    notifyError(error, '加载作者统计失败')
  }
}

/**
 * 渲染作者统计图表
 */
const renderAuthorChart = (data) => {
  if (!authorChart) {
    authorChart = echarts.init(authorChartRef.value)
  }

  // 取前10名
  const top10 = data.slice(0, 10)
  const chartData = top10.map(item => ({
    name: item.author || '未知作者',
    value: item.publishCount
  }))

  const option = getHorizontalBarChartOption(chartData, {
    color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
      { offset: 0, color: '#667eea' },
      { offset: 1, color: '#764ba2' }
    ]),
    showLabel: true
  })

  authorChart.setOption(option)
}

/**
 * 加载热门信息
 */
const loadHotInfo = async () => {
  const requestId = loadHotInfoRequest.begin()
  loading.value = true
  try {
    const params = {
      limit: 10
    }
    const res = await statisticsApi.infoPublication.getHotInfo(params)
    if (!loadHotInfoRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      hotInfo.value = res.data
    }
  } catch (error) {
    if (!loadHotInfoRequest.isCurrent(requestId)) return

    notifyError(error, '加载热门信息失败')
  } finally {
    if (loadHotInfoRequest.isCurrent(requestId)) {
      loading.value = false
    }
  }
}

/**
 * 窗口大小改变时重绘图表
 */
const handleResize = () => {
  if (trendChart) trendChart.resize()
  if (typeChart) typeChart.resize()
  if (authorChart) authorChart.resize()
}

onMounted(() => {
  loadOverview()
  loadTrendData()
  loadTypeDistribution()
  loadAuthorStatistics()
  loadHotInfo()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (trendChart) trendChart.dispose()
  if (typeChart) typeChart.dispose()
  if (authorChart) authorChart.dispose()
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
.page-container {
  padding: 0;
}

.overview-cards {
  margin-bottom: 20px;
}

.stat-card {
  display: flex;
  align-items: center;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  color: white;
  margin-right: 15px;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  color: #909399;
}

.chart-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
<template>
  <div class="page-container">
    <!-- 概览卡片 -->
    <el-row :gutter="20" class="overview-cards">
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #409eff;">
              <el-icon><Document /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.totalAnnouncements || 0 }}</div>
              <div class="stat-label">公告总数</div>
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
              <el-select v-model="trendBusinessType" size="small" style="width: 120px;" @change="loadTrendData">
                <el-option label="全部" value="" />
                <el-option label="政府采购" value="GOV_PROCUREMENT" />
                <el-option label="建设工程" value="CONSTRUCTION" />
              </el-select>
            </div>
          </template>
          <div ref="trendChartRef" style="height: 350px;"></div>
        </el-card>
      </el-col>

      <!-- 公告类型分布 -->
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <span>公告类型分布</span>
          </template>
          <div ref="typeChartRef" style="height: 350px;"></div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" style="margin-top: 20px;">
      <!-- 状态分布 -->
      <el-col :span="12">
        <el-card>
          <template #header>
            <span>公告状态分布</span>
          </template>
          <div ref="statusChartRef" style="height: 350px;"></div>
        </el-card>
      </el-col>

      <!-- 区域分布 -->
      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>项目区域分布 TOP 10</span>
              <el-select v-model="regionBusinessType" size="small" style="width: 120px;" @change="loadRegionDistribution">
                <el-option label="全部" value="" />
                <el-option label="政府采购" value="GOV_PROCUREMENT" />
                <el-option label="建设工程" value="CONSTRUCTION" />
              </el-select>
            </div>
          </template>
          <div ref="regionChartRef" style="height: 350px;"></div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 热门公告 -->
    <el-card style="margin-top: 20px;">
      <template #header>
        <div class="card-header">
          <span>热门公告 TOP 10</span>
          <el-select v-model="hotBusinessType" size="small" style="width: 120px;" @change="loadHotAnnouncements">
            <el-option label="全部" value="" />
            <el-option label="政府采购" value="GOV_PROCUREMENT" />
            <el-option label="建设工程" value="CONSTRUCTION" />
          </el-select>
        </div>
      </template>
      <el-table :data="hotAnnouncements" v-loading="loading" border stripe>
        <el-table-column type="index" label="排名" width="60" align="center" />
        <el-table-column prop="title" label="公告标题" min-width="300" show-overflow-tooltip />
        <el-table-column prop="businessType" label="业务类型" width="110" align="center">
          <template #default="{ row }">
            <el-tag :type="getBusinessTypeColor(row.businessType)">
              {{ getBusinessTypeName(row.businessType) }}
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
const loadStatusDistributionRequest = useLatestRequest()
const loadRegionDistributionRequest = useLatestRequest()
const loadHotAnnouncementsRequest = useLatestRequest()

import { notifyError } from '@/utils/errors'
import { localDate } from '@/utils/form'
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Document, Calendar, View, TrendCharts } from '@element-plus/icons-vue'
import { statisticsApi } from '@/api'
import * as echarts from '@/utils/echarts'
import { getDoughnutChartOption, getHorizontalBarChartOption } from '@/utils/chartOptions'

// 概览数据
const overview = reactive({
  totalAnnouncements: 0,
  todayAdded: 0,
  weekAdded: 0,
  monthAdded: 0,
  govProcurementCount: 0,
  constructionCount: 0,
  totalViews: 0,
  averageViews: 0
})

// 图表实例
let trendChart = null
let typeChart = null
let statusChart = null
let regionChart = null

const trendChartRef = ref(null)
const typeChartRef = ref(null)
const statusChartRef = ref(null)
const regionChartRef = ref(null)

// 筛选条件
const trendBusinessType = ref('')
const regionBusinessType = ref('')
const hotBusinessType = ref('')

// 热门公告
const hotAnnouncements = ref([])
const loading = ref(false)

/**
 * 获取业务类型名称
 */
const getBusinessTypeName = (type) => {
  const typeMap = {
    'GOV_PROCUREMENT': '政府采购',
    'CONSTRUCTION': '建设工程',
    '政府采购': '政府采购',
    '建设工程': '建设工程'
  }
  return typeMap[type] || type || '-'
}

/**
 * 获取业务类型颜色
 */
const getBusinessTypeColor = (type) => {
  if (type === 'GOV_PROCUREMENT' || type === '政府采购') {
    return 'primary'
  } else if (type === 'CONSTRUCTION' || type === '建设工程') {
    return 'success'
  }
  return 'info'
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
    const res = await statisticsApi.announcement.getOverview()
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
      businessType: trendBusinessType.value || undefined,
      groupBy: 'day'
    }
    const res = await statisticsApi.announcement.getTrend(params)
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
  const govData = data.map(item => item.govProcurementCount)
  const constructionData = data.map(item => item.constructionCount)

  const option = {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      }
    },
    legend: {
      data: ['政府采购', '建设工程']
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
        name: '政府采购',
        type: 'line',
        data: govData,
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
        name: '建设工程',
        type: 'line',
        data: constructionData,
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
 * 加载公告类型分布
 */
const loadTypeDistribution = async () => {
  const requestId = loadTypeDistributionRequest.begin()
  try {
    const res = await statisticsApi.announcement.getTypeDistribution()
    if (!loadTypeDistributionRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      const typeData = res.data.map(item => ({
        name: item.typeName || item.type,
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
 * 渲染公告类型分布图表
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
 * 加载状态分布
 */
const loadStatusDistribution = async () => {
  const requestId = loadStatusDistributionRequest.begin()
  try {
    const res = await statisticsApi.announcement.getStatusDistribution()
    if (!loadStatusDistributionRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      const statusData = res.data.map(item => ({
        name: item.statusName || item.status,
        value: item.count
      }))
      renderStatusChart(statusData)
    }
  } catch (error) {
    if (!loadStatusDistributionRequest.isCurrent(requestId)) return

    notifyError(error, '加载状态分布失败')
  }
}

/**
 * 渲染状态分布图表
 */
const renderStatusChart = (data) => {
  if (!statusChart) {
    statusChart = echarts.init(statusChartRef.value)
  }

  const option = getDoughnutChartOption(data, {
    radius: ['40%', '70%'],
    center: ['50%', '50%']
  })

  statusChart.setOption(option)
}

/**
 * 加载区域分布
 */
const loadRegionDistribution = async () => {
  const requestId = loadRegionDistributionRequest.begin()
  try {
    const params = {
      businessType: regionBusinessType.value || undefined,
      limit: 10
    }
    const res = await statisticsApi.announcement.getRegionDistribution(params)
    if (!loadRegionDistributionRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderRegionChart(res.data)
    }
  } catch (error) {
    if (!loadRegionDistributionRequest.isCurrent(requestId)) return

    notifyError(error, '加载区域分布失败')
  }
}

/**
 * 渲染区域分布图表
 */
const renderRegionChart = (data) => {
  if (!regionChart) {
    regionChart = echarts.init(regionChartRef.value)
  }

  const chartData = data.map(item => ({
    name: item.region,
    value: item.count
  }))

  const option = getHorizontalBarChartOption(chartData, {
    color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
      { offset: 0, color: '#83bff6' },
      { offset: 0.5, color: '#188df0' },
      { offset: 1, color: '#188df0' }
    ]),
    showLabel: true
  })

  regionChart.setOption(option)
}

/**
 * 加载热门公告
 */
const loadHotAnnouncements = async () => {
  const requestId = loadHotAnnouncementsRequest.begin()
  loading.value = true
  try {
    const params = {
      businessType: hotBusinessType.value || undefined,
      limit: 10
    }
    const res = await statisticsApi.announcement.getHotAnnouncements(params)
    if (!loadHotAnnouncementsRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      hotAnnouncements.value = res.data
    }
  } catch (error) {
    if (!loadHotAnnouncementsRequest.isCurrent(requestId)) return

    notifyError(error, '加载热门公告失败')
  } finally {
    if (loadHotAnnouncementsRequest.isCurrent(requestId)) {
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
  if (statusChart) statusChart.resize()
  if (regionChart) regionChart.resize()
}

onMounted(() => {
  loadOverview()
  loadTrendData()
  loadTypeDistribution()
  loadStatusDistribution()
  loadRegionDistribution()
  loadHotAnnouncements()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (trendChart) trendChart.dispose()
  if (typeChart) typeChart.dispose()
  if (statusChart) statusChart.dispose()
  if (regionChart) regionChart.dispose()
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
<template>
  <div class="page-container">
    <!-- 概览卡片 -->
    <el-row :gutter="20" class="overview-cards">
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #409eff;">
              <el-icon><View /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.totalVisits || 0 }}</div>
              <div class="stat-label">总访问量</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #67c23a;">
              <el-icon><User /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.uniqueVisitors || 0 }}</div>
              <div class="stat-label">独立访客</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #e6a23c;">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.todayVisits || 0 }}</div>
              <div class="stat-label">今日访问</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-card">
            <div class="stat-icon" style="background: #f56c6c;">
              <el-icon><Document /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overview.totalPages || 0 }}</div>
              <div class="stat-label">访问页面数</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 访问趋势图表 -->
    <el-card class="chart-card">
      <template #header>
        <div class="card-header">
          <span>访问趋势</span>
          <el-radio-group v-model="trendPeriod" size="small" @change="loadTrendData">
            <el-radio-button label="7">最近7天</el-radio-button>
            <el-radio-button label="30">最近30天</el-radio-button>
            <el-radio-button label="90">最近90天</el-radio-button>
          </el-radio-group>
        </div>
      </template>
      <div ref="trendChartRef" style="height: 400px;"></div>
    </el-card>

    <!-- 热门页面和访问来源 -->
    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="12">
        <el-card>
          <template #header>
            <span>热门页面 TOP 10</span>
          </template>
          <div ref="hotPagesChartRef" style="height: 400px;"></div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card>
          <template #header>
            <span>访问来源 TOP 10</span>
          </template>
          <div ref="refererChartRef" style="height: 400px;"></div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 访问记录详情 -->
    <el-card style="margin-top: 20px;">
      <template #header>
        <div class="card-header">
          <span>访问记录详情</span>
          <div>
            <el-button type="success" size="small" icon="Refresh" @click="loadTableData" :loading="tableLoading">
              刷新
            </el-button>
            <el-button type="primary" size="small" icon="Download" @click="exportData" :loading="exportLoading">
              导出数据
            </el-button>
          </div>
        </div>
      </template>

      <!-- 搜索区域 -->
      <el-form :model="searchForm" inline class="search-form">
        <el-form-item label="页面URL">
          <el-input
            v-model="searchForm.pageUrl"
            placeholder="请输入页面URL"
            clearable
            style="width: 200px;"
          />
        </el-form-item>
        <el-form-item label="页面标题">
          <el-input
            v-model="searchForm.pageTitle"
            placeholder="请输入页面标题"
            clearable
            style="width: 200px;"
          />
        </el-form-item>
        <el-form-item label="访问日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="YYYY-MM-DD"
            style="width: 240px;"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">搜索</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="tableData" v-loading="tableLoading" border stripe>
        <el-table-column type="index" label="序号" width="60" align="center" />
        <el-table-column prop="visitDate" label="访问日期" width="120" align="center" />
        <el-table-column prop="pageUrl" label="页面URL" min-width="200" show-overflow-tooltip />
        <el-table-column prop="pageTitle" label="页面标题" min-width="150" show-overflow-tooltip />
        <el-table-column prop="visitorIp" label="访问者IP" width="140" align="center" />
        <el-table-column prop="referer" label="来源页面" min-width="150" show-overflow-tooltip />
        <el-table-column prop="visitCount" label="访问次数" width="100" align="center" sortable />
        <el-table-column prop="createdAt" label="记录时间" width="160" align="center" />
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="loadTableData"
        @current-change="loadTableData"
        style="margin-top: 20px; justify-content: flex-end;"
      />
    </el-card>
  </div>
</template>

<script setup>
import { useLatestRequest } from '@/composables/useLatestRequest'
const loadOverviewRequest = useLatestRequest()
const loadTrendDataRequest = useLatestRequest()
const loadHotPagesRequest = useLatestRequest()
const loadRefererStatisticsRequest = useLatestRequest()
const loadTableDataRequest = useLatestRequest()
const loadRealtimeDataRequest = useLatestRequest()

import { notifyError } from '@/utils/errors'
import { localDate } from '@/utils/form'
import { ref, reactive, onMounted, onUnmounted, onBeforeUnmount } from 'vue'
import { ElMessage } from 'element-plus'
import { View, User, Calendar, Document } from '@element-plus/icons-vue'
import { statisticsApi } from '@/api'
import * as echarts from '@/utils/echarts'
import { getHorizontalBarChartOption } from '@/utils/chartOptions'

// 概览数据
const overview = reactive({
  totalVisits: 0,
  uniqueVisitors: 0,
  todayVisits: 0,
  totalPages: 0
})

// 图表实例
const trendChartRef = ref(null)
const hotPagesChartRef = ref(null)
const refererChartRef = ref(null)
let trendChart = null
let hotPagesChart = null
let refererChart = null
const trendPeriod = ref('7')

// 实时数据定时器
let realtimeTimer = null

// 搜索表单
const dateRange = ref([])
const searchForm = reactive({
  pageUrl: '',
  pageTitle: '',
  startDate: '',
  endDate: ''
})

const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const tableData = ref([])
const tableLoading = ref(false)
const exportLoading = ref(false)

/**
 * 加载概览数据
 */
const loadOverview = async () => {
  const requestId = loadOverviewRequest.begin()
  try {
    const res = await statisticsApi.visit.getOverview()
    if (!loadOverviewRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      Object.assign(overview, res.data)
    }
  } catch (error) {
    if (!loadOverviewRequest.isCurrent(requestId)) return
    notifyError(error, '加载概览数据失败:')
  }
}

/**
 * 加载趋势数据
 */
const loadTrendData = async () => {
  const requestId = loadTrendDataRequest.begin()
  try {
    const days = parseInt(trendPeriod.value)
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - days)

    const params = {
      startDate: localDate(startDate),
      endDate: localDate(endDate),
      groupBy: 'day'
    }

    const res = await statisticsApi.visit.getTrend(params)
    if (!loadTrendDataRequest.isCurrent(requestId)) return

    if (res.success && res.data) {

      renderTrendChart(res.data)
    } else {
      console.warn('访问趋势数据为空或请求失败')
      // 渲染空图表
      renderTrendChart([])
    }
  } catch (error) {
    if (!loadTrendDataRequest.isCurrent(requestId)) return
    notifyError(error, '加载趋势数据失败:')
    // 渲染空图表
    renderTrendChart([])
  }
}

/**
 * 渲染趋势图表
 */
const renderTrendChart = (data) => {
  if (!trendChart) {
    trendChart = echarts.init(trendChartRef.value)
  }

  // 处理空数据情况
  if (!data || data.length === 0) {
    console.warn('访问趋势数据为空，显示空图表')
    data = []
  }

  // 后端返回的是数组格式：[{date, visitCount, uniqueVisitors}]
  const dates = data.map(item => item.date || '')
  const visits = data.map(item => item.visitCount || 0)
  const uniqueVisitors = data.map(item => item.uniqueVisitors || 0)

  const option = {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      }
    },
    legend: {
      data: ['访问量', '独立访客']
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
        name: '访问量',
        type: 'line',
        data: visits,
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
        name: '独立访客',
        type: 'line',
        data: uniqueVisitors,
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
 * 加载热门页面
 */
const loadHotPages = async () => {
  const requestId = loadHotPagesRequest.begin()
  try {
    const res = await statisticsApi.visit.getHotPages({ limit: 10, days: 30 })
    if (!loadHotPagesRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderHotPagesChart(res.data)
    }
  } catch (error) {
    if (!loadHotPagesRequest.isCurrent(requestId)) return
    notifyError(error, '加载热门页面失败:')
  }
}

/**
 * 渲染热门页面图表
 */
const renderHotPagesChart = (data) => {
  if (!hotPagesChart) {
    hotPagesChart = echarts.init(hotPagesChartRef.value)
  }

  // 反转数组，让数值大的显示在上面
  const chartData = data.map(item => ({
    name: item.pageTitle || item.pageUrl || '未知页面',
    value: item.visitCount || item.totalViews || 0
  })).reverse()

  const option = getHorizontalBarChartOption(chartData, {
    color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
      { offset: 0, color: '#667eea' },
      { offset: 1, color: '#764ba2' }
    ]),
    showLabel: true
  })

  hotPagesChart.setOption(option)
}

/**
 * 加载访问来源统计
 */
const loadRefererStatistics = async () => {
  const requestId = loadRefererStatisticsRequest.begin()
  try {
    const res = await statisticsApi.visit.getSources({ limit: 10 })
    if (!loadRefererStatisticsRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderRefererChart(res.data)
    }
  } catch (error) {
    if (!loadRefererStatisticsRequest.isCurrent(requestId)) return
    notifyError(error, '加载访问来源失败:')
  }
}

/**
 * 渲染访问来源图表
 */
const renderRefererChart = (data) => {
  if (!refererChart) {
    refererChart = echarts.init(refererChartRef.value)
  }

  // 反转数组，让数值大的显示在上面
  const chartData = data.map(item => ({
    name: item.referer || item.source || '直接访问',
    value: item.count || item.visitCount || 0
  })).reverse()

  const option = getHorizontalBarChartOption(chartData, {
    color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
      { offset: 0, color: '#f093fb' },
      { offset: 1, color: '#f5576c' }
    ]),
    showLabel: true
  })

  refererChart.setOption(option)
}

/**
 * 加载访问记录
 */
const loadTableData = async () => {
  const requestId = loadTableDataRequest.begin()
  tableLoading.value = true
  try {
    if (dateRange.value && dateRange.value.length === 2) {
      searchForm.startDate = dateRange.value[0]
      searchForm.endDate = dateRange.value[1]
    } else {
      searchForm.startDate = ''
      searchForm.endDate = ''
    }

    const params = {
      pageUrl: searchForm.pageUrl || undefined,
      pageTitle: searchForm.pageTitle || undefined,
      startDate: searchForm.startDate || undefined,
      endDate: searchForm.endDate || undefined,
      page: pagination.pageIndex,
      pageSize: pagination.pageSize
    }

    const res = await statisticsApi.visit.getList(params)
    if (!loadTableDataRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      tableData.value = res.data.items || []
      pagination.total = res.data.totalCount || 0
    }
  } catch (error) {
    if (!loadTableDataRequest.isCurrent(requestId)) return

    notifyError(error, '加载访问记录失败')
  } finally {
    if (loadTableDataRequest.isCurrent(requestId)) {
      tableLoading.value = false
    }
  }
}

/**
 * 搜索
 */
const handleSearch = () => {
  pagination.pageIndex = 1
  loadTableData()
}

/**
 * 重置
 */
const handleReset = () => {
  searchForm.pageUrl = ''
  searchForm.pageTitle = ''
  dateRange.value = []
  handleSearch()
}

/**
 * 导出数据
 */
const exportData = async () => {
  exportLoading.value = true
  try {
    if (dateRange.value && dateRange.value.length === 2) {
      searchForm.startDate = dateRange.value[0]
      searchForm.endDate = dateRange.value[1]
    }

    const params = {
      pageUrl: searchForm.pageUrl || undefined,
      pageTitle: searchForm.pageTitle || undefined,
      startDate: searchForm.startDate || undefined,
      endDate: searchForm.endDate || undefined
    }

    const res = await statisticsApi.visit.export(params)

    // 创建下载链接
    const blob = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `访问统计_${new Date().getTime()}.xlsx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('导出成功')
  } catch (error) {

    notifyError(error, '导出失败')
  } finally {
    exportLoading.value = false
  }
}

/**
 * 加载实时数据
 */
const loadRealtimeData = async () => {
  const requestId = loadRealtimeDataRequest.begin()
  try {
    const res = await statisticsApi.system.getRealtime()
    if (!loadRealtimeDataRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      overview.todayVisits = res.data.todayVisits || overview.todayVisits
    }
  } catch (error) {
    if (!loadRealtimeDataRequest.isCurrent(requestId)) return
    notifyError(error, '加载实时数据失败:')
  }
}

/**
 * 启动实时数据更新
 */
const startRealtimeUpdate = () => {
  realtimeTimer = setInterval(() => {
    loadRealtimeData()
  }, 30000) // 每30秒更新一次
}

/**
 * 停止实时数据更新
 */
const stopRealtimeUpdate = () => {
  if (realtimeTimer) {
    clearInterval(realtimeTimer)
    realtimeTimer = null
  }
}

/**
 * 窗口大小改变时重绘图表
 */
const handleResize = () => {
  if (trendChart) trendChart.resize()
  if (hotPagesChart) hotPagesChart.resize()
  if (refererChart) refererChart.resize()
}

onMounted(() => {
  loadOverview()
  loadTrendData()
  loadHotPages()
  loadRefererStatistics()
  loadTableData()
  startRealtimeUpdate()
  window.addEventListener('resize', handleResize)
})

onBeforeUnmount(() => {
  stopRealtimeUpdate()
})

onUnmounted(() => {
  if (trendChart) trendChart.dispose()
  if (hotPagesChart) hotPagesChart.dispose()
  if (refererChart) refererChart.dispose()
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

.search-form {
  margin-bottom: 16px;
}

.search-form :deep(.el-form-item) {
  margin-bottom: 10px;
}
</style>
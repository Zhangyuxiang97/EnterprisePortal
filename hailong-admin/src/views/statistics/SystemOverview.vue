<template>
  <div class="page-container">
    <!-- 系统概览卡片 -->
    <el-row :gutter="20" class="overview-cards">
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card-wrapper">
          <div class="stat-card">
            <div class="stat-icon" style="background: #edf3f8;
  color: #245b85;">
              <el-icon><Document /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ systemData.totalAnnouncements }}</div>
              <div class="stat-label">公告总数</div>
              <div class="stat-trend" :class="systemData.announcementTrend > 0 ? 'up' : 'down'">
                <el-icon><CaretTop v-if="systemData.announcementTrend > 0" /><CaretBottom v-else /></el-icon>
                {{ Math.abs(systemData.announcementTrend) }}%
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card-wrapper">
          <div class="stat-card">
            <div class="stat-icon" style="background: #edf3f8;
  color: #245b85;">
              <el-icon><DocumentCopy /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ systemData.totalInfoPublications }}</div>
              <div class="stat-label">信息发布总数</div>
              <div class="stat-trend" :class="systemData.infoPubTrend > 0 ? 'up' : 'down'">
                <el-icon><CaretTop v-if="systemData.infoPubTrend > 0" /><CaretBottom v-else /></el-icon>
                {{ Math.abs(systemData.infoPubTrend) }}%
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card-wrapper">
          <div class="stat-card">
            <div class="stat-icon" style="background: #edf3f8;
  color: #245b85;">
              <el-icon><View /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ systemData.totalVisits }}</div>
              <div class="stat-label">总访问量</div>
              <div class="stat-trend" :class="systemData.visitTrend > 0 ? 'up' : 'down'">
                <el-icon><CaretTop v-if="systemData.visitTrend > 0" /><CaretBottom v-else /></el-icon>
                {{ Math.abs(systemData.visitTrend) }}%
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card-wrapper">
          <div class="stat-card">
            <div class="stat-icon" style="background: #edf3f8;
  color: #245b85;">
              <el-icon><User /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ systemData.activeUsers }}</div>
              <div class="stat-label">活跃用户</div>
              <div class="stat-trend" :class="systemData.userTrend > 0 ? 'up' : 'down'">
                <el-icon><CaretTop v-if="systemData.userTrend > 0" /><CaretBottom v-else /></el-icon>
                {{ Math.abs(systemData.userTrend) }}%
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 实时数据 -->
    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>综合数据趋势（最近30天）</span>
              <el-tag type="success" effect="plain">
                <el-icon><Clock /></el-icon>
                实时更新
              </el-tag>
            </div>
          </template>
          <div ref="trendChartRef" style="height: 400px;"></div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card style="height: 100%;">
          <template #header>
            <span>今日实时数据</span>
          </template>
          <div class="realtime-stats">
            <div class="realtime-item">
              <div class="realtime-label">
                <el-icon><View /></el-icon>
                今日访问
              </div>
              <div class="realtime-value">{{ realtimeData.todayVisits }}</div>
            </div>
            <el-divider />
            <div class="realtime-item">
              <div class="realtime-label">
                <el-icon><Document /></el-icon>
                今日发布公告
              </div>
              <div class="realtime-value">{{ realtimeData.todayAnnouncements }}</div>
            </div>
            <el-divider />
            <div class="realtime-item">
              <div class="realtime-label">
                <el-icon><DocumentCopy /></el-icon>
                今日发布信息
              </div>
              <div class="realtime-value">{{ realtimeData.todayInfoPublications }}</div>
            </div>
            <el-divider />
            <div class="realtime-item">
              <div class="realtime-label">
                <el-icon><User /></el-icon>
                在线用户
              </div>
              <div class="realtime-value">{{ realtimeData.onlineUsers }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 数据分布 -->
    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>公告类型分布</span>
          </template>
          <div ref="announcementTypeChartRef" style="height: 300px;"></div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>信息类型分布</span>
          </template>
          <div ref="infoTypeChartRef" style="height: 300px;"></div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>系统健康度</span>
          </template>
          <div ref="healthChartRef" style="height: 300px;"></div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 快速访问 -->
    <el-card style="margin-top: 20px;">
      <template #header>
        <span>快速访问</span>
      </template>
      <el-row :gutter="20">
        <el-col :span="6">
          <div class="quick-link" @click="navigateTo('/statistics/visit')">
            <el-icon class="quick-icon" style="color: #409eff;"><View /></el-icon>
            <div class="quick-text">
              <div class="quick-title">访问统计</div>
              <div class="quick-desc">查看详细访问数据</div>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="quick-link" @click="navigateTo('/statistics/announcement')">
            <el-icon class="quick-icon" style="color: #67c23a;"><Document /></el-icon>
            <div class="quick-text">
              <div class="quick-title">公告统计</div>
              <div class="quick-desc">查看公告发布统计</div>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="quick-link" @click="navigateTo('/statistics/info-publication')">
            <el-icon class="quick-icon" style="color: #e6a23c;"><DocumentCopy /></el-icon>
            <div class="quick-text">
              <div class="quick-title">信息发布统计</div>
              <div class="quick-desc">查看信息发布数据</div>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="quick-link" @click="exportAllData">
            <el-icon class="quick-icon" style="color: #f56c6c;"><Download /></el-icon>
            <div class="quick-text">
              <div class="quick-title">导出报表</div>
              <div class="quick-desc">导出综合统计报表</div>
            </div>
          </div>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup>
import { useLatestRequest } from '@/composables/useLatestRequest'
const loadSystemOverviewRequest = useLatestRequest()
const loadRealtimeDataRequest = useLatestRequest()
const loadTrendDataRequest = useLatestRequest()
const loadAnnouncementTypeDistributionRequest = useLatestRequest()
const loadInfoTypeDistributionRequest = useLatestRequest()

import { notifyError } from '@/utils/errors'
import { localDate } from '@/utils/form'
import { ref, reactive, onMounted, onUnmounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
  Document,
  DocumentCopy,
  View,
  User,
  Clock,
  CaretTop,
  CaretBottom,
  Download
} from '@element-plus/icons-vue'
import { statisticsApi } from '@/api'
import * as echarts from '@/utils/echarts'
import { getLineChartOption, getDoughnutChartOption, getGaugeChartOption } from '@/utils/chartOptions'

const router = useRouter()

// 系统数据
const systemData = reactive({
  totalAnnouncements: 0,
  announcementTrend: 0,
  totalInfoPublications: 0,
  infoPubTrend: 0,
  totalVisits: 0,
  visitTrend: 0,
  activeUsers: 0,
  userTrend: 0
})

// 实时数据
const realtimeData = reactive({
  todayVisits: 0,
  todayAnnouncements: 0,
  todayInfoPublications: 0,
  onlineUsers: 0
})

// 图表实例
let trendChart = null
let announcementTypeChart = null
let infoTypeChart = null
let healthChart = null

const trendChartRef = ref(null)
const announcementTypeChartRef = ref(null)
const infoTypeChartRef = ref(null)
const healthChartRef = ref(null)

// 实时更新定时器
let realtimeTimer = null

/**
 * 加载系统概览数据
 */
const loadSystemOverview = async () => {
  const requestId = loadSystemOverviewRequest.begin()
  try {
    const res = await statisticsApi.system.getOverview()
    if (!loadSystemOverviewRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      Object.assign(systemData, res.data)
    }
  } catch (error) {
    if (!loadSystemOverviewRequest.isCurrent(requestId)) return
    notifyError(error, '加载系统概览失败:')
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
      Object.assign(realtimeData, res.data)
    }
  } catch (error) {
    if (!loadRealtimeDataRequest.isCurrent(requestId)) return
    notifyError(error, '加载实时数据失败:')
  }
}

/**
 * 加载综合趋势数据
 */
const loadTrendData = async () => {
  const requestId = loadTrendDataRequest.begin()
  try {
    const [visitRes, announcementRes, infoRes] = await Promise.all([
      statisticsApi.visit.getTrend({ days: 30 }),
      statisticsApi.announcement.getTrend({ days: 30 }),
      statisticsApi.infoPublication.getTrend({ days: 30 })
    ])
    if (!loadTrendDataRequest.isCurrent(requestId)) return

    if (visitRes.success && announcementRes.success && infoRes.success) {
      renderTrendChart({
        dates: visitRes.data.dates,
        series: [
          { name: '访问量', data: visitRes.data.visits || [] },
          { name: '公告发布', data: announcementRes.data.govProcurement?.map((v, i) =>
            v + (announcementRes.data.construction?.[i] || 0)) || [] },
          { name: '信息发布', data: infoRes.data.companyNews?.map((v, i) =>
            v + (infoRes.data.policyRegulation?.[i] || 0) +
            (infoRes.data.policyInfo?.[i] || 0) +
            (infoRes.data.notice?.[i] || 0)) || [] }
        ]
      })
    }
  } catch (error) {
    if (!loadTrendDataRequest.isCurrent(requestId)) return
    notifyError(error, '加载趋势数据失败:')
  }
}

/**
 * 渲染综合趋势图表
 */
const renderTrendChart = (data) => {
  if (!trendChart) {
    trendChart = echarts.init(trendChartRef.value)
  }

  const option = getLineChartOption(data, {
    smooth: true,
    showArea: true,
    colors: ['#4facfe', '#667eea', '#f093fb']
  })

  trendChart.setOption(option)
}

/**
 * 加载公告类型分布
 */
const loadAnnouncementTypeDistribution = async () => {
  const requestId = loadAnnouncementTypeDistributionRequest.begin()
  try {
    const res = await statisticsApi.announcement.getTypeDistribution()
    if (!loadAnnouncementTypeDistributionRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderAnnouncementTypeChart(res.data)
    }
  } catch (error) {
    if (!loadAnnouncementTypeDistributionRequest.isCurrent(requestId)) return
    notifyError(error, '加载公告类型分布失败:')
  }
}

/**
 * 渲染公告类型图表
 */
const renderAnnouncementTypeChart = (data) => {
  if (!announcementTypeChart) {
    announcementTypeChart = echarts.init(announcementTypeChartRef.value)
  }

  const option = getDoughnutChartOption(data, {
    radius: ['40%', '70%']
  })

  announcementTypeChart.setOption(option)
}

/**
 * 加载信息类型分布
 */
const loadInfoTypeDistribution = async () => {
  const requestId = loadInfoTypeDistributionRequest.begin()
  try {
    const res = await statisticsApi.infoPublication.getTypeDistribution()
    if (!loadInfoTypeDistributionRequest.isCurrent(requestId)) return
    if (res.success && res.data) {
      renderInfoTypeChart(res.data)
    }
  } catch (error) {
    if (!loadInfoTypeDistributionRequest.isCurrent(requestId)) return
    notifyError(error, '加载信息类型分布失败:')
  }
}

/**
 * 渲染信息类型图表
 */
const renderInfoTypeChart = (data) => {
  if (!infoTypeChart) {
    infoTypeChart = echarts.init(infoTypeChartRef.value)
  }

  const option = getDoughnutChartOption(data, {
    radius: ['40%', '70%']
  })

  infoTypeChart.setOption(option)
}

/**
 * 渲染系统健康度图表
 */
const renderHealthChart = () => {
  if (!healthChart) {
    healthChart = echarts.init(healthChartRef.value)
  }

  // 计算系统健康度（示例：基于各项指标）
  const healthScore = Math.min(100, Math.round(
    (systemData.totalAnnouncements > 0 ? 25 : 0) +
    (systemData.totalInfoPublications > 0 ? 25 : 0) +
    (systemData.totalVisits > 100 ? 25 : systemData.totalVisits / 4) +
    (systemData.activeUsers > 0 ? 25 : 0)
  ))

  const option = getGaugeChartOption(healthScore, {
    min: 0,
    max: 100,
    unit: '%'
  })

  healthChart.setOption(option)
}

/**
 * 导航到指定页面
 */
const navigateTo = (path) => {
  router.push(path)
}

/**
 * 导出所有数据
 */
const exportAllData = () => {
  ElMessage.info('综合报表导出功能开发中...')
}

/**
 * 启动实时更新
 */
const startRealtimeUpdate = () => {
  realtimeTimer = setInterval(() => {
    loadRealtimeData()
  }, 30000) // 每30秒更新一次
}

/**
 * 停止实时更新
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
  if (announcementTypeChart) announcementTypeChart.resize()
  if (infoTypeChart) infoTypeChart.resize()
  if (healthChart) healthChart.resize()
}

let healthRenderTimer
onMounted(() => {
  loadSystemOverview()
  loadRealtimeData()
  loadTrendData()
  loadAnnouncementTypeDistribution()
  loadInfoTypeDistribution()

  // 延迟渲染健康度图表，确保数据已加载
  healthRenderTimer = setTimeout(() => {
    renderHealthChart()
  }, 500)

  startRealtimeUpdate()
  window.addEventListener('resize', handleResize)
})

onBeforeUnmount(() => {
  clearTimeout(healthRenderTimer)
  stopRealtimeUpdate()
})

onUnmounted(() => {
  if (trendChart) trendChart.dispose()
  if (announcementTypeChart) announcementTypeChart.dispose()
  if (infoTypeChart) infoTypeChart.dispose()
  if (healthChart) healthChart.dispose()
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

.stat-card-wrapper {
  transition: transform 0.3s ease;
}

.stat-card-wrapper:hover {
  transform: translateY(-5px);
}

.stat-card {
  display: flex;
  align-items: center;
}

.stat-icon {
  width: 70px;
  height: 70px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 32px;
  color: white;
  margin-right: 20px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 28px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  color: #909399;
  margin-bottom: 5px;
}

.stat-trend {
  font-size: 12px;
  display: flex;
  align-items: center;
  gap: 4px;
}

.stat-trend.up {
  color: #67c23a;
}

.stat-trend.down {
  color: #f56c6c;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.realtime-stats {
  padding: 10px 0;
}

.realtime-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 0;
}

.realtime-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #606266;
}

.realtime-value {
  font-size: 24px;
  font-weight: bold;
  color: #409eff;
}

.quick-link {
  display: flex;
  align-items: center;
  padding: 20px;
  border: 1px solid #ebeef5;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.quick-link:hover {
  border-color: #409eff;
  background: #f0f9ff;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(64, 158, 255, 0.15);
}

.quick-icon {
  font-size: 36px;
  margin-right: 15px;
}

.quick-text {
  flex: 1;
}

.quick-title {
  font-size: 16px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 5px;
}

.quick-desc {
  font-size: 12px;
  color: #909399;
}
</style>

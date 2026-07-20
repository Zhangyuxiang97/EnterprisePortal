import request from './request'

/**
 * 统计分析 API
 * 包含：访问统计、公告统计、信息发布统计等
 */

// ==================== 访问统计 ====================
/**
 * 获取访问统计列表
 * @param {Object} params - 查询参数
 * @param {number} params.page - 页码
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 * @param {string} params.pagePath - 页面路径
 */
export const getVisitStatisticsList = (params) => {
  return request({
    url: '/statistics/visits',
    method: 'get',
    params
  })
}

/**
 * 获取访问统计概览
 * @param {Object} params - 查询参数
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 */
export const getVisitStatisticsOverview = (params) => {
  return request({
    url: '/statistics/visits/overview',
    method: 'get',
    params
  })
}

/**
 * 获取访问趋势数据
 * @param {Object} params - 查询参数
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 * @param {string} params.groupBy - 分组方式：day/week/month
 */
export const getVisitTrend = (params) => {
  return request({
    url: '/statistics/visits/trend',
    method: 'get',
    params
  })
}

/**
 * 获取热门页面统计
 * @param {Object} params - 查询参数
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 * @param {number} params.limit - 返回数量
 */
export const getPopularPages = (params) => {
  return request({
    url: '/statistics/visits/popular-pages',
    method: 'get',
    params
  })
}

/**
 * 记录访问统计
 * @param {Object} data - 访问数据
 * @param {string} data.pageUrl - 页面URL
 * @param {string} data.pageTitle - 页面标题
 * @param {string} data.referer - 来源页面
 */
export const recordVisit = (data) => {
  return request({
    url: '/statistics/visits/record',
    method: 'post',
    data
  })
}

/**
 * 导出访问统计数据
 * @param {Object} params - 查询参数
 */
export const exportVisitStatistics = (params) => {
  return request({
    url: '/statistics/visits/export',
    method: 'get',
    params,
    responseType: 'blob'
  })
}

/**
 * 获取访问来源统计
 * @param {Object} params - 查询参数
 */
export const getVisitSourceStatistics = (params) => {
  return request({
    url: '/statistics/visits/sources',
    method: 'get',
    params
  })
}

/**
 * 获取设备类型统计
 * @param {Object} params - 查询参数
 */
export const getDeviceStatistics = (params) => {
  return request({
    url: '/statistics/visits/devices',
    method: 'get',
    params
  })
}

/**
 * 获取浏览器统计
 * @param {Object} params - 查询参数
 */
export const getBrowserStatistics = (params) => {
  return request({
    url: '/statistics/visits/browsers',
    method: 'get',
    params
  })
}

// ==================== 公告统计 ====================
/**
 * 获取公告统计概览
 * @param {Object} params - 查询参数
 */
export const getAnnouncementStatisticsOverview = (params) => {
  return request({
    url: '/statistics/announcements/overview',
    method: 'get',
    params
  })
}

/**
 * 获取公告发布趋势
 * @param {Object} params - 查询参数
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 * @param {string} params.businessType - 业务类型
 * @param {string} params.groupBy - 分组方式：day/week/month
 */
export const getAnnouncementPublishTrend = (params) => {
  return request({
    url: '/statistics/announcements/publish-trend',
    method: 'get',
    params
  })
}

/**
 * 获取公告类型分布
 * @param {Object} params - 查询参数
 */
export const getAnnouncementTypeDistribution = (params) => {
  return request({
    url: '/statistics/announcements/type-distribution',
    method: 'get',
    params
  })
}

/**
 * 获取公告区域分布
 * @param {Object} params - 查询参数
 * @param {string} params.businessType - 业务类型
 * @param {number} params.limit - 返回数量
 */
export const getAnnouncementRegionDistribution = (params) => {
  return request({
    url: '/statistics/announcements/region-distribution',
    method: 'get',
    params
  })
}

/**
 * 获取热门公告
 * @param {Object} params - 查询参数
 * @param {string} params.businessType - 业务类型
 * @param {number} params.limit - 返回数量
 */
export const getPopularAnnouncements = (params) => {
  return request({
    url: '/statistics/announcements/popular',
    method: 'get',
    params
  })
}

/**
 * 导出公告统计数据
 * @param {Object} params - 查询参数
 */
export const exportAnnouncementStatistics = (params) => {
  return request({
    url: '/statistics/announcements/export',
    method: 'get',
    params,
    responseType: 'blob'
  })
}

/**
 * 获取公告状态分布
 * @param {Object} params - 查询参数
 */
export const getAnnouncementStatusDistribution = (params) => {
  return request({
    url: '/statistics/announcements/status-distribution',
    method: 'get',
    params
  })
}

/**
 * 获取公告时间段分析
 * @param {Object} params - 查询参数
 */
export const getAnnouncementTimeAnalysis = (params) => {
  return request({
    url: '/statistics/announcements/time-analysis',
    method: 'get',
    params
  })
}

// ==================== 信息发布统计 ====================
/**
 * 获取信息发布统计概览
 */
export const getInfoPublicationStatisticsOverview = (params) => {
  return request({
    url: '/statistics/info-publications/overview',
    method: 'get',
    params
  })
}

/**
 * 获取信息发布趋势
 */
export const getInfoPublicationPublishTrend = (params) => {
  return request({
    url: '/statistics/info-publications/publish-trend',
    method: 'get',
    params
  })
}

/**
 * 获取信息类型分布
 */
export const getInfoPublicationTypeDistribution = (params) => {
  return request({
    url: '/statistics/info-publications/type-distribution',
    method: 'get',
    params
  })
}

/**
 * 获取热门信息
 * @param {Object} params - 查询参数
 */
export const getPopularInfoPublications = (params) => {
  return request({
    url: '/statistics/info-publications/popular',
    method: 'get',
    params
  })
}

/**
 * 导出信息发布统计数据
 * @param {Object} params - 查询参数
 */
export const exportInfoPublicationStatistics = (params) => {
  return request({
    url: '/statistics/info-publications/export',
    method: 'get',
    params,
    responseType: 'blob'
  })
}

/**
 * 获取发布单位统计
 * @param {Object} params - 查询参数
 */
export const getAuthorPublishStatistics = (params) => {
  return request({
    url: '/statistics/info-publications/author-statistics',
    method: 'get',
    params
  })
}

// ==================== 综合统计 ====================
/**
 * 获取系统概览统计
 */
export const getSystemOverview = () => {
  return request({
    url: '/statistics/system/overview',
    method: 'get'
  })
}

/**
 * 获取实时统计数据
 */
export const getRealtimeStatistics = () => {
  return request({
    url: '/statistics/realtime',
    method: 'get'
  })
}

// ==================== 首页统计 ====================
/**
 * 获取首页统计数据
 */
export const getHomeStatistics = () => {
  return request({
    url: '/statistics/home',
    method: 'get'
  })
}

// 统一导出对象，按模块组织
export default {
  visit: {
    getList: getVisitStatisticsList,
    getOverview: getVisitStatisticsOverview,
    getTrend: getVisitTrend,
    getHotPages: getPopularPages,
    record: recordVisit,
    export: exportVisitStatistics,
    getSources: getVisitSourceStatistics,
    getDevices: getDeviceStatistics,
    getBrowsers: getBrowserStatistics
  },
  announcement: {
    getOverview: getAnnouncementStatisticsOverview,
    getTrend: getAnnouncementPublishTrend,
    getTypeDistribution: getAnnouncementTypeDistribution,
    getRegionDistribution: getAnnouncementRegionDistribution,
    getHotAnnouncements: getPopularAnnouncements,
    export: exportAnnouncementStatistics,
    getStatusDistribution: getAnnouncementStatusDistribution,
    getTimeAnalysis: getAnnouncementTimeAnalysis
  },
  infoPublication: {
    getOverview: getInfoPublicationStatisticsOverview,
    getTrend: getInfoPublicationPublishTrend,
    getTypeDistribution: getInfoPublicationTypeDistribution,
    getHotInfo: getPopularInfoPublications,
    export: exportInfoPublicationStatistics,
    getAuthorStatistics: getAuthorPublishStatistics
  },
  system: {
    getOverview: getSystemOverview,
    getRealtime: getRealtimeStatistics,
    getHomeStatistics
  }
}
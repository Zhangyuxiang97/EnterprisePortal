import request from './request'

/**
 * 获取首页统计概览
 * @returns {Promise}
 */
export function getStatisticsOverview() {
  return request({
    url: '/home/statistics/overview',
    method: 'get'
  })
}

/**
 * 获取最新公告列表
 * @returns {Promise}
 */
export function getRecentAnnouncements() {
  return request({
    url: '/home/recent-announcements',
    method: 'get'
  })
}
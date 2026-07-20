import request from './request'

/**
 * 公告管理 API
 * 对应数据库表：announcements
 * 统一管理政府采购和建设工程公告
 */

/**
 * 获取公告列表
 * @param {Object} params - 查询参数
 * @param {number} params.pageNumber - 页码
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.businessType - 业务类型：GOV_PROCUREMENT/CONSTRUCTION
 * @param {string} params.noticeType - 公告类型：bidding/correction/result
 * @param {string} params.procurementType - 采购类型：goods/service/project
 * @param {string} params.province - 省份
 * @param {string} params.city - 城市
 * @param {string} params.district - 区县
 * @param {string} params.keyword - 搜索关键词
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 */
export const getAnnouncementList = (params) => {
  return request({
    url: '/announcements',
    method: 'get',
    params
  })
}

/**
 * 获取公告详情
 * @param {number} id - 公告ID
 */
export const getAnnouncementDetail = (id) => {
  return request({
    url: `/announcements/${id}`,
    method: 'get'
  })
}

/**
 * 创建公告
 * @param {Object} data - 公告数据
 * @param {string} data.title - 公告标题
 * @param {string} data.businessType - 业务类型
 * @param {string} data.noticeType - 公告类型
 * @param {string} data.procurementType - 采购类型（政府采购必填）
 * @param {string} data.bidder - 招标人
 * @param {string} data.winner - 中标人
 * @param {number} data.budgetAmount - 预算金额
 * @param {string} data.deadline - 截止时间
 * @param {string} data.province - 省份
 * @param {string} data.city - 城市
 * @param {string} data.district - 区县
 * @param {string} data.projectRegion - 项目区域
 * @param {string} data.content - 公告内容
 * @param {string} data.publisher - 发布人
 * @param {string} data.publishTime - 发布时间
 * @param {Array<number>} data.attachmentIds - 附件ID列表
 * @param {number} data.isTop - 是否置顶
 * @param {number} data.status - 状态
 */
export const createAnnouncement = (data) => {
  return request({
    url: '/announcements',
    method: 'post',
    data
  })
}

/**
 * 更新公告
 * @param {number} id - 公告ID
 * @param {Object} data - 公告数据
 */
export const updateAnnouncement = (id, data) => {
  return request({
    url: `/announcements/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除公告
 * @param {number} id - 公告ID
 */
export const deleteAnnouncement = (id) => {
  return request({
    url: `/announcements/${id}`,
    method: 'delete'
  })
}

/**
 * 批量删除公告
 * @param {Array<number>} ids - 公告ID数组
 */
export const batchDeleteAnnouncements = (ids) => {
  return request({
    url: '/announcements/batch-delete',
    method: 'post',
    data: { ids }
  })
}

/**
 * 置顶/取消置顶公告
 * @param {number} id - 公告ID
 * @param {number} isTop - 是否置顶：0-否，1-是
 */
export const toggleAnnouncementTop = (id, isTop) => {
  return request({
    url: `/announcements/${id}/top`,
    method: 'put',
    data: { isTop }
  })
}

/**
 * 启用/禁用公告
 * @param {number} id - 公告ID
 * @param {number} status - 状态：0-禁用，1-启用
 */
export const toggleAnnouncementStatus = (id, status) => {
  return request({
    url: `/announcements/${id}/status`,
    method: 'put',
    data: { status }
  })
}

/**
 * 增加公告浏览次数
 * @param {number} id - 公告ID
 */
export const incrementAnnouncementView = (id) => {
  return request({
    url: `/announcements/${id}/view`,
    method: 'post'
  })
}

/**
 * 获取公告统计数据
 * @param {Object} params - 查询参数
 */
export const getAnnouncementStatistics = (params) => {
  return request({
    url: '/announcements/statistics',
    method: 'get',
    params
  })
}

export default {
  getAnnouncementList,
  getAnnouncementDetail,
  createAnnouncement,
  updateAnnouncement,
  deleteAnnouncement,
  batchDeleteAnnouncements,
  toggleAnnouncementTop,
  toggleAnnouncementStatus,
  incrementAnnouncementView,
  getAnnouncementStatistics
}
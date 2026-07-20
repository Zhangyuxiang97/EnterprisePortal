import request from './request'

/**
 * 信息发布 API
 * 对应数据库表：info_publications
 * 统一管理新闻中心、政策法规
 */

/**
 * 获取信息发布列表
 * @param {Object} params - 查询参数
 * @param {number} params.page - 页码
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.type - 信息类型：COMPANY_NEWS-新闻中心, POLICY_REGULATION-政策法规
 * @param {string} params.category - 二级分类
 * @param {string} params.keyword - 搜索关键词
 * @param {string} params.author - 作者
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 */
export const getInfoPublicationList = (params) => {
  return request({
    url: '/info-publications',
    method: 'get',
    params
  })
}

/**
 * 获取信息发布详情
 * @param {number} id - 信息ID
 */
export const getInfoPublicationDetail = (id) => {
  return request({
    url: `/info-publications/${id}`,
    method: 'get'
  })
}

/**
 * 创建信息发布
 * @param {Object} data - 信息数据
 * @param {string} data.type - 信息类型：COMPANY_NEWS-新闻中心, POLICY_REGULATION-政策法规
 * @param {string} data.category - 二级分类
 * @param {string} data.title - 标题
 * @param {string} data.summary - 摘要
 * @param {string} data.content - 内容
 * @param {number} data.coverImageId - 封面图片ID
 * @param {string} data.author - 作者
 * @param {string} data.publisher - 发布人
 * @param {string} data.publishTime - 发布时间
 * @param {Array<number>} data.attachmentIds - 附件ID列表
 * @param {number} data.isTop - 是否置顶
 * @param {number} data.status - 状态
 */
export const createInfoPublication = (data) => {
  return request({
    url: '/info-publications',
    method: 'post',
    data
  })
}

/**
 * 更新信息发布
 * @param {number} id - 信息ID
 * @param {Object} data - 信息数据
 */
export const updateInfoPublication = (id, data) => {
  return request({
    url: `/info-publications/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除信息发布
 * @param {number} id - 信息ID
 */
export const deleteInfoPublication = (id) => {
  return request({
    url: `/info-publications/${id}`,
    method: 'delete'
  })
}

/**
 * 批量删除信息发布
 * @param {Array<number>} ids - 信息ID数组
 */
export const batchDeleteInfoPublications = (ids) => {
  return request({
    url: '/info-publications/batch-delete',
    method: 'post',
    data: { ids }
  })
}

/**
 * 置顶/取消置顶信息
 * @param {number} id - 信息ID
 * @param {number} isTop - 是否置顶：0-否，1-是
 */
export const toggleInfoPublicationTop = (id, isTop) => {
  return request({
    url: `/info-publications/${id}/top`,
    method: 'put',
    data: { isTop }
  })
}

/**
 * 启用/禁用信息
 * @param {number} id - 信息ID
 * @param {number} status - 状态：0-禁用，1-启用
 */
export const toggleInfoPublicationStatus = (id, status) => {
  return request({
    url: `/info-publications/${id}/status`,
    method: 'put',
    data: { status }
  })
}

/**
 * 增加信息浏览次数
 * @param {number} id - 信息ID
 */
export const incrementInfoPublicationView = (id) => {
  return request({
    url: `/info-publications/${id}/view`,
    method: 'post'
  })
}

/**
 * 获取信息发布统计数据
 * @param {Object} params - 查询参数
 */
export const getInfoPublicationStatistics = (params) => {
  return request({
    url: '/info-publications/statistics',
    method: 'get',
    params
  })
}

/**
 * 获取分类列表
 * @param {string} type - 信息类型：COMPANY_NEWS-新闻中心, POLICY_REGULATION-政策法规
 */
export const getCategoryList = (type) => {
  return request({
    url: '/info-publications/categories',
    method: 'get',
    params: { type }
  })
}

export default {
  getInfoPublicationList,
  getInfoPublicationDetail,
  createInfoPublication,
  updateInfoPublication,
  deleteInfoPublication,
  batchDeleteInfoPublications,
  toggleInfoPublicationTop,
  toggleInfoPublicationStatus,
  incrementInfoPublicationView,
  getInfoPublicationStatistics,
  getCategoryList
}
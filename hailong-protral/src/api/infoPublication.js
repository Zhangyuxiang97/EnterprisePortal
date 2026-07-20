import request from './request'

/**
 * 获取信息发布列表
 * @param {Object} params - 查询参数
 * @param {string} params.type - 信息类型 (COMPANY_NEWS: 新闻中心, POLICY_REGULATION: 政策法规)
 * @param {string} params.category - 二级分类
 * @param {string} params.keyword - 关键词搜索
 * @param {string} params.startDate - 开始日期 (YYYY-MM-DD)
 * @param {string} params.endDate - 结束日期 (YYYY-MM-DD)
 * @param {number} params.pageNumber - 页码 (从1开始)
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.sortBy - 排序字段
 * @param {string} params.sortOrder - 排序方式 (asc/desc)
 * @returns {Promise}
 */
export function getInfoPublicationList(params) {
  return request({
    url: '/info-publications',
    method: 'get',
    params: {
      type: params.type,
      category: params.category,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10,
      sortBy: params.sortBy || 'PublishTime',
      sortOrder: params.sortOrder || 'desc'
    }
  })
}

/**
 * 获取信息发布详情
 * @param {number} id - 信息发布ID
 * @returns {Promise}
 */
export function getInfoPublicationDetail(id) {
  return request({
    url: `/info-publications/${id}`,
    method: 'get'
  })
}

/**
 * 获取公司新闻列表
 * @param {Object} params - 查询参数
 * @returns {Promise}
 */
export function getCompanyNewsList(params) {
  return request({
    url: '/info-publications/company-news',
    method: 'get',
    params: {
      category: params.category,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10,
      sortBy: params.sortBy || 'PublishTime',
      sortOrder: params.sortOrder || 'desc'
    }
  })
}

/**
 * 获取政策法规列表
 * @param {Object} params - 查询参数
 * @returns {Promise}
 */
export function getPolicyRegulationsList(params) {
  return request({
    url: '/info-publications/policy-regulations',
    method: 'get',
    params: {
      category: params.category,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10,
      sortBy: params.sortBy || 'PublishTime',
      sortOrder: params.sortOrder || 'desc'
    }
  })
}

/**
 * 获取政策信息列表
 * @param {Object} params - 查询参数
 * @returns {Promise}
 */
export function getPolicyInfoList(params) {
  return request({
    url: '/info-publications/policy-info',
    method: 'get',
    params: {
      category: params.category,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10,
      sortBy: params.sortBy || 'PublishTime',
      sortOrder: params.sortOrder || 'desc'
    }
  })
}

/**
 * 获取通知公告列表
 * @param {Object} params - 查询参数
 * @returns {Promise}
 */
export function getNoticesList(params) {
  return request({
    url: '/info-publications/notices',
    method: 'get',
    params: {
      category: params.category,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10,
      sortBy: params.sortBy || 'PublishTime',
      sortOrder: params.sortOrder || 'desc'
    }
  })
}
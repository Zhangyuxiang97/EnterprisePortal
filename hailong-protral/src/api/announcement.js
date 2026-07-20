import request from './request'

/**
 * 获取公告列表
 * @param {Object} params - 查询参数
 * @param {string} params.businessType - 业务类型 (GOV_PROCUREMENT: 政府采购, CONSTRUCTION: 建设工程)
 * @param {string} params.keyword - 关键词搜索
 * @param {string} params.noticeType - 公告类型 (bidding: 招标/采购公告, correction: 更正公告, result: 结果公告)
 * @param {string} params.procurementType - 采购类型 (goods: 货物, service: 服务, project: 工程)
 * @param {string} params.province - 省份编码
 * @param {string} params.city - 城市编码
 * @param {string} params.district - 区县编码
 * @param {string} params.startDate - 开始日期 (YYYY-MM-DD)
 * @param {string} params.endDate - 结束日期 (YYYY-MM-DD)
 * @param {number} params.pageNumber - 页码 (从1开始)
 * @param {number} params.pageSize - 每页数量
 * @returns {Promise}
 */
export function getAnnouncementList(params) {
  return request({
    url: '/announcements',
    method: 'get',
    params: {
      businessType: params.businessType,
      noticeType: params.noticeType,
      procurementType: params.procurementType,
      province: params.province,
      city: params.city,
      district: params.district,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10
    }
  })
}

/**
 * 获取有公告数据的区域筛选项及对应数量
 * @param {Object} params - 与公告列表一致的筛选参数
 * @returns {Promise}
 */
export function getAnnouncementRegionOptions(params = {}) {
  return request({
    url: '/announcements/region-options',
    method: 'get',
    params: {
      businessType: params.businessType,
      noticeType: params.noticeType,
      procurementType: params.procurementType,
      province: params.province,
      city: params.city,
      district: params.district,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate
    }
  })
}

/**
 * 获取公告详情（通过hashId）
 * @param {string} hashId - 公告hashId
 * @returns {Promise}
 */
export function getAnnouncementByHash(hashId) {
  return request({
    url: `/announcements/by-hash/${hashId}`,
    method: 'get'
  })
}

/**
 * 获取政府采购公告列表
 * @param {Object} params - 查询参数
 * @returns {Promise}
 */
export function getGovProcurementList(params) {
  return request({
    url: '/announcements/gov-procurement',
    method: 'get',
    params: {
      noticeType: params.noticeType,
      procurementType: params.procurementType,
      province: params.province,
      city: params.city,
      district: params.district,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10
    }
  })
}

/**
 * 获取建设工程公告列表
 * @param {Object} params - 查询参数
 * @returns {Promise}
 */
export function getConstructionList(params) {
  return request({
    url: '/announcements/construction',
    method: 'get',
    params: {
      noticeType: params.noticeType,
      province: params.province,
      city: params.city,
      district: params.district,
      keyword: params.keyword,
      startDate: params.startDate,
      endDate: params.endDate,
      pageNumber: params.pageNumber || 1,
      pageSize: params.pageSize || 10
    }
  })
}

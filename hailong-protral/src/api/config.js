import request from './request'

/**
 * ==================== 企业简介 ====================
 */

/**
 * 获取企业简介
 * @returns {Promise}
 */
export function getCompanyProfile() {
  return request({
    url: '/config/company-intro',
    method: 'get'
  })
}

/**
 * ==================== 业务范围 ====================
 */

/**
 * 获取所有业务范围
 * @returns {Promise}
 */
export function getBusinessScope() {
  return request({
    url: '/config/business-scope',
    method: 'get'
  })
}

/**
 * 根据ID获取业务范围详情
 * @param {number} id - 业务范围ID
 * @returns {Promise}
 */
export function getBusinessScopeById(id) {
  return request({
    url: `/config/business-scope/${id}`,
    method: 'get'
  })
}

/**
 * ==================== 企业资质 ====================
 */

/**
 * 获取所有企业资质
 * @returns {Promise}
 */
export function getCompanyQualifications() {
  return request({
    url: '/config/qualifications',
    method: 'get'
  })
}

/**
 * 根据ID获取企业资质详情
 * @param {number} id - 资质ID
 * @returns {Promise}
 */
export function getQualificationById(id) {
  return request({
    url: `/config/qualifications/${id}`,
    method: 'get'
  })
}

/**
 * ==================== 企业荣誉 ====================
 */

/**
 * 获取所有企业荣誉
 * @returns {Promise}
 */
export function getCompanyHonors() {
  return request({
    url: '/config/honors',
    method: 'get'
  })
}

/**
 * 根据ID获取企业荣誉详情
 * @param {number} id - 荣誉ID
 * @returns {Promise}
 */
export function getHonorById(id) {
  return request({
    url: `/config/honors/${id}`,
    method: 'get'
  })
}

/**
 * ==================== 重要业绩 ====================
 */

/**
 * 获取所有重要业绩
 * @returns {Promise}
 */
export function getMajorAchievements() {
  return request({
    url: '/config/achievements',
    method: 'get'
  })
}

/**
 * 根据ID获取重要业绩详情
 * @param {number} id - 业绩ID
 * @returns {Promise}
 */
export function getAchievementById(id) {
  return request({
    url: `/config/achievements/${id}`,
    method: 'get'
  })
}

/**
 * ==================== 访问统计 ====================
 */

/**
 * 获取网站访问统计
 * @returns {Promise}
 */
export function getVisitStatistics() {
  return request({
    url: '/config/statistics',
    method: 'get'
  })
}

/**
 * 记录访问
 * @param {Object} data - 访问数据
 * @param {string} data.pageUrl - 页面URL
 * @param {string} data.pageTitle - 页面标题
 * @param {string} data.referer - 来源页面
 * @returns {Promise}
 */
export function recordVisit(data) {
  return request({
    url: '/config/statistics/record',
    method: 'post',
    data
  })
}
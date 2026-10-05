import request from './request'

export const siteSettings = {
  get: () => request({ url: '/config/site-settings', method: 'get' }),
  update: data => request({ url: '/config/site-settings', method: 'put', data })
}

/**
 * 系统配置 API
 * 包含：企业简介、业务范围、企业资质、重要业绩、企业荣誉、轮播图、友情链接
 */

// ==================== 企业简介 ====================
export const companyProfile = {
  get: () => request({
    url: '/config/manage/company-intro',
    method: 'get'
  }),
  update: (data) => request({
    url: '/config/company-intro',
    method: 'put',
    data
  })
}

// ==================== 业务范围 ====================
export const businessScope = {
  getList: (params) => request({
    url: '/config/manage/business-scope',
    method: 'get',
    params
  }),
  getDetail: (id) => request({
    url: `/config/manage/business-scope/${id}`,
    method: 'get'
  }),
  create: (data) => request({
    url: '/config/business-scope',
    method: 'post',
    data
  }),
  update: (id, data) => request({
    url: `/config/business-scope/${id}`,
    method: 'put',
    data
  }),
  delete: (id) => request({
    url: `/config/business-scope/${id}`,
    method: 'delete'
  })
}

// ==================== 企业资质 ====================
export const qualifications = {
  getList: (params) => request({
    url: '/config/manage/qualifications',
    method: 'get',
    params
  }),
  getDetail: (id) => request({
    url: `/config/manage/qualifications/${id}`,
    method: 'get'
  }),
  create: (data) => request({
    url: '/config/qualifications',
    method: 'post',
    data
  }),
  update: (id, data) => request({
    url: `/config/qualifications/${id}`,
    method: 'put',
    data
  }),
  delete: (id) => request({
    url: `/config/qualifications/${id}`,
    method: 'delete'
  })
}

// ==================== 重要业绩 ====================
export const achievements = {
  getList: (params) => request({
    url: '/config/manage/achievements',
    method: 'get',
    params
  }),
  getDetail: (id) => request({
    url: `/config/manage/achievements/${id}`,
    method: 'get'
  }),
  create: (data) => request({
    url: '/config/achievements',
    method: 'post',
    data
  }),
  update: (id, data) => request({
    url: `/config/achievements/${id}`,
    method: 'put',
    data
  }),
  delete: (id) => request({
    url: `/config/achievements/${id}`,
    method: 'delete'
  }),
  updateSort: (id, direction) => request({
    url: `/config/achievements/${id}/sort`,
    method: 'put',
    data: { direction }
  })
}

// ==================== 企业荣誉 ====================
export const honors = {
  getList: (params) => request({
    url: '/config/manage/honors',
    method: 'get',
    params
  }),
  getDetail: (id) => request({
    url: `/config/manage/honors/${id}`,
    method: 'get'
  }),
  create: (data) => request({
    url: '/config/honors',
    method: 'post',
    data
  }),
  update: (id, data) => request({
    url: `/config/honors/${id}`,
    method: 'put',
    data
  }),
  delete: (id) => request({
    url: `/config/honors/${id}`,
    method: 'delete'
  })
}

// ==================== 轮播图 ====================
export const banners = {
  getList: (params) => request({
    url: '/config/manage/banners',
    method: 'get',
    params
  }),
  getDetail: (id) => request({
    url: `/config/manage/banners/${id}`,
    method: 'get'
  }),
  create: (data) => request({
    url: '/config/banners',
    method: 'post',
    data
  }),
  update: (id, data) => request({
    url: `/config/banners/${id}`,
    method: 'put',
    data
  }),
  delete: (id) => request({
    url: `/config/banners/${id}`,
    method: 'delete'
  }),
  updateSort: (id, direction) => request({
    url: `/config/banners/${id}/sort`,
    method: 'put',
    data: { direction }
  })
}

// ==================== 友情链接 ====================
export const friendlyLinks = {
  getList: (params) => request({
    url: '/config/manage/links',
    method: 'get',
    params
  }),
  getDetail: (id) => request({
    url: `/config/manage/links/${id}`,
    method: 'get'
  }),
  create: (data) => request({
    url: '/config/links',
    method: 'post',
    data
  }),
  update: (id, data) => request({
    url: `/config/links/${id}`,
    method: 'put',
    data
  }),
  delete: (id) => request({
    url: `/config/links/${id}`,
    method: 'delete'
  }),
  updateSort: (id, direction) => request({
    url: `/config/links/${id}/sort`,
    method: 'put',
    data: { direction }
  })
}

export default {
  siteSettings,
  companyProfile,
  businessScope,
  qualifications,
  achievements,
  honors,
  banners,
  friendlyLinks
}

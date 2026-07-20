import request from './request'

/**
 * 系统管理 API
 * 包含：用户管理、系统日志、区域字典
 */

// ==================== 用户管理 ====================
/**
 * 获取用户列表
 * @param {Object} params - 查询参数
 * @param {number} params.page - 页码
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.keyword - 搜索关键词
 * @param {string} params.role - 角色
 * @param {number} params.status - 状态
 */
export const getUserList = (params) => {
  return request({
    url: '/User',
    method: 'get',
    params
  })
}

/**
 * 获取用户详情
 * @param {number} id - 用户ID
 */
export const getUserDetail = (id) => {
  return request({
    url: `/User/${id}`,
    method: 'get'
  })
}

/**
 * 创建用户
 * @param {Object} data - 用户数据
 * @param {string} data.username - 用户名
 * @param {string} data.password - 密码
 * @param {string} data.email - 邮箱
 * @param {string} data.phone - 手机号
 * @param {string} data.realName - 真实姓名
 * @param {string} data.role - 角色
 * @param {number} data.status - 状态
 */
export const createUser = (data) => {
  return request({
    url: '/User',
    method: 'post',
    data
  })
}

/**
 * 更新用户
 * @param {number} id - 用户ID
 * @param {Object} data - 用户数据
 */
export const updateUser = (id, data) => {
  return request({
    url: `/User/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除用户
 * @param {number} id - 用户ID
 */
export const deleteUser = (id) => {
  return request({
    url: `/User/${id}`,
    method: 'delete'
  })
}

/**
 * 重置用户密码
 * @param {number} id - 用户ID
 * @param {string} newPassword - 新密码
 */
export const resetUserPassword = (id, newPassword) => {
  return request({
    url: `/User/${id}/reset-password`,
    method: 'post',
    data: { newPassword }
  })
}

/**
 * 修改当前用户密码
 * @param {Object} data - 密码数据
 * @param {string} data.oldPassword - 旧密码
 * @param {string} data.newPassword - 新密码
 */
export const changePassword = (data) => {
  return request({
    url: '/system/users/change-password',
    method: 'put',
    data
  })
}

/**
 * 启用/禁用用户
 * @param {number} id - 用户ID
 * @param {number} status - 状态：0-禁用，1-启用
 */
export const toggleUserStatus = (id) => {
  return request({
    url: `/User/${id}/toggle-status`,
    method: 'post'
  })
}

// ==================== 系统日志 ====================
/**
 * 获取系统日志列表
 * @param {Object} params - 查询参数
 * @param {number} params.page - 页码
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.username - 用户名
 * @param {string} params.action - 操作动作
 * @param {string} params.module - 操作模块
 * @param {string} params.status - 操作状态
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 */
export const getSystemLogList = (params) => {
  return request({
    url: '/SystemLog',
    method: 'get',
    params
  })
}

/**
 * 获取系统日志详情
 * @param {number} id - 日志ID
 */
export const getSystemLogDetail = (id) => {
  return request({
    url: `/SystemLog/${id}`,
    method: 'get'
  })
}

/**
 * 清理系统日志
 * @param {Object} data - 清理参数
 * @param {string} data.beforeDate - 清理此日期之前的日志
 */
export const clearSystemLogs = () => {
  return request({
    url: '/SystemLog/clear',
    method: 'delete'
  })
}

/**
 * 导出系统日志
 * @param {Object} params - 查询参数
 */
export const exportSystemLogs = (params) => {
  return request({
    url: '/system/logs/export',
    method: 'get',
    params,
    responseType: 'blob'
  })
}

// ==================== 区域字典 ====================
/**
 * 获取区域字典列表
 * @param {Object} params - 查询参数
 * @param {number} params.regionLevel - 区域级别：1-省份，2-城市，3-区县
 * @param {string} params.parentCode - 父级区域代码
 * @param {string} params.keyword - 搜索关键词
 */
export const getRegionList = (params) => {
  return request({
    url: '/regions',
    method: 'get',
    params
  })
}

/**
 * 获取区域树形结构
 */
export const getRegionTree = () => {
  return request({
    url: '/regions/tree',
    method: 'get'
  })
}

/**
 * 获取省份列表
 */
export const getProvinceList = () => {
  return request({
    url: '/regions/provinces',
    method: 'get'
  })
}

/**
 * 获取城市列表
 * @param {string} provinceCode - 省份代码
 */
export const getCityList = (provinceCode) => {
  return request({
    url: '/regions/cities',
    method: 'get',
    params: { provinceCode }
  })
}

/**
 * 获取区县列表
 * @param {string} cityCode - 城市代码
 */
export const getDistrictList = (cityCode) => {
  return request({
    url: '/regions/districts',
    method: 'get',
    params: { cityCode }
  })
}

/**
 * 获取区域详情
 * @param {number} id - 区域ID
 */
export const getRegionDetail = (id) => {
  return request({
    url: `/regions/${id}`,
    method: 'get'
  })
}

/**
 * 根据区域代码获取区域详情
 * @param {string} regionCode - 区域代码
 */
export const getRegionByCode = (regionCode) => {
  return request({
    url: `/regions/by-code/${regionCode}`,
    method: 'get'
  })
}

/**
 * 创建区域
 * @param {Object} data - 区域数据
 * @param {string} data.regionCode - 区域代码
 * @param {string} data.regionName - 区域名称
 * @param {number} data.regionLevel - 区域级别
 * @param {string} data.parentCode - 父级区域代码
 * @param {number} data.sortOrder - 排序顺序
 */
export const createRegion = (data) => {
  return request({
    url: '/regions',
    method: 'post',
    data
  })
}

/**
 * 更新区域
 * @param {number} id - 区域ID
 * @param {Object} data - 区域数据
 */
export const updateRegion = (id, data) => {
  return request({
    url: `/regions/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除区域
 * @param {number} id - 区域ID
 */
export const deleteRegion = (id) => {
  return request({
    url: `/regions/${id}`,
    method: 'delete'
  })
}

/**
 * 批量导入区域数据
 * @param {Array} regions - 区域数据数组
 */
export const importRegions = (regions) => {
  return request({
    url: '/regions/import',
    method: 'post',
    data: regions
  })
}

export default {
  // 用户管理 - 直接导出方法（向后兼容）
  getUserList,
  getUserDetail,
  createUser,
  updateUser,
  deleteUser,
  resetUserPassword,
  changePassword,
  toggleUserStatus,
  
  // 系统日志 - 直接导出方法（向后兼容）
  getSystemLogList,
  getSystemLogDetail,
  clearSystemLogs,
  exportSystemLogs,
  
  // 用户管理 - 对象形式
  users: {
    getList: getUserList,
    getDetail: getUserDetail,
    create: createUser,
    update: updateUser,
    delete: deleteUser,
    resetPassword: resetUserPassword,
    changePassword: changePassword,
    toggleStatus: toggleUserStatus
  },
  
  // 系统日志 - 对象形式
  logs: {
    getList: getSystemLogList,
    getDetail: getSystemLogDetail,
    clear: clearSystemLogs,
    export: exportSystemLogs
  },
  
  // 区域字典
  regions: {
    getList: getRegionList,
    getTree: getRegionTree,
    getProvinces: getProvinceList,
    getCities: getCityList,
    getDistricts: getDistrictList,
    getDetail: getRegionDetail,
    getByCode: getRegionByCode,
    create: createRegion,
    update: updateRegion,
    delete: deleteRegion,
    import: importRegions
  }
}
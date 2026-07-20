import request from './request'

/**
 * 用户管理 API
 */
export default {
  /**
   * 获取用户列表（分页）
   */
  getList(params) {
    return request({
      url: '/User',
      method: 'get',
      params
    })
  },

  /**
   * 根据ID获取用户
   */
  getById(id) {
    return request({
      url: `/User/${id}`,
      method: 'get'
    })
  },

  /**
   * 创建用户
   */
  create(data) {
    return request({
      url: '/User',
      method: 'post',
      data
    })
  },

  /**
   * 更新用户
   */
  update(id, data) {
    return request({
      url: `/User/${id}`,
      method: 'put',
      data
    })
  },

  /**
   * 删除用户
   */
  delete(id) {
    return request({
      url: `/User/${id}`,
      method: 'delete'
    })
  },

  /**
   * 重置用户密码
   */
  resetPassword(id, data) {
    return request({
      url: `/User/${id}/reset-password`,
      method: 'post',
      data
    })
  },

  /**
   * 启用/禁用用户
   */
  toggleStatus(id) {
    return request({
      url: `/User/${id}/toggle-status`,
      method: 'post'
    })
  }
}
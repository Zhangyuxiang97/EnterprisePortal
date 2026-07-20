import request from './request'

/**
 * 认证相关 API
 */
export const authApi = {
  /**
   * 登录
   * @param {Object} data - { username, password }
   * @returns {Promise}
   */
  login(data) {
    return request({
      url: '/auth/login',
      method: 'POST',
      data
    })
  },

  /**
   * 刷新Token
   * @param {string} refreshToken 
   * @returns {Promise}
   */
  refreshToken(refreshToken) {
    return request({
      url: '/auth/refresh',
      method: 'POST',
      data: { refreshToken }
    })
  },

  /**
   * 获取当前用户信息
   * @returns {Promise}
   */
  getCurrentUser() {
    return request({
      url: '/auth/me',
      method: 'GET'
    })
  },

  /**
   * 修改密码
   * @param {Object} data - { oldPassword, newPassword }
   * @returns {Promise}
   */
  changePassword(data) {
    return request({
      url: '/auth/change-password',
      method: 'POST',
      data
    })
  }
}

export default authApi

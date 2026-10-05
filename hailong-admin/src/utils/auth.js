import { API_CONFIG } from '@/config/api.config'

/**
 * Token 工具类
 */
export const tokenUtils = {
  /**
   * 获取Token
   */
  getToken() {
    return localStorage.getItem(API_CONFIG.TOKEN_KEY)
  },

  /**
   * 设置Token
   */
  setToken(token) {
    localStorage.setItem(API_CONFIG.TOKEN_KEY, token)
  },

  /**
   * 移除Token
   */
  removeToken() {
    localStorage.removeItem(API_CONFIG.TOKEN_KEY)
  },

  /**
   * 获取RefreshToken
   */
  getRefreshToken() {
    return localStorage.getItem(API_CONFIG.REFRESH_TOKEN_KEY)
  },

  /**
   * 设置RefreshToken
   */
  setRefreshToken(refreshToken) {
    localStorage.setItem(API_CONFIG.REFRESH_TOKEN_KEY, refreshToken)
  },

  /**
   * 移除RefreshToken
   */
  removeRefreshToken() {
    localStorage.removeItem(API_CONFIG.REFRESH_TOKEN_KEY)
  },

  /**
   * 获取用户信息
   */
  getUserInfo() {
    const userInfo = localStorage.getItem(API_CONFIG.USER_INFO_KEY)
    try { return userInfo ? JSON.parse(userInfo) : null } catch { return null }
  },

  /**
   * 设置用户信息
   */
  setUserInfo(userInfo) {
    localStorage.setItem(API_CONFIG.USER_INFO_KEY, JSON.stringify(userInfo))
  },

  /**
   * 移除用户信息
   */
  removeUserInfo() {
    localStorage.removeItem(API_CONFIG.USER_INFO_KEY)
  },

  /**
   * 清除所有认证信息
   */
  clearAuth() {
    this.removeToken()
    this.removeRefreshToken()
    this.removeUserInfo()
  }
}

export default tokenUtils

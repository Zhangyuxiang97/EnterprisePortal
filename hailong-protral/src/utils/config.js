import { reactive } from 'vue'
import request from '@/api/request'
import staticConfig from '../../config/site-config.json'
import defaults from '../../../BackEnd/HailongConsulting.API/SiteSettingsDefaults.json'
import { createSiteSettingsLoader } from './siteSettingsLoader'

// 导航和首页组件布局保持静态；站点展示内容在启动时从 API 更新。
const siteConfig = reactive(JSON.parse(JSON.stringify({ ...staticConfig, ...defaults })))
let storage
try { storage = window.localStorage } catch { /* 浏览器禁用存储时保留网络读取 */ }
const settingsLoader = createSiteSettingsLoader({
  fetchSettings: () => request({ url: '/config/site-settings', method: 'get', timeout: 3000 }),
  applySettings: data => Object.assign(siteConfig, data),
  storage
})

export const loadSiteConfig = settingsLoader.load

/**
 * 获取站点配置
 * @returns {Object} 站点配置对象
 */
export const getSiteConfig = () => {
  return siteConfig
}

/**
 * 获取公司信息
 * @returns {Object} 公司信息
 */
export const getCompanyInfo = () => {
  return siteConfig.company
}

/**
 * 获取联系方式
 * @returns {Object} 联系方式
 */
export const getContactInfo = () => {
  return siteConfig.contact
}

/**
 * 获取交通指引
 * @returns {Object} 交通指引
 */
export const getTransportation = () => {
  return siteConfig.transportation
}

/**
 * 获取导航配置
 * @returns {Object} 导航配置
 */
export const getNavigation = () => {
  return siteConfig.navigation
}

/**
 * 获取常见问题
 * @returns {Array} FAQ列表
 */
export const getFAQs = () => {
  return siteConfig.faqs
}

// 导出默认配置
export default siteConfig

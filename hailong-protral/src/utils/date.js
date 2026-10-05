/**
 * 日期格式化工具函数
 */

/**
 * 格式化日期时间为 yyyy-MM-dd HH:mm:ss
 * @param {string|Date} date - 日期字符串或Date对象
 * @returns {string} 格式化后的日期时间字符串
 */
export function formatDateTime(date) {
  if (!date) return '-'
  
  try {
    const dateObj = date instanceof Date ? date : new Date(date)
    
    if (isNaN(dateObj.getTime())) {
      return '-'
    }
    
    const year = dateObj.getFullYear()
    const month = String(dateObj.getMonth() + 1).padStart(2, '0')
    const day = String(dateObj.getDate()).padStart(2, '0')
    const hours = String(dateObj.getHours()).padStart(2, '0')
    const minutes = String(dateObj.getMinutes()).padStart(2, '0')
    const seconds = String(dateObj.getSeconds()).padStart(2, '0')
    
    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
  } catch (e) {
    console.error('日期格式化错误:', e)
    return '-'
  }
}

/**
 * 格式化日期为 yyyy-MM-dd
 * @param {string|Date} date - 日期字符串或Date对象
 * @returns {string} 格式化后的日期字符串
 */
export function formatDate(date) {
  if (!date) return '-'
  
  try {
    const dateObj = date instanceof Date ? date : new Date(date)
    
    if (isNaN(dateObj.getTime())) {
      return '-'
    }
    
    const year = dateObj.getFullYear()
    const month = String(dateObj.getMonth() + 1).padStart(2, '0')
    const day = String(dateObj.getDate()).padStart(2, '0')
    
    return `${year}-${month}-${day}`
  } catch (e) {
    console.error('日期格式化错误:', e)
    return '-'
  }
}

/**
 * 格式化日期为中文格式 yyyy年MM月dd日
 * @param {string|Date} date - 日期字符串或Date对象
 * @returns {string} 格式化后的日期字符串
 */
export function formatDateCN(date) {
  if (!date) return '-'
  
  try {
    const dateObj = date instanceof Date ? date : new Date(date)
    
    if (isNaN(dateObj.getTime())) {
      return '-'
    }
    
    const year = dateObj.getFullYear()
    const month = dateObj.getMonth() + 1
    const day = dateObj.getDate()
    
    return `${year}年${month}月${day}日`
  } catch (e) {
    console.error('日期格式化错误:', e)
    return '-'
  }
}

/**
 * 获取当前日期时间字符串 yyyy-MM-dd HH:mm:ss
 * @returns {string} 当前日期时间字符串
 */
export function getCurrentDateTime() {
  return formatDateTime(new Date())
}

/**
 * 获取当前日期字符串 yyyy-MM-dd
 * @returns {string} 当前日期字符串
 */
export function getCurrentDate() {
  return formatDate(new Date())
}

// 以本地日历日期计算，范围包含今天；避免 UTC 转换与夏令时导致偏移。
export function getDateRange(range, today = new Date()) {
  const days = { today: 1, '3days': 3, week: 7, month: 30 }[range]
  if (!days) return { startDate: '', endDate: '' }
  const start = new Date(today)
  start.setDate(start.getDate() - days + 1)
  return { startDate: formatDate(start), endDate: formatDate(today) }
}

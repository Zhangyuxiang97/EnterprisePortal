import request from './request'

/**
 * 获取省份列表
 * @returns {Promise}
 */
export function getProvinceList() {
  return request({
    url: '/regions/provinces',
    method: 'get'
  })
}

/**
 * 获取城市列表
 * @param {string} provinceCode - 省份代码
 * @returns {Promise}
 */
export function getCityList(provinceCode) {
  return request({
    url: '/regions/cities',
    method: 'get',
    params: { provinceCode }
  })
}

/**
 * 获取区县列表
 * @param {string} cityCode - 城市代码
 * @returns {Promise}
 */
export function getDistrictList(cityCode) {
  return request({
    url: '/regions/districts',
    method: 'get',
    params: { cityCode }
  })
}

/**
 * 获取区域树形结构
 * @returns {Promise}
 */
export function getRegionTree() {
  return request({
    url: '/regions/tree',
    method: 'get'
  })
}

/**
 * 根据区域代码获取区域详情
 * @param {string} regionCode - 区域代码
 * @returns {Promise}
 */
export function getRegionByCode(regionCode) {
  return request({
    url: `/regions/by-code/${regionCode}`,
    method: 'get'
  })
}
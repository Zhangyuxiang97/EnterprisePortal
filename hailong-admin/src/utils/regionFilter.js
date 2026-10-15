export const UNLOCATED_PROVINCE = '__unlocated__'

// 业务筛选按当前计数排列；并列与零数量仍遵循字典排序，不修改原字典数组。
export function sortRegionsByCount(regions, counts) {
  const countByCode = new Map((counts || []).map(item => [item.regionCode, item.count || 0]))
  return [...regions].sort((a, b) =>
    (countByCode.get(b.regionCode) || 0) - (countByCode.get(a.regionCode) || 0) ||
    (a.sortOrder || 0) - (b.sortOrder || 0) ||
    a.regionCode.localeCompare(b.regionCode))
}

export function toAnnouncementRegionQuery(params) {
  if (params.province !== UNLOCATED_PROVINCE) return params
  return { ...params, province: undefined, city: undefined, district: undefined, provinceUnlocated: true }
}

export const UNLOCATED_PROVINCE = '__unlocated__'

// 未明确是筛选状态，接口与数据库仍使用实际地区编码。
export function toAnnouncementRegionQuery(params) {
  if (params.province !== UNLOCATED_PROVINCE) return params
  return { ...params, province: undefined, city: undefined, district: undefined, provinceUnlocated: true }
}

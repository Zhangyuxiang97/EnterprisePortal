export const UNLOCATED_PROVINCE = '__unlocated__'

export function toAnnouncementRegionQuery(params) {
  if (params.province !== UNLOCATED_PROVINCE) return params
  return { ...params, province: undefined, city: undefined, district: undefined, provinceUnlocated: true }
}

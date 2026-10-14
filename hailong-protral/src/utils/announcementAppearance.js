export function getAnnouncementTone(noticeType) {
  return ['bidding', 'result', 'correction'].includes(noticeType)
    ? `announcement-tone-${noticeType}`
    : 'announcement-tone-default'
}

export function getAnnouncementClassification(announcement) {
  const business = { GOV_PROCUREMENT: '政府采购', CONSTRUCTION: '建设工程' }[announcement.businessType] || '其他'
  if (announcement.businessType !== 'GOV_PROCUREMENT' || !announcement.procurementType) return business
  const procurement = { goods: '货物', service: '服务', project: '工程' }[announcement.procurementType] || announcement.procurementType
  return `${business} · ${procurement}`
}

import governmentProcurement from '@/assets/business-defaults/government-procurement.webp'
import constructionTendering from '@/assets/business-defaults/construction-tendering.webp'
import costConsulting from '@/assets/business-defaults/cost-consulting.webp'
import constructionSupervision from '@/assets/business-defaults/construction-supervision.webp'
import projectManagement from '@/assets/business-defaults/project-management.webp'
import tenderDocumentPreparation from '@/assets/business-defaults/tender-document-preparation.webp'

const defaultImages = {
  '政府采购代理': governmentProcurement,
  '建设工程招标': constructionTendering,
  '工程造价咨询': costConsulting,
  '工程监理': constructionSupervision,
  '项目管理': projectManagement,
  '招标文件编制': tenderDocumentPreparation
}

export function getBusinessImage(business) {
  return business.imageUrl || defaultImages[business.name?.trim()] || ''
}

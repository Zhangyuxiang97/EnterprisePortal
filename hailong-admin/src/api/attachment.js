import request from './request'

/**
 * 附件管理 API
 * 对应数据库表：attachments
 * 管理系统中的所有文件附件
 */

/**
 * 获取附件列表
 * @param {Object} params - 查询参数
 * @param {number} params.page - 页码
 * @param {number} params.pageSize - 每页数量
 * @param {string} params.fileType - 文件类型：image/document/video/other
 * @param {string} params.relatedType - 关联类型：announcement/info_publication/config
 * @param {number} params.relatedId - 关联ID
 * @param {string} params.keyword - 搜索关键词
 * @param {string} params.startDate - 开始日期
 * @param {string} params.endDate - 结束日期
 */
export const getAttachmentList = (params) => {
  return request({
    url: '/attachments',
    method: 'get',
    params
  })
}

/**
 * 获取附件详情
 * @param {number} id - 附件ID
 */
export const getAttachmentDetail = (id) => {
  return request({
    url: `/attachments/${id}`,
    method: 'get'
  })
}

/**
 * 上传附件
 * @param {FormData} formData - 包含文件的表单数据
 * @param {string} relatedType - 关联类型（可选）
 * @param {number} relatedId - 关联ID（可选）
 */
export const uploadAttachment = (formData) => {
  return request({
    url: '/attachments/upload',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 批量上传附件
 * @param {FormData} formData - 包含多个文件的表单数据
 */
export const batchUploadAttachments = (formData) => {
  return request({
    url: '/attachments/batch-upload',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 更新附件信息
 * @param {number} id - 附件ID
 * @param {Object} data - 附件数据
 * @param {string} data.fileName - 文件名
 * @param {string} data.description - 描述
 * @param {string} data.relatedType - 关联类型
 * @param {number} data.relatedId - 关联ID
 */
export const updateAttachment = (id, data) => {
  return request({
    url: `/attachments/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除附件
 * @param {number} id - 附件ID
 */
export const deleteAttachment = (id) => {
  return request({
    url: `/attachments/${id}`,
    method: 'delete'
  })
}

/**
 * 批量删除附件
 * @param {Array<number>} ids - 附件ID数组
 */
export const batchDeleteAttachments = (ids) => {
  return request({
    url: '/attachments/batch',
    method: 'delete',
    data: ids
  })
}

/**
 * 下载附件
 * @param {number} id - 附件ID
 */
export const downloadAttachment = (id) => {
  return request({
    url: `/attachments/${id}/download`,
    method: 'get',
    responseType: 'blob'
  })
}

/**
 * 获取附件统计信息
 * @param {Object} params - 查询参数
 */
export const getAttachmentStatistics = (params) => {
  return request({
    url: '/attachments/statistics',
    method: 'get',
    params
  })
}

/**
 * 关联附件到实体
 * @param {Object} data - 关联数据
 * @param {Array<number>} data.attachmentIds - 附件ID列表
 * @param {string} data.relatedType - 关联类型
 * @param {number} data.relatedId - 关联ID
 */
export const linkAttachments = (data) => {
  return request({
    url: '/attachments/link',
    method: 'post',
    data
  })
}

/**
 * 取消附件关联
 * @param {Object} data - 关联数据
 * @param {Array<number>} data.attachmentIds - 附件ID列表
 * @param {string} data.relatedType - 关联类型
 * @param {number} data.relatedId - 关联ID
 */
export const unlinkAttachments = (data) => {
  return request({
    url: '/attachments/unlink',
    method: 'post',
    data
  })
}

export default {
  getAttachmentList,
  getAttachmentDetail,
  uploadAttachment,
  batchUploadAttachments,
  updateAttachment,
  deleteAttachment,
  batchDeleteAttachments,
  downloadAttachment,
  getAttachmentStatistics,
  linkAttachments,
  unlinkAttachments
}
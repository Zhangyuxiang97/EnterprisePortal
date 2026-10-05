import { reactive } from 'vue'
import request from '@/api/request'

export const uploadOptions = reactive({
  maxFileSize: 10 * 1024 * 1024,
  image: ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.ico'],
  document: ['.pdf', '.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx', '.txt'],
  video: ['.mp4', '.avi', '.mov', '.wmv', '.flv', '.mkv', '.webm']
})
let pending
export function loadUploadOptions() {
  if (!pending) pending = request({ url: '/attachments/upload-options', method: 'get' })
    .then(res => {
      if (!res.success) throw new Error(res.message || '无法读取上传限制')
      Object.assign(uploadOptions, res.data)
      return uploadOptions
    }).catch(error => { pending = undefined; throw error })
  return pending
}
export function validateUpload(file, extensions, maxSize = uploadOptions.maxFileSize) {
  if (!file.size) return '不能上传空文件'
  if (file.size > maxSize) return `单个文件不能超过 ${maxSize / 1024 / 1024}MB`
  const extension = `.${file.name.split('.').pop().toLowerCase()}`
  if (!extensions.includes(extension)) return `不支持此格式，可上传 ${extensions.join('、')}`
  return ''
}

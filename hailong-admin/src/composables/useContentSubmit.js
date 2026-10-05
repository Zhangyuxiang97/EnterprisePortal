import { ElMessage } from 'element-plus'
import { notifyError } from '@/utils/errors'

// 公告和资讯保留各自字段，统一校验、重复提交保护、错误及保存后的状态。
export function useContentSubmit({ formRef, submitting, isEdit, formData, buildPayload, create, update, onSaved }) {
  return async () => {
    if (!formRef.value || submitting.value) return
    try { await formRef.value.validate() }
    catch { ElMessage.warning('请正确填写表单'); return }
    submitting.value = true
    try {
      const response = isEdit.value ? await update(formData.id, buildPayload()) : await create(buildPayload())
      if (!response.success) throw new Error(response.message || '保存失败')
      ElMessage.success(response.message || (isEdit.value ? '更新成功' : '创建成功'))
      await onSaved()
    } catch (error) { notifyError(error, '保存失败，请稍后重试') }
    finally { submitting.value = false }
  }
}

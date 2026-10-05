import { computed, ref, watch, onMounted, onBeforeUnmount, provide } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useUserStore } from '@/stores/user'
import { useEditorLeaveGuard } from '@/composables/useEditorLeaveGuard'

export function useEditorForm(form, visible, submitting) {
  const baseline = ref('')
  const activeUploads = ref(0)
  provide('editorActiveUploads', activeUploads)
  const canManage = computed(() => useUserStore().userInfo?.role === 'admin')
  const snapshot = () => JSON.stringify(form)
  const markSaved = () => { baseline.value = snapshot() }
  const dirty = computed(() => visible.value && baseline.value !== snapshot())
  watch(visible, open => { if (open) markSaved() }, { flush: 'sync', immediate: true })
  const canLeave = async () => {
    if (submitting.value || activeUploads.value) {
      ElMessage.warning('正在保存或上传文件，请等待完成')
      return false
    }
    if (!dirty.value) return true
    try {
      await ElMessageBox.confirm('修改尚未保存，离开后将丢失本次修改。', '放弃修改？', {
        confirmButtonText: '放弃修改', cancelButtonText: '继续编辑', type: 'warning', closeOnClickModal: false
      })
      return true
    } catch { return false }
  }
  const handleCancel = async () => { if (await canLeave()) visible.value = false }
  const beforeEditorClose = async done => { if (await canLeave()) done() }
  const beforeUnload = event => {
    if (dirty.value || activeUploads.value || submitting.value) {
      event.preventDefault()
      event.returnValue = ''
    }
  }
  useEditorLeaveGuard(canLeave)
  onMounted(() => window.addEventListener('beforeunload', beforeUnload))
  onBeforeUnmount(() => window.removeEventListener('beforeunload', beforeUnload))
  return { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads }
}

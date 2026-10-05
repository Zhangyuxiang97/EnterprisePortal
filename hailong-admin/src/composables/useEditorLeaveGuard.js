import { onBeforeUnmount } from 'vue'
import { onBeforeRouteLeave } from 'vue-router'
import { registerEditorGuard } from '@/utils/editorLeave'

export function useEditorLeaveGuard(canLeave) {
  let approved = false
  const guard = async () => { approved = await canLeave(); return approved }
  guard.reset = () => { approved = false }
  const unregister = registerEditorGuard(guard)
  onBeforeRouteLeave(() => {
    if (approved) { approved = false; return true }
    return canLeave()
  })
  onBeforeUnmount(unregister)
}

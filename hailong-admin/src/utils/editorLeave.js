const guards = new Set()

export function registerEditorGuard(guard) {
  guards.add(guard)
  return () => guards.delete(guard)
}

export async function confirmEditorsLeave() {
  for (const guard of guards) if (!await guard()) return false
  return true
}

export function resetEditorLeaveApprovals() { for (const guard of guards) guard.reset?.() }

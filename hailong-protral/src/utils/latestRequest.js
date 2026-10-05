// 同一资源只允许最后一次请求修改状态；即使底层请求不支持取消也有效。
export function createLatestRequest() {
  let version = 0
  let controller
  const cancel = () => {
    version += 1
    controller?.abort()
  }
  return {
    cancel,
    begin() {
      cancel()
      controller = new AbortController()
      const current = version
      return { signal: controller.signal, isCurrent: () => current === version }
    }
  }
}

import { onMounted, onUnmounted } from 'vue'

// 首页异步模块会在路由恢复滚动后改变高度。布局变化时再次对齐保存位置，
// 用户主动滚动/点击后立即交回控制，离页或超过接口超时窗口后释放观察器。
export function useScrollRestoration(element, position) {
  let observer, frame, timeout
  const events = ['wheel', 'touchstart', 'pointerdown', 'keydown']
  const stop = () => {
    observer?.disconnect()
    cancelAnimationFrame(frame)
    clearTimeout(timeout)
    for (const event of events) window.removeEventListener(event, stop, true)
  }
  onMounted(() => {
    if (!position || !element.value || !window.ResizeObserver) return
    const restore = () => {
      cancelAnimationFrame(frame)
      frame = requestAnimationFrame(() => window.scrollTo({ ...position, behavior: 'instant' }))
    }
    observer = new ResizeObserver(restore)
    observer.observe(element.value)
    for (const event of events) window.addEventListener(event, stop, { capture: true, passive: true })
    timeout = setTimeout(stop, 10000)
    restore()
  })
  onUnmounted(stop)
}

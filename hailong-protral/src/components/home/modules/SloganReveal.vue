<template>
  <span ref="root" class="slogan-reveal" :data-state="state" :data-phase="phase" aria-hidden="true">
    <span ref="layout" class="slogan-layout">
      <span v-for="(character, index) in characters" :key="index" class="slogan-measure">{{ character }}</span>
    </span>
    <span v-if="!measured" class="slogan-initial">{{ initialText }}</span>
    <span v-show="measured" ref="layer" class="slogan-layer">
      <span v-for="(character, index) in characters" :key="index" class="slogan-glyph">{{ character }}</span>
    </span>
  </span>
</template>

<script setup>
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from 'vue'

const props = defineProps({ text: { type: String, default: '' }, ready: Boolean })
const segmenter = typeof Intl.Segmenter === 'function'
  ? new Intl.Segmenter('zh', { granularity: 'grapheme' }) : null
const characters = computed(() => segmenter
  ? Array.from(segmenter.segment(props.text), item => item.segment) : Array.from(props.text))
const initialText = computed(() => characters.value.length <= 2
  ? props.text : characters.value[0] + characters.value.at(-1))
const root = ref(null)
const layout = ref(null)
const layer = ref(null)
const measured = ref(false)
const state = ref('waiting')
const phase = ref('waiting')
const motionPreference = window.matchMedia('(prefers-reduced-motion: reduce)')
const reducedMotion = ref(motionPreference.matches)
let mounted = false
let generation = 0
let frameId = 0
let resizeObserver
let lastSize = { width: 0, height: 0 }

const ease = value => {
  const t = Math.max(0, Math.min(1, value))
  return t * t * (3 - 2 * t)
}

// 两端短暂柔和加减速，中段匀速；整段展开只有一条连续时间线。
const expansionProgress = value => {
  const t = Math.max(0, Math.min(1, value))
  const ramp = 0.12
  const rampDistance = u => ramp * (u ** 3 - u ** 4 / 2) / (1 - ramp)
  if (t < ramp) return rampDistance(t / ramp)
  if (t > 1 - ramp) return 1 - rampDistance((1 - t) / ramp)
  return (t - ramp / 2) / (1 - ramp)
}

async function prepareAnimation(finishImmediately = false) {
  const currentGeneration = ++generation
  window.cancelAnimationFrame(frameId)
  measured.value = false
  state.value = phase.value = 'waiting'
  await nextTick()
  await document.fonts.ready
  if (!mounted || currentGeneration !== generation) return

  // 只在启动或尺寸变化时测量；动画帧使用缓存尺寸。
  const bounds = root.value.getBoundingClientRect()
  const metrics = Array.from(layout.value.children, element => {
    const rect = element.getBoundingClientRect()
    return { width: rect.width, height: rect.height, x: rect.left - bounds.left, y: rect.top - bounds.top }
  })
  const glyphs = Array.from(layer.value.children)
  const count = metrics.length
  const pairCount = Math.max(0, Math.ceil(count / 2) - 1)
  const singleLine = metrics.reduce((sum, item) => sum + item.width, 0) <= bounds.width + 0.5
  const expandDuration = Math.min(reducedMotion.value ? 1300 : 1100, (reducedMotion.value ? 380 : 320) * Math.max(1, pairCount))
  const fadeDuration = reducedMotion.value ? 140 : 100
  const duration = expandDuration + fadeDuration
  const initialWidth = count ? metrics[0].width + (count > 1 ? metrics.at(-1).width : 0) : 0
  const pairWidths = Array.from({ length: pairCount }, () => 0)
  metrics.forEach((item, index) => {
    const distance = Math.min(index, count - 1 - index)
    if (distance) pairWidths[distance - 1] += item.width
  })
  const totalSpace = pairWidths.reduce((sum, width) => sum + width, 0)
  let allocatedSpace = 0
  const pairs = pairWidths.map(width => {
    const startSpace = allocatedSpace
    allocatedSpace += width
    const threshold = allocatedSpace / totalSpace
    // 每对字的显现时间提前求好，动画帧不测量布局或求解曲线。
    let low = 0
    let high = 1
    for (let step = 0; step < 30; step++) {
      const middle = (low + high) / 2
      if (expansionProgress(middle) < threshold) low = middle
      else high = middle
    }
    return { width, startSpace, revealTime: high * expandDuration }
  })
  lastSize = { width: bounds.width, height: bounds.height }

  const draw = elapsed => {
    const availableSpace = totalSpace * expansionProgress(elapsed / expandDuration)
    const spaces = pairs.map(pair => Math.max(0, Math.min(1, (availableSpace - pair.startSpace) / pair.width)))
    const widths = metrics.map((item, index) => {
      const distance = Math.min(index, count - 1 - index)
      return item.width * (distance === 0 ? 1 : spaces[distance - 1])
    })
    let cursor = (bounds.width - widths.reduce((sum, width) => sum + width, 0)) / 2

    glyphs.forEach((element, index) => {
      const item = metrics[index]
      const distance = Math.min(index, count - 1 - index)
      let x = item.x
      let y = item.y
      if (singleLine) {
        x = cursor + (widths[index] - item.width) / 2
        y = (bounds.height - item.height) / 2
        cursor += widths[index]
      } else if (distance === 0) {
        // 多行标语先把两端移动到最终排版，随后在正常字位逐对补齐。
        const progress = ease(elapsed / (pairs[0]?.revealTime || expandDuration))
        const startX = (bounds.width - initialWidth) / 2 + (index === 0 ? 0 : metrics[0].width)
        const startY = (bounds.height - item.height) / 2
        x = startX + (item.x - startX) * progress
        y = startY + (item.y - startY) * progress
      }
      element.style.transform = 'translate3d(' + x.toFixed(3) + 'px, ' + y.toFixed(3) + 'px, 0)'
      // 空间足够才浮现；显现期间展开继续进行，文字保持正常字宽。
      const opacity = distance === 0 ? 1 : spaces[distance - 1] === 1
        ? ease((elapsed - pairs[distance - 1].revealTime) / fadeDuration) : 0
      element.style.opacity = opacity.toFixed(4)
    })
    return elapsed < expandDuration ? 'expanding' : 'revealing'
  }

  draw(finishImmediately || count <= 2 ? duration : 0)
  measured.value = true
  if (finishImmediately || count <= 2) {
    state.value = phase.value = 'complete'
    return
  }
  if (!props.ready) return

  const start = performance.now() + 300
  const tick = time => {
    if (!mounted || currentGeneration !== generation) return
    if (time >= start) {
      const elapsed = Math.min(duration, time - start)
      state.value = 'playing'
      phase.value = draw(elapsed)
      if (elapsed >= duration) {
        state.value = phase.value = 'complete'
        return
      }
    }
    frameId = window.requestAnimationFrame(tick)
  }
  frameId = window.requestAnimationFrame(tick)
}

const updateMotionPreference = event => { reducedMotion.value = event.matches }
watch([characters, () => props.ready, reducedMotion], () => {
  if (mounted) prepareAnimation()
}, { flush: 'post' })

onMounted(() => {
  mounted = true
  motionPreference.addEventListener('change', updateMotionPreference)
  resizeObserver = new ResizeObserver(entries => {
    const { width, height } = entries[0].contentRect
    if (measured.value && (Math.abs(width - lastSize.width) > 0.5 || Math.abs(height - lastSize.height) > 0.5)) prepareAnimation(true)
  })
  resizeObserver.observe(root.value)
  prepareAnimation()
})

onUnmounted(() => {
  mounted = false
  generation += 1
  window.cancelAnimationFrame(frameId)
  resizeObserver.disconnect()
  motionPreference.removeEventListener('change', updateMotionPreference)
})
</script>

<style scoped>
.slogan-reveal {
  display: block;
  position: relative;
}

.slogan-layout {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  visibility: hidden;
}

.slogan-measure,
.slogan-glyph {
  display: block;
  white-space: pre;
}

.slogan-initial {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #f1f5f9;
}

.slogan-layer {
  position: absolute;
  inset: 0;
}

.slogan-glyph {
  position: absolute;
  top: 0;
  left: 0;
  color: #f1f5f9;
  opacity: 0;
}

.slogan-reveal[data-state='playing'] .slogan-glyph {
  will-change: transform, opacity;
}
</style>

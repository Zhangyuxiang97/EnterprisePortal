<template>
  <div ref="container" class="home-module" :data-home-module="title" :data-home-surface="surface">
    <component v-if="visible" :is="component" v-bind="$attrs" />
    <section v-else class="module-placeholder" aria-busy="true" :aria-label="`${title}待加载`">
      <h2>{{ title }}</h2>
      <div class="placeholder-line"></div>
      <div class="placeholder-card"></div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
defineOptions({ inheritAttrs: false })
const props = defineProps({ component: { type: [Object, Function], required: true }, title: String, eager: Boolean, surface: { type: String, default: 'content' } })
const container = ref(null)
const visible = ref(props.eager)
let observer
onMounted(() => {
  if (visible.value) return
  if (!('IntersectionObserver' in window)) { visible.value = true; return }
  observer = new IntersectionObserver(entries => {
    if (entries.some(entry => entry.isIntersecting)) {
      visible.value = true
      observer.disconnect()
    }
  }, { rootMargin: '320px 0px' })
  observer.observe(container.value)
})
onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
.module-placeholder { min-height: 480px; padding: var(--home-section-space, 80px) 24px; background: transparent; text-align: center; }
.home-module[data-home-surface="closing"] .module-placeholder { background: #0f1419; border-radius: 2rem 2rem 0 0; }
.home-module[data-home-surface="closing"] .module-placeholder h2 { color: #fff; }
.home-module[data-home-surface="closing"] .placeholder-card { background: #263140; }
.module-placeholder h2 { margin: 0 0 16px; color: #1e293b; font-size: 30px; font-weight: 800; }
.placeholder-line { width: 48px; height: 4px; margin: 0 auto 48px; border-radius: 4px; background: #cbd5e1; }
.placeholder-card { height: 160px; max-width: 1000px; margin: auto; border-radius: 16px; background: #e2e8f0; }
</style>

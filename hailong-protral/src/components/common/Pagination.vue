<template>
  <nav v-if="total > 0" aria-label="列表分页" class="mt-10 flex flex-wrap items-center justify-center gap-2 text-sm">
    <button type="button" :disabled="disabled || page <= 1" @click="change(page - 1)" class="page-button">上一页</button>
    <span class="px-2 sm:hidden">{{ page }} / {{ pages }}</span>
    <template v-for="(number, index) in numbers" :key="`${number}-${index}`">
      <span v-if="number === '…'" class="hidden px-1 sm:inline">…</span>
      <button v-else type="button" class="page-button hidden sm:block" :class="{ selected: page === number }" :aria-current="page === number ? 'page' : undefined" :aria-label="`第 ${number} 页`" :disabled="disabled" @click="change(number)">{{ number }}</button>
    </template>
    <button type="button" :disabled="disabled || page >= pages" @click="change(page + 1)" class="page-button">下一页</button>
  </nav>
</template>
<script setup>
import { computed } from 'vue'
const props = defineProps({ total: { type: Number, default: 0 }, page: { type: Number, default: 1 }, pageSize: { type: Number, default: 10 }, disabled: Boolean })
const emit = defineEmits(['change'])
const pages = computed(() => Math.max(1, Math.ceil(props.total / props.pageSize)))
const numbers = computed(() => {
  const selected = new Set([1, pages.value])
  const start = Math.max(1, Math.min(props.page - 2, pages.value - 4))
  for (let index = start; index <= Math.min(pages.value, start + 4); index++) selected.add(index)
  const result = []
  for (const page of [...selected].sort((a, b) => a - b)) {
    if (result.length && page - result.at(-1) > 1) result.push('…')
    result.push(page)
  }
  return result
})
const change = page => { if (!props.disabled && page >= 1 && page <= pages.value && page !== props.page) emit('change', page) }
</script>
<style scoped>
.page-button { @apply rounded-lg border border-slate-200 bg-white px-3 py-2 text-slate-600 transition-colors hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40; }
.page-button.selected { @apply border-hailong-primary bg-hailong-primary text-white; }
</style>

<template>
  <div class="rounded-2xl border border-slate-100 bg-white px-5 py-12 text-center" :role="error ? 'alert' : 'status'" :aria-busy="loading">
    <div v-if="loading" class="mx-auto mb-4 h-8 w-8 animate-spin rounded-full border-2 border-hailong-primary border-t-transparent"></div>
    <p class="text-sm text-slate-600">{{ loading ? '正在加载，请稍候…' : error || emptyText }}</p>
    <button v-if="error && !loading" type="button" class="mt-4 rounded-lg bg-hailong-primary px-5 py-2 text-sm text-white hover:bg-hailong-secondary" @click="$emit('retry')">重新加载</button>
  </div>
</template>
<script setup>
defineProps({ loading: Boolean, error: String, emptyText: { type: String, default: '暂无相关内容，可以尝试调整筛选条件。' } })
defineEmits(['retry'])
</script>

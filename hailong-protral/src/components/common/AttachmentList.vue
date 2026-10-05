<template>
  <section v-if="attachments?.length" class="mb-6 rounded-xl bg-white p-5 shadow-sm md:p-8">
    <h2 class="mb-5 border-b border-slate-200 pb-4 text-xl font-bold text-slate-800">附件列表</h2>
    <ul class="space-y-3">
      <li v-for="attachment in attachments" :key="attachment.id || attachment.fileUrl" class="flex flex-col gap-3 rounded-lg bg-slate-50 p-4 sm:flex-row sm:items-center sm:justify-between">
        <div class="min-w-0">
          <p class="break-all font-medium text-slate-800">{{ attachment.fileName }}</p>
          <p class="mt-1 text-xs text-slate-500">{{ formatFileSize(attachment.fileSize) }}</p>
        </div>
        <div class="no-print flex shrink-0 gap-2 text-sm">
          <button type="button" @click="$emit('preview', attachment)" class="whitespace-nowrap rounded-lg border border-slate-300 bg-white px-4 py-2 text-slate-700 hover:bg-slate-100">预览</button>
          <a :href="attachment.fileUrl" download class="whitespace-nowrap rounded-lg bg-hailong-primary px-4 py-2 text-white hover:bg-hailong-secondary">下载</a>
        </div>
      </li>
    </ul>
  </section>
</template>
<script setup>
import { formatFileSize } from '@/utils/file'
defineProps({ attachments: { type: Array, default: () => [] } })
defineEmits(['preview'])
</script>

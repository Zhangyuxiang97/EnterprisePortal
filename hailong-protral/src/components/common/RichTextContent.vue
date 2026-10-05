<template>
  <div class="rich-text" v-html="content || ''"></div>
</template>
<script setup>
// 内容由后端公共 HTML 过滤器清洗；此处统一排版，不改写媒体 URL。
defineProps({ content: { type: String, default: '' } })
</script>
<style scoped>
.rich-text { min-width: 0; max-width: 100%; overflow-x: auto; overflow-wrap: anywhere; color: #374151; line-height: 1.85; }
.rich-text :deep(h1), .rich-text :deep(h2), .rich-text :deep(h3), .rich-text :deep(h4) { margin: 1.5em 0 .75em; font-weight: 700; line-height: 1.5; color: #1e293b; }
.rich-text :deep(h1) { font-size: 1.75em; }
.rich-text :deep(h2) { font-size: 1.5em; }
.rich-text :deep(h3) { font-size: 1.25em; }
.rich-text :deep(p) { margin: 0 0 1em; }
.rich-text :deep(img), .rich-text :deep(video) { max-width: 100% !important; height: auto !important; }
.rich-text :deep(ul) { list-style: disc; padding-left: 1.75em; margin-bottom: 1em; }
.rich-text :deep(ol) { list-style: decimal; padding-left: 1.75em; margin-bottom: 1em; }
.rich-text :deep(li) { margin-bottom: .35em; }
.rich-text :deep(a) { color: #2563eb; text-decoration: underline; text-underline-offset: 3px; }
.rich-text :deep(table) { border-collapse: collapse; margin: 1.25em 0; min-width: 100%; }
.rich-text :deep(th), .rich-text :deep(td) { border: 1px solid #cbd5e1; padding: .6em .8em; vertical-align: top; }
.rich-text :deep(th) { background: #f1f5f9; font-weight: 600; }
.rich-text :deep(blockquote) { margin: 1em 0; padding-left: 1em; border-left: 3px solid #cbd5e1; color: #64748b; }
.rich-text :deep(pre) { overflow-x: auto; white-space: pre-wrap; }
@media print {
  .rich-text { overflow: visible; color: #000; }
  .rich-text :deep(table) { width: 100% !important; min-width: 0; table-layout: fixed; }
  .rich-text :deep(tr), .rich-text :deep(img) { break-inside: avoid; }
}
</style>

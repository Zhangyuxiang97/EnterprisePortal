<template>
  <nav class="detail-breadcrumb" aria-label="面包屑导航">
    <ol>
      <li><router-link to="/">首页</router-link></li>
      <li>
        <span class="separator" aria-hidden="true">/</span>
        <button v-if="embedded" type="button" @click="$emit('back')">{{ sectionLabel }}</button>
        <router-link v-else :to="sectionTo">{{ sectionLabel }}</router-link>
      </li>
      <li>
        <span class="separator" aria-hidden="true">/</span>
        <span class="current" aria-current="page">{{ currentLabel }}</span>
      </li>
    </ol>
  </nav>
</template>

<script setup>
defineProps({
  sectionLabel: { type: String, required: true },
  sectionTo: { type: String, required: true },
  currentLabel: { type: String, required: true },
  embedded: Boolean
})
defineEmits(['back'])
</script>

<style scoped>
.detail-breadcrumb { padding-inline: 2rem; color: #64748b; font-size: 13px; line-height: 1.5; }
ol { display: flex; flex-wrap: wrap; align-items: center; gap: 0 12px; margin: 0; padding: 0; list-style: none; }
li { display: flex; align-items: center; gap: 12px; white-space: nowrap; }
a, button, .current { display: inline-flex; align-items: center; min-height: 32px; }
a, button { border-radius: 3px; transition: color 150ms ease; }
a:hover, button:hover { color: #245b85; text-decoration: underline; text-underline-offset: 4px; }
a:focus-visible, button:focus-visible { outline: 2px solid #4f83b1; outline-offset: 4px; }
.separator { color: #cbd5e1; font-size: 12px; }
.current { color: #475569; font-weight: 500; }
@media print { .detail-breadcrumb { display: none; } }
</style>

<template>
  <a href="#portal-content" class="skip-link">跳转到正文</a>
  <Header />
  <main id="portal-content" tabindex="-1"><slot /></main>
  <Footer :class="{ 'border-t border-hailong-cyan/20': route.path !== '/' }" />
</template>
<script setup>
import { watchEffect } from 'vue'
import { useRoute } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import logoUrl from '@/assets/logo.png'
import { getCompanyInfo } from '@/utils/config'
import { pageMeta } from '@/composables/usePageMeta'
import { buildPageMetadata, applyPageMetadata } from '@/utils/metadata'

const route = useRoute()
watchEffect(() => {
  const detail = pageMeta.path === route.path ? pageMeta : {}
  const meta = buildPageMetadata({ origin: window.location.origin, path: route.path, company: getCompanyInfo(), title: detail.title || route.meta.title, description: detail.description || route.meta.description, noindex: route.meta.noindex || detail.unavailable })
  applyPageMetadata(document, meta, new URL(logoUrl, window.location.origin).href)
})
</script>
<style scoped>
.skip-link { position: fixed; top: -5rem; left: 1rem; z-index: 200; padding: .75rem 1rem; background: white; color: #1e293b; border-radius: .5rem; }
.skip-link:focus { top: 1rem; }
main { min-height: 70vh; outline: none; }
@media print { :deep(nav), :deep(footer), .skip-link { display: none !important; } }
</style>

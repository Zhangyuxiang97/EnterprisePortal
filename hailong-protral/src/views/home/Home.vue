<template>
  <div ref="homeElement" class="portal-home min-h-screen overflow-hidden">

    <!-- 动态渲染首页模块 -->
    <HomeModule
      v-for="module in enabledModules"
      :key="module.name"
      :component="moduleComponents[module.component]"
      :title="module.title"
      :surface="module.component === 'HeroBanner' ? 'hero' : module.component === 'ContactInfo' ? 'closing' : 'content'"
      :eager="restoringScroll || ['HeroBanner', 'AnnouncementList'].includes(module.component)"
      @show-contact="showContactModal = true"
      @announcement-click="handleAnnouncementClick"
      @business-click="handleBusinessClick"
      @qualification-click="handleQualificationClick"
      @achievement-click="handleAchievementClick"
    />

    <!-- 联系信息模态框 -->
    <ContactDialog v-if="showContactModal" @close="showContactModal = false" />
  </div>
</template>

<script setup>
import { ref, computed, defineAsyncComponent } from 'vue'
import { useRouter } from 'vue-router'
import { getSiteConfig } from '@/utils/config'
import HomeModule from '@/components/home/HomeModule.vue'
import HeroBanner from '@/components/home/modules/HeroBanner.vue'
import ContactDialog from '@/components/home/ContactDialog.vue'
import { useScrollRestoration } from '@/composables/useScrollRestoration'

const router = useRouter()
const restoringScroll = Boolean(window.history.state?.scroll)
const homeElement = ref(null)
useScrollRestoration(homeElement, window.history.state?.scroll)
const showContactModal = ref(false)
const moduleComponents = {
  HeroBanner,
  CompanyIntro: defineAsyncComponent(() => import('@/components/home/modules/CompanyIntro.vue')),
  AnnouncementList: defineAsyncComponent(() => import('@/components/home/modules/AnnouncementList.vue')),
  BusinessScope: defineAsyncComponent(() => import('@/components/home/modules/BusinessScope.vue')),
  QualificationList: defineAsyncComponent(() => import('@/components/home/modules/QualificationList.vue')),
  AchievementList: defineAsyncComponent(() => import('@/components/home/modules/AchievementList.vue')),
  DataVisualization: defineAsyncComponent(() => import('@/components/home/modules/DataVisualization.vue')),
  ContactInfo: defineAsyncComponent(() => import('@/components/home/modules/ContactInfo.vue'))
}
const enabledModules = computed(() => (getSiteConfig().homeModules || []).filter(module => module.enabled && moduleComponents[module.component]).sort((a, b) => a.order - b.order))
const handleBusinessClick = id => router.push(`/business-scope/${id}`)
const handleAchievementClick = id => router.push(`/achievement/${id}`)
const handleAnnouncementClick = id => router.push(`/announcement/${id}`)
const handleQualificationClick = id => router.push(`/qualification/${id}`)
</script>

<style scoped>
.portal-home {
  --home-surface: #f5f7fb;
  --home-ink: #1e293b;
  --home-section-space: clamp(3.5rem, 6vw, 5rem);
  background: var(--home-surface);
  isolation: isolate;
}

/* 背景由页面统一管理，模块调整顺序或隐藏时仍保持连续底色。 */
:deep(.home-module[data-home-surface="content"]) {
  position: relative;
  background: var(--home-surface);
}

:deep(.home-module[data-home-surface="hero"] + .home-module[data-home-surface="content"]) {
  z-index: 1;
  margin-top: -2.5rem;
  border-radius: 2.5rem 2.5rem 0 0;
  box-shadow: 0 -12px 40px rgb(15 23 42 / .05);
}

:deep(.home-section) {
  position: relative;
  padding-block: var(--home-section-space);
  background: transparent;
  color: var(--home-ink);
}

:deep(.home-section > .container-wide > .text-center) {
  margin-bottom: 2.5rem;
}

:deep(.home-module[data-home-surface="content"] + .home-module[data-home-surface="content"] .home-section::before) {
  content: '';
  position: absolute;
  top: 0;
  left: 50%;
  width: min(80%, 1200px);
  height: 1px;
  transform: translateX(-50%);
  background: linear-gradient(90deg, transparent, #dce3ee 20%, #dce3ee 80%, transparent);
}

:deep(.home-closing) {
  border-radius: 2rem 2rem 0 0;
  padding-block: 2.5rem;
}

@media (max-width: 640px) {
  :deep(.home-module[data-home-surface="hero"] + .home-module[data-home-surface="content"]) {
    margin-top: -1.5rem;
    border-radius: 1.5rem 1.5rem 0 0;
  }
  :deep(.home-closing) { border-radius: 1.5rem 1.5rem 0 0; }
}
</style>

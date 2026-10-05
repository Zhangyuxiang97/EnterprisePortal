<template>
  <div class="min-h-screen bg-slate-50">
    <div class="bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950 px-6 pt-32 pb-16 text-center text-white">
      <h1 class="text-4xl font-bold md:text-5xl">关于海隆</h1>
      <p class="mt-4 text-slate-300">{{ companyInfo.slogan }}</p>
    </div>
    <nav aria-label="公司信息栏目" class="sticky top-20 z-40 border-b border-slate-100 bg-white/95 backdrop-blur-md">
      <div class="container-wide flex gap-2 overflow-x-auto">
        <router-link v-for="tab in tabs" :key="tab.id" :to="{ path: '/about', query: { tab: tab.id } }" class="whitespace-nowrap border-b-2 px-5 py-4 text-sm font-medium" :class="activeTab === tab.id ? 'border-hailong-primary text-hailong-primary' : 'border-transparent text-slate-600'" :aria-current="activeTab === tab.id ? 'page' : undefined">{{ tab.name }}</router-link>
      </div>
    </nav>
    <div class="py-12"><div class="container-wide">
      <CompanyProfile v-if="activeTab === 'intro'" :profile="current.data" :loading="current.loading" :error="current.error" @retry="loadTab" @preview="previewImage = $event" />
      <CompanyCollection v-else :title="tabs.find(tab => tab.id === activeTab).name" :items="current.data" :certificate-images="['qualifications', 'honors'].includes(activeTab)" :loading="current.loading" :error="current.error" @retry="loadTab" @preview="previewImage = $event" />
    </div></div>
    <ImageDialog v-if="previewImage" :src="previewImage" @close="previewImage = ''" />
  </div>
</template>
<script setup>
import { ref, computed, reactive, watch } from 'vue'
import { useRoute } from 'vue-router'
import { getCompanyInfo } from '@/utils/config'
import { getCompanyProfile, getBusinessScope, getCompanyQualifications, getCompanyHonors, getMajorAchievements } from '@/api/config'
import { useResource } from '@/composables/useResource'
import CompanyProfile from '@/components/company/CompanyProfile.vue'
import CompanyCollection from '@/components/company/CompanyCollection.vue'
import ImageDialog from '@/components/common/ImageDialog.vue'

const route = useRoute()
const companyInfo = computed(getCompanyInfo)
const previewImage = ref('')
const tabs = [{ id: 'intro', name: '企业简介' }, { id: 'business', name: '业务范围' }, { id: 'qualifications', name: '企业资质' }, { id: 'achievements', name: '重要业绩' }, { id: 'honors', name: '企业荣誉' }]
const activeTab = computed(() => tabs.some(tab => tab.id === route.query.tab) ? route.query.tab : 'intro')
const resources = Object.fromEntries(tabs.map(tab => [tab.id, reactive(useResource(tab.id === 'intro' ? null : []))]))
const current = computed(() => resources[activeTab.value])
const loaded = new Set()
const fetchers = { intro: getCompanyProfile, business: getBusinessScope, qualifications: getCompanyQualifications, achievements: getMajorAchievements, honors: getCompanyHonors }
const detailPaths = { business: 'business-scope', qualifications: 'qualification', achievements: 'achievement' }

// 与原企业简介一致，兼容后端返回的完整 URL 和本站 /uploads 路径。
const resolveImageUrl = value => {
  if (!value) return ''
  if (/^(https?:)?\/\//i.test(value)) return value
  const base = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'
  const origin = /^https?:\/\//i.test(base) ? new URL(base).origin : ''
  return `${origin}/${value.replace(/^\/+/, '')}`
}
const loadTab = async () => {
  const tab = activeTab.value
  await resources[tab].execute(async signal => {
    const response = await fetchers[tab]({ signal })
    if (!response.success || !response.data) throw new Error('内容加载失败，请重试')
    if (tab === 'intro') {
      const data = response.data
      const image = data.imageUrls?.[0] || (data.imageIds?.[0] ? `/api/attachments/${data.imageIds[0]}/download` : '')
      return { ...data, imageUrl: resolveImageUrl(image) }
    }
    return response.data.filter(item => item.status !== false).sort((a, b) => a.sortOrder - b.sortOrder).map(item => ({
      id: item.id,
      title: item.name || item.projectName,
      description: item.description || '',
      image: item.imageUrl || item.certificateImageUrl || item.imageUrls?.[0] || '',
      features: item.features || [item.projectType, item.clientName, item.projectAmount ? `${Number(item.projectAmount).toLocaleString()} 万元` : ''].filter(Boolean),
      to: detailPaths[tab] ? { path: `/${detailPaths[tab]}/${item.id}`, query: { from: 'about', tab } } : undefined
    }))
  })
  if (!resources[tab].error) loaded.add(tab)
}
watch(activeTab, () => { previewImage.value = ''; if (!loaded.has(activeTab.value)) loadTab() }, { immediate: true })
</script>

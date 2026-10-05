<template>
  <footer class="bg-hailong-dark text-white py-12">
    <div class="container-wide">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-8 mb-8">
        <div>
          <h3 class="text-xl font-bold mb-4 text-hailong-cyan font-tech">{{ companyInfo.fullName }}</h3>
          <p class="text-sm text-gray-400">{{ companyInfo.slogan }}</p>
        </div>
        <div v-for="section in footerSections" :key="section.title">
          <h4 class="font-semibold mb-4">{{ section.title }}</h4>
          <ul v-if="section.type !== 'contact'" class="space-y-2 text-sm text-gray-400">
            <li v-for="link in section.links" :key="link.name">
              <router-link :to="link.path" class="hover:text-hailong-cyan transition-colors">
                {{ link.name }}
              </router-link>
            </li>
          </ul>
          <ul v-else class="space-y-2 text-sm text-gray-400">
            <li v-for="item in footerContactItems" :key="item.label">
              {{ item.label }}: {{ item.value }}
            </li>
          </ul>
        </div>
      </div>
      <div class="border-t border-gray-800 pt-8 text-center text-gray-400 text-sm">
        <p>{{ footerCopyright }}</p>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { computed } from 'vue'
import { getCompanyInfo, getContactInfo, getNavigation } from '@/utils/config'

// 获取公司信息
const companyInfo = computed(() => getCompanyInfo())

// 获取页脚配置
const navigation = computed(() => getNavigation())
const footerSections = computed(() => navigation.value.footer.sections)
const footerContactItems = computed(() => {
  const contact = getContactInfo()
  return [
    { label: '电话', value: contact.phone },
    { label: '邮箱', value: contact.email },
    { label: '地址', value: contact.address.fullAddress }
  ]
})
const footerCopyright = computed(() => {
  const currentYear = new Date().getFullYear()
  return `© ${currentYear} ${companyInfo.value.fullName} 版权所有`
})
</script>

<style scoped>
/* 组件特定样式 */
</style>

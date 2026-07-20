<template>
  <nav class="fixed top-0 left-0 right-0 z-50 bg-hailong-dark/90 backdrop-blur-md text-white shadow-2xl border-b border-white/10">
    <div class="container-wide">
      <div class="flex items-center justify-between h-20">
        <div class="flex items-center space-x-3">
          <img :src="logoUrl" :alt="companyInfo.fullName" class="h-12 w-auto" />
          <div class="text-3xl font-bold font-tech bg-gradient-to-r from-hailong-primary to-hailong-secondary bg-clip-text text-transparent">
            {{ companyInfo.fullName }}
          </div>
        </div>
        <div class="hidden md:flex items-center space-x-8">
          <template v-for="link in navLinks" :key="link.name">
            <!-- 带下拉菜单的导航项 -->
            <div v-if="link.children" class="relative group">
              <button
                class="hover:text-hailong-cyan transition-colors text-sm font-medium flex items-center gap-1"
                :class="{ 'text-hailong-cyan': isActiveParent(link) }"
              >
                {{ link.name }}
                <svg class="w-4 h-4 transition-transform group-hover:rotate-180" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </button>
              <!-- 下拉菜单 -->
              <div class="absolute top-full left-0 mt-2 w-48 bg-white rounded-lg shadow-xl opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 overflow-hidden">
                <router-link
                  v-for="child in link.children"
                  :key="child.name"
                  :to="child.path"
                  class="block px-4 py-3 text-gray-700 hover:bg-hailong-primary hover:text-white transition-colors text-sm"
                  :class="{ 'bg-hailong-primary/10 text-hailong-primary': isActive(child.path) }"
                >
                  {{ child.name }}
                </router-link>
              </div>
            </div>
            <!-- 普通导航项 -->
            <router-link v-else :to="link.path"
              class="hover:text-hailong-cyan transition-colors text-sm font-medium"
              :class="{ 'text-hailong-cyan': isActive(link.path) }">
              {{ link.name }}
            </router-link>
          </template>
        </div>
        <!-- 移动端菜单按钮 -->
        <button @click="toggleMobileMenu" class="md:hidden text-white">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path v-if="!showMobileMenu" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
            <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
    </div>
    <!-- 移动端菜单 -->
    <div v-if="showMobileMenu" class="md:hidden bg-hailong-dark/95 border-t border-white/10">
      <div class="container-wide py-4">
        <template v-for="link in navLinks" :key="link.name">
          <!-- 带子菜单的导航项 -->
          <div v-if="link.children">
            <button
              @click="toggleMobileSubmenu(link.name)"
              class="w-full flex items-center justify-between py-3 px-4 hover:bg-hailong-primary/20 rounded-lg transition-colors"
              :class="{ 'bg-hailong-primary/30 text-hailong-cyan': isActiveParent(link) }"
            >
              <span>{{ link.name }}</span>
              <svg
                class="w-4 h-4 transition-transform"
                :class="{ 'rotate-180': openMobileSubmenu === link.name }"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            <!-- 子菜单 -->
            <div v-if="openMobileSubmenu === link.name" class="ml-4 mt-1 space-y-1">
              <router-link
                v-for="child in link.children"
                :key="child.name"
                :to="child.path"
                @click="showMobileMenu = false"
                class="block py-2 px-4 hover:bg-hailong-primary/20 rounded-lg transition-colors text-sm"
                :class="{ 'bg-hailong-primary/30 text-hailong-cyan': isActive(child.path) }"
              >
                {{ child.name }}
              </router-link>
            </div>
          </div>
          <!-- 普通导航项 -->
          <router-link v-else :to="link.path"
            @click="showMobileMenu = false"
            class="block py-3 px-4 hover:bg-hailong-primary/20 rounded-lg transition-colors"
            :class="{ 'bg-hailong-primary/30 text-hailong-cyan': isActive(link.path) }">
            {{ link.name }}
          </router-link>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import logoUrl from '@/assets/logo.png'
import { getCompanyInfo, getNavigation } from '@/utils/config'

const route = useRoute()
const showMobileMenu = ref(false)
const openMobileSubmenu = ref(null)

// 获取公司信息和导航配置
const companyInfo = computed(() => getCompanyInfo())
const navigation = computed(() => getNavigation())
const navLinks = computed(() => navigation.value.header)

const isActive = (path) => {
  return route.path === path || route.path.startsWith(path + '/')
}

const isActiveParent = (link) => {
  if (link.children) {
    return link.children.some(child => isActive(child.path))
  }
  return isActive(link.path)
}

const toggleMobileMenu = () => {
  showMobileMenu.value = !showMobileMenu.value
  openMobileSubmenu.value = null
}

const toggleMobileSubmenu = (name) => {
  openMobileSubmenu.value = openMobileSubmenu.value === name ? null : name
}
</script>

<style scoped>
/* 组件特定样式 */
</style>
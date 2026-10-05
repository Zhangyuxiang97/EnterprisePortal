<template>
  <header class="fixed inset-x-0 top-0 z-50 border-b border-white/10 bg-hailong-dark/95 text-white backdrop-blur-md shadow-lg">
    <nav ref="navigationElement" aria-label="主导航" class="container-wide" @keydown.esc="closeMenus">
      <div class="flex h-20 items-center justify-between gap-4">
        <router-link to="/" class="flex min-w-0 items-center gap-2 sm:gap-3" aria-label="返回首页">
          <img :src="logoUrl" alt="" class="h-10 sm:h-12 w-auto shrink-0" />
          <span class="text-base sm:text-xl xl:text-2xl font-bold tracking-tight">{{ companyInfo.fullName }}</span>
        </router-link>
        <div class="hidden lg:flex items-center gap-6 xl:gap-8 shrink-0">
          <template v-for="(link, index) in navLinks" :key="link.name">
            <div v-if="link.children" class="relative" @mouseleave="desktopMenu = null" @focusout="closeOnFocusOut">
              <button type="button" @mouseenter="desktopMenu = link.name" @click="desktopMenu = link.name" :aria-expanded="desktopMenu === link.name" :aria-controls="`desktop-submenu-${index}`" class="nav-link flex items-center gap-1 py-5" :class="{ active: isActiveParent(link) }">
                {{ link.name }} <span aria-hidden="true" class="text-xs">⌄</span>
              </button>
              <div v-show="desktopMenu === link.name" :id="`desktop-submenu-${index}`" class="absolute left-0 top-full w-44 overflow-hidden rounded-xl border border-slate-100 bg-white py-2 text-slate-700 shadow-xl">
                <router-link v-for="child in link.children" :key="child.path" :to="child.path" class="block px-5 py-3 text-sm hover:bg-slate-50 focus:bg-slate-50" :class="{ 'text-hailong-primary font-bold': isActive(child.path) }" :aria-current="isActive(child.path) ? 'page' : undefined">{{ child.name }}</router-link>
              </div>
            </div>
            <router-link v-else :to="link.path" class="nav-link" :class="{ active: isActive(link.path) }" :aria-current="isActive(link.path) ? 'page' : undefined">{{ link.name }}</router-link>
          </template>
        </div>
        <button type="button" @click="showMobileMenu = !showMobileMenu" class="lg:hidden shrink-0 rounded-lg p-2 hover:bg-white/10" :aria-expanded="showMobileMenu" aria-controls="mobile-navigation" :aria-label="showMobileMenu ? '关闭导航菜单' : '打开导航菜单'">
          <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true"><path stroke-linecap="round" stroke-width="2" :d="showMobileMenu ? 'M6 18L18 6M6 6l12 12' : 'M4 6h16M4 12h16M4 18h16'" /></svg>
        </button>
      </div>
      <div v-if="showMobileMenu" id="mobile-navigation" class="lg:hidden max-h-[calc(100dvh-5rem)] overflow-y-auto border-t border-white/10 py-3">
        <template v-for="(link, index) in navLinks" :key="link.name">
          <div v-if="link.children">
            <button type="button" @click="mobileMenu = mobileMenu === link.name ? null : link.name" :aria-expanded="mobileMenu === link.name" :aria-controls="`mobile-submenu-${index}`" class="flex w-full items-center justify-between rounded-lg px-4 py-3" :class="{ active: isActiveParent(link) }">{{ link.name }} <span aria-hidden="true">⌄</span></button>
            <div v-show="mobileMenu === link.name" :id="`mobile-submenu-${index}`" class="ml-4 border-l border-white/20">
              <router-link v-for="child in link.children" :key="child.path" :to="child.path" class="block rounded-lg px-4 py-3 text-sm" :class="{ active: isActive(child.path) }">{{ child.name }}</router-link>
            </div>
          </div>
          <router-link v-else :to="link.path" class="block rounded-lg px-4 py-3" :class="{ active: isActive(link.path) }">{{ link.name }}</router-link>
        </template>
      </div>
    </nav>
  </header>
</template>
<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import logoUrl from '@/assets/logo.png'
import { getCompanyInfo, getNavigation } from '@/utils/config'
const route = useRoute()
const navigationElement = ref(null)
const showMobileMenu = ref(false)
const desktopMenu = ref(null)
const mobileMenu = ref(null)
const companyInfo = computed(getCompanyInfo)
const navLinks = computed(() => getNavigation().header)
const isActive = path => route.meta.navPath === path || route.path === path || (path !== '/' && route.path.startsWith(`${path}/`))
const isActiveParent = link => link.children.some(child => isActive(child.path))
const closeMenus = () => { desktopMenu.value = null; mobileMenu.value = null; showMobileMenu.value = false }
const closeOnFocusOut = event => { if (!event.currentTarget.contains(event.relatedTarget)) desktopMenu.value = null }
const closeOutside = event => { if (!navigationElement.value?.contains(event.target)) closeMenus() }
watch(() => route.fullPath, closeMenus)
onMounted(() => document.addEventListener('click', closeOutside))
onUnmounted(() => document.removeEventListener('click', closeOutside))
</script>
<style scoped>
.nav-link { @apply text-sm font-medium transition-colors hover:text-hailong-cyan; }
.active { @apply text-hailong-cyan; }
@media print { header { display: none !important; } }
</style>

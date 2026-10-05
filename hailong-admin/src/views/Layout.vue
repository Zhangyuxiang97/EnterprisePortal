<template>
  <el-container class="layout-container">
    <!-- 侧边栏 -->
    <el-aside :width="isCollapse ? '64px' : '200px'" class="layout-aside">
      <Sidebar :isCollapse="isCollapse" />
    </el-aside>

    <!-- 主内容区 -->
    <el-container>
      <!-- 顶部导航栏 -->
      <el-header class="layout-header">
        <Header @toggle-collapse="toggleCollapse" :isCollapse="isCollapse" />
      </el-header>

      <!-- 内容区域 -->
      <el-main class="layout-main">
        <el-alert v-if="!session.token" type="warning" :closable="false" show-icon style="margin-bottom: 16px">
          登录已过期，当前编辑内容已保留。<a href="/login" target="_blank" rel="noopener">在新窗口重新登录</a>后返回此页继续保存。
        </el-alert>
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref } from 'vue'
import { session } from '@/utils/session'
import Sidebar from '@/components/Sidebar.vue'
import Header from '@/components/Header.vue'

// 侧边栏折叠状态
const isCollapse = ref(false)

/**
 * 切换侧边栏折叠状态
 */
const toggleCollapse = () => {
  isCollapse.value = !isCollapse.value
}
</script>

<style scoped>
.layout-container {
  height: 100vh;
  overflow: hidden;
}

.layout-aside {
  background-color: #304156;
  transition: width 0.3s;
  overflow-x: hidden;
}

.layout-header {
  background-color: #fff;
  border-bottom: 1px solid #e6e6e6;
  padding: 0 20px;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
}

.layout-main {
  background-color: #f0f2f5;
  padding: 20px;
  overflow-y: auto;
}

/* 路由切换动画 */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

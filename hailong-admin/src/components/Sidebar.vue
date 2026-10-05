<template>
  <div class="sidebar-container">
    <!-- Logo区域 -->
    <div class="logo-container">
      <img v-if="!isCollapse" src="@/assets/logo.png" alt="海隆咨询" class="logo-img" />
      <img v-else src="@/assets/hailong.ico" alt="HL" class="logo-img-mini" />
      <h2 v-if="!isCollapse" class="logo-title">海隆咨询</h2>
    </div>

    <!-- 菜单 -->
    <el-menu
      :default-active="activeMenu"
      :collapse="isCollapse"
      :unique-opened="true"
      background-color="#304156"
      text-color="#bfcbd9"
      active-text-color="#409eff"
      router
    >
      <el-menu-item index="/dashboard">
        <el-icon><Odometer /></el-icon>
        <template #title>数据看板</template>
      </el-menu-item>

      <el-sub-menu index="announcements">
        <template #title>
          <el-icon><Document /></el-icon>
          <span>公告管理</span>
        </template>
        <el-menu-item index="/gov-procurement">政府采购公告</el-menu-item>
        <el-menu-item index="/construction">建设工程公告</el-menu-item>
      </el-sub-menu>

      <el-sub-menu index="info-publish">
        <template #title>
          <el-icon><DocumentCopy /></el-icon>
          <span>信息发布</span>
        </template>
        <el-menu-item index="/info-publish/news-center">新闻中心</el-menu-item>
        <el-menu-item index="/info-publish/policy-regulation">政策法规</el-menu-item>
      </el-sub-menu>

      <!-- 仅管理员可见 -->
      <el-menu-item v-if="isAdmin" index="/attachments">
        <el-icon><Paperclip /></el-icon>
        <template #title>附件管理</template>
      </el-menu-item>

      <!-- 仅管理员可见 -->
      <el-menu-item v-if="isAdmin" index="/config/site-settings">
        <el-icon><Setting /></el-icon><template #title>门户设置</template>
      </el-menu-item>
      <el-sub-menu v-if="isAdmin" index="config">
        <template #title>
          <el-icon><Setting /></el-icon>
          <span>企业资料</span>
        </template>
        <el-menu-item index="/config/company-profile">企业简介</el-menu-item>
        <el-menu-item index="/config/business-scope">业务范围</el-menu-item>
        <el-menu-item index="/config/qualifications">企业资质</el-menu-item>
        <el-menu-item index="/config/achievements">重要业绩</el-menu-item>
        <el-menu-item index="/config/honors">企业荣誉</el-menu-item>
        <!-- <el-menu-item index="/config/banners">轮播图管理</el-menu-item> -->
        <!-- <el-menu-item index="/config/friendly-links">友情链接</el-menu-item> -->
      </el-sub-menu>

      <!-- 仅管理员可见 -->
      <el-sub-menu v-if="isAdmin" index="statistics">
        <template #title>
          <el-icon><DataAnalysis /></el-icon>
          <span>统计分析</span>
        </template>
        <el-menu-item index="/statistics/visit">访问统计</el-menu-item>
        <el-menu-item index="/statistics/announcement">公告统计</el-menu-item>
        <el-menu-item index="/statistics/info-publication">信息发布统计</el-menu-item>
      </el-sub-menu>

      <el-sub-menu index="system">
        <template #title>
          <el-icon><Tools /></el-icon>
          <span>系统管理</span>
        </template>
        <!-- 仅管理员可见 -->
        <el-menu-item v-if="isAdmin" index="/system/users">用户管理</el-menu-item>
        <el-menu-item v-if="isAdmin" index="/system/logs">系统日志</el-menu-item>
        <el-menu-item v-if="isAdmin" index="/system/regions">区域字典</el-menu-item>
        <!-- 所有用户可见 -->
        <el-menu-item index="/system/change-password">修改密码</el-menu-item>
      </el-sub-menu>
    </el-menu>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { Odometer, Document, DocumentCopy, Paperclip, Setting, DataAnalysis, Tools } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'

defineProps({
  isCollapse: {
    type: Boolean,
    default: false
  }
})

const route = useRoute()
const userStore = useUserStore()

// 当前激活的菜单
const activeMenu = computed(() => {
  return route.path
})

// 获取用户角色
const userRole = computed(() => {
  return userStore.userInfo?.role || 'user'
})

// 判断是否为管理员
const isAdmin = computed(() => {
  return userRole.value === 'admin'
})
</script>

<style scoped>
.sidebar-container {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.logo-container {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  background-color: #2b3a4b;
  padding: 0 15px;
}

.logo-img {
  height: 40px;
  width: auto;
  object-fit: contain;
}

.logo-img-mini {
  height: 32px;
  width: 32px;
  object-fit: contain;
}

.logo-title {
  color: #fff;
  font-size: 18px;
  font-weight: bold;
  margin: 0;
  white-space: nowrap;
}

.el-menu {
  border-right: none;
  flex: 1;
}

:deep(.el-sub-menu__title) {
  color: #bfcbd9;
}

:deep(.el-sub-menu__title:hover) {
  background-color: #263445 !important;
}

:deep(.el-menu-item:hover) {
  background-color: #263445 !important;
}

:deep(.el-menu-item.is-active) {
  background-color: #001528 !important;
}
</style>

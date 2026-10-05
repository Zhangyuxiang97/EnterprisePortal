<template>
  <div class="header-container">
    <div class="header-left">
      <el-icon class="collapse-icon" :size="20" @click="handleToggleCollapse">
        <component :is="isCollapse ? 'Expand' : 'Fold'" />
      </el-icon>

      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/home' }">
          <img src="@/assets/hailong.ico" alt="首页" class="breadcrumb-icon" />
          首页
        </el-breadcrumb-item>
        <el-breadcrumb-item v-if="breadcrumbs.length > 0">
          {{ breadcrumbs[breadcrumbs.length - 1] }}
        </el-breadcrumb-item>
      </el-breadcrumb>
    </div>

    <div class="header-right">
      <el-dropdown @command="handleCommand">
        <span class="user-info">
          <el-icon><User /></el-icon>
          <span class="username">{{ userStore.userInfo?.fullName || '用户' }}</span>
          <el-icon class="el-icon--right"><ArrowDown /></el-icon>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item disabled>
              {{ userStore.userInfo?.email }}
            </el-dropdown-item>
            <el-dropdown-item divided command="changePassword">
              <el-icon><Lock /></el-icon>
              修改密码
            </el-dropdown-item>
            <el-dropdown-item divided command="logout">
              <el-icon><SwitchButton /></el-icon>
              退出登录
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup>
import { confirmEditorsLeave, resetEditorLeaveApprovals } from '@/utils/editorLeave'
import { notifyError } from '@/utils/errors'
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessageBox, ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import {
  Fold,
  Expand,
  User,
  ArrowDown,
  Lock,
  SwitchButton
} from '@element-plus/icons-vue'

const props = defineProps({
  isCollapse: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['toggle-collapse'])
const router = useRouter()
const route = useRoute()
const userStore = useUserStore()

/**
 * 面包屑导航
 */
const breadcrumbs = computed(() => {
  const matched = route.matched
  const crumbs = []

  matched.forEach(item => {
    if (item.meta?.title && item.path !== '/') {
      crumbs.push(item.meta.title)
    }
  })

  return crumbs
})

/**
 * 切换侧边栏折叠
 */
const handleToggleCollapse = () => {
  emit('toggle-collapse')
}

/**
 * 处理下拉菜单命令
 */
const handleCommand = async (command) => {
  switch (command) {
    case 'changePassword':
      router.push('/system/change-password')
      break

    case 'logout':
      try {
        await ElMessageBox.confirm('确定要退出登录吗？', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        })

        if (!await confirmEditorsLeave()) break
        await userStore.logout()
        ElMessage.success('已退出登录')
        await router.push('/login')
      } catch (error) {
        notifyError(error, '退出失败，请重试')
      } finally {
        resetEditorLeaveApprovals()
      }
      break
  }
}
</script>

<style scoped>
.header-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 100%;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 20px;
}

.breadcrumb-icon {
  width: 16px;
  height: 16px;
  margin-right: 4px;
  vertical-align: middle;
}

.collapse-icon {
  cursor: pointer;
  transition: color 0.3s;
}

.collapse-icon:hover {
  color: #409eff;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.user-info:hover {
  background-color: #f5f7fa;
}

.username {
  font-size: 14px;
  color: #333;
}
</style>

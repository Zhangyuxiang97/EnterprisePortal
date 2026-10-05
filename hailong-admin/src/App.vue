<template>
  <el-config-provider :locale="zhCn"><router-view /></el-config-provider>
</template>

<script setup>
import { ElConfigProvider } from 'element-plus'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
</script>

<style>
/* 全局样式已在 style.css 中定义 */
</style>

<template>
  <el-cascader
    v-model="selectedRegion"
    :options="regionOptions"
    :props="cascaderProps"
    :placeholder="placeholder"
    :disabled="disabled"
    :clearable="clearable"
    :filterable="filterable"
    :show-all-levels="showAllLevels"
    @change="handleChange"
    :style="{ width: width }"
  />
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { ref, watch, onMounted, onBeforeUnmount } from 'vue'
import { systemApi } from '@/api'
import { ElMessage } from 'element-plus'

// 全局缓存区域数据，避免重复加载
const regionDataCache = {
  data: null,
  loading: false,
  loadPromise: null
}

const props = defineProps({
  // 初始值（区域代码数组，如：['410000', '410100', '410122']）
  modelValue: {
    type: Array,
    default: () => []
  },
  // 占位符
  placeholder: {
    type: String,
    default: '请选择省/市/区'
  },
  // 是否禁用
  disabled: {
    type: Boolean,
    default: false
  },
  // 是否可清空
  clearable: {
    type: Boolean,
    default: true
  },
  // 是否可搜索
  filterable: {
    type: Boolean,
    default: true
  },
  // 是否显示完整路径
  showAllLevels: {
    type: Boolean,
    default: true
  },
  // 宽度
  width: {
    type: String,
    default: '100%'
  },
  // 最大级别（1=省，2=市，3=区）
  maxLevel: {
    type: Number,
    default: 3
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedRegion = ref([])
const regionOptions = ref([])
const isInitializing = ref(false)

// 级联选择器配置 - 不使用懒加载，改为一次性加载所有数据
const cascaderProps = {
  value: 'code',
  label: 'name',
  children: 'children',
  checkStrictly: true,
  emitPath: true
}

// 构建树形结构的区域数据
const buildRegionTree = async () => {
  try {
    // 使用 tree 接口一次性获取所有区域数据
    const treeRes = await systemApi.regions.getTree()
    if (!treeRes.success || !treeRes.data) {
      return []
    }

    const tree = treeRes.data

    // 根据 maxLevel 过滤数据
    const filterTreeByLevel = (nodes, currentLevel = 1) => {
      return nodes.map(node => {
        const filtered = {
          code: node.code,
          name: node.name
        }

        // 如果当前级别小于最大级别，且有子节点，则递归处理
        if (currentLevel < props.maxLevel && node.children && node.children.length > 0) {
          filtered.children = filterTreeByLevel(node.children, currentLevel + 1)
        }

        return filtered
      })
    }

    return filterTreeByLevel(tree)
  } catch (error) {

    notifyError(error, '加载区域数据失败')
    return []
  }
}

// 初始化区域数据（使用缓存）
const initRegionData = async () => {
  // 如果缓存中已有数据，直接使用
  if (regionDataCache.data) {
    regionOptions.value = regionDataCache.data
    isInitializing.value = false
    return
  }

  // 如果正在加载，等待加载完成
  if (regionDataCache.loading && regionDataCache.loadPromise) {
    isInitializing.value = true
    await regionDataCache.loadPromise
    regionOptions.value = regionDataCache.data || []
    isInitializing.value = false
    return
  }

  // 开始加载
  isInitializing.value = true
  regionDataCache.loading = true

  regionDataCache.loadPromise = buildRegionTree()
  const data = await regionDataCache.loadPromise

  regionDataCache.data = data
  regionDataCache.loading = false
  regionOptions.value = data
  isInitializing.value = false
}

// 从树形数据中查找区域名称
const findRegionName = (code, tree) => {
  for (const node of tree) {
    if (node.code === code) {
      return node.name
    }
    if (node.children) {
      const found = findRegionName(code, node.children)
      if (found) return found
    }
  }
  return ''
}

// 从树形数据中根据名称查找区域代码
const findRegionCodeByName = (name, tree) => {
  for (const node of tree) {
    if (node.name === name) {
      return node.code
    }
    if (node.children) {
      const found = findRegionCodeByName(name, node.children)
      if (found) return found
    }
  }
  return ''
}

// 处理选择变化
const handleChange = (value) => {
  emit('update:modelValue', value)

  // 获取选中的区域信息
  if (value && value.length > 0) {
    const regionInfo = {
      provinceCode: value[0] || '',
      provinceName: findRegionName(value[0], regionOptions.value),
      cityCode: value[1] || '',
      cityName: value[1] ? findRegionName(value[1], regionOptions.value) : '',
      districtCode: value[2] || '',
      districtName: value[2] ? findRegionName(value[2], regionOptions.value) : '',
      fullPath: value
    }

    emit('change', regionInfo)
  } else {
    // 清空选择
    emit('change', {
      provinceCode: '',
      provinceName: '',
      cityCode: '',
      cityName: '',
      districtCode: '',
      districtName: '',
      fullPath: []
    })
  }
}

// 监听外部值变化
watch(() => props.modelValue, (newVal) => {
  if (JSON.stringify(newVal) !== JSON.stringify(selectedRegion.value)) {
    selectedRegion.value = newVal || []
  }
}, { immediate: true })

// 暴露方法
defineExpose({
  reset: () => {
    selectedRegion.value = []
  },
  getValue: () => selectedRegion.value,
  findCodeByName: (name) => findRegionCodeByName(name, regionOptions.value),
  isDataLoaded: () => regionOptions.value.length > 0,
  resolvePath: async (values) => {
    await initRegionData()
    let nodes = regionOptions.value
    const result = []
    for (const value of values) {
      if (!value) break
      const match = nodes.find(node => node.code === value || node.name === value)
      if (!match) throw new Error('历史地区未匹配到字典，请重新选择已确认的地区')
      result.push(match.code)
      nodes = match.children || []
    }
    return result
  }
})

onMounted(async () => {
  await initRegionData()

  // 设置初始值
  if (props.modelValue && props.modelValue.length > 0) {
    selectedRegion.value = [...props.modelValue]
  }
})
</script>

<style scoped>
/* 可以添加自定义样式 */
</style>

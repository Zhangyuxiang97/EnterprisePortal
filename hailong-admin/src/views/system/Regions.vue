<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>地区字典</span>
          <el-button v-if="canManage" type="primary" icon="Plus" @click="handleAdd">新增地区</el-button>
        </div>
      </template>

      <!-- 搜索表单 -->
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="地区名称">
          <el-input v-model="searchForm.name" placeholder="请输入地区名称" clearable />
        </el-form-item>
        <el-form-item label="地区代码">
          <el-input v-model="searchForm.code" placeholder="请输入地区代码" clearable />
        </el-form-item>
        <el-form-item label="级别">
          <el-select v-model="searchForm.level" placeholder="请选择级别" clearable>
            <el-option label="省级" :value="1" />
            <el-option label="市级" :value="2" />
            <el-option label="区县级" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">搜索</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 数据表格 -->
      <el-table
        :key="treeVersion"
        :default-expand-all="isFiltering"
        :data="tableData"
        v-loading="loading"
        border
        stripe
        row-key="id"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      >
        <el-table-column prop="name" label="地区名称" min-width="200" />
        <el-table-column prop="code" label="地区代码" width="120" />
        <el-table-column label="级别" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getLevelTag(row.level)" size="small">
              {{ getLevelText(row.level) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="sortOrder" label="排序" width="80" align="center" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button v-if="canManage" type="primary" size="small" link @click="handleEdit(row)">编辑</el-button>
            <el-button
              type="success"
              size="small"
              link
              @click="handleAddChild(row)"
              v-if="row.level < 3"
            >添加下级</el-button>
            <el-button v-if="canManage" type="danger" size="small" link :disabled="hasChildren(row.code)" :title="hasChildren(row.code) ? '请先处理下级地区' : '删除地区'" @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      class="editor-dialog"
      :before-close="beforeEditorClose"
      :title="dialogTitle"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="formData" :rules="rules" ref="formRef" label-width="100px">
        <el-form-item label="上级地区" v-if="formData.parentCode">
          <el-input :model-value="formData.parentName" disabled />
        </el-form-item>

        <el-form-item label="地区名称" prop="name">
          <el-input v-model="formData.name" placeholder="请输入地区名称" maxlength="100" />
        </el-form-item>

        <el-form-item label="地区代码" prop="code">
          <el-input v-model="formData.code" :disabled="!!formData.id" placeholder="请输入地区代码" maxlength="20" />
          <div class="form-tip">行政区划代码，如：110000（北京市）</div>
        </el-form-item>

        <el-form-item label="级别" prop="level">
          <el-select v-model="formData.level" placeholder="请选择级别" style="width: 100%" :disabled="true">
            <el-option label="省级" :value="1" />
            <el-option label="市级" :value="2" />
            <el-option label="区县级" :value="3" />
          </el-select>
        </el-form-item>

        <el-form-item label="排序" prop="sortOrder">
          <el-input-number v-model="formData.sortOrder" :min="0" :max="9999" />
          <span class="form-tip" style="margin-left: 10px;">数字越小越靠前</span>
        </el-form-item>


      </el-form>

      <template #footer>
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitting" :disabled="activeUploads > 0">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { useLatestRequest } from '@/composables/useLatestRequest'
const loadDataRequest = useLatestRequest()

import { notifyError } from '@/utils/errors'
import { useEditorForm } from '@/composables/useEditorForm'
import { filterRegionTree } from '@/utils/form'
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemApi } from '@/api'

const allRegions = ref([])
const appliedFilters = ref({})
const treeVersion = ref(0)
const isFiltering = computed(() => Object.values(appliedFilters.value).some(Boolean))
const tableData = computed(() => filterRegionTree(allRegions.value, appliedFilters.value))
const flatten = nodes => nodes.flatMap(node => [node, ...flatten(node.children || [])])
const regionIndex = computed(() => new Map(flatten(allRegions.value).map(node => [node.code, node])))
const findRegionName = code => regionIndex.value.get(code)?.name || code || ''
const hasChildren = code => !!regionIndex.value.get(code)?.children?.length
const loading = ref(false)
const dialogVisible = ref(false)
const dialogTitle = ref('')
const submitting = ref(false)
const formRef = ref(null)

const searchForm = reactive({
  name: '',
  code: '',
  level: null
})

const formData = reactive({
  id: null,
  parentCode: null,
  parentName: '',
  name: '',
  code: '',
  level: 1,
  sortOrder: 0
})
const { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads } = useEditorForm(formData, dialogVisible, submitting)


const rules = {
  name: [
    { required: true, message: '请输入地区名称', trigger: 'blur' },
    { min: 2, max: 100, message: '地区名称长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '请输入地区代码', trigger: 'blur' },
    { pattern: /^\d{6}$/, message: '地区代码必须是6位数字', trigger: 'blur' }
  ],
  level: [
    { required: true, message: '请选择级别', trigger: 'change' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序', trigger: 'blur' }
  ]
}

const getLevelText = (level) => {
  const levelMap = {
    1: '省级',
    2: '市级',
    3: '区县级'
  }
  return levelMap[level] || '-'
}

const getLevelTag = (level) => {
  const tagMap = {
    1: 'danger',
    2: 'warning',
    3: 'primary'
  }
  return tagMap[level] || ''
}

const loadData = async () => {
  const requestId = loadDataRequest.begin()
  loading.value = true
  try {
    const res = await systemApi.regions.getTree()
    if (!loadDataRequest.isCurrent(requestId)) return
    if (res.success) {
      allRegions.value = res.data || []
    } else {
      ElMessage.error(res.message || '加载数据失败')
    }
  } catch (error) {
    if (!loadDataRequest.isCurrent(requestId)) return

    notifyError(error, '加载数据失败，请稍后重试')
  } finally {
    if (loadDataRequest.isCurrent(requestId)) {
    loading.value = false

    }
  }
}

const handleSearch = () => {
  appliedFilters.value = { ...searchForm }
  treeVersion.value++
}

const handleReset = () => {
  Object.assign(searchForm, {
    name: '',
    code: '',
    level: null
  })
  handleSearch()
}

const handleAdd = () => {
  dialogTitle.value = '新增地区'
  Object.assign(formData, {
    id: null,
    parentCode: null,
    parentName: '',
    name: '',
    code: '',
    level: 1,
    sortOrder: 0
  })
  dialogVisible.value = true
}

const handleAddChild = (row) => {
  dialogTitle.value = '新增下级地区'
  Object.assign(formData, {
    id: null,
    parentCode: row.code,
    parentName: row.name,
    name: '',
    code: '',
    level: row.level + 1,
    sortOrder: 0
  })
  dialogVisible.value = true
}

const handleEdit = (row) => {
  dialogTitle.value = '编辑地区'
  Object.assign(formData, {
    id: row.id,
    parentCode: row.parentCode,
    parentName: findRegionName(row.parentCode),
    name: row.name,
    code: row.code,
    level: row.level,
    sortOrder: row.sortOrder
  })
  dialogVisible.value = true
}

const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    submitting.value = true
    try {
      const res = formData.id
        ? await systemApi.regions.update(formData.id, { regionName: formData.name.trim(), sortOrder: formData.sortOrder })
        : await systemApi.regions.create({ regionName: formData.name.trim(), regionCode: formData.code, regionLevel: formData.level, parentCode: formData.parentCode || null, sortOrder: formData.sortOrder })

      if (res.success) {
        ElMessage.success(formData.id ? '更新成功' : '创建成功')
        markSaved()
        dialogVisible.value = false
        loadData()
      } else {
        ElMessage.error(res.message || '操作失败')
      }
    } catch (error) {

      notifyError(error, '操作失败，请稍后重试')
    } finally {
      submitting.value = false
    }
  })
}

const handleDelete = async (row) => {
  if (hasChildren(row.code)) return ElMessage.warning('该地区有下级地区，请先处理下级')
  try {
    await ElMessageBox.confirm(
      '确定要删除该地区吗？有下级地区时须先处理下级。',
      '提示',
      {
        type: 'warning'
      }
    )

    const res = await systemApi.regions.delete(row.id)
    if (res.success) {
      ElMessage.success('删除成功')
      loadData()
    } else {
      ElMessage.error(res.message || '删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {

      notifyError(error, '删除失败，请稍后重试')
    }
  }
}

const handleDialogClose = () => {
  formRef.value?.resetFields()
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.page-container {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-form {
  margin-bottom: 20px;
}

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}
</style>

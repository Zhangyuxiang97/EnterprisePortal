<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>友情链接管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增链接</el-button>
        </div>
      </template>

      <!-- 表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column type="index" label="序号" width="60" align="center" />
        <el-table-column label="Logo" width="100" align="center">
          <template #default="{ row }">
            <el-image
              v-if="row.logoId"
              :src="`/api/attachments/${row.logoId}/download`"
              :preview-src-list="[`/api/attachments/${row.logoId}/download`]"
              fit="contain"
              style="width: 60px; height: 40px; border-radius: 4px;"
            />
            <span v-else style="color: #909399;">-</span>
          </template>
        </el-table-column>
        <el-table-column prop="name" label="链接名称" min-width="200" show-overflow-tooltip />
        <el-table-column prop="url" label="链接地址" min-width="300" show-overflow-tooltip />
        <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="sortOrder" label="排序" width="100" align="center" />
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'danger'" size="small">
              {{ row.status ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="180" align="center">
          <template #default="{ row }">
            {{ formatDateTime(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right" align="center">
          <template #default="{ row }">
            <el-button type="primary" size="small" link @click="handleEdit(row)">编辑</el-button>
            <el-button type="success" size="small" link @click="handleSort(row, 'up')" :disabled="row.sortOrder === 1">上移</el-button>
            <el-button type="success" size="small" link @click="handleSort(row, 'down')">下移</el-button>
            <el-button type="danger" size="small" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="isEdit ? '编辑链接' : '新增链接'"
      width="600px"
      destroy-on-close
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="链接名称" prop="name">
          <el-input v-model="formData.name" placeholder="请输入链接名称" clearable />
        </el-form-item>

        <el-form-item label="链接地址" prop="url">
          <el-input v-model="formData.url" placeholder="请输入链接URL，如：https://www.example.com" clearable />
        </el-form-item>

        <el-form-item label="描述" prop="description">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="3"
            placeholder="请输入链接描述（可选）"
            clearable
          />
        </el-form-item>

        <el-form-item label="Logo" prop="logoId">
          <FileUpload
            v-model="formData.logoId"
            file-type="image"
            :limit="1"
            list-type="picture-card"
            return-type="id"
            related-type="config"
            :related-id="formData.id"
          />
          <div style="margin-top: 8px; color: #909399; font-size: 12px;">
            建议尺寸：200x100px，支持 JPG、PNG 格式
          </div>
        </el-form-item>

        <el-form-item label="排序" prop="sortOrder">
          <el-input-number v-model="formData.sortOrder" :min="0" :max="9999" />
          <span style="margin-left: 10px; color: #909399; font-size: 12px;">
            数字越小越靠前
          </span>
        </el-form-item>

        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="formData.status">
            <el-radio :label="1">启用</el-radio>
            <el-radio :label="0">禁用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitting">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { systemConfigApi } from '@/api'
import { formatDateTime } from '@/utils/date'
import FileUpload from '@/components/FileUpload.vue'

const tableData = ref([])
const loading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitting = ref(false)
const formRef = ref(null)

const formData = reactive({
  id: null,
  name: '',
  url: '',
  logoId: null,
  description: '',
  sortOrder: 1,
  status: 1
})

const formRules = {
  name: [
    { required: true, message: '请输入链接名称', trigger: 'blur' },
    { max: 255, message: '链接名称不能超过255个字符', trigger: 'blur' }
  ],
  url: [
    { required: true, message: '请输入链接地址', trigger: 'blur' },
    { type: 'url', message: '请输入正确的URL格式', trigger: 'blur' },
    { max: 500, message: 'URL不能超过500个字符', trigger: 'blur' }
  ],
  description: [
    { max: 500, message: '描述不能超过500个字符', trigger: 'blur' }
  ]
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await systemConfigApi.friendlyLinks.getList()
    if (res.success && res.data) {
      tableData.value = res.data.sort((a, b) => a.sortOrder - b.sortOrder)
    } else {
      ElMessage.error(res.message || '加载数据失败')
    }
  } catch (error) {

    notifyError(error, '加载数据失败，请稍后重试')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  isEdit.value = false
  Object.assign(formData, {
    id: null,
    name: '',
    url: '',
    logoId: null,
    description: '',
    sortOrder: tableData.value.length + 1,
    status: 1
  })
  dialogVisible.value = true
}

const handleEdit = async (row) => {
  isEdit.value = true
  try {
    const res = await systemConfigApi.friendlyLinks.getDetail(row.id)
    if (res.success && res.data) {
      Object.assign(formData, {
        id: res.data.id,
        version: res.data.version,
        name: res.data.name,
        url: res.data.url,
        logoId: res.data.logoId,
        description: res.data.description || '',
        sortOrder: res.data.sortOrder,
        status: res.data.status
      })
      dialogVisible.value = true
    } else {
      ElMessage.error(res.message || '获取详情失败')
    }
  } catch (error) {

    notifyError(error, '获取详情失败，请稍后重试')
  }
}

const handleSort = async (row, direction) => {
  try {
    const res = await systemConfigApi.friendlyLinks.updateSort(row.id, direction)
    if (res.success) {
      ElMessage.success('排序成功')
      loadData()
    } else {
      ElMessage.error(res.message || '排序失败')
    }
  } catch (error) {

    notifyError(error, '排序失败，请稍后重试')
  }
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除友情链接"${row.name}"吗？删除后将无法恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const res = await systemConfigApi.friendlyLinks.delete(row.id)
    if (res.success) {
      ElMessage.success(res.message || '删除成功')
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

const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch (error) {
    ElMessage.warning('请正确填写表单')
    return
  }

  submitting.value = true
  try {
    const submitData = {
      version: formData.version,
      name: formData.name,
      url: formData.url,
      logoId: formData.logoId,
      description: formData.description || null,
      sortOrder: formData.sortOrder,
      status: formData.status
    }

    let res
    if (isEdit.value) {
      res = await systemConfigApi.friendlyLinks.update(formData.id, submitData)
    } else {
      res = await systemConfigApi.friendlyLinks.create(submitData)
    }

    if (res.success) {
      ElMessage.success(res.message || (isEdit.value ? '更新成功' : '创建成功'))
      dialogVisible.value = false
      loadData()
    } else {
      ElMessage.error(res.message || (isEdit.value ? '更新失败' : '创建失败'))
    }
  } catch (error) {

    notifyError(error, isEdit.value ? '更新失败，请稍后重试' : '创建失败，请稍后重试')
  } finally {
    submitting.value = false
  }
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
</style>

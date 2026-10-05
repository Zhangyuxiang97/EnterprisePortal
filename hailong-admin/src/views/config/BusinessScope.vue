<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>业务范围管理</span>
          <el-button v-if="canManage" type="primary" icon="Plus" @click="handleAdd">新增业务范围</el-button>
        </div>
      </template>

      <!-- 表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe row-key="id">
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="name" label="业务名称" min-width="200" />
        <el-table-column prop="description" label="业务描述" min-width="300" show-overflow-tooltip />
        <el-table-column prop="sortOrder" label="排序" width="100" align="center" />
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'danger'">
              {{ row.status ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right" align="center">
          <template #default="{ row }">
            <el-button v-if="canManage" type="primary" size="small" link @click="handleEdit(row)">编辑</el-button>
            <el-button type="success" size="small" link @click="handleSort(row, 'up')" :disabled="row.sortOrder === 1">上移</el-button>
            <el-button type="success" size="small" link @click="handleSort(row, 'down')">下移</el-button>
            <el-button v-if="canManage" type="danger" size="small" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      class="editor-dialog"
      :before-close="beforeEditorClose"
      :title="isEdit ? '编辑业务范围' : '新增业务范围'"
      width="800px"
      destroy-on-close
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="业务名称" prop="name">
          <el-input
            v-model="formData.name"
            placeholder="请输入业务名称（最多100个字符）"
            maxlength="100"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="业务描述" prop="description">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="4"
            placeholder="请输入业务描述（最多500个字符）"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="详细内容" prop="content">
          <RichEditor
            v-model="formData.content"
            :height="300"
            placeholder="请输入详细内容..."
          />
        </el-form-item>

        <el-form-item label="业务特点" prop="features">
          <el-tag
            v-for="(feature, index) in formData.features"
            :key="index"
            closable
            @close="removeFeature(index)"
            style="margin-right: 8px; margin-bottom: 8px;"
          >
            {{ feature }}
          </el-tag>
          <el-input
            v-if="featureInputVisible"
            ref="featureInputRef"
            v-model="featureInputValue"
            size="small"
            style="width: 200px;"
            @keyup.enter="handleFeatureInputConfirm"
            @blur="handleFeatureInputConfirm"
          />
          <el-button
            v-else
            size="small"
            @click="showFeatureInput"
          >
            + 添加特点
          </el-button>
        </el-form-item>

        <el-form-item label="业务图片" prop="imageId">
          <FileUpload
            v-model="formData.imageId"
            file-type="image"
            :limit="1"
            list-type="picture-card"
            return-type="id"
            related-type="config"
            :related-id="formData.id"
          />
        </el-form-item>

        <el-form-item label="排序" prop="sortOrder">
          <el-input-number
            v-model="formData.sortOrder"
            :min="1"
            :max="999"
            placeholder="数字越小越靠前"
          />
        </el-form-item>

        <el-form-item label="状态">
          <el-radio-group v-model="formData.status">
            <el-radio :value="true">启用</el-radio>
            <el-radio :value="false">禁用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitting" :disabled="activeUploads > 0">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { createLatestRequest } from '@/utils/latestRequest'
import { useEditorForm } from '@/composables/useEditorForm'
import { ref, reactive, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemConfigApi } from '@/api'
import FileUpload from '@/components/FileUpload.vue'
import RichEditor from '@/components/RichEditor.vue'

// 表格数据
const tableData = ref([])
const loading = ref(false)

// 对话框
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitting = ref(false)
const formRef = ref(null)

// 业务特点输入
const featureInputVisible = ref(false)
const featureInputValue = ref('')
const featureInputRef = ref(null)

// 表单数据
const formData = reactive({
  id: null,
  name: '',
  description: '',
  content: '',
  features: [],
  imageId: [],
  sortOrder: 1,
  status: true
})
const { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads } = useEditorForm(formData, dialogVisible, submitting)


// 表单验证规则
const formRules = {
  name: [
    { required: true, message: '请输入业务名称', trigger: 'blur' },
    { max: 100, message: '业务名称长度不能超过100个字符', trigger: 'blur' }
  ],
  description: [
    { max: 500, message: '业务描述长度不能超过500个字符', trigger: 'blur' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序', trigger: 'blur' }
  ]
}

/**
 * 加载数据
 */
const listRequests = createLatestRequest()
const loadData = async () => {
  const requestId = listRequests.begin()
  loading.value = true
  try {
    const res = await systemConfigApi.businessScope.getList()
    if (!listRequests.isCurrent(requestId)) return

    if (res.success && res.data) {
      tableData.value = res.data.sort((a, b) => a.sortOrder - b.sortOrder)
    } else {
      ElMessage.error(res.message || '加载数据失败')
    }
  } catch (error) {
    if (!listRequests.isCurrent(requestId)) return

    notifyError(error, '加载数据失败，请稍后重试')
  } finally {
    if (listRequests.isCurrent(requestId)) loading.value = false
  }
}

/**
 * 新增
 */
const handleAdd = () => {
  isEdit.value = false
  Object.assign(formData, {
    id: null,
    name: '',
    description: '',
    content: '',
    features: [],
    imageId: [],
    sortOrder: tableData.value.length + 1,
    status: true
  })
  dialogVisible.value = true
}

/**
 * 编辑
 */
const handleEdit = async (row) => {
  isEdit.value = true
  try {
    const res = await systemConfigApi.businessScope.getDetail(row.id)
    if (res.success && res.data) {
      Object.assign(formData, {
        id: res.data.id,
        version: res.data.version,
        name: res.data.name,
        description: res.data.description || '',
        content: res.data.content || '',
        features: res.data.features || [],
        imageId: res.data.imageId ? [res.data.imageId] : [],
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

/**
 * 删除
 */
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除业务范围"${row.name}"吗？删除后将无法恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const res = await systemConfigApi.businessScope.delete(row.id)
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

/**
 * 排序
 */
const handleSort = async (row, direction) => {
  try {
    const res = await systemConfigApi.businessScope.updateSort(row.id, direction)
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

/**
 * 添加业务特点
 */
const showFeatureInput = () => {
  featureInputVisible.value = true
  nextTick(() => {
    featureInputRef.value?.focus()
  })
}

const handleFeatureInputConfirm = () => {
  if (featureInputValue.value) {
    formData.features.push(featureInputValue.value)
    featureInputValue.value = ''
  }
  featureInputVisible.value = false
}

const removeFeature = (index) => {
  formData.features.splice(index, 1)
}

/**
 * 提交表单
 */
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
      description: formData.description || null,
      content: formData.content || null,
      features: formData.features.length > 0 ? formData.features : null,
      imageId: formData.imageId && formData.imageId.length > 0 ? formData.imageId[0] : null,
      sortOrder: formData.sortOrder,
      status: formData.status
    }

    let res
    if (isEdit.value) {
      res = await systemConfigApi.businessScope.update(formData.id, submitData)
    } else {
      res = await systemConfigApi.businessScope.create(submitData)
    }

    if (res.success) {
      ElMessage.success(res.message || (isEdit.value ? '更新成功' : '创建成功'))
      markSaved()
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
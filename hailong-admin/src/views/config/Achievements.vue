<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>重大业绩</span>
          <el-button v-if="canManage" type="primary" icon="Plus" @click="handleAdd">新增业绩</el-button>
        </div>
      </template>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="projectName" label="项目名称" min-width="200" show-overflow-tooltip />
        <el-table-column prop="projectType" label="项目类型" width="120" />
        <el-table-column prop="clientName" label="客户名称" width="150" show-overflow-tooltip />
        <el-table-column prop="projectAmount" label="项目金额(万元)" width="140" align="right">
          <template #default="{ row }">
            {{ row.projectAmount ? row.projectAmount.toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="completionDate" label="完成日期" width="120" />
        <el-table-column prop="sortOrder" label="排序" width="80" align="center" />
        <el-table-column label="状态" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'info'" size="small">
              {{ row.status ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row, $index }">
            <el-button v-if="canManage" type="primary" size="small" link @click="handleEdit(row)">编辑</el-button>
            <el-button
              type="info"
              size="small"
              link
              @click="handleSort(row, 'up')"
              :disabled="$index === 0"
            >上移</el-button>
            <el-button
              type="info"
              size="small"
              link
              @click="handleSort(row, 'down')"
              :disabled="$index === tableData.length - 1"
            >下移</el-button>
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
      :title="dialogTitle"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="formData" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="项目名称" prop="projectName">
          <el-input v-model="formData.projectName" placeholder="请输入项目名称" maxlength="200" show-word-limit />
        </el-form-item>

        <el-form-item label="项目类型" prop="projectType">
          <el-input v-model="formData.projectType" placeholder="请输入项目类型" maxlength="50" />
        </el-form-item>

        <el-form-item label="客户名称" prop="clientName">
          <el-input v-model="formData.clientName" placeholder="请输入客户名称" maxlength="200" />
        </el-form-item>

        <el-form-item label="项目金额(万元)" prop="projectAmount">
          <el-input-number
            v-model="formData.projectAmount"
            :min="0"
            :precision="2"
            :controls="false"
            placeholder="请输入项目金额（万元）"
            style="width: 100%"
          />
        </el-form-item>

        <el-form-item label="完成日期" prop="completionDate">
          <el-date-picker
            v-model="formData.completionDate"
            type="date"
            placeholder="选择完成日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>

        <el-form-item label="项目图片">
          <FileUpload
            v-model="formData.imageIds"
            file-type="image"
            :limit="1"
            :multiple="false"
            list-type="picture-card"
            return-type="id"
            related-type="config"
            :related-id="formData.id"
          />
          <div class="form-tip">建议尺寸：800x600，仅支持上传1张图片</div>
        </el-form-item>

        <el-form-item label="项目描述">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="4"
            placeholder="请输入项目描述"
            maxlength="1000"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="排序" prop="sortOrder">
          <el-input-number v-model="formData.sortOrder" :min="0" :max="9999" />
          <span class="form-tip" style="margin-left: 10px;">数字越小越靠前</span>
        </el-form-item>

        <el-form-item label="状态">
          <el-switch
            v-model="formData.status"
            :active-value="true"
            :inactive-value="false"
            active-text="启用"
            inactive-text="禁用"
          />
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
import { notifyError } from '@/utils/errors'
import { createLatestRequest } from '@/utils/latestRequest'
import { useEditorForm } from '@/composables/useEditorForm'
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemConfigApi } from '@/api'
import FileUpload from '@/components/FileUpload.vue'

const tableData = ref([])
const loading = ref(false)
const dialogVisible = ref(false)
const dialogTitle = ref('')
const submitting = ref(false)
const formRef = ref(null)

const formData = reactive({
  id: null,
  projectName: '',
  projectType: '',
  clientName: '',
  projectAmount: null,
  completionDate: '',
  imageIds: [],
  description: '',
  sortOrder: 0,
  status: true
})
const { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads } = useEditorForm(formData, dialogVisible, submitting)


const rules = {
  projectName: [
    { required: true, message: '请输入项目名称', trigger: 'blur' },
    { min: 2, max: 200, message: '项目名称长度在 2 到 200 个字符', trigger: 'blur' }
  ],
  projectType: [
    { required: true, message: '请输入项目类型', trigger: 'blur' }
  ],
  clientName: [
    { required: true, message: '请输入客户名称', trigger: 'blur' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序', trigger: 'blur' }
  ]
}

const listRequests = createLatestRequest()
const loadData = async () => {
  const requestId = listRequests.begin()
  loading.value = true
  try {
    const res = await systemConfigApi.achievements.getList()
    if (!listRequests.isCurrent(requestId)) return
    if (res.success) {
      tableData.value = res.data || []
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

const handleAdd = () => {
  dialogTitle.value = '新增业绩'
  Object.assign(formData, {
    id: null,
    projectName: '',
    projectType: '',
    clientName: '',
    projectAmount: null,
    completionDate: '',
    imageIds: [],
    description: '',
    sortOrder: 0,
    status: true
  })
  dialogVisible.value = true
}

const handleEdit = (row) => {
  dialogTitle.value = '编辑业绩'
  Object.assign(formData, {
    id: row.id,
        version: row.version,
    projectName: row.projectName,
    projectType: row.projectType,
    clientName: row.clientName,
    projectAmount: row.projectAmount,
    completionDate: row.completionDate,
    imageIds: row.imageIds || [],
    description: row.description || '',
    sortOrder: row.sortOrder,
    status: row.status
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
        ? await systemConfigApi.achievements.update(formData.id, formData)
        : await systemConfigApi.achievements.create(formData)

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

const handleSort = async (row, direction) => {
  try {
    const res = await systemConfigApi.achievements.updateSort(row.id, direction)
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
    await ElMessageBox.confirm('确定要删除该业绩吗？', '提示', {
      type: 'warning'
    })

    const res = await systemConfigApi.achievements.delete(row.id)
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

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}
</style>

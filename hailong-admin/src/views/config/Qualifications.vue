<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>企业资质管理</span>
          <el-button v-if="canManage" type="primary" icon="Plus" @click="handleAdd">新增资质</el-button>
        </div>
      </template>

      <!-- 表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="sortOrder" label="排序" width="80" align="center" />
        <el-table-column prop="name" label="资质名称" min-width="200" />
        <el-table-column prop="certificateNumber" label="证书编号" width="180" />
        <el-table-column prop="issueDate" label="颁发日期" width="120" align="center">
          <template #default="{ row }">
            {{ formatDate(row.issueDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="expiryDate" label="有效期至" width="120" align="center">
          <template #default="{ row }">
            {{ formatDate(row.expiryDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="status" label="展示状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'danger'">
              {{ row.status ? '显示' : '隐藏' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="证书有效期" min-width="155"><template #default="{ row }">{{ certificateValidity(row) }}</template></el-table-column>
        <el-table-column label="操作" width="150" fixed="right" align="center">
          <template #default="{ row }">
            <el-button v-if="canManage" type="primary" size="small" link @click="handleEdit(row)">编辑</el-button>
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
      :title="isEdit ? '编辑资质' : '新增资质'"
      width="700px"
      destroy-on-close
      :close-on-click-modal="false"
    >
      <p class="form-tip">编号、日期不详可留空；长期证书请在描述中注明，不填写推测日期。展示状态仅控制门户是否显示。</p>
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="资质名称" prop="name">
          <el-input
            v-model="formData.name"
            placeholder="请输入资质名称（最多200个字符）"
            maxlength="200"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="证书编号" prop="certificateNumber">
          <el-input
            v-model="formData.certificateNumber"
            placeholder="请输入证书编号（最多100个字符）"
            maxlength="100"
          />
        </el-form-item>

        <el-form-item label="颁发机构">
          <el-input
            v-model="formData.issuingAuthority"
            placeholder="请输入颁发机构（最多200个字符）"
            maxlength="200"
          />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="颁发日期" prop="issueDate">
              <el-date-picker
                v-model="formData.issueDate"
                type="date"
                placeholder="选择颁发日期"
                value-format="YYYY-MM-DD"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="有效期至" prop="expiryDate">
              <el-date-picker
                v-model="formData.expiryDate"
                type="date"
                placeholder="选择有效期"
                value-format="YYYY-MM-DD"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="证书图片">
          <FileUpload
            v-model="formData.certificateImageId"
            file-type="image"
            :limit="1"
            list-type="picture-card"
            return-type="id"
            related-type="config"
            :related-id="formData.id"
          />
          <div class="form-tip">建议上传清晰的证书扫描件或照片</div>
        </el-form-item>

        <el-form-item label="资质描述">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="4"
            placeholder="请输入资质描述（最多500个字符）"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="排序">
          <el-input-number
            v-model="formData.sortOrder"
            :min="0"
            :step="1"
            controls-position="right"
            style="width: 200px;"
          />
          <div class="form-tip">数字越小越靠前，相同数字按颁发日期排序</div>
        </el-form-item>

        <el-form-item label="展示状态">
          <el-radio-group v-model="formData.status">
            <el-radio :value="true">显示</el-radio>
            <el-radio :value="false">隐藏</el-radio>
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
import { certificateValidity } from '@/utils/form'
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemConfigApi } from '@/api'
import FileUpload from '@/components/FileUpload.vue'

// 表格数据
const tableData = ref([])
const loading = ref(false)

// 对话框
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitting = ref(false)
const formRef = ref(null)

// 表单数据
const formData = reactive({
  id: null,
  name: '',
  certificateNumber: '',
  issuingAuthority: '',
  issueDate: '',
  expiryDate: '',
  certificateImageId: [],
  description: '',
  sortOrder: 0,
  status: true
})
const { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads } = useEditorForm(formData, dialogVisible, submitting)


// 表单验证规则
const formRules = {
  name: [
    { required: true, message: '请输入资质名称', trigger: 'blur' },
    { max: 200, message: '资质名称长度不能超过200个字符', trigger: 'blur' }
  ],
  certificateNumber: [
    { max: 100, message: '证书编号长度不能超过100个字符', trigger: 'blur' }
  ],
  expiryDate: [{ validator: (_rule, value, done) => {
    done(value && formData.issueDate && value < formData.issueDate ? new Error('到期日不能早于颁发日') : undefined)
  }, trigger: 'change' }]

}

/**
 * 格式化日期
 */
const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

/**
 * 加载数据
 */
const listRequests = createLatestRequest()
const loadData = async () => {
  const requestId = listRequests.begin()
  loading.value = true
  try {
    const res = await systemConfigApi.qualifications.getList()
    if (!listRequests.isCurrent(requestId)) return

    if (res.success && res.data) {
      tableData.value = res.data
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
    certificateNumber: '',
    issuingAuthority: '',
    issueDate: '',
    expiryDate: '',
    certificateImageId: [],
    description: '',
    sortOrder: 0,
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
    const res = await systemConfigApi.qualifications.getDetail(row.id)
    if (res.success && res.data) {
      Object.assign(formData, {
        id: res.data.id,
        version: res.data.version,
        name: res.data.name,
        certificateNumber: res.data.certificateNumber,
        issuingAuthority: res.data.issuingAuthority || '',
        issueDate: res.data.issueDate,
        expiryDate: res.data.expiryDate,
        certificateImageId: res.data.certificateImageId ? [res.data.certificateImageId] : [],
        description: res.data.description || '',
        sortOrder: res.data.sortOrder || 0,
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
      `确定要删除资质"${row.name}"吗？删除后将无法恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const res = await systemConfigApi.qualifications.delete(row.id)
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
      certificateNumber: formData.certificateNumber || null,
      issuingAuthority: formData.issuingAuthority || null,
      issueDate: formData.issueDate || null,
      expiryDate: formData.expiryDate || null,
      certificateImageId: formData.certificateImageId && formData.certificateImageId.length > 0
        ? formData.certificateImageId[0]
        : null,
      description: formData.description || null,
      sortOrder: formData.sortOrder || 0,
      status: formData.status
    }

    let res
    if (isEdit.value) {
      res = await systemConfigApi.qualifications.update(formData.id, submitData)
    } else {
      res = await systemConfigApi.qualifications.create(submitData)
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

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}
</style>
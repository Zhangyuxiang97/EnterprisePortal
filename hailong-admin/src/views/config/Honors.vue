<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>企业荣誉管理</span>
          <el-button v-if="canManage" type="primary" icon="Plus" @click="handleAdd">新增荣誉</el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <el-form :model="searchForm" inline class="search-form">
        <el-form-item label="荣誉级别">
          <el-select v-model="searchForm.honorLevel" placeholder="请选择" clearable style="width: 150px;">
            <el-option label="国家级" value="国家级" />
            <el-option label="省级" value="省级" />
            <el-option label="市级" value="市级" />
            <el-option label="行业级" value="行业级" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">搜索</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="name" label="荣誉名称" min-width="200" />
        <el-table-column prop="honorLevel" label="荣誉级别" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getLevelType(row.honorLevel)">{{ row.honorLevel || '未注明' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="awardOrganization" label="颁发机构" width="180" show-overflow-tooltip />
        <el-table-column prop="awardDate" label="获奖日期" width="120" align="center">
          <template #default="{ row }">
            {{ formatDate(row.awardDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="sortOrder" label="排序" width="80" align="center" />
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'danger'">
              {{ row.status ? '显示' : '隐藏' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right" align="center">
          <template #default="{ row }">
            <el-button v-if="canManage" type="primary" size="small" link @click="handleEdit(row)">编辑</el-button>
            <el-button v-if="canManage" type="danger" size="small" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSearch"
        @current-change="loadData"
        style="margin-top: 20px; justify-content: flex-end;"
      />
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      class="editor-dialog"
      :before-close="beforeEditorClose"
      :title="isEdit ? '编辑荣誉' : '新增荣誉'"
      width="700px"
      destroy-on-close
      :close-on-click-modal="false"
    >
      <p class="form-tip">获奖日期、荣誉级别无确切资料时可留空，保留证书原文即可。</p>
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="荣誉名称" prop="name">
          <el-input
            v-model="formData.name"
            placeholder="请输入荣誉名称（最多200个字符）"
            maxlength="200"
            show-word-limit
          />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="荣誉级别" prop="honorLevel">
              <el-select v-model="formData.honorLevel" placeholder="请选择荣誉级别" style="width: 100%;">
                <el-option label="国家级" value="国家级" />
                <el-option label="省级" value="省级" />
                <el-option label="市级" value="市级" />
                <el-option label="行业级" value="行业级" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="获奖日期" prop="awardDate">
              <el-date-picker
                v-model="formData.awardDate"
                type="date"
                placeholder="选择获奖日期"
                value-format="YYYY-MM-DD"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="颁发机构">
          <el-input
            v-model="formData.awardOrganization"
            placeholder="请输入颁发机构（最多200个字符）"
            maxlength="200"
          />
        </el-form-item>

        <el-form-item label="荣誉图片">
          <FileUpload
            v-model="formData.imageId"
            file-type="image"
            :limit="1"
            list-type="picture-card"
            return-type="id"
            related-type="config"
            :related-id="formData.id"
          />
          <div class="form-tip">建议上传荣誉证书或奖牌照片</div>
        </el-form-item>

        <el-form-item label="荣誉描述">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="4"
            placeholder="请输入荣誉描述（最多500个字符）"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="排序">
              <el-input-number
                v-model="formData.sortOrder"
                :min="1"
                :max="999"
                placeholder="数字越小越靠前"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态">
              <el-radio-group v-model="formData.status">
                <el-radio :value="true">显示</el-radio>
                <el-radio :value="false">隐藏</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
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
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemConfigApi } from '@/api'
import FileUpload from '@/components/FileUpload.vue'

// 搜索表单
const searchForm = reactive({
  honorLevel: ''
})

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

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
  honorLevel: '',
  awardOrganization: '',
  awardDate: '',
  imageId: [],
  description: '',
  sortOrder: 1,
  status: true
})
const { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads } = useEditorForm(formData, dialogVisible, submitting)


// 表单验证规则
const formRules = {
  name: [
    { required: true, message: '请输入荣誉名称', trigger: 'blur' },
    { max: 200, message: '荣誉名称长度不能超过200个字符', trigger: 'blur' }
  ]
}

/**
 * 获取级别标签类型
 */
const getLevelType = (level) => {
  const typeMap = {
    '国家级': 'danger',
    '省级': 'warning',
    '市级': 'success',
    '行业级': 'info'
  }
  return typeMap[level] || 'info'
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
    const params = {
      honorLevel: searchForm.honorLevel || undefined,
      page: pagination.pageIndex,
      pageSize: pagination.pageSize
    }

    const res = await systemConfigApi.honors.getList(params)
    if (!listRequests.isCurrent(requestId)) return

    if (res.success && res.data) {
      const items = (res.data || []).filter(item => !params.honorLevel || item.honorLevel === params.honorLevel)
      pagination.total = items.length
      pagination.pageIndex = Math.min(pagination.pageIndex, Math.max(1, Math.ceil(items.length / pagination.pageSize)))
      const start = (pagination.pageIndex - 1) * pagination.pageSize
      tableData.value = items.slice(start, start + pagination.pageSize)
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
 * 搜索
 */
const handleSearch = () => {
  pagination.pageIndex = 1
  loadData()
}

/**
 * 重置
 */
const handleReset = () => {
  searchForm.honorLevel = ''
  handleSearch()
}

/**
 * 新增
 */
const handleAdd = () => {
  isEdit.value = false
  Object.assign(formData, {
    id: null,
    name: '',
    honorLevel: '',
    awardOrganization: '',
    awardDate: '',
    imageId: [],
    description: '',
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
    const res = await systemConfigApi.honors.getDetail(row.id)
    if (res.success && res.data) {
      Object.assign(formData, {
        id: res.data.id,
        version: res.data.version,
        name: res.data.name,
        honorLevel: res.data.honorLevel,
        awardOrganization: res.data.awardOrganization || '',
        awardDate: res.data.awardDate,
        imageId: res.data.imageId ? [res.data.imageId] : [],
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

/**
 * 删除
 */
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除荣誉"${row.name}"吗？删除后将无法恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const res = await systemConfigApi.honors.delete(row.id)
    if (res.success) {
      ElMessage.success(res.message || '删除成功')
      if (tableData.value.length === 1 && pagination.pageIndex > 1) {
        pagination.pageIndex--
      }
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
      honorLevel: formData.honorLevel || null,
      awardOrganization: formData.awardOrganization || null,
      awardDate: formData.awardDate || null,
      imageId: formData.imageId && formData.imageId.length > 0 ? formData.imageId[0] : null,
      description: formData.description || null,
      sortOrder: formData.sortOrder,
      status: formData.status
    }

    let res
    if (isEdit.value) {
      res = await systemConfigApi.honors.update(formData.id, submitData)
    } else {
      res = await systemConfigApi.honors.create(submitData)
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

.search-form {
  margin-bottom: 16px;
}

.search-form :deep(.el-form-item) {
  margin-bottom: 10px;
}

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}
</style>
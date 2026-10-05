<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>轮播图管理</span>
          <el-button type="primary" icon="Plus" @click="handleAdd">新增轮播图</el-button>
        </div>
      </template>

      <!-- 表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column label="图片" width="150">
          <template #default="{ row }">
            <el-image
              :src="row.imageUrl"
              :preview-src-list="[row.imageUrl]"
              fit="cover"
              style="width: 100px; height: 60px;"
            />
          </template>
        </el-table-column>
        <el-table-column prop="title" label="标题" min-width="200" show-overflow-tooltip />
        <el-table-column prop="linkUrl" label="链接地址" min-width="200" show-overflow-tooltip />
        <el-table-column prop="sortOrder" label="排序" width="100" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'danger'">
              {{ row.status ? '启用' : '禁用' }}
            </el-tag>
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
      :title="isEdit ? '编辑轮播图' : '新增轮播图'"
      width="600px"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="标题" prop="title">
          <el-input
            v-model="formData.title"
            placeholder="请输入标题（最多200个字符）"
            maxlength="200"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="轮播图" prop="imageId">
          <FileUpload
            v-model="formData.imageId"
            file-type="image"
            :limit="1"
            list-type="picture-card"
            return-type="id"
            related-type="config"
            :related-id="formData.id"
          />
          <div class="form-tip">建议尺寸：1920x600px，支持jpg、png格式</div>
        </el-form-item>

        <el-form-item label="链接地址">
          <el-input
            v-model="formData.linkUrl"
            placeholder="请输入链接URL（选填）"
            maxlength="500"
          />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="排序" prop="sortOrder">
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
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
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
import { systemConfigApi } from '@/api'
import FileUpload from '@/components/FileUpload.vue'

const tableData = ref([])
const loading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitting = ref(false)
const formRef = ref(null)

const formData = reactive({
  id: null,
  title: '',
  imageId: null,
  linkUrl: '',
  sortOrder: 1,
  status: 1
})

const formRules = {
  title: [
    { required: true, message: '请输入标题', trigger: 'blur' },
    { max: 200, message: '标题长度不能超过200个字符', trigger: 'blur' }
  ],
  imageId: [
    { required: true, message: '请上传轮播图图片', trigger: 'change' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序', trigger: 'blur' }
  ]
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await systemConfigApi.banners.getList()
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
    title: '',
    imageId: null,
    linkUrl: '',
    sortOrder: tableData.value.length + 1,
    status: 1
  })
  dialogVisible.value = true
}

const handleEdit = async (row) => {
  isEdit.value = true
  try {
    const res = await systemConfigApi.banners.getDetail(row.id)
    if (res.success && res.data) {
      Object.assign(formData, {
        id: res.data.id,
        version: res.data.version,
        title: res.data.title,
        imageId: res.data.imageId,
        linkUrl: res.data.linkUrl || '',
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

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除轮播图"${row.title}"吗？删除后将无法恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const res = await systemConfigApi.banners.delete(row.id)
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

const handleSort = async (row, direction) => {
  try {
    const res = await systemConfigApi.banners.updateSort(row.id, direction)
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
      title: formData.title,
      imageId: formData.imageId,
      linkUrl: formData.linkUrl || null,
      sortOrder: formData.sortOrder,
      status: formData.status
    }

    let res
    if (isEdit.value) {
      res = await systemConfigApi.banners.update(formData.id, submitData)
    } else {
      res = await systemConfigApi.banners.create(submitData)
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

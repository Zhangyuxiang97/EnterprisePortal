<template>
  <div class="attachment-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="附件分类">
          <el-select v-model="searchForm.category" placeholder="请选择" clearable style="width: 180px;">
            <el-option label="图片" value="image" />
            <el-option label="文档" value="document" />
            <el-option label="视频" value="video" />
            <el-option label="其他" value="other" />
          </el-select>
        </el-form-item>
        <el-form-item label="关联类型">
          <el-select v-model="searchForm.relatedType" placeholder="请选择" clearable style="width: 180px;">
            <el-option label="公告" value="announcement" />
            <el-option label="信息发布" value="info_publication" />
            <el-option label="企业简介" value="company_profile" />
            <!-- <el-option label="轮播图" value="carousel_banner" /> -->
            <el-option label="业务范围" value="business_scope" />
            <el-option label="企业荣誉" value="company_honor" />
            <el-option label="企业资质" value="company_qualification" />
            <el-option label="重大业绩" value="major_achievement" />
          </el-select>
        </el-form-item>
        <el-form-item label="关键词">
          <el-input v-model="searchForm.keyword" placeholder="文件名称" clearable style="width: 200px;" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" :icon="Search" @click="handleSearch">搜索</el-button>
          <el-button :icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="table-card">
      <template #header>
        <div class="card-header">
          <span>附件列表</span>
          <div>
            <el-button type="primary" :icon="Upload" @click="handleUpload">上传附件</el-button>
            <el-button type="danger" :icon="Delete" @click="handleBatchDelete" :disabled="!selectedIds.length">
              批量删除
            </el-button>
          </div>
        </div>
      </template>

      <el-table
        :data="tableData"
        v-loading="loading"
        @selection-change="handleSelectionChange"
        stripe
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column label="预览" width="100">
          <template #default="{ row }">
            <el-image
              v-if="row.category === 'image'"
              :src="row.fileUrl"
              :preview-src-list="[row.fileUrl]"
              :preview-teleported="true"
              :z-index="9999"
              fit="cover"
              style="width: 60px; height: 60px; border-radius: 4px;"
            />
            <el-icon v-else :size="40" color="#409EFF">
              <Document v-if="row.category === 'document'" />
              <VideoPlay v-else-if="row.category === 'video'" />
              <Files v-else />
            </el-icon>
          </template>
        </el-table-column>
        <el-table-column prop="fileName" label="文件名称" min-width="200" show-overflow-tooltip />
        <el-table-column label="文件大小" width="120">
          <template #default="{ row }">
            {{ formatFileSize(row.fileSize) }}
          </template>
        </el-table-column>
        <el-table-column label="文件类型" width="120">
          <template #default="{ row }">
            <el-tag :type="getCategoryType(row.category)">
              {{ getCategoryLabel(row.category) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="关联类型" width="150">
          <template #default="{ row }">
            <el-tag v-if="row.relatedType" type="info">
              {{ getRelatedTypeLabel(row.relatedType) }}
            </el-tag>
            <span v-else style="color: #999;">-</span>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="上传时间" width="180" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleView(row)">查看</el-button>
            <el-button link type="primary" @click="handleDownload(row)">下载</el-button>
            <el-button link type="danger" @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        v-model:current-page="pagination.page"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        class="pagination"
      />
    </el-card>

    <!-- 上传对话框 -->
    <el-dialog v-model="uploadDialogVisible" title="上传附件" width="600px" @close="handleDialogClose">
      <el-form :model="uploadForm" label-width="100px">
        <el-form-item label="附件分类" required>
          <el-select v-model="uploadForm.category" placeholder="请选择分类" style="width: 100%;">
            <el-option label="图片" value="image" />
            <el-option label="文档" value="document" />
            <el-option label="视频" value="video" />
            <el-option label="其他" value="other" />
          </el-select>
        </el-form-item>
        <el-form-item label="关联类型">
          <el-select v-model="uploadForm.relatedType" placeholder="请选择关联类型（可选）" clearable style="width: 100%;">
            <el-option label="公告" value="announcement" />
            <el-option label="信息发布" value="info_publication" />
            <el-option label="企业简介" value="company_profile" />
            <!-- <el-option label="轮播图" value="carousel_banner" /> -->
            <el-option label="业务范围" value="business_scope" />
            <el-option label="企业荣誉" value="company_honor" />
            <el-option label="企业资质" value="company_qualification" />
            <el-option label="重大业绩" value="major_achievement" />
          </el-select>
        </el-form-item>
        <el-form-item label="关联ID">
          <el-input-number 
            v-model="uploadForm.relatedId" 
            :min="1" 
            placeholder="请输入关联ID（可选）"
            style="width: 100%;"
            :disabled="!uploadForm.relatedType"
          />
        </el-form-item>
        <el-form-item label="选择文件" required>
          <el-upload
            ref="uploadRef"
            :action="getUploadAction"
            :headers="uploadHeaders"
            :on-success="handleUploadSuccess"
            :on-error="handleUploadError"
            :before-upload="beforeUpload"
            :file-list="fileList"
            :auto-upload="false"
            multiple
            drag
          >
            <el-icon class="el-icon--upload"><upload-filled /></el-icon>
            <div class="el-upload__text">
              将文件拖到此处，或<em>点击上传</em>
            </div>
            <template #tip>
              <div class="el-upload__tip">
                支持图片、文档、视频等多种格式，单个文件不超过50MB
              </div>
            </template>
          </el-upload>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="uploadDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleUploadConfirm" :loading="uploading">
          {{ uploading ? '上传中...' : '开始上传' }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Upload, Delete, Document, VideoPlay, Files, UploadFilled } from '@element-plus/icons-vue'
import { attachmentApi } from '@/api'
import { tokenUtils } from '@/utils/auth'

// 搜索表单
const searchForm = reactive({
  category: '',
  relatedType: '',
  keyword: ''
})

// 上传表单
const uploadForm = reactive({
  category: 'other',
  relatedType: '',
  relatedId: null
})

// 分页
const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

// 表格数据
const tableData = ref([])
const loading = ref(false)
const selectedIds = ref([])

// 上传相关
const uploadDialogVisible = ref(false)
const uploadRef = ref(null)
const fileList = ref([])
const uploading = ref(false)
const uploadAction = import.meta.env.VITE_API_BASE_URL + '/attachments/upload'
const uploadHeaders = {
  Authorization: `Bearer ${tokenUtils.getToken()}`
}

// 动态生成上传URL，包含查询参数
const getUploadAction = computed(() => {
  const params = new URLSearchParams()
  params.append('category', uploadForm.category || 'other')
  if (uploadForm.relatedType) {
    params.append('relatedType', uploadForm.relatedType)
  }
  if (uploadForm.relatedId) {
    params.append('relatedId', uploadForm.relatedId.toString())
  }
  return `${uploadAction}?${params.toString()}`
})

/**
 * 获取附件列表
 */
const getAttachmentList = async () => {
  loading.value = true
  try {
    const params = {
      page: pagination.page,
      pageSize: pagination.pageSize,
      ...searchForm
    }
    const res = await attachmentApi.getAttachmentList(params)
    console.log('附件列表响应:', res)
    if (res.success && res.data) {
      tableData.value = res.data.items || []
      pagination.total = res.data.totalCount || 0
      console.log('表格数据:', tableData.value)
      console.log('总数:', pagination.total)
    } else if (res.code === 200 && res.data) {
      tableData.value = res.data.items || []
      pagination.total = res.data.totalCount || 0
    }
  } catch (error) {
    console.error('获取附件列表失败:', error)
    ElMessage.error('获取附件列表失败')
  } finally {
    loading.value = false
  }
}

/**
 * 搜索
 */
const handleSearch = () => {
  pagination.page = 1
  getAttachmentList()
}

/**
 * 重置
 */
const handleReset = () => {
  Object.assign(searchForm, {
    category: '',
    relatedType: '',
    keyword: ''
  })
  handleSearch()
}

/**
 * 分页变化
 */
const handlePageChange = (page) => {
  pagination.page = page
  getAttachmentList()
}

const handleSizeChange = (size) => {
  pagination.pageSize = size
  pagination.page = 1
  getAttachmentList()
}

/**
 * 选择变化
 */
const handleSelectionChange = (selection) => {
  selectedIds.value = selection.map(item => item.id)
}

/**
 * 上传附件
 */
const handleUpload = () => {
  uploadDialogVisible.value = true
  fileList.value = []
  Object.assign(uploadForm, {
    category: 'other',
    relatedType: '',
    relatedId: null
  })
}

const handleDialogClose = () => {
  fileList.value = []
  uploadRef.value?.clearFiles()
}

const beforeUpload = (file) => {
  const maxSize = 50 * 1024 * 1024 // 50MB
  if (file.size > maxSize) {
    ElMessage.error('文件大小不能超过50MB')
    return false
  }
  return true
}

const handleUploadSuccess = (response, file) => {
  if (response.code === 200) {
    ElMessage.success(`${file.name} 上传成功`)
  } else {
    ElMessage.error(`${file.name} 上传失败：${response.message}`)
  }
}

const handleUploadError = (error, file) => {
  ElMessage.error(`${file.name} 上传失败`)
}

const handleUploadConfirm = () => {
  if (!uploadRef.value) return
  
  const files = uploadRef.value.uploadFiles
  if (!files || files.length === 0) {
    ElMessage.warning('请先选择文件')
    return
  }

  if (!uploadForm.category) {
    ElMessage.warning('请选择附件分类')
    return
  }

  uploading.value = true
  uploadRef.value.submit()
  
  // 等待上传完成
  setTimeout(() => {
    uploading.value = false
    uploadDialogVisible.value = false
    getAttachmentList()
  }, 2000)
}

/**
 * 查看附件
 */
const handleView = (row) => {
  window.open(row.fileUrl, '_blank')
}

/**
 * 下载附件
 */
const handleDownload = (row) => {
  const link = document.createElement('a')
  link.href = row.fileUrl
  link.download = row.fileName
  link.click()
}

/**
 * 删除附件
 */
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('确定要删除该附件吗？', '提示', {
      type: 'warning'
    })
    const res = await attachmentApi.deleteAttachment(row.id)
    if (res.code === 200) {
      ElMessage.success('删除成功')
      getAttachmentList()
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

/**
 * 批量删除
 */
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(`确定要删除选中的 ${selectedIds.value.length} 个附件吗？`, '提示', {
      type: 'warning'
    })
    const res = await attachmentApi.batchDeleteAttachments(selectedIds.value)
    if (res.code === 200) {
      ElMessage.success('删除成功')
      getAttachmentList()
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

/**
 * 格式化文件大小
 */
const formatFileSize = (bytes) => {
  if (!bytes) return '-'
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB'
  if (bytes < 1024 * 1024 * 1024) return (bytes / 1024 / 1024).toFixed(2) + ' MB'
  return (bytes / 1024 / 1024 / 1024).toFixed(2) + ' GB'
}

/**
 * 获取分类标签
 */
const getCategoryLabel = (category) => {
  const map = {
    image: '图片',
    document: '文档',
    video: '视频',
    other: '其他'
  }
  return map[category] || category
}

/**
 * 获取分类类型
 */
const getCategoryType = (category) => {
  const map = {
    image: 'success',
    document: 'primary',
    video: 'warning',
    other: 'info'
  }
  return map[category] || 'info'
}

/**
 * 获取关联类型标签
 */
const getRelatedTypeLabel = (relatedType) => {
  const map = {
    announcement: '公告',
    info_publication: '信息发布',
    company_profile: '企业简介',
    // carousel_banner: '轮播图',
    business_scope: '业务范围',
    company_honor: '企业荣誉',
    company_qualification: '企业资质',
    major_achievement: '重大业绩'
  }
  return map[relatedType] || relatedType
}

onMounted(() => {
  getAttachmentList()
})
</script>

<style scoped>
.attachment-container {
  padding: 20px;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin-bottom: 0;
}

.table-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pagination {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.el-icon--upload {
  font-size: 67px;
  color: #409EFF;
  margin-bottom: 16px;
}

:deep(.el-upload-dragger) {
  padding: 40px;
}

</style>
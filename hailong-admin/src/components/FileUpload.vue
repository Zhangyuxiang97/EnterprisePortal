<template>
  <div class="file-upload" :class="{ 'hide-upload-btn': fileList.length >= limit }">
    <el-upload
      ref="uploadRef"
      v-model:file-list="fileList"
      :http-request="performUpload"
      :data="uploadData"
      :on-success="handleSuccess"
      :on-error="handleError"
      :on-remove="handleRemove"
      :before-upload="beforeUpload"
      :on-exceed="handleExceed"
      :on-preview="handlePreview"
      :limit="limit"
      :accept="computedAccept"
      :list-type="listType"
      :multiple="multiple"
      :disabled="disabled"
      :auto-upload="true"
      :show-file-list="true"
    >
      <template v-if="listType === 'picture-card'" #default>
        <el-icon><Plus /></el-icon>
      </template>
      <template v-else #default>
        <el-button type="primary" :icon="Upload" :disabled="disabled">
          选择{{ fileTypeText }}
        </el-button>
      </template>
      <template #tip>
        <div class="el-upload__tip">
          {{ tipText }}
        </div>
      </template>
    </el-upload>

    <!-- 图片预览对话框 -->
    <el-dialog v-model="previewVisible" title="图片预览" width="800px">
      <img :src="previewUrl" style="width: 100%" alt="预览图片" />
    </el-dialog>
  </div>
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { inject, onBeforeUnmount } from 'vue'
import { uploadOptions, loadUploadOptions, validateUpload } from '@/utils/upload'
import { ref, computed, watch, onMounted, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import { Upload, Plus } from '@element-plus/icons-vue'
import { getAttachmentDetail, uploadAttachment } from '@/api/attachment'

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => []
  },
  // 文件类型：image/document/video
  fileType: {
    type: String,
    default: 'document'
  },
  // 关联类型（用于后端存储）
  relatedType: {
    type: String,
    default: ''
  },
  // 关联ID
  relatedId: {
    type: [Number, String],
    default: null
  },
  // 最大文件大小（MB）
  maxSize: {
    type: Number,
    default: Infinity // 默认跟随后端限制；业务页面仍可设置更小的上限。
  },
  // 最多上传文件数量
  limit: {
    type: Number,
    default: 5
  },
  // 是否支持多选
  multiple: {
    type: Boolean,
    default: true
  },
  // 列表类型：text/picture/picture-card
  listType: {
    type: String,
    default: 'text'
  },
  // 接受的文件类型（自定义）
  accept: {
    type: String,
    default: ''
  },
  // 是否禁用
  disabled: {
    type: Boolean,
    default: false
  },
  // 返回值类型：url-返回URL数组，id-返回ID数组
  returnType: {
    type: String,
    default: 'url',
    validator: (value) => ['url', 'id'].includes(value)
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

// 上传组件引用
const uploadRef = ref()
const activeUploads = inject('editorActiveUploads', ref(0))
const pendingUploads = new Set()
const uploadControllers = new Set()
const finishUpload = file => { if (pendingUploads.delete(file.uid)) activeUploads.value-- }
onBeforeUnmount(() => {
  for (const controller of uploadControllers) controller.abort()
  activeUploads.value -= pendingUploads.size
  pendingUploads.clear()
})

// 文件列表
const fileList = ref([])

// 图片预览
const previewVisible = ref(false)
const previewUrl = ref('')

// 与普通请求共用认证刷新，组件销毁时终止仍在上传的请求。
const performUpload = ({ file, onProgress }) => {
  const form = new FormData()
  form.append('file', file)
  for (const [key, value] of Object.entries(uploadData.value)) if (value !== undefined && value !== null) form.append(key, value)
  const controller = new AbortController()
  uploadControllers.add(controller)
  const pending = uploadAttachment(form, {
    signal: controller.signal,
    onUploadProgress: event => onProgress({ percent: event.total ? event.loaded / event.total * 100 : 0 })
  }).finally(() => uploadControllers.delete(controller))
  pending.abort = () => controller.abort()
  return pending
}

// 上传附加数据（包含关联类型和关联ID）
const uploadData = computed(() => {
  const data = {}
  if (props.relatedType) {
    data.relatedType = props.relatedType
  }
  if (props.relatedId) {
    data.relatedId = props.relatedId
  }
  return data
})

// 文件类型文本
const fileTypeText = computed(() => {
  const typeMap = {
    image: '图片',
    document: '文件',
    video: '视频'
  }
  return typeMap[props.fileType] || '文件'
})

// 允许的文件类型配置
const allowedExtensions = computed(() => props.fileType === 'document'
  ? [...uploadOptions.document, ...uploadOptions.image]
  : (uploadOptions[props.fileType] || [...uploadOptions.document, ...uploadOptions.image, ...uploadOptions.video]))
const effectiveMaxSize = computed(() => Math.min(props.maxSize, uploadOptions.maxFileSize / 1024 / 1024))
const computedAccept = computed(() => props.accept || allowedExtensions.value.join(','))

// 提示文本
const tipText = computed(() => {
  const sizeText = `单个文件不超过 ${effectiveMaxSize.value}MB`
  const limitText = props.limit > 1 ? `，最多上传 ${props.limit} 个` : ''

  let typeText = ''
  if (props.fileType === 'image') {
    typeText = '支持 JPG、PNG、GIF、BMP、WEBP、ICO 等图片格式'
  } else if (props.fileType === 'document') {
    typeText = '支持 PDF、DOC、DOCX、XLS、XLSX、PPT、PPTX、TXT 等文档格式，以及 JPG、PNG、GIF 等图片格式'
  } else if (props.fileType === 'video') {
    typeText = '支持 MP4、AVI、MOV、WMV、FLV、MKV、WEBM 等视频格式'
  }

  return `${typeText}，${sizeText}${limitText}`
})

/**
 * 初始化文件列表
 */
let fileListRevision = 0
const initFileList = async () => {
  const revision = ++fileListRevision
  const values = [...(props.modelValue || [])]
  if (props.returnType === 'id') {
    const ids = values.filter(id => typeof id === 'number')
    const responses = await Promise.allSettled(ids.map(id => getAttachmentDetail(id)))
    if (revision !== fileListRevision) return
    fileList.value = responses.map((result, index) => {
      const data = result.status === 'fulfilled' && result.value.success ? result.value.data : null
      return { name: data?.fileName || `附件 #${ids[index]}（暂无法读取）`, url: data?.fileUrl,
        id: ids[index], uid: -ids[index], status: 'success', response: data }
    })
  } else {
    fileList.value = values.map((url, index) => ({ name: getFileName(url), url, uid: -(index + 1), status: 'success' }))
  }
}

/**
 * 从URL获取文件名
 */
const getFileName = (url) => {
  if (!url) return ''
  const parts = url.split('/')
  return parts[parts.length - 1]
}

/**
 * 监听外部值变化
 */
watch(() => props.modelValue, (newVal) => {
  const currentValues = props.returnType === 'id' ? getIds() : getUrls()
  if (JSON.stringify(newVal) !== JSON.stringify(currentValues)) {
    initFileList()
  }
}, { deep: true })

/**
 * 上传前校验
 */
const beforeUpload = async (file) => {
  pendingUploads.add(file.uid)
  activeUploads.value++
  try {
    await loadUploadOptions()
    const error = validateUpload(file, allowedExtensions.value, effectiveMaxSize.value * 1024 * 1024)
    if (error) { finishUpload(file); ElMessage.error(error); return false }
    return true
  } catch {
    finishUpload(file)
    ElMessage.error('无法读取上传限制，请稍后重试')
    return false
  }
}

/**
 * 上传成功
 */
const handleSuccess = (response, file, fileListParam) => {
  finishUpload(file)
  if (response.success && response.data) {
    // 更新文件列表中的URL和ID
    const index = fileListParam.findIndex(f => f.uid === file.uid)
    if (index > -1) {
      fileListParam[index].url = response.data.fileUrl || response.data.url
      fileListParam[index].id = response.data.id
      fileListParam[index].response = response.data
      fileListParam[index].name = response.data.fileName || file.name
      fileListParam[index].status = 'success'
    }

    // 立即更新值
    updateValue()
    ElMessage.success('文件上传成功')
  } else {
    ElMessage.error(response.message || '文件上传失败')
    // 移除失败的文件
    const index = fileListParam.findIndex(f => f.uid === file.uid)
    if (index > -1) {
      fileListParam.splice(index, 1)
    }
  }
}

/**
 * 上传失败
 */
const handleError = (error, file, fileListParam) => {
  finishUpload(file)
  notifyError(error, '文件上传失败，请重试')

  // 移除失败的文件
  const index = fileListParam.findIndex(f => f.uid === file.uid)
  if (index > -1) {
    fileListParam.splice(index, 1)
  }
}

/**
 * 移除文件
 */
const handleRemove = (file, fileListParam) => {
  finishUpload(file)
  // 延迟更新，确保文件已从列表中移除
  nextTick(() => {
    updateValue()
  })
}

/**
 * 超出文件数量限制
 */
const handleExceed = () => {
  ElMessage.warning(`最多只能上传 ${props.limit} 个文件`)
}

/**
 * 预览文件
 */
const handlePreview = (file) => {
  if (props.fileType === 'image') {
    previewUrl.value = file.url
    previewVisible.value = true
  } else {
    // 非图片文件，尝试在新窗口打开
    if (file.url) {
      window.open(file.url, '_blank')
    }
  }
}

/**
 * 获取所有文件URL
 */
const getUrls = () => {
  return fileList.value
    .filter(file => file.status === 'success' && file.url)
    .map(file => file.url)
}

/**
 * 获取所有文件ID
 */
const getIds = () => {
  return fileList.value
    .filter(file => file.status === 'success' && file.id)
    .map(file => file.id)
}

/**
 * 更新值
 */
const updateValue = () => {
  let value
  if (props.returnType === 'id') {
    value = getIds()
  } else {
    value = getUrls()
  }
  emit('update:modelValue', value)
  emit('change', value)
}

/**
 * 清空文件列表
 */
const clearFiles = () => {
  fileList.value = []
  updateValue()
}

/**
 * 手动触发上传
 */
const submit = () => {
  if (uploadRef.value) {
    uploadRef.value.submit()
  }
}

// 暴露方法给父组件
defineExpose({
  clearFiles,
  submit,
  getUrls,
  getIds
})

// 组件挂载时初始化
onMounted(() => {
  loadUploadOptions().catch(() => {})
  initFileList()
})
</script>

<style scoped>
.file-upload {
  width: 100%;
}

:deep(.el-upload__tip) {
  margin-top: 8px;
  font-size: 12px;
  color: #909399;
  line-height: 1.5;
}

:deep(.el-upload-list__item) {
  transition: all 0.3s;
}

:deep(.el-upload-list__item:hover) {
  background-color: #f5f7fa;
}

/* 当达到上传限制时，隐藏上传按钮 */
.hide-upload-btn :deep(.el-upload--picture-card) {
  display: none !important;
}

.hide-upload-btn :deep(.el-upload) {
  display: none !important;
}
</style>

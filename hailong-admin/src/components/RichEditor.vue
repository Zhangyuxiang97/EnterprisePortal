<template>
  <div class="rich-editor">
    <Toolbar
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      :mode="mode"
      class="toolbar"
    />
    <Editor
      v-model="valueHtml"
      :defaultConfig="editorConfig"
      :mode="mode"
      :style="{ height: height + 'px' }"
      class="editor"
      @onCreated="handleCreated"
      @onChange="handleChange"
    />
  </div>
</template>

<script setup>
import { ref, shallowRef, watch, onBeforeUnmount, computed } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import '@wangeditor/editor/dist/css/style.css'
import { ElMessage } from 'element-plus'
import { tokenUtils } from '@/utils/auth'
import { API_CONFIG } from '@/config/api.config'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  mode: {
    type: String,
    default: 'default' // default 或 simple
  },
  placeholder: {
    type: String,
    default: '请输入内容...'
  },
  height: {
    type: Number,
    default: 400
  },
  // 是否禁用
  disabled: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

// 编辑器实例
const editorRef = shallowRef()

// 内容 HTML
const valueHtml = ref(props.modelValue)

// 工具栏配置
const toolbarConfig = {
  excludeKeys: props.disabled ? ['uploadImage', 'uploadVideo'] : []
}

// 上传请求头
const uploadHeaders = computed(() => {
  const token = tokenUtils.getToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
})

// 编辑器配置
const editorConfig = {
  placeholder: props.placeholder,
  readOnly: props.disabled,
  MENU_CONF: {
    // 配置上传图片
    uploadImage: {
      server: `${API_CONFIG.baseURL}/attachments/upload`,
      fieldName: 'file',
      maxFileSize: 5 * 1024 * 1024, // 5M
      allowedFileTypes: ['image/*'],
      headers: uploadHeaders.value,
      
      // 上传前的钩子
      onBeforeUpload(file) {
        console.log('上传图片前:', file)
        return file
      },
      
      // 自定义插入图片
      customInsert(res, insertFn) {
        console.log('图片上传响应:', res)
        if (res.success && res.data) {
          const url = res.data.fileUrl || res.data.url
          const alt = res.data.fileName || res.data.name || ''
          insertFn(url, alt, url)
          ElMessage.success('图片上传成功')
        } else {
          ElMessage.error(res.message || '图片上传失败')
        }
      },
      
      // 上传错误
      onError(file, err, res) {
        console.error('图片上传失败:', err, res)
        ElMessage.error('图片上传失败，请重试')
      }
    },
    
    // 配置上传视频
    uploadVideo: {
      server: `${API_CONFIG.baseURL}/attachments/upload`,
      fieldName: 'file',
      maxFileSize: 100 * 1024 * 1024, // 100M
      allowedFileTypes: ['video/*'],
      headers: uploadHeaders.value,
      
      // 上传前的钩子
      onBeforeUpload(file) {
        console.log('上传视频前:', file)
        return file
      },
      
      // 自定义插入视频
      customInsert(res, insertFn) {
        console.log('视频上传响应:', res)
        if (res.success && res.data) {
          const url = res.data.fileUrl || res.data.url
          const poster = res.data.poster || ''
          insertFn(url, poster)
          ElMessage.success('视频上传成功')
        } else {
          ElMessage.error(res.message || '视频上传失败')
        }
      },
      
      // 上传错误
      onError(file, err, res) {
        console.error('视频上传失败:', err, res)
        ElMessage.error('视频上传失败，请重试')
      }
    },
    
    // 配置插入链接
    insertLink: {
      checkLink: (text, url) => {
        // 自定义校验链接
        if (!url) {
          return '链接不能为空'
        }
        if (!/^https?:\/\/.+/.test(url)) {
          return '链接格式不正确，必须以 http:// 或 https:// 开头'
        }
        return true
      }
    },
    
    // 配置代码高亮
    codeSelectLang: {
      codeLangs: [
        { text: 'CSS', value: 'css' },
        { text: 'HTML', value: 'html' },
        { text: 'JavaScript', value: 'javascript' },
        { text: 'TypeScript', value: 'typescript' },
        { text: 'Java', value: 'java' },
        { text: 'Python', value: 'python' },
        { text: 'C#', value: 'csharp' },
        { text: 'C++', value: 'cpp' },
        { text: 'Go', value: 'go' },
        { text: 'SQL', value: 'sql' },
        { text: 'JSON', value: 'json' },
        { text: 'XML', value: 'xml' },
        { text: 'Markdown', value: 'markdown' }
      ]
    }
  }
}

/**
 * 编辑器创建完成
 */
const handleCreated = (editor) => {
  editorRef.value = editor
  
  // 如果是禁用状态，设置为只读
  if (props.disabled) {
    editor.disable()
  }
}

/**
 * 内容变化
 */
const handleChange = (editor) => {
  const html = editor.getHtml()
  emit('update:modelValue', html)
  emit('change', html)
}

/**
 * 监听外部值变化
 */
watch(() => props.modelValue, (newVal) => {
  if (newVal !== valueHtml.value) {
    valueHtml.value = newVal
  }
})

/**
 * 监听禁用状态变化
 */
watch(() => props.disabled, (newVal) => {
  const editor = editorRef.value
  if (editor) {
    if (newVal) {
      editor.disable()
    } else {
      editor.enable()
    }
  }
})

/**
 * 获取编辑器内容（HTML）
 */
const getHtml = () => {
  const editor = editorRef.value
  return editor ? editor.getHtml() : ''
}

/**
 * 获取编辑器内容（纯文本）
 */
const getText = () => {
  const editor = editorRef.value
  return editor ? editor.getText() : ''
}

/**
 * 设置编辑器内容
 */
const setHtml = (html) => {
  const editor = editorRef.value
  if (editor) {
    editor.setHtml(html)
  }
}

/**
 * 清空编辑器内容
 */
const clear = () => {
  const editor = editorRef.value
  if (editor) {
    editor.clear()
  }
}

/**
 * 聚焦编辑器
 */
const focus = () => {
  const editor = editorRef.value
  if (editor) {
    editor.focus()
  }
}

/**
 * 组件销毁时，销毁编辑器
 */
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor) {
    editor.destroy()
  }
})

// 暴露方法给父组件
defineExpose({
  getHtml,
  getText,
  setHtml,
  clear,
  focus
})
</script>

<style scoped>
.rich-editor {
  border: 1px solid #ccc;
  border-radius: 4px;
  overflow: hidden;
}

.toolbar {
  border-bottom: 1px solid #ccc;
  background-color: #fff;
}

.editor {
  overflow-y: auto;
}

:deep(.w-e-text-container) {
  background-color: #fff;
}

:deep(.w-e-text-placeholder) {
  color: #999;
  font-style: normal;
}

:deep(.w-e-text-container [data-slate-editor]) {
  padding: 15px;
  line-height: 1.8;
}

:deep(.w-e-text-container p) {
  margin: 10px 0;
}

:deep(.w-e-text-container img) {
  max-width: 100%;
  height: auto;
}

:deep(.w-e-text-container video) {
  max-width: 100%;
  height: auto;
}

:deep(.w-e-text-container pre) {
  background-color: #f5f5f5;
  padding: 10px;
  border-radius: 4px;
  overflow-x: auto;
}

:deep(.w-e-text-container code) {
  background-color: #f5f5f5;
  padding: 2px 6px;
  border-radius: 3px;
  font-family: 'Courier New', Courier, monospace;
}

:deep(.w-e-text-container blockquote) {
  border-left: 4px solid #ddd;
  padding-left: 15px;
  margin: 15px 0;
  color: #666;
}

:deep(.w-e-text-container table) {
  border-collapse: collapse;
  width: 100%;
  margin: 15px 0;
}

:deep(.w-e-text-container table td,
.w-e-text-container table th) {
  border: 1px solid #ddd;
  padding: 8px;
}

:deep(.w-e-text-container table th) {
  background-color: #f5f5f5;
  font-weight: bold;
}
</style>

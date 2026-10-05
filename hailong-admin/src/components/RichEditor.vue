<template>
  <div class="rich-editor">
    <div class="editor-actions"><span>正文编辑</span><el-button text size="small" @click="previewVisible = true">预览正文</el-button></div>
    <el-dialog v-model="previewVisible" title="正文预览" width="min(960px, 94vw)" append-to-body>
      <iframe title="正文预览" :srcdoc="previewDocument" sandbox="" style="width: 100%; height: 65vh; border: 0;" />
    </el-dialog>
    <el-alert v-if="editorError" :title="editorError" type="error" :closable="false" />
    <Toolbar
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      :mode="mode"
      class="toolbar"
    />
    <Editor
      :defaultHtml="''"
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
import { notifyError } from '@/utils/errors'
import { ref, shallowRef, watch, onBeforeUnmount, computed, nextTick, inject } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import '@wangeditor/editor/dist/css/style.css'
import { ElMessage } from 'element-plus'
import { uploadOptions, loadUploadOptions, validateUpload } from '@/utils/upload'
import { uploadAttachment } from '@/api/attachment'

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
const editorError = ref('')
const previewVisible = ref(false)
const activeUploads = inject('editorActiveUploads', ref(0))
const previewDocument = computed(() => `<!doctype html><html lang="zh-CN"><head><meta charset="utf-8"><meta http-equiv="Content-Security-Policy" content="default-src 'none'; img-src http: https: data:; media-src http: https:; style-src 'unsafe-inline'; base-uri 'none'; form-action 'none'"><style>body{font:16px/1.85 system-ui,sans-serif;color:#273548;padding:24px;overflow-wrap:anywhere}img,video{max-width:100%;height:auto}table{border-collapse:collapse;max-width:100%}td,th{border:1px solid #ddd;padding:8px}a{color:#245b85}</style></head><body>${props.modelValue || '<p>暂无正文</p>'}</body></html>`)
let lastHtml = ''
let applying = false
let revision = 0

// 工具栏配置
const toolbarConfig = {
  excludeKeys: props.disabled ? ['uploadImage', 'uploadVideo'] : []
}

async function uploadEditorFile(file, category) {
  activeUploads.value++
  try {
    await loadUploadOptions()
    const error = validateUpload(file, uploadOptions[category])
    if (error) throw new Error(error)
    const data = new FormData()
    data.append('file', file)
    data.append('category', category)
    const res = await uploadAttachment(data)
    if (!res.success) throw new Error(res.message || '上传失败')
    return res.data
  } catch (error) { notifyError(error, error.message || '上传失败，请重试') }
  finally { activeUploads.value-- }
}

// 编辑器配置
const editorConfig = {
  placeholder: props.placeholder,
  readOnly: props.disabled,
  MENU_CONF: {
    uploadImage: {
      maxFileSize: Number.MAX_SAFE_INTEGER,
      allowedFileTypes: ['image/*'],
      async customUpload(file, insertFn) {
        const attachment = await uploadEditorFile(file, 'image')
        if (attachment) insertFn(attachment.fileUrl, attachment.fileName, '')
      }
    },
    uploadVideo: {
      maxFileSize: Number.MAX_SAFE_INTEGER,
      allowedFileTypes: ['video/*'],
      async customUpload(file, insertFn) {
        const attachment = await uploadEditorFile(file, 'video')
        if (attachment) insertFn(attachment.fileUrl, '')
      }
    },

    // 配置插入链接
    insertLink: {
      checkLink: (text, url) => {
        // 自定义校验链接
        if (!url) {
          return '链接不能为空'
        }
        if (!/^https?:\/\/.+/.test(url) && !/^\/uploads\/[^\s]+$/.test(url)) {
          return '请输入 http(s) 链接或站内 /uploads/ 附件路径'
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
// 先完成空编辑器的插件初始化，再导入历史 HTML。直接作为初始 html 会触发 Slate 路径失效。
const applyHtml = async (html) => {
  const current = ++revision
  applying = true
  await nextTick()
  const editor = editorRef.value
  if (!editor || current !== revision) return
  try {
    editor.setHtml(html || '')
    lastHtml = editor.getHtml()
    editorError.value = ''
  } catch {
    editorError.value = '正文载入失败，请关闭后重试；原始内容尚未改动。'
  } finally {
    await nextTick()
    if (current === revision) applying = false
  }
}
const handleCreated = (editor) => {
  editorRef.value = editor
  applyHtml(props.modelValue)
}
const handleChange = (editor) => {
  const html = editor.getHtml()
  if (applying || html === lastHtml || editorError.value) return
  lastHtml = html
  emit('update:modelValue', html)
  emit('change', html)
}
watch(() => props.modelValue, (html) => {
  if (editorRef.value && html !== lastHtml) applyHtml(html)
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
    applyHtml(html)
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
  ++revision
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
.editor-actions { display: flex; align-items: center; justify-content: space-between; padding: 4px 12px; background: #f7f9fb; color: #64748b; font-size: 12px; }
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

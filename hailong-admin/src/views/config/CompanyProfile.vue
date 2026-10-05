<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <span>企业简介</span>
      </template>

      <el-form :model="formData" :rules="rules" ref="formRef" label-width="120px" v-loading="loading">
        <el-form-item label="标题" prop="title">
          <el-input v-model="formData.title" placeholder="请输入标题" maxlength="200" show-word-limit />
        </el-form-item>

        <el-form-item label="企业图片">
          <FileUpload
            v-model="formData.imageIds"
            file-type="image"
            :limit="1"
            :multiple="false"
            list-type="picture-card"
            return-type="id"
            related-type="config"
          />
          <div class="form-tip">建议尺寸：1200x800，仅支持上传1张图片</div>
        </el-form-item>

        <el-form-item label="企业简介" prop="content">
          <RichEditor v-model="formData.content" :height="400" />
        </el-form-item>

        <el-form-item label="企业亮点">
          <div class="highlights-container">
            <div v-for="(highlight, index) in formData.highlights" :key="index" class="highlight-item">
              <el-input
                v-model="formData.highlights[index]"
                placeholder="请输入企业亮点"
                maxlength="20"
                show-word-limit
              >
                <template #append>
                  <el-button
                    :icon="Delete"
                    @click="removeHighlight(index)"
                    :disabled="formData.highlights.length <= 1"
                  />
                </template>
              </el-input>
            </div>
            <el-button
              type="primary"
              :icon="Plus"
              @click="addHighlight"
              :disabled="formData.highlights.length >= 8"
              style="width: 100%"
            >
              添加亮点
            </el-button>
          </div>
          <div class="form-tip">企业亮点将显示在首页企业简介区域，建议4-8个，每个不超过20字</div>
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

        <el-form-item>
          <el-button type="primary" @click="handleSubmit" :loading="submitting" :disabled="activeUploads > 0 || loading">保存</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { useEditorForm } from '@/composables/useEditorForm'
import { richContentRule } from '@/utils/form'
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import { systemConfigApi } from '@/api'
import RichEditor from '@/components/RichEditor.vue'
import FileUpload from '@/components/FileUpload.vue'

const loading = ref(false)
const submitting = ref(false)
const formRef = ref(null)

const formData = reactive({
  title: '',
  content: '',
  imageIds: [],
  highlights: [],
  status: true
})
const { markSaved, activeUploads } = useEditorForm(formData, ref(true), submitting)


const rules = {
  title: [
    { required: true, message: '请输入标题', trigger: 'blur' },
    { min: 2, max: 200, message: '标题长度在 2 到 200 个字符', trigger: 'blur' }
  ],
  content: [richContentRule]
}

// 添加亮点
const addHighlight = () => {
  if (formData.highlights.length < 8) {
    formData.highlights.push('')
  }
}

// 删除亮点
const removeHighlight = (index) => {
  if (formData.highlights.length > 1) {
    formData.highlights.splice(index, 1)
  }
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await systemConfigApi.companyProfile.get()
    if (res.success && res.data) {
      Object.assign(formData, {
        version: res.data.version,
        title: res.data.title || '',
        content: res.data.content || '',
        imageIds: res.data.imageIds || [],
        highlights: res.data.highlights || [],
        status: res.data.status ?? true
      })
    }
  } catch (error) {

    notifyError(error, '加载数据失败，请稍后重试')
  } finally {
    markSaved()
    loading.value = false
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  // 过滤掉空的亮点
  const filteredHighlights = formData.highlights.filter(h => h && h.trim())
  if (filteredHighlights.length === 0) {
    ElMessage.warning('请至少添加一个企业亮点')
    return
  }

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    submitting.value = true
    try {
      const submitData = {
        ...formData,
        highlights: filteredHighlights
      }
      const res = await systemConfigApi.companyProfile.update(submitData)
      if (res.success) {
        ElMessage.success('保存成功')
        loadData()
      } else {
        ElMessage.error(res.message || '保存失败')
      }
    } catch (error) {

      notifyError(error, '保存失败，请稍后重试')
    } finally {
      submitting.value = false
    }
  })
}

const handleReset = async () => {
  try { await ElMessageBox.confirm('重新加载将放弃未保存的修改，是否继续？', '重新加载', { type: 'warning' }); await loadData() } catch {}
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.page-container {
  padding: 0;
}

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}

.highlights-container {
  width: 100%;
}

.highlight-item {
  margin-bottom: 12px;
}

.highlight-item:last-of-type {
  margin-bottom: 16px;
}
</style>

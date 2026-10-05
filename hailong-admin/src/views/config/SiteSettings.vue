<template>
  <el-card v-loading="loading">
    <template #header>
      <div class="settings-heading">
        <span>门户站点设置</span>
        <span class="settings-tip">保存后刷新门户即可看到更新</span>
        <span v-if="form?.updatedAt" class="settings-tip">最近更新：{{ new Date(form.updatedAt).toLocaleString() }}</span>
      </div>
    </template>

    <el-alert v-if="loadFailed" title="读取设置失败，请重新加载后再编辑。" type="error" :closable="false" show-icon />
    <el-form v-if="form" ref="formRef" :model="form" :rules="rules" label-width="125px" :disabled="saving">
      <el-tabs v-model="activeTab">
        <el-tab-pane label="公司信息" name="company">
          <el-form-item v-for="field in companyFields" :key="field.key" :label="field.label" :prop="`company.${field.key}`">
            <el-input v-model="form.company[field.key]" :maxlength="field.max" :type="field.type || 'text'" :rows="3" show-word-limit />
          </el-form-item>
        </el-tab-pane>

        <el-tab-pane label="联系方式" name="contact">
          <el-form-item label="联系电话" prop="contact.phone">
            <el-input v-model="form.contact.phone" maxlength="50" />
          </el-form-item>
          <el-form-item label="联系邮箱" prop="contact.email">
            <el-input v-model="form.contact.email" maxlength="120" />
          </el-form-item>
          <el-form-item v-for="field in addressFields" :key="field.key" :label="field.label" :prop="`contact.address.${field.key}`">
            <el-input v-model="form.contact.address[field.key]" :maxlength="field.max" />
          </el-form-item>
          <el-form-item label="工作日时间" prop="contact.workingHours.weekdays">
            <el-input v-model="form.contact.workingHours.weekdays" maxlength="100" placeholder="例如：周一至周五 9:00–17:20" />
          </el-form-item>
          <el-form-item label="周末/节假日" prop="contact.workingHours.weekend">
            <el-input v-model="form.contact.workingHours.weekend" maxlength="100" placeholder="例如：休息" />
          </el-form-item>
          <p class="settings-tip">首页、联系我们、专家库和页脚统一使用这里的联系方式。</p>
        </el-tab-pane>

        <el-tab-pane label="地图与交通" name="transport">
          <el-form-item label="高德浏览器 Key" prop="contact.map.apiKey">
            <el-input v-model="form.contact.map.apiKey" maxlength="200" />
            <div class="settings-tip">此 Key 用于浏览器地图，请使用已限制网站域名的 Key。</div>
          </el-form-item>
          <el-form-item label="地图经度">
            <el-input-number v-model="form.contact.map.longitude" :min="-180" :max="180" :precision="6" :step="0.000001" />
          </el-form-item>
          <el-form-item label="地图纬度">
            <el-input-number v-model="form.contact.map.latitude" :min="-90" :max="90" :precision="6" :step="0.000001" />
          </el-form-item>
          <el-form-item label="地图缩放级别">
            <el-input-number v-model="form.contact.map.zoom" :min="1" :max="20" :precision="0" />
          </el-form-item>
          <el-divider content-position="left">地铁出行</el-divider>
          <el-form-item label="显示地铁指引"><el-switch v-model="form.transportation.metro.enabled" /></el-form-item>
          <div v-for="(line, index) in form.transportation.metro.lines" :key="index" class="settings-list-item">
            <el-row :gutter="12">
              <el-col :xs="24" :sm="12"><el-form-item label="线路"><el-input v-model="line.line" maxlength="80" /></el-form-item></el-col>
              <el-col :xs="24" :sm="12"><el-form-item label="站点"><el-input v-model="line.station" maxlength="80" /></el-form-item></el-col>
              <el-col :xs="24" :sm="12"><el-form-item label="出口"><el-input v-model="line.exit" maxlength="40" /></el-form-item></el-col>
              <el-col :xs="24" :sm="12"><el-form-item label="步行距离"><el-input v-model="line.walkingDistance" maxlength="80" /></el-form-item></el-col>
            </el-row>
            <el-button type="danger" plain @click="form.transportation.metro.lines.splice(index, 1)">删除此线路</el-button>
          </div>
          <el-button :disabled="form.transportation.metro.lines.length >= 20" @click="addMetroLine">添加地铁线路</el-button>
          <el-divider content-position="left">公交出行</el-divider>
          <el-form-item label="显示公交指引"><el-switch v-model="form.transportation.bus.enabled" /></el-form-item>
          <el-form-item label="公交线路"><el-input v-model="busRoutesText" type="textarea" :rows="3" placeholder="每行一条公交线路" /></el-form-item>
          <el-form-item label="公交站点"><el-input v-model="form.transportation.bus.station" maxlength="100" /></el-form-item>
          <el-form-item label="公交说明"><el-input v-model="form.transportation.bus.description" maxlength="300" /></el-form-item>
          <el-divider content-position="left">自驾与周边</el-divider>
          <el-form-item label="显示自驾指引"><el-switch v-model="form.transportation.driving.enabled" /></el-form-item>
          <el-form-item label="导航搜索名称"><el-input v-model="form.transportation.driving.navigation" maxlength="200" /></el-form-item>
          <el-form-item label="停车说明"><el-input v-model="form.transportation.driving.parking" maxlength="200" /></el-form-item>
          <el-form-item label="周边地标"><el-input v-model="landmarksText" type="textarea" :rows="3" placeholder="每行一个地标" /></el-form-item>
        </el-tab-pane>

        <el-tab-pane label="常见问题" name="faq">
          <div v-for="(faq, index) in form.faqs" :key="index" class="settings-list-item">
            <el-form-item :label="`问题 ${index + 1}`" :prop="`faqs.${index}.question`" :rules="requiredRule">
              <el-input v-model="faq.question" maxlength="300" show-word-limit />
            </el-form-item>
            <el-form-item label="回答" :prop="`faqs.${index}.answer`" :rules="requiredRule">
              <el-input v-model="faq.answer" type="textarea" :rows="3" maxlength="2000" show-word-limit />
            </el-form-item>
            <el-button type="danger" plain @click="form.faqs.splice(index, 1)">删除此问题</el-button>
          </div>
          <el-button :disabled="form.faqs.length >= 30" @click="form.faqs.push({ question: '', answer: '' })">添加常见问题</el-button>
        </el-tab-pane>
      </el-tabs>
      <div class="settings-actions">
        <el-button type="primary" :loading="saving" @click="save">保存设置</el-button>
        <el-button @click="reload">重新加载</el-button>
      </div>
    </el-form>
    <el-button v-else-if="loadFailed" @click="load">重新加载</el-button>
  </el-card>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useEditorLeaveGuard } from '@/composables/useEditorLeaveGuard'
import { ElMessage, ElMessageBox } from 'element-plus'
import { siteSettings } from '@/api/systemConfig'

const loading = ref(false)
const saving = ref(false)
const loadFailed = ref(false)
const form = ref(null)
const formRef = ref(null)
const activeTab = ref('company')
const busRoutesText = ref('')
const landmarksText = ref('')
const savedSnapshot = ref('')
const snapshot = () => JSON.stringify([form.value, busRoutesText.value, landmarksText.value])
const dirty = computed(() => Boolean(form.value && savedSnapshot.value && snapshot() !== savedSnapshot.value))
const confirmDiscard = async () => {
  if (!dirty.value) return true
  try {
    await ElMessageBox.confirm('有尚未保存的修改，是否放弃这些修改？', '未保存的修改', {
      confirmButtonText: '放弃修改', cancelButtonText: '继续编辑', type: 'warning'
    })
    return true
  } catch { return false }
}
const reload = async () => { if (await confirmDiscard()) await load() }
useEditorLeaveGuard(() => saving.value ? false : confirmDiscard())
const beforeUnload = event => { if (dirty.value) { event.preventDefault(); event.returnValue = '' } }
const companyFields = [
  { key: 'fullName', label: '公司全称', max: 120 },
  { key: 'slogan', label: '首页标语', max: 200 },
  { key: 'description', label: '首页简短介绍', max: 1000, type: 'textarea' }
]
const addressFields = [
  { key: 'fullAddress', label: '完整地址', max: 400 }
]
const requiredRule = [{ required: true, whitespace: true, message: '请填写此项', trigger: 'blur' }]
const rules = {
  'company.fullName': requiredRule,
  'company.slogan': requiredRule,
  'company.description': requiredRule,
  'contact.phone': requiredRule,
  'contact.email': [...requiredRule, { type: 'email', message: '请输入有效邮箱', trigger: 'blur' }],
  'contact.address.fullAddress': requiredRule,
  'contact.workingHours.weekdays': requiredRule,
  'contact.workingHours.weekend': requiredRule,
  'contact.map.apiKey': requiredRule
}

const load = async () => {
  loading.value = true
  loadFailed.value = false
  try {
    const response = await siteSettings.get()
    if (!response.success || !response.data) throw new Error('设置内容为空')
    form.value = response.data
    busRoutesText.value = response.data.transportation.bus.routes.join('\n')
    landmarksText.value = response.data.transportation.landmarks.join('\n')
    savedSnapshot.value = snapshot()
  } catch {
    form.value = null
    loadFailed.value = true
  } finally {
    loading.value = false
  }
}

const addMetroLine = () => {
  form.value.transportation.metro.lines.push({ line: '', station: '', exit: '', walkingDistance: '' })
}

const toLines = text => text.split(/\r?\n/).map(line => line.trim()).filter(Boolean)

const save = async () => {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) {
    ElMessage.warning('请检查各页签的必填字段')
    return
  }
  const data = JSON.parse(JSON.stringify(form.value))
  data.transportation.bus.routes = toLines(busRoutesText.value)
  data.transportation.landmarks = toLines(landmarksText.value)
  if (data.transportation.bus.routes.length > 50 || data.transportation.bus.routes.some(x => x.length > 50) ||
      data.transportation.landmarks.length > 30 || data.transportation.landmarks.some(x => x.length > 100)) {
    ElMessage.warning('公交线路最多 50 条且每条不超过 50 字；地标最多 30 个且每个不超过 100 字')
    return
  }
  saving.value = true
  try {
    const response = await siteSettings.update(data)
    form.value = response.data
    busRoutesText.value = response.data.transportation.bus.routes.join('\n')
    landmarksText.value = response.data.transportation.landmarks.join('\n')
    savedSnapshot.value = snapshot()
    ElMessage.success('保存成功，刷新门户即可看到更新')
  } catch {
    // 通用请求拦截器已展示失败原因，保留表单以便重试。
  } finally {
    saving.value = false
  }
}

onMounted(() => { window.addEventListener('beforeunload', beforeUnload); load() })
onUnmounted(() => window.removeEventListener('beforeunload', beforeUnload))
</script>

<style scoped>
.settings-heading { display: flex; flex-wrap: wrap; gap: 12px; align-items: center; }
.settings-tip { color: #606266; font-size: 13px; line-height: 1.6; }
.settings-list-item { padding: 16px; border: 1px solid #e4e7ed; border-radius: 6px; margin-bottom: 16px; }
.settings-actions { border-top: 1px solid #e4e7ed; padding-top: 20px; margin-top: 24px; }
:deep(.el-input), :deep(.el-textarea) { max-width: 800px; }
</style>

<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>建设工程公告管理</span>
          <el-button v-if="canManage" type="primary" icon="Plus" @click="handleAdd">新增公告</el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <el-form :model="searchForm" inline class="search-form">
        <el-form-item label="关键词">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜索标题、招标人、中标人"
            clearable
            style="width: 220px;"
          />
        </el-form-item>
        <el-form-item label="公告类型">
          <el-select v-model="searchForm.type" placeholder="请选择" clearable style="width: 150px;">
            <el-option label="招标公告" value="bidding" />
            <el-option label="中标公告" value="result" />
            <el-option label="变更公告" value="correction" />
          </el-select>
        </el-form-item>
        <el-form-item label="项目区域">
          <RegionSelector
            ref="searchRegionSelectorRef"
            @change="handleRegionChange"
          />
        </el-form-item>
        <el-form-item label="时间范围">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="YYYY-MM-DD"
            style="width: 240px;"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">搜索</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="title" label="标题" min-width="200" show-overflow-tooltip />
        <el-table-column prop="noticeType" label="公告类型" width="100" align="center">
          <template #default="{ row }">
            {{ formatNoticeType(row.noticeType) }}
          </template>
        </el-table-column>
        <el-table-column prop="bidder" label="招标人" min-width="150" show-overflow-tooltip />
        <el-table-column prop="winner" label="中标人" min-width="150" show-overflow-tooltip />
        <el-table-column prop="budgetAmount" label="预算（万元）" width="125" align="right">
          <template #default="{ row }">{{ row.budgetAmount ?? '—' }}</template>
        </el-table-column>
        <el-table-column prop="awardAmount" label="中标（万元）" width="125" align="right">
          <template #default="{ row }">{{ row.awardAmount ?? '—' }}</template>
        </el-table-column>
        <el-table-column prop="projectRegion" label="项目区域" width="180" align="center" show-overflow-tooltip />
        <el-table-column prop="publishTime" label="发布时间" width="110" align="center">
          <template #default="{ row }">
            {{ formatDate(row.publishTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="viewCount" label="访问量" width="80" align="center" />
        <el-table-column label="操作" width="150" fixed="right" align="center">
          <template #default="{ row }">
            <el-button type="primary" size="small" link @click="handleEdit(row)">{{ canManage ? '编辑' : '查看' }}</el-button>
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
        @size-change="loadData"
        @current-change="loadData"
        style="margin-top: 20px; justify-content: flex-end;"
      />
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
    v-model="dialogVisible"
      class="editor-dialog"
      :before-close="beforeEditorClose"
    :title="isEdit ? '编辑公告' : '新增公告'"
    width="900px"
    :close-on-click-modal="false"
  >
      <el-form :disabled="!canManage"
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="公告标题" prop="title">
          <el-input
            v-model="formData.title"
            placeholder="请输入公告标题（最多255个字符）"
            maxlength="255"
            show-word-limit
          />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="公告类型" prop="noticeType">
              <el-select v-model="formData.noticeType" placeholder="请选择公告类型" style="width: 100%;">
                <el-option label="招标公告" value="bidding" />
                <el-option label="中标公告" value="result" />
                <el-option label="变更公告" value="correction" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="项目区域" prop="regionPath">
              <RegionCascader
                v-if="dialogVisible"
                ref="regionCascaderRef"
                v-model="formData.regionPath"
                placeholder="可选到已确认的省或市，未知可留空"
                @change="handleFormRegionChange"
                :width="'100%'"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="招标人" prop="bidder">
              <el-input
                v-model="formData.bidder"
                placeholder="请输入招标人名称（最多255个字符）"
                maxlength="255"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="formData.noticeType === 'result'">
            <el-form-item label="中标人" prop="winner">
              <el-input
                v-model="formData.winner"
                placeholder="请输入中标人名称（最多255个字符）"
                maxlength="255"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="预算金额" prop="budgetAmount">
              <el-input-number
                v-model="formData.budgetAmount"
                :min="0"
                :precision="2"
                placeholder="请输入预算金额(万元)"
                style="width: 100%;"
                :controls="false"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="截止时间" prop="deadline">
              <el-date-picker
                v-model="formData.deadline"
                type="datetime"
                placeholder="选择截止时间"
                value-format="YYYY-MM-DD HH:mm:ss"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item v-if="formData.noticeType === 'result'" label="中标金额" prop="awardAmount">
          <el-input-number v-model="formData.awardAmount" :min="0" :precision="2" :controls="false" placeholder="中标/成交金额（万元）" style="width: 100%;" />
        </el-form-item>

        <el-form-item label="发布时间" prop="publishTime">
          <el-date-picker
            v-model="formData.publishTime"
            type="datetime"
            placeholder="选择发布时间"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%;"
          />
        </el-form-item>

        <el-form-item label="公告内容" prop="content">
          <RichEditor :disabled="!canManage" v-model="formData.content" />
        </el-form-item>

        <el-form-item label="附件" prop="attachmentIds">
          <FileUpload
            v-model="formData.attachmentIds"
            :limit="10"
            list-type="text"
            return-type="id"
            related-type="announcement"
            :related-id="formData.id"
          />
          <div class="form-tip">支持上传PDF、Word、Excel等文档，最多10个附件</div>
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="handleCancel">取消</el-button>
        <el-button v-if="canManage" type="primary" @click="handleSubmit" :loading="submitting" :disabled="activeUploads > 0">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { useContentSubmit } from '@/composables/useContentSubmit'
import { notifyError } from '@/utils/errors'
import { createLatestRequest } from '@/utils/latestRequest'
import { useEditorForm } from '@/composables/useEditorForm'
import { localDateTime, richContentRule } from '@/utils/form'
import { ref, reactive, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { announcementApi } from '@/api'
import RichEditor from '@/components/RichEditor.vue'
import FileUpload from '@/components/FileUpload.vue'
import RegionCascader from '@/components/RegionCascader.vue'
import RegionSelector from '@/components/RegionSelector.vue'
import { formatDate } from '@/utils/date'

// 公告类型映射
const noticeTypeMap = {
  'bidding': '招标公告',
  'result': '中标公告',
  'correction': '变更公告'
}

// 日期范围
const dateRange = ref([])

// 搜索表单
const searchForm = reactive({
  keyword: '',
  type: '',
  provinceCode: '',
  cityCode: '',
  districtCode: '',
  startDate: '',
  endDate: ''
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
const regionCascaderRef = ref(null)
const searchRegionSelectorRef = ref(null)

// 表单数据
const formData = reactive({
  id: null,
  title: '',
  businessType: 'CONSTRUCTION', // 固定为建设工程
  noticeType: '',
  content: '',
  bidder: '', // 招标人
  winner: '', // 中标人
  regionPath: [],
  province: '',
  city: '',
  district: '',
  projectRegion: '',
  publishTime: '',
  budgetAmount: null,
  awardAmount: null,
  deadline: '',
  attachmentIds: [], status: 1, isTop: false
})
const { markSaved, handleCancel, beforeEditorClose, canManage, activeUploads } = useEditorForm(formData, dialogVisible, submitting)


// 表单验证规则
const formRules = {
  title: [
    { required: true, message: '请输入公告标题', trigger: 'blur' },
    { max: 500, message: '标题长度不能超过500个字符', trigger: 'blur' }
  ],
  noticeType: [
    { required: true, message: '请选择公告类型', trigger: 'change' }
  ],
  content: [richContentRule],
  publishTime: [
    { required: true, message: '请选择发布日期', trigger: 'change' }
  ]
}

/**
 * 格式化公告类型
 */
const formatNoticeType = (type) => {
  return noticeTypeMap[type] || type
}

/**
 * 加载数据
 */
const listRequests = createLatestRequest()
const loadData = async () => {
  const requestId = listRequests.begin()
  loading.value = true
  try {
    // 处理时间范围
    if (dateRange.value && dateRange.value.length === 2) {
      searchForm.startDate = dateRange.value[0]
      searchForm.endDate = dateRange.value[1]
    } else {
      searchForm.startDate = ''
      searchForm.endDate = ''
    }

    const params = {
      keyword: searchForm.keyword || undefined,
      businessType: 'CONSTRUCTION', // 固定为建设工程
      noticeType: searchForm.type || undefined,
      province: searchForm.provinceCode || undefined,
      city: searchForm.cityCode || undefined,
      district: searchForm.districtCode || undefined,
      startDate: searchForm.startDate || undefined,
      endDate: searchForm.endDate || undefined,
      pageNumber: pagination.pageIndex,
      pageSize: pagination.pageSize
    }

    const res = await announcementApi.getAnnouncementList(params)
    if (!listRequests.isCurrent(requestId)) return

    if (res.success && res.data) {
      tableData.value = res.data.items || []
      pagination.total = res.data.totalCount || 0
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
 * 搜索区域变化
 */
const handleRegionChange = (regionInfo) => {
  // 根据选择的级别设置对应的区域代码
  searchForm.provinceCode = regionInfo.provinceCode || ''
  searchForm.cityCode = regionInfo.cityCode || ''
  searchForm.districtCode = regionInfo.districtCode || ''
}

/**
 * 表单区域变化
 */
const handleFormRegionChange = (regionInfo) => {
  formData.province = regionInfo.provinceCode
  formData.city = regionInfo.cityCode
  formData.district = regionInfo.districtCode
  // 组合完整地址：省 + 市 + 区
  const parts = [regionInfo.provinceName, regionInfo.cityName, regionInfo.districtName].filter(Boolean)
  formData.projectRegion = parts.join('')
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
  searchForm.keyword = ''
  searchForm.type = ''
  searchForm.provinceCode = ''
  searchForm.cityCode = ''
  searchForm.districtCode = ''
  dateRange.value = []
  // 重置区域选择器
  if (searchRegionSelectorRef.value) {
    searchRegionSelectorRef.value.reset()
  }
  handleSearch()
}

/**
 * 新增
 */
const handleAdd = () => {
  isEdit.value = false
  // 获取当前时间并格式化为 yyyy-MM-dd HH:mm:ss 格式
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const hours = String(now.getHours()).padStart(2, '0')
  const minutes = String(now.getMinutes()).padStart(2, '0')
  const seconds = String(now.getSeconds()).padStart(2, '0')
  const dateTimeString = `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`

  Object.assign(formData, {
    id: null,
    title: '',
    businessType: 'CONSTRUCTION',
    noticeType: '',
    content: '',
    bidder: '',
    winner: '',
    regionPath: [],
    province: '',
    city: '',
    district: '',
    projectRegion: '',
    publishTime: dateTimeString,
    budgetAmount: null,
    awardAmount: null,
    deadline: '',
    attachmentIds: [], status: 1, isTop: false
  })
  dialogVisible.value = true
}

/**
 * 编辑
 */
const handleEdit = async (row) => {
  isEdit.value = true
  try {
    const res = await announcementApi.getAnnouncementDetail(row.id)
    if (res.success && res.data) {
      // 先设置基本表单数据（不包括 regionPath）
      Object.assign(formData, {
        id: res.data.id,
        version: res.data.version,
        title: res.data.title,
        businessType: res.data.businessType,
        noticeType: res.data.noticeType,
        content: res.data.content,
        bidder: res.data.bidder || '',
        winner: res.data.winner || '',
        regionPath: [], // 先设置为空
        province: res.data.province || '',
        city: res.data.city || '',
        district: res.data.district || '',
        projectRegion: res.data.projectRegion || '',
        publishTime: res.data.publishTime,
        budgetAmount: res.data.budgetAmount,
        awardAmount: res.data.awardAmount,
        deadline: res.data.deadline || '',
        attachmentIds: res.data.attachmentIds || [],
        status: res.data.status, isTop: res.data.isTop
      })

      // 打开对话框
      dialogVisible.value = true

      // 等待 RegionCascader 组件挂载并加载数据
      await nextTick()

      const regionPath = await regionCascaderRef.value.resolvePath([
        res.data.provinceCode || res.data.province,
        res.data.cityCode || res.data.city,
        res.data.districtCode || res.data.district
      ])
      formData.regionPath = regionPath
      formData.province = regionPath[0] || ''
      formData.city = regionPath[1] || ''
      formData.district = regionPath[2] || ''
      await nextTick()
      markSaved()
    } else {
      ElMessage.error(res.message || '获取公告详情失败')
    }
  } catch (error) {

    notifyError(error, dialogVisible.value ? (error.message || '表单初始化失败，请关闭后重试') : '获取公告详情失败，请稍后重试')
  }
}

/**
 * 删除
 */
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除公告"${row.title}"吗？删除后将无法恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const res = await announcementApi.deleteAnnouncement(row.id)
    if (res.success) {
      ElMessage.success(res.message || '删除成功')
      // 如果当前页只有一条数据且不是第一页，则返回上一页
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
const handleSubmit = useContentSubmit({
  formRef, submitting, isEdit, formData,
  buildPayload: () => ({
      version: formData.version,
      title: formData.title,
      businessType: formData.businessType,
      noticeType: formData.noticeType,
      content: formData.content,
      bidder: formData.bidder || null,
      winner: formData.winner || null,
      province: formData.province || '',
      city: formData.city || '',
      district: formData.district || '',
      projectRegion: formData.projectRegion,
      publishTime: formData.publishTime,
      budgetAmount: formData.budgetAmount ?? null,
      awardAmount: formData.noticeType === 'result' ? (formData.awardAmount ?? null) : null,
      deadline: formData.deadline || null,
      attachmentIds: formData.attachmentIds,
      status: formData.status,
      isTop: formData.isTop
    }),
  create: announcementApi.createAnnouncement, update: announcementApi.updateAnnouncement,
  onSaved: () => { markSaved(); dialogVisible.value = false; return loadData() }
})

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
  color: #909399;
  margin-top: 4px;
  line-height: 1.5;
}
</style>

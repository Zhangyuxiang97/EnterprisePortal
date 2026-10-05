<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <span>系统日志</span>
      </template>

      <!-- 搜索表单 -->
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="操作类型">
          <el-select v-model="searchForm.action" placeholder="请选择操作类型" clearable style="width: 150px;">
            <el-option label="登录" value="LOGIN" />
            <el-option label="登出" value="LOGOUT" />
            <el-option label="创建" value="CREATE" />
            <el-option label="更新" value="UPDATE" />
            <el-option label="删除" value="DELETE" />
            <el-option label="查询" value="QUERY" />
            <el-option label="错误" value="ERROR" />
          </el-select>
        </el-form-item>
        <el-form-item label="操作人">
          <el-input v-model="searchForm.username" placeholder="请输入操作人" clearable style="width: 150px;" />
        </el-form-item>
        <el-form-item label="模块">
          <el-input v-model="searchForm.module" placeholder="请输入模块" clearable style="width: 150px;" />
        </el-form-item>
        <el-form-item label="IP地址">
          <el-input v-model="searchForm.ipAddress" placeholder="请输入IP地址" clearable style="width: 150px;" />
        </el-form-item>
        <el-form-item label="操作时间">
          <el-date-picker
            v-model="searchForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 240px;"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">搜索</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
          <el-button type="danger" icon="Delete" @click="handleClearLogs">清空日志</el-button>
        </el-form-item>
      </el-form>

      <!-- 数据表格 -->
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="action" label="操作类型" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getOperationTypeTag(row.action)" size="small">
              {{ getOperationTypeText(row.action) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="username" label="操作人" width="120" />
        <el-table-column prop="module" label="模块" width="120" />
        <el-table-column prop="description" label="操作描述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="ipAddress" label="IP地址" width="140" />
        <el-table-column prop="userAgent" label="浏览器" width="150" show-overflow-tooltip />
        <el-table-column prop="createdAt" label="操作时间" width="160" />
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" link @click="handleViewDetail(row)">详情</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="pagination.page"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="loadData"
        @current-change="loadData"
        class="pagination"
      />
    </el-card>

    <!-- 详情对话框 -->
    <el-dialog v-model="detailVisible" title="日志详情" width="800px">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="ID">{{ detailData.id }}</el-descriptions-item>
        <el-descriptions-item label="操作类型">
          <el-tag :type="getOperationTypeTag(detailData.action)" size="small">
            {{ getOperationTypeText(detailData.action) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="操作人">{{ detailData.username }}</el-descriptions-item>
        <el-descriptions-item label="用户ID">{{ detailData.userId }}</el-descriptions-item>
        <el-descriptions-item label="模块">{{ detailData.module }}</el-descriptions-item>
        <el-descriptions-item label="操作时间">{{ detailData.createdAt }}</el-descriptions-item>
        <el-descriptions-item label="IP地址">{{ detailData.ipAddress }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="detailData.status === 'Success' ? 'success' : 'danger'" size="small">
            {{ detailData.status }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="操作描述" :span="2">{{ detailData.description }}</el-descriptions-item>
        <el-descriptions-item label="浏览器信息" :span="2">{{ detailData.userAgent }}</el-descriptions-item>
        <el-descriptions-item label="请求参数" :span="2">
          <el-input
            v-model="detailData.requestData"
            type="textarea"
            :rows="4"
            readonly
          />
        </el-descriptions-item>
        <el-descriptions-item label="响应结果" :span="2">
          <el-input
            v-model="detailData.responseData"
            type="textarea"
            :rows="4"
            readonly
          />
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup>
import { useLatestRequest } from '@/composables/useLatestRequest'
const loadDataRequest = useLatestRequest()

import { notifyError } from '@/utils/errors'
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemApi } from '@/api'

const tableData = ref([])
const loading = ref(false)
const detailVisible = ref(false)
const detailData = ref({})

const searchForm = reactive({
  action: '',
  username: '',
  module: '',
  ipAddress: '',
  dateRange: []
})

const pagination = reactive({
  page: 1,
  pageSize: 10,
  total: 0
})

const getOperationTypeText = (type) => {
  const typeMap = {
    'LOGIN': '登录',
    'LOGOUT': '登出',
    'CREATE': '创建',
    'UPDATE': '更新',
    'DELETE': '删除',
    'QUERY': '查询',
    'ERROR': '错误'
  }
  return typeMap[type] || type
}

const getOperationTypeTag = (type) => {
  const tagMap = {
    'LOGIN': 'success',
    'LOGOUT': 'info',
    'CREATE': 'primary',
    'UPDATE': 'warning',
    'DELETE': 'danger',
    'QUERY': '',
    'ERROR': 'danger'
  }
  return tagMap[type] || ''
}

const loadData = async () => {
  const requestId = loadDataRequest.begin()
  loading.value = true
  try {
    const params = {
      page: pagination.page,
      pageSize: pagination.pageSize
    }

    // 添加搜索条件
    if (searchForm.action) params.action = searchForm.action
    if (searchForm.username) params.username = searchForm.username
    if (searchForm.module) params.module = searchForm.module
    if (searchForm.ipAddress) params.ipAddress = searchForm.ipAddress

    // 处理日期范围
    if (searchForm.dateRange && searchForm.dateRange.length === 2) {
      params.startDate = searchForm.dateRange[0]
      params.endDate = searchForm.dateRange[1]
    }

    const res = await systemApi.logs.getList(params)
    if (!loadDataRequest.isCurrent(requestId)) return
    if (res.success) {
      tableData.value = res.data?.items || []
      pagination.total = res.data?.totalCount || 0
    } else {
      ElMessage.error(res.message || '加载数据失败')
    }
  } catch (error) {
    if (!loadDataRequest.isCurrent(requestId)) return

    notifyError(error, '加载数据失败，请稍后重试')
  } finally {
    if (loadDataRequest.isCurrent(requestId)) {
    loading.value = false

    }
  }
}

const handleSearch = () => {
  pagination.page = 1
  loadData()
}

const handleReset = () => {
  Object.assign(searchForm, {
    action: '',
    username: '',
    module: '',
    ipAddress: '',
    dateRange: []
  })
  handleSearch()
}

const handleViewDetail = (row) => {
  detailData.value = { ...row }
  detailVisible.value = true
}

const handleClearLogs = async () => {
  try {
    await ElMessageBox.confirm(
      '确定要清空所有日志吗？此操作不可恢复！',
      '警告',
      {
        type: 'warning',
        confirmButtonText: '确定清空',
        cancelButtonText: '取消'
      }
    )

    const res = await systemApi.logs.clear()
    if (res.success) {
      ElMessage.success('日志清空成功')
      loadData()
    } else {
      ElMessage.error(res.message || '日志清空失败')
    }
  } catch (error) {
    if (error !== 'cancel') {

      notifyError(error, '日志清空失败，请稍后重试')
    }
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

.search-form {
  margin-bottom: 20px;
}

.search-form :deep(.el-form-item) {
  margin-bottom: 10px;
}

.pagination {
  margin-top: 20px;
  justify-content: flex-end;
}
</style>
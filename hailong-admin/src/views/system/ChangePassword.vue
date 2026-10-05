<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <span>修改密码</span>
      </template>

      <el-form
        ref="formRef"
        :model="formData"
        :rules="rules"
        label-width="120px"
        style="max-width: 500px;"
      >
        <el-form-item label="旧密码" prop="oldPassword">
          <el-input
            v-model="formData.oldPassword"
            type="password"
            placeholder="请输入旧密码"
            show-password
          />
        </el-form-item>

        <el-form-item label="新密码" prop="newPassword">
          <el-input
            v-model="formData.newPassword"
            type="password"
            placeholder="请输入新密码"
            show-password
          />
        </el-form-item>

        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="formData.confirmPassword"
            type="password"
            placeholder="请再次输入新密码"
            show-password
          />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="handleSubmit" :loading="submitting">
            确认修改
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { notifyError } from '@/utils/errors'
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { clearSession } from '@/utils/session'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()
const router = useRouter()
const formRef = ref(null)
const submitting = ref(false)

const formData = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const validateConfirmPassword = (rule, value, callback) => {
  if (value === '') {
    callback(new Error('请再次输入新密码'))
  } else if (value !== formData.newPassword) {
    callback(new Error('两次输入的密码不一致'))
  } else {
    callback()
  }
}

const rules = {
  oldPassword: [
    { required: true, message: '请输入旧密码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, message: '密码长度不能少于6位', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, validator: validateConfirmPassword, trigger: 'blur' }
  ]
}

const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    submitting.value = true
    try {
      const success = await userStore.changePassword({
        oldPassword: formData.oldPassword,
        newPassword: formData.newPassword
      })

      if (success) {
        clearSession()
        await router.replace('/login')
        ElMessage.success('密码修改成功，请重新登录')
        handleReset()
      } else {
        ElMessage.error('密码修改失败')
      }
    } catch (error) {

      notifyError(error, '密码修改失败')
    } finally {
      submitting.value = false
    }
  })
}

const handleReset = () => {
  formData.oldPassword = ''
  formData.newPassword = ''
  formData.confirmPassword = ''
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}
</script>

<style scoped>
.page-container {
  padding: 0;
}
</style>

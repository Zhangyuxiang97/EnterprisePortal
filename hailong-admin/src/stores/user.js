import { defineStore } from 'pinia'
import { computed } from 'vue'
import { authApi } from '@/api'
import { session, saveSession, clearSession } from '@/utils/session'

export const useUserStore = defineStore('user', () => {
  const userInfo = computed(() => session.user)
  const token = computed(() => session.token)
  const login = async data => {
    const response = await authApi.login(data)
    saveSession(response.data)
    return true
  }
  const getCurrentUser = async () => {
    const response = await authApi.getCurrentUser()
    saveSession(response.data)
    return true
  }
  const changePassword = async data => (await authApi.changePassword(data)).success
  const logout = async () => {
    if (session.token) await authApi.logout()
    clearSession()
  }
  return { userInfo, token, login, getCurrentUser, changePassword, logout }
})

export default useUserStore

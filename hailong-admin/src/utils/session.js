import { reactive } from 'vue'
import { tokenUtils } from './auth'

export const session = reactive({ token: tokenUtils.getToken(), user: tokenUtils.getUserInfo(), generation: 0, verified: false })

export function saveSession(data) {
  if (data.token) {
    tokenUtils.setToken(data.token)
    tokenUtils.setRefreshToken(data.refreshToken)
    session.token = data.token
  }
  session.user = {
    userId: data.userId ?? data.id, username: data.username, fullName: data.fullName ?? data.realName,
    email: data.email, role: data.role
  }
  tokenUtils.setUserInfo(session.user)
  session.verified = true
}

export function clearSession() {
  session.generation++
  session.token = null
  session.user = null
  session.verified = false
  tokenUtils.clearAuth()
}

window.addEventListener('storage', event => {
  if (!event.key || event.key.startsWith('hailong_admin_')) {
    session.token = tokenUtils.getToken()
    session.user = tokenUtils.getUserInfo()
    session.verified = false
    session.generation++
  }
})

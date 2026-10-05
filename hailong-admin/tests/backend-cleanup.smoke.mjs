import { chromium } from 'playwright'
import assert from 'node:assert/strict'
import { randomUUID } from 'node:crypto'

const base = process.env.ADMIN_TEST_URL || 'http://127.0.0.1:3002'
const apiBase = process.env.ADMIN_TEST_API || 'http://127.0.0.1:5000/api'
for (const url of [base, apiBase]) assert.ok(['127.0.0.1', 'localhost'].includes(new URL(url).hostname), '仅允许本机回归环境')
const browser = await chromium.launch({ ...(process.env.ADMIN_TEST_CHANNEL === 'chromium' ? {} : { channel: process.env.ADMIN_TEST_CHANNEL || 'chrome' }), headless: true })
const context = await browser.newContext({ viewport: { width: 1440, height: 1000 } })
const page = await context.newPage()
const tag = `收尾回归-${Date.now()}`
const created = [], errors = []
let rootToken, token
page.on('pageerror', error => errors.push(error.message.slice(0, 150)))
page.on('console', message => { if (/Failed to resolve component|not exists\. Import it first/.test(message.text())) errors.push(message.text().slice(0, 150)) })
async function raw(path, method = 'GET', data, auth = token) {
  return fetch(apiBase + path, { method, headers: { 'Content-Type': 'application/json', ...(auth ? { Authorization: `Bearer ${auth}` } : {}) }, ...(data === undefined ? {} : { body: JSON.stringify(data) }) })
}
async function api(path, method = 'GET', data, auth = token) {
  const response = await raw(path, method, data, auth), body = await response.json()
  assert.ok(response.ok && body.success, `${method} ${path}: ${response.status} ${body.message}`)
  return body.data
}
async function login(username, password) {
  await page.goto(base + '/login')
  await page.getByPlaceholder('请输入用户名', { exact: true }).fill(username)
  await page.getByPlaceholder('请输入密码', { exact: true }).fill(password)
  await page.getByRole('button', { name: /登\s*录/ }).click()
  await page.waitForURL('**/dashboard')
  await page.waitForLoadState('networkidle')
  token = await page.evaluate(() => localStorage.getItem('hailong_admin_token'))
}
const dialog = () => page.locator('.editor-dialog:visible')
try {
  rootToken = (await api('/auth/login', 'POST', { username: process.env.ADMIN_TEST_USERNAME, password: process.env.ADMIN_TEST_PASSWORD }, null)).token
  const temporaryPassword = `Regression!${randomUUID()}`
  const account = await api('/User', 'POST', { username: `cleanup_${Date.now()}`, password: temporaryPassword, email: `cleanup_${Date.now()}@example.invalid`, role: 'admin', realName: '收尾回归', status: 1 }, rootToken)
  created.push(['/User', account.id])
  await login(account.username, temporaryPassword)
  let meRequests = 0
  page.on('request', req => { if (new URL(req.url()).pathname === '/api/auth/me') meRequests++ })
  await page.reload({ waitUntil: 'networkidle' })
  assert.ok(meRequests > 0, '首次加载应从服务端验证身份')

  const article = await api('/info-publications', 'POST', { type: 'COMPANY_NEWS', category: '公司新闻', title: tag, content: '<p>测试正文</p>', publishTime: new Date().toISOString(), status: 0 })
  created.push(['/info-publications', article.id])
  assert.equal((await raw(`/info-publications/${article.id}`, 'GET', undefined, null)).status, 404)
  assert.equal((await api(`/info-publications?keyword=${encodeURIComponent(tag)}`, 'GET', undefined, null)).totalCount, 0)
  const before = await api(`/info-publications/manage/${article.id}`)
  await api(`/info-publications/manage/${article.id}`)
  const after = await api(`/info-publications/manage/${article.id}`)
  assert.equal(after.viewCount, before.viewCount)
  assert.equal(after.version, before.version)
  assert.equal((await raw('/info-publications/manage', 'GET', undefined, null)).status, 401)
  const list = await api(`/info-publications/manage?keyword=${encodeURIComponent(tag)}`)
  assert.equal(list.totalCount, 1)
  assert.ok(!Object.hasOwn(list.items[0], 'content'))
  for (const path of ['/announcements?pageNumber=0', '/announcements?pageSize=101', '/announcements?pageNumber=2147483647&pageSize=100', '/User?page=0']) {
    const invalid = await raw(path); assert.equal(invalid.status, 400, path); assert.ok((await invalid.json()).traceId)
  }
  console.log('PASS 公开/管理隔离、列表投影、浏览计数与分页边界')

  await page.goto(base + '/info-publish/news-center', { waitUntil: 'networkidle' })
  await page.getByPlaceholder('搜索标题、作者').fill(tag)
  await page.getByRole('button', { name: '搜索', exact: true }).click()
  await page.locator('.el-table__row').filter({ hasText: tag }).getByRole('button', { name: '编辑', exact: true }).click()
  await dialog().waitFor()
  const title = page.getByPlaceholder('请输入新闻标题（最多255个字符）')
  await title.fill(tag + '-本地未保存')
  let uploadAttempts = 0
  await page.route('**/api/attachments/upload', async route => {
    if (++uploadAttempts === 1) await route.fulfill({ status: 401, json: { success: false } })
    else await route.continue()
  })
  const uploadedResponse = page.waitForResponse(r => r.url().endsWith('/api/attachments/upload') && r.status() === 200)
  await dialog().locator('input[type=file]').first().setInputFiles({ name: tag + '.png', mimeType: 'image/png', buffer: Buffer.from('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jzV0AAAAASUVORK5CYII=', 'base64') })
  const uploaded = (await (await uploadedResponse).json()).data
  created.push(['/attachments', uploaded.id])
  await page.getByText('文件上传成功', { exact: true }).waitFor()
  assert.equal(uploadAttempts, 2)
  await page.unroute('**/api/attachments/upload')
  await api(`/info-publications/${article.id}`, 'PUT', { version: before.version, title: tag + '-另一人已保存' })
  const conflict = page.waitForResponse(r => r.request().method() === 'PUT' && r.url().includes(`/info-publications/${article.id}`))
  await dialog().getByRole('button', { name: '提交', exact: true }).click()
  assert.equal((await conflict).status(), 409)
  assert.ok(await dialog().isVisible()); assert.equal(await title.inputValue(), tag + '-本地未保存')
  await page.getByText(/内容已被其他人修改/).waitFor()
  assert.equal(await page.locator('.el-message--error').count(), 1, '同一错误只提示一次')
  console.log('PASS 过期版本返回409、保留编辑内容且只显示一次错误')
  console.log('PASS 编辑器附件上传共用认证刷新并成功重试')

  // Concurrent protected requests simulate the same expired access token; only one real refresh may run.
  let refreshRequests = 0
  page.on('request', req => { if (new URL(req.url()).pathname === '/api/auth/refresh') refreshRequests++ })
  const challenged = new Set()
  await page.route('**/api/announcements/manage?probe=*', async route => {
    const key = route.request().url()
    if (!challenged.has(key)) { challenged.add(key); await route.fulfill({ status: 401, json: { success: false } }) }
    else await route.continue()
  })
  const success = await page.evaluate(async () => {
    const { default: request } = await import('/src/api/request.js')
    const results = await Promise.all([1,2,3].map(probe => request.get('/announcements/manage', { params: { probe } })))
    return results.every(result => result.success)
  })
  assert.ok(success); assert.equal(refreshRequests, 1)
  await page.unroute('**/api/announcements/manage?probe=*')
  console.log('PASS 三个并发401只刷新一次并各自重试成功')

  await page.route('**/api/auth/refresh', route => route.fulfill({ status: 401, json: { success: false, message: '刷新令牌失效' } }))
  await page.route('**/api/announcements/manage?expired=true', route => route.fulfill({ status: 401, json: { success: false } }))
  await page.evaluate(async () => {
    const { default: request } = await import('/src/api/request.js')
    await request.get('/announcements/manage?expired=true').catch(() => {})
  })
  assert.ok(await dialog().isVisible()); assert.equal(await title.inputValue(), tag + '-本地未保存')
  assert.equal(await page.evaluate(() => localStorage.getItem('hailong_admin_token')), null)
  await page.unroute('**/api/auth/refresh'); await page.unroute('**/api/announcements/manage?expired=true')
  const loginTab = await context.newPage()
  await loginTab.goto(base + '/login')
  await loginTab.getByPlaceholder('请输入用户名', { exact: true }).fill(account.username)
  await loginTab.getByPlaceholder('请输入密码', { exact: true }).fill(temporaryPassword)
  await loginTab.getByRole('button', { name: /登\s*录/ }).click()
  await loginTab.waitForURL('**/dashboard')
  await page.waitForFunction(() => Boolean(localStorage.getItem('hailong_admin_token')))
  await loginTab.close()
  assert.equal(await title.inputValue(), tag + '-本地未保存')
  console.log('PASS 登录失效保留表单，新窗口重新登录后恢复编辑')

  // Cancelling the dirty editor warning must not revoke the active server session.
  let logoutRequests = 0
  page.on('request', req => { if (new URL(req.url()).pathname === '/api/auth/logout') logoutRequests++ })
  async function requestLogout() {
    // The editor is modal, so close it only after confirming the guard through the shared action registry.
    return page.evaluate(async () => {
      const { confirmEditorsLeave, resetEditorLeaveApprovals } = await import('/src/utils/editorLeave.js')
      const allowed = await confirmEditorsLeave(); resetEditorLeaveApprovals(); return allowed
    })
  }
  const cancelLeave = requestLogout()
  await page.getByRole('button', { name: '继续编辑', exact: true }).click()
  assert.equal(await cancelLeave, false); assert.equal(logoutRequests, 0)
  assert.equal(await title.inputValue(), tag + '-本地未保存')
  await dialog().getByRole('button', { name: '取消', exact: true }).click()
  await page.getByRole('button', { name: '放弃修改', exact: true }).click()
  await dialog().waitFor({ state: 'hidden' })
  // Settings use a full-page editor: the actual header must ask before calling server logout.
  await page.goto(base + '/config/site-settings', { waitUntil: 'networkidle' })
  const companyName = page.locator('.el-form-item').filter({ hasText: '公司全称' }).locator('input')
  const editedName = (await companyName.inputValue()) + '-未保存'
  await companyName.fill(editedName)
  await page.locator('.user-info').hover()
  await page.getByRole('menuitem', { name: '退出登录', exact: true }).click()
  await page.locator('.el-message-box').getByRole('button', { name: '确定', exact: true }).click()
  await page.getByRole('button', { name: '继续编辑', exact: true }).click()
  assert.equal(logoutRequests, 0); assert.equal(await companyName.inputValue(), editedName)
  const credentials = await page.evaluate(() => ({ token: localStorage.getItem('hailong_admin_token'), refreshToken: localStorage.getItem('hailong_admin_refresh_token') }))
  await page.locator('.user-info').hover()
  await page.getByRole('menuitem', { name: '退出登录', exact: true }).click()
  await page.locator('.el-message-box').getByRole('button', { name: '确定', exact: true }).click()
  await page.getByRole('button', { name: '放弃修改', exact: true }).click()
  await page.waitForURL('**/login')
  assert.equal(logoutRequests, 1)
  assert.equal((await raw('/auth/me', 'GET', undefined, credentials.token)).status, 401)
  assert.equal((await raw('/auth/refresh', 'POST', { refreshToken: credentials.refreshToken }, null)).status, 401)
  console.log('PASS 未保存取消不注销，实际退出同时撤销访问令牌与刷新令牌')
  assert.deepEqual(errors, [])
} catch (error) {
  console.error(`FAIL ${error.message.slice(0, 700)}`)
  console.error('页面错误摘要:', errors)
  process.exitCode = 1
} finally {
  for (const [path, id] of created.reverse()) {
    try { await api(`${path}/${id}`, 'DELETE', undefined, rootToken) } catch (error) { console.error(`清理失败 ${path}/${id}: ${error.message.slice(0, 150)}`); process.exitCode = 1 }
  }
  await browser.close()
}

import { chromium } from 'playwright'
import assert from 'node:assert/strict'
import { randomUUID } from 'node:crypto'

// 仅针对本机预览：运行前启动两端，并通过环境变量提供本机测试管理员。
const base = process.env.ADMIN_TEST_URL || 'http://127.0.0.1:3002'
const apiBase = process.env.ADMIN_TEST_API || 'http://127.0.0.1:5000/api'
for (const url of [base, apiBase]) assert.ok(['127.0.0.1', 'localhost'].includes(new URL(url).hostname), '仅允许本机测试环境')
const username = process.env.ADMIN_TEST_USERNAME, password = process.env.ADMIN_TEST_PASSWORD
assert.ok(username && password, '请设置 ADMIN_TEST_USERNAME / ADMIN_TEST_PASSWORD')
const tag = `后台回归-${Date.now()}`
const browser = await chromium.launch({ ...(process.env.ADMIN_TEST_CHANNEL === 'chromium' ? {} : { channel: process.env.ADMIN_TEST_CHANNEL || 'chrome' }), headless: true })
const context = await browser.newContext({ viewport: { width: 1440, height: 1000 }, timezoneId: 'Asia/Shanghai' })
const page = await context.newPage()
const errors = [], created = [], failedRequests = []
let token
page.on('response', response => { if (response.url().includes('/api/') && response.status() >= 400) failedRequests.push({ url: new URL(response.url()).pathname, status: response.status() }) })
page.on('pageerror', error => errors.push(error.message.split(' in node:')[0].slice(0, 180)))
async function api(path, method = 'GET', data, auth = token) {
  const response = await fetch(`${apiBase}${path}`, { method, headers: { 'Content-Type': 'application/json', ...(auth ? { Authorization: `Bearer ${auth}` } : {}) }, ...(data === undefined ? {} : { body: JSON.stringify(data) }) })
  const raw = await response.text()
  assert.ok(raw, `${method} ${path}: 空响应 ${response.status}`)
  const body = JSON.parse(raw)
  assert.ok(response.ok && body.success, `${method} ${path}: ${response.status} ${body.message}`)
  return body.data
}
async function visit(route) { await page.goto(base + route, { waitUntil: 'networkidle' }) }
const dialog = () => page.locator('.editor-dialog:visible')
async function save() {
  await dialog().getByRole('button', { name: /^(提交|确定)$/ }).click()
  await dialog().waitFor({ state: 'hidden', timeout: 10000 })
  await page.waitForLoadState('networkidle')
}
async function editRow(word) {
  await page.locator('.el-table__row').filter({ hasText: word }).first().getByRole('button', { name: '编辑', exact: true }).click()
  await dialog().waitFor()
}
try {
  await visit('/login')
  await page.getByPlaceholder('请输入用户名', { exact: true }).fill(username)
  await page.getByPlaceholder('请输入密码', { exact: true }).fill(password)
  await page.getByRole('button', { name: /登\s*录/ }).click()
  await page.waitForURL('**/dashboard')
  token = await page.evaluate(() => localStorage.getItem('hailong_admin_token'))
  assert.ok(token)

  // 已复现崩溃的真实历史公告：打开不会改写正文或误标脏数据。
  await visit('/gov-procurement')
  await editRow('安阳市中医院中医药大健康产品线下展厅实体运营项目废标公告')
  await page.waitForFunction(() => document.querySelector('[data-slate-editor]')?.textContent.includes('安阳市中医院'))
  await page.waitForFunction(() => document.querySelector('.el-dialog .el-cascader input')?.value.includes('安阳'))
  await dialog().getByRole('button', { name: '取消', exact: true }).click()
  await dialog().waitFor({ state: 'hidden' })
  assert.equal(await page.locator('.el-message-box:visible').count(), 0)
  console.log('PASS 历史公告编辑与无改动关闭')
  const historicalBefore = await api('/announcements/manage/2299')
  await editRow('安阳市中医院中医药大健康产品线下展厅实体运营项目废标公告')
  await page.waitForFunction(() => document.querySelector('.el-dialog .el-cascader input')?.value.includes('安阳'))
  await save()
  const historicalAfter = await api('/announcements/manage/2299')
  for (const key of ['content', 'province', 'city', 'district', 'budgetAmount', 'awardAmount', 'isTop', 'status']) assert.deepEqual(historicalAfter[key], historicalBefore[key], `历史公告 ${key} 被意外改写`)
  console.log('PASS 历史公告保存保持正文、地区、金额和状态')


  await visit('/info-publish/news-center')
  await page.getByRole('button', { name: '新增新闻', exact: true }).click()
  await dialog().waitFor()
  const date = await dialog().locator('.el-date-editor input').first().inputValue()
  const local = await page.evaluate(() => new Date().toLocaleString('sv-SE'))
  assert.equal(date.slice(0, 13), local.slice(0, 13))
  await page.getByPlaceholder('请输入新闻标题（最多255个字符）').fill(tag)
  await dialog().getByRole('button', { name: '取消', exact: true }).click()
  await page.getByRole('button', { name: '继续编辑', exact: true }).click()
  assert.ok(await dialog().isVisible())
  await dialog().getByRole('button', { name: '取消', exact: true }).click()
  await page.getByRole('button', { name: '放弃修改', exact: true }).click()
  await dialog().waitFor({ state: 'hidden' })
  console.log('PASS 本地默认时间与未保存提醒')

  for (const [route, word] of [['/config/qualifications', '营业执照'], ['/config/honors', '2018—2024']]) {
    await visit(route); await editRow(word); await save()
  }
  console.log('PASS 历史企业资料缺失可选字段仍可保存')
  await visit('/info-publish/news-center')
  await page.getByPlaceholder('搜索标题、作者').fill('河南招标采购综合网关于停止发布招标公告及公示信息的通知')
  await page.getByRole('button', { name: '搜索', exact: true }).click()
  await editRow('河南招标采购综合网关于停止发布招标公告及公示信息的通知')
  await page.waitForFunction(() => document.querySelectorAll('[data-slate-editor] img').length === 2)
  await dialog().getByRole('button', { name: '预览正文', exact: true }).click()
  const preview = page.getByRole('dialog', { name: '正文预览', exact: true })
  await preview.waitFor()
  assert.equal(await preview.locator('iframe').getAttribute('sandbox'), '')
  const previewFrame = page.frameLocator('iframe[title="正文预览"]')
  assert.equal(await previewFrame.locator('img').count(), 2)
  await preview.locator('.el-dialog__headerbtn').click()
  await save()
  console.log('PASS 扫描件正文载入、预览和保存')


  const article = await api('/info-publications', 'POST', { type: 'POLICY_REGULATION', category: '地方政策', title: tag, content: '<p>测试政策正文</p>', documentNumber: '测试〔2026〕1号', publishTime: '2026-10-01T23:59:00', status: 1 })
  created.push(['/info-publications', article.id])
  await visit('/info-publish/policy-regulation')
  await page.getByPlaceholder('搜索标题、发文单位').fill(tag)
  await page.getByRole('button', { name: '搜索', exact: true }).click()
  await editRow(tag)
  assert.equal(await page.getByPlaceholder('请输入文号').inputValue(), '测试〔2026〕1号')
  await page.getByPlaceholder('请输入文号').fill('测试〔2026〕2号')
  await save()
  assert.equal((await api(`/info-publications/manage/${article.id}`)).documentNumber, '测试〔2026〕2号')
  await editRow(tag); await page.getByPlaceholder('请输入文号').fill(''); await save()
  assert.ok(!(await api(`/info-publications/manage/${article.id}`)).documentNumber)
  const matches = await api(`/info-publications?type=POLICY_REGULATION&keyword=${encodeURIComponent(tag)}&startDate=2026-10-01&endDate=2026-10-01`)
  assert.equal(matches.totalCount, 1)
  assert.equal((await api('/info-publications?type=COMPANY_NEWS&startDate=2030-01-01&endDate=2030-01-02')).totalCount, 0)
  console.log('PASS 文号编辑往返与日期筛选边界')

  await visit('/system/regions')
  await page.getByPlaceholder('请输入地区名称', { exact: true }).fill('中原')
  await page.getByRole('button', { name: '搜索', exact: true }).click()
  await page.getByText('中原区', { exact: true }).waitFor()
  assert.equal(await page.getByText('禁用', { exact: true }).count(), 0)
  await page.getByRole('button', { name: '重置', exact: true }).click()
  await page.getByRole('button', { name: '新增地区', exact: true }).click()
  await dialog().getByPlaceholder('请输入地区名称', { exact: true }).fill(tag)
  // 测试前要求该临时代码不存在；不会覆盖原有行政区划。
  const tree = await api('/regions/tree')
  assert.ok(!tree.some(node => node.code === '990000'), '临时测试地区代码已存在')
  await dialog().getByPlaceholder('请输入地区代码', { exact: true }).fill('990000')
  const regionResponse = page.waitForResponse(r => r.url().endsWith('/api/regions') && r.request().method() === 'POST')
  await save()
  const region = (await (await regionResponse).json()).data
  created.push(['/regions', region.id])
  await editRow(tag)
  assert.ok(await dialog().getByPlaceholder('请输入地区代码', { exact: true }).isDisabled())
  await dialog().getByPlaceholder('请输入地区名称', { exact: true }).fill(tag + '修改')
  await save()
  assert.equal((await api(`/regions/${region.id}`)).regionName, tag + '修改')
  console.log('PASS 地区树筛选、新增和编辑')

  await visit('/attachments')
  await page.getByRole('button', { name: '上传附件', exact: true }).click()
  const uploadDialog = page.getByRole('dialog', { name: '上传附件', exact: true })
  await uploadDialog.locator('.el-form-item').filter({ hasText: '关联类型' }).locator('.el-select').click()
  await page.getByRole('option', { name: '信息发布', exact: true }).click()
  await uploadDialog.getByRole('spinbutton').fill(String(article.id))
  await page.route('**/api/attachments/upload', async route => {
    const response = await route.fetch()
    await new Promise(resolve => setTimeout(resolve, 3000))
    await route.fulfill({ response })
  })
  await page.locator('input[type=file]').setInputFiles({ name: tag + '.txt', mimeType: 'text/plain', buffer: Buffer.from('后台附件上传回归验证') })
  const uploadedResponse = page.waitForResponse(r => r.url().endsWith('/api/attachments/upload') && r.request().method() === 'POST')
  await page.getByRole('button', { name: '开始上传', exact: true }).click()
  await new Promise(resolve => setTimeout(resolve, 2200))
  assert.ok(await uploadDialog.isVisible())
  assert.ok(await uploadDialog.getByRole('button', { name: '取消', exact: true }).isDisabled())
  const uploaded = (await (await uploadedResponse).json()).data
  assert.ok(uploaded?.id)
  created.push(['/attachments', uploaded.id])
  await page.unroute('**/api/attachments/upload')
  assert.equal(uploaded.category, 'document')
  assert.equal(uploaded.relatedType, 'info_publication')
  assert.equal(uploaded.relatedId, article.id)
  assert.ok((await fetch(new URL(uploaded.fileUrl, base))).ok)
  await page.getByRole('dialog', { name: '上传附件', exact: true }).waitFor({ state: 'hidden' })
  await page.getByPlaceholder('文件名称', { exact: true }).fill(tag)
  await page.getByRole('button', { name: '搜索', exact: true }).click()
  const uploadedRow = page.locator('.el-table__row').filter({ hasText: tag })
  await uploadedRow.getByRole('button', { name: '删除', exact: true }).click()
  await page.locator('.el-message-box').getByRole('button', { name: '确定', exact: true }).click()
  await uploadedRow.waitFor({ state: 'hidden' })
  created.splice(created.findIndex(x => x[0] === '/attachments' && x[1] === uploaded.id), 1)
  const certificates = await api('/attachments?relatedType=company_qualification')
  assert.equal(certificates.totalCount, 4)
  assert.ok((await api(`/attachments/${certificates.items[0].id}/references`)).length > 0)
  const protectedDelete = await fetch(`${apiBase}/attachments/${certificates.items[0].id}`, { method: 'DELETE', headers: { Authorization: `Bearer ${token}` } })
  assert.equal(protectedDelete.status, 409)
  console.log('PASS 实际文件上传、读取、删除与恢复附件引用')

  for (const path of ['/statistics/announcements/overview', '/statistics/announcements/publish-trend?startDate=2024-01-01&endDate=2026-12-31&groupBy=month', '/statistics/announcements/type-distribution', '/statistics/announcements/region-distribution', '/statistics/announcements/popular', '/statistics/announcements/status-distribution', '/statistics/info-publications/overview', '/statistics/info-publications/publish-trend?startDate=2024-01-01&endDate=2026-12-31&groupBy=month', '/statistics/info-publications/author-statistics']) await api(path)
  assert.equal((await api('/statistics/home')).knownAwardAmountCount, 497)
  console.log('PASS MySQL 统计聚合接口与金额覆盖数量')

  const readerPassword = `Test!9${randomUUID()}`
  const reader = await api('/User', 'POST', { username: `reader_${Date.now()}`, password: readerPassword, role: 'user', status: 1, email: `reader_${Date.now()}@example.invalid`, realName: '本机回归验证' })
  created.push(['/User', reader.id])
  const readerContext = await browser.newContext()
  const readerPage = await readerContext.newPage()
  await readerPage.goto(base + '/login')
  await readerPage.getByPlaceholder('请输入用户名', { exact: true }).fill(reader.username)
  await readerPage.getByPlaceholder('请输入密码', { exact: true }).fill(readerPassword)
  await readerPage.getByRole('button', { name: /登\s*录/ }).click()
  await readerPage.waitForURL('**/dashboard')
  for (const route of ['/gov-procurement', '/construction', '/info-publish/news-center', '/info-publish/policy-regulation']) {
    await readerPage.goto(base + route, { waitUntil: 'networkidle' })
    assert.equal(await readerPage.getByRole('button', { name: /新增|编辑|删除|置顶/ }).count(), 0)
    assert.ok(await readerPage.getByRole('button', { name: '查看', exact: true }).count() > 0)
  }
  const readerToken = await readerPage.evaluate(() => localStorage.getItem('hailong_admin_token'))
  const forbidden = await fetch(`${apiBase}/info-publications/${article.id}`, { method: 'PUT', headers: { Authorization: `Bearer ${readerToken}`, 'Content-Type': 'application/json' }, body: JSON.stringify({ title: '不得写入' }) })
  assert.equal(forbidden.status, 403)
  await readerContext.close()
  console.log('PASS 普通用户只读界面与后端写入权限')
  assert.deepEqual(errors, [])
  console.log('PASS 后台浏览器回归全部完成，无页面脚本错误')
} catch (error) {
  console.error(`FAIL ${error.message.slice(0, 600)}`)
  console.error('页面错误摘要:', errors)
  console.error('失败请求:', failedRequests)
  if (process.env.ADMIN_TEST_SCREENSHOT) await page.screenshot({ path: process.env.ADMIN_TEST_SCREENSHOT, fullPage: true })
  process.exitCode = 1
} finally {
  // 只清理本次创建的测试记录；已上传文件遵循服务端软删除规则。
  for (const [path, id] of created.reverse()) {
    try { await api(`${path}/${id}`, 'DELETE') } catch (error) { console.error(`清理 ${path}/${id} 失败：${error.message.slice(0, 180)}`); process.exitCode = 1 }
  }
  await browser.close()
}

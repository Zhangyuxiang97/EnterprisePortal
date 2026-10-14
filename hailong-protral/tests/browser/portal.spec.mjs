import { test, expect, api, noHorizontalOverflow, healthyImages, screenshot, fulfill } from './helpers.mjs'
import { readFileSync } from 'node:fs'

test('首页、全站入口和手机横屏布局', async ({ page }, testInfo) => {
  await page.goto('/')
  await expect(page.locator('.slogan-reveal')).toHaveAttribute('data-state', 'complete')
  await noHorizontalOverflow(page)
  await screenshot(page, testInfo, '首页', false)
  const consultation = page.getByRole('button', { name: '联系业务咨询', exact: true })
  await consultation.press('Enter')
  const dialog = page.getByRole('dialog', { name: '联系我们', exact: true })
  await expect(dialog).toBeVisible()
  await expect(dialog).toContainText('联系电话')
  await noHorizontalOverflow(page)
  await page.keyboard.press('Escape')
  await expect(dialog).toHaveCount(0)
  await expect(consultation).toBeFocused()
  for (const [route, heading] of [['/contact', '联系我们'], ['/tools', '实用工具'], ['/expert-database', '专家库']]) {
    await page.goto(route)
    await expect(page.getByRole('heading', { name: heading, exact: true }).first()).toBeVisible()
    await noHorizontalOverflow(page)
  }
  if (testInfo.project.name.includes('phone')) {
    await page.setViewportSize({ width: 844, height: 390 })
    await page.goto('/')
    await expect(page.locator('.slogan-reveal')).toHaveAttribute('data-state', 'complete')
    await noHorizontalOverflow(page)
    const hero = page.locator('[data-home-surface="hero"]')
    // 横屏首屏内容应留在自己的区域内，不能被后续公告模块覆盖。
    const button = await consultation.boundingBox(), area = await hero.boundingBox()
    expect(button.y + button.height).toBeLessThanOrEqual(area.y + area.height)
    await screenshot(page, testInfo, '横屏首页', false)
  }
})

test('首页两类公告与列表使用相同链接，历史数字详情可打开和刷新', async ({ page, request }) => {
  const recent = await api(request, '/home/recent-announcements')
  for (const [sourceType, keyboard] of [['政府采购', false], ['建设工程', true]]) {
    const item = recent.find(value => value.sourceType === sourceType)
    expect(item, sourceType + '首页测试数据').toBeTruthy()
    expect(item.hashId).toBeTruthy()
    const businessType = sourceType === '政府采购' ? 'GOV_PROCUREMENT' : 'CONSTRUCTION'
    const list = await api(request, '/announcements?' + new URLSearchParams({ businessType, keyword: item.title }))
    expect(list.items.find(value => value.id === item.id)?.hashId).toBe(item.hashId)
    await page.goto('/')
    const card = page.getByRole('link').filter({ has: page.getByRole('heading', { name: item.title, exact: true }) })
    await card.scrollIntoViewIfNeeded()
    if (keyboard) await card.press('Enter')
    else await card.click()
    await expect(page).toHaveURL(new RegExp('/announcement/' + item.hashId + '$'))
    await expect(page.getByRole('heading', { name: item.title, exact: true }).first()).toBeVisible()
    await noHorizontalOverflow(page)
  }
  const detail = await api(request, '/announcements/by-hash/M0Ggj90m')
  expect(detail.id).toBe(2299)
  await page.goto('/announcement/2299')
  await expect(page.getByRole('heading', { name: detail.title, exact: true }).first()).toBeVisible()
  await page.reload()
  await expect(page.getByRole('heading', { name: detail.title, exact: true }).first()).toBeVisible()
  await expect(page.getByText('内容不存在或已下架', { exact: true })).toHaveCount(0)
})

test('主导航、子菜单、关闭与栏目高亮', async ({ page }, testInfo) => {
  await page.goto('/announcements')
  const nav = page.getByRole('navigation', { name: '主导航', exact: true })
  const mobile = testInfo.project.name !== 'desktop'
  if (mobile) {
    const toggle = nav.getByRole('button', { name: '打开导航菜单', exact: true })
    await toggle.click()
    await expect(nav.getByRole('button', { name: '关闭导航菜单', exact: true })).toHaveAttribute('aria-expanded', 'true')
    await nav.getByRole('button', { name: '资讯中心', exact: true }).click()
    await nav.getByRole('link', { name: '新闻中心', exact: true }).click()
    await expect(page).toHaveURL(/\/news$/)
    await expect(page.locator('#mobile-navigation')).toHaveCount(0)
    await toggle.click()
    await expect(nav.getByRole('button', { name: '资讯中心', exact: true })).toHaveClass(/active/)
    await page.keyboard.press('Escape')
    await expect(page.locator('#mobile-navigation')).toHaveCount(0)
    await toggle.click()
    await page.locator('main').click({ position: { x: 10, y: 600 } })
    await expect(page.locator('#mobile-navigation')).toHaveCount(0)
  } else {
    const menu = nav.getByRole('button', { name: '资讯中心', exact: true })
    await menu.focus()
    await page.keyboard.press('Enter')
    await expect(menu).toHaveAttribute('aria-expanded', 'true')
    await page.keyboard.press('Escape')
    await expect(menu).toHaveAttribute('aria-expanded', 'false')
    await menu.click()
    await nav.getByRole('link', { name: '新闻中心', exact: true }).click()
    await expect(page).toHaveURL(/\/news$/)
    await expect(menu).toHaveAttribute('aria-expanded', 'false')
    await expect(menu).toHaveClass(/active/)
  }
  await noHorizontalOverflow(page)
})

test('咨询电话电脑复制、普通 HTTP 回退、失败反馈与手机拨打', async ({ page, browser, baseURL }, testInfo) => {
  const defaults = JSON.parse(readFileSync(new URL('../../../config/portal-defaults.json', import.meta.url), 'utf8'))
  const mock = target => target.route(baseURL + '/api/**', route => {
    const pathname = new URL(route.request().url()).pathname
    return fulfill(route, pathname.endsWith('/site-settings') ? defaults
      : pathname.endsWith('/company-intro') ? { content: '', highlights: [] } : [])
  })
  await mock(page)
  // 模拟复制，避免自动验收改写用户的系统剪贴板。
  await page.addInitScript(() => {
    Object.defineProperty(navigator, 'userAgent', { configurable: true, value: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/141.0' })
    Object.defineProperty(navigator, 'userAgentData', { configurable: true, value: { mobile: false } })
    window.copyMode = 'success'
    window.copiedPhones = []
    window.legacyCopies = []
    Object.defineProperty(navigator, 'clipboard', { configurable: true, value: { writeText: async value => {
      if (window.copyMode !== 'success') throw new Error('Clipboard denied')
      window.copiedPhones.push(value)
    } } })
    document.execCommand = command => {
      const input = document.activeElement
      window.legacyCopies.push({ command, value: input.value, insideDialog: Boolean(input.closest('dialog')) })
      return window.copyMode === 'fallback'
    }
  })
  await page.goto('/')
  const trigger = page.getByRole('button', { name: '联系业务咨询', exact: true })
  await trigger.press('Enter')
  const dialog = page.getByRole('dialog', { name: '联系我们', exact: true })
  await expect(dialog).toBeVisible()
  await expect(dialog.locator('a[href^="tel:"]')).toHaveCount(0)
  await screenshot(page, testInfo, '咨询电话电脑端', false)
  await dialog.getByRole('button', { name: '复制电话', exact: true }).click()
  await expect(dialog.getByRole('status')).toHaveText('电话号码已复制')
  expect(await page.evaluate(() => window.copiedPhones)).toEqual([defaults.contact.phone])
  await page.evaluate(() => { window.copyMode = 'fallback'; Object.defineProperty(navigator, 'clipboard', { configurable: true, value: undefined }) })
  await dialog.getByRole('button', { name: '已复制', exact: true }).click()
  await expect(dialog.getByRole('status')).toHaveText('电话号码已复制')
  expect(await page.evaluate(() => window.legacyCopies.at(-1))).toEqual({ command: 'copy', value: defaults.contact.phone, insideDialog: true })
  await expect(dialog.locator('textarea')).toHaveCount(0)
  await expect(dialog.getByRole('button', { name: '已复制', exact: true })).toBeFocused()
  await page.evaluate(() => { window.copyMode = 'denied' })
  await dialog.getByRole('button', { name: '已复制', exact: true }).click()
  await expect(dialog.getByRole('status')).toContainText('复制失败，请选中上方号码手动复制')
  await expect(dialog.getByRole('button', { name: '复制电话', exact: true })).toBeVisible()
  await expect(dialog.locator('textarea')).toHaveCount(0)
  await noHorizontalOverflow(page)
  await page.keyboard.press('Escape')
  await expect(dialog).toHaveCount(0)
  await expect(trigger).toBeFocused()

  const mobile = await browser.newContext({ baseURL, viewport: { width: 390, height: 844 }, isMobile: true, hasTouch: true,
    userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 Version/18.0 Mobile/15E148 Safari/604.1' })
  try {
    const phone = await mobile.newPage(), errors = []
    phone.on('pageerror', error => errors.push(error.message))
    await mock(phone)
    await phone.goto('/')
    await phone.getByRole('button', { name: '联系业务咨询', exact: true }).click()
    const phoneDialog = phone.getByRole('dialog', { name: '联系我们', exact: true })
    await expect(phoneDialog.getByRole('link', { name: '拨打电话', exact: true })).toHaveAttribute('href', 'tel:' + defaults.contact.phone)
    await expect(phoneDialog.getByRole('button', { name: '复制电话', exact: true })).toHaveCount(0)
    await noHorizontalOverflow(phone)
    await screenshot(phone, testInfo, '咨询电话手机端', false)
    expect(errors).toEqual([])
  } finally { await mobile.close() }
})

test('公告分页、键盘打开详情、返回与刷新恢复条件', async ({ page }) => {
  await page.goto('/announcements')
  const cards = page.locator('main [role="link"]')
  await expect(cards).toHaveCount(10)
  await page.getByRole('button', { name: '下一页', exact: true }).click()
  await expect(page).toHaveURL(/page=2/)
  await expect(cards).toHaveCount(10)
  const title = await cards.first().locator('h3').innerText()
  await cards.first().focus()
  await page.keyboard.press('Enter')
  await expect(page).toHaveURL(/\/announcement\//)
  await expect(page.getByRole('heading', { level: 1 })).toHaveText(title.trim())
  await expect(page.locator('.rich-text')).not.toBeEmpty()
  await noHorizontalOverflow(page)
  await page.goBack()
  await expect(page).toHaveURL(/page=2/)
  await expect(cards).toHaveCount(10)
  await page.reload()
  await expect(page).toHaveURL(/page=2/)
  await expect(cards.first().locator('h3')).toHaveText(title.trim())
  await noHorizontalOverflow(page)
})

test('公告业务类型、地区联动、搜索、日期校验与重置', async ({ page }, testInfo) => {
  await page.goto('/announcements')
  await expect(page.locator('main [role="link"]')).toHaveCount(10)
  await page.getByRole('button', { name: '政府采购', exact: true }).click()
  await expect(page).toHaveURL(/tab=GOV_PROCUREMENT/)
  await expect(page.getByLabel('省份')).toContainText('河南省')
  await page.getByLabel('省份').selectOption('410000')
  await expect(page.getByLabel('城市')).toContainText('郑州市')
  await page.getByLabel('城市').selectOption('410100')
  await expect(page.getByLabel('区县')).toBeVisible()
  await expect(page).toHaveURL(/city=410100/)
  await noHorizontalOverflow(page)
  await page.getByRole('button', { name: '自定义', exact: true }).click()
  await page.getByLabel('开始日期').fill('2026-10-06')
  await page.getByLabel('开始日期').press('Tab')
  await expect(page).toHaveURL(/startDate=2026-10-06/)
  await page.getByLabel('结束日期').fill('2020-01-01')
  await page.getByLabel('结束日期').press('Tab')
  await expect(page.getByRole('alert')).toContainText('开始日期不能晚于结束日期')
  await noHorizontalOverflow(page)
  await page.getByRole('button', { name: '重置', exact: true }).click()
  await expect(page).toHaveURL(/\/announcements$/)
  await expect(page.getByLabel('城市')).toHaveCount(0)
  await expect(page.getByLabel('开始日期')).toHaveCount(0)
  await page.getByLabel('搜索公告').fill('门户验收无匹配内容-9f42e')
  await page.getByLabel('搜索公告').press('Enter')
  await expect(page.getByRole('status')).toContainText('暂无相关公告')
  await expect(page.getByRole('navigation', { name: '列表分页' })).toHaveCount(0)
  await page.reload()
  await expect(page.getByLabel('搜索公告')).toHaveValue('门户验收无匹配内容-9f42e')
  await page.getByRole('button', { name: '重置', exact: true }).click()
  await expect(page.locator('main [role="link"]')).toHaveCount(10)
  await noHorizontalOverflow(page)
  await screenshot(page, testInfo, '公告列表')
})

test('未明确省份可选择、分页、详情返回及刷新恢复', async ({ page }, testInfo) => {
  const defaults = JSON.parse(readFileSync(new URL('../../../config/portal-defaults.json', import.meta.url), 'utf8'))
  await page.route('**/api/config/site-settings', route => fulfill(route, defaults))
  const records = Array.from({ length: 12 }, (_, i) => ({ id: 1000 + i, hashId: `missing-province-${i + 1}`,
    title: `未明确省份${i + 1}`, province: '', businessType: 'GOV_PROCUREMENT', procurementType: 'service',
    noticeType: 'result', noticeTypeName: '结果公告', publishTime: '2026-10-14T09:00:00', content: '<p>项目省份待补充</p>'
  })).concat([
    { id: 2001, hashId: 'henan-one', title: '河南项目一', province: '410000' },
    { id: 2002, hashId: 'henan-two', title: '河南项目二', province: '410000' },
    { id: 2003, hashId: 'guangdong', title: '广东项目', province: '440000' }
  ])
  await page.route('**/api/announcements/region-options**', route => {
    const query = new URL(route.request().url()).searchParams
    const rows = records.filter(row => !query.get('keyword') || row.title.includes(query.get('keyword')))
    const province = query.get('province') || null
    return fulfill(route, { provinces: [
      { regionCode: '410000', regionName: '河南省', count: rows.filter(row => row.province === '410000').length },
      { regionCode: '440000', regionName: '广东省', count: rows.filter(row => row.province === '440000').length }
    ], cities: province === '410000' ? [{ regionCode: '410100', regionName: '郑州市', count: 2 }] : [], districts: [],
    provinceTotalCount: rows.length, provinceUnlocatedCount: rows.filter(row => !row.province).length,
    selectedProvinceCode: province, selectedCityCode: null, selectedDistrictCode: null })
  })
  const listQueries = []
  await page.route('**/api/announcements?**', route => {
    const query = new URL(route.request().url()).searchParams
    listQueries.push(Object.fromEntries(query))
    const rows = records.filter(row => (!query.get('keyword') || row.title.includes(query.get('keyword')))
      && (!query.get('province') || row.province === query.get('province'))
      && (query.get('provinceUnlocated') !== 'true' || !row.province))
    const offset = (Number(query.get('pageNumber') || 1) - 1) * 10
    return fulfill(route, { items: rows.slice(offset, offset + 10), totalCount: rows.length })
  })
  await page.route('**/api/announcements/by-hash/missing-province-*', route => fulfill(route,
    records.find(row => route.request().url().endsWith(row.hashId))))
  await page.goto('/announcements')
  const province = page.getByLabel('省份'), cards = page.locator('main [role="link"]')
  await expect(province.getByRole('option', { name: '省份未明确 (12)', exact: true })).toBeEnabled()
  await province.selectOption('410000')
  await expect(page.getByLabel('城市')).toBeVisible()
  await province.selectOption('__unlocated__')
  await expect(page).toHaveURL(/province=__unlocated__/)
  await expect(page.getByLabel('城市')).toHaveCount(0)
  await expect(page.getByLabel('区县')).toHaveCount(0)
  await expect(cards).toHaveCount(10)
  expect(listQueries.at(-1).provinceUnlocated).toBe('true')
  for (const key of ['province', 'city', 'district']) expect(listQueries.at(-1)[key]).toBeUndefined()
  await page.getByRole('button', { name: '下一页', exact: true }).click()
  await expect(page).toHaveURL(/page=2/)
  await expect(cards).toHaveCount(2)
  await page.reload()
  await expect(province).toHaveValue('__unlocated__')
  await expect(cards).toHaveCount(2)
  await cards.first().click()
  await expect(page.getByRole('heading', { level: 1 })).toHaveText('未明确省份11')
  await page.goBack()
  await expect(province).toHaveValue('__unlocated__')
  await expect(page).toHaveURL(/page=2/)
  await expect(cards).toHaveCount(2)
  await page.getByLabel('搜索公告').fill('未明确省份12')
  await page.getByLabel('搜索公告').press('Enter')
  await expect(cards).toHaveCount(1)
  await expect(province.getByRole('option', { name: '省份未明确 (1)', exact: true })).toBeEnabled()
  await noHorizontalOverflow(page)
  await screenshot(page, testInfo, '未明确省份筛选')
  await page.getByLabel('搜索公告').fill('省份验收没有匹配记录')
  await page.getByLabel('搜索公告').press('Enter')
  await expect(page.getByRole('status')).toContainText('暂无相关公告')
  await expect(province.getByRole('option', { name: /省份未明确/ })).toHaveCount(0)
  await expect(province).toHaveValue('__unlocated__')
  await page.reload()
  await expect(province).toHaveValue('__unlocated__')
  await page.getByRole('button', { name: '重置', exact: true }).click()
  await expect(province).toHaveValue('')
  await expect(cards).toHaveCount(10)
})

for (const [route, endpoint, detail, name] of [
  ['/news', '/info-publications/company-news', '/news/', '新闻中心'],
  ['/policies', '/info-publications/policy-regulations', '/policy/', '政策法规']
]) {
  test(`${name}摘要、搜索、详情、面包屑与刷新`, async ({ page, request }) => {
    const data = await api(request, endpoint + '?pageSize=1')
    const item = data.items[0]
    expect(item).toBeTruthy()
    await page.goto(route)
    const cards = page.locator('main [role="link"]')
    await expect(cards.first()).toBeVisible()
    expect(await cards.allTextContents()).not.toEqual([])
    for (const content of await cards.allTextContents()) expect(content).not.toMatch(/<\/?(?:p|span|div|style|table)\b|&nbsp;/i)
    const input = page.getByPlaceholder(/请输入您想检索/)
    await input.fill(item.title.slice(0, 12))
    await input.press('Enter')
    await expect(page).toHaveURL(/keyword=/)
    await expect(cards.filter({ hasText: item.title })).toHaveCount(1)
    await page.reload()
    await expect(input).toHaveValue(item.title.slice(0, 12))
    await cards.filter({ hasText: item.title }).click()
    await expect(page).toHaveURL(new RegExp(detail + item.id + '$'))
    await expect(page.getByRole('heading', { level: 1 })).toHaveText(item.title)
    await expect(page.locator('.rich-text')).not.toBeEmpty()
    await expect(page).toHaveTitle(new RegExp(item.title.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')))
    await noHorizontalOverflow(page)
    await page.getByRole('navigation', { name: '面包屑导航' }).getByRole('link', { name, exact: true }).click()
    await expect(page).toHaveURL(new RegExp(route + '\\?keyword='))
    await expect(input).toHaveValue(item.title.slice(0, 12))
  })
}

test('五个企业栏目、资质详情、荣誉图片预览与返回', async ({ page }, testInfo) => {
  await page.goto('/about')
  const tabs = page.getByRole('navigation', { name: '公司信息栏目' })
  await expect(page.locator('.rich-text')).not.toBeEmpty()
  for (const [tab, name] of [['business', '业务范围'], ['qualifications', '企业资质'], ['achievements', '重要业绩'], ['honors', '企业荣誉'], ['intro', '企业简介']]) {
    await tabs.getByRole('link', { name, exact: true }).click()
    await expect(page).toHaveURL(new RegExp('tab=' + tab))
    await expect(tabs.getByRole('link', { name, exact: true })).toHaveAttribute('aria-current', 'page')
    await expect(page.locator('[aria-busy="true"]')).toHaveCount(0)
    if (tab === 'achievements') await expect(page.getByRole('status')).toContainText('暂无重要业绩内容')
    else if (tab !== 'intro') await expect(page.locator('main section h3').first()).toBeVisible()
    await noHorizontalOverflow(page)
    if (tab === 'business') {
      await page.locator('main section a').first().click()
      await expect(page).toHaveURL(/\/business-scope\//)
      await expect(page.getByRole('heading', { level: 1 })).toBeVisible()
      await noHorizontalOverflow(page)
      await page.getByRole('navigation', { name: '面包屑导航' }).getByRole('link', { name, exact: true }).click()
      await expect(page).toHaveURL(/tab=business/)
    }
    if (tab === 'qualifications') {
      await healthyImages(page, page.locator('main section img'))
      await screenshot(page, testInfo, '企业资质')
      await page.locator('main section a').first().click()
      await expect(page).toHaveURL(/\/qualification\//)
      await expect(page.getByRole('heading', { level: 1 })).toBeVisible()
      await healthyImages(page, page.locator('main img'))
      await noHorizontalOverflow(page)
      await page.getByRole('navigation', { name: '面包屑导航' }).getByRole('link', { name, exact: true }).click()
    }
    if (tab === 'honors') {
      await healthyImages(page, page.locator('main section img'))
      const honor = page.locator('main section button').first()
      await honor.press('Enter')
      const dialog = page.getByRole('dialog', { name: '图片预览', exact: true })
      await expect(dialog).toBeVisible()
      await healthyImages(page, dialog.locator('img'))
      await noHorizontalOverflow(page)
      await page.keyboard.press('Escape')
      await expect(dialog).toHaveCount(0)
      await expect(honor).toBeFocused()
    }
  }
  await page.goto('/about?tab=honors')
  await expect(page.getByRole('heading', { name: '企业荣誉', exact: true })).toBeVisible()
})

test('历史扫描件正文与真实附件访问', async ({ page, request }) => {
  const scan = (await api(request, '/info-publications/company-news?keyword=' + encodeURIComponent('河南招标采购综合网关于停止发布'))).items[0]
  expect(scan).toBeTruthy()
  await page.goto('/news/' + scan.id)
  await expect(page.locator('.rich-text img')).toHaveCount(2)
  await healthyImages(page, page.locator('.rich-text img'))
  await noHorizontalOverflow(page)
  const document = (await api(request, '/info-publications/company-news?keyword=' + encodeURIComponent('招标采购管理人员高级研修班'))).items[0]
  expect(document).toBeTruthy()
  await page.goto('/news/' + document.id)
  const attachment = page.locator('.rich-text a[href^="/uploads/"]').first()
  await expect(attachment).toBeVisible()
  const response = await request.get(await attachment.getAttribute('href'))
  expect(response.ok()).toBeTruthy()
  expect((await response.body()).byteLength).toBeGreaterThan(0)
  await noHorizontalOverflow(page)
})

test('宽表格、大图、长英文标题与长附件名保持可读', async ({ page }, testInfo) => {
  const title = '工程咨询项目-' + 'LongProjectReference'.repeat(12)
  const imageUrl = '/src/assets/logo.png'
  await page.route('**/api/announcements/by-hash/portal-layout', route => fulfill(route, {
    id: 999999, hashId: 'portal-layout', title, noticeType: 'result', businessType: 'GOV_PROCUREMENT',
    bidder: 'Purchaser'.repeat(40), winner: 'Contractor'.repeat(40), budgetAmount: 123.45, awardAmount: 100,
    publishTime: '2026-10-06T09:00:00', province: '河南省', city: '郑州市',
    content: `<p>宽表格验收</p><table style="width:1600px"><tr><th>项目</th><th>金额</th></tr><tr><td>完整表格最后一列应可滚动查看</td><td>100万元</td></tr></table><p><a href="https://example.invalid/">${'https://example.invalid/' + 'reference'.repeat(60)}</a></p><p><img src="${imageUrl}" style="width:2000px;height:2000px"></p>`,
    attachments: [{ id: 999999, fileName: '长附件名称-' + 'attachment'.repeat(30) + '.png', fileUrl: imageUrl, fileSize: 1024 }]
  }))
  await page.goto('/announcement/portal-layout')
  await expect(page.getByRole('heading', { level: 1 })).toHaveText(title)
  await healthyImages(page, page.locator('.rich-text img'))
  await noHorizontalOverflow(page)
  const rich = page.locator('.rich-text')
  expect(await rich.evaluate(node => node.scrollWidth > node.clientWidth)).toBe(true)
  expect(await rich.evaluate(node => getComputedStyle(node).overflowX)).toBe('auto')
  await rich.evaluate(node => { node.scrollLeft = node.scrollWidth })
  expect(await rich.evaluate(node => node.scrollLeft)).toBeGreaterThan(0)
  await expect(page.getByText('预算金额', { exact: true })).toBeVisible()
  await expect(page.getByText('中标 / 成交金额', { exact: true })).toBeVisible()
  await page.getByRole('button', { name: '预览', exact: true }).click()
  const dialog = page.getByRole('dialog', { name: '附件预览' })
  await expect(dialog).toBeVisible()
  await expect(dialog.getByRole('button', { name: '关闭附件预览' })).toBeVisible()
  await noHorizontalOverflow(page)
  expect(await dialog.evaluate(node => node.scrollWidth - node.clientWidth)).toBeLessThanOrEqual(1)
  await screenshot(page, testInfo, '长附件预览', false)
  await page.keyboard.press('Escape')
  await expect(dialog).not.toBeVisible()
  await screenshot(page, testInfo, '富文本边界')
})

test('列表失败重试、空结果与不存在内容', async ({ page, expectedFailures }) => {
  let fail = true
  expectedFailures.add('503 /api/info-publications/company-news')
  await page.route('**/api/info-publications/company-news?**', route => fail
    ? route.fulfill({ status: 503, json: { success: false } }) : fulfill(route, { items: [], totalCount: 0 }))
  await page.goto('/news')
  await expect(page.getByRole('alert')).toContainText('暂时无法连接服务')
  fail = false
  await page.getByRole('button', { name: '重新加载', exact: true }).click()
  await expect(page.getByRole('status')).toContainText('暂无相关新闻资讯')
  await expect(page.getByRole('navigation', { name: '列表分页' })).toHaveCount(0)
  await page.route('**/api/announcements/by-hash/not-found', route => route.fulfill({ status: 404, json: { success: false } }))
  expectedFailures.add('404 /api/announcements/by-hash/not-found')
  await page.goto('/announcement/not-found')
  await expect(page.getByText('内容不存在或已下架', { exact: true })).toBeVisible()
  await expect(page.locator('meta[name="robots"]')).toHaveAttribute('content', /noindex/)
  await page.getByRole('link', { name: '返回列表', exact: true }).click()
  await expect(page).toHaveURL(/\/announcements$/)
  await page.goto('/portal-page-does-not-exist')
  await expect(page.getByRole('heading', { name: '页面不存在', exact: true })).toBeVisible()
  await expect(page.locator('meta[name="robots"]')).toHaveAttribute('content', /noindex/)
  await page.locator('main').getByRole('link', { name: '返回首页', exact: true }).click()
  await expect(page).toHaveURL(/\/$/)
  await noHorizontalOverflow(page)
})

test('标语首尾字、对称展开和逐字浮现，刷新可重播', async ({ page, request }) => {
  // 按固定帧步长检查真实 DOM，避免后台浏览器的帧节流跳过 100ms 浮现阶段。
  const initialTime = new Date('2026-10-06T01:00:00Z')
  await page.clock.install({ time: initialTime })
  await page.clock.pauseAt(new Date(initialTime.getTime() + 60_000))
  const settings = await api(request, '/config/site-settings')
  const slogan = '海纳百川 才望兼隆'
  await page.route('**/api/config/site-settings', route => fulfill(route, { ...settings, company: { ...settings.company, slogan } }))
  await page.addInitScript(() => {
    window.sloganFrames = []
    const sample = () => {
      const root = document.querySelector('.slogan-reveal')
      if (root && root.dataset.state !== 'complete') {
        window.sloganFrames.push({ state: root.dataset.state, glyphs: [...root.querySelectorAll('.slogan-glyph')].map(node => ({ text: node.textContent, opacity: Number(getComputedStyle(node).opacity), x: node.getBoundingClientRect().x, width: node.getBoundingClientRect().width })) })
      }
      if (!root || root.dataset.state !== 'complete') requestAnimationFrame(sample)
    }
    requestAnimationFrame(sample)
  })
  const verify = async () => {
    await expect(page.locator('.slogan-reveal')).toHaveAttribute('data-state', 'complete')
    await expect(page.getByRole('heading', { level: 1 })).toHaveAttribute('aria-label', slogan)
    const frames = await page.evaluate(() => window.sloganFrames)
    expect(frames.some(frame => frame.glyphs.filter(g => g.opacity > .99).map(g => g.text).join('') === '海隆')).toBe(true)
    expect(frames.some(frame => frame.glyphs.some(g => g.opacity > 0 && g.opacity < 1))).toBe(true)
    const playing = frames.filter(frame => frame.state === 'playing' && frame.glyphs.length === 9)
    expect(playing.length).toBeGreaterThan(2)
    const rootCenter = await page.locator('.slogan-reveal').evaluate(node => { const r = node.getBoundingClientRect(); return r.x + r.width / 2 })
    for (const frame of playing) {
      const glyphs = frame.glyphs, center = (glyphs[0].x + glyphs.at(-1).x + glyphs.at(-1).width) / 2
      expect(Math.abs(center - rootCenter)).toBeLessThan(1)
      for (let i = 1; i < glyphs.length / 2; i++) expect(Math.abs(glyphs[i].opacity - glyphs[glyphs.length - 1 - i].opacity)).toBeLessThan(.01)
    }
    expect(await page.locator('.slogan-glyph').evaluateAll(nodes => nodes.every(node => Number(node.style.opacity) === 1))).toBe(true)
    await noHorizontalOverflow(page)
  }
  const open = async reload => {
    const configResponse = page.waitForResponse(response => new URL(response.url()).pathname === '/api/config/site-settings')
    if (reload) await page.reload()
    else await page.goto('/')
    await configResponse
    await expect(page.getByRole('heading', { level: 1 })).toHaveAttribute('aria-label', slogan)
    await expect(page.locator('.slogan-layer')).toBeVisible()
    await page.clock.runFor(1800)
  }
  await open(false)
  await verify()
  await open(true)
  await verify()
  await page.setViewportSize({ width: 844, height: 390 })
  await expect(page.locator('.slogan-reveal')).toHaveAttribute('data-state', 'complete')
  expect(await page.locator('.slogan-glyph').evaluateAll(nodes => nodes.every(node => Number(node.style.opacity) === 1))).toBe(true)
})

test('减少动态效果时标语直接完整展示', async ({ page }) => {
  await page.emulateMedia({ reducedMotion: 'reduce' })
  await page.addInitScript(() => {
    window.sloganPlayed = false
    new MutationObserver(() => {
      if (document.querySelector('.slogan-reveal[data-state="playing"]')) window.sloganPlayed = true
    }).observe(document, { subtree: true, attributes: true, attributeFilter: ['data-state'] })
  })
  await page.goto('/')
  await expect(page.locator('.slogan-reveal')).toHaveAttribute('data-state', 'complete')
  expect(await page.evaluate(() => window.sloganPlayed)).toBe(false)
  expect(await page.locator('.slogan-glyph').evaluateAll(nodes => nodes.every(node => Number(node.style.opacity) === 1))).toBe(true)
  expect(await page.evaluate(() => getComputedStyle(document.documentElement).scrollBehavior)).toBe('auto')
  await noHorizontalOverflow(page)
})

test('多行配置标语与短标语在手机上完整显示', async ({ page, request }) => {
  const settings = await api(request, '/config/site-settings')
  let slogan = '海纳百川 才望兼隆 专业服务支撑项目发展'
  await page.route('**/api/config/site-settings', route => fulfill(route, { ...settings, company: { ...settings.company, slogan } }))
  await page.setViewportSize({ width: 360, height: 740 })
  await page.goto('/')
  const reveal = page.locator('.slogan-reveal')
  await expect(reveal).toHaveAttribute('data-state', 'complete')
  const bounds = await reveal.boundingBox()
  const glyphs = await page.locator('.slogan-glyph').evaluateAll(nodes => nodes.map(node => ({ opacity: Number(node.style.opacity), rect: node.getBoundingClientRect().toJSON() })))
  expect(new Set(glyphs.map(item => Math.round(item.rect.top))).size).toBeGreaterThan(1)
  for (const glyph of glyphs) {
    expect(glyph.opacity).toBe(1)
    expect(glyph.rect.right).toBeLessThanOrEqual(bounds.x + bounds.width + 1)
    expect(glyph.rect.bottom).toBeLessThanOrEqual(bounds.y + bounds.height + 1)
  }
  await noHorizontalOverflow(page)
  slogan = '海隆'
  await page.reload()
  await expect(page.getByRole('heading', { level: 1 })).toHaveAttribute('aria-label', slogan)
  await expect(reveal).toHaveAttribute('data-state', 'complete')
  await expect(page.locator('.slogan-glyph')).toHaveCount(2)
  await noHorizontalOverflow(page)
})

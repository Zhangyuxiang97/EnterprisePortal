import { chromium } from 'playwright'
import { expect } from 'playwright/test'
import { mkdir } from 'node:fs/promises'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

// UI 使用临时响应，后端关系数据库测试负责真实筛选、计数与更新合同。
const base = process.env.ADMIN_TEST_URL || 'http://127.0.0.1:3002'
if (!['127.0.0.1', 'localhost'].includes(new URL(base).hostname)) throw Error('仅允许本机 UI 验收')
const output = fileURLToPath(new URL('../../.runtime/province-admin-browser/', import.meta.url))
await mkdir(output, { recursive: true })
const provinces = [{ regionCode: '410000', regionName: '河南省' }, { regionCode: '440000', regionName: '广东省' }]
const browser = await chromium.launch()
try {
  for (const [businessType, routePath, prefix] of [
    ['GOV_PROCUREMENT', '/gov-procurement', '采购'], ['CONSTRUCTION', '/construction', '建设']
  ]) {
    const context = await browser.newContext({ viewport: { width: 1440, height: 1000 } })
    const page = await context.newPage(), errors = [], queries = [], updates = []
    page.on('pageerror', error => errors.push(error.message))
    await page.addInitScript(() => localStorage.setItem('hailong_admin_token', 'mock-province-ui-session'))
    const records = ['', '', '410000', '440000'].map((province, i) => ({ id: i + 1,
      title: [prefix + '未明确一', prefix + '未明确二', prefix + '河南公告', prefix + '广东公告'][i],
      businessType, noticeType: 'bidding', procurementType: businessType === 'GOV_PROCUREMENT' ? 'service' : null,
      content: '<p>省份补充验收正文</p>', province, provinceCode: province, city: '', district: '', projectRegion: '',
      publishTime: '2026-10-14T09:00:00', version: 'original', status: i === 1 ? 0 : 1, isTop: false, viewCount: 0, attachments: []
    }))
    await page.route(base + '/api/**', async route => {
      const url = new URL(route.request().url()), query = url.searchParams
      const pathname = url.pathname
      let data
      if (pathname === '/api/auth/me') data = { id: 1, username: 'ui-test', fullName: 'UI验收', role: 'admin' }
      else if (pathname === '/api/regions/provinces') data = provinces
      else if (pathname === '/api/regions/cities') data = [{ regionCode: '410100', regionName: '郑州市' }]
      else if (pathname === '/api/regions/tree') data = provinces.map(p => ({ code: p.regionCode, name: p.regionName, children: [] }))
      else if (pathname === '/api/announcements/manage' || pathname === '/api/announcements/manage/region-options') {
        expect(query.get('businessType')).toBe(businessType)
        const rows = records.filter(row => (!query.get('keyword') || row.title.includes(query.get('keyword')))
          && (!query.get('noticeType') || row.noticeType === query.get('noticeType')))
        queries.push({ pathname, ...Object.fromEntries(query) })
        if (pathname.endsWith('/region-options')) data = {
          provinces: provinces.map(p => ({ ...p, count: rows.filter(row => row.province === p.regionCode).length })),
          provinceTotalCount: rows.length, provinceUnlocatedCount: rows.filter(row => !row.province).length,
          cities: [], districts: []
        }
        else {
          const selected = rows.filter(row => (!query.get('province') || row.province === query.get('province'))
            && (query.get('provinceUnlocated') !== 'true' || !row.province))
          data = { items: selected, totalCount: selected.length }
        }
      } else if (/^\/api\/announcements\/manage\/\d+$/.test(pathname)) {
        data = records.find(row => row.id === Number(pathname.split('/').at(-1)))
      } else if (/^\/api\/announcements\/\d+$/.test(pathname) && route.request().method() === 'PUT') {
        const update = route.request().postDataJSON()
        expect(update.province).toBe('410000')
        expect(update.provinceUnlocated).toBeUndefined()
        updates.push(update)
        const row = records.find(row => row.id === Number(pathname.split('/').at(-1)))
        Object.assign(row, update, { provinceCode: update.province, version: 'updated' }); data = row
      } else data = []
      await route.fulfill({ json: { success: true, data } })
    })
    await page.goto(base + routePath)
    const region = page.locator('.search-form .region-selector')
    const province = region.locator('.el-select').first()
    const select = async label => {
      await province.click()
      await page.getByRole('option', { name: label, exact: true }).click()
    }
    const search = () => page.getByRole('button', { name: '搜索', exact: true }).click()
    await expect(province).toContainText('全部省份 (4)')
    await province.click()
    await expect(page.getByRole('option', { name: '河南省 (1)', exact: true })).toBeVisible()
    await expect(page.getByRole('option', { name: '广东省 (1)', exact: true })).toBeVisible()
    await page.getByRole('option', { name: '省份未明确 (2)', exact: true }).click()
    await expect(region.locator('.el-select')).toHaveCount(1)
    await search()
    await expect(page.locator('.el-table__row')).toHaveCount(2)
    const requested = queries.filter(q => q.pathname === '/api/announcements/manage').at(-1)
    expect(requested.provinceUnlocated).toBe('true')
    expect(requested.province).toBeUndefined(); expect(requested.city).toBeUndefined()
    await page.getByPlaceholder(/搜索标题、.*、中标人/).fill('未明确一')
    await search()
    await expect(province).toContainText('省份未明确 (1)')
    await expect(page.locator('.el-table__row')).toHaveCount(1)
    await page.getByPlaceholder(/搜索标题、.*、中标人/).fill('省份验收没有匹配记录')
    await search()
    await expect(province).toContainText('省份未明确')
    await expect(province).not.toContainText('(0)')
    await expect(page.locator('.el-table__row')).toHaveCount(0)
    await province.click()
    await expect(page.getByRole('option', { name: /省份未明确/ })).toHaveCount(0)
    await page.locator('.card-header').click()
    await page.getByRole('button', { name: '重置', exact: true }).click()
    await expect(province).toContainText('全部省份 (4)')
    await select('河南省 (1)')
    await expect(region.locator('.el-select')).toHaveCount(2)
    await select('省份未明确 (2)')
    await expect(region.locator('.el-select')).toHaveCount(1)
    await search()
    await expect(page.locator('.el-table__row')).toHaveCount(2)
    await page.locator('.el-table__row').filter({ hasText: prefix + '未明确一' }).getByRole('button', { name: '编辑', exact: true }).click()
    const dialog = page.locator('.editor-dialog:visible')
    await expect(dialog).toBeVisible()
    await expect(dialog.locator('[data-slate-editor]')).toContainText('省份补充验收正文')
    await dialog.locator('.el-cascader').click()
    await page.locator('.el-cascader-node').filter({ hasText: '河南省' }).locator('.el-radio').click()
    await dialog.locator('.el-dialog__header').click()
    await dialog.getByRole('button', { name: '提交', exact: true }).click()
    await expect(dialog).toHaveCount(0)
    await expect(page.locator('.el-table__row')).toHaveCount(1)
    await expect(province).toContainText('省份未明确 (1)')
    expect(updates).toHaveLength(1)
    await select('河南省 (2)')
    await search()
    await expect(page.locator('.el-table__row')).toHaveCount(2)
    await expect(page.locator('.el-table')).toContainText(prefix + '未明确一')
    await province.click()
    await expect(page.getByRole('option', { name: '全部省份 (4)', exact: true })).toBeVisible()
    await page.screenshot({ path: path.join(output, `${businessType}.png`) })
    expect(errors).toEqual([])
    console.log(`PASS ${prefix}公告：省份数量、未明确筛选、搜索重置和补充后刷新`)
    await context.close()
  }
} finally { await browser.close() }

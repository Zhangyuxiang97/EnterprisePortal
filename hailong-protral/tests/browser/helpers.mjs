import { test as base, expect } from 'playwright/test'

export const test = base.extend({
  expectedFailures: async ({}, use) => { await use(new Set()) },
  browserAudit: [async ({ page, baseURL, expectedFailures }, use) => {
    const errors = [], failures = []
    page.on('pageerror', error => errors.push(error.message))
    page.on('response', response => {
      const url = new URL(response.url())
      if (url.origin === new URL(baseURL).origin && response.status() >= 400 && /^\/(api|uploads)\//.test(url.pathname)) {
        failures.push(`${response.status()} ${url.pathname}`)
      }
    })
    await use()
    expect(errors, '页面运行时异常').toEqual([])
    expect(failures.filter(item => !expectedFailures.has(item)), '非预期的 API / 上传资源错误').toEqual([])
  }, { auto: true }]
})
export { expect }

export async function api(request, route) {
  const response = await request.get('/api' + route)
  expect(response.ok(), route).toBeTruthy()
  const body = await response.json()
  expect(body.success, route).toBe(true)
  return body.data
}

export async function noHorizontalOverflow(page) {
  await expect.poll(() => page.evaluate(() => Math.max(
    document.documentElement.scrollWidth - document.documentElement.clientWidth,
    document.body.scrollWidth - document.documentElement.clientWidth
  )), { message: '页面整体出现横向溢出' }).toBeLessThanOrEqual(1)
}

export async function healthyImages(page, locator) {
  for (const image of await locator.all()) {
    await image.scrollIntoViewIfNeeded()
    await expect.poll(() => image.evaluate(node => node.complete && node.naturalWidth > 0), { message: `图片加载失败: ${await image.getAttribute('src')}` }).toBe(true)
    const bounds = await image.boundingBox()
    expect(bounds.width).toBeGreaterThan(0)
  }
}

export async function screenshot(page, testInfo, name, fullPage = true) {
  const path = testInfo.outputPath(`${name}.png`)
  await page.screenshot({ path, fullPage })
  await testInfo.attach(name, { path, contentType: 'image/png' })
}

export const success = data => ({ success: true, data })
export const fulfill = (route, data) => route.fulfill({ json: success(data) })

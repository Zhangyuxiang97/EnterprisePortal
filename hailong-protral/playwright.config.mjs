import { defineConfig } from 'playwright/test'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const directory = path.dirname(fileURLToPath(import.meta.url))
const runtime = path.resolve(directory, '../.runtime/portal-browser')
const baseURL = process.env.PORTAL_TEST_URL || 'http://127.0.0.1:3000'
const target = new URL(baseURL)
if (!['localhost', '127.0.0.1'].includes(target.hostname) || target.protocol !== 'http:') {
  throw new Error('门户验收仅允许本机 HTTP 测试服务')
}
const chromium = { browserName: 'chromium', ...(process.env.PORTAL_TEST_CHANNEL ? { channel: process.env.PORTAL_TEST_CHANNEL } : {}) }

export default defineConfig({
  testDir: './tests/browser',
  timeout: 45_000,
  expect: { timeout: 10_000 },
  fullyParallel: false,
  workers: 1,
  retries: 0,
  outputDir: path.join(runtime, 'results'),
  reporter: [
    ['list'],
    ['html', { outputFolder: path.join(runtime, 'report'), open: 'never' }],
    ['json', { outputFile: path.join(runtime, 'results.json') }]
  ],
  use: {
    baseURL,
    locale: 'zh-CN',
    timezoneId: 'Asia/Shanghai',
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure'
  },
  projects: [
    { name: 'desktop', use: { ...chromium, viewport: { width: 1440, height: 1000 } } },
    { name: 'tablet', use: { ...chromium, viewport: { width: 768, height: 1024 }, hasTouch: true } },
    { name: 'phone', use: { ...chromium, viewport: { width: 390, height: 844 }, isMobile: true, hasTouch: true, deviceScaleFactor: 2 } },
    { name: 'small-phone', use: { ...chromium, viewport: { width: 360, height: 740 }, isMobile: true, hasTouch: true, deviceScaleFactor: 2 } },
    // Windows WebKit 的导航和受控帧执行较慢，单独给足等待时间，不自动重试。
    { name: 'webkit-phone', timeout: 120_000, expect: { timeout: 20_000 }, use: { browserName: 'webkit', viewport: { width: 390, height: 844 }, isMobile: true, hasTouch: true, deviceScaleFactor: 2 } }
  ],
  webServer: {
    command: `node node_modules/vite/bin/vite.js --host 127.0.0.1 --port ${target.port || 80} --strictPort`,
    url: baseURL,
    reuseExistingServer: !process.env.CI,
    env: { BROWSER: 'none', VITE_API_BASE_URL: '/api', API_PROXY_TARGET: process.env.PORTAL_TEST_API_PROXY || 'http://127.0.0.1:5000' }
  }
})

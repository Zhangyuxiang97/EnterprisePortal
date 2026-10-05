import { spawn } from 'node:child_process'
import fs from 'node:fs'
import path from 'node:path'
import { randomBytes } from 'node:crypto'
import { fileURLToPath } from 'node:url'

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..')
const runtime = path.join(root, '.runtime', 'ci-browser')
fs.mkdirSync(runtime, { recursive: true })
const children = []
function start(exe, args, cwd, env = {}) {
  const child = spawn(exe, args, { cwd, env: { ...process.env, ...env }, stdio: 'ignore' })
  children.push(child); return child
}
async function waitFor(url, child) {
  for (let i = 0; i < 240; i++) {
    if (child.exitCode !== null) throw Error(`本地测试服务启动失败: ${new URL(url).port}`)
    try { if ((await fetch(url)).ok) return } catch {}
    await new Promise(resolve => setTimeout(resolve, 500))
  }
  throw Error(`本地测试服务未就绪: ${url}`)
}
try {
  const api = start('dotnet', [path.join(root, 'BackEnd/HailongConsulting.API/bin/Release/net8.0/HailongConsulting.API.dll')], runtime, {
    ASPNETCORE_ENVIRONMENT: 'Development', ASPNETCORE_URLS: 'http://127.0.0.1:5000', ASPNETCORE_CONTENTROOT: path.join(root, 'BackEnd/HailongConsulting.API'),
    ConnectionStrings__DefaultConnection: process.env.CI_DATABASE_CONNECTION,
    Jwt__Key: randomBytes(48).toString('base64')
  })
  await waitFor('http://127.0.0.1:5000/health/ready', api)
  const admin = start(process.execPath, [path.join(root, 'hailong-admin/node_modules/vite/bin/vite.js'), '--host', '127.0.0.1', '--port', '3002', '--strictPort'], path.join(root, 'hailong-admin'), { VITE_API_BASE_URL: '/api', API_PROXY_TARGET: 'http://127.0.0.1:5000' })
  await waitFor('http://127.0.0.1:3002', admin)
  const credentials = fs.readFileSync(path.join(runtime, 'logs/bootstrap/initial-admin-credentials.txt'), 'utf8')
  const env = { ...process.env, ADMIN_TEST_CHANNEL: 'chromium', ADMIN_TEST_USERNAME: credentials.match(/^Username: (.+)$/m)[1].trim(), ADMIN_TEST_PASSWORD: credentials.match(/^Password: (.+)$/m)[1].trim() }
  for (const script of ['admin.smoke.mjs', 'backend-cleanup.smoke.mjs']) {
    const test = spawn(process.execPath, [path.join(root, 'hailong-admin/tests', script)], { cwd: root, env, stdio: 'inherit' })
    const code = await new Promise(resolve => test.on('exit', resolve))
    if (code !== 0) throw Error(`浏览器回归失败: ${script}`)
  }
} catch (error) { console.error(error.message); process.exitCode = 1 }
finally { for (const child of children.reverse()) child.kill('SIGTERM') }

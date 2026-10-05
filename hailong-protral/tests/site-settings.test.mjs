import { test } from 'node:test'
import assert from 'node:assert/strict'
import { readFileSync } from 'node:fs'
import { createSiteSettingsLoader } from '../src/utils/siteSettingsLoader.js'

const defaults = JSON.parse(readFileSync(new URL('../../BackEnd/HailongConsulting.API/SiteSettingsDefaults.json', import.meta.url)))
const response = { success: true, data: defaults }
const tick = () => new Promise(resolve => setImmediate(resolve))

test('首次失败后后台重试成功，缓存最新内容且成功后不重复请求', async () => {
  let count = 0
  let applied
  let cache
  const scheduled = []
  const loader = createSiteSettingsLoader({
    fetchSettings: async () => { if (++count === 1) throw new Error('暂时断网'); return response },
    applySettings: data => { applied = data },
    storage: { getItem: () => null, setItem: (_, value) => { cache = JSON.parse(value) } },
    schedule: (fn, delay) => scheduled.push({ fn, delay })
  })
  await loader.load()
  assert.equal(count, 1)
  assert.equal(scheduled[0].delay, 500)
  scheduled[0].fn()
  await tick()
  assert.equal(count, 2)
  assert.equal(applied.company.slogan, defaults.company.slogan)
  assert.deepEqual(cache.data, defaults)
  await loader.load()
  assert.equal(count, 2)
})

test('持续失败最多尝试三次，不阻塞默认展示', async () => {
  let count = 0
  const scheduled = []
  const loader = createSiteSettingsLoader({ fetchSettings: async () => { count++; throw new Error('失败') },
    applySettings: () => assert.fail('失败时不应替换内容'), schedule: (fn, delay) => scheduled.push({ fn, delay }) })
  await loader.load()
  while (scheduled.length) { scheduled.shift().fn(); await tick() }
  await loader.load()
  assert.equal(count, 3)
})

test('并行加载复用同一个请求', async () => {
  let count = 0
  let complete
  const loader = createSiteSettingsLoader({ fetchSettings: () => { count++; return new Promise(resolve => { complete = resolve }) }, applySettings: () => {} })
  const first = loader.load()
  assert.equal(loader.load(), first)
  await tick()
  complete(response)
  await first
  assert.equal(count, 1)
})

test('有效缓存立即展示，过期和损坏缓存不覆盖默认内容', () => {
  for (const [savedAt, data, expected] of [[950, defaults, 1], [0, defaults, 0], [950, {}, 0]]) {
    let applied = 0
    createSiteSettingsLoader({ fetchSettings: async () => response, applySettings: () => applied++, now: () => savedAt === 0 ? 8 * 86400000 : 1000,
      storage: { getItem: () => JSON.stringify({ savedAt, data }) } })
    assert.equal(applied, expected)
  }
})

test('存储访问异常和错误响应结构不会中断页面', async () => {
  const scheduled = []
  let applied = 0
  const loader = createSiteSettingsLoader({ fetchSettings: async () => ({ success: true, data: {} }), applySettings: () => applied++,
    storage: { getItem: () => { throw new Error('存储禁用') } }, schedule: fn => scheduled.push(fn) })
  await loader.load()
  assert.equal(applied, 0)
  assert.equal(scheduled.length, 1)
})

import { test } from 'node:test'
import assert from 'node:assert/strict'
import { effectScope, ref, nextTick } from 'vue'
import { useResource } from '../src/composables/useResource.js'
import { useDetail } from '../src/composables/useDetail.js'
import { getDateRange } from '../src/utils/date.js'
import { buildPageMetadata } from '../src/utils/metadata.js'
import { queryPage } from '../src/utils/query.js'

const deferred = () => { let resolve, reject; const promise = new Promise((yes, no) => { resolve = yes; reject = no }); return { promise, resolve, reject } }

test('旧成功响应不得覆盖新请求，旧请求结束不得提前关闭 loading', async () => {
  const scope = effectScope()
  const state = scope.run(() => useResource([]))
  const old = deferred(), latest = deferred()
  let signal
  const first = state.execute(current => { signal = current; return old.promise })
  const second = state.execute(() => latest.promise)
  assert.equal(signal.aborted, true)
  old.resolve(['旧结果']); await first
  assert.equal(state.loading.value, true)
  assert.deepEqual(state.data.value, [])
  latest.resolve(['新结果']); await second
  assert.deepEqual(state.data.value, ['新结果'])
  assert.equal(state.loading.value, false)
  scope.stop()
})

test('旧请求失败不能覆盖新结果，当前失败可重试', async () => {
  const scope = effectScope(), old = deferred()
  const state = scope.run(() => useResource([]))
  const first = state.execute(() => old.promise)
  await state.execute(async () => ['最新'])
  old.reject(new Error('旧错误')); await first
  assert.deepEqual(state.data.value, ['最新'])
  assert.equal(state.error.value, '')
  await state.execute(async () => { throw new Error('请求失败') })
  assert.equal(state.error.value, '请求失败')
  assert.deepEqual(state.data.value, [])
  await state.execute(async () => ['重试成功'])
  assert.equal(state.error.value, '')
  assert.deepEqual(state.data.value, ['重试成功'])
  scope.stop()
})

test('组件卸载后在途请求不得更新数据', async () => {
  const scope = effectScope(), pending = deferred()
  const state = scope.run(() => useResource())
  const task = state.execute(() => pending.promise)
  scope.stop(); pending.resolve('已卸载'); await task
  assert.equal(state.data.value, null)
})

test('详情 ID 改变后重新读取，忽略旧 ID 的迟到结果', async () => {
  const scope = effectScope(), id = ref('a'), a = deferred(), b = deferred()
  const state = scope.run(() => useDetail(() => id.value, key => key === 'a' ? a.promise : b.promise))
  id.value = 'b'; await nextTick()
  b.resolve({ success: true, data: { id: 'b', title: '第二篇' } }); await new Promise(resolve => setImmediate(resolve))
  a.resolve({ success: true, data: { id: 'a' } }); await new Promise(resolve => setImmediate(resolve))
  assert.equal(state.data.value.id, 'b')
  scope.stop()
})

test('北京时间凌晨当天日期正确，近三天包含今天且跨月正确', () => {
  const previous = process.env.TZ
  process.env.TZ = 'Asia/Shanghai'
  try {
    const now = new Date('2026-10-01T01:00:00+08:00')
    assert.deepEqual(getDateRange('today', now), { startDate: '2026-10-01', endDate: '2026-10-01' })
    assert.deepEqual(getDateRange('3days', now), { startDate: '2026-09-29', endDate: '2026-10-01' })
    assert.deepEqual(getDateRange('week', now), { startDate: '2026-09-25', endDate: '2026-10-01' })
  } finally { if (previous === undefined) delete process.env.TZ; else process.env.TZ = previous }
})

test('规范地址使用实际域名和文章路径，404 禁止索引', () => {
  const result = buildPageMetadata({ origin: 'https://portal.example.cn', path: '/news/12', company: { fullName: '海隆', description: '工程咨询' }, title: '新闻标题' })
  assert.equal(result.title, '新闻标题 | 海隆')
  assert.equal(result.canonical, 'https://portal.example.cn/news/12')
  assert.equal(result.description, '工程咨询')
  assert.equal(buildPageMetadata({ origin: 'https://portal.example.cn', path: '/missing', company: {}, noindex: true }).robots, 'noindex, follow')
})

test('非法、负数和数组页码不传入后端', () => {
  for (const value of [-1, 0, '2x', '1.5', ['2','3'], Number.MAX_SAFE_INTEGER + 1]) assert.equal(queryPage(value), 1)
  assert.equal(queryPage('3'), 3)
})

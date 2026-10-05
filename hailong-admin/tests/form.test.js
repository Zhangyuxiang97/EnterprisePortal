import { test } from 'node:test'
import assert from 'node:assert/strict'
import { localDateTime, hasRichContent, filterRegionTree, certificateValidity } from '../src/utils/form.js'
import { createLatestRequest } from '../src/utils/latestRequest.js'

test('本地录入时间不通过 UTC 截断，保留本地时分秒', () => {
  assert.equal(localDateTime(new Date(2026, 9, 4, 23, 45, 6)), '2026-10-04 23:45:06')
})
test('空富文本不能提交，扫描图片和表格内容可以提交', () => {
  for (const html of ['', '<p><br></p>', '<p>&nbsp;\u200b</p>', '<style>body{color:red}</style><p> </p>']) assert.equal(hasRichContent(html), false)
  for (const html of ['<p><img src="/uploads/test.jpg"></p>', '<table><tr><td>正文</td></tr></table>']) assert.equal(hasRichContent(html), true)
})
test('地区筛选保留祖先且不改变原始树，复合条件全部生效', () => {
  const tree = [{ name: '河南省', code: '410000', level: 1, children: [
    { name: '郑州市', code: '410100', level: 2, children: [{ name: '中原区', code: '410102', level: 3, children: [] }] },
    { name: '开封市', code: '410200', level: 2, children: [] }
  ] }]
  const selected = filterRegionTree(tree, { name: '中原', level: 3 })
  assert.equal(selected[0].children[0].children[0].code, '410102')
  assert.equal(selected[0].children.length, 1)
  assert.equal(tree[0].children.length, 2)
  assert.deepEqual(filterRegionTree(tree, { name: '中原', code: '420', level: 3 }), [])
})
test('展示状态不能让过期证书变为有效，不根据空日期推断长期有效', () => {
  assert.equal(certificateValidity({ status: true, expiryDate: '2021-12-10' }, '2026-10-04'), '已到期 · 历史资料')
  assert.equal(certificateValidity({ status: true, expiryDate: null }), '未注明到期日')
  assert.equal(certificateValidity({ expiryDate: '2026-10-04' }, '2026-10-04'), '有效期内')
})
test('过时查询不能覆盖最新筛选结果', () => {
  const requests = createLatestRequest()
  const old = requests.begin(), current = requests.begin()
  assert.equal(requests.isCurrent(old), false)
  assert.equal(requests.isCurrent(current), true)
  requests.cancel()
  assert.equal(requests.isCurrent(current), false)
})

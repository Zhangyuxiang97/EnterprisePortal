import {test} from 'node:test'
import assert from 'node:assert/strict'
import {sortRegionsByCount} from '../src/utils/regionFilter.js'

test('业务地区按数量降序，同数量和零数量沿用字典顺序，更新计数后重排', () => {
  const dictionary = [
    {regionCode:'110000',sortOrder:1}, {regionCode:'120000',sortOrder:2},
    {regionCode:'310000',sortOrder:9}, {regionCode:'410000',sortOrder:16}, {regionCode:'440000',sortOrder:19}
  ]
  const before = structuredClone(dictionary)
  const counts = [{regionCode:'410000',count:10},{regionCode:'310000',count:2},{regionCode:'110000',count:2}]
  assert.deepEqual(sortRegionsByCount(dictionary,counts).map(r=>r.regionCode),['410000','110000','310000','120000','440000'])
  assert.deepEqual(dictionary,before)
  assert.deepEqual(sortRegionsByCount(dictionary,[{regionCode:'440000',count:20}]).map(r=>r.regionCode),['440000','110000','120000','310000','410000'])
  assert.deepEqual(sortRegionsByCount(dictionary,[]),dictionary)
})

import test from 'node:test'
import assert from 'node:assert/strict'
import { toAnnouncementRegionQuery, UNLOCATED_PROVINCE } from '../src/utils/regionFilter.js'
import { toAnnouncementRegionQuery as adminQuery } from '../../hailong-admin/src/utils/regionFilter.js'

for (const [name, convert] of [['门户', toAnnouncementRegionQuery], ['后台', adminQuery]]) {
  test(`${name}未明确省份筛选清除具体地区条件并保留业务和分页条件`, () => {
    const original = { province: UNLOCATED_PROVINCE, city: '410100', district: '410102', keyword: '项目', businessType: 'CONSTRUCTION', pageNumber: 2 }
    const query = convert(original)
    assert.equal(query.provinceUnlocated, true)
    assert.equal(query.province, undefined); assert.equal(query.city, undefined); assert.equal(query.district, undefined)
    assert.equal(query.pageNumber, 2); assert.equal(query.keyword, '项目'); assert.equal(query.businessType, 'CONSTRUCTION')
    assert.equal(original.province, UNLOCATED_PROVINCE)
    const known = { province: '410000', city: '410100' }
    assert.deepEqual(convert(known), known)
  })
}

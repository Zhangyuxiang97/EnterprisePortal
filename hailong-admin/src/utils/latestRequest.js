// 让最近一次查询拥有更新界面的权利，防止慢请求覆盖新筛选结果。
export function createLatestRequest() {
  let revision = 0
  return { begin: () => ++revision, isCurrent: value => value === revision, cancel: () => ++revision }
}

import { onBeforeUnmount } from 'vue'
import { createLatestRequest } from '@/utils/latestRequest'

// A page owns its requests; late responses cannot update a newer filter or a disposed chart.
export function useLatestRequest() {
  const request = createLatestRequest()
  onBeforeUnmount(request.cancel)
  return request
}

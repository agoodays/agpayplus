import { req } from '@/lib/ag-axios'

export const mainApi = {
  queryPayDayCount(params) {
    return req.get('/api/mainChart/payDayCount', params)
  },
  queryIsvAndMchCount() {
    return req.get('/api/mainChart/isvAndMchCount')
  }
}

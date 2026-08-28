import { req } from '@/lib/ag-axios'

const API_URL_MAIN_STATISTIC = '/api/mainChart'

export const dashboardApi = {
  queryPayDayCount(queryDateRange) {
    return req.get(`${API_URL_MAIN_STATISTIC}/payDayCount`, { queryDateRange })
  },
  queryPayTrendCount(days) {
    return req.get(`${API_URL_MAIN_STATISTIC}/payTrendCount`, { recentDay: days })
  },
  queryIsvAndMchCount() {
    return req.get(`${API_URL_MAIN_STATISTIC}/isvAndMchCount`)
  },
  queryPayCount(params) {
    return req.get(`${API_URL_MAIN_STATISTIC}/payCount`, params)
  },
  queryPayType(params) {
    return req.get(`${API_URL_MAIN_STATISTIC}/payTypeCount`, params)
  }
}

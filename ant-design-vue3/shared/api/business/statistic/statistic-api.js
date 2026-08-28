import { req } from '@/lib/ag-axios'

const API_URL_ORDER_STATISTIC = '/api/statistic'
const API_URL_MCH_LIST = '/api/mchInfo'

export const statisticApi = {
  queryOrderStatistic(params) {
    return req.list(API_URL_ORDER_STATISTIC, params)
  },
  queryOrderStatisticTotal(params) {
    return req.total(API_URL_ORDER_STATISTIC, params)
  },
  exportExcel(params) {
    return req.export(API_URL_ORDER_STATISTIC, 'excel', params)
  },
  listMch(params) {
    return req.list(API_URL_MCH_LIST, params)
  }
}

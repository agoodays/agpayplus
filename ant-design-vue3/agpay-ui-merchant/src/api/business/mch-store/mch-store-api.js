import { req } from '@/lib/ag-axios'

const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_MCH_STORE = '/api/mchStore'

export const mchStoreApi = {
  queryPage(params) {
    return req.list(API_URL_MCH_STORE, params)
  },
  getById(storeId) {
    return req.getById(API_URL_MCH_STORE, storeId)
  },
  add(data) {
    return req.add(API_URL_MCH_STORE, data)
  },
  updateById(storeId, data) {
    return req.updateById(API_URL_MCH_STORE, storeId, data)
  },
  delById(storeId) {
    return req.delById(API_URL_MCH_STORE, storeId)
  },
  queryMchPage(params) {
    return req.list(API_URL_MCH_LIST, params)
  },
  updateBindAppById(storeId, bindAppId) {
    return req.updateById(API_URL_MCH_STORE, storeId, { bindAppId })
  }
}

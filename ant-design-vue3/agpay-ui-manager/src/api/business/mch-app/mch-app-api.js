import { req } from '@/lib/ag-axios'

const API_URL_MCH_APP = '/api/mchApps'
const API_URL_MCH_LIST = '/api/mchInfo'

export const mchAppApi = {
  queryPage(params) {
    return req.list(API_URL_MCH_APP, params)
  },
  getById(appId) {
    return req.getById(API_URL_MCH_APP, appId)
  },
  add(data) {
    return req.add(API_URL_MCH_APP, data)
  },
  updateById(appId, data) {
    return req.updateById(API_URL_MCH_APP, appId, data)
  },
  delById(appId) {
    return req.delById(API_URL_MCH_APP, appId)
  },
  queryMchPage(params) {
    return req.list(API_URL_MCH_LIST, params)
  },
  queryByMchNo(mchNo) {
    return req.list(API_URL_MCH_APP, { pageSize: -1, mchNo })
  }
}

import { req } from '@/lib/ag-axios'

const API_URL_ISV_LIST = '/api/isvInfo'

export const isvApi = {
  queryPage(params) {
    return req.list(API_URL_ISV_LIST, params)
  },
  getById(recordId) {
    return req.getById(API_URL_ISV_LIST, recordId)
  },
  add(data) {
    return req.add(API_URL_ISV_LIST, data)
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_ISV_LIST, recordId, data)
  },
  delById(recordId) {
    return req.delById(API_URL_ISV_LIST, recordId)
  }
}

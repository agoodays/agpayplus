import { req } from '@/lib/ag-axios'

const API_URL_DIVISION_RECEIVER_GROUP = '/api/divisionReceiverGroups'
const API_URL_MCH_LIST = '/api/mchInfo'

export const divisionGroupApi = {
  queryPage(params) {
    return req.list(API_URL_DIVISION_RECEIVER_GROUP, params)
  },
  getById(recordId) {
    return req.getById(API_URL_DIVISION_RECEIVER_GROUP, recordId)
  },
  add(data) {
    return req.add(API_URL_DIVISION_RECEIVER_GROUP, data)
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_DIVISION_RECEIVER_GROUP, recordId, data)
  },
  delById(recordId) {
    return req.delById(API_URL_DIVISION_RECEIVER_GROUP, recordId)
  },
  listMch(params) {
    return req.list(API_URL_MCH_LIST, params)
  }
}

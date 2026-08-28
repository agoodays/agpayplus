import { basicApi } from '@/api/system/basic-api'
import { req } from '@/lib/ag-axios'

const API_URL_DIVISION_RECEIVER = '/api/divisionReceivers'
const API_URL_DIVISION_RECEIVER_GROUP = '/api/divisionReceiverGroups'
const API_URL_IFDEFINES_LIST = '/api/payIfDefines'
const API_URL_MCH_APP = '/api/mchApps'
const API_URL_MCH_LIST = '/api/mchInfo'

export const divisionReceiverApi = {
  queryPage(params) {
    return req.list(API_URL_DIVISION_RECEIVER, params)
  },
  getById(recordId) {
    return req.getById(API_URL_DIVISION_RECEIVER, recordId)
  },
  add(data) {
    return req.add(API_URL_DIVISION_RECEIVER, data)
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_DIVISION_RECEIVER, recordId, data)
  },
  listMch(params) {
    return req.list(API_URL_MCH_LIST, params)
  },
  listMchApp(params) {
    return req.list(API_URL_MCH_APP, params)
  },
  listReceiverGroup(params) {
    return req.list(API_URL_DIVISION_RECEIVER_GROUP, params)
  },
  listIfDefine(params) {
    return req.list(API_URL_IFDEFINES_LIST, params)
  },
  listIfCodeByAppId(appId) {
    return basicApi.getIfCodeByAppId(appId)
  }
}

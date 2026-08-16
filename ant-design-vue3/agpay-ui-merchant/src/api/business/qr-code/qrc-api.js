import { req, reqLoad } from '@/lib/ag-axios'

const API_URL_QRC_LIST = '/api/qrc'
const API_URL_AGENT_LIST = '/api/agentInfo'
const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_MCH_APP = '/api/mchApps'
const API_URL_MCH_STORE = '/api/mchStore'
const API_URL_QRC_SHELL_LIST = '/api/qrc/shell'

export const qrcApi = {
  queryPage(params) {
    return req.list(API_URL_QRC_LIST, params)
  },
  getById(recordId) {
    return req.getById(API_URL_QRC_LIST, recordId)
  },
  viewQrc(recordId) {
    return req.get(`${API_URL_QRC_LIST}/view/${recordId}`)
  },
  add(data) {
    return req.add(API_URL_QRC_LIST, data)
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_QRC_LIST, recordId, data)
  },
  delById(recordId) {
    return req.delById(API_URL_QRC_LIST, recordId)
  },
  bindById(recordId, data) {
    return req.updateById(`${API_URL_QRC_LIST}/bind`, recordId, data)
  },
  unbindById(recordId) {
    return reqLoad.updateById(`${API_URL_QRC_LIST}/unbind`, recordId, {})
  },
  updateStateById(recordId, state) {
    return reqLoad.updateById(API_URL_QRC_LIST, recordId, { state })
  },
  getBatchIdDistinctCount() {
    return req.get(`${API_URL_QRC_LIST}/batchIdDistinctCount`)
  },
  listShells(params = { pageSize: -1, state: 1 }) {
    return req.list(API_URL_QRC_SHELL_LIST, params)
  },
  searchAgent(params) {
    return req.list(API_URL_AGENT_LIST, params)
  },
  searchMch(params) {
    return req.list(API_URL_MCH_LIST, params)
  },
  listMchApps(params) {
    return req.list(API_URL_MCH_APP, params)
  },
  listMchStores(params) {
    return req.list(API_URL_MCH_STORE, params)
  }
}

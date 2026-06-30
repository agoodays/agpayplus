import { req, upload } from '@/lib/ag-axios'

const API_URL_IFDEFINES_LIST = '/api/payIfDefines'
const API_URL_PAYWAYS_LIST = '/api/payWays'

export const payConfigApi = {
  queryIfDefineList(params) {
    return req.list(API_URL_IFDEFINES_LIST, params)
  },
  getIfDefineById(ifCode) {
    return req.getById(API_URL_IFDEFINES_LIST, ifCode)
  },
  addIfDefine(data) {
    return req.add(API_URL_IFDEFINES_LIST, data)
  },
  updateIfDefineById(ifCode, data) {
    return req.updateById(API_URL_IFDEFINES_LIST, ifCode, data)
  },
  delIfDefineById(ifCode) {
    return req.delById(API_URL_IFDEFINES_LIST, ifCode)
  },
  queryPayWayList(params) {
    return req.list(API_URL_PAYWAYS_LIST, params)
  },
  getPayWayById(wayCode) {
    return req.getById(API_URL_PAYWAYS_LIST, wayCode)
  },
  addPayWay(data) {
    return req.add(API_URL_PAYWAYS_LIST, data)
  },
  updatePayWayById(wayCode, data) {
    return req.updateById(API_URL_PAYWAYS_LIST, wayCode, data)
  },
  delPayWayById(wayCode) {
    return req.delById(API_URL_PAYWAYS_LIST, wayCode)
  },
  getIfBgUploadAction() {
    return upload.ifBG
  }
}

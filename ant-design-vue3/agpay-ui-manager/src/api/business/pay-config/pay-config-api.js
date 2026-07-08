import { req, upload, request } from '@/lib/ag-axios'

const API_URL_IFDEFINES_LIST = '/api/payIfDefines'
const API_URL_PAYWAYS_LIST = '/api/payWays'
const API_URL_PAYCONFIGS_LIST = '/api/payConfig'
const API_URL_RATECONFIGS_LIST = '/api/rateConfig'
const API_URL_ISV_PAYCONFIGS_LIST = '/api/isv/payConfigs'
const API_URL_AGENT_PAYCONFIGS_LIST = '/api/agent/payConfigs'
const API_URL_MCH_PAYCONFIGS_LIST = '/api/mch/payConfigs'
const API_URL_MCH_PAYPASSAGE_LIST = '/api/mch/payPassages'

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
  },
  queryPayConfigIfCodes(params) {
    return req.list(`${API_URL_PAYCONFIGS_LIST}/ifCodes`, params)
  },
  getPayConfigById(infoId, ifCode) {
    return req.get(`${API_URL_PAYCONFIGS_LIST}`, { infoId, infoType: '', ifCode })
  },
  addPayConfig(data) {
    return req.add(API_URL_PAYCONFIGS_LIST, data)
  },
  queryMchPayPassagePage(params) {
    return req.list(API_URL_MCH_PAYPASSAGE_LIST, params)
  },
  getAvailablePayInterfaceList(mchNo, wayCode, params) {
    return req.get(`/api/mch/payPassages/availablePayInterface/${mchNo}/${wayCode}`, params)
  },
  updateMchPassageState(appId, wayCode, ifCode, state) {
    const params = { appId, wayCode, ifCode, state }
    const queryString = Object.keys(params)
      .map((key) => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
      .join('&')
    return req.add(`${API_URL_MCH_PAYPASSAGE_LIST}/mchPassage?${queryString}`)
  },
  getIsvPayConfigUnique(infoId, ifCode) {
    return req.get(`${API_URL_ISV_PAYCONFIGS_LIST}/${infoId}/${ifCode}`)
  },
  getMchPayConfigUnique(infoId, ifCode) {
    return req.get(`${API_URL_MCH_PAYCONFIGS_LIST}/${infoId}/${ifCode}`)
  },
  queryRateConfigList(path, params) {
    if (typeof path === 'object') {
      params = path
      path = ''
    }
    return req.list(API_URL_RATECONFIGS_LIST + path, params)
  },
  getRateConfigById(rateId) {
    return req.getById(API_URL_RATECONFIGS_LIST, rateId)
  },
  addRateConfig(data) {
    return req.add(API_URL_RATECONFIGS_LIST, data)
  },
  updateRateConfigById(rateId, data) {
    return req.updateById(API_URL_RATECONFIGS_LIST, rateId, data)
  },
  delRateConfigById(rateId) {
    return req.delById(API_URL_RATECONFIGS_LIST, rateId)
  },
  queryOauth2DiyList(params) {
    return req.get(`/api/payOauth2Config/diyList`, params)
  },
  queryAlipayIsvsubMchAuthUrl(mchAppId) {
    return req.get(`/api/mch/payConfigs/alipayIsvsubMchAuthUrls/${mchAppId}`)
  },
  getIfCodeByAppId(appId) {
    return request({
      url: `/api/mch/payConfigs/ifCodes/${appId}`,
      method: 'GET'
    }, true, true, true)
  }
}

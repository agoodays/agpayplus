import { req } from '@/lib/ag-axios'

const API_URL_MCH_APP = '/api/mchApps'
const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_MCH_PAYCONFIGS_LIST = '/api/mch/payConfigs'
const API_URL_MCH_PAYPASSAGE_LIST = '/api/mch/payPassages'

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
  },
  queryCardList(appId) {
    return req.list(API_URL_MCH_PAYCONFIGS_LIST, { pageSize: -1, appId })
  },
  getMchPayConfigUnique(infoId, ifCode) {
    return req.get(`${API_URL_MCH_PAYCONFIGS_LIST}/${infoId}/${ifCode}`)
  },
  addMchPayConfig(data) {
    return req.add(API_URL_MCH_PAYCONFIGS_LIST, data)
  },
  updateMchPayConfig(infoId, data) {
    return req.updateById(API_URL_MCH_PAYCONFIGS_LIST, infoId, data)
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
  queryMchPayPassagePage(params) {
    return req.list(API_URL_MCH_PAYPASSAGE_LIST, params)
  },
  saveMchPayPassages(data) {
    return req.add(API_URL_MCH_PAYPASSAGE_LIST, data)
  },
  queryAlipayIsvsubMchAuthUrl(mchAppId) {
    return req.get(`/api/mch/payConfigs/alipayIsvsubMchAuthUrls/${mchAppId}`)
  }
}

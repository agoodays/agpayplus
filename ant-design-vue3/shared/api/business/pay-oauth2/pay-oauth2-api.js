import { req } from '@/lib/ag-axios'

const API_URL_PAYOAUTH2CONFIGS = '/api/payOauth2Config'

export const payOauth2Api = {
  queryList(params) {
    return req.list(API_URL_PAYOAUTH2CONFIGS, params)
  },
  getById(oauth2Id) {
    return req.getById(API_URL_PAYOAUTH2CONFIGS, oauth2Id)
  },
  add(data) {
    return req.add(API_URL_PAYOAUTH2CONFIGS, data)
  },
  updateById(oauth2Id, data) {
    return req.updateById(API_URL_PAYOAUTH2CONFIGS, oauth2Id, data)
  },
  delById(oauth2Id) {
    return req.delById(API_URL_PAYOAUTH2CONFIGS, oauth2Id)
  },
  queryDiyList(params) {
    return req.get(`${API_URL_PAYOAUTH2CONFIGS}/diyList`, params)
  },
  createDiyList(params) {
    return req.post(`${API_URL_PAYOAUTH2CONFIGS}/diyList`, params)
  },
  querySavedConfigs(params) {
    return req.get(`${API_URL_PAYOAUTH2CONFIGS}/savedConfigs`, params)
  },
  saveConfigParams(data) {
    return req.add(`${API_URL_PAYOAUTH2CONFIGS}/configParams`, data)
  }
}

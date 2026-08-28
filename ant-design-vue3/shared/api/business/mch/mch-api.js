import { req } from '@/lib/ag-axios'

const API_URL_AGENT_LIST = '/api/agentInfo'
const API_URL_ISV_LIST = '/api/isvInfo'
const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_MCH_CONFIG = '/api/mchConfig'

export const mchApi = {
  queryPage(params) {
    return req.list(API_URL_MCH_LIST, params)
  },
  getById(mchNo) {
    return req.getById(API_URL_MCH_LIST, mchNo)
  },
  add(data) {
    return req.add(API_URL_MCH_LIST, data)
  },
  updateById(mchNo, data) {
    return req.updateById(API_URL_MCH_LIST, mchNo, data)
  },
  delById(mchNo) {
    return req.delById(API_URL_MCH_LIST, mchNo)
  },
  queryAgentPage(params) {
    return req.list(API_URL_AGENT_LIST, params)
  },
  queryIsvPage(params) {
    return req.list(API_URL_ISV_LIST, params)
  },
  getMchConfigs(groupKey, mchNo) {
    return req.get(`${API_URL_MCH_CONFIG}/${groupKey}`, { mchNo })
  },
  updateMchConfigs(groupKey, data) {
    return req.updateById(API_URL_MCH_CONFIG, groupKey, data)
  }
}

import { basicApi } from '@/api/system/basic-api'
import { req, reqLoad, request } from '@/lib/ag-axios'

const API_URL_ENT_LIST = '/api/sysEnts'

export const entApi = {
  queryEntTree(sysType) {
    return basicApi.getEntTree(sysType)
  },
  getBySysType(entId, sysType) {
    return request({
      url: '/api/sysEnts/bySysType',
      method: 'GET',
      params: { entId, sysType }
    })
  },
  updateStateById(recordId, state, sysType) {
    return reqLoad.updateById(API_URL_ENT_LIST, recordId, { state, sysType })
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_ENT_LIST, recordId, data)
  },
  setMatchRule(data) {
    return req.updateById(API_URL_ENT_LIST, 'setMatchRule', data)
  }
}

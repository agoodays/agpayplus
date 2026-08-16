import { req } from '@/lib/ag-axios'

const API_URL_MCH_CONFIG = '/api/mchConfig'

export const mchConfigApi = {
  getConfig(groupKey) {
    return req.get(`${API_URL_MCH_CONFIG}/${groupKey}`)
  },
  updateConfig(groupKey, data) {
    return req.updateById(API_URL_MCH_CONFIG, groupKey, data)
  }
}

export default mchConfigApi

import { req } from '@/lib/ag-axios'

const API_URL_AGENT_CONFIG = '/api/agentConfig'

export const agentConfigApi = {
  getConfig(groupKey) {
    return req.get(`${API_URL_AGENT_CONFIG}/${groupKey}`)
  },
  updateConfig(groupKey, data) {
    return req.updateById(API_URL_AGENT_CONFIG, groupKey, data)
  }
}

export default agentConfigApi

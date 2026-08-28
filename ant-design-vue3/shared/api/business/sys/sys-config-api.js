import { req } from '@/lib/ag-axios'

const API_URL_SYS_CONFIG = '/api/sysConfigs'

export const sysConfigApi = {
  queryGroupConfigs(groupKey) {
    return req.getById(API_URL_SYS_CONFIG, groupKey)
  },
  updateGroupConfigs(groupKey, data) {
    return req.updateById(API_URL_SYS_CONFIG, groupKey, data)
  }
}

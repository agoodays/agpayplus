import { req } from '@/lib/ag-axios'

const API_URL_SYS_LOG = '/api/sysLog'

export const sysApi = {
  querySysLogPage(params) {
    return req.list(API_URL_SYS_LOG, params)
  },
  getSysLogById(sysLogId) {
    return req.getById(API_URL_SYS_LOG, sysLogId)
  },
  delSysLogById(sysLogIds) {
    return req.delById(API_URL_SYS_LOG, sysLogIds)
  }
}

import { basicApi } from '@/api/system/basic-api'
import { req } from '@/lib/ag-axios'

const API_URL_ROLE_LIST = '/api/sysRoles'
const API_URL_ROLE_ENT_RELA_LIST = '/api/sysRoleEntRelas'

export const roleApi = {
  queryPage(params) {
    return req.list(API_URL_ROLE_LIST, params)
  },
  getById(recordId) {
    return req.getById(API_URL_ROLE_LIST, recordId)
  },
  add(data) {
    return req.add(API_URL_ROLE_LIST, data)
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_ROLE_LIST, recordId, data)
  },
  delById(recordId) {
    return req.delById(API_URL_ROLE_LIST, recordId)
  },
  queryEntTree(sysType) {
    return basicApi.getEntTree(sysType)
  },
  queryRoleEntRelaList(roleId) {
    return req.list(API_URL_ROLE_ENT_RELA_LIST, { roleId: roleId || 'NONE', pageSize: -1 })
  }
}

import { basicApi } from '@/api/system/basic-api'
import { req, reqLoad, request } from '@/lib/ag-axios'

const API_URL_ROLE_LIST = '/api/sysRoles'
const API_URL_SYS_USER_LIST = '/api/sysUsers'
const API_URL_UR_TEAM_LIST = '/api/userTeams'
const API_URL_USER_ROLE_RELA_LIST = '/api/sysUserRoleRelas'

export const sysUserApi = {
  queryPage(params) {
    return req.list(API_URL_SYS_USER_LIST, params)
  },
  add(data) {
    return req.add(API_URL_SYS_USER_LIST, data)
  },
  updateById(id, data) {
    return req.updateById(API_URL_SYS_USER_LIST, id, data)
  },
  updateStateById(id, data) {
    return reqLoad.updateById(API_URL_SYS_USER_LIST, id, data)
  },
  getById(id) {
    return req.getById(API_URL_SYS_USER_LIST, id)
  },
  delById(id) {
    return req.delById(API_URL_SYS_USER_LIST, id)
  },
  relieveLoginLimit(id) {
    return req.delById(`${API_URL_SYS_USER_LIST}/loginLimit`, id)
  },
  queryTeamPage(params) {
    return req.list(API_URL_UR_TEAM_LIST, params)
  },
  queryRolePageWithLoading(params) {
    return reqLoad.list(API_URL_ROLE_LIST, params)
  },
  queryUserRoleRelaPage(params) {
    return req.list(API_URL_USER_ROLE_RELA_LIST, params)
  },
  updateUserRoleRela(userId, roleIds) {
    return request({
      url: `api/sysUserRoleRelas/relas/${userId}`,
      method: 'POST',
      data: { roleIds }
    })
  },
  queryPwdRulesRegexp() {
    return basicApi.getPwdRulesRegexp()
  }
}

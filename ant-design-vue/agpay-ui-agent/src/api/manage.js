import request from '@/http/request'

/*
 *  全系列 restful api格式, 定义通用req对象
 *
 *  @author terrfly
 *  @site https://www.agpay.vip
 *  @date 2021/5/8 07:18
 */
export const req = {

  // 通用列表查询接口
  list: (url, params) => {
    return request.request({ url: url, method: 'GET', params: params }, true, true, false)
  },

  // 通用获取数据接口
  get: (url, params) => {
    return request.request({ url: url, method: 'GET', params: params }, true, true, false)
  },

  // 通用列表查询统计接口
  count: (url, params) => {
    return request.request({ url: url + '/count', method: 'GET', params: params }, true, true, false)
  },

  // 通用列表数据导出接口
  export: (url, bizType, params) => {
    return request.request({ url: url + '/export/' + bizType, method: 'GET', params: params, responseType: 'blob' }, true, true, false)
  },

  // 通用新增接口
  add: (url, data) => {
    return request.request({ url: url, method: 'POST', data: data }, true, true, false)
  },

  // 通用查询单条数据接口
  getById: (url, bizId) => {
    return request.request({ url: url + '/' + bizId, method: 'GET' }, true, true, false)
  },

  // 通用修改接口
  updateById: (url, bizId, data) => {
    return request.request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, false)
  },

  // 通用删除接口
  delById: (url, bizId) => {
    return request.request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, false)
  }
}

// 全系列 restful api格式 (全局loading方式)
export const reqLoad = {

  // 通用列表查询接口
  list: (url, params) => {
    return request.request({ url: url, method: 'GET', params: params }, true, true, true)
  },

  // 通用新增接口
  add: (url, data) => {
    return request.request({ url: url, method: 'POST', data: data }, true, true, true)
  },

  // 通用查询单条数据接口
  getById: (url, bizId) => {
    return request.request({ url: url + '/' + bizId, method: 'GET' }, true, true, true)
  },

  // 通用修改接口
  updateById: (url, bizId, data) => {
    return request.request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, true)
  },

  // 通用删除接口
  delById: (url, bizId) => {
    return request.request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, true)
  }
}

/** 角色管理页面 **/
export const API_URL_ROLE_LIST = '/api/sysRoles'
export const API_URL_ROLE_ENT_RELA_LIST = '/api/sysRoleEntRelas'
export const API_URL_SYS_USER_LIST = '/api/sysUsers'
export const API_URL_UR_TEAM_LIST = '/api/userTeams'
export const API_URL_USER_ROLE_RELA_LIST = '/api/sysUserRoleRelas'

/** 代理商、商户管理 **/
export const API_URL_AGENT_LIST = '/api/agentInfo'
export const API_URL_MCH_LIST = '/api/mchInfo'
/** 商户App管理 **/
export const API_URL_MCH_APP = '/api/mchApps'
/** 商户门店管理 **/
export const API_URL_MCH_STORE = '/api/mchStore'
/** 支付订单管理 **/
export const API_URL_PAY_ORDER_LIST = '/api/payOrder'
/** 退款订单管理 **/
export const API_URL_REFUND_ORDER_LIST = '/api/refundOrder'
/** 商户通知管理 **/
export const API_URL_MCH_NOTIFY_LIST = '/api/mchNotify'
/** 系统配置 **/
export const API_URL_AGENT_CONFIG = 'api/agentConfig'
/** 公告管理 **/
export const API_URL_ARTICLE_LIST = 'api/sysArticles'
/** 首页统计 **/
export const API_URL_MAIN_STATISTIC = 'api/mainChart'

/** 支付接口定义页面 **/
export const API_URL_IFDEFINES_LIST = '/api/payIfDefines'
export const API_URL_PAYWAYS_LIST = '/api/payWays'
/** 代理商、商户支付参数配置 **/
export const API_URL_AGENT_PAYCONFIGS_LIST = '/api/agent/payConfigs'
export const API_URL_MCH_PAYCONFIGS_LIST = '/api/mch/payConfigs'
/** 服务商、代理商、商户支付参数配置 **/
export const API_URL_PAYCONFIGS_LIST = '/api/payConfig'
export const API_URL_RATECONFIGS_LIST = '/api/rateConfig'
/** 商户支付通道配置 **/
export const API_URL_MCH_PAYPASSAGE_LIST = '/api/mch/payPassages'
/** 转账订单管理 **/
export const API_URL_TRANSFER_ORDER_LIST = '/api/transferOrders'

/** 上传图片/文件地址 **/
export const upload = {
  avatar: '/api/ossFiles/avatar',
  ifBG: '/api/ossFiles/ifBG',
  cert: '/api/ossFiles/cert',
  form: '/api/ossFiles/form',
  /** 获取上传表单参数 */
  getFormParams: (url, fileName, fileSize) => {
    return request.request({ url: url, method: 'GET', params: { fileName, fileSize } })
  },
  /** 上传单个文件 */
  singleFile: (url, isLocalFile, data) => {
    const formData = new FormData()
    for (const key in data) {
      if (data.hasOwnProperty(key)) {
        formData.append(key, data[key])
      }
    }
    const actionUrl = isLocalFile ? { url: url } : { baseURL: url }
    const options = Object.assign(actionUrl, { method: 'POST', data: formData })
    return request.request(options)
  }
}

const api = {
  user: '/user',
  role_list: '/role',
  service: '/service',
  permission: '/permission',
  permissionNoPager: '/permission/no-pager',
  orgTree: '/org/tree'
}

export default api

/** 获取权限树状结构图 **/
export function getEntTree () {
  return request.request({ url: '/api/sysEnts/showTree', method: 'GET' })
}

/** 退款接口 */
export function payOrderRefund (payOrderId, refundAmount, refundReason, refundPassword) {
  return request.request({
    url: '/api/payOrder/refunds/' + payOrderId,
    method: 'POST',
    data: { refundAmount, refundReason, refundPassword }
  })
}

/** 更新用户角色信息 */
export function uSysUserRoleRela (sysUserId, roleIdList) {
  return request.request({
    url: 'api/sysUserRoleRelas/relas/' + sysUserId,
    method: 'POST',
    data: { roleIdListStr: JSON.stringify(roleIdList) }
  })
}

export function getRoleList (parameter) {
  return request({
    url: '/api/sysRoles',
    method: 'get',
    params: parameter
  })
}

export function getAgentConfigs (parameter, data) {
  return request.request({
    url: API_URL_AGENT_CONFIG + '/' + parameter,
    method: 'GET',
    params: data
  })
}

export function getServiceList (parameter) {
  return request({
    url: api.service,
    method: 'get',
    params: parameter
  })
}

export function getPermissions (parameter) {
  return request({
    url: api.permissionNoPager,
    method: 'get',
    params: parameter
  })
}

export function getOrgTree (parameter) {
  return request({
    url: api.orgTree,
    method: 'get',
    params: parameter
  })
}

// id == 0 add     post
// id != 0 update  put
export function saveService (parameter) {
  return request({
    url: api.service,
    method: parameter.id === 0 ? 'post' : 'put',
    data: parameter
  })
}

export function saveSub (sub) {
  return request({
    url: '/sub',
    method: sub.id === 0 ? 'post' : 'put',
    data: sub
  })
}

export function getMchPayConfigUnique (infoId, ifCode) {
  return request.request({
    url: '/api/mch/payConfigs/' + infoId + '/' + ifCode,
    method: 'get'
  })
}

export function getAvailablePayInterfaceList (mchNo, wayCode) {
  return request.request({
    url: '/api/mch/payPassages/availablePayInterface/' + mchNo + '/' + wayCode,
    method: 'GET'
  })
}

export function getPayTrendCount (parameter) {
  return request.request({
    url: API_URL_MAIN_STATISTIC + '/payTrendCount?recentDay=' + parameter,
    method: 'GET'
  })
}

export function getIsvAndMchCount () {
  return request.request({
    url: API_URL_MAIN_STATISTIC + '/isvAndMchCount',
    method: 'GET'
  })
}

export function getPayDayCount (parameter) {
  return request.request({
    url: API_URL_MAIN_STATISTIC + '/payDayCount?queryDateRange=' + parameter,
    method: 'GET'
  })
}

export function getPayCount (parameter) {
  return request.request({
    url: API_URL_MAIN_STATISTIC + '/payCount',
    method: 'GET',
    params: parameter
  })
}

export function getPayType (parameter) {
  return request.request({
    url: API_URL_MAIN_STATISTIC + '/payTypeCount',
    method: 'GET',
    params: parameter
  })
}

export function getMainUserInfo () {
  return request.request({
    url: API_URL_MAIN_STATISTIC,
    method: 'GET'
  })
}

export function updateUserPass (parameter) {
  return request.request({
    url: '/api/current/modifyPwd',
    method: 'put',
    data: parameter
  })
}

export function updateUserInfo (parameter) {
  return request.request({
    url: '/api/current/user',
    method: 'put',
    data: parameter
  })
}

export function getUserInfo () {
  return request.request({
    url: '/api/current/user',
    method: 'get'
  })
}

export function mchNotifyResend (notifyId) {
  return request.request({
    url: '/api/mchNotify/resend/' + notifyId,
    method: 'POST'
  })
}

/** 查询支付宝授权地址URL **/
export function queryAlipayIsvsubMchAuthUrl (mchAppId) {
  return request.request({
    url: '/api/mch/payConfigs/alipayIsvsubMchAuthUrls/' + mchAppId,
    method: 'GET'
  })
}

/** 获取支付网关系统公钥 **/
export function getSysRSA2PublicKey () {
  return request.request({ url: '/api/mchApps/sysRSA2PublicKey', method: 'GET' })
}

/** 获取地图配置 **/
export function getMapConfig () {
  return request.request({ url: '/api/mchStore/mapConfig', method: 'GET' })
}

/** 获取密码规则 **/
export function getPwdRulesRegexp () {
  return request.request({ url: '/api/anon/cipher/pwdRulesRegexp', method: 'GET' })
}

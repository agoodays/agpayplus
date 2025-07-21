/*
 *  全系列 restful api格式, 定义通用req对象
 *
 *  @author terrfly
 *  @site https://www.agpay.vip
 *  @date 2021/5/8 07:18
 */

import request from '@/http/request'

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
export const API_URL_ENT_LIST = '/api/sysEnts'
export const API_URL_ROLE_LIST = '/api/sysRoles'
export const API_URL_ROLE_ENT_RELA_LIST = '/api/sysRoleEntRelas'
export const API_URL_SYS_USER_LIST = '/api/sysUsers'
export const API_URL_USER_ROLE_RELA_LIST = '/api/sysUserRoleRelas'
/** 首页统计 **/
export const API_URL_MAIN_STATISTIC = 'api/mainChart'

/** 商户App管理 **/
export const API_URL_MCH_APP = '/api/mchApps'
/** 商户门店管理 **/
export const API_URL_MCH_STORE = '/api/mchStore'
/** 支付订单管理 **/
export const API_URL_PAY_ORDER_LIST = '/api/payOrder'
/** 退款订单管理 **/
export const API_URL_REFUND_ORDER_LIST = '/api/refundOrder'
/** 支付方式列表 **/
export const API_URL_PAYWAYS_LIST = '/api/payWays'
/** 商户支付参数配置 **/
export const API_URL_MCH_PAYCONFIGS_LIST = '/api/mch/payConfigs'
export const API_URL_PAYOAUTH2CONFIGS = '/api/payOauth2Config'
/** 服务商、代理商、商户支付参数配置 **/
export const API_URL_PAYCONFIGS_LIST = '/api/payConfig'
export const API_URL_RATECONFIGS_LIST = '/api/rateConfig'
/** 商户支付通道配置 **/
export const API_URL_MCH_PAYPASSAGE_LIST = '/api/mch/payPassages'
/** 转账订单管理 **/
export const API_URL_TRANSFER_ORDER_LIST = '/api/transferOrders'

/** 公告管理 **/
export const API_URL_ARTICLE_LIST = 'api/sysArticles'

/** 分账组管理 **/
export const API_URL_DIVISION_RECEIVER_GROUP = '/api/divisionReceiverGroups'

/** 分账账号管理 **/
export const API_URL_DIVISION_RECEIVER = '/api/divisionReceivers'

/** 分账记录管理 **/
export const API_URL_PAY_ORDER_DIVISION_RECORD_LIST = '/api/division/records'

export const API_URL_IFDEFINES_LIST = '/api/payIfDefines'

/** 商户配置 **/
export const API_URL_MCH_CONFIG = 'api/mchConfig'

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

/** 更新用户角色信息 */
export function uSysUserRoleRela (sysUserId, roleIdList) {
  return request.request({
    url: 'api/sysUserRoleRelas/relas/' + sysUserId,
    method: 'POST',
    data: { roleIds: roleIdList }
  })
}

export function getRoleList (parameter) {
  return request({
    url: '/api/sysRoles',
    method: 'get',
    params: parameter
  })
}

export function getMchConfigs (parameter, data) {
  return request.request({
    url: API_URL_MCH_CONFIG + '/' + parameter,
    method: 'GET',
    params: data
  })
}

/** 根据支付接口查询支付参数配置 **/
export function getMchPayConfigUnique (infoId, ifCode) {
  return request.request({
    url: '/api/mch/payConfigs/' + infoId + '/' + ifCode,
    method: 'get'
  })
}

/** 支付体验配置 **/
export function payTest (appId) {
  return request.request({
    url: 'api/paytest/payways/' + appId,
    method: 'GET'
  })
}

/** 支付体验下单配置 **/
export function payTestOrder (parameter) {
  return request.request({
    url: '/api/paytest/payOrders',
    method: 'POST',
    data: parameter
  })
}

/** 根据支付方式查询可用支付接口 **/
export function getAvailablePayInterfaceList (appId, wayCode) {
  return request.request({
    url: '/api/mch/payPassages/availablePayInterface/' + appId + '/' + wayCode,
    method: 'GET'
  })
}

export function getPayTrendCount (parameter) {
  return request.request({
    url: API_URL_MAIN_STATISTIC + '/payTrendCount?recentDay=' + parameter,
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

/** 获取到webSocket的前缀 （ws://localhost） **/
export function getWebSocketPrefix () {
  // 获取网站域名 +  端口号
  let domain = document.location.protocol + '//' + document.location.host

  // 判断api_base_url 是否设置
  if (process.env.VUE_APP_API_BASE_URL && process.env.VUE_APP_API_BASE_URL !== '/') {
    domain = process.env.VUE_APP_API_BASE_URL
  }

  if (domain.startsWith('https:')) {
    return 'wss://' + domain.replace('https://', '')
  } else {
    return 'ws://' + domain.replace('http://', '')
  }
}

/** 查询支付宝授权地址URL **/
export function queryAlipayIsvsubMchAuthUrl (mchAppId) {
  return request.request({
    url: '/api/mch/payConfigs/alipayIsvsubMchAuthUrls/' + mchAppId,
    method: 'GET'
  })
}

/** 查询商户转账支出的接口 **/
export function queryMchTransferIfCode (appId) {
  return request.request({
    url: 'api/mchTransfers/ifCodes/' + appId,
    method: 'GET'
  })
}

/** 获取渠道用户ID二维码地址 **/
export function getChannelUserQrImgUrl (ifCode, appId, extParam) {
  return request.request({
    url: '/api/mchTransfers/channelUserId',
    method: 'GET',
    params: { ifCode, appId, extParam }
  })
}

/** 转账 **/
export function doTransfer (parameter) {
  return request.request({
    url: '/api/mchTransfers/doTransfer',
    method: 'POST',
    data: parameter
  }, true, true, true)
}

/** 查询当前应用支持的支付接口 **/
export function getIfCodeByAppId (appId) {
  return request.request({
    url: '/api/mch/payConfigs/ifCodes/' + appId,
    method: 'GET'
  }, true, true, true)
}

/** 退款接口 */
export function payOrderRefund (payOrderId, refundAmount, refundReason, refundPassword) {
  return request.request({
    url: '/api/payOrder/refunds/' + payOrderId,
    method: 'POST',
    data: { refundAmount, refundReason, refundPassword }
  })
}

/** 分账重试 */
export function resendDivision (recordId) {
  return request.request({
    url: '/api/division/records/resend/' + recordId,
    method: 'POST'
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

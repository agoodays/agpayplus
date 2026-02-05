import request from '/@/lib/request'

/**
 * 全系列 restful api格式, 定义通用req对象
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
  total: (url, params) => {
    return request.request({ url: url + '/total', method: 'GET', params: params }, true, true, false)
  },

  // 通用列表查询统计接口
  count: (url, params) => {
    return request.request({ url: url + '/count', method: 'GET', params: params }, true, true, false)
  },

  // 通用列表数据导出接口
  export: (url, bizType, params) => {
    return request.request(
      { url: url + '/export/' + bizType, method: 'GET', params: params, responseType: 'blob' },
      true,
      true,
      false
    )
  },

  // 通用Post接口
  post: (url, data) => {
    return request.request({ url: url, method: 'POST', data: data }, true, true, false)
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
export const API_URL_UR_TEAM_LIST = '/api/userTeams'
export const API_URL_USER_ROLE_RELA_LIST = '/api/sysUserRoleRelas'

/** 服务商、代理商、商户管理 **/
export const API_URL_ISV_LIST = '/api/isvInfo'
export const API_URL_AGENT_LIST = '/api/agentInfo'
export const API_URL_ACCOUNT_BILL_LIST = '/api/accountBill'
export const API_URL_MCH_LIST = '/api/mchInfo'
/** 商户App管理 **/
export const API_URL_MCH_APP = '/api/mchApps'
/** 商户门店管理 **/
export const API_URL_MCH_STORE = '/api/mchStore'
/** 支付订单管理 **/
export const API_URL_PAY_ORDER = '/api/payOrder'
export const API_URL_PAY_ORDER_LIST = '/api/payOrder' // 兼容旧代码
/** 退款订单管理 **/
export const API_URL_REFUND_ORDER = '/api/refundOrder'
export const API_URL_REFUND_ORDER_LIST = '/api/refundOrder' // 兼容旧代码
/** 商户通知管理 **/
export const API_URL_MCH_NOTIFY_LIST = '/api/mchNotify'

/** 数据统计 **/
export const API_URL_ORDER_STATISTIC = '/api/statistic'

/** 分账组管理 **/
export const API_URL_DIVISION_RECEIVER_GROUP = '/api/divisionReceiverGroups'
/** 分账账号管理 **/
export const API_URL_DIVISION_RECEIVER = '/api/divisionReceivers'
/** 分账记录管理 **/
export const API_URL_PAY_ORDER_DIVISION_RECORD_LIST = '/api/division/records'

/** 系统日志 **/
export const API_URL_SYS_LOG = 'api/sysLog'
/** 系统配置 **/
export const API_URL_SYS_CONFIG = 'api/sysConfigs'
/** 公告管理 **/
export const API_URL_ARTICLE_LIST = 'api/sysArticles'
/** 首页统计 **/
export const API_URL_MAIN_STATISTIC = 'api/mainChart'

/** 支付接口定义页面 **/
export const API_URL_IFDEFINES_LIST = '/api/payIfDefines'
export const API_URL_PAYWAYS_LIST = '/api/payWays'
/** 服务商、代理商、商户支付参数配置 **/
export const API_URL_PAYCONFIGS_LIST = '/api/payConfig'
export const API_URL_PAYOAUTH2CONFIGS = '/api/payOauth2Config'
export const API_URL_RATECONFIGS_LIST = '/api/rateConfig'
export const API_URL_ISV_PAYCONFIGS_LIST = '/api/isv/payConfigs'
export const API_URL_AGENT_PAYCONFIGS_LIST = '/api/agent/payConfigs'
export const API_URL_MCH_PAYCONFIGS_LIST = '/api/mch/payConfigs'
/** 商户支付通道配置 **/
export const API_URL_MCH_PAYPASSAGE_LIST = '/api/mch/payPassages'
/** 商户高级配置 **/
export const API_URL_MCH_CONFIG = 'api/mchConfig'
/** 转账订单管理 **/
export const API_URL_TRANSFER_ORDER_LIST = '/api/transferOrders'

/** 码牌模板管理 **/
export const API_URL_QRC_SHELL_LIST = '/api/qrc/shell'
/** 码牌管理 **/
export const API_URL_QRC_LIST = '/api/qrc'

/** 上传图片/文件地址 **/
export const upload = {
  avatar: '/api/ossFiles/avatar',
  ifBG: '/api/ossFiles/ifBG',
  cert: '/api/ossFiles/cert',
  form: '/api/ossFiles/form',
  /** 获取上传表单参数 */
  getFormParams: (url, fileName, fileSize) => {
    return request.request({ url, method: 'GET', params: { fileName, fileSize } })
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

/** 获取权限树状结构图 **/
export function getEntTree(sysType) {
  return request.request({ url: '/api/sysEnts/showTree?sysType=' + sysType, method: 'GET' })
}

/** 查询当前应用支持的支付接口 **/
export function getIfCodeByAppId(appId) {
  return request.request({
    url: '/api/mch/payConfigs/ifCodes/' + appId,
    method: 'GET'
  })
}

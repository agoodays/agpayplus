/*
 *  基础API
 *
 */
import { request, getRequest } from '@/lib/ag-axios'

export const basicApi = {
  /**
   * 获取到webSocket的前缀 （ws://localhost）
   */
  getWebSocketPrefix: () => {
    let domain = document.location.protocol + '//' + document.location.host

    if (import.meta.env.VITE_APP_API_BASE_URL && import.meta.env.VITE_APP_API_BASE_URL !== '/') {
      domain = import.meta.env.VITE_APP_API_BASE_URL
    }

    if (domain.startsWith('https:')) {
      return 'wss://' + domain.replace('https://', '')
    } else {
      return 'ws://' + domain.replace('http://', '')
    }
  },
  /**
   * 获取渠道用户ID二维码地址
   */
  getChannelUserQrImgUrl: (ifCode, appId, extParam) => {
    return request({
      url: '/api/mchChannel/channelUserId',
      method: 'GET',
      params: { ifCode, appId, extParam }
    })
  },
  /**
   * 获取权限树状结构图
   */
  getEntTree: (sysType) => {
    return getRequest(`/api/sysEnts/showTree?sysType=${sysType}`)
  },
  /**
   * 获取权限树状结构图
   */
  getIfCodeByAppId: (appId) => {
    return request(
      {
        url: `/api/mch/payConfigs/ifCodes/${appId}`,
        method: 'GET'
      },
      true,
      true,
      true
    )
  },
  /**
   * 获取站点信息
   */
  getSiteInfos: () => {
    return getRequest('/api/anon/siteInfos')
  },
  /**
   * 获取密码规则
   */
  getPwdRulesRegexp: () => {
    return getRequest('/api/anon/cipher/pwdRulesRegexp')
  },
  /**
   * 获取地图配置
   */
  getMapConfig: () => {
    return getRequest('/api/mchStore/mapConfig')
  },
  /**
   * 获取支付网关系统公钥
   */
  getSysRSA2PublicKey: () => {
    return getRequest('/api/mchApps/sysRSA2PublicKey')
  }
}

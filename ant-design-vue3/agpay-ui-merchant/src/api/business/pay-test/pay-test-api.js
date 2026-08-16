import { req } from '@/lib/ag-axios'

/**
 * 支付测试 API
 * 对应 Vue2 manage.js 中的 payTest / payTestOrder / getWebSocketPrefix
 */

export const payTestApi = {
  /**
   * 获取指定应用支持的支付方式列表
   * GET /api/paytest/payways/{appId}
   */
  getPayways(appId) {
    return req.get(`/api/paytest/payways/${appId}`)
  },

  /**
   * 创建测试支付订单
   * POST /api/paytest/payOrders
   */
  createOrder(params) {
    return req.post('/api/paytest/payOrders', params)
  }
}

/**
 * 获取 WebSocket 连接前缀（ws:// / wss://）
 * 自动适配当前页面协议和 VITE_API_BASE_URL
 */
export function getWsPrefix() {
  let domain = document.location.protocol + '//' + document.location.host
  const baseUrl = import.meta.env.VITE_API_BASE_URL
  if (baseUrl && baseUrl !== '/') {
    domain = baseUrl
  }
  if (domain.startsWith('https:')) {
    return 'wss://' + domain.replace('https://', '')
  }
  return 'ws://' + domain.replace('http://', '')
}

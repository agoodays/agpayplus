import { request } from '@/lib/ag-axios'

/**
 * 获取今日/昨日统计数据
 * @param {string} queryDateRange - today 或 yesterday
 */
export function getPayDayCount(queryDateRange = 'today') {
  return request({
    url: '/api/mainChart/payDayCount',
    method: 'GET',
    params: { queryDateRange }
  })
}

/**
 * 获取支付趋势统计
 * @param {number} recentDay - 7 或 30
 */
export function getPayTrendCount(recentDay = 30) {
  return request({
    url: '/api/mainChart/payTrendCount',
    method: 'GET',
    params: { recentDay }
  })
}

/**
 * 获取服务商和商户数量
 */
export function getIsvAndMchCount() {
  return request({
    url: '/api/mainChart/isvAndMchCount',
    method: 'GET'
  })
}

/**
 * 获取支付方式统计
 * @param {object} params - 查询参数
 */
export function getPayTypeCount(params) {
  return request({
    url: '/api/mainChart/payTypeCount',
    method: 'GET',
    params
  })
}

/**
 * 获取交易统计
 * @param {object} params - 查询参数
 */
export function getPayCount(params) {
  return request({
    url: '/api/mainChart/payCount',
    method: 'GET',
    params
  })
}

export default {
  getPayDayCount,
  getPayTrendCount,
  getIsvAndMchCount,
  getPayTypeCount,
  getPayCount
}

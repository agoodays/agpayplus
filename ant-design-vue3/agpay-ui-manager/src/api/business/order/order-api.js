import { req } from '@/lib/ag-axios'

const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_PAY_ORDER = '/api/payOrder'
const API_URL_REFUND_ORDER = '/api/refundOrder'

export const orderApi = {
  queryPayOrderPage(params) {
    return req.list(API_URL_PAY_ORDER, params)
  },
  queryPayOrderCount(params) {
    return req.count(API_URL_PAY_ORDER, params)
  },
  getPayOrderById(payOrderId) {
    return req.getById(API_URL_PAY_ORDER, payOrderId)
  },
  queryRefundOrderPage(params) {
    return req.list(API_URL_REFUND_ORDER, params)
  },
  getRefundOrderById(refundOrderId) {
    return req.getById(API_URL_REFUND_ORDER, refundOrderId)
  },
  createRefund(data) {
    return req.add(API_URL_REFUND_ORDER, data)
  },
  queryMchPage(params) {
    return req.list(API_URL_MCH_LIST, params)
  }
}

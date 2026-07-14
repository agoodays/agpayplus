import { req } from '@/lib/ag-axios'

const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_PAY_ORDER = '/api/payOrder'
const API_URL_REFUND_ORDER = '/api/refundOrder'
const API_URL_TRANSFER_ORDER = '/api/transferOrders'
const API_URL_MCH_NOTIFY = '/api/mchNotify'

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
  },
  queryTransferOrderPage(params) {
    return req.list(API_URL_TRANSFER_ORDER, params)
  },
  queryTransferOrderCount(params) {
    return req.count(API_URL_TRANSFER_ORDER, params)
  },
  getTransferOrderById(transferId) {
    return req.getById(API_URL_TRANSFER_ORDER, transferId)
  },
  listMchNotify(params) {
    return req.list(API_URL_MCH_NOTIFY, params)
  },
  getMchNotifyById(notifyId) {
    return req.getById(API_URL_MCH_NOTIFY, notifyId)
  },
  resendMchNotify(notifyId) {
    return req.post(`${API_URL_MCH_NOTIFY}/resend/${notifyId}`)
  },
  exportMchNotify(params) {
    return req.export(API_URL_MCH_NOTIFY, 'mchNotify', params)
  }
}

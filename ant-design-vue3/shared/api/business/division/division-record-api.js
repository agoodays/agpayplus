import { req, request } from '@/lib/ag-axios'

const API_URL_IFDEFINES_LIST = '/api/payIfDefines'
const API_URL_MCH_LIST = '/api/mchInfo'
const API_URL_PAY_ORDER_DIVISION_RECORD_LIST = '/api/division/records'

export const divisionRecordApi = {
  queryPage(params) {
    return req.list(API_URL_PAY_ORDER_DIVISION_RECORD_LIST, params)
  },
  getById(recordId) {
    return req.getById(API_URL_PAY_ORDER_DIVISION_RECORD_LIST, recordId)
  },
  listIfDefine(params) {
    return req.list(API_URL_IFDEFINES_LIST, params)
  },
  listMch(params) {
    return req.list(API_URL_MCH_LIST, params)
  },
  resendDivision(recordId) {
    return request({
      url: `/api/division/records/resend/${recordId}`,
      method: 'POST'
    })
  }
}

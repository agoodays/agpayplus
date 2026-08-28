import { req } from '@/lib/ag-axios'

const API_URL_ACCOUNT_BILL_LIST = '/api/accountBill'

export const accountBillApi = {
  queryPage(params) {
    return req.list(API_URL_ACCOUNT_BILL_LIST, params)
  },
  getById(id) {
    return req.getById(API_URL_ACCOUNT_BILL_LIST, id)
  }
}

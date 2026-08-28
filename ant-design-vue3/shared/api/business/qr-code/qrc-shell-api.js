import { req } from '@/lib/ag-axios'

const API_URL_QRC_SHELL_LIST = '/api/qrc/shell'

export const qrcShellApi = {
  queryCardList(params) {
    return req.list(API_URL_QRC_SHELL_LIST, params)
  },
  getById(recordId) {
    return req.getById(API_URL_QRC_SHELL_LIST, recordId)
  },
  previewImage(data) {
    return req.post(`${API_URL_QRC_SHELL_LIST}/view`, data)
  },
  add(data) {
    return req.add(API_URL_QRC_SHELL_LIST, data)
  },
  updateById(recordId, data) {
    return req.updateById(API_URL_QRC_SHELL_LIST, recordId, data)
  },
  delById(recordId) {
    return req.delById(API_URL_QRC_SHELL_LIST, recordId)
  }
}

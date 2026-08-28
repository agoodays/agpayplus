import { req, request } from '@/lib/ag-axios'

const API_URL_ISV_PAYCONFIGS_LIST = '/api/isv/payConfigs'
const CERT_UPLOAD_ACTION = '/api/ossFiles/cert'

export const isvPayConfigApi = {
  queryCardList(isvNo) {
    return req.list(API_URL_ISV_PAYCONFIGS_LIST, { isvNo })
  },
  getUnique(infoId, ifCode) {
    return request({ url: `${API_URL_ISV_PAYCONFIGS_LIST}/${infoId}/${ifCode}`, method: 'GET' }, true, true, false)
  },
  save(data) {
    return req.add(API_URL_ISV_PAYCONFIGS_LIST, data)
  },
  certUploadAction: CERT_UPLOAD_ACTION
}

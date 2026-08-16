import { req } from '@/lib/ag-axios'

const API_URL = '/api/mchApply'

export const mchApplyApi = {
  queryPage(params) {
    return req.list(API_URL, params)
  },
  getById(applyId) {
    return req.getById(API_URL, applyId)
  },
  saveDraft(data) {
    return req.add(API_URL, data)
  },
  submit(applyId) {
    return req.post(`${API_URL}/${applyId}/submit`)
  },
  audit(applyId, data) {
    return req.post(`${API_URL}/${applyId}/audit`, data)
  },
  queryChannelResult(applyId) {
    return req.post(`${API_URL}/${applyId}/queryChannel`)
  },
  getSignUrl(applyId) {
    return req.post(`${API_URL}/${applyId}/signUrl`)
  },
  verify(applyId, data) {
    return req.post(`${API_URL}/${applyId}/verify`, data)
  },
  removeDraft(applyId) {
    return req.delById(API_URL, applyId)
  }
}

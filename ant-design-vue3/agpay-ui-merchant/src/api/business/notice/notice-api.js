import { req } from '@/lib/ag-axios'

export const API_URL_NOTICE = '/api/sysArticles'

/**
 * 公告领域 API。
 * 页面层仅依赖该对象，避免直接散落 URL 常量和通用 req 调用。
 */
export const noticeApi = {
  queryPage: (params) => req.list(API_URL_NOTICE, params),
  getById: (id) => req.getById(API_URL_NOTICE, id),
  add: (payload) => req.add(API_URL_NOTICE, payload),
  updateById: (id, payload) => req.updateById(API_URL_NOTICE, id, payload),
  delById: (id) => req.delById(API_URL_NOTICE, id)
}

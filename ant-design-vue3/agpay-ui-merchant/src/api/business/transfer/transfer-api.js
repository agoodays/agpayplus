import { req } from '@/lib/ag-axios'

/**
 * 商户转账 API
 * 对应 Vue2 manage.js 中的 queryMchTransferIfCode / doTransfer
 */

export const transferApi = {
  /**
   * 查询指定应用支持的转账通道列表
   * GET /api/mchTransfers/ifCodes/{appId}
   */
  queryIfCodes(appId) {
    return req.get(`/api/mchTransfers/ifCodes/${appId}`)
  },

  /**
   * 发起转账
   * POST /api/mchTransfers/doTransfer
   */
  doTransfer(params) {
    return req.post('/api/mchTransfers/doTransfer', params)
  }
}

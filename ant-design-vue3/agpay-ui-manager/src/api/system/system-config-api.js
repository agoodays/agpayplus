import request from '/@/lib/request'

/**
 * 系统配置 API
 */
export const systemConfigApi = {
  /**
   * 获取系统配置信息（包含主题、布局等）
   * @returns {Promise}
   */
  getSiteConfig() {
    // 在本地开发时，如果设置了 VITE_BYPASS_LOGIN，避免真实请求后端配置，返回 null
    if (import.meta.env.VITE_BYPASS_LOGIN === 'true') {
      return Promise.resolve(null)
    }

    return request.request({
      url: '/api/anon/siteInfos',
      method: 'GET',
      params: { queryConfig: 1 }
    })
  }
}

export default systemConfigApi

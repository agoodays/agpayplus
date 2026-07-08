import axios from 'axios'
import { message, Modal } from 'ant-design-vue'
import { useUserStore } from '@/store/modules/system/user'
import { translate } from '@/utils/i18n-util'
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { AgLoading } from '@/components'
import _ from 'lodash'

/**
 * 退出登录处理：清除用户状态并刷新页面
 */
function logout() {
  useUserStore().logout()
  location.reload()
}

/**
 * AgAxios HTTP 请求封装类（单例模式）
 * 
 * 功能特性：
 * - 请求队列管理：避免重复请求
 * - 请求取消：支持单个/全部取消
 * - 响应缓存：GET 请求可配置缓存（默认5分钟）
 * - 请求重试：失败自动重试（默认3次）
 * - 全局 Loading：可配置显示/隐藏
 * - Token 注入：自动添加 Authorization header
 * - 401 处理：会话超时自动跳转登录
 */
class AgAxios {
  constructor(baseUrl = import.meta.env.VITE_APP_API_BASE_URL) {
    this.baseUrl = baseUrl
    this.queue = {}           // 请求队列，用于全局 Loading 控制
    this.cancelTokens = {}    // 取消令牌映射
    this.cache = new Map()    // 响应缓存（key: 请求标识, value: 响应数据）
    this.instance = axios.create(this.baseConfig())
    this.setupInterceptors()  // 注册拦截器
  }

  /**
   * 获取 axios 基础配置
   * @returns {Object} axios 配置对象
   */
  baseConfig() {
    return {
      baseURL: this.baseUrl,
      timeout: 30000,         // 请求超时时间：30秒
      retry: 3,               // 重试次数：3次
      retryDelay: 1000        // 重试间隔：1秒
    }
  }

  /**
   * 生成缓存键
   * @param {Object} config - axios 请求配置
   * @returns {string} 缓存键（method:url:params:data）
   */
  generateCacheKey(config) {
    const { url, method, params, data } = config
    return `${method}:${url}:${JSON.stringify(params || {})}:${JSON.stringify(data || {})}`
  }

  /**
   * 添加请求到队列
   * @param {string} url - 请求 URL
   */
  addToQueue(url) {
    this.queue[url] = (this.queue[url] || 0) + 1
  }

  /**
   * 从队列移除请求
   * @param {string} url - 请求 URL
   */
  destroy(url) {
    if (this.queue[url]) {
      this.queue[url]--
      if (this.queue[url] === 0) {
        delete this.queue[url]
      }
    }
  }

  /**
   * 取消指定 URL 的请求
   * @param {string} url - 请求 URL
   */
  cancelRequest(url) {
    if (this.cancelTokens[url]) {
      this.cancelTokens[url]()
      delete this.cancelTokens[url]
    }
  }

  /**
   * 取消所有正在进行的请求
   */
  cancelAllRequests() {
    Object.values(this.cancelTokens).forEach((cancel) => cancel())
    this.cancelTokens = {}
    this.queue = {}
  }

  /**
   * 注册请求/响应拦截器
   */
  setupInterceptors() {
    // 请求拦截器
    this.instance.interceptors.request.use(
      (config) => {
        // 1. 创建取消令牌，支持手动取消请求
        const source = axios.CancelToken.source()
        config.cancelToken = source.token
        this.cancelTokens[config.url] = source.cancel

        // 2. 缓存命中检查：GET 请求且启用缓存时，优先返回缓存数据
        if (config.useCache && config.method === 'get') {
          const cacheKey = this.generateCacheKey(config)
          if (this.cache.has(cacheKey)) {
            return Promise.reject({ cached: true, data: this.cache.get(cacheKey) })
          }
        }

        // 3. 全局 Loading 控制：队列为空且配置显示 Loading 时显示
        if (!Object.keys(this.queue).length && config.showLoading) {
          AgLoading.show()
        }
        this.addToQueue(config.url)

        // 4. Token 注入：自动添加 Authorization header
        const token = useUserStore().getToken
        if (token && config.headers) {
          config.headers[ACCESS_TOKEN_NAME] = `Bearer ${token}`
        }

        // 5. GET 请求防缓存：添加时间戳参数
        if (config.method === 'get' && !config.params) {
          config.params = {}
        }
        if (config.method === 'get' && config.params) {
          config.params._t = Date.now()
        }

        return config
      },
      (error) => {
        AgLoading.hide()
        return Promise.reject(error)
      }
    )

    // 响应拦截器
    this.instance.interceptors.response.use(
      (response) => {
        // 1. 从队列移除请求
        this.destroy(response.config.url)

        // 2. 隐藏全局 Loading
        if (response.config.showLoading) {
          AgLoading.hide()
        }

        // 3. 缓存响应数据：GET 请求且启用缓存时，缓存5分钟后自动清除
        if (response.config.method === 'get' && response.config.useCache) {
          const cacheKey = this.generateCacheKey(response.config)
          this.cache.set(cacheKey, response.data)
          setTimeout(() => {
            this.cache.delete(cacheKey)
          }, 5 * 60 * 1000)
        }

        // 4. 非 JSON 响应直接返回（如文件下载）
        const contentType = response.headers['content-type'] || response.headers['Content-Type']
        if (contentType && contentType.indexOf('application/json') === -1) {
          return Promise.resolve(response)
        }

        // 5. Blob 响应直接返回
        if (response.data && response.data instanceof Blob) {
          return Promise.resolve(response.data)
        }

        // 6. JSON 响应处理：code !== 0 视为业务异常
        const resData = response.data
        if (resData.code && resData.code !== 0) {
          if (response.config.showErrorMsg) {
            message.error(resData.msg)
          }
          return Promise.reject(resData)
        } else {
          return Promise.resolve(resData.data)
        }
      },
      (error) => {
        // 1. 从队列移除请求
        this.destroy(error.config?.url)

        // 2. 隐藏全局 Loading
        if (error.config?.showLoading) {
          AgLoading.hide()
        }

        // 3. 缓存命中处理：返回缓存数据
        if (error.cached) {
          return Promise.resolve(error.data)
        }

        // 4. 请求取消处理
        if (axios.isCancel(error)) {
          return Promise.reject({ cancelled: true, message: '请求已取消' })
        }

        // 5. 请求重试机制：失败时自动重试（最多3次）
        const config = error.config
        if (config && config.retry > 0) {
          config.retry--
          const delay = config.retryDelay || 1000
          return new Promise((resolve) => {
            setTimeout(() => {
              resolve(this.instance(config))
            }, delay)
          })
        }

        // 6. 提取错误信息
        let errorInfo = error.response?.data?.data
        if (!errorInfo) {
          errorInfo = error.response?.data || error.message
        }

        // 7. 401 会话超时处理：弹出提示，3秒后自动跳转登录
        if (error.response?.status === 401) {
          const toLoginTimeout = setTimeout(logout, 3000)
          Modal.warning({
            title: '会话超时，请重新登录',
            content: '3s后将自动退出...',
            okText: '重新登录',
            cancelText: '关闭对话',
            onOk: logout,
            onCancel() {
              clearTimeout(toLoginTimeout)
            }
          })
        } else {
          // 8. 其他错误：显示错误消息
          if (error.config?.showErrorMsg) {
            message.error(JSON.stringify(errorInfo))
          }
        }

        return Promise.reject(errorInfo)
      }
    )
  }

  /**
   * 发起请求（内部方法）
   * @param {Object} options - axios 请求配置
   * @param {boolean} interceptorsFlag - 是否启用拦截器（保留参数，暂未使用）
   * @param {boolean} showErrorMsg - 是否显示错误消息
   * @param {boolean} showLoading - 是否显示全局 Loading
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 请求结果
   */
  request(options, interceptorsFlag = true, showErrorMsg = true, showLoading = false, useCache = false) {
    const isFormData = options.data instanceof FormData
    const defaultHeaders = isFormData ? {} : { 'Content-Type': 'application/json;charset=utf-8' }
    const mergedOptions = Object.assign(
      { headers: defaultHeaders },
      options,
      {
        showErrorMsg,
        showLoading,
        useCache
      }
    )

    // 补充 Token（拦截器中已处理，此处为冗余保护）
    if (!mergedOptions.headers[ACCESS_TOKEN_NAME]) {
      const token = useUserStore().getToken
      mergedOptions.headers[ACCESS_TOKEN_NAME] = `Bearer ${token}`
    }

    // 处理缓存命中的特殊情况
    return this.instance(mergedOptions).catch((error) => {
      if (error.cached) {
        return Promise.resolve(error.data)
      }
      throw error
    })
  }

  /**
   * 清除所有缓存
   */
  clearCache() {
    this.cache.clear()
  }
}

// ================================= 对外提供请求方法：通用请求，get， post, 下载download等 =================================

// 单例实例
const agAxios = new AgAxios()

/**
 * 通用请求方法
 * @param {Object} config - axios 请求配置
 * @param {boolean} interceptorsFlag - 是否启用拦截器
 * @param {boolean} showErrorMsg - 是否显示错误消息
 * @param {boolean} showLoading - 是否显示全局 Loading
 * @param {boolean} useCache - 是否启用缓存
 * @returns {Promise} 请求结果
 */
export const request = (
  config,
  interceptorsFlag = true,
  showErrorMsg = true,
  showLoading = false,
  useCache = false
) => {
  return agAxios.request(config, interceptorsFlag, showErrorMsg, showLoading, useCache)
}

/**
 * GET 请求（简化版）
 * @param {string} url - 请求 URL
 * @param {Object} params - 查询参数
 * @returns {Promise} 请求结果
 */
export const getRequest = (url, params) => {
  return request({ url, method: 'get', params })
}

/**
 * POST 请求（简化版）
 * @param {string} url - 请求 URL
 * @param {Object} data - 请求体数据
 * @returns {Promise} 请求结果
 */
export const postRequest = (url, data) => {
  return request({ data, url, method: 'post' })
}

/**
 * GET 请求（灵活版）
 * @param {string} url - 请求 URL
 * @param {Object} params - 查询参数
 * @param {Object} options - 额外配置（showLoading、useCache、showErrorMsg 等）
 * @returns {Promise} 响应数据
 * 
 * @example
 * // 普通 GET 请求
 * get('/api/mch', { page: 1, pageSize: 10 })
 * 
 * // 带缓存的 GET 请求
 * get('/api/mch', { page: 1 }, { useCache: true })
 * 
 * // 带全局 Loading 的 GET 请求
 * get('/api/mch', {}, { showLoading: true })
 */
export const get = (url, params = {}, options = {}) => {
  return request({
    url,
    method: 'get',
    params,
    ...options
  })
}

/**
 * POST 请求（灵活版）
 * @param {string} url - 请求 URL
 * @param {Object} data - 请求体数据
 * @param {Object} options - 额外配置（showLoading、showErrorMsg 等）
 * @returns {Promise} 响应数据
 * 
 * @example
 * // 普通 POST 请求
 * post('/api/mch', { name: 'test', code: 'M001' })
 * 
 * // 带全局 Loading 的 POST 请求
 * post('/api/mch', { name: 'test' }, { showLoading: true })
 */
export const post = (url, data = {}, options = {}) => {
  return request({
    url,
    method: 'post',
    data,
    ...options
  })
}

/**
 * PUT 请求（灵活版）
 * @param {string} url - 请求 URL
 * @param {Object} data - 请求体数据
 * @param {Object} options - 额外配置（showLoading、showErrorMsg 等）
 * @returns {Promise} 响应数据
 * 
 * @example
 * // 普通 PUT 请求
 * put('/api/mch/123', { name: 'updated' })
 * 
 * // 带全局 Loading 的 PUT 请求
 * put('/api/mch/123', { name: 'updated' }, { showLoading: true })
 */
export const put = (url, data = {}, options = {}) => {
  return request({
    url,
    method: 'put',
    data,
    ...options
  })
}

/**
 * DELETE 请求（灵活版）
 * @param {string} url - 请求 URL
 * @param {Object} params - 查询参数
 * @param {Object} options - 额外配置（showLoading、showErrorMsg 等）
 * @returns {Promise} 响应数据
 * 
 * @example
 * // 普通 DELETE 请求
 * del('/api/mch', { id: '123' })
 * 
 * // 带全局 Loading 的 DELETE 请求
 * del('/api/mch', { id: '123' }, { showLoading: true })
 */
export const del = (url, params = {}, options = {}) => {
  return request({
    url,
    method: 'delete',
    params,
    ...options
  })
}

/**
 * 通用 RESTful API 请求封装（不带全局 Loading）
 * 
 * 用法示例：
 * ```js
 * import { req } from '@/lib/ag-axios'
 * 
 * // 查询列表
 * const list = await req.list('/api/mch', { page: 1, pageSize: 10 })
 * 
 * // 查询单条
 * const item = await req.getById('/api/mch', '123')
 * 
 * // 新增
 * await req.add('/api/mch', { name: 'test' })
 * 
 * // 修改
 * await req.updateById('/api/mch', '123', { name: 'updated' })
 * 
 * // 删除
 * await req.delById('/api/mch', '123')
 * 
 * // 导出
 * await req.export('/api/mch', 'MERCHANT', { startTime: '2024-01-01' })
 * ```
 */
export const req = {
  /**
   * GET 分页列表
   * @param {string} url - 请求 URL
   * @param {Object} params - 查询参数
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 列表数据
   */
  list: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false, useCache)
  },

  /**
   * GET 请求（通用）
   * @param {string} url - 请求 URL
   * @param {Object} params - 查询参数
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 响应数据
   */
  get: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false, useCache)
  },

  /**
   * GET 统计总数（url + '/total'）
   * @param {string} url - 请求 URL
   * @param {Object} params - 查询参数
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 总数
   */
  total: (url, params, useCache = false) => {
    return request({ url: url + '/total', method: 'GET', params: params }, true, true, false, useCache)
  },

  /**
   * GET 统计数量（url + '/count'）
   * @param {string} url - 请求 URL
   * @param {Object} params - 查询参数
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 数量
   */
  count: (url, params, useCache = false) => {
    return request({ url: url + '/count', method: 'GET', params: params }, true, true, false, useCache)
  },

  /**
   * GET 导出数据（url + '/export/{bizType}'）
   * @param {string} url - 请求 URL
   * @param {string} bizType - 业务类型
   * @param {Object} params - 查询参数
   * @returns {Promise} 文件 Blob
   */
  export: (url, bizType, params) => {
    return request(
      { url: url + '/export/' + bizType, method: 'GET', params: params, responseType: 'blob' },
      true,
      true,
      false
    )
  },

  /**
   * POST 请求
   * @param {string} url - 请求 URL
   * @param {Object} data - 请求体数据
   * @returns {Promise} 响应数据
   */
  post: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false)
  },

  /**
   * POST 新增数据（与 post 等价）
   * @param {string} url - 请求 URL
   * @param {Object} data - 请求体数据
   * @returns {Promise} 响应数据
   */
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false)
  },

  /**
   * GET 根据 ID 查询（url + '/{bizId}'）
   * @param {string} url - 请求 URL
   * @param {string} bizId - 业务 ID
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 单条数据
   */
  getById: (url, bizId, useCache = false) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, false, useCache)
  },

  /**
   * PUT 根据 ID 更新（url + '/{bizId}'）
   * @param {string} url - 请求 URL
   * @param {string} bizId - 业务 ID
   * @param {Object} data - 更新数据
   * @returns {Promise} 响应数据
   */
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, false)
  },

  /**
   * DELETE 根据 ID 删除（url + '/{bizId}'）
   * @param {string} url - 请求 URL
   * @param {string} bizId - 业务 ID
   * @returns {Promise} 响应数据
   */
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, false)
  }
}

/**
 * 通用 RESTful API 请求封装（带全局 Loading）
 * 
 * 与 req 区别：所有请求都会显示全局 Loading
 * 
 * 用法示例：
 * ```js
 * import { reqLoad } from '@/lib/ag-axios'
 * 
 * // 查询列表（显示 Loading）
 * const list = await reqLoad.list('/api/mch', { page: 1, pageSize: 10 })
 * ```
 */
export const reqLoad = {
  /**
   * GET 分页列表（显示 Loading）
   * @param {string} url - 请求 URL
   * @param {Object} params - 查询参数
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 列表数据
   */
  list: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, true, useCache)
  },

  /**
   * POST 新增数据（显示 Loading）
   * @param {string} url - 请求 URL
   * @param {Object} data - 请求体数据
   * @returns {Promise} 响应数据
   */
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, true)
  },

  /**
   * GET 根据 ID 查询（显示 Loading）
   * @param {string} url - 请求 URL
   * @param {string} bizId - 业务 ID
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 单条数据
   */
  getById: (url, bizId, useCache = false) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, true, useCache)
  },

  /**
   * PUT 根据 ID 更新（显示 Loading）
   * @param {string} url - 请求 URL
   * @param {string} bizId - 业务 ID
   * @param {Object} data - 更新数据
   * @returns {Promise} 响应数据
   */
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, true)
  },

  /**
   * DELETE 根据 ID 删除（显示 Loading）
   * @param {string} url - 请求 URL
   * @param {string} bizId - 业务 ID
   * @returns {Promise} 响应数据
   */
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, true)
  }
}

/**
 * 文件上传配置与方法
 * 
 * 预定义上传路径：
 * - avatar: 头像上传
 * - ifBG: 接口背景图上传
 * - cert: 证书上传
 * - form: 表单文件上传
 */
export const upload = {
  avatar: '/api/ossFiles/avatar',
  ifBG: '/api/ossFiles/ifBG',
  cert: '/api/ossFiles/cert',
  form: '/api/ossFiles/form',

  /**
   * 获取上传表单参数（通过后端接口获取 OSS 上传所需的签名等参数）
   * @param {string} url - 本地接口地址（如 /api/ossFiles/form）
   * @param {string} fileName - 文件名
   * @param {number} fileSize - 文件大小
   * @param {boolean} useCache - 是否启用缓存
   * @returns {Promise} 表单参数（包含 formActionUrl、formParams、ossFileUrl 等）
   */
  getFormParams: (url, fileName, fileSize, useCache = false) => {
    return request({ url: url, method: 'GET', params: { fileName, fileSize } }, true, true, false, useCache)
  },

  /**
   * 上传单个文件
   * 
   * 上传逻辑说明：
   * - isLocalFile = true：本地上传，url 为相对路径，使用默认 VITE_APP_API_BASE_URL
   * - isLocalFile = false：OSS 上传，url 为 OSS 地址（如阿里云），需要清空 baseURL 避免拼接
   * 
   * @param {string} url - 上传地址（本地相对路径或 OSS 完整地址）
   * @param {boolean} isLocalFile - 是否本地文件上传（true=本地，false=OSS）
   * @param {Object} data - 表单数据（包含 file 对象及 OSS 签名参数）
   * @returns {Promise} 上传结果（本地返回 URL，OSS 返回空需使用 ossFileUrl）
   */
  singleFile: (url, isLocalFile, data) => {
    const formData = new FormData()
    for (const key in data) {
      if (Object.prototype.hasOwnProperty.call(data, key)) {
        formData.append(key, data[key])
      }
    }
    // 本地上传使用默认 baseURL，OSS 上传清空 baseURL 直接请求完整地址
    const actionUrl = isLocalFile ? { url: url } : { baseURL: '', url: url }
    const options = Object.assign(actionUrl, { method: 'POST', data: formData })
    return request(options)
    
    //  * - isLocalFile = true：本地上传，使用自定义 request 方法（经过拦截器，带 token）
    //  * - isLocalFile = false：OSS 上传，直接使用 axios 原生方法（避免单例 baseURL 污染）
    // if (isLocalFile) {
    //   // 本地上传：使用自定义 request 方法（经过拦截器，带 token）
    //   return request({ url: url, method: 'POST', data: formData })
    // } else {
    //   // OSS 上传：直接使用 axios 原生方法
    //   // 原因：单例模式的 agAxios 已固定 baseURL，无法动态切换到 OSS 地址
    //   // 使用原生 axios 避免经过自定义拦截器，直接请求 OSS 完整地址
    //   return axios({ url: url, method: 'POST', data: formData })
    // }
  }
}

// ================================= 加密 =================================

/**
 * 加密请求参数的post请求
 */
// export const postEncryptRequest = (url, data) => {
//   return request({
//     data: { encryptData: encryptData(data) },
//     url,
//     method: 'post',
//   })
// }

// ================================= 下载 =================================

/**
 * POST 下载文件
 * @param {string} url - 下载 URL
 * @param {Object} data - 请求体数据
 */
export const postDownload = function (url, data) {
  request({ method: 'post', url, data, responseType: 'blob' })
    .then((data) => {
      handleDownloadData(data)
    })
    .catch((error) => {
      handleDownloadError(error)
    })
}

/**
 * GET 下载文件
 * @param {string} url - 下载 URL
 * @param {Object} params - 查询参数
 */
export const getDownload = function (url, params) {
  request({ method: 'get', url, params, responseType: 'blob' })
    .then((data) => {
      handleDownloadData(data)
    })
    .catch((error) => {
      handleDownloadError(error)
    })
}

/**
 * 清除所有响应缓存
 */
export const clearCache = () => {
  agAxios.clearCache()
}

/**
 * 取消指定 URL 的请求
 * @param {string} url - 请求 URL
 */
export const cancelRequest = (url) => {
  agAxios.cancelRequest(url)
}

/**
 * 取消所有正在进行的请求
 */
export const cancelAllRequests = () => {
  agAxios.cancelAllRequests()
}

/**
 * 处理下载错误
 * 
 * 错误处理策略：
 * 1. Blob 类型错误：后端返回的 JSON 格式错误信息（如权限不足、参数错误等）
 *    - 使用 FileReader 读取 Blob 内容
 *    - 解析 JSON 获取错误消息
 *    - 显示错误提示
 * 2. 普通错误：网络错误、请求超时等
 *    - 显示国际化的网络错误提示
 * 
 * @param {Error|Blob} error - 错误对象或 Blob（后端返回的错误信息）
 */
function handleDownloadError(error) {
  if (error instanceof Blob) {
    // 后端返回的错误信息以 Blob 形式传递（通常是 JSON 格式）
    const fileReader = new FileReader()
    fileReader.readAsText(error)
    fileReader.onload = () => {
      const msg = fileReader.result
      const jsonMsg = JSON.parse(msg)
      message.destroy()
      message.error(jsonMsg.msg)
    }
  } else {
    // 网络错误、请求取消等普通错误
    message.destroy()
    message.error(translate('common.networkError'), error)
  }
}

/**
 * 处理下载数据：创建下载链接并触发浏览器下载
 * 
 * 步骤说明：
 * 1. 获取响应的 Content-Type（支持小写和大写 header）
 * 2. 若响应为 Blob 且无 Content-Type，使用 Blob 的 type 或默认值
 * 3. 创建 Blob URL
 * 4. 解析 Content-Disposition header 获取文件名（支持 fileName 和 filename）
 * 5. 创建临时 <a> 标签触发下载
 * 6. 清理临时资源
 * 
 * @param {Object|Blob} response - 响应对象（含 headers 和 data）或直接的 Blob 数据
 */
function handleDownloadData(response) {
  if (!response) {
    return
  }

  // 1. 获取 Content-Type：优先小写，其次大写
  let contentType = _.isUndefined(response.headers?.['content-type'])
    ? response.headers?.['Content-Type']
    : response.headers?.['content-type']

  // 2. 若无 Content-Type 且为 Blob，使用 Blob 自身的 type 或默认值
  if (!contentType && response instanceof Blob) {
    contentType = response.type || 'application/octet-stream'
  }

  // 3. 创建 Blob URL：response.data 为响应数据，直接 response 为 Blob
  let url = window.URL.createObjectURL(new Blob([response.data || response], { type: contentType }))
  
  // 4. 创建临时下载链接
  let link = document.createElement('a')
  link.style.display = 'none'
  link.href = url

  // 5. 解析文件名：支持 Content-Disposition header 的两种写法（fileName 和 filename）
  let str = _.isUndefined(response.headers?.['content-disposition'])
    ? response.headers?.['Content-Disposition']?.split(';')[1]
    : response.headers?.['content-disposition']?.split(';')[1]
  
  // 优先尝试 fileName，其次 filename，默认 'download'
  let filename = str ? (str.split('fileName=')[1] || str.split('filename=')[1]) : 'download'
  link.setAttribute('download', decodeURIComponent(filename))

  // 6. 触发下载
  document.body.appendChild(link)
  link.click()

  // 7. 清理临时资源
  document.body.removeChild(link)
  window.URL.revokeObjectURL(url)
}
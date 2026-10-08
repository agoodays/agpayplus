/**
 * ag-axios-core.js - 核心 HTTP 请求封装
 *
 * 提供 AgAxios 类（单例模式）和基础 HTTP 方法：
 * - request / get / post / put / del
 * - req（RESTful 封装，无全局 Loading）
 * - reqLoad（RESTful 封装，带全局 Loading）
 */
import { AgLoading } from '@/components'
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { useUserStore } from '@/store/modules/system/user'
import { message, Modal } from 'ant-design-vue'
import axios from 'axios'

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
    this.queue = {} // 请求队列，用于全局 Loading 控制
    this.cancelTokens = {} // 取消令牌映射
    this.cache = new Map() // 响应缓存（key: 请求标识, value: 响应数据）
    this.instance = axios.create(this.baseConfig())
    this.setupInterceptors() // 注册拦截器
  }

  /**
   * 获取 axios 基础配置
   */
  baseConfig() {
    return {
      baseURL: this.baseUrl,
      timeout: 30000,
      retry: 3,
      retryDelay: 1000
    }
  }

  /**
   * 生成缓存键
   */
  generateCacheKey(config) {
    const { url, method, params, data } = config
    return `${method}:${url}:${JSON.stringify(params || {})}:${JSON.stringify(data || {})}`
  }

  /**
   * 添加请求到队列
   */
  addToQueue(url) {
    this.queue[url] = (this.queue[url] || 0) + 1
  }

  /**
   * 从队列移除请求
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

        // 2. 缓存命中检查
        if (config.useCache && config.method === 'get') {
          const cacheKey = this.generateCacheKey(config)
          if (this.cache.has(cacheKey)) {
            return Promise.reject({ cached: true, data: this.cache.get(cacheKey) })
          }
        }

        // 3. 全局 Loading 控制
        if (!Object.keys(this.queue).length && config.showLoading) {
          AgLoading.show()
        }
        this.addToQueue(config.url)

        // 4. Token 注入
        const token = useUserStore().getToken
        if (token && config.headers) {
          config.headers[ACCESS_TOKEN_NAME] = `Bearer ${token}`
        }

        // 5. GET 请求防缓存
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
        this.destroy(response.config.url)

        if (response.config.showLoading) {
          AgLoading.hide()
        }

        // 缓存 GET 响应
        if (response.config.method === 'get' && response.config.useCache) {
          const cacheKey = this.generateCacheKey(response.config)
          this.cache.set(cacheKey, response.data)
          setTimeout(() => {
            this.cache.delete(cacheKey)
          }, 5 * 60 * 1000)
        }

        // 非 JSON 响应直接返回
        const contentType = response.headers['content-type'] || response.headers['Content-Type']
        if (contentType && contentType.indexOf('application/json') === -1) {
          return Promise.resolve(response)
        }

        // Blob 响应直接返回
        if (response.data && response.data instanceof Blob) {
          return Promise.resolve(response.data)
        }

        // JSON 响应处理：code 为 undefined/null/0 视为成功，其他为业务错误
        const resData = response.data
        const isSuccess = resData.code === undefined || resData.code === null || resData.code === 0
        if (isSuccess) {
          return Promise.resolve(resData.data)
        }
        if (response.config.showErrorMsg) {
          message.error(resData.msg)
        }
        return Promise.reject(resData)
      },
      (error) => {
        this.destroy(error.config?.url)

        if (error.config?.showLoading) {
          AgLoading.hide()
        }

        // 缓存命中
        if (error.cached) {
          return Promise.resolve(error.data)
        }

        // 请求取消
        if (axios.isCancel(error)) {
          return Promise.reject({ cancelled: true, message: '请求已取消' })
        }

        // 请求重试：仅对幂等请求（GET/HEAD/OPTIONS）且仅在网络错误时重试，避免非幂等请求重复提交
        const config = error.config
        const method = config?.method?.toLowerCase()
        const isIdempotent = method === 'get' || method === 'head' || method === 'options'
        const isNetworkError = !error.response // 仅无 response（网络错误/超时）才重试，4xx/5xx 不重试
        if (config && isIdempotent && isNetworkError && config.retry > 0) {
          config.retry--
          const delay = config.retryDelay || 1000
          return new Promise((resolve) => {
            setTimeout(() => {
              resolve(this.instance(config))
            }, delay)
          })
        }

        // 提取错误信息
        let errorInfo = error.response?.data?.data
        if (!errorInfo) {
          errorInfo = error.response?.data || error.message
        }

        // 401 会话超时（Modal.warning 仅显示 OK 按钮，无需 onCancel/cancelText）
        if (error.response?.status === 401 && import.meta.env.VITE_BYPASS_LOGIN !== 'true') {
          const toLoginTimeout = setTimeout(logout, 3000)
          Modal.warning({
            title: '会话超时，请重新登录',
            content: '3s后将自动退出...',
            okText: '重新登录',
            onOk: () => {
              clearTimeout(toLoginTimeout)
              logout()
            }
          })
        } else {
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
   */
  request(options, interceptorsFlag = true, showErrorMsg = true, showLoading = false, useCache = false) {
    const isFormData = options.data instanceof FormData
    const defaultHeaders = isFormData ? {} : { 'Content-Type': 'application/json;charset=utf-8' }
    const mergedOptions = Object.assign(
      { headers: defaultHeaders },
      options,
      { showErrorMsg, showLoading, useCache }
    )

    // 补充 Token
    if (!mergedOptions.headers[ACCESS_TOKEN_NAME]) {
      const token = useUserStore().getToken
      mergedOptions.headers[ACCESS_TOKEN_NAME] = `Bearer ${token}`
    }

    return this.instance(mergedOptions)
  }

  /**
   * 清除所有缓存
   */
  clearCache() {
    this.cache.clear()
  }
}

// 单例实例
const agAxios = new AgAxios()

// ==================== 对外提供请求方法 ====================

/**
 * 通用请求方法
 */
export const request = (config, interceptorsFlag = true, showErrorMsg = true, showLoading = false, useCache = false) => {
  return agAxios.request(config, interceptorsFlag, showErrorMsg, showLoading, useCache)
}

/**
 * GET 请求（简化版）
 */
export const getRequest = (url, params) => {
  return request({ url, method: 'get', params })
}

/**
 * POST 请求（简化版）
 */
export const postRequest = (url, data) => {
  return request({ data, url, method: 'post' })
}

/**
 * GET 请求（灵活版）
 */
export const get = (url, params = {}, options = {}) => {
  return request({ url, method: 'get', params, ...options })
}

/**
 * POST 请求（灵活版）
 */
export const post = (url, data = {}, options = {}) => {
  return request({ url, method: 'post', data, ...options })
}

/**
 * PUT 请求（灵活版）
 */
export const put = (url, data = {}, options = {}) => {
  return request({ url, method: 'put', data, ...options })
}

/**
 * DELETE 请求（灵活版）
 */
export const del = (url, params = {}, options = {}) => {
  return request({ url, method: 'delete', params, ...options })
}

/**
 * 通用 RESTful API 请求封装（不带全局 Loading）
 */
export const req = {
  list: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false, useCache)
  },
  get: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false, useCache)
  },
  total: (url, params, useCache = false) => {
    return request({ url: url + '/total', method: 'GET', params: params }, true, true, false, useCache)
  },
  count: (url, params, useCache = false) => {
    return request({ url: url + '/count', method: 'GET', params: params }, true, true, false, useCache)
  },
  export: (url, bizType, params) => {
    return request({ url: url + '/export/' + bizType, method: 'GET', params: params, responseType: 'blob' }, true, true, false)
  },
  post: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false)
  },
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false)
  },
  getById: (url, bizId, useCache = false) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, false, useCache)
  },
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, false)
  },
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, false)
  }
}

/**
 * 通用 RESTful API 请求封装（带全局 Loading）
 */
export const reqLoad = {
  list: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, true, useCache)
  },
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, true)
  },
  getById: (url, bizId, useCache = false) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, true, useCache)
  },
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, true)
  },
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, true)
  }
}

/**
 * 清除所有响应缓存
 */
export const clearCache = () => {
  agAxios.clearCache()
}

/**
 * 取消指定 URL 的请求
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

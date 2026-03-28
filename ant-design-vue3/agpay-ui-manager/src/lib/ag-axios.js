/*
 *  ajax请求
 *
 */
import axios from 'axios'
import { message, Modal } from 'ant-design-vue'
import { useUserStore } from '@/store/modules/system/user'
import { translate } from '@/utils/i18n-util'
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { AgLoading } from '@/components'
import _ from 'lodash'

// 退出系统
function logout() {
  useUserStore().logout()
  location.reload() // 退出时 重置缓存
}

class AgAxios {
  constructor(baseUrl = import.meta.env.VITE_APP_API_BASE_URL) {
    this.baseUrl = baseUrl
    this.queue = {} // 发送队列
    this.cancelTokens = {} // 取消令牌
    this.cache = new Map() // 请求缓存
  }
  // 基础配置信息
  baseConfig() {
    const headers = {
      'Content-Type': 'application/json;charset=utf-8'
    }
    const token = useUserStore().getToken
    headers[ACCESS_TOKEN_NAME] = `Bearer ${token}`
    return {
      baseURL: this.baseUrl,
      headers: headers,
      timeout: 30000,
      retry: 3,
      retryDelay: 1000
    }
  }
  // 生成缓存键
  generateCacheKey(config) {
    const { url, method, params, data } = config
    return `${method}:${url}:${JSON.stringify(params || {})}:${JSON.stringify(data || {})}`
  }
  // 添加到发送队列
  addToQueue(url) {
    this.queue[url] = (this.queue[url] || 0) + 1
  }
  // 从发送队列移除
  destroy(url) {
    if (this.queue[url]) {
      this.queue[url]--
      if (this.queue[url] === 0) {
        delete this.queue[url]
      }
    }
  }
  // 取消请求
  cancelRequest(url) {
    if (this.cancelTokens[url]) {
      this.cancelTokens[url]()
      delete this.cancelTokens[url]
    }
  }
  // 取消所有请求
  cancelAllRequests() {
    Object.values(this.cancelTokens).forEach((cancel) => cancel())
    this.cancelTokens = {}
    this.queue = {}
  }
  useInterceptors(instance, url, showErrorMsg, showLoading, useCache) {
    // 请求拦截
    instance.interceptors.request.use(
      (config) => {
        // 生成取消令牌
        const source = axios.CancelToken.source()
        config.cancelToken = source.token
        this.cancelTokens[url] = source.cancel

        // 检查缓存
        if (useCache && config.method === 'get') {
          const cacheKey = this.generateCacheKey(config)
          if (this.cache.has(cacheKey)) {
            return Promise.reject({ cached: true, data: this.cache.get(cacheKey) })
          }
        }

        // 添加全局的loading...
        if (!Object.keys(this.queue).length && showLoading) {
          AgLoading.show() // 加载中显示loading组件
        }
        this.addToQueue(url)

        // 添加时间戳，防止缓存
        if (config.method === 'get' && !config.params) {
          config.params = {}
        }
        if (config.method === 'get' && config.params) {
          config.params._t = Date.now()
        }

        return config
      },
      (error) => {
        AgLoading.hide() // 报错关闭loading组件
        return Promise.reject(error)
      }
    )
    // 响应拦截
    instance.interceptors.response.use(
      (response) => {
        this.destroy(url)

        if (showLoading) {
          AgLoading.hide() // 报错关闭loading组件
        }

        // 缓存处理
        if (response.config.method === 'get' && response.config.useCache) {
          const cacheKey = this.generateCacheKey(response.config)
          this.cache.set(cacheKey, response.data)
          // 设置缓存过期时间
          setTimeout(
            () => {
              this.cache.delete(cacheKey)
            },
            5 * 60 * 1000
          ) // 5分钟过期
        }

        // 根据content-type ，判断是否为 json 数据
        const contentType = response.headers['content-type']
          ? response.headers['content-type']
          : response.headers['Content-Type']
        if (contentType.indexOf('application/json') === -1) {
          return Promise.resolve(response)
        }
        // 如果是json数据
        if (response.data && response.data instanceof Blob) {
          return Promise.resolve(response.data)
        }
        const resData = response.data // 接口实际返回数据 格式为：{code: '', msg: '', data: ''}， res.data 是axios封装对象的返回数据；
        if (resData.code && resData.code !== 0) {
          // 相应结果不为0， 说明异常
          if (showErrorMsg) {
            message.error(resData.msg) // 显示异常信息
          }
          return Promise.reject(resData)
        } else {
          return Promise.resolve(resData.data)
        }
      },
      (error) => {
        this.destroy(url)

        if (showLoading) {
          AgLoading.hide() // 报错关闭loading组件
        }

        // 处理缓存命中
        if (error.cached) {
          return Promise.resolve(error.data)
        }

        // 处理取消请求
        if (axios.isCancel(error)) {
          return Promise.reject({ cancelled: true, message: '请求已取消' })
        }

        // 处理重试
        const config = error.config
        if (config && config.retry > 0) {
          config.retry--
          const delay = config.retryDelay || 1000
          return new Promise((resolve) => {
            setTimeout(() => {
              resolve(instance(config))
            }, delay)
          })
        }

        let errorInfo = error.response && error.response.data && error.response.data.data
        if (!errorInfo) {
          errorInfo = error.response && error.response.data ? error.response.data : error.message
        }
        if (error.response && error.response.status === 401) {
          // 无访问权限，会话超时， 提示用户信息 & 退出系统
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
          if (showErrorMsg) {
            message.error(JSON.stringify(errorInfo)) // 显示异常信息
          }
        }

        return Promise.reject(errorInfo)
      }
    )
  }
  /**
   * ajax请求
   * @options options 参数
   * @interceptorsFlag 是否进行自定义拦截器处理，默认为： true
   * @showErrorMsg 发送请求出现异常是否全局提示错误信息，默认为： true
   * @showLoading 发送请求前后显示全局loading，默认为： false
   * @useCache 是否使用缓存，默认为： false
   */
  request(options, interceptorsFlag = true, showErrorMsg = true, showLoading = false, useCache = false) {
    const instance = axios.create()
    options = Object.assign(this.baseConfig(), options, { useCache })
    if (interceptorsFlag) {
      // 注入 req, respo 拦截器
      this.useInterceptors(instance, options.url, showErrorMsg, showLoading, useCache)
    }

    return instance(options)
  }
  /**
   * 清空缓存
   */
  clearCache() {
    this.cache.clear()
  }
}

// ================================= 对外提供请求方法：通用请求，get， post, 下载download等 =================================

const agAxios = new AgAxios()

/**
 * 通用请求封装
 *
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
 * get请求
 */
export const getRequest = (url, params) => {
  return request({ url, method: 'get', params })
}

/**
 * post请求
 */
export const postRequest = (url, data) => {
  return request({ data, url, method: 'post' })
}

/**
 * GET 请求
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
 * POST 请求
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
 * PUT 请求
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
 * DELETE 请求
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
 *  全系列 restful api格式, 定义通用req对象
 *
 */
export const req = {
  // 通用列表查询接口
  list: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false, useCache)
  },

  // 通用获取数据接口
  get: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false, useCache)
  },

  // 通用列表查询统计接口
  total: (url, params, useCache = false) => {
    return request({ url: url + '/total', method: 'GET', params: params }, true, true, false, useCache)
  },

  // 通用列表查询统计接口
  count: (url, params, useCache = false) => {
    return request({ url: url + '/count', method: 'GET', params: params }, true, true, false, useCache)
  },

  // 通用列表数据导出接口
  export: (url, bizType, params) => {
    return request(
      { url: url + '/export/' + bizType, method: 'GET', params: params, responseType: 'blob' },
      true,
      true,
      false
    )
  },

  // 通用Post接口
  post: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false)
  },

  // 通用新增接口
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false)
  },

  // 通用查询单条数据接口
  getById: (url, bizId, useCache = false) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, false, useCache)
  },

  // 通用修改接口
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, false)
  },

  // 通用删除接口
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, false)
  }
}

/**
 *  全系列 restful api格式 (全局loading方式)
 *
 */
export const reqLoad = {
  // 通用列表查询接口
  list: (url, params, useCache = false) => {
    return request({ url: url, method: 'GET', params: params }, true, true, true, useCache)
  },

  // 通用新增接口
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, true)
  },

  // 通用查询单条数据接口
  getById: (url, bizId, useCache = false) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, true, useCache)
  },

  // 通用修改接口
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, true)
  },

  // 通用删除接口
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, true)
  }
}

/**
 * 上传图片/文件地址
 *
 */
export const upload = {
  avatar: agAxios.baseUrl + '/api/ossFiles/avatar',
  ifBG: agAxios.baseUrl + '/api/ossFiles/ifBG',
  cert: agAxios.baseUrl + '/api/ossFiles/cert',
  form: agAxios.baseUrl + '/api/ossFiles/form',
  /**
   * 获取上传表单参数
   *
   */
  getFormParams: (url, fileName, fileSize, useCache = false) => {
    return request({ baseURL: url, method: 'GET', params: { fileName, fileSize } }, true, true, false, useCache)
  },
  /**
   * 上传单个文件
   *
   */
  singleFile: (url, data) => {
    const formData = new FormData()
    for (const key in data) {
      if (Object.prototype.hasOwnProperty.call(data, key)) {
        formData.append(key, data[key])
      }
    }
    return request({ baseURL: url, method: 'POST', data: formData })
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
 * 文件下载
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
 * 清空缓存
 */
export const clearCache = () => {
  agAxios.clearCache()
}

/**
 * 取消请求
 */
export const cancelRequest = (url) => {
  agAxios.cancelRequest(url)
}

/**
 * 取消所有请求
 */
export const cancelAllRequests = () => {
  agAxios.cancelAllRequests()
}

function handleDownloadError(error) {
  if (error instanceof Blob) {
    const fileReader = new FileReader()
    fileReader.readAsText(error)
    fileReader.onload = () => {
      const msg = fileReader.result
      const jsonMsg = JSON.parse(msg)
      message.destroy()
      message.error(jsonMsg.msg)
    }
  } else {
    message.destroy()
    message.error(translate('common.networkError'), error)
  }
}

function handleDownloadData(response) {
  if (!response) {
    return
  }

  // 获取返回类型
  let contentType = _.isUndefined(response.headers['content-type'])
    ? response.headers['Content-Type']
    : response.headers['content-type']

  // 构建下载数据
  let url = window.URL.createObjectURL(new Blob([response.data], { type: contentType }))
  let link = document.createElement('a')
  link.style.display = 'none'
  link.href = url

  // 从消息头获取文件名
  let str = _.isUndefined(response.headers['content-disposition'])
    ? response.headers['Content-Disposition'].split(';')[1]
    : response.headers['content-disposition'].split(';')[1]

  let filename = _.isUndefined(str.split('fileName=')[1]) ? str.split('filename=')[1] : str.split('fileName=')[1]
  link.setAttribute('download', decodeURIComponent(filename))

  // 触发点击下载
  document.body.appendChild(link)
  link.click()

  // 下载完释放
  document.body.removeChild(link) // 下载完成移除元素
  window.URL.revokeObjectURL(url) // 释放掉blob对象
}

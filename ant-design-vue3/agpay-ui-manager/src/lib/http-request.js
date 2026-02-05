/**
 * HTTP 请求包装对象
 * 基于 axios 实现
 */

import axios from 'axios'
import { message } from 'ant-design-vue'
import { useUserStore } from '/@/store/modules/system/user'
import { useAppStore } from '/@/store/modules/system/app'
import { router } from '/@/router'
import localStorageKeyConst from '/@/constants/local-storage-key-const'
import { PAGE_PATH_LOGIN } from '/@/constants/common-const'

class HttpRequest {
  constructor(baseUrl = import.meta.env.VITE_APP_API_BASE_URL) {
    this.baseUrl = baseUrl
    this.queue = {} // 发送队列
  }

  // 基础配置信息
  baseConfig() {
    const headers = {}
    const token = localStorage.getItem(localStorageKeyConst.USER_TOKEN)
    if (token) {
      headers[import.meta.env.VITE_APP_TOKEN_NAME || 'authorization'] = `Bearer ${token}`
    }
    return {
      baseURL: this.baseUrl,
      headers: headers,
      timeout: 30000
    }
  }

  destroy(url, showLoading) {
    delete this.queue[url]
  }

  interceptors(instance, url, showErrorMsg, showLoading) {
    // 请求拦截
    instance.interceptors.request.use(
      (config) => {
        // 添加全局的loading...
        if (!Object.keys(this.queue).length && showLoading) {
          const appStore = useAppStore()
          appStore.showLoading() // 加载中显示loading组件
        }
        this.queue[url] = true
        return config
      },
      (error) => {
        const appStore = useAppStore()
        appStore.hideLoading() // 报错关闭loading组件
        return Promise.reject(error)
      }
    )

    // 响应拦截
    instance.interceptors.response.use(
      (res) => {
        this.destroy(url, showLoading)

        if (showLoading) {
          const appStore = useAppStore()
          appStore.hideLoading() // 关闭loading组件
        }

        const resData = res.data // 接口实际返回数据 格式为：{code: '', msg: '', data: ''}

        if (!resData) {
          return resData
        }

        if (res.config.responseType) {
          return resData
        }

        if (resData.code !== 0) {
          // 相应结果不为0， 说明异常
          if (showErrorMsg) {
            message.error(resData.msg) // 显示异常信息
          }

          return Promise.reject(resData)
        } else {
          return resData.data
        }
      },
      (error) => {
        this.destroy(url, showLoading)

        if (showLoading) {
          const appStore = useAppStore()
          appStore.hideLoading() // 报错关闭loading组件
        }

        let errorInfo = error.response && error.response.data && error.response.data.data
        if (!errorInfo) {
          errorInfo = error.response?.data || error.message
        }

        if (error.response?.status === 401) {
          // 无访问权限，会话超时， 提示用户信息 & 退出系统
          const toLoginTimeout = setTimeout(function () {
            const userStore = useUserStore()
            userStore.logout()
            router.push({ path: PAGE_PATH_LOGIN })
          }, 3000)

          message.warning({
            content: '会话超时，请重新登录！3s后将自动退出...',
            duration: 3,
            onClose: () => {
              clearTimeout(toLoginTimeout)
              const userStore = useUserStore()
              userStore.logout()
              router.push({ path: PAGE_PATH_LOGIN })
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
   * 发送请求
   * @param {*} options 请求配置
   * @param {*} interceptorsFlag 是否进行自定义拦截器处理，默认为： true
   * @param {*} showErrorMsg 发送请求出现异常是否全局提示错误信息
   * @param {*} showLoading 发送请求前后显示全局loading
   * @returns
   */
  request(options, interceptorsFlag = true, showErrorMsg = true, showLoading = false) {
    const instance = axios.create()
    options = Object.assign(this.baseConfig(), options)
    if (interceptorsFlag) {
      // 注入 req, respo 拦截器
      this.interceptors(instance, options.url, showErrorMsg, showLoading)
    }

    return instance(options)
  }
}

export default HttpRequest

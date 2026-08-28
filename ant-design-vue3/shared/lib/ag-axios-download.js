/**
 * ag-axios-download.js - 文件下载模块
 *
 * 提供文件下载相关功能：
 * - postDownload: POST 下载文件
 * - getDownload: GET 下载文件
 * - downloadFile: 通用文件下载
 * - downloadExcel: Excel 下载（deprecated）
 */
import { request } from './ag-axios-core'
import { message } from 'ant-design-vue'
import { translate } from '@/utils/i18n-util'
import _ from 'lodash'

/**
 * 将 Blob 内容读取为文本
 */
function readBlobAsText(blob) {
  return new Promise((resolve, reject) => {
    const fileReader = new FileReader()
    fileReader.onload = () => resolve(fileReader.result)
    fileReader.onerror = () => reject(fileReader.error)
    fileReader.readAsText(blob)
  })
}

/**
 * 处理下载错误
 */
async function handleDownloadError(error) {
  message.destroy()
  try {
    if (error instanceof Blob) {
      const msg = await readBlobAsText(error)
      const jsonMsg = JSON.parse(msg)
      message.error(jsonMsg.msg)
    } else {
      message.error(translate('common.networkError'), error)
    }
  } catch (readError) {
    message.error(translate('common.networkError'), error)
  }
}

/**
 * 处理下载数据
 */
function handleDownloadData(response) {
  if (!response) {
    return
  }

  let contentType = _.isUndefined(response.headers?.['content-type'])
    ? response.headers?.['Content-Type']
    : response.headers?.['content-type']

  if (!contentType && response instanceof Blob) {
    contentType = response.type || 'application/octet-stream'
  }

  let url = window.URL.createObjectURL(new Blob([response.data || response], { type: contentType }))

  let link = document.createElement('a')
  link.style.display = 'none'
  link.href = url

  let str = _.isUndefined(response.headers?.['content-disposition'])
    ? response.headers?.['Content-Disposition']?.split(';')[1]
    : response.headers?.['content-disposition']?.split(';')[1]

  let filename = str ? (str.split('fileName=')[1] || str.split('filename=')[1]) : 'download'
  link.setAttribute('download', decodeURIComponent(filename))

  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  window.URL.revokeObjectURL(url)
}

/**
 * POST 下载文件
 */
export const postDownload = async (url, data) => {
  try {
    const res = await request({ method: 'post', url, data, responseType: 'blob' })
    handleDownloadData(res)
    return res
  } catch (error) {
    await handleDownloadError(error)
    throw error
  }
}

/**
 * GET 下载文件
 */
export const getDownload = async (url, params) => {
  try {
    const res = await request({ method: 'get', url, params, responseType: 'blob' })
    handleDownloadData(res)
    return res
  } catch (error) {
    await handleDownloadError(error)
    throw error
  }
}

/**
 * 通用文件下载工具函数
 */
export const downloadFile = async (apiPromise, fileName) => {
  try {
    const res = await apiPromise
    const blob = new Blob([res])
    if ('download' in document.createElement('a')) {
      const elink = document.createElement('a')
      elink.download = fileName
      elink.style.display = 'none'
      elink.href = URL.createObjectURL(blob)
      document.body.appendChild(elink)
      elink.click()
      URL.revokeObjectURL(elink.href)
      document.body.removeChild(elink)
    } else {
      navigator.msSaveBlob(blob, fileName)
    }
    return res
  } catch (error) {
    await handleDownloadError(error)
    throw error
  }
}

/**
 * Excel 下载工具函数（deprecated）
 */
export const downloadExcel = downloadFile

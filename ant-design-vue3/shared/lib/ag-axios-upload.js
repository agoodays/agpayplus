/**
 * ag-axios-upload.js - 文件上传模块
 *
 * 提供文件上传相关功能：
 * - uploadUrl: 预定义上传路径
 * - getUploadFormParams: 获取 OSS 上传参数
 * - uploadSingleFile: 上传单个文件
 * - uploadFile: 完整上传流程
 * - upload: 兼容旧版上传对象
 */
import { request } from './ag-axios-core'

/**
 * 文件上传 URL 常量配置
 */
export const uploadUrl = {
  avatar: '/api/ossFiles/avatar',
  ifBG: '/api/ossFiles/ifBG',
  cert: '/api/ossFiles/cert',
  form: '/api/ossFiles/form'
}

/**
 * 获取上传表单参数
 */
export const getUploadFormParams = async (url, fileName, fileSize, useCache = false) => {
  return await request({ url, method: 'GET', params: { fileName, fileSize } }, true, true, false, useCache)
}

/**
 * 上传单个文件
 */
export const uploadSingleFile = async (url, isLocalFile, data) => {
  const formData = new FormData()
  for (const key in data) {
    if (Object.prototype.hasOwnProperty.call(data, key)) {
      formData.append(key, data[key])
    }
  }
  const actionUrl = isLocalFile ? { url } : { baseURL: '', url }
  const options = Object.assign(actionUrl, { method: 'POST', data: formData })
  return await request(options)
}

/**
 * 统一文件上传方法
 */
export const uploadFile = async (uploadUrl, file) => {
  const res = await getUploadFormParams(uploadUrl, file.name, file.size)
  const isLocalFile = res.formActionUrl === 'LOCAL_SINGLE_FILE_URL'

  const formParams = isLocalFile
    ? res.formParams
    : {
        OSSAccessKeyId: res.formParams.ossAccessKeyId,
        key: res.formParams.key,
        Signature: res.formParams.signature,
        policy: res.formParams.policy,
        success_action_status: res.formParams.successActionStatus
      }

  const data = Object.assign(formParams, { file })
  const formActionUrl = isLocalFile ? uploadUrl : res.formActionUrl

  const response = await uploadSingleFile(formActionUrl, isLocalFile, data)
  return isLocalFile ? response : res.ossFileUrl
}

/**
 * 兼容旧版上传对象
 */
export const upload = {
  ...uploadUrl,
  getFormParams: getUploadFormParams,
  singleFile: uploadSingleFile
}

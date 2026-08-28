/**
 * ag-axios.js - HTTP 请求统一入口
 *
 * 重新导出 core / upload / download 三个模块的所有功能，
 * 保持向后兼容，原有 import 路径无需修改。
 *
 * 模块结构：
 * - ag-axios-core.js: 核心 HTTP 请求（AgAxios 类 + 基础方法）
 * - ag-axios-upload.js: 文件上传功能
 * - ag-axios-download.js: 文件下载功能
 */

// 核心请求方法
export {
  request,
  getRequest,
  postRequest,
  get,
  post,
  put,
  del,
  req,
  reqLoad,
  clearCache,
  cancelRequest,
  cancelAllRequests
} from './ag-axios-core'

// 上传功能
export {
  uploadUrl,
  getUploadFormParams,
  uploadSingleFile,
  uploadFile,
  upload
} from './ag-axios-upload'

// 下载功能
export {
  postDownload,
  getDownload,
  downloadFile,
  downloadExcel
} from './ag-axios-download'

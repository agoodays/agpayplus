import { req, upload, uploadFile } from '@/lib/ag-axios'

export const currentApi = {
  modifyUserInfo(data) {
    return req.post('/api/current/modifyUserInfo', data)
  },
  modifyPwd(data) {
    return req.post('/api/current/modifyPwd', data)
  },
  /**
   * 上传头像
   * @param {File} file - 文件对象
   * @returns {Promise<string>} 上传后的文件 URL
   */
  async uploadAvatar(file) {
    return await uploadFile(upload.avatar, file)
  }
}

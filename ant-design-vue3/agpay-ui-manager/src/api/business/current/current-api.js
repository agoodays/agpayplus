import { req, upload } from '@/lib/ag-axios'

export const currentApi = {
  modifyUserInfo(data) {
    return req.post('/api/current/modifyUserInfo', data)
  },
  modifyPwd(data) {
    return req.post('/api/current/modifyPwd', data)
  },
  uploadAvatar(formData) {
    // 头像上传是本地上传，isLocalFile = true
    return upload.singleFile(upload.avatar, true, formData)
  }
}

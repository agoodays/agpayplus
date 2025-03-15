import storage from '@/utils/agpayStorageWrapper'
import { login, qrCodeLogin, logout } from '@/api/login'
import appConfig from '@/config/appConfig'

const user = {
  state: {
    token: '',
    userName: '', // 真实姓名
    safeWord: '', // 预留信息
    shortName: '', // 简称
    userId: '', // 用户ID
    avatarImgPath: '', // 头像
    allMenuRouteTree: [], // 全部动态 router
    accessList: [], // 用户权限集合
    isAdmin: '', // 是否是超级管理员
    loginUsername: '', // 登录用户名
    state: '', // 用户状态
    sysType: '', // 所属系统
    belongInfoId: '', // 所属代理商ID
    telphone: '', // 手机号
    sex: '' // 性别
  },

  mutations: {
    SET_TOKEN: (state, token) => {
      state.token = token
    },
    // 设置头像
    SET_AVATAR (state, avatarPath) {
      state.avatarImgPath = avatarPath
    },
    // 设置用户信息
    SET_USER_INFO: (state, userInfo) => {
      state.userId = userInfo.sysUserId // 用户ID
      state.userName = userInfo.realname // 真实姓名
      state.safeWord = userInfo.safeWord // 预留信息
      state.shortName = userInfo.shortName // 简称
      state.avatarImgPath = userInfo.avatarUrl // 头像
      state.accessList = userInfo.entIdList // 权限集合
      state.allMenuRouteTree = userInfo.allMenuRouteTree // 全部路由集合
      state.isAdmin = userInfo.isAdmin // 是否是超级管理员
      state.loginUsername = userInfo.loginUsername // 登录用户名
      state.state = userInfo.state // 用户状态
      state.sysType = userInfo.sysType // 所属系统
      state.belongInfoId = userInfo.belongInfoId // 所属代理商ID
      state.telphone = userInfo.telphone // 手机号
      state.sex = userInfo.sex // 性别
    }
  },

  actions: {
    // 登录
    Login ({ commit }, { loginParams, isSaveStorage }) {
      return new Promise((resolve, reject) => {
        login(loginParams).then(bizData => {
          storage.setToken(bizData[appConfig.ACCESS_TOKEN_NAME], isSaveStorage)
          commit('SET_TOKEN', bizData[appConfig.ACCESS_TOKEN_NAME])
          resolve(bizData) // 返回登录响应结果
        }).catch(error => {
          reject(error)
        })
      })
    },

    QrCodeLogin ({ commit }, { loginParams, isSaveStorage }) {
      return new Promise((resolve, reject) => {
        qrCodeLogin(loginParams).then(bizData => {
          if (bizData.qrcodeStatus === 'confirmed') {
            storage.setToken(bizData[appConfig.ACCESS_TOKEN_NAME], isSaveStorage)
            commit('SET_TOKEN', bizData[appConfig.ACCESS_TOKEN_NAME])
          }
          resolve(bizData) // 返回登录响应结果
        }).catch(error => {
          reject(error)
        })
      })
    },

    // 登出
    Logout ({ commit, state }) {
      return new Promise((resolve) => {
        logout(state.token).then(() => {
          commit('SET_TOKEN', '')
          storage.cleanToken()
          location.reload() // 退出时 重置缓存
          resolve()
        }).catch(() => {
          resolve()
        }).finally(() => {

        })
      })
    }

  }
}

export default user

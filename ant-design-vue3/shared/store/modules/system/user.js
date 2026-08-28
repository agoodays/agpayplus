/*
 * 登录用户
 *
 * 使用 pinia-plugin-persistedstate 自动持久化
 *
 * 合并说明：三项目差异取并集
 *   belongInfoId — Agent 独有（所属主体ID），Manager/Merchant 场景忽略即可
 *   shortName    — Agent/Merchant 均有，注释中性化避免角色绑定
 */
import { defineStore } from 'pinia'

export const useUserStore = defineStore('userStore', {
  state: () => ({
    token: '',
    realname: '',
    safeWord: '',
    userId: '',
    avatarUrl: '',
    allMenuRouteTree: [],
    accessList: [],
    isAdmin: '',
    loginUsername: '',
    state: '',
    sysType: '',
    telphone: '',
    sex: '',
    belongInfoId: '',
    shortName: ''
  }),

  persist: {
    key: 'user-store',
    storage: localStorage,
    paths: [
      'token',
      'realname',
      'safeWord',
      'userId',
      'avatarUrl',
      'allMenuRouteTree',
      'accessList',
      'isAdmin',
      'loginUsername',
      'state',
      'sysType',
      'telphone',
      'sex',
      'belongInfoId',
      'shortName'
    ]
  },

  getters: {
    getToken(state) {
      return state.token || ''
    }
  },

  actions: {
    async logout() {
      this.$reset()
    },

    setUserLoginInfo(data) {
      this.userId = data.sysUserId
      this.realname = data.realname
      this.safeWord = data.safeWord
      this.avatarUrl = data.avatarUrl
      this.accessList = data.entIdList
      this.allMenuRouteTree = data.allMenuRouteTree
      this.isAdmin = data.isAdmin
      this.loginUsername = data.loginUsername
      this.state = data.state
      this.sysType = data.sysType
      this.telphone = data.telphone
      this.sex = data.sex
      this.belongInfoId = data.belongInfoId ?? ''
      this.shortName = data.shortName ?? ''
    },

    setToken(token, isAutoLogin) {
      this.token = token || ''
    },

    hasAccess(entId) {
      if (!entId) return true
      if (this.isAdmin) return true
      return this.accessList && this.accessList.includes(entId)
    }
  }
})

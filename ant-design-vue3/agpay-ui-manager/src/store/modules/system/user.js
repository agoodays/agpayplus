/*
 * 登录用户
 * 
 * 使用 pinia-plugin-persistedstate 自动持久化
 */
import { defineStore } from 'pinia'

export const useUserStore = defineStore('userStore', {
  state: () => ({
    token: '',
    userName: '', // 真实姓名
    safeWord: '', // 预留信息
    userId: '', // 用户ID
    avatarImgPath: '', // 头像
    allMenuRouteTree: [], // 全部动态 router
    accessList: [], // 用户权限集合
    isAdmin: '', // 是否是超级管理员
    loginUsername: '', // 登录用户名
    state: '', // 用户状态
    sysType: '', // 所属系统
    telphone: '', // 手机号
    sex: '' // 性别
  }),
  
  // 🔥 启用持久化 - 自动保存到 localStorage
  persist: {
    key: 'user-store',
    storage: localStorage,
    paths: ['token', 'userName', 'userId', 'avatarImgPath', 'allMenuRouteTree', 'accessList', 'isAdmin', 'loginUsername', 'state', 'sysType', 'telphone', 'sex']
  },
  
  getters: {
    // ✅ 简化：persist 会自动恢复 token，无需手动读取
    getToken(state) {
      return state.token || ''
    },
  },

  actions: {
    // ✅ 简化：使用 $reset() 重置状态，persist 会自动清除 localStorage
    logout() {
      return new Promise((resolve) => {
        // 重置所有状态到初始值
        this.$reset()
        resolve()
      })
    },
    
    // ✅ 简化：直接修改状态，persist 会自动保存
    setUserLoginInfo(data) {
      this.userId = data.sysUserId // 用户ID
      this.userName = data.realname // 真实姓名
      this.safeWord = data.safeWord // 预留信息
      this.avatarImgPath = data.avatarUrl // 头像
      this.accessList = data.entIdList // 权限集合
      this.allMenuRouteTree = data.allMenuRouteTree // 全部路由集合
      this.isAdmin = data.isAdmin // 是否是超级管理员
      this.loginUsername = data.loginUsername // 登录用户名
      this.state = data.state // 用户状态
      this.sysType = data.sysType // 所属系统
      this.telphone = data.telphone // 手机号
      this.sex = data.sex // 性别
    },
    
    // ✅ 简化：直接修改 token，persist 会自动保存
    setToken(token) {
      this.token = token || ''
    },
    // 检查用户是否具有指定权限
    hasAccess(entId) {
      if (!entId) return true

      // 超级管理员绕过权限检查
      if (this.isAdmin === true || this.isAdmin === '1' || this.isAdmin === 'true') {
        return true
      }

      const list = this.accessList || []

      // 支持数组和单个值的检查
      if (Array.isArray(entId)) {
        return entId.some((id) => list.includes(id))
      }

      return list.includes(entId)
    },
  },
})

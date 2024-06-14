/*
 * 登录用户
 *
 */
import { defineStore } from 'pinia';
import { localClear, localRead, localSave } from '/@/utils/local-util';
import LocalStorageKeyConst from '/@/constants/local-storage-key-const';

export const useUserStore = defineStore({
  id: 'userStore',
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
  getters: {
    getToken(state) {
      if (state.token) {
        return state.token;
      }
      return localRead(LocalStorageKeyConst.USER_TOKEN);
    },
  },

  actions: {
    logout() {
      this.token = '';
      this.allMenuRouteTree = [];
      localClear();
    },
    //设置登录信息
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
    setToken(token, isSaveLocal) {
      console.log(token)
      this.token = token;
      if (isSaveLocal) {
        localSave(LocalStorageKeyConst.USER_TOKEN, token ? token : '');
      }
    },
  },
});

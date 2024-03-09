import router from './router'
import store from './store'
import storage from '@/utils/agpayStorageWrapper'
import NProgress from 'nprogress' // progress bar
import '@/components/NProgress/nprogress.less' // progress bar custom style
import { setDocumentTitle } from '@/utils/domUtil'
import { getInfo } from '@/api/login'
import appConfig from '@/config/appConfig'

NProgress.configure({ showSpinner: false }) // NProgress Configuration

const allowList = ['login', 'forget', 'register', 'registerResult'] // no redirect allowList
const loginRoutePath = '/login'

// 封装跳转到指定路由的函数
function redirectToTargetRoute (path, next) {
  next(path === '/' ? redirectFunc() : undefined)
}

// 动态跳转路径 func
function redirectFunc () {
  let mainPageUri = ''
  store.state.user.allMenuRouteTree.forEach(item => {
    if (item.entId === 'ENT_C_MAIN') { // 当前用户是否拥有主页权限， 如果有直接跳转到该路径
      mainPageUri = item.menuUri
      return false
    }
  })

  if (mainPageUri) {
    return mainPageUri
  }

  return getOneUri(store.state.user.allMenuRouteTree)
}

// 获取到第一个uri (递归查找)
function getOneUri (item) {
  let result = ''
  for (let i = 0; i < item.length; i++) {
    if (item[i].menuUri && item[i].entType === 'ML') {
      return item[i].menuUri
    }

    if (item[i].children) {
      result = getOneUri(item[i].children)
      if (result) {
        return result
      }
    }
  }
  return result
}

// 路由守卫
router.beforeEach((to, from, next) => {
  NProgress.start() // start progress bar

  to.meta && (typeof to.meta.title !== 'undefined' && setDocumentTitle(`${to.meta.title} - ${appConfig.APP_TITLE}`)) // 设置浏览器标题

  // 如果在免登录页面则直接放行
  if (allowList.includes(to.name)) {
    // 在免登录名单，直接进入
    next()
    NProgress.done() // if current page is login will not trigger afterEach hook, so manually handle it
    return false
  }

  // 不包含Token 则直接跳转到登录页面
  if (!storage.getToken()) {
    next({ path: loginRoutePath, query: { redirect: to.fullPath } })
    NProgress.done() // if current page is login will not trigger afterEach hook, so manually handle it
    return false
  }

  // 以下为包含Token的情况
  // 如果用户信息不存在， 则重新获取 [用户登录成功 & 强制刷新浏览器时 会执行该函数]
  if (!store.state.user.userId) {
    // request login userInfo

    getInfo().then(bizData => {
      store.commit('SET_USER_INFO', bizData) // 调用vuex设置用户基本信息

      // 动态添加路由
      store.dispatch('GenerateRoutes', {}).then(() => {
        router.addRoutes(store.state.asyncRouter.addRouters)
      })

      // 判断是否存在路由参数 redirect
      if (to.query.redirect) {
        next(to.query.redirect) // 跳转到指定的路由
      } else {
        redirectToTargetRoute(to.path, next)
      }
    }).catch(() => {
      // 失败时，获取用户信息失败时，调用登出，来清空历史保留信息
      store.dispatch('Logout').then(() => {
        next({ path: loginRoutePath, query: { redirect: to.fullPath } })
      })
    })
  } else {
    redirectToTargetRoute(to.path, next)
  }
})

router.afterEach(() => {
  NProgress.done() // finish progress bar
})

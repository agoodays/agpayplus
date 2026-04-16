/**
 * 路由配置
 * 支持生产模式和开发模式（VITE_BYPASS_LOGIN）
 */
import nProgress from 'nprogress'
import 'nprogress/nprogress.css'
import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'
import UserLayout from '@/layouts/user-layout.vue'
import AgLayout from '@/layouts/index.vue'
import { useUserStore } from '@/store/modules/system/user'
import { setDocumentTitle } from '../utils/dom-util'
import { translateWithFallback } from '@/utils/i18n-util'
import { PAGE_PATH_404, PAGE_PATH_LOGIN } from '@/constants/common-const'
import { loginApi } from '@/api/system/login-api'
import { asyncRouteDefine } from '@/config/app-config'
import { devMenuTree, devUserInfo } from '@/config/dev-menu-config'

// ==================== 常量配置 ====================

// 使用 Vite 的 glob 导入预加载所有视图组件（必须使用相对路径）
const modules = import.meta.glob('../views/**/*.vue')

// 无需登录验证的路由白名单
const ALLOW_LIST = ['login', 'forget', 'register', 'registerResult']

// 开发模式日志开关
const DEV_LOG_ENABLED = import.meta.env.MODE === 'development'

// ==================== 工具函数 ====================

/**
 * 开发模式日志
 */
function devLog(emoji, message, ...args) {
  if (DEV_LOG_ENABLED) {
    console.log(`${emoji} ${message}`, ...args)
  }
}

/**
 * 从菜单树中查找第一个可用的路径
 */
function findFirstAvailableUri(menuTree) {
  if (!menuTree || !Array.isArray(menuTree)) return ''

  for (const item of menuTree) {
    // 优先查找主页
    if (item.entId === 'ENT_C_MAIN' && item.menuUri) {
      return item.menuUri
    }

    // 查找第一个菜单链接
    if (item.menuUri && item.entType === 'ML') {
      return item.menuUri
    }

    // 递归查找子菜单
    if (item.children) {
      const uri = findFirstAvailableUri(item.children)
      if (uri) return uri
    }
  }

  return ''
}

/**
 * 生成路由配置
 */
function generateRoutes(menuTree) {
  const routes = []

  function walk(nodes) {
    nodes.forEach((node) => {
      // 先处理子节点
      if (node.children) walk(node.children)
      
      // 跳过无路径的节点
      if (!node.menuUri) {
        return
      }

      const defComponent = asyncRouteDefine[node.componentName || node.entId]
      const path = node.menuUri || defComponent?.defaultPath

      if (!path) {
        devLog('⚠️', `跳过无路径的菜单: ${node.entName}`)
        return
      }

      let component = defComponent?.component
      if (!component && node.componentName) {
        const componentPath = `../views/${node.componentName}.vue`
        component = modules[componentPath]

        // 调试日志
        if (!component) {
          devLog('❌', `组件未找到: ${componentPath}`, {
            菜单名: node.entName,
            组件名: node.componentName,
            路径: path,
            可用组件: Object.keys(modules).filter((k) => k.includes('demo'))
          })
          component = modules['../views/exception/404.vue']
        } else {
          devLog('✅', `组件加载成功: ${node.entName} → ${componentPath}`)
        }
      }

      if (!component) {
        devLog('⚠️', `最终未找到组件: ${node.entName}`)
        return
      }

      routes.push({
        path,
        name: node.entId,
        component,
        meta: {
          title: node.entName,
          i18nKey: node.menuI18nKey || node.i18nKey,
          icon: node.menuIcon || node.icon,
          keepAlive: false,
          showInMenu: node.entType === 'ML' // 标记是否在左侧菜单显示
        }
      })
    })
  }

  walk(menuTree)
  return routes
}

// ==================== 路由状态 ====================

let routesInitialized = false

// ==================== 静态路由 ====================

const routes = [
  {
    path: '/',
    name: '用户',
    component: UserLayout,
    children: [
      { path: 'login', name: '登录', component: () => import('@/views/user/login.vue') },
      { path: 'forget', name: '注册', component: () => import('@/views/user/forget.vue') }
    ]
  },
  {
    path: '/exception',
    name: '异常',
    component: UserLayout,
    children: [
      { path: '403', name: '异常403', component: () => import('@/views/exception/403.vue') },
      { path: '404', name: '异常404', component: () => import('@/views/exception/404.vue') },
      { path: '500', name: '异常500', component: () => import('@/views/exception/500.vue') }
    ]
  }
]

// ==================== 创建路由实例 ====================

// 路由模式配置
const routerMode = import.meta.env.VITE_ROUTER_MODE || 'history'
const history = routerMode === 'hash' ? createWebHashHistory() : createWebHistory()

export const router = createRouter({
  history,
  routes,
  strict: true,
  scrollBehavior: () => ({ left: 0, top: 0 })
})

// ==================== 动态路由注册 ====================

/**
 * 注册动态路由
 */
function registerDynamicRoutes(menuTree) {
  if (routesInitialized) {
    devLog('⚠️', '路由已初始化')
    return
  }

  const pageRoutes = generateRoutes(menuTree)

  if (pageRoutes.length === 0) {
    devLog('⚠️', '没有可用的路由')
    return
  }

  devLog('🔨', `注册 ${pageRoutes.length} 个动态路由`)

  // 为每个页面创建带布局的路由
  pageRoutes.forEach((route) => {
    router.addRoute({
      path: route.path,
      component: AgLayout,
      children: [
        {
          path: '',
          name: route.name,
          component: route.component,
          meta: route.meta
        }
      ]
    })
  })

  routesInitialized = true
  devLog('✅', '路由注册完成')
}

// ==================== 开发模式支持 ====================

/**
 * 初始化开发模式
 */
function initDevMode() {
  const userStore = useUserStore()
  devLog('🚀', '开发模式初始化')

  userStore.setToken('dev-bypass-token')
  userStore.setUserLoginInfo({
    ...devUserInfo,
    allMenuRouteTree: devMenuTree,
    entIdList: ['ENT_DEMO', 'ENT_DEMO_INDEX', 'ENT_DEMO_SEARCH_TABLE', 'ENT_DEMO_REFACTOR_TABLE', 'ENT_DEMO_STATE_SWITCH', 'ENT_DEMO_FORM', 'ENT_DEMO_FLOAT_INPUT', 'ENT_DEMO_SELECT_INFINITE', 'ENT_DEMO_CARD', 'ENT_DEMO_UPLOAD', 'ENT_DEMO_EDITOR', 'ENT_DEMO_CONTAINER']
  })

  registerDynamicRoutes(devMenuTree)
}

/**
 * 检查并恢复开发模式状态
 */
function checkDevModeState() {
  const userStore = useUserStore()
  const hasUserInfo = userStore.userId
  const hasMenuData = userStore.allMenuRouteTree?.length > 0

  if (!hasUserInfo || !hasMenuData) {
    devLog('⚠️', '恢复用户信息')
    userStore.setUserLoginInfo({
      ...devUserInfo,
      allMenuRouteTree: devMenuTree,
      entIdList: ['ENT_DEMO', 'ENT_DEMO_INDEX', 'ENT_DEMO_SEARCH_TABLE', 'ENT_DEMO_REFACTOR_TABLE', 'ENT_DEMO_STATE_SWITCH', 'ENT_DEMO_FORM', 'ENT_DEMO_FLOAT_INPUT', 'ENT_DEMO_SELECT_INFINITE', 'ENT_DEMO_CARD', 'ENT_DEMO_UPLOAD', 'ENT_DEMO_EDITOR', 'ENT_DEMO_CONTAINER']
    })
  }

  if (!routesInitialized) {
    devLog('⚠️', '重新注册路由')
    const menuTree = userStore.allMenuRouteTree
    if (menuTree && menuTree.length > 0) {
      registerDynamicRoutes(menuTree)
      return true
    }
  }

  return false
}

// ==================== 路由守卫 ====================

router.beforeEach((to, from, next) => {
  nProgress.start()

  // 设置页面标题
  const routeTitle = to.meta?.i18nKey
    ? translateWithFallback(to.meta.i18nKey, to.meta?.title || to.name)
    : to.meta?.title || to.name
  const appTitle = translateWithFallback('app.title', import.meta.env.VITE_APP_TITLE)
  setDocumentTitle(routeTitle ? `${routeTitle} - ${appTitle}` : appTitle)

  // 公共页面直接放行
  if (to.path === PAGE_PATH_404 || ALLOW_LIST.includes(to.name)) {
    next()
    return
  }

  const userStore = useUserStore()
  const bypassLogin = import.meta.env.VITE_BYPASS_LOGIN === 'true'
  const token = userStore.getToken

  // ========== 开发模式处理 ==========
  if (bypassLogin) {
    // 初始化
    if (!token) {
      initDevMode()
      next({ ...to, replace: true })
      return
    }

    // 检查状态
    const needReNavigate = checkDevModeState()
    if (needReNavigate) {
      next({ ...to, replace: true })
      return
    }

    // 首页重定向（避免重定向到相同路径导致循环）
    if (to.path === '/' || to.path === '/main') {
      const firstUri = findFirstAvailableUri(userStore.allMenuRouteTree)
      if (firstUri && firstUri !== to.path) {
        next({ path: firstUri, replace: true })
        return
      }
    }

    next()
    return
  }

  // ========== 生产模式处理 ==========

  // 未登录
  if (!token) {
    userStore.logout()
    // 如果已经在登录页则直接放行，避免重定向循环
    if (to.path === PAGE_PATH_LOGIN) {
      next()
    } else {
      next({ path: PAGE_PATH_LOGIN, query: { redirect: to.fullPath } })
    }
    return
  }

  // 获取用户信息
  console.log('检查用户信息:', userStore.userId)
  if (!userStore.userId) {
    // 本地存储没有用户信息，调用API获取
    loginApi
      .getCurrentInfo()
      .then((bizData) => {
        userStore.setUserLoginInfo(bizData)
        registerDynamicRoutes(bizData.allMenuRouteTree)

        const redirectPath = to.query.redirect
        if (redirectPath && redirectPath !== to.path) {
          next(redirectPath)
        } else if (to.path === '/') {
          const firstUri = findFirstAvailableUri(bizData.allMenuRouteTree)
          if (firstUri && firstUri !== to.path) {
            next({ path: firstUri || PAGE_PATH_404 })
          } else {
            next()
          }
        } else {
          next({ ...to, replace: true })
        }
      })
      .catch((error) => {
        console.error('获取用户信息失败:', error)
        userStore.logout()
        next({ path: PAGE_PATH_LOGIN, query: { redirect: to.fullPath } })
      })
    return
  }

  // 确保路由已注册
  if (!routesInitialized && userStore.allMenuRouteTree && userStore.allMenuRouteTree.length > 0) {
    registerDynamicRoutes(userStore.allMenuRouteTree)
    next({ ...to, replace: true })
    return
  }

  // 首页重定向
  if (to.path === '/') {
    const firstUri = findFirstAvailableUri(userStore.allMenuRouteTree)
    if (firstUri && firstUri !== to.path) {
      next({ path: firstUri })
    } else {
      next()
    }
    return
  }

  next()
})

router.afterEach(() => {
  nProgress.done()
})

// ==================== 错误处理 ====================

router.onError((error) => {
  console.error('路由错误:', error)
  nProgress.done()
})

export default router

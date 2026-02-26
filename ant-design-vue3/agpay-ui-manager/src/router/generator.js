/**
 * 动态路由生成器
 * 根据后端返回的菜单数据动态生成路由
 */

import { h } from 'vue'
import { RouterView } from 'vue-router'

// 使用 Vite 的 glob 导入预加载所有视图组件
const modules = import.meta.glob('@/views/**/*.vue')

/**
 * 组件懒加载
 * @param {string} view 组件路径（相对于 views 目录）
 * @returns {Function} 组件加载函数
 */
const lazyLoad = (view) => {
  // 确保路径以 / 开头
  const path = view.startsWith('/') ? view : `/${view}`
  // 确保路径以 .vue 结尾
  const fullPath = path.endsWith('.vue') ? path : `${path}.vue`
  // 完整路径
  const componentPath = `@/views${fullPath}`
  
  // 从预加载的模块中查找
  const component = modules[componentPath]
  
  if (!component) {
    console.error(`Component not found: ${componentPath}`)
    console.log('Available components:', Object.keys(modules))
    // 返回 404 页面
    return modules['@/views/exception/404.vue']
  }
  
  return component
}

/**
 * 根据后端菜单数据生成路由
 * @param {Array} menuData 后端返回的菜单数据
 * @param {Boolean} isTopLevel 是否是顶层路由
 * @returns {Array} 路由配置数组
 */
export function generator(menuData, isTopLevel = true) {
  const routes = []

  for (const item of menuData) {
    // 跳过无效菜单项
    if (!item || item.state !== 1) {
      continue
    }

    const currentRoute = {
      path: item.menuUri || `/${item.entId}`,
      name: item.entId,
      meta: {
        title: item.entName,
        i18nKey: item.menuI18nKey || item.i18nKey,
        icon: item.menuIcon,
        entId: item.entId,
        entType: item.entType,
        quickJump: item.quickJump,
        matchRule: item.matchRule
      },
      hidden: item.entType === 'MO' // 当其他菜单时需要隐藏显示
    }

    // 处理组件
    if (item.componentName) {
      if (item.componentName === 'RouteView') {
        // 路由视图组件
        currentRoute.component = () => h(RouterView)
      } else {
        // 普通业务组件
        try {
          // 根据 componentName 转换为文件路径
          const componentPath = convertComponentName(item.componentName)
          currentRoute.component = lazyLoad(componentPath)
        } catch (error) {
          console.error(`Failed to load component: ${item.componentName}`, error)
          currentRoute.component = () => import('@/views/exception/404.vue')
        }
      }
    }

    // 递归处理子菜单
    if (item.children && item.children.length > 0) {
      currentRoute.children = generator(item.children, false)
      
      // 如果没有设置组件且有子路由，设置为 RouteView
      if (!currentRoute.component) {
        currentRoute.component = () => h(RouterView)
      }

      // 如果有默认跳转路由
      if (currentRoute.children.length > 0 && !item.menuUri) {
        currentRoute.redirect = currentRoute.children[0].path
      }
    }

    routes.push(currentRoute)
  }

  return routes
}

/**
 * 将组件名称转换为文件路径
 * 例如: MainPage -> main/MainPage
 *       UserList -> user/UserList
 */
function convertComponentName(componentName) {
  // 特殊组件映射
  const specialComponents = {
    MainPage: 'main/MainPage',
    CurrentUserInfo: 'current/UserInfo',
    RouteView: 'layouts/RouteView'
  }

  if (specialComponents[componentName]) {
    return specialComponents[componentName]
  }

  // 其他组件按照目录结构查找
  // 这里需要根据实际项目结构调整
  return componentName
}

/**
 * 扁平化路由树
 * @param {Array} routes 路由数组
 * @returns {Array} 扁平化后的路由数组
 */
export function flattenRoutes(routes) {
  const result = []

  function flatten(routeList, parent = '') {
    for (const route of routeList) {
      const currentPath = parent ? `${parent}/${route.path}` : route.path
      
      result.push({
        ...route,
        fullPath: currentPath
      })

      if (route.children && route.children.length > 0) {
        flatten(route.children, currentPath)
      }
    }
  }

  flatten(routes)
  return result
}

/**
 * 根据路由路径查找路由
 * @param {Array} routes 路由数组
 * @param {String} path 路由路径
 * @returns {Object|null} 路由对象
 */
export function findRoute(routes, path) {
  for (const route of routes) {
    if (route.path === path) {
      return route
    }

    if (route.children && route.children.length > 0) {
      const found = findRoute(route.children, path)
      if (found) {
        return found
      }
    }
  }

  return null
}

/**
 * 获取面包屑路径
 * @param {Array} routes 路由数组
 * @param {String} path 当前路由路径
 * @returns {Array} 面包屑数组
 */
export function getBreadcrumb(routes, path) {
  const breadcrumb = []

  function find(routeList, targetPath, parents = []) {
    for (const route of routeList) {
      const currentParents = [...parents, route]

      if (route.path === targetPath) {
        return currentParents
      }

      if (route.children && route.children.length > 0) {
        const found = find(route.children, targetPath, currentParents)
        if (found) {
          return found
        }
      }
    }

    return null
  }

  return find(routes, path) || []
}

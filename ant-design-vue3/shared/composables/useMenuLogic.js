/**
 * useMenuLogic - 菜单逻辑 composable
 *
 * 负责：
 * - 菜单树可见性过滤（仅保留 entType=ML 或含可见子节点的父级）
 * - 菜单树 → Ant Design Vue Menu items 转换
 * - 根据当前路径同步菜单选中/展开状态
 * - 查找菜单（按路径 / 按 ID / 按父级链路）
 * - openKeys 持久化（非折叠时缓存，折叠后恢复）
 *
 * 使用方式：
 *   const {
 *     visibleMenuTree, menuItems, selectedKeys, openKeys,
 *     menuClick, openChange, syncMenuFromRoute
 *   } = useMenuLogic({ route, userStore, appConfigStore, collapsed })
 */
import * as antIcons from '@ant-design/icons-vue'
import { computed, h, ref, watch } from 'vue'

/** 将短横线图标名转为 Ant Icon 组件（自动补 Outlined 后缀） */
function getIconComponent(iconName) {
  if (!iconName) return null
  const camel = iconName.replace(/-([a-z])/g, (_, c) => c.toUpperCase())
  let Pascal = camel.charAt(0).toUpperCase() + camel.slice(1)
  if (!Pascal.endsWith('Outlined')) Pascal += 'Outlined'
  return antIcons[Pascal] ?? null
}

/** i18n 菜单标题解析，翻译缺失时回退到中文名称 */
function resolveMenuTitle(menu, t) {
  const i18nKey = menu.menuI18nKey || menu.i18nKey
  const fallback = menu.entName || menu.name || ''
  if (!i18nKey || typeof t !== 'function') return fallback
  const translated = t(i18nKey)
  return translated === i18nKey ? fallback : translated
}

/** 面包屑/路由标题解析 */
export function resolveRouteTitle(routeItem, t) {
  const i18nKey = routeItem?.meta?.i18nKey
  const fallback = routeItem?.meta?.title || routeItem?.name || ''
  if (!i18nKey || typeof t !== 'function') return fallback
  const translated = t(i18nKey)
  return translated === i18nKey ? fallback : translated
}

/**
 * 过滤菜单树：
 * 1) 叶子节点 entType === visibleEntType 时保留
 * 2) 非 ML 但存在可见子节点时保留父级壳子
 */
function filterVisibleMenus(menus = [], visibleEntType = 'ML') {
  return menus
    .map((menu) => {
      const children = menu.children && menu.children.length > 0 ? filterVisibleMenus(menu.children, visibleEntType) : undefined
      const shouldKeep = menu.entType === visibleEntType || (children && children.length > 0)
      if (!shouldKeep) return null
      return { ...menu, children }
    })
    .filter(Boolean)
}

/** 递归转换为 Ant Design Vue Menu items */
function transformMenuToItems(menus = [], t) {
  return menus.map((menu) => {
    const children = menu.children && menu.children.length > 0 ? transformMenuToItems(menu.children, t) : undefined
    const iconComponent = getIconComponent(menu.menuIcon || menu.icon)
    return {
      key: String(menu.entId),
      label: resolveMenuTitle(menu, t),
      icon: iconComponent ? h(iconComponent) : undefined,
      children
    }
  })
}

/** 在菜单树中按 URI 查找节点 */
function findMenuByPath(menus, path) {
  for (const menu of menus) {
    if (menu.menuUri === path) return menu
    if (menu.children) {
      const found = findMenuByPath(menu.children, path)
      if (found) return found
    }
  }
  return null
}

/** 在菜单树中按 ID 查找节点 */
function findMenuById(menus, id) {
  const idStr = String(id)
  for (const menu of menus) {
    if (String(menu.entId) === idStr) return menu
    if (menu.children) {
      const found = findMenuById(menu.children, id)
      if (found) return found
    }
  }
  return null
}

/** 递归查找目标节点的父级 entId 链路 */
function findParentKeys(menus, targetKey, parentKeys = []) {
  for (const menu of menus) {
    if (menu.children) {
      const hasTarget = menu.children.some((child) => child.entId === targetKey)
      if (hasTarget) return [...parentKeys, menu.entId]
      const found = findParentKeys(menu.children, targetKey, [...parentKeys, menu.entId])
      if (found.length > 0) return found
    }
  }
  return []
}

// ==================== composable 主函数 ====================

export function useMenuLogic({ route, userStore, t, collapsed }) {
  const selectedKeys = ref([])
  const openKeys = ref([])
  const cachedOpenKeys = ref([])

  const menuData = computed(() => userStore.allMenuRouteTree || [])
  const visibleMenuTree = computed(() => filterVisibleMenus(menuData.value))
  const menuItems = computed(() => transformMenuToItems(visibleMenuTree.value, t))

  /** 根据当前路径同步 selectedKeys / openKeys */
  function syncMenuFromRoute(path) {
    const currentMenu = findMenuByPath(visibleMenuTree.value, path)
    if (!currentMenu) return
    selectedKeys.value = [String(currentMenu.entId)]
    const parentKeys = findParentKeys(visibleMenuTree.value, currentMenu.entId)
    openKeys.value = parentKeys.map((key) => String(key))
  }

  /** 折叠切换：折叠时缓存 openKeys；展开时恢复缓存 */
  watch(
    () => collapsed?.value,
    (isCollapsed) => {
      if (isCollapsed) {
        cachedOpenKeys.value = [...openKeys.value]
        openKeys.value = []
        return
      }
      if (cachedOpenKeys.value.length > 0) {
        openKeys.value = [...cachedOpenKeys.value]
      }
    }
  )

  /** 路由变化时同步选中/展开状态 */
  watch(
    () => route?.value?.path,
    (newPath) => {
      if (newPath) syncMenuFromRoute(newPath)
    }
  )

  /** 菜单点击：跳转到对应 URI */
  function menuClick({ key }, router) {
    const menu = findMenuById(visibleMenuTree.value, key)
    if (menu && menu.menuUri && router) {
      router.push(menu.menuUri)
    }
  }

  /** 子菜单展开变更：非折叠时持久化 openKeys */
  function openChange(keys) {
    openKeys.value = keys
    if (!collapsed?.value) {
      cachedOpenKeys.value = [...keys]
    }
  }

  return {
    // 数据
    visibleMenuTree,
    menuItems,
    selectedKeys,
    openKeys,
    cachedOpenKeys,
    // 操作
    syncMenuFromRoute,
    menuClick,
    openChange,
    // 工具函数（对外暴露便于其他地方复用）
    findMenuByPath: (path) => findMenuByPath(visibleMenuTree.value, path),
    findMenuById: (id) => findMenuById(visibleMenuTree.value, id),
    findParentKeys: (targetKey) => findParentKeys(visibleMenuTree.value, targetKey)
  }
}

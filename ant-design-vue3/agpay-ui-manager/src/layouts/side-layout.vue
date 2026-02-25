<template>
  <a-layout class="ag-layout">
    <!-- 左侧菜单 -->
    <a-layout-sider 
      class="ag-layout-side" 
      v-model:collapsed="collapsed" 
      :width="260" 
      :trigger="null" 
      :theme="menuTheme"
      collapsible
    >
      <div class="ag-side-logo">
        <!-- 当侧边栏卷起来的时候，切换仅有图标 -->
        <img src="/@/assets/logo.svg" alt="agooday" class="logo-icon">
        <!-- 在这里可以添加title，我们以图片的方式替代文字 -->
        <img v-show="!collapsed" src="/@/assets/agpay.svg" alt="agpay" class="logo-full">
      </div>
      
      <!-- 动态菜单 -->
      <a-menu 
        class="ag-side-menu" 
        :items="menuItems"
        v-model:openKeys="openKeys"
        v-model:selectedKeys="selectedKeys"
        :theme="menuTheme" 
        mode="inline"
        :inline-collapsed="collapsed"
        @openChange="handleOpenChange"
        @click="handleMenuClick"
      />
    </a-layout-sider>
    
    <a-layout class="ag-layout-main">
      <!-- 顶部 -->
      <a-layout-header class="ag-layout-header">
        <a-row class="ag-layout-header-main" justify="space-between">
          <a-col class="ag-layout-header-left">
            <!-- 菜单折叠按钮 -->
            <menu-unfold-outlined
              v-if="collapsed"
              class="trigger"
              @click="() => collapsed = !collapsed"
            />
            <menu-fold-outlined 
              v-else 
              class="trigger" 
              @click="() => collapsed = !collapsed" 
            />
            
            <!-- 刷新按钮 -->
            <reload-outlined 
              class="trigger" 
              @click="handleReload" 
              title="刷新页面"
            />
            
            <!-- 面包屑 -->
            <a-breadcrumb class="ag-breadcrumb" separator="/">
              <a-breadcrumb-item v-for="item in breadcrumbs" :key="item.path">
                <router-link v-if="item.path" :to="item.path" class="breadcrumb-link">
                  {{ item.meta?.title || item.name }}
                </router-link>
                <span v-else class="breadcrumb-text">{{ item.meta?.title || item.name }}</span>
              </a-breadcrumb-item>
            </a-breadcrumb>
          </a-col>
          
          <a-col class="ag-layout-header-right">            
            <a-dropdown>
              <div class="ag-layout-header-user">
                <a-avatar 
                  shape="square" 
                  size="small" 
                  class="ag-layout-header-user-avatar"
                  :src="userStore.avatarUrl"
                >
                  {{ userStore.realname?.charAt(0) || 'U' }}
                </a-avatar>
                <span class="user-name">{{ userStore.realname || userStore.loginUsername }}</span>
                <down-outlined />
              </div>
              <template #overlay>
                <a-menu>
                  <a-menu-item @click="handleUserCenter">
                    <user-outlined /> 个人中心
                  </a-menu-item>
                  <a-menu-item @click="handleSetting">
                    <setting-outlined /> 账户设置
                  </a-menu-item>
                  <a-menu-divider />
                  <a-menu-item @click="handleLogout">
                    <logout-outlined /> 退出登录
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </a-col>
        </a-row>
      </a-layout-header>
      
      <!-- 主内容区 -->
      <a-layout-content class="ag-layout-content">
        <router-view v-if="isRouterAlive" />
      </a-layout-content>
      
      <!-- footer 版权公司信息 -->
      <a-layout-footer class="ag-layout-footer">
        <div class="ag-version">
          <a target="_blank" class="ag-copyright" href="https://www.agpay.com">
            Copyright &copy;2023-{{ currentYear }} AgPay | 吉日科技
          </a>
        </div>
      </a-layout-footer>
      
      <!-- 回到顶部 -->
      <a-back-top :target="backTopTarget" :visibilityHeight="80" />
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, computed, getCurrentInstance, nextTick, watch, onMounted, onBeforeUnmount, h } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { 
  MenuUnfoldOutlined, 
  MenuFoldOutlined, 
  ReloadOutlined,
  DownOutlined,
  UserOutlined,
  SettingOutlined,
  LogoutOutlined
} from '@ant-design/icons-vue'
import * as antIcons from '@ant-design/icons-vue'
import dayjs from 'dayjs'
import { useUserStore } from '/@/store/modules/system/user'
import { useAppStore } from '/@/store/modules/system/app'

// ==================== 图标动态加载 ====================

/**
 * 动态获取图标组件
 * @param {string} iconName - 图标名称（如 'HomeOutlined'）
 * @returns {Component} 图标组件
 */
function getIconComponent(iconName) {
  if (!iconName) return null
  return antIcons[iconName]
}

// ==================== 基础设置 ====================

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const appStore = useAppStore()
const { proxy } = getCurrentInstance()

// ==================== 响应式状态 ====================

// 菜单选中的 key
const selectedKeys = ref([])
// 菜单展开的 key
const openKeys = ref([])
// 菜单收起前的展开 key
const cachedOpenKeys = ref([])
// 侧边栏折叠状态
const collapsed = ref(false)
// 是否由响应式尺寸自动收起
const collapsedByResponsive = ref(false)
// 路由刷新控制
const isRouterAlive = ref(true)
// 当前年份（用于版权信息）
const currentYear = dayjs().year()
// 响应式收起断点
const responsiveBreakpoint = 1200
// 仅展示左侧菜单类型：目录/菜单（ML）
const visibleEntType = 'ML'
let resizeRafId = 0

// ==================== 计算属性 ====================

// 获取菜单数据
const menuData = computed(() => {
  return userStore.allMenuRouteTree || []
})

// 先过滤可见菜单树，再转换为 Ant Menu items
const visibleMenuTree = computed(() => {
  return filterVisibleMenus(menuData.value)
})

const menuTheme = computed(() => {
  return appStore.themeConfig?.darkMode ? 'dark' : 'light'
})

const menuItems = computed(() => {
  return transformMenuToItems(visibleMenuTree.value)
})

// 生成面包屑
const breadcrumbs = computed(() => {
  const matched = route.matched.filter(item => item.meta && item.meta.title)
  return matched
})

// 回到顶部目标元素
const backTopTarget = () => {
  return document.querySelector('.ag-layout-content')
}

// 监听路由变化，更新选中的菜单
watch(
  () => route.path,
  (newPath) => {
    updateMenuKeys(newPath)
  }
)

watch(
  () => collapsed.value,
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

onMounted(() => {
  handleWindowResize()
  window.addEventListener('resize', onWindowResize)
  updateMenuKeys(route.path)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', onWindowResize)
  if (resizeRafId) {
    cancelAnimationFrame(resizeRafId)
    resizeRafId = 0
  }
})

function onWindowResize() {
  if (resizeRafId) {
    cancelAnimationFrame(resizeRafId)
  }
  resizeRafId = requestAnimationFrame(() => {
    handleWindowResize()
    resizeRafId = 0
  })
}

function handleWindowResize() {
  const shouldCollapse = window.innerWidth < responsiveBreakpoint

  if (shouldCollapse && !collapsed.value) {
    collapsed.value = true
    collapsedByResponsive.value = true
    return
  }

  if (!shouldCollapse && collapsedByResponsive.value) {
    collapsed.value = false
    collapsedByResponsive.value = false
  }
}

// 更新菜单选中状态
function updateMenuKeys(path) {
  // 从路由中获取当前菜单的 entId
  const currentMenu = findMenuByPath(visibleMenuTree.value, path)
  if (currentMenu) {
    selectedKeys.value = [String(currentMenu.entId)]

    // 展开父级菜单
    const parentKeys = findParentKeys(visibleMenuTree.value, currentMenu.entId)
    openKeys.value = parentKeys.map(key => String(key))
  }
}

// 根据路径查找菜单项
function findMenuByPath(menus, path) {
  for (const menu of menus) {
    if (menu.menuUri === path) {
      return menu
    }
    if (menu.children) {
      const found = findMenuByPath(menu.children, path)
      if (found) return found
    }
  }
  return null
}

/**
 * 过滤菜单树：
 * 1) 当前节点 entType=ML 时保留
 * 2) 非 ML 但存在可见子节点时保留父级壳子
 */
function filterVisibleMenus(menus = []) {
  return menus
    .map(menu => {
      const children = menu.children && menu.children.length > 0 ? filterVisibleMenus(menu.children) : undefined
      const shouldKeep = menu.entType === visibleEntType || (children && children.length > 0)

      if (!shouldKeep) {
        return null
      }

      return {
        ...menu,
        children
      }
    })
    .filter(Boolean)
}

/**
 * 转换为 Ant Design Vue Menu items
 */
function transformMenuToItems(menus = []) {
  return menus
    .map(menu => {
      const children = menu.children && menu.children.length > 0 ? transformMenuToItems(menu.children) : undefined

      const iconComponent = getIconComponent(menu.menuIcon || menu.icon)

      return {
        key: String(menu.entId),
        label: menu.entName,
        icon: iconComponent ? h(iconComponent) : undefined,
        children
      }
    })
}

// 查找父级菜单的 key
function findParentKeys(menus, targetKey, parentKeys = []) {
  for (const menu of menus) {
    if (menu.children) {
      const hasTarget = menu.children.some(child => child.entId === targetKey)
      if (hasTarget) {
        return [...parentKeys, menu.entId]
      }
      const found = findParentKeys(menu.children, targetKey, [...parentKeys, menu.entId])
      if (found.length > 0) return found
    }
  }
  return []
}

// 菜单点击事件
const handleMenuClick = ({ key }) => {
  const menu = findMenuById(visibleMenuTree.value, key)
  if (menu && menu.menuUri) {
    router.push(menu.menuUri)
  }
}

const handleOpenChange = (keys) => {
  openKeys.value = keys
  if (!collapsed.value) {
    cachedOpenKeys.value = [...keys]
  }
}

// 根据ID查找菜单
function findMenuById(menus, id) {
  for (const menu of menus) {
    if (String(menu.entId) === String(id)) {
      return menu
    }
    if (menu.children) {
      const found = findMenuById(menu.children, id)
      if (found) return found
    }
  }
  return null
}

// 刷新页面
const handleReload = () => {
  isRouterAlive.value = false
  nextTick(() => {
    isRouterAlive.value = true
  })
}

// 个人中心
const handleUserCenter = () => {
  router.push({ path: '/current/userinfoPage' })
}

// 账户设置
const handleSetting = () => {
  router.push({ path: '/current/modifyPwd' })
}

// 退出登录
const handleLogout = () => {
  proxy.$infoBox.confirmPrimary(
    '确认退出',
    `你好 ${userStore.realname || userStore.loginUsername}，确认退出登录吗？`,
    async () => {
      await userStore.logout()
      router.push({ name: 'login' })
    }
  )
}
</script>

<style lang="less" scoped>
:global([data-theme='dark']) .ag-layout .ag-layout-side {
  background: var(--sider-bg-dark);
}

:global([data-theme='light']) .ag-layout .ag-layout-side {
  background: var(--sider-bg-light);
}

.ag-layout {
  height: 100vh;
  
  .ag-layout-side {
    overflow: auto;
    height: 100vh;
    position: fixed;
    left: 0;
    top: 0;
    bottom: 0;
    background: inherit;
    
    .ag-side-logo {
      height: 32px;
      margin: 16px;
      display: flex;
      align-items: center;
      background: inherit;
      
      .logo-icon { 
        width: 32px; 
        height: 32px; 
        margin-left: 8px; 
      }
      .logo-full { 
        width: 90px; 
        height: 32px; 
        margin: 5px 0 0 10px;
      }
    }
    
    .ag-side-menu {
      border-right: 0;
      background: inherit;
      
      // 菜单项图标样式
      :deep(.ant-menu-item) {
        display: flex;
        align-items: center;
        
        .anticon {
          font-size: 16px;
        }
      }
      
      // 子菜单图标样式
      :deep(.ant-menu-submenu-title) {
        display: flex;
        align-items: center;
        
        .anticon {
          font-size: 16px;
        }
      }
      
      // 折叠状态下的样式
      &:deep(.ant-menu-inline-collapsed) {
        .ant-menu-item {
          padding: 0 calc(50% - 16px / 2);
          
          .anticon {
            font-size: 18px;
            line-height: 40px;
          }
        }
      }
    }
  }
  
  .ag-layout-main {
    margin-left: 260px;
    transition: margin-left 0.2s;
    
    .ag-layout-header {
      background: var(--layout-bg);
      padding: 0;
      position: sticky;
      top: 0;
      z-index: 1;
      box-shadow: 0 1px 4px var(--shadow-color);
      
      .ag-layout-header-main {
        height: 64px;
        padding: 0 24px;
        
        .ag-layout-header-left {
          display: flex;
          align-items: center;
          
          .trigger {
            font-size: 18px;
            line-height: 64px;
            padding-right: 16px;
            cursor: pointer;
            transition: color 0.3s;
            
            &:hover {
              color: var(--primary-color);
            }
          }
          
          .ag-breadcrumb {
            line-height: 64px;
            
            :deep(.ant-breadcrumb-link) {
              .breadcrumb-link {
                color: var(--text-color);
                transition: color 0.3s;
                
                &:hover {
                  color: var(--primary-color);
                  background: none;
                }
              }
            }
            
            :deep(.ant-breadcrumb-separator) {
              color: var(--text-color-muted);
            }
            
            .breadcrumb-text {
              color: var(--text-color);
            }
          }
        }
        
        .ag-layout-header-right {
          .ag-layout-header-user {
            display: flex;
            align-items: center;
            cursor: pointer;
            height: 64px;
            padding: 0 12px;
            transition: background-color 0.3s;
            
            &:hover {
              background-color: var(--surface-subtle);
            }
            
            .ag-layout-header-user-avatar {
              margin-right: 8px;
            }
            
            .user-name {
              padding: 0 8px;
              color: var(--text-color);
            }
          }
        }
      }
    }
    
    .ag-layout-content {
      // margin: 24px 16px;
      padding: 24px;
      min-height: calc(100vh - 64px - 69px - 48px);
      background: var(--layout-bg);
      overflow: auto;
    }
    
    .ag-layout-footer {
      position: relative;
      padding: 16px 0;
      text-align: center;
      background: var(--layout-bg);
      
      .ag-version {
        font-size: 14px;
        color: var(--text-color-muted);
        
        a {
          color: var(--text-color-muted);
          
          &:hover {
            color: var(--primary-color);
          }
        }
      }
    }
  }
}

// 侧边栏收起时的样式调整
.ag-layout :deep(.ag-layout-side.ant-layout-sider-collapsed) {
  ~ .ag-layout-main {
    margin-left: 80px;
  }
}
</style>

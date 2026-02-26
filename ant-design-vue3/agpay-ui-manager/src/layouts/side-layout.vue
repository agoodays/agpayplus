<template>
  <a-layout class="ag-layout">
    <a-layout-sider
      class="ag-layout-side"
      v-model:collapsed="collapsed"
      :width="260"
      :trigger="null"
      :theme="menuTheme"
      collapsible
    >
      <div class="ag-side-logo">
        <img src="@/assets/logo.svg" alt="agooday" class="logo-icon">
        <img v-show="!collapsed" src="@/assets/agpay.svg" alt="agpay" class="logo-full">
      </div>

      <a-menu
        class="ag-side-menu"
        :items="menuItems"
        v-model:openKeys="openKeys"
        v-model:selectedKeys="selectedKeys"
        :theme="menuTheme"
        mode="inline"
        @openChange="handleOpenChange"
        @click="handleMenuClick"
      />
    </a-layout-sider>

    <a-layout class="ag-layout-main">
      <a-layout-header class="ag-layout-header">
        <a-row class="ag-layout-header-main" justify="space-between">
          <a-col class="ag-layout-header-left">
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

            <reload-outlined
              class="trigger"
              @click="handleReload"
              :title="t('layout.refreshPage')"
            />

            <a-breadcrumb class="ag-breadcrumb" separator="/">
              <a-breadcrumb-item v-for="item in breadcrumbs" :key="item.path">
                <router-link v-if="item.path" :to="item.path" class="breadcrumb-link">
                  {{ resolveRouteTitle(item) }}
                </router-link>
                <span v-else class="breadcrumb-text">{{ resolveRouteTitle(item) }}</span>
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
                    <user-outlined /> {{ t('layout.userCenter') }}
                  </a-menu-item>
                  <a-menu-item @click="handleSetting">
                    <setting-outlined /> {{ t('layout.accountSetting') }}
                  </a-menu-item>
                  <a-sub-menu key="language-menu">
                    <template #title>
                      {{ t('layout.language') }}
                    </template>
                    <a-menu-item @click="handleLanguageChange('zh_CN')">
                      {{ t('layout.languageZhCN') }}
                    </a-menu-item>
                    <a-menu-item @click="handleLanguageChange('en_US')">
                      {{ t('layout.languageEnUS') }}
                    </a-menu-item>
                  </a-sub-menu>
                  <a-menu-divider />
                  <a-menu-item @click="handleLogout">
                    <logout-outlined /> {{ t('layout.logout') }}
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </a-col>
        </a-row>
      </a-layout-header>

      <a-layout-content class="ag-layout-content">
        <router-view v-if="isRouterAlive" />
      </a-layout-content>

      <a-layout-footer class="ag-layout-footer">
        <div class="ag-version">
          <a target="_blank" class="ag-copyright" href="https://www.agpay.com">
            {{ t('layout.footerCopyright', { year: currentYear }) }}
          </a>
        </div>
      </a-layout-footer>

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
import { useUserStore } from '@/store/modules/system/user'
import { useAppStore } from '@/store/modules/system/app'
import { useAppConfigStore } from '@/store/modules/system/app-config'
import { useI18n } from 'vue-i18n'

function getIconComponent(iconName) {
  if (!iconName) return null
  return antIcons[iconName]
}

function resolveMenuTitle(menu) {
  const i18nKey = menu.menuI18nKey || menu.i18nKey
  const fallbackTitle = menu.entName || menu.name || ''
  if (!i18nKey) {
    return fallbackTitle
  }

  const translated = t(i18nKey)
  return translated === i18nKey ? fallbackTitle : translated
}

function resolveRouteTitle(routeItem) {
  const i18nKey = routeItem?.meta?.i18nKey
  const fallbackTitle = routeItem?.meta?.title || routeItem?.name || ''
  if (!i18nKey) {
    return fallbackTitle
  }

  const translated = t(i18nKey)
  return translated === i18nKey ? fallbackTitle : translated
}

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const appStore = useAppStore()
const appConfigStore = useAppConfigStore()
const { t } = useI18n()
const { proxy } = getCurrentInstance()

const selectedKeys = ref([])
const openKeys = ref([])
const cachedOpenKeys = ref([])
const collapsed = ref(false)
const collapsedByResponsive = ref(false)
const isRouterAlive = ref(true)
const currentYear = dayjs().year()
const responsiveBreakpoint = 1200
const visibleEntType = 'ML'
let resizeRafId = 0

const menuData = computed(() => userStore.allMenuRouteTree || [])
const visibleMenuTree = computed(() => filterVisibleMenus(menuData.value))
const menuTheme = computed(() => (appStore.themeConfig?.darkMode ? 'dark' : 'light'))
const menuItems = computed(() => transformMenuToItems(visibleMenuTree.value))
const breadcrumbs = computed(() => route.matched.filter(item => item.meta && item.meta.title))
const backTopTarget = () => document.querySelector('.ag-layout-content')

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

function updateMenuKeys(path) {
  const currentMenu = findMenuByPath(visibleMenuTree.value, path)
  if (currentMenu) {
    selectedKeys.value = [String(currentMenu.entId)]
    const parentKeys = findParentKeys(visibleMenuTree.value, currentMenu.entId)
    openKeys.value = parentKeys.map(key => String(key))
  }
}

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
        label: resolveMenuTitle(menu),
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

// 语言切换
const handleLanguageChange = (language) => {
  appConfigStore.setLanguage(language)
}

// 退出登录
const handleLogout = () => {
  const username = userStore.realname || userStore.loginUsername
  proxy.$infoBox.confirmPrimary(
    t('layout.confirmLogoutTitle'),
    t('layout.confirmLogoutContent', { name: username }),
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

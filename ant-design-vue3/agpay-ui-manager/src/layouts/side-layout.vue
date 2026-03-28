<template>
  <a-layout class="ag-layout">
    <a-layout-sider
      v-model:collapsed="collapsed"
      class="ag-layout-side"
      :width="sideMenuWidth"
      :collapsed-width="80"
      :trigger="null"
      :theme="menuTheme"
      collapsible
    >
      <div class="ag-side-logo">
        <img src="@/assets/logo.svg" alt="agooday" class="logo-icon" />
        <img v-show="!collapsed" src="@/assets/agpay.svg" alt="agpay" class="logo-full" />
      </div>

      <a-menu
        v-model:open-keys="openKeys"
        v-model:selected-keys="selectedKeys"
        class="ag-side-menu"
        :items="menuItems"
        :theme="menuTheme"
        mode="inline"
        @open-change="handleOpenChange"
        @click="handleMenuClick"
      />
    </a-layout-sider>

    <a-layout class="ag-layout-main">
      <a-layout-header class="ag-layout-header">
        <div class="ag-layout-header-main">
          <div class="ag-layout-header-left">
            <div class="ag-layout-header-controls">
              <menu-unfold-outlined v-if="collapsed" class="trigger" @click="() => (collapsed = !collapsed)" />
              <menu-fold-outlined v-else class="trigger" @click="() => (collapsed = !collapsed)" />
              <reload-outlined class="trigger" :title="t('layout.refreshPage')" @click="handleReload" />
            </div>
            <div v-if="breadCrumbFlag" class="ag-layout-header-breadcrumb">
              <a-breadcrumb class="ag-breadcrumb" separator="/">
                <a-breadcrumb-item v-for="item in breadcrumbs" :key="item.path">
                  <router-link v-if="item.path" :to="item.path" class="breadcrumb-link">
                    {{ resolveRouteTitle(item) }}
                  </router-link>
                  <span v-else class="breadcrumb-text">{{ resolveRouteTitle(item) }}</span>
                </a-breadcrumb-item>
              </a-breadcrumb>
            </div>
          </div>
          <div class="ag-layout-header-right">
            <a-dropdown>
              <div class="ag-layout-header-user">
                <a-avatar shape="square" size="small" class="ag-layout-header-user-avatar" :src="userStore.avatarUrl">
                  {{ userStore.realname?.charAt(0) || 'U' }}
                </a-avatar>
                <span class="user-name">{{ userStore.realname || userStore.loginUsername }}</span>
                <down-outlined />
              </div>
              <template #overlay>
                <a-menu>
                  <a-menu-item @click="handleUserCenter"> <user-outlined /> {{ t('layout.userCenter') }} </a-menu-item>
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
                  <a-menu-item @click="handleLogout"> <logout-outlined /> {{ t('layout.logout') }} </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </div>
        </div>
      </a-layout-header>

      <a-layout-content class="ag-layout-content">
        <router-view />
      </a-layout-content>

      <a-layout-footer v-if="footerFlag" class="ag-layout-footer">
        <div class="ag-version">
          <a target="_blank" class="ag-copyright" href="https://www.agpay.com">
            {{ t('layout.footerCopyright', { year: currentYear }) }}
          </a>
        </div>
      </a-layout-footer>

      <a-back-top :target="backTopTarget" :visibility-height="80" />
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, computed, getCurrentInstance, watch, onMounted, onBeforeUnmount, h } from 'vue'
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
import { appDefaultConfig } from '@/config/app-config'

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
const currentYear = dayjs().year()
const responsiveBreakpoint = 1200
const visibleEntType = 'ML'
let resizeRafId = 0

// 侧边菜单宽度
const sideMenuWidth = computed(() => appDefaultConfig.sideMenuWidth)

// 配置标志
const breadCrumbFlag = computed(() => appConfigStore.breadCrumbFlag)
const footerFlag = computed(() => appConfigStore.footerFlag)

const menuData = computed(() => {
  const data = userStore.allMenuRouteTree || []
  console.log('菜单数据:', data)
  return data
})
const visibleMenuTree = computed(() => {
  const data = filterVisibleMenus(menuData.value)
  console.log('可见菜单树:', data)
  return data
})
const menuTheme = computed(() => (appStore.themeConfig?.darkMode ? 'dark' : 'light'))
const menuItems = computed(() => {
  const items = transformMenuToItems(visibleMenuTree.value)
  console.log('菜单Items:', items)
  return items
})
const breadcrumbs = computed(() => route.matched.filter((item) => item.meta && item.meta.title))
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
  console.log('SideLayout 组件初始化')
  console.log('当前路由:', route.path)
  console.log('用户状态:', userStore)
  console.log('菜单数据:', userStore.allMenuRouteTree)
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
    openKeys.value = parentKeys.map((key) => String(key))
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
    .map((menu) => {
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
  return menus.map((menu) => {
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
      const hasTarget = menu.children.some((child) => child.entId === targetKey)
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
  window.location.reload()
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
  background: var(--layout-bg);
}

:global([data-theme='light']) .ag-layout .ag-layout-side {
  background: var(--layout-bg);
}

/* 非暗色模式下的菜单样式 */
:global([data-theme='light']) .ag-layout .ag-layout-side .ag-side-menu {
  :deep(.ant-menu-item-selected) {
    background-color: var(--primary-color) !important;
    color: #ffffff !important;

    .ant-menu-title-content {
      color: #ffffff !important;
    }

    .anticon {
      color: #ffffff !important;
    }
  }
}

/* 暗色模式下的菜单样式 */
:global([data-theme='dark']) .ag-layout .ag-layout-side .ag-side-menu {
  :deep(.ant-menu-item-selected) {
    background-color: var(--primary-color) !important;
    color: #ffffff !important;

    .ant-menu-title-content {
      color: #ffffff !important;
    }

    .anticon {
      color: #ffffff !important;
    }
  }
}

.ag-layout {
  height: 100vh;
  display: flex;
  overflow: hidden;

  .ag-layout-side {
    overflow: auto;
    height: 100vh;
    width: var(--side-menu-width);
    position: fixed;
    left: 0;
    top: 0;
    bottom: 0;
    z-index: 10;
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

      // 覆盖子菜单背景色
      :deep(.ant-menu-sub.ant-menu-inline) {
        background: var(--layout-bg);
      }

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

      // 选中菜单项样式
      :deep(.ant-menu-item-selected) {
        background-color: var(--primary-color) !important;

        > a,
        > a:hover {
          color: #ffffff !important;
        }

        .anticon {
          color: #ffffff !important;
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

        .ant-menu-item-selected {
          .anticon {
            color: #ffffff !important;
          }
        }
      }
    }
  }

  .ag-layout-main {
    flex: 1;
    margin-left: var(--side-menu-width);
    transition: margin-left 0.2s;
    height: 100vh;
    overflow: hidden;
    display: flex;
    flex-direction: column;

    .ag-layout-header {
      background: var(--layout-bg);
      padding: 0;
      position: sticky;
      top: 0;
      z-index: 5;

      .ag-layout-header-main {
        display: flex;
        justify-content: space-between;
        align-items: center;
        height: 64px;

        .ag-layout-header-left {
          display: flex;
          align-items: center;
          flex: 1;

          .ag-layout-header-controls {
            display: flex;
            align-items: center;
            margin-right: 24px;

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
          }

          .ag-layout-header-breadcrumb {
            flex: 1;

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
        }

        .ag-layout-header-right {
          display: flex;
          justify-content: flex-end;
          align-items: center;

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
      flex: 1;
      // margin: 24px 16px;
      // padding: 24px;
      overflow: auto;
      // max-width: var(--page-width);
      // margin-left: auto;
      // margin-right: auto;
    }

    .ag-layout-footer {
      background: var(--layout-bg);
      position: relative;
      padding: 16px 0;
      text-align: center;

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

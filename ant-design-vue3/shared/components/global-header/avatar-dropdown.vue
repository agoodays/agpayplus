<template>
  <a-dropdown placement="bottomRight">
    <span class="ant-pro-account-avatar">
      <a-avatar size="small" :src="greetImg" class="antd-pro-global-header-index-avatar">
        <template v-if="!greetImg">
          {{ userInitial }}
        </template>
      </a-avatar>
      <span>{{ currentUserName }}</span>
    </span>
    <template #overlay>
      <a-menu class="ant-pro-drop-down menu" :selected-keys="[]">
        <!-- 个人中心 -->
        <a-menu-item key="profile" @click="handleToProfile">
          <user-outlined />
          {{ t('layout.userCenter') }}
        </a-menu-item>

        <!-- 账户设置 -->
        <a-menu-item v-if="hasPermission('ENT_C_USERINFO')" key="settings" @click="handleToSettings">
          <setting-outlined />
          {{ t('layout.accountSetting') }}
        </a-menu-item>

        <a-menu-divider />

        <!-- 语言切换 -->
        <a-sub-menu key="language">
          <template #title>
            <global-outlined />
            {{ t('layout.language') }}
          </template>
          <a-menu-item key="zh-CN" @click="handleLanguageChange('zh-CN')">
            {{ t('layout.languageZhCN') }}
          </a-menu-item>
          <a-menu-item key="en-US" @click="handleLanguageChange('en-US')">
            {{ t('layout.languageEnUS') }}
          </a-menu-item>
        </a-sub-menu>

        <a-menu-divider />

        <!-- 退出登录 -->
        <a-menu-item key="logout" @click="handleLogout">
          <logout-outlined />
          {{ t('layout.logout') }}
        </a-menu-item>
      </a-menu>
    </template>
  </a-dropdown>
</template>

<script setup>
/**
 * 全局头部 - 用户头像下拉菜单组件
 * 功能：展示当前用户信息，提供个人中心、账户设置、语言切换和退出登录入口
 */
import { usePermission } from '@/composables/useCommon'
import { setAppLocale } from '@/i18n'
import { useAppConfigStore } from '@/store/modules/system/app-config'
import { useUserStore } from '@/store/modules/system/user'
import { infoBox } from '@/utils/info-box'
import { GlobalOutlined, LogoutOutlined, SettingOutlined, UserOutlined } from '@ant-design/icons-vue'
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'

const userStore = useUserStore()
const appConfigStore = useAppConfigStore()
const router = useRouter()
const route = useRoute()
const { hasPermission } = usePermission()
const { t, i18n } = useI18n()

/** 当前用户名称 */
const currentUserName = computed(() => userStore.realname || userStore.loginUsername || '')

/** 用户头像地址 */
const greetImg = computed(() => userStore.avatarUrl || '')

/** 用户姓名首字母（作为头像 fallback） */
const userInitial = computed(() => {
  const name = currentUserName.value
  return name ? name.charAt(0).toUpperCase() : 'U'
})

/**
 * 跳转到个人中心
 */
function handleToProfile() {
  router.push({ name: 'ENT_C_USERINFO' })
}

/**
 * 跳转到账户设置页面
 */
function handleToSettings() {
  router.push({ name: 'ENT_C_USERINFO' })
}

/**
 * 切换语言
 */
function handleLanguageChange(language) {
  appConfigStore.setLanguage(language)
  setAppLocale(i18n, language)
}

/**
 * 处理退出登录
 */
function handleLogout() {
  infoBox.confirmPrimary(
    t('layout.confirmLogoutTitle'),
    t('layout.confirmLogoutContent', { name: currentUserName.value }),
    async () => {
      await userStore.logout()
      router.push({ name: 'login' })
    }
  )
}
</script>

<style lang="less" scoped>
// .ant-pro-account-avatar {
//   display: flex;
//   align-items: center;
//   gap: 8px;
//   cursor: pointer;
//   padding: 4px 8px;
//   border-radius: 4px;
//   transition: background-color 0.3s;

//   &:hover {
//     background-color: rgba(0, 0, 0, 0.04);
//   }
// }

// .ant-pro-drop-down {
//   :deep(.action) {
//     margin-right: 8px;
//   }
//   :deep(.ant-dropdown-menu-item) {
//     min-width: 160px;
//   }
//   :deep(.ant-dropdown-menu-submenu-title) {
//     min-width: 160px;
//   }
// }
</style>

<template>
  <a-dropdown placement="bottomRight">
    <span class="ant-pro-account-avatar">
      <a-avatar size="small" :src="greetImg" class="antd-pro-global-header-index-avatar" />
      <span>{{ currentUserName }}</span>
    </span>
    <template #overlay>
      <a-menu class="ant-pro-drop-down menu" :selected-keys="[]">
        <a-menu-item v-if="hasPermission('ENT_C_USERINFO')" key="settings" @click="handleToSettings">
          <setting-outlined />
          账户设置
        </a-menu-item>

        <a-menu-divider />

        <a-menu-item key="logout" @click="handleLogout">
          <logout-outlined />
          退出登录
        </a-menu-item>
      </a-menu>
    </template>
  </a-dropdown>
</template>

<script setup>
/**
 * 全局头部 - 用户头像下拉菜单组件
 * 功能：展示当前用户信息，提供账户设置和退出登录入口
 */
import { LogoutOutlined, SettingOutlined } from '@ant-design/icons-vue'
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { infoBox } from '@/utils/info-box'
import { usePermission } from '@/composables/useCommon'
import { useUserStore } from '@/store/modules/system/user'

const userStore = useUserStore()
const router = useRouter()
const { hasPermission } = usePermission()

/** 当前用户名称 */
const currentUserName = computed(() => userStore.userInfo?.userName || '')

/** 用户头像地址 */
const greetImg = computed(() => userStore.userInfo?.avatarImgPath || '')

/**
 * 跳转到账户设置页面
 */
function handleToSettings() {
  router.push({ name: 'ENT_C_USERINFO' })
}

/**
 * 处理退出登录
 */
function handleLogout() {
  infoBox.confirmPrimary('是否退出登录？', `你好${currentUserName.value}，确认退出登录吗？`, async () => {
    await userStore.logout()
    router.push({ name: 'login' })
  })
}
</script>

<style lang="less" scoped>
.ant-pro-drop-down {
  :deep(.action) {
    margin-right: 8px;
  }
  :deep(.ant-dropdown-menu-item) {
    min-width: 160px;
  }
}
</style>

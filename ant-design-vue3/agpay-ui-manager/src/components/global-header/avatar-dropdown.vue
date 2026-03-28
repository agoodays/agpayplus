<template>
  <a-dropdown placement="bottomRight">
    <span class="ant-pro-account-avatar">
      <a-avatar size="small" :src="greetImg" class="antd-pro-global-header-index-avatar" />
      <span>{{ currentUserName }}</span>
    </span>
    <template #overlay>
      <a-menu class="ant-pro-drop-down menu" :selected-keys="[]">
        <a-menu-item v-if="hasPermission('ENT_C_USERINFO')" key="settings" @click="handleToSettings">
          <a-icon type="setting" />
          账户设置
        </a-menu-item>

        <a-menu-divider />

        <a-menu-item key="logout" @click="handleLogout">
          <a-icon type="logout" />
          退出登录
        </a-menu-item>
      </a-menu>
    </template>
  </a-dropdown>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'pinia'
import { infoBox } from '@/utils/info-box'
import { usePermission } from '@/hooks/common-hooks'

const store = useStore()
const router = useRouter()
const { hasPermission } = usePermission()

const currentUserName = computed(() => {
  return store.state.user.userName
})

const greetImg = computed(() => {
  return store.state.user.avatarImgPath
})

const handleToSettings = () => {
  router.push({ name: 'ENT_C_USERINFO' })
}

const handleLogout = () => {
  infoBox.confirmPrimary('是否退出登录？', `你好${currentUserName.value}确认退出登录吗？`, () => {
    store.dispatch('Logout').then(() => {
      router.push({ name: 'login' })
    })
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

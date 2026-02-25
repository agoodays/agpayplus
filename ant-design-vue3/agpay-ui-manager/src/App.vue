<template>
  <a-config-provider
    :locale="locale"
    :theme="{
      token: {
        colorPrimary: appStore.themeConfig.primaryColor,
      },
    }"
  >
    <!-- 全局 Loading -->
    <a-spin :spinning="spinning" size="large">
      <!-- 路由视图 -->
      <RouterView />
    </a-spin>
  </a-config-provider>
</template>

<script setup>
import { computed } from 'vue'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import { useSpinStore } from '/@/store/modules/system/spin'
import { useAppStore } from '/@/store/modules/system/app'

// ==================== 状态管理 ====================

// 全局 Loading 状态
const spinStore = useSpinStore()
const spinning = computed(() => spinStore.loading)

// 应用配置（主题颜色等）
const appStore = useAppStore()

// Ant Design 组件中文配置（DatePicker 等依赖）
const locale = zhCN

// ==================== 开发调试 ====================

// 开发环境输出主题色（生产环境自动移除）
if (import.meta.env.DEV) {
  console.log('🎨 当前主题色:', appStore.themeConfig.primaryColor)
}
</script>

<style scoped>
/* App.vue 根组件样式 */
</style>


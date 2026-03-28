<template>
  <a-config-provider :locale="locale" :theme="antdThemeConfig">
    <!-- 全局 Loading -->
    <a-spin :spinning="spinning" size="large">
      <!-- 路由视图 -->
      <RouterView />
    </a-spin>
  </a-config-provider>
</template>

<script setup>
import { computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { theme as antdTheme, ConfigProvider, Spin } from 'ant-design-vue'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import enUS from 'ant-design-vue/es/locale/en_US'
import { useSpinStore } from '@/store/modules/system/spin'
import { useAppStore } from '@/store/modules/system/app'
import { useAppConfigStore } from '@/store/modules/system/app-config'
import { i18n, setAppLocale } from '@/i18n'
import { setDocumentTitle } from '@/utils/dom-util'
import { translateWithFallback } from '@/utils/i18n-util'

// ==================== 状态管理 ====================

// 全局 Loading 状态
const spinStore = useSpinStore()
const spinning = computed(() => spinStore.loading)

// 应用配置（主题颜色等）
const appStore = useAppStore()
const appConfigStore = useAppConfigStore()
const route = useRoute()

// Ant Design 组件语言配置（DatePicker 等依赖）
const locale = computed(() => (appConfigStore.language === 'en_US' ? enUS : zhCN))
const antdThemeConfig = computed(() => ({
  algorithm: appStore.themeConfig.darkMode ? antdTheme.darkAlgorithm : antdTheme.defaultAlgorithm,
  token: {
    colorPrimary: appStore.themeConfig.primaryColor
  }
}))

// 监听语言变化，联动 i18n + dayjs
watch(
  () => appConfigStore.language,
  (language) => {
    setAppLocale(i18n, language)

    const appTitle = translateWithFallback('app.title', import.meta.env.VITE_APP_TITLE)
    const pageTitle = route.meta?.i18nKey
      ? translateWithFallback(route.meta.i18nKey, route.meta?.title)
      : route.meta?.title
    setDocumentTitle(pageTitle ? `${pageTitle} - ${appTitle}` : appTitle)
  },
  { immediate: true }
)

// ==================== 开发调试 ====================

// 开发环境输出主题色（生产环境自动移除）
if (import.meta.env.DEV) {
  console.log('🎨 当前主题色:', appStore.themeConfig.primaryColor)
}
</script>

<style scoped>
/* App.vue 根组件样式 */
</style>

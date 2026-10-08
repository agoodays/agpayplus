<template>
  <div class="user-layout" :style="layoutStyle">
    <div class="content-header">
      <img :src="loginLogoUrl" class="header-logo" :alt="sysTitle" />
      <span v-if="sysTitle" class="header-title">{{ sysTitle }}</span>
    </div>
    <div class="content-body">
      <div class="content-main" :class="cardPositionClass">
        <div v-if="bannerVisible" class="banner-content">
          <!-- 预留左侧插画/宣传位，后续可扩展为 bannerImageUrl -->
        </div>
        <div class="user-layout-content">
          <router-view />
        </div>
      </div>
      <div class="content-footer">
        <div class="footer-info">
          <div>
            <span>{{ t('userLayout.footerCopyright', { year: currentYear }) }}</span>
          </div>
          <div class="top-list">
            <div v-if="icpRecord" class="t-list-item">
              <img src="@/assets/beian.png" /> {{ icpRecord }}
            </div>
            <div v-if="pcacRecord" class="t-list-item">
              <img src="@/assets/pcac.png" /> {{ pcacRecord }}
            </div>
            <div v-if="telecomPermit" class="t-list-item">
              <img src="@/assets/permit.png" /> {{ telecomPermit }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
/**
 * 登录页布局（UserLayout）
 *
 * 动态配置项（从 appStore.resolveCurrentSiteSysConfig() 读取，字段：
 *   - logoUrl:          顶部 Logo 图片
 *   - sysTitle:         系统名称（头部 + 浏览器标题）
 *   - loginBgUrl:       登录页背景图 URL
 *   - loginCardPosition: 登录卡片位置 'left' | 'center' | 'right'(默认)
 *   - icpRecord:        ICP 备案文案
 *   - pcacRecord:       公安备案文案
 *   - telecomPermit:    电信增值业务许可证文案
 * 以上字段缺失时回退到静态默认值。
 */
import { useAppStore } from '@/store/modules/system/app'
import dayjs from 'dayjs'
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import defaultLogo from '@/assets/logo.svg'

const { t } = useI18n()
const appStore = useAppStore()
const currentYear = dayjs().year()

/** 当前系统子配置（mgr/agent/mch，按 __APP_ID__ 映射） */
const sysConfig = computed(() => appStore.resolveCurrentSiteSysConfig() || {})

/** siteInfo 通用字段兜底 */
const siteInfo = computed(() => appStore.siteInfo || {})

/** 顶部 Logo（优先当前系统配置，其次 siteInfo.logoUrl，最后用内置 SVG） */
const loginLogoUrl = computed(() => sysConfig.value.logoUrl || siteInfo.value.logoUrl || defaultLogo)

/** 系统名称（优先当前系统配置，其次 siteInfo，最后 env 标题） */
const sysTitle = computed(() =>
  sysConfig.value.sysTitle || siteInfo.value.sysTitle || import.meta.env.VITE_APP_TITLE || ''
)

/** 登录页背景（URL 则设为 background-image，否则使用 CSS 变量默认背景） */
const loginBgUrl = computed(() => sysConfig.value.loginBgUrl || '')

const layoutStyle = computed(() => {
  if (loginBgUrl.value) {
    return {
      backgroundImage: `url(${loginBgUrl.value})`,
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      backgroundRepeat: 'no-repeat'
    }
  }
  return {}
})

/** 登录卡片位置（left/center/right），映射到 flex justify 类 */
const cardPosition = computed(() => sysConfig.value.loginCardPosition || 'right')
const cardPositionClass = computed(() => {
  switch (cardPosition.value) {
    case 'left':   return 'justify-start'
    case 'center': return 'justify-center'
    default:       return 'justify-end'
  }
})

/** 左侧是否显示插画/宣传位：仅当卡片非居中且非左对齐时显示 */
const bannerVisible = computed(() => cardPosition.value !== 'center')

/** 备案信息（优先 siteInfo 通用字段，其次当前系统子配置） */
const icpRecord = computed(() =>
  sysConfig.value.icpRecord || siteInfo.value.icpRecord || t('userLayout.icpRecord')
)
const pcacRecord = computed(() =>
  sysConfig.value.pcacRecord || siteInfo.value.pcacRecord || t('userLayout.pcacRecord')
)
const telecomPermit = computed(() =>
  sysConfig.value.telecomPermit || siteInfo.value.telecomPermit || t('userLayout.telecomPermit')
)
</script>

<style lang="less">
.user-layout {
  min-height: 100vh;
  background-color: var(--base-bg-color);
  transition: background-color 0.3s ease;
}

.content-header {
  display: flex;
  align-items: center;
  gap: 12px;
  position: fixed;
  z-index: 100;
  padding: 0 50px;
  left: 0;
  right: 0;
  height: 80px;
  background-color: var(--base-bg-color);
  backdrop-filter: blur(50px) saturate(180%);
}

.header-logo {
  height: 35px;
}

.header-title {
  font-size: 20px;
  font-weight: 600;
  color: var(--text-color);
  letter-spacing: 0.04em;
}

.content-body {
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  object-fit: cover;
  padding: 80px 10vw 0;
  height: 100vh;
  min-height: 100vh;
  overflow-y: auto;
}

.content-body .content-main {
  flex-grow: 1;
  display: flex;
  align-items: center;
  width: 100%;
}

.content-body .content-main.justify-end    { justify-content: flex-end; }
.content-body .content-main.justify-start  { justify-content: flex-start; }
.content-body .content-main.justify-center { justify-content: center; }

.content-main .banner-content {
  flex: 1 1 0;
  min-height: 300px;
  max-width: 50%;
}

.user-layout-content {
  transition: 0.3s ease;
  flex-shrink: 0;
  width: 420px;
  max-width: 90vw;
}

.user-layout-content .login-error-message {
  margin-bottom: 24px;
}
.user-layout-content .main {
  padding: 50px 30px;
  box-sizing: border-box;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  background: var(--base-bg-color);
  box-shadow: 0 30px 60px -35px var(--shadow-color);
  border: 1px solid var(--border-color);
  align-items: center;
  border-radius: var(--border-radius-lg);
  transition:
    all 0.3s,
    border 0s ease;
}

.user-layout-content .main .ant-form {
  max-width: 300px;
  padding: 15px;
  border-radius: var(--border-radius-lg);
}

.user-layout-content .main .desc {
  font-weight: 700;
  font-size: 20px;
  letter-spacing: 0.04em;
  color: var(--ant-primary-color);
  margin-bottom: 50px;
  text-align: center;
}

.user-layout-content .footer {
  width: 100%;
  bottom: 0;
  padding: 0 16px;
  margin: 48px 0 24px;
  text-align: center;
}

.user-layout-content .footer .links {
  margin-bottom: 8px;
  font-size: 14px;
}

.user-layout-content .footer .links a {
  color: var(--text-color-muted);
  transition: all 0.3s;
}

.user-layout-content .footer .links a:not(:last-child) {
  margin-right: 40px;
}

.user-layout-content .footer .copyright {
  color: var(--text-color-muted);
  font-size: 14px;
}

.code-body {
  width: 300px;
  overflow-x: hidden;
}

.code-layout {
  display: flex;
  width: 600px;
  transition: 0.3s;
}

.code-layout .code-layout-item {
  width: 50%;
}

.footer-info {
  padding-bottom: 25px;
  color: var(--text-color-secondary);
  font-size: 14px;
  letter-spacing: 1.5px;
  text-align: center;
}

.footer-info span {
  cursor: pointer;
}

.footer-info img {
  margin-right: 5px;
  width: 20px;
  height: 20px;
}

.footer-info .top-list {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  margin-top: 20px;
}

.footer-info .top-list .t-list-item {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  margin-left: 30px;
  cursor: pointer;
}

.footer-info a {
  color: var(--text-color-secondary);
}

/* 小屏幕兼容 */
@media (max-width: 900px) {
  .content-main .banner-content { display: none; }
  .content-body { padding: 80px 5vw 0; }
  .content-body .content-main { justify-content: center !important; }
}
</style>

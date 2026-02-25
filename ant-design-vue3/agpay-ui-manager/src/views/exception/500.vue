<template>
  <div class="exception-page">
    <div class="exception-content">
      <div class="exception-icon">
        <svg viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
          <text x="50%" y="50%" font-size="120" font-weight="bold" fill="#f5222d" text-anchor="middle" dominant-baseline="middle">500</text>
        </svg>
      </div>
      <h1 class="exception-title">{{ t('exception500.title') }}</h1>
      <p class="exception-desc">{{ t('exception500.desc') }}</p>
      <a-space>
        <a-button type="primary" size="large" @click="handleGoHome">
          {{ t('exception500.backHome') }}
        </a-button>
        <a-button size="large" @click="handleRefresh">
          {{ t('layout.refreshPage') }}
        </a-button>
      </a-space>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

const router = useRouter()
const { t } = useI18n()

/**
 * 返回首页
 */
const handleGoHome = () => {
  router.push('/')
}

/**
 * 刷新页面
 */
const handleRefresh = () => {
  window.location.reload()
}
</script>

<style lang="less" scoped>
.exception-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 100px);
  padding: 40px 20px;
}

.exception-content {
  text-align: center;
  max-width: 500px;
}

.exception-icon {
  margin-bottom: 32px;

  svg {
    width: 200px;
    height: 200px;
  }
}

.exception-title {
  font-size: 32px;
  font-weight: 600;
  color: var(--text-color);
  margin-bottom: 16px;
}

.exception-desc {
  font-size: 16px;
  color: var(--text-color-weak);
  margin-bottom: 32px;
}
</style>

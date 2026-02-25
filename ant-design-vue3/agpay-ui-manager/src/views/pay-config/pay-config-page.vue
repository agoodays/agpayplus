<template>
  <div class="pay-config-page">
    <a-card :bordered="false">
      <a-result
        status="info"
        :title="t('payConfig.comingSoonTitle')"
        :sub-title="t('common.comingSoon')"
      >
        <template #extra>
          <a-space direction="vertical" size="large" style="width: 100%">
            <a-alert
              :message="t('common.featureDescription')"
              :description="t('payConfig.featureIntro')"
              type="info"
              show-icon
            />

            <a-card :title="t('common.plannedFeatures')" size="small">
              <a-list :data-source="features" size="small">
                <template #renderItem="{ item }">
                  <a-list-item>
                    <a-list-item-meta>
                      <template #avatar>
                        <check-circle-outlined style="color: #52c41a; font-size: 20px" />
                      </template>
                      <template #title>
                        {{ item.title }}
                      </template>
                      <template #description>
                        {{ item.description }}
                      </template>
                    </a-list-item-meta>
                  </a-list-item>
                </template>
              </a-list>
            </a-card>

            <a-space>
              <a-button type="primary" @click="handleGoBack">
                {{ t('common.backPrevious') }}
              </a-button>
              <a-button @click="handleGoHome">
                {{ t('exception500.backHome') }}
              </a-button>
            </a-space>
          </a-space>
        </template>
      </a-result>
    </a-card>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { CheckCircleOutlined } from '@ant-design/icons-vue'
import { useI18n } from 'vue-i18n'

const router = useRouter()
const { t } = useI18n()

// 计划功能列表
const features = [
  {
    title: t('payConfig.features.apiConfig.title'),
    description: t('payConfig.features.apiConfig.desc')
  },
  {
    title: t('payConfig.features.paramManage.title'),
    description: t('payConfig.features.paramManage.desc')
  },
  {
    title: t('payConfig.features.apiTest.title'),
    description: t('payConfig.features.apiTest.desc')
  },
  {
    title: t('payConfig.features.statusMonitor.title'),
    description: t('payConfig.features.statusMonitor.desc')
  },
  {
    title: t('payConfig.features.auditLog.title'),
    description: t('payConfig.features.auditLog.desc')
  }
]

/**
 * 返回上一页
 */
const handleGoBack = () => {
  router.back()
}

/**
 * 返回首页
 */
const handleGoHome = () => {
  router.push('/')
}
</script>

<style lang="less" scoped>
.pay-config-page {
  :deep(.ant-result) {
    padding: 48px 32px;
  }

  :deep(.ant-list-item-meta-title) {
    font-weight: 500;
  }
}
</style>

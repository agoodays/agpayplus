<template>
  <div class="pay-config-page">
    <a-card :bordered="false">
      <a-result
        status="info"
        title="支付配置功能"
        sub-title="此功能正在开发中，敬请期待"
      >
        <template #extra>
          <a-space direction="vertical" size="large" style="width: 100%">
            <a-alert
              message="功能说明"
              description="商户支付配置功能允许为不同商户配置支付接口参数，包括支付宝、微信支付等第三方支付渠道。"
              type="info"
              show-icon
            />

            <a-card title="计划功能" size="small">
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
                返回上一页
              </a-button>
              <a-button @click="handleGoHome">
                返回首页
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

const router = useRouter()

// 计划功能列表
const features = [
  {
    title: '支付接口配置',
    description: '支持配置支付宝、微信支付、云闪付等主流支付接口'
  },
  {
    title: '参数管理',
    description: '管理商户的支付参数，如商户号、密钥、证书等'
  },
  {
    title: '接口测试',
    description: '提供接口测试功能，快速验证配置是否正确'
  },
  {
    title: '状态监控',
    description: '实时监控支付接口状态，及时发现和处理异常'
  },
  {
    title: '日志记录',
    description: '记录配置变更日志，便于追溯和审计'
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

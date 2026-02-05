<template>
  <div class="transfer-order-page">
    <a-card :bordered="false">
      <a-result
        status="info"
        title="转账订单管理"
        sub-title="此功能正在开发中，敬请期待"
      >
        <template #extra>
          <a-space direction="vertical" size="large" style="width: 100%">
            <a-alert
              message="功能说明"
              description="转账订单管理功能允许查看和管理商户的转账订单，包括商户提现、代付等场景。"
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
    title: '转账订单列表',
    description: '查看所有转账订单，支持多维度筛选和搜索'
  },
  {
    title: '转账详情',
    description: '查看转账订单的详细信息，包括收款人、金额、状态等'
  },
  {
    title: '转账审核',
    description: '对转账订单进行审核，确保资金安全'
  },
  {
    title: '批量转账',
    description: '支持批量转账功能，提高操作效率'
  },
  {
    title: '转账统计',
    description: '提供转账数据统计分析功能'
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
.transfer-order-page {
  :deep(.ant-result) {
    padding: 48px 32px;
  }

  :deep(.ant-list-item-meta-title) {
    font-weight: 500;
  }
}
</style>

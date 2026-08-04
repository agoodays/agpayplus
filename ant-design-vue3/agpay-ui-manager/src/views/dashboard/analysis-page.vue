<template>
  <div class="main-page">
    <a-card :bordered="false" class="welcome-card">
      <a-row :gutter="24">
        <a-col :span="24">
          <h2>{{ greetingText }}</h2>
          <p class="welcome-desc">{{ t('main.welcomeDesc') }}</p>
        </a-col>
      </a-row>
    </a-card>

    <!-- 数据统计卡片 -->
    <a-row :gutter="[16, 16]" style="margin-top: 16px">
      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic
            :title="t('main.todayAmount')"
            :value="statistics.todayAmount"
            :precision="2"
            :suffix="t('main.yuan')"
            :value-style="{ color: 'var(--success-color)' }"
          >
            <template #prefix>
              <transaction-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic :title="t('main.todayCount')" :value="statistics.todayCount" :suffix="t('main.countUnit')">
            <template #prefix>
              <file-text-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic :title="t('main.totalMch')" :value="statistics.totalMch" :suffix="t('main.itemUnit')">
            <template #prefix>
              <shop-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic :title="t('main.totalAgent')" :value="statistics.totalAgent" :suffix="t('main.itemUnit')">
            <template #prefix>
              <team-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- 快速入口 -->
    <a-card :title="t('main.quickEntry')" style="margin-top: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col v-for="menu in quickMenuList" :key="menu.entId" :xs="24" :sm="12" :md="8" :lg="6">
          <a-card hoverable class="quick-menu-card" @click="handleMenuClick(menu)">
            <div class="quick-menu-content">
              <component :is="menu.icon" class="quick-menu-icon" />
              <span class="quick-menu-title">{{ menu.entName }}</span>
            </div>
          </a-card>
        </a-col>
      </a-row>
    </a-card>
  </div>
</template>

<script setup>
import { dashboardApi } from '@/api/business/dashboard/dashboard-api'
import { useUserStore } from '@/store/modules/system/user'
import { timeFix } from '@/utils/time-util'
import { FileTextOutlined, ShopOutlined, TeamOutlined, TransactionOutlined } from '@ant-design/icons-vue'
import { computed, onMounted, reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
const router = useRouter()
const userStore = useUserStore()
const { t } = useI18n()

const loading = ref(true)
const statistics = reactive({
  todayAmount: 0,
  todayCount: 0,
  totalMch: 0,
  totalAgent: 0
})

const greetingText = computed(() => {
  const userName = userStore.realname || userStore.loginUsername || t('main.defaultUser')
  return t('main.greeting', { greet: timeFix(), name: userName })
})

const quickMenuList = computed(() => {
  const allMenus = userStore.allMenuRouteTree || []
  const quickMenus = []

  const findQuickMenus = (menus) => {
    menus.forEach((menu) => {
      if (menu.entType === 'ML' && menu.menuUri) {
        quickMenus.push(menu)
      }
      if (menu.children && menu.children.length > 0) {
        findQuickMenus(menu.children)
      }
    })
  }

  findQuickMenus(allMenus)
  return quickMenus.slice(0, 8)
})

const fetchStatistics = async () => {
  try {
    loading.value = true

    const dayCountRes = await dashboardApi.queryPayDayCount({ queryDateRange: 'today' })
    if (dayCountRes) {
      statistics.todayAmount = dayCountRes.payAmount || 0
      statistics.todayCount = dayCountRes.payCount || 0
    }

    const countRes = await dashboardApi.queryIsvAndMchCount()
    if (countRes) {
      statistics.totalMch = countRes.totalMch || 0
      statistics.totalAgent = countRes.totalAgent || 0
    }
  } catch (error) {
    console.error('获取统计数据失败:', error)
  } finally {
    loading.value = false
  }
}

const handleMenuClick = (menu) => {
  if (menu.menuUri) {
    router.push(menu.menuUri)
  }
}

onMounted(() => {
  fetchStatistics()
})
</script>

<style lang="less" scoped>
.main-page {
  .welcome-card {
    h2 {
      font-size: 24px;
      margin-bottom: 8px;
    }

    .welcome-desc {
      color: var(--text-color-muted);
      margin-bottom: 0;
    }
  }

  .quick-menu-card {
    cursor: pointer;
    transition: all 0.3s;

    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 2px 8px var(--shadow-color);
    }

    .quick-menu-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 16px 0;

      .quick-menu-icon {
        font-size: 32px;
        color: var(--primary-color);
        margin-bottom: 12px;
      }

      .quick-menu-title {
        font-size: 14px;
        color: var(--text-color);
      }
    }
  }
}
</style>

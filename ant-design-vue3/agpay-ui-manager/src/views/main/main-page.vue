<template>
  <div class="main-page">
    <a-card :bordered="false" class="welcome-card">
      <a-row :gutter="24">
        <a-col :span="24">
          <h2>{{ greetingText }}</h2>
          <p class="welcome-desc">欢迎使用 AgPay 运营管理平台</p>
        </a-col>
      </a-row>
    </a-card>

    <!-- 数据统计卡片 -->
    <a-row :gutter="[16, 16]" style="margin-top: 16px">
      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic
            title="今日交易金额"
            :value="statistics.todayAmount"
            :precision="2"
            suffix="元"
            :value-style="{ color: '#3f8600' }"
          >
            <template #prefix>
              <transaction-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic
            title="今日交易笔数"
            :value="statistics.todayCount"
            suffix="笔"
          >
            <template #prefix>
              <file-text-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic
            title="商户总数"
            :value="statistics.totalMch"
            suffix="个"
          >
            <template #prefix>
              <shop-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :loading="loading">
          <a-statistic
            title="代理商总数"
            :value="statistics.totalAgent"
            suffix="个"
          >
            <template #prefix>
              <team-outlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- 快速入口 -->
    <a-card title="快速入口" style="margin-top: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col
          v-for="menu in quickMenuList"
          :key="menu.entId"
          :xs="24"
          :sm="12"
          :md="8"
          :lg="6"
        >
          <a-card
            hoverable
            class="quick-menu-card"
            @click="handleMenuClick(menu)"
          >
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

<script>
import { defineComponent, ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import {
  TransactionOutlined,
  FileTextOutlined,
  ShopOutlined,
  TeamOutlined
} from '@ant-design/icons-vue'
import { useUserStore } from '/@/store/modules/system/user'
import { timeFix } from '/@/utils/time-util'
import { req } from '/@/api/manage'

export default defineComponent({
  name: 'MainPage',
  components: {
    TransactionOutlined,
    FileTextOutlined,
    ShopOutlined,
    TeamOutlined
  },
  setup() {
    const router = useRouter()
    const userStore = useUserStore()

    const loading = ref(true)
    const statistics = reactive({
      todayAmount: 0,
      todayCount: 0,
      totalMch: 0,
      totalAgent: 0
    })

    // 问候语
    const greetingText = computed(() => {
      const userName = userStore.realname || userStore.loginUsername || '用户'
      return `${timeFix()}，${userName}！`
    })

    // 获取快速菜单列表
    const quickMenuList = computed(() => {
      // 从用户的菜单中筛选出常用菜单
      const allMenus = userStore.allMenuRouteTree || []
      const quickMenus = []

      // 递归查找菜单
      const findQuickMenus = (menus) => {
        menus.forEach(menu => {
          // 如果是菜单链接类型且有路径
          if (menu.entType === 'ML' && menu.menuUri) {
            quickMenus.push(menu)
          }
          // 递归查找子菜单
          if (menu.children && menu.children.length > 0) {
            findQuickMenus(menu.children)
          }
        })
      }

      findQuickMenus(allMenus)

      // 只返回前8个菜单
      return quickMenus.slice(0, 8)
    })

    /**
     * 获取统计数据
     */
    const fetchStatistics = async () => {
      try {
        loading.value = true

        // 获取今日统计数据
        const dayCountRes = await req.get('/api/mainChart/payDayCount', {
          queryDateRange: 'today'
        })
        if (dayCountRes) {
          statistics.todayAmount = dayCountRes.payAmount || 0
          statistics.todayCount = dayCountRes.payCount || 0
        }

        // 获取商户和代理商数量
        const countRes = await req.get('/api/mainChart/isvAndMchCount')
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

    /**
     * 菜单点击
     */
    const handleMenuClick = (menu) => {
      if (menu.menuUri) {
        router.push(menu.menuUri)
      }
    }

    onMounted(() => {
      fetchStatistics()
    })

    return {
      loading,
      statistics,
      greetingText,
      quickMenuList,
      handleMenuClick
    }
  }
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
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
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

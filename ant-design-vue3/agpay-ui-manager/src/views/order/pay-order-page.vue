<template>
  <div class="pay-order-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <a-form
        :model="searchParams"
        layout="inline"
        class="search-form"
      >
        <!-- 日期范围 -->
        <a-form-item label="创建时间">
          <a-range-picker
            v-model:value="dateRange"
            :show-time="{ format: 'HH:mm:ss' }"
            format="YYYY-MM-DD HH:mm:ss"
            @change="handleDateChange"
          />
        </a-form-item>

        <!-- 订单号类型 -->
        <a-form-item label="订单号">
          <a-input-group compact>
            <a-select
              v-model:value="orderNoType"
              style="width: 140px"
            >
              <a-select-option value="payOrderId">支付订单号</a-select-option>
              <a-select-option value="mchOrderNo">商户订单号</a-select-option>
              <a-select-option value="channelOrderNo">渠道订单号</a-select-option>
            </a-select>
            <a-input
              v-model:value="searchParams[orderNoType]"
              placeholder="请输入订单号"
              allow-clear
              style="width: 200px"
              @pressEnter="handleSearch"
            />
          </a-input-group>
        </a-form-item>

        <!-- 商户号 -->
        <a-form-item label="商户号">
          <a-select
            v-model:value="searchParams.mchNo"
            placeholder="请选择商户"
            show-search
            :filter-option="false"
            allow-clear
            style="width: 200px"
            @search="handleSearchMch"
          >
            <a-select-option
              v-for="item in mchList"
              :key="item.mchNo"
              :value="item.mchNo"
            >
              {{ item.mchName }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <!-- 展开更多 -->
        <template v-if="showMore">
          <a-form-item label="支付状态">
            <a-select
              v-model:value="searchParams.state"
              placeholder="全部"
              allow-clear
              style="width: 140px"
            >
              <a-select-option :value="0">订单生成</a-select-option>
              <a-select-option :value="1">支付中</a-select-option>
              <a-select-option :value="2">支付成功</a-select-option>
              <a-select-option :value="3">支付失败</a-select-option>
              <a-select-option :value="4">已撤销</a-select-option>
              <a-select-option :value="5">已退款</a-select-option>
              <a-select-option :value="6">订单关闭</a-select-option>
            </a-select>
          </a-form-item>

          <a-form-item label="回调状态">
            <a-select
              v-model:value="searchParams.notifyState"
              placeholder="全部"
              allow-clear
              style="width: 140px"
            >
              <a-select-option :value="0">未发送</a-select-option>
              <a-select-option :value="1">已发送</a-select-option>
            </a-select>
          </a-form-item>

          <a-form-item label="应用ID">
            <a-input
              v-model:value="searchParams.appId"
              placeholder="请输入应用ID"
              allow-clear
              @pressEnter="handleSearch"
            />
          </a-form-item>

          <a-form-item label="门店ID">
            <a-input
              v-model:value="searchParams.storeId"
              placeholder="请输入门店ID"
              allow-clear
              @pressEnter="handleSearch"
            />
          </a-form-item>
        </template>

        <!-- 操作按钮 -->
        <a-form-item>
          <a-space>
            <a-button type="primary" @click="handleSearch">
              <search-outlined />
              查询
            </a-button>
            <a-button @click="handleReset">
              <redo-outlined />
              重置
            </a-button>
            <a-button type="link" @click="showMore = !showMore">
              {{ showMore ? '收起' : '展开' }}
              <down-outlined v-if="!showMore" />
              <up-outlined v-else />
            </a-button>
          </a-space>
        </a-form-item>
      </a-form>

      <!-- 统计信息 -->
      <a-card v-if="statistics" class="statistics-card" :bordered="false">
        <a-row :gutter="16">
          <a-col :span="6">
            <a-statistic
              title="成交订单"
              :value="statistics.payAmount"
              :precision="2"
              suffix="元"
            >
              <template #prefix>
                <transaction-outlined style="color: #1890ff" />
              </template>
            </a-statistic>
            <div class="statistic-detail">{{ statistics.payCount }} 笔</div>
          </a-col>

          <a-col :span="6">
            <a-statistic
              title="手续费金额"
              :value="statistics.mchFeeAmount"
              :precision="2"
              suffix="元"
            >
              <template #prefix>
                <dollar-outlined style="color: #faad14" />
              </template>
            </a-statistic>
          </a-col>

          <a-col :span="6">
            <a-statistic
              title="实际收款"
              :value="statistics.payAmount - statistics.mchFeeAmount"
              :precision="2"
              suffix="元"
            >
              <template #prefix>
                <wallet-outlined style="color: #52c41a" />
              </template>
            </a-statistic>
          </a-col>

          <a-col :span="6">
            <a-statistic
              title="退款订单"
              :value="statistics.refundAmount"
              :precision="2"
              suffix="元"
              :value-style="{ color: '#cf1322' }"
            >
              <template #prefix>
                <undo-outlined />
              </template>
            </a-statistic>
            <div class="statistic-detail">{{ statistics.refundCount }} 笔</div>
          </a-col>
        </a-row>
      </a-card>

      <!-- 操作按钮 -->
      <div class="table-operations">
        <a-space>
          <a-button @click="refresh">
            <reload-outlined />
            刷新
          </a-button>
          <a-button @click="handleExport">
            <download-outlined />
            导出
          </a-button>
        </a-space>
      </div>

      <!-- 数据表格 -->
      <a-table
        :columns="columns"
        :data-source="dataSource"
        :loading="loading"
        :pagination="pagination"
        row-key="payOrderId"
        :scroll="{ x: 1800 }"
        @change="handleTableChange"
      >
        <!-- 支付订单号 -->
        <template #payOrderId="{ text }">
          <a-typography-text copyable>{{ text }}</a-typography-text>
        </template>

        <!-- 商户订单号 -->
        <template #mchOrderNo="{ text }">
          <a-typography-text copyable>{{ text }}</a-typography-text>
        </template>

        <!-- 支付金额 -->
        <template #amount="{ text }">
          <span style="color: #1890ff; font-weight: 500">
            ¥{{ (text / 100).toFixed(2) }}
          </span>
        </template>

        <!-- 手续费 -->
        <template #mchFeeAmount="{ text }">
          <span>¥{{ (text / 100).toFixed(2) }}</span>
        </template>

        <!-- 支付状态 -->
        <template #state="{ text }">
          <a-tag :color="getStateColor(text)">
            {{ getStateText(text) }}
          </a-tag>
        </template>

        <!-- 回调状态 -->
        <template #notifyState="{ text }">
          <a-badge
            :status="text === 1 ? 'success' : 'default'"
            :text="text === 1 ? '已发送' : '未发送'"
          />
        </template>

        <!-- 操作 -->
        <template #action="{ record }">
          <a-space>
            <a-button
              v-if="hasPermission('ENT_PAY_ORDER_VIEW')"
              type="link"
              size="small"
              @click="handleDetail(record)"
            >
              详情
            </a-button>

            <a-button
              v-if="hasPermission('ENT_PAY_ORDER_REFUND') && record.state === 2"
              type="link"
              size="small"
              @click="handleRefund(record)"
            >
              退款
            </a-button>
          </a-space>
        </template>
      </a-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail-drawer
      v-model:open="detailOpen"
      :pay-order-id="currentPayOrderId"
    />

    <!-- 退款弹窗 -->
    <refund-modal
      v-model:open="refundOpen"
      :pay-order="currentPayOrder"
      @success="handleRefundSuccess"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import {
  SearchOutlined,
  RedoOutlined,
  DownOutlined,
  UpOutlined,
  ReloadOutlined,
  DownloadOutlined,
  TransactionOutlined,
  DollarOutlined,
  WalletOutlined,
  UndoOutlined
} from '@ant-design/icons-vue'
import { useTable, useModal, usePermission } from '@/hooks/common-hooks'
import { API_URL_PAY_ORDER, API_URL_MCH_LIST, req } from '@/api/manage'
import DetailDrawer from './detail-drawer.vue'
import RefundModal from './refund-modal.vue'

const { t } = useI18n()

// 使用 Hooks
const { loading, dataSource, pagination, searchParams, handleTableChange, handleSearch, handleReset, refresh } = 
  useTable((params) => req.list(API_URL_PAY_ORDER, params))

const { open: detailOpen, showModal: showDetail } = useModal()
const { open: refundOpen, showModal: showRefund } = useModal()
const { hasPermission } = usePermission()

// State
const showMore = ref(false)
const dateRange = ref([])
const orderNoType = ref('payOrderId')
const mchList = ref([])
const currentPayOrderId = ref('')
const currentPayOrder = ref(null)
const statistics = ref(null)

// 表格列定义
const columns = [
  {
    title: '支付订单号',
    dataIndex: 'payOrderId',
    key: 'payOrderId',
    width: 180,
    fixed: 'left',
    slots: { customRender: 'payOrderId' }
  },
  {
    title: '商户订单号',
    dataIndex: 'mchOrderNo',
    key: 'mchOrderNo',
    width: 180,
    slots: { customRender: 'mchOrderNo' }
  },
  {
    title: '商户名称',
    dataIndex: 'mchName',
    key: 'mchName',
    width: 150,
    ellipsis: true
  },
  {
    title: '支付金额',
    dataIndex: 'amount',
    key: 'amount',
    width: 120,
    align: 'right',
    slots: { customRender: 'amount' }
  },
  {
    title: '手续费',
    dataIndex: 'mchFeeAmount',
    key: 'mchFeeAmount',
    width: 100,
    align: 'right',
    slots: { customRender: 'mchFeeAmount' }
  },
  {
    title: '支付方式',
    dataIndex: 'wayName',
    key: 'wayName',
    width: 120
  },
  {
    title: '支付状态',
    dataIndex: 'state',
    key: 'state',
    width: 100,
    slots: { customRender: 'state' }
  },
  {
    title: '回调状态',
    dataIndex: 'notifyState',
    key: 'notifyState',
    width: 100,
    slots: { customRender: 'notifyState' }
  },
  {
    title: '创建时间',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 180
  },
  {
    title: '操作',
    key: 'action',
    width: 150,
    fixed: 'right',
    slots: { customRender: 'action' }
  }
]

/**
 * 初始化
 */
onMounted(() => {
  handleSearch()
  loadStatistics()
})

/**
 * 日期范围变化
 */
const handleDateChange = (dates) => {
  if (dates && dates.length === 2) {
    searchParams.createdStart = dates[0].format('YYYY-MM-DD HH:mm:ss')
    searchParams.createdEnd = dates[1].format('YYYY-MM-DD HH:mm:ss')
  } else {
    searchParams.createdStart = ''
    searchParams.createdEnd = ''
  }
}

/**
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }
  
  try {
    const res = await req.list(API_URL_MCH_LIST, {
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 加载统计数据
 */
const loadStatistics = async () => {
  try {
    const res = await req.getById(API_URL_PAY_ORDER + '/statistics', searchParams)
    statistics.value = res || {
      payAmount: 0,
      payCount: 0,
      mchFeeAmount: 0,
      refundAmount: 0,
      refundCount: 0
    }
  } catch (error) {
    console.error('加载统计数据失败:', error)
  }
}

/**
 * 获取状态颜色
 */
const getStateColor = (state) => {
  const colorMap = {
    0: 'default',
    1: 'processing',
    2: 'success',
    3: 'error',
    4: 'warning',
    5: 'orange',
    6: 'default'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取状态文本
 */
const getStateText = (state) => {
  const textMap = {
    0: '订单生成',
    1: '支付中',
    2: '支付成功',
    3: '支付失败',
    4: '已撤销',
    5: '已退款',
    6: '订单关闭'
  }
  return textMap[state] || '未知'
}

/**
 * 查看详情
 */
const handleDetail = (record) => {
  currentPayOrderId.value = record.payOrderId
  showDetail()
}

/**
 * 退款
 */
const handleRefund = (record) => {
  currentPayOrder.value = record
  showRefund()
}

/**
 * 退款成功
 */
const handleRefundSuccess = () => {
  refresh()
  loadStatistics()
}

/**
 * 导出
 */
const handleExport = () => {
  message.info(t('common.exportInDevelopment'))
}
</script>

<style lang="less" scoped>
.pay-order-page {
  .search-form {
    margin-bottom: 16px;
  }

  .statistics-card {
    margin-bottom: 16px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

    :deep(.ant-card-body) {
      padding: 24px;
    }

    :deep(.ant-statistic-title) {
      color: rgba(255, 255, 255, 0.85);
      font-size: 14px;
    }

    :deep(.ant-statistic-content) {
      color: #fff;
      font-size: 24px;
      font-weight: 600;
    }

    .statistic-detail {
      margin-top: 8px;
      color: rgba(255, 255, 255, 0.65);
      font-size: 12px;
    }
  }

  .table-operations {
    margin-bottom: 16px;
  }
}
</style>

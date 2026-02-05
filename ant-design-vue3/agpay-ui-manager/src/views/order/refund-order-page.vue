<template>
  <div class="refund-order-page">
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

        <!-- 订单号 -->
        <a-form-item label="退款订单号">
          <a-input
            v-model:value="searchParams.refundOrderId"
            placeholder="请输入退款订单号"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <!-- 支付订单号 -->
        <a-form-item label="支付订单号">
          <a-input
            v-model:value="searchParams.payOrderId"
            placeholder="请输入支付订单号"
            allow-clear
            @pressEnter="handleSearch"
          />
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
          <a-form-item label="退款状态">
            <a-select
              v-model:value="searchParams.state"
              placeholder="全部"
              allow-clear
              style="width: 140px"
            >
              <a-select-option :value="0">订单生成</a-select-option>
              <a-select-option :value="1">退款中</a-select-option>
              <a-select-option :value="2">退款成功</a-select-option>
              <a-select-option :value="3">退款失败</a-select-option>
              <a-select-option :value="4">退款任务关闭</a-select-option>
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
        row-key="refundOrderId"
        :scroll="{ x: 1600 }"
        @change="handleTableChange"
      >
        <!-- 退款订单号 -->
        <template #refundOrderId="{ text }">
          <a-typography-text copyable>{{ text }}</a-typography-text>
        </template>

        <!-- 支付订单号 -->
        <template #payOrderId="{ text }">
          <a-typography-text copyable>{{ text }}</a-typography-text>
        </template>

        <!-- 退款金额 -->
        <template #refundAmount="{ text }">
          <span style="color: #cf1322; font-weight: 500">
            ¥{{ (text / 100).toFixed(2) }}
          </span>
        </template>

        <!-- 退款状态 -->
        <template #state="{ text }">
          <a-tag :color="getStateColor(text)">
            {{ getStateText(text) }}
          </a-tag>
        </template>

        <!-- 操作 -->
        <template #action="{ record }">
          <a-button
            v-if="hasPermission('ENT_REFUND_ORDER_VIEW')"
            type="link"
            size="small"
            @click="handleDetail(record)"
          >
            详情
          </a-button>
        </template>
      </a-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail-drawer
      v-model:visible="detailVisible"
      :refund-order-id="currentRefundOrderId"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  SearchOutlined,
  RedoOutlined,
  DownOutlined,
  UpOutlined,
  ReloadOutlined,
  DownloadOutlined
} from '@ant-design/icons-vue'
import { useTable, useModal, usePermission } from '/@/hooks/common-hooks'
import { API_URL_REFUND_ORDER, API_URL_MCH_LIST, req } from '/@/api/manage'
import DetailDrawer from './refund-detail-drawer.vue'

// 使用 Hooks
const { loading, dataSource, pagination, searchParams, handleTableChange, handleSearch, handleReset, refresh } = 
  useTable((params) => req.list(API_URL_REFUND_ORDER, params))

const { visible: detailVisible, showModal: showDetail } = useModal()
const { hasPermission } = usePermission()

// State
const showMore = ref(false)
const dateRange = ref([])
const mchList = ref([])
const currentRefundOrderId = ref('')

// 表格列定义
const columns = [
  {
    title: '退款订单号',
    dataIndex: 'refundOrderId',
    key: 'refundOrderId',
    width: 180,
    fixed: 'left',
    slots: { customRender: 'refundOrderId' }
  },
  {
    title: '支付订单号',
    dataIndex: 'payOrderId',
    key: 'payOrderId',
    width: 180,
    slots: { customRender: 'payOrderId' }
  },
  {
    title: '商户名称',
    dataIndex: 'mchName',
    key: 'mchName',
    width: 150,
    ellipsis: true
  },
  {
    title: '退款金额',
    dataIndex: 'refundAmount',
    key: 'refundAmount',
    width: 120,
    align: 'right',
    slots: { customRender: 'refundAmount' }
  },
  {
    title: '退款原因',
    dataIndex: 'refundReason',
    key: 'refundReason',
    width: 150,
    ellipsis: true
  },
  {
    title: '退款状态',
    dataIndex: 'state',
    key: 'state',
    width: 100,
    slots: { customRender: 'state' }
  },
  {
    title: '创建时间',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 180
  },
  {
    title: '成功时间',
    dataIndex: 'successTime',
    key: 'successTime',
    width: 180
  },
  {
    title: '操作',
    key: 'action',
    width: 100,
    fixed: 'right',
    slots: { customRender: 'action' }
  }
]

/**
 * 初始化
 */
onMounted(() => {
  handleSearch()
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
 * 获取状态颜色
 */
const getStateColor = (state) => {
  const colorMap = {
    0: 'default',
    1: 'processing',
    2: 'success',
    3: 'error',
    4: 'warning'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取状态文本
 */
const getStateText = (state) => {
  const textMap = {
    0: '订单生成',
    1: '退款中',
    2: '退款成功',
    3: '退款失败',
    4: '任务关闭'
  }
  return textMap[state] || '未知'
}

/**
 * 查看详情
 */
const handleDetail = (record) => {
  currentRefundOrderId.value = record.refundOrderId
  showDetail()
}

/**
 * 导出
 */
const handleExport = () => {
  message.info('导出功能开发中')
}
</script>

<style lang="less" scoped>
.refund-order-page {
  .search-form {
    margin-bottom: 16px;
  }

  .table-operations {
    margin-bottom: 16px;
  }
}
</style>

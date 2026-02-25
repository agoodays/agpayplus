<template>
  <a-card>
    <!-- 搜索表单 -->
    <template #title>
      订单列表
    </template>

    <a-form :model="searchForm" layout="inline" class="search-form">
      <a-form-item label="订单号">
        <a-input v-model:value="searchForm.orderNo" placeholder="请输入订单号" allow-clear />
      </a-form-item>
      <a-form-item label="状态">
        <a-select v-model:value="searchForm.status" placeholder="请选择状态" allow-clear>
          <a-select-option value="pending">待支付</a-select-option>
          <a-select-option value="success">成功</a-select-option>
          <a-select-option value="failed">失败</a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item label="日期">
        <a-range-picker v-model:value="searchForm.dateRange" />
      </a-form-item>

      <a-form-item>
        <a-space>
          <a-button type="primary" @click="handleSearch">
            <search-outlined /> 搜索
          </a-button>
          <a-button @click="handleReset">
            <reload-outlined /> 重置
          </a-button>
        </a-space>
      </a-form-item>

      <a-form-item>
        <a-space>
          <a-button type="primary" @click="handleAdd">
            <plus-outlined /> 新增
          </a-button>
          <a-button danger :disabled="selectedRowKeys.length === 0" @click="handleBatchDelete">
            <delete-outlined /> 批量删除
          </a-button>
        </a-space>
      </a-form-item>
    </a-form>

    <!-- 表格 -->
    <ag-table
      ref="tableRef"
      :columns="columns"
      :search-data="tableSearchParams"
      :on-load="loadTable"
      :row-selection="rowSelection"
      :show-toolbar="true"
      :show-auto-refresh="true"
      :enable-statistics="true"
      :show-download="true"
      :enable-auto-refresh="false"
      state-key="order-list-table"
      @change="handleTableChange"
      @reload="handleTableReload"
      @statistics-loaded="handleStatsLoaded"
    >
      <template #toolbar-left>
        <a-space>
          <a-button type="primary" @click="handleAdd">
            <plus-outlined /> 新增
          </a-button>
          <a-button danger :disabled="selectedRowKeys.length === 0" @click="handleBatchDelete">
            <delete-outlined /> 批量删除
          </a-button>
        </a-space>
      </template>
      <!-- 订单号插槽 -->
      <template #orderNo="{ record }">
        <a-button type="link" size="small" @click="handleViewDetail(record)">
          {{ record.orderNo }}
        </a-button>
      </template>

      <!-- 状态插槽 -->
      <template #status="{ record }">
        <a-tag :color="getStatusColor(record.status)">
          {{ getStatusLabel(record.status) }}
        </a-tag>
      </template>

      <!-- 金额插槽 -->
      <template #amount="{ record }">
        <span style="color: #52c41a">¥{{ record.amount }}</span>
      </template>

      <!-- 操作插槽 -->
      <template #actions="{ record }">
        <a-space size="small">
          <a-button type="link" size="small" @click="handleViewDetail(record)">查看</a-button>
          <a-button 
            v-if="record.status === 'pending'"
            type="link" 
            size="small" 
            @click="handleEdit(record)"
          >
            编辑
          </a-button>
          <a-popconfirm title="确认删除?" @confirm="handleDelete(record)">
            <a-button type="link" size="small" danger>删除</a-button>
          </a-popconfirm>
        </a-space>
      </template>
    </ag-table>

    <!-- 详情抽屉 -->
    <a-drawer
      v-model:open="detailVisible"
      title="订单详情"
      width="600"
    >
      <div v-if="currentRecord">
        <a-descriptions :column="1" bordered>
          <a-descriptions-item label="订单号">
            {{ currentRecord.orderNo }}
          </a-descriptions-item>
          <a-descriptions-item label="商户">
            {{ currentRecord.merchant }}
          </a-descriptions-item>
          <a-descriptions-item label="金额">
            ¥{{ currentRecord.amount }}
          </a-descriptions-item>
          <a-descriptions-item label="状态">
            <a-tag :color="getStatusColor(currentRecord.status)">
              {{ getStatusLabel(currentRecord.status) }}
            </a-tag>
          </a-descriptions-item>
          <a-descriptions-item label="创建时间">
            {{ currentRecord.createTime }}
          </a-descriptions-item>
        </a-descriptions>
      </div>
    </a-drawer>

    <!-- 编辑弹窗 -->
    <a-modal
      v-model:open="editVisible"
      title="编辑订单"
      ok-text="保存"
      cancel-text="取消"
      @ok="handleSaveEdit"
    >
      <a-form v-if="editForm" :model="editForm" layout="vertical">
        <a-form-item label="订单号">
          <a-input v-model:value="editForm.orderNo" disabled />
        </a-form-item>
        <a-form-item label="商户">
          <a-input v-model:value="editForm.merchant" />
        </a-form-item>
        <a-form-item label="金额">
          <a-input-number v-model:value="editForm.amount" :min="0" />
        </a-form-item>
        <a-form-item label="备注">
          <a-textarea v-model:value="editForm.remark" rows="3" />
        </a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { message } from 'ant-design-vue'
import dayjs from 'dayjs'
import {
  SearchOutlined,
  ReloadOutlined,
  PlusOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'
import { AgTable } from '/@/components'
import { req } from '/@/api/manage'

// ==================== 搜索表单 ====================
const searchForm = reactive({
  orderNo: '',
  status: '',
  dateRange: []
})

// ==================== 使用 onLoad 调用后端 ====================
// 将请求函数传入 AgTable 的 `onLoad` prop，组件会在需要时调用它。
const tableSearchParams = computed(() => ({
  orderNo: searchForm.orderNo,
  status: searchForm.status,
  startDate: searchForm.dateRange?.[0]?.format('YYYY-MM-DD'),
  endDate: searchForm.dateRange?.[1]?.format('YYYY-MM-DD')
}))

// onLoad 函数，AgTable 会传入分页等参数作为参数对象
async function loadTable(params) {
  // params 可能包含: pageNumber, pageSize, sort, filters, ...
  const res = await req.list('/order/list', params)
  // req.list 已通过全局 request 处理并返回 data 字段内容
  // 适配后端返回格式为 { total, records } 或 { total, list }
  return {
    total: res.total || res.totalCount || 0,
    records: res.records || res.list || []
  }
}

// ==================== 表格配置 ====================
const tableRef = ref()
const selectedRowKeys = ref([])

const columns = [
  {
    title: '订单号',
    dataIndex: 'orderNo',
    key: 'orderNo',
    width: 150,
    fixed: 'left',
    customRender: 'orderNo'
  },
  {
    title: '商户',
    dataIndex: 'merchant',
    key: 'merchant',
    width: 120
  },
  {
    title: '金额',
    dataIndex: 'amount',
    key: 'amount',
    width: 100,
    customRender: 'amount',
    align: 'right'
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
    width: 100,
    customRender: 'status'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
    width: 180
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    fixed: 'right',
    customRender: 'actions'
  }
]

// 行选择配置
const rowSelection = {
  selectedRowKeys: selectedRowKeys,
  onChange: (keys) => {
    selectedRowKeys.value = keys
  }
}

// ==================== 详情抽屉 ====================
const detailVisible = ref(false)
const currentRecord = ref(null)

const handleViewDetail = (record) => {
  currentRecord.value = record
  detailVisible.value = true
}

// ==================== 编辑弹窗 ====================
const editVisible = ref(false)
const editForm = ref(null)

const handleEdit = (record) => {
  editForm.value = { ...record }
  editVisible.value = true
}

const handleSaveEdit = async () => {
  try {
    await api.update('/order/edit', editForm.value)
    message.success('编辑成功')
    editVisible.value = false
    tableRef.value.reload()
  } catch (error) {
    message.error(error.message || '编辑失败')
  }
}

// ==================== 操作方法 ====================

const handleSearch = () => {
  // 通知表格使用新的 search params 并重置到第一页
  tableRef.value?.reload(true)
}

const handleReset = () => {
  searchForm.orderNo = ''
  searchForm.status = ''
  searchForm.dateRange = []
  selectedRowKeys.value = []
  tableRef.value?.reload(true)
}

const handleAdd = () => {
  message.info('打开新增订单弹窗')
}

const handleDelete = async (record) => {
  try {
    await api.delete(`/order/${record.id}`)
    message.success('删除成功')
    // 重新加载表格
    tableRef.value.reload()
  } catch (error) {
    message.error(error.message || '删除失败')
  }
}

const handleBatchDelete = async () => {
  try {
    await api.batchDelete('/order/batch-delete', {
      ids: selectedRowKeys.value
    })
    message.success('批量删除成功')
    selectedRowKeys.value = []
    tableRef.value.reload()
  } catch (error) {
    message.error(error.message || '批量删除失败')
  }
}

// ==================== 表格事件 ====================

const handleTableChange = ({ pagination: pag, filters, sorter }) => {
  // 已通过 useTable 自动处理
  console.log('表格变化:', { pag, filters, sorter })
}

const handleTableReload = (tableData) => {
  console.log('表格重新加载，新数据:', tableData)
}

const handleStatsLoaded = (stats) => {
  console.log('统计数据加载完成:', stats)
}

// ==================== 辅助函数 ====================

const getStatusLabel = (status) => {
  const map = {
    pending: '待支付',
    success: '成功',
    failed: '失败'
  }
  return map[status] || status
}

const getStatusColor = (status) => {
  const map = {
    pending: 'orange',
    success: 'green',
    failed: 'red'
  }
  return map[status] || 'default'
}
</script>

<style scoped>
.search-form {
  margin-bottom: 16px;
  padding: 16px;
  /* background: #fafafa; */
  border-radius: 4px;
}
</style>

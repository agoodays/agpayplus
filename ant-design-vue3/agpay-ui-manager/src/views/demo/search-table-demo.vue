<template>
  <a-card :bordered="false">
    <div style="margin-bottom:16px;">
      <ag-search 
        v-model:modelValue="searchForm" 
        @search="onSearch" 
        @reset="onReset"
        :collapsible="true"
        :default-collapsed="true"
      >
        <!-- 基础搜索条件（始终显示） -->
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <AgInput
                v-model:value="searchForm.orderNo"
                label="订单号"
                placeholder="请输入订单号"
                :allow-clear="true"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <AgSelect v-model:value="searchForm.state" label="支付状态" placeholder="支付状态" allow-clear
              :options="[
                { value: '', label: '全部' },
                { value: '0', label: '订单生成' },
                { value: '1', label: '支付中' },
                { value: '2', label: '支付成功' },
                { value: '3', label: '支付失败' },
                { value: '4', label: '已撤销' },
                { value: '5', label: '已退款' },
                { value: '6', label: '订单关闭' }
              ]" />
            </a-form-item>
          </a-col>
        </template>

        <!-- 高级搜索条件（可展开显示） -->
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <AgDateRangePicker label="创建时间" v-model:value="searchForm.dateRange" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <AgInputNumberRange 
                v-model="searchForm.amountRange" 
                :min="0"
                :precision="2"
                label="金额范围"
                :placeholder="['最小金额', '最大金额']"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
    </div>

    <ag-table
      :columns="columns"
      :on-load="reqTableDataFunc"
      :on-load-statistics="reqTableCountFunc"
      :on-download="reqDownloadDataFunc"
      :enable-statistics="true"
      :show-download="true"
      :show-auto-refresh="true"
      :show-toolbar="true"
      :search-data="searchForm"
      state-key="demo_search_table_columns"
    >
      <template #actions="{ record }">
        <ag-table-actions :max-show-num="3">
          <a-button type="link" size="small" @click="onView(record)">查看</a-button>
          <a-button type="link" size="small" @click="onEdit(record)">编辑</a-button>
          <a-button type="link" size="small" @click="onCopy(record)">复制</a-button>
          <a-popconfirm title="确认删除？" @confirm="() => onDelete(record)">
            <a-button type="link" size="small" danger>删除</a-button>
          </a-popconfirm>
          <a-button type="link" size="small" @click="onExport(record)">导出</a-button>
        </ag-table-actions>
      </template>
    </ag-table>
  </a-card>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import { AgSearch, AgTable, AgInput, AgSelect, AgDateRangePicker, AgTableActions, AgInputNumberRange } from '@/components'

// 搜索参数
const searchForm = reactive({ 
  orderNo: '', 
  dateRange: '', 
  state: '',
  amountRange: [undefined, undefined]
})

// 表格列定义
const columns = ref([
  { 
    title: 'ID', 
    key: 'id', 
    dataIndex: 'id', 
    width: 80,
    fixed: 'left'
  },
  { 
    title: '订单号', 
    key: 'orderNo', 
    dataIndex: 'orderNo', 
    width: 180
  },
  { 
    title: '商户名称', 
    key: 'merchantName', 
    dataIndex: 'merchantName', 
    width: 150
  },
  { 
    title: '订单金额', 
    key: 'amount', 
    dataIndex: 'amount', 
    width: 120
  },
  { 
    title: '手续费', 
    key: 'fee', 
    dataIndex: 'fee', 
    width: 100
  },
  { 
    title: '实际金额', 
    key: 'actualAmount', 
    dataIndex: 'actualAmount', 
    width: 120
  },
  { 
    title: '支付状态', 
    key: 'status', 
    dataIndex: 'status', 
    width: 100
  },
  { 
    title: '支付方式', 
    key: 'payWay', 
    dataIndex: 'payWay', 
    width: 120
  },
  { 
    title: '支付时间', 
    key: 'payTime', 
    dataIndex: 'payTime', 
    width: 180
  },
  { 
    title: '创建时间', 
    key: 'createTime', 
    dataIndex: 'createTime', 
    width: 180
  },
  { 
    title: '备注', 
    key: 'remark', 
    dataIndex: 'remark', 
    width: 200
  },
  { 
    title: '操作', 
    key: 'actions', 
    customRender: 'actions', 
    width: 200,
    fixed: 'right'
  }
])

// 模拟后台数据
const TOTAL = 45
const statusMap = ['待处理', '支付中', '支付成功', '支付失败', '已退款']
const payWayMap = ['微信支付', '支付宝', '银行卡', '余额支付']
const merchantMap = ['商户A', '商户B', '商户C', '商户D', '商户E']

const allData = Array.from({ length: TOTAL }).map((_, i) => {
  const amount = (Math.random() * 1000 + 100).toFixed(2)
  const fee = (amount * 0.006).toFixed(2)
  const actualAmount = (amount - fee).toFixed(2)
  
  return {
    id: i + 1,
    orderNo: `ORD${String(1000 + i).padStart(8, '0')}`,
    merchantName: merchantMap[i % merchantMap.length],
    amount: amount,
    fee: fee,
    actualAmount: actualAmount,
    status: statusMap[i % statusMap.length],
    payWay: payWayMap[i % payWayMap.length],
    payTime: i % 3 === 0 ? '2024-01-' + String(10 + i % 20).padStart(2, '0') + ' 10:' + String(i % 60).padStart(2, '0') + ':00' : '-',
    createTime: '2024-01-' + String(10 + i % 20).padStart(2, '0') + ' ' + String(8 + i % 12).padStart(2, '0') + ':' + String(i % 60).padStart(2, '0') + ':00',
    remark: i % 5 === 0 ? '备注信息 ' + (i + 1) : '-'
  }
})

// 请求表格数据函数
function reqTableDataFunc(params) {
  // params: { pageNumber, pageSize, ... }
  return new Promise((resolve) => {
    const pageNumber = params.pageNumber || 1
    const pageSize = params.pageSize || 10
    const start = (pageNumber - 1) * pageSize
    const records = allData.slice(start, start + pageSize)
    setTimeout(() => resolve({ total: TOTAL, records }), 400)
  })
}

// 请求统计数据
function reqTableCountFunc(params) {
  return new Promise((resolve) => {
    const totalAmount = allData.reduce((s, r) => s + Number(r.amount), 0).toFixed(2)
    const totalFee = allData.reduce((s, r) => s + Number(r.fee), 0).toFixed(2)
    const success = allData.filter(r => r.status === '支付成功').length
    const pending = allData.filter(r => r.status === '待处理').length
    const failed = allData.filter(r => r.status === '支付失败').length
    
    // setTimeout(() => resolve({ 
    //   '总订单金额': `¥${totalAmount}`,
    //   '总手续费': `¥${totalFee}`,
    //   '支付成功': `${success}笔`,
    //   '待处理': `${pending}笔`,
    //   '支付失败': `${failed}笔`
    // }), 300)

    
    setTimeout(() => resolve([{
      "_groupName": "支付统计",
      '总订单金额': `¥${totalAmount}`,
      '总手续费': `¥${totalFee}`,
      '支付成功': `${success}笔`,
      '待处理': `${pending}笔`,
      '支付失败': `${failed}笔`
    },
    {
      "_groupName": "退款统计",
      "退款金额": `¥${totalAmount}`,
      '退款成功': `${success}笔`,
      '退款失败': `${failed}笔`,
      '退款中': `${pending}笔`
    }]), 300)
  })
}

// 模拟导出
function reqDownloadDataFunc(params) {
  return new Promise((resolve) => {
    setTimeout(() => {
      // 模拟触发导出
      resolve(true)
    }, 600)
  })
}

function onSearch(vals) {
  // AgSearch 已更新 searchForm，通过 searchForm 触发查询
  message.success('开始搜索')
}

function onReset() {
  searchForm.orderNo = ''
  searchForm.dateRange = 'today'
  searchForm.state = ''
  searchForm.amountRange = [undefined, undefined]
}

function onView(record) {
  message.info(`查看 ${record.orderNo}`)
}
function onEdit(record) {
  message.info(`编辑 ${record.orderNo}`)
}
function onCopy(record) {
  message.info(`复制 ${record.orderNo}`)
}
function onDelete(record) {
  message.success(`已删除 ${record.orderNo}`)
}
function onExport(record) {
  message.info(`导出 ${record.orderNo}`)
}
</script>

<style scoped>
.col-settings { min-width: 320px; max-width: 480px }
.col-item { display:flex; justify-content:space-between; align-items:center; padding:8px 4px }
.col-main { display:flex; gap:8px; align-items:center }
.col-width { display:flex; gap:8px; align-items:center }
.resizer { width:8px; height:18px; background:rgba(0,0,0,0.08); cursor:col-resize; border-radius:2px }
</style>
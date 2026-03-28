<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :btn-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.infoType" placeholder="" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="PLATFORM">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.bizType" placeholder="业务类型" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option :value="1">平台佣金收入</a-select-option>
              <a-select-option :value="2">提现支出</a-select-option>
              <a-select-option :value="3">佣金支出</a-select-option>
              <a-select-option :value="4">充值收入</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.accountType" placeholder="账户类型" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option :value="1">钱包账户</a-select-option>
              <a-select-option :value="2">用途账户</a-select-option>
            </a-select>
          </a-form-item>
          <ag-input v-model="searchData.infoId" placeholder="角色ID" />
          <ag-input v-model="searchData.id" placeholder="流水号" />
          <ag-input v-model="searchData.relaBizOrderId" placeholder="关联业务订单" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="orderId"
        @btn-load-close="btnLoading = false"
      >
        <template #bizTypeSlot="{ record }">
          {{
            record.bizType === 1
              ? '平台佣金收入'
              : record.bizType === 2
                ? '提现支出'
                : record.bizType === 3
                  ? '佣金支出'
                  : record.bizType === 4
                    ? '充值收入'
                    : ''
          }}
        </template>
        <template #infoNameSlot="{ record }">
          <span v-if="record.infoType === 'PLATFORM'">
            {{
              record.infoId === 'PLATFORM_PROFIT'
                ? '运营平台利润账户'
                : record.infoId === 'PLATFORM_INACCOUNT'
                  ? '运营平台收入账户'
                  : ''
            }}
          </span>
          <span v-if="record.infoType === 'AGENT'"> 代理商:{{ `${record.infoName}(${record.infoId})` }} </span>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_MCH_NOTIFY_VIEW')" type="link" @click="detailFunc(record.id)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 详情弹窗 -->
    <template>
      <a-drawer
        placement="right"
        :closable="true"
        :visible="visible"
        :title="visible === true ? '流水详情' : ''"
        :drawer-style="{ overflow: 'hidden' }"
        :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
        width="40%"
        @close="onClose"
      >
        <a-row justify="space-between" type="flex">
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="流水号">
                {{ detailData.id }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="账单号">
                {{ detailData.billId }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="账户类型">
                {{ detailData.accountType === 1 ? '钱包账户' : detailData.accountType === 2 ? '用途账户' : '' }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="业务类型">
                {{
                  detailData.bizType === 1
                    ? '平台佣金收入'
                    : detailData.bizType === 2
                      ? '提现支出'
                      : detailData.bizType === 3
                        ? '佣金支出'
                        : detailData.bizType === 4
                          ? '充值收入'
                          : ''
                }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="角色名称">
                <span v-if="detailData.infoType === 'PLATFORM'">
                  {{
                    detailData.infoId === 'PLATFORM_PROFIT'
                      ? '运营平台利润账户'
                      : detailData.infoId === 'PLATFORM_INACCOUNT'
                        ? '运营平台收入账户'
                        : ''
                  }}
                </span>
                <span v-if="detailData.infoType === 'AGENT'">
                  代理商:{{ `${detailData.infoName}(${detailData.infoId})` }}
                </span>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="关联业务订单类型">
                <a-tag
                  :color="
                    detailData.relaBizOrderType === 1
                      ? 'green'
                      : detailData.relaBizOrderType === 2
                        ? 'volcano'
                        : detailData.relaBizOrderType === 3
                          ? 'blue'
                          : 'orange'
                  "
                >
                  {{
                    detailData.relaBizOrderType === 1
                      ? '支付订单'
                      : detailData.relaBizOrderType === 2
                        ? '提现订单'
                        : detailData.relaBizOrderType === 3
                          ? '分润结算订单'
                          : '未知'
                  }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="关联业务订单号">
                <a-tag color="purple">
                  {{ detailData.relaBizOrderId }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="时间">
                {{ detailData.createdAt }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="变动前余额">
                <a-tag color="green">
                  {{ detailData.beforeBalance / 100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="变动金额">
                <a-tag color="cyan">
                  {{ detailData.changeAmount / 100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="变动后余额">
                <a-tag color="pink">
                  {{ detailData.afterBalance / 100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex">
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="备注">
                {{ detailData.remark }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
        </a-row>
      </a-drawer>
    </template>
  </div>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { AgSearch, AgTable, AgTableActions, AgDateRangePicker, AgInput } from '@/components'
import { API_URL_ACCOUNT_BILL_LIST, req } from '@/api/manage'

// 表格列配置
const tableColumns = [
  { key: 'id', dataIndex: 'id', title: '流水号', width: 120, fixed: 'left' },
  { key: 'bizType', title: '业务类型', width: 160, scopedSlots: { customRender: 'bizTypeSlot' } },
  { key: 'infoName', title: '角色名称', width: 260, scopedSlots: { customRender: 'infoNameSlot' } },
  {
    key: 'beforeBalance',
    dataIndex: 'beforeBalance',
    title: '变动前账户余额',
    width: 180,
    customRender: (text) => '￥' + (text / 100).toFixed(2)
  },
  {
    key: 'changeAmount',
    dataIndex: 'changeAmount',
    title: '变动金额',
    width: 180,
    customRender: (text) => '￥' + (text / 100).toFixed(2)
  },
  {
    key: 'afterBalance',
    dataIndex: 'afterBalance',
    title: '变动后账户余额',
    width: 180,
    customRender: (text) => '￥' + (text / 100).toFixed(2)
  },
  { key: 'relaBizOrderId', dataIndex: 'relaBizOrderId', title: '关联业务订单号', width: 200 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 默认查询参数对象模板
const defaultSearchData = {
  queryDateRange: 'today',
  infoType: 'PLATFORM',
  accountType: 1
}

// 响应式数据
const infoTable = ref(null)
const btnLoading = ref(true)
const searchData = reactive({ ...defaultSearchData })
const visible = ref(false)
const detailData = reactive({})

// 查询函数
const queryFunc = () => {
  btnLoading.value = true
  infoTable.value.loadData()
}

// 对接table接口函数
const reqTableDataFunc = (params) => {
  return req.list(API_URL_ACCOUNT_BILL_LIST, params)
}

// 搜索函数
const searchFunc = () => {
  // 点击查询按钮事件
  infoTable.value.loadData()
}

// 详情函数
const detailFunc = (recordId) => {
  req.getById(API_URL_ACCOUNT_BILL_LIST, recordId).then((res) => {
    Object.assign(detailData, res)
  })
  visible.value = true
}

// 关闭函数
const onClose = () => {
  visible.value = false
}

// 组件挂载时
onMounted(() => {
  // 组件初始化时将默认数据赋值给 searchData
  Object.assign(searchData, defaultSearchData)
})
</script>

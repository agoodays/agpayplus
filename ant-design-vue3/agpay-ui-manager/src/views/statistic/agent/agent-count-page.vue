<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker v-model:value="searchData.queryDateRange" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.agentNo" label="代理商号" placeholder="请输入代理商号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.agentName" label="代理商名称" placeholder="请输入代理商名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" label="服务商号" placeholder="请输入服务商号" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      
      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
        :initial-statistics="countInitData"
        row-key="agentNo"
        :show-download="true"
        :enable-statistics="true"
      >
        <template #statistics="{ data }">
          <div class="data-statistics" style="background: rgb(250, 250, 250)">
            <div class="statistics-list">
              <div class="item">
                <div class="title">总交易金额</div>
                <div class="amount" style="color: rgb(26, 102, 255)">
                  <span class="amount-num">{{ data.payAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总交易笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ data.payCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">退款金额</div>
                <div class="amount">
                  <span class="amount-num">{{ data.refundAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">退款笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ data.refundCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">支付成功率</div>
                <div class="amount" style="color: rgb(250, 173, 20)">
                  <span class="amount-num">{{ (data.round * 100).toFixed(2) }}%</span>
                </div>
              </div>
            </div>
          </div>
        </template>

        <template #payAmountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="支付成功的订单金额，不包含退款和全额退款的订单">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #amountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="实际收入=支付金额-手续费">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #feeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="支付手续费=支付金额*费率">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #refundFeeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="退款手续费=退款金额*费率">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #refundCountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="实际退款笔数=退款订单数-全额退款订单数">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #roundTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="支付成功数/总订单数得出的百分比">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>

        <template #payAmountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.payAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #amountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ ((record.payAmount - record.fee) / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #feeSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.fee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #refundAmountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.refundAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #refundFeeSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.refundFee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #refundCountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b></template
        >
        <!-- 自定义列 -->
        <template #countSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b></template
        >
        <!-- 自定义列 -->
        <template #roundSlot="{ record }"
          ><b style="color: rgb(255, 136, 0)">{{ (record.round * 100).toFixed(2) }}%</b></template
        >
        <!-- 自定义列 -->
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_STATISTIC_MCH')" type="link" @click="detailFunc(record.agentNo)">商户统计</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
  </div>
</template>
<script setup>
/**
 * 代理商交易统计页面组件
 * 功能：展示代理商交易统计数据，支持搜索、导出和查看商户统计详情
 */
import { InfoCircleOutlined } from '@ant-design/icons-vue'
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { downloadExcel } from '@/lib/ag-axios'

const icons = { InfoCircleOutlined }

// 权限检查
const { hasPermission } = usePermission()

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'agentName', dataIndex: 'agentName', title: '代理商名称', width: 160, fixed: 'left', ellipsis: true },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'payAmount', title: '交易金额', width: 110, ellipsis: true, customRender: 'payAmountSlot' },
  { key: 'amount', title: '实际收入', width: 110, customRender: 'amountSlot' },
  { key: 'fee', title: '手续费', width: 110, customRender: 'feeSlot' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  { key: 'refundFee', title: '退款手续费', width: 125, customRender: 'refundFeeSlot' },
  { key: 'refundCount', title: '退款笔数', width: 110, customRender: 'refundCountSlot' },
  { key: 'count', title: '成功/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot' },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const route = useRoute()
const router = useRouter()

const tableRef = ref(null)
const loading = ref(false)
const queryDateRange = route.query.queryDateRange || 'today'
const isvNo = route.query.isvNo || ''
const detailQueryDateRange = ref(queryDateRange)

const defaultSearchData = {
  method: 'agent',
  isvNo,
  queryDateRange
}

const searchData = reactive({ ...defaultSearchData })

const countInitData = {
  allAmount: 0.0,
  allCount: 0,
  payAmount: 0.0,
  payCount: 0,
  fee: 0.0,
  refundAmount: 0.0,
  refundCount: 0,
  refundFeeAmount: 0.0,
  round: 0.0
}

const reqTableDataFunc = (params) => statisticApi.queryOrderStatistic(params)

const reqDownloadDataFunc = (params) => {
  downloadExcel(statisticApi.exportExcel(params), '代理商统计.xlsx')
}

const searchFunc = () => {
  loading.value = true
  detailQueryDateRange.value = searchData.queryDateRange
  tableRef.value?.reload()
}

const detailFunc = (agentNo) => {
  router.push({
    path: '/statistic/mch',
    query: { agentNo, queryDateRange: detailQueryDateRange.value }
  })
}
</script>
<style lang="less" scoped>
.order-list {
  -webkit-text-size-adjust: none;
  font-size: 12px;
  display: flex;
  flex-direction: column;

  p {
    white-space: nowrap;
    span {
      display: inline-block;
      font-weight: 800;
      height: 16px;
      line-height: 16px;
      width: 35px;
      border-radius: 5px;
      text-align: center;
      margin-right: 2px;
    }
  }
}

.modal-title,
.modal-describe {
  text-align: center;
  margin-bottom: 15px;
}

.modal-title {
  margin-bottom: 20px;
  text-align: center;
  font-size: 18px;
  font-weight: 600;
}

.close {
  position: absolute;
  left: 0;
  bottom: 0;
  width: 100%;
  border-top: 1px solid #efefef;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px 0;
}

.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
}

.icon {
  width: 15px;
  height: 14px;
  margin-bottom: 3px;
}

.data-statistics {
  margin: 0 30px 10px;
  padding: 28px 0 32px;
  border-radius: 3px;
  border: 1px solid #ebebeb;
  transform: translateY(-10px);
}

.statistics-list {
  display: flex;
  flex-direction: row;
  justify-content: space-around;
}

.statistics-list .item .title {
  color: gray;
  margin-bottom: 10px;
}

.statistics-list .item .amount {
  margin-bottom: 10px;
  max-width: 150px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.statistics-list .item .amount .amount-num {
  padding-right: 3px;
  font-weight: 600;
  font-size: 20px;
}

.statistics-list .item .symbol {
  padding-right: 3px;
}

.statistics-list .item .detail-text {
  color: rgb(26, 102, 255);
  padding-left: 5px;
  cursor: pointer;
}

.statistics-list .line {
  width: 1px;
  height: 100%;
  border-right: 1px solid #efefef;
}
</style>

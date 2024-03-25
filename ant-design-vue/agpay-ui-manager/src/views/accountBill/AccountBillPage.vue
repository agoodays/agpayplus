<template>
  <div>
    <a-card>
      <AgSearchForm
        :searchData="searchData"
        :openIsShowMore="true"
        :isShowMore="isShowMore"
        :btnLoading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc">
        <template slot="formItem">
          <a-form-item label="" class="table-head-layout">
            <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
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
              <a-select-option value="1">订单佣金计算</a-select-option>
              <a-select-option value="2">退款轧差</a-select-option>
              <a-select-option value="3">佣金提现</a-select-option>
              <a-select-option value="4">人工调账</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.accountType" placeholder="账户类型" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="1">钱包账户</a-select-option>
              <a-select-option value="2">在途账户</a-select-option>
            </a-select>
          </a-form-item>
          <ag-text-up :placeholder="'角色ID'" :msg="searchData.infoId" v-model="searchData.infoId" />
          <ag-text-up :placeholder="'流水号'" :msg="searchData.id" v-model="searchData.id" />
          <ag-text-up :placeholder="'关联订单号'" :msg="searchData.relaBizOrderId" v-model="searchData.relaBizOrderId" />
        </template>
      </AgSearchForm>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="true"
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="orderId"
      >
        <template slot="bizTypeSlot" slot-scope="{record}">
          {{ record.bizType === 1 ? '订单佣金计算' : (record.bizType === 2 ? '退款轧差' : (record.bizType === 3 ? '佣金提现' : (record.bizType === 4 ? '人工调账' : ''))) }}
        </template>
        <template slot="infoNameSlot" slot-scope="{record}">
          <span v-if="record.infoType === 'PLATFORM'">
            {{ record.infoId === 'PLATFORM_PROFIT' ? '运营平台利润账户' : (record.infoId === 'PLATFORM_INACCOUNT' ? '运营平台入账账户' : '') }}
          </span>
          <span v-if="record.infoType === 'AGENT'">
            代理商：{{ `${record.infoName}(${record.infoId})` }}
          </span>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_MCH_NOTIFY_VIEW')" @click="detailFunc(record.id)">详情</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 详情抽屉 -->
    <template>
      <a-drawer
        placement="right"
        :closable="true"
        :visible="visible"
        :title="visible === true? '流水详情':''"
        @close="onClose"
        :drawer-style="{ overflow: 'hidden' }"
        :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
        width="40%"
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
                {{ detailData.accountType === 1 ? '钱包账户' : (detailData.accountType === 2 ? '在途账户' : '') }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="业务类型">
                {{ detailData.bizType === 1 ? '订单佣金计算' : (detailData.bizType === 2 ? '退款轧差' : (detailData.bizType === 3 ? '佣金提现' : (detailData.bizType === 4 ? '人工调账' : ''))) }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="角色名称">
                <span v-if="detailData.infoType === 'PLATFORM'">
                  {{ detailData.infoId === 'PLATFORM_PROFIT' ? '运营平台利润账户' : (detailData.infoId === 'PLATFORM_INACCOUNT' ? '运营平台入账账户' : '') }}
                </span>
                <span v-if="detailData.infoType === 'AGENT'">
                  代理商：{{ `${detailData.infoName}(${detailData.infoId})` }}
                </span>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="关联订单类型">
                <a-tag :color="detailData.relaBizOrderType === 1? 'green' : (detailData.relaBizOrderType === 2 ? 'volcano' : (detailData.relaBizOrderType === 3 ? 'blue' : 'orange'))">
                  {{ detailData.relaBizOrderType === 1? '支付订单' : (detailData.relaBizOrderType === 2 ? '退款订单' : (detailData.relaBizOrderType === 3 ? '提现申请订单' : '未知')) }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="关联订单号">
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
                  {{ detailData.beforeBalance/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="变动金额">
                <a-tag color="cyan">
                  {{ detailData.changeAmount/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="变动后余额">
                <a-tag color="green">
                  {{ detailData.afterBalance/100 }}
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
<script>
  import AgSearchForm from '@/components/AgSearch/AgSearchForm'
  import AgTable from '@/components/AgTable/AgTable'
  import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
  import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
  import AgTableColumns from '@/components/AgTable/AgTableColumns'
  import { API_URL_ACCOUNT_BILL_LIST, req } from '@/api/manage'

  // eslint-disable-next-line no-unused-vars
  const tableColumns = [
    { key: 'id', dataIndex: 'id', title: '流水号', width: 120, fixed: 'left' },
    { key: 'bizType', title: '业务类型', width: 160, scopedSlots: { customRender: 'bizTypeSlot' } },
    { key: 'infoName', title: '角色名称', width: 130, scopedSlots: { customRender: 'infoNameSlot' } },
    { key: 'beforeBalance', dataIndex: 'beforeBalance', title: '变动前账户余额', width: 120 },
    { key: 'changeAmount', dataIndex: 'changeAmount', title: '变动金额', width: 120 },
    { key: 'afterBalance', dataIndex: 'afterBalance', title: '变动后账户余额', width: 120 },
    { key: 'relaBizOrderId', dataIndex: 'relaBizOrderId', title: '关联订单号', width: 200 },
    { key: 'createdAt', dataIndex: 'createdAt', title: '时间', width: 200 },
    { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
  ]

  export default {
    name: 'AccountBillPage',
    components: { AgSearchForm, AgTable, AgTableColumns, AgDateRangePicker, AgTextUp },
    data () {
      return {
        isShowMore: false,
        btnLoading: true,
        tableColumns: tableColumns,
        searchData: {
          queryDateRange: 'today',
          infoType: 'PLATFORM',
          accountType: 1
        },
        visible: false,
        detailData: {}
      }
    },
    computed: {
    },
    mounted () {
    },
    methods: {
      handleSearchFormData (searchData) {
        this.searchData = searchData
      },
      setIsShowMore (isShowMore) {
        this.isShowMore = isShowMore
      },
      queryFunc () {
        this.btnLoading = true
        this.$refs.infoTable.refTable(true)
      },
      // 请求table接口数据
      reqTableDataFunc: (params) => {
        return req.list(API_URL_ACCOUNT_BILL_LIST, params)
      },
      searchFunc: function () { // 点击【查询】按钮点击事件
        this.$refs.infoTable.refTable(true)
      },
      detailFunc: function (recordId) {
        const that = this
        req.getById(API_URL_ACCOUNT_BILL_LIST, recordId).then(res => {
          that.detailData = res
        })
        this.visible = true
      },
      onClose () {
        this.visible = false
      }
    }
  }
</script>

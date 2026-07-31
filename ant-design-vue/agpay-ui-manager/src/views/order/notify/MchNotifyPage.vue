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
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <ag-text-up :placeholder="'订单ID'" :msg="searchData.orderId" v-model="searchData.orderId" />
          <ag-text-up :placeholder="'商户订单号'" :msg="searchData.mchOrderNo" v-model="searchData.mchOrderNo" />
          <!-- <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" /> -->
          <a-form-item label="" class="table-head-layout">
            <ag-select
              v-model="searchData.mchNo"
              :api="searchMch"
              valueField="mchNo"
              labelField="mchName"
              placeholder="商户号（搜索商户名称）"
            />
          </a-form-item>
          <ag-text-up :placeholder="'服务商号'" :msg="searchData.isvNo" v-model="searchData.isvNo" />
          <ag-text-up v-if="isShowMore" :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId"/>
          <a-form-item v-if="isShowMore" label="" class="table-head-layout">
            <a-select v-model="searchData.state" placeholder="通知状态" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="1">通知中</a-select-option>
              <a-select-option value="2">通知成功</a-select-option>
              <a-select-option value="3">通知失败</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item v-if="isShowMore" label="" class="table-head-layout">
            <a-select v-model="searchData.orderType" placeholder="订单类型" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="1">支付</a-select-option>
              <a-select-option value="2">退款</a-select-option>
              <a-select-option value="3">转账</a-select-option>
            </a-select>
          </a-form-item>
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
        <template slot="stateSlot" slot-scope="{record}">
          <a-tag
            :key="record.state"
            :color="record.state === 1?'orange':record.state === 2?'green':'volcano'"
          >
            {{ record.state === 1?'通知中':record.state === 2?'通知成功':record.state === 3?'通知失败':'未知' }}
          </a-tag>
        </template>
        <template slot="orderTypeSlot" slot-scope="{record}">
          <a-tag
            :key="record.orderType"
            :color="record.orderType === 1?'green':record.orderType === 2?'volcano': record.orderType === 3? 'blue': 'orange'"
          >
            {{ record.orderType === 1?'支付':record.orderType === 2?'退款':record.orderType === 3? '转账':'未知' }}
          </a-tag>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_MCH_NOTIFY_VIEW')" @click="detailFunc(record.notifyId)">详情</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_NOTIFY_RESEND') && record.state === 3" @click="resendFunc(record.notifyId)">重发通知</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 通知详情抽屉 -->
    <template>
      <a-drawer
        placement="right"
        :closable="true"
        :visible="visible"
        :title="visible === true? '商户通知详情':''"
        @close="onClose"
        :drawer-style="{ overflow: 'hidden' }"
        :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
        width="40%"
      >
        <a-row justify="space-between" type="flex">
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="订单ID">
                <a-tag color="purple">
                  {{ detailData.orderId }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="服务商号">
                {{ detailData.isvNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商户订单号">
                {{ detailData.mchOrderNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商户号">
                {{ detailData.mchNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="订单类型">
                <a-tag :color="detailData.orderType === 1?'green':detailData.orderType === 2?'volcano': detailData.orderType === 3? 'blue' : 'orange'">
                  {{ detailData.orderType === 1?'支付':detailData.orderType === 2?'退款':detailData.orderType === 3 ? '转账': '未知' }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="应用APPID">
                {{ detailData.appId }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="通知状态">
                <a-tag :color="detailData.state === 1?'orange':detailData.state === 2?'green':'volcano'">
                  {{ detailData.state === 1?'通知中':detailData.state === 2?'通知成功':detailData.state === 3?'通知失败':'未知' }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="通知次数">
                {{ detailData.notifyCount }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="Http发送方式">
                <a-tag :color="'green'">
                  {{ detailData.reqMethod }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="Http媒体类型">
                <a-tag :color="'green'">
                  {{ detailData.reqMediaType }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="24">
            <a-descriptions>
              <a-descriptions-item label="最后通知时间">
                {{ detailData.lastNotifyTime }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="创建时间">
                {{ detailData.createdAt }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="更新时间">
                {{ detailData.updatedAt }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="24">
            <a-form-model-item label="通知地址">
              <a-input
                type="textarea"
                disabled="disabled"
                style="height: 100px;color: black"
                v-model="detailData.notifyUrl"
              />
            </a-form-model-item>
          </a-col>
          <a-col :sm="24">
            <a-form-model-item label="请求Body">
              <a-input
                type="textarea"
                disabled="disabled"
                style="height: 100px;color: black"
                v-model="detailData.reqBody"
              />
            </a-form-model-item>
          </a-col>
          <a-col :sm="24">
            <a-form-model-item label="响应结果">
              <a-input
                type="textarea"
                disabled="disabled"
                style="height: 100px;color: black"
                v-model="detailData.resResult"
              />
            </a-form-model-item>
          </a-col>
        </a-row>
      </a-drawer>
    </template>
  </div>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgSelect from '@/components/AgSelect/AgSelect'
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_MCH_NOTIFY_LIST, API_URL_MCH_LIST, req, mchNotifyResend } from '@/api/manage'
import moment from 'moment'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'orderId', dataIndex: 'orderId', title: '订单ID', width: 210, fixed: 'left' },
  { key: 'mchOrderNo', dataIndex: 'mchOrderNo', title: '商户订单号', width: 200 },
  { key: 'state', title: '通知状态', width: 130, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'orderType', title: '订单类型', width: 130, scopedSlots: { customRender: 'orderTypeSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]
// 默认查询条件数据
const defaultSearchData = {
  queryDateRange: 'today' // 查询时间范围，默认今天
}

export default {
  name: 'MchNotifyList',
  components: { AgSearchForm, AgTable, AgSelect, AgTableColumns, AgDateRangePicker, AgTextUp },
  data () {
    return {
      isShowMore: false,
      btnLoading: true,
      tableColumns: tableColumns,
      searchData: defaultSearchData,
      createdStart: '', // 选择开始时间
      createdEnd: '', // 选择结束时间
      visible: false,
      detailData: {}
    }
  },
  computed: {
  },
  mounted () {
  },
  methods: {
    searchMch (params) {
      return req.list(API_URL_MCH_LIST, params)
    },
    handleSearchFormData (searchData) {
      // 如果是空对象或者为null/undefined
      if (!searchData || Object.keys(searchData).length === 0) {
        this.searchData = { ...defaultSearchData }
      } else {
        this.searchData = { ...searchData }
      }
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
      return req.list(API_URL_MCH_NOTIFY_LIST, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    detailFunc: function (recordId) {
      const that = this
      req.getById(API_URL_MCH_NOTIFY_LIST, recordId).then(res => {
        that.detailData = res
      })
      this.visible = true
    },
    moment,
    onChange (date, dateString) {
      this.searchData.createdStart = dateString[0] // 开始时间
      this.searchData.createdEnd = dateString[1] // 结束时间
    },
    disabledDate (current) { // 今日之后日期不可选
      return current && current > moment().endOf('day')
    },
    onClose () {
      this.visible = false
    },
    resendFunc (notifyId) { // 重发通知
      const that = this

      this.$infoBox.confirmPrimary('确认重发通知？', '', () => {
        mchNotifyResend(notifyId).then(res => {
          that.$message.success('任务更新成功，请稍后查看最新状态！')
          that.searchFunc()
        })
      })
    }
  }
}
</script>

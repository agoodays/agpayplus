<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="orderNoType" @change="orderNoTypeChange">
                <a-select-option :value="'payOrderId'">支付订单号</a-select-option>
                <a-select-option :value="'mchOrderNo'">商户订单号</a-select-option>
                <a-select-option :value="'channelOrderNo'" >渠道订单号</a-select-option>
                <a-select-option :value="'platformOrderNo'">用户支付凭证交易单号</a-select-option>
                <a-select-option :value="'platformMchOrderNo'">用户支付凭证商户单号</a-select-option>
              </a-select>
            </a-form-item>
<!--            <ag-text-up :placeholder="'支付/商户/渠道订单号'" :msg="searchData.unionOrderId" v-model="searchData.unionOrderId" />-->
            <ag-text-up v-show="orderNoType==='payOrderId'" :placeholder="'支付订单号'" :msg="searchData.payOrderId" v-model="searchData.payOrderId" />
            <ag-text-up v-show="orderNoType==='mchOrderNo'" :placeholder="'商户订单号'" :msg="searchData.mchOrderNo" v-model="searchData.mchOrderNo" />
            <ag-text-up v-show="orderNoType==='channelOrderNo'" :placeholder="'渠道订单号'" :msg="searchData.channelOrderNo" v-model="searchData.channelOrderNo" />
            <ag-text-up v-show="orderNoType==='platformOrderNo'" :placeholder="'用户支付凭证交易单号'" :msg="searchData.platformOrderNo" v-model="searchData.platformOrderNo" />
            <ag-text-up v-show="orderNoType==='platformMchOrderNo'" :placeholder="'用户支付凭证商户单号'" :msg="searchData.platformMchOrderNo" v-model="searchData.platformMchOrderNo" />
            <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId"/>
            <a-form-item v-if="$access('ENT_PAY_ORDER_SEARCH_PAY_WAY')" label="" class="table-head-layout">
              <a-select v-model="searchData.wayCode" placeholder="支付方式" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option :key="item.wayCode" v-for="item in payWayList" :value="item.wayCode">
                  {{ item.wayName }}
                </a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="支付状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">订单生成</a-select-option>
                <a-select-option value="1">支付中</a-select-option>
                <a-select-option value="2">支付成功</a-select-option>
                <a-select-option value="3">支付失败</a-select-option>
                <a-select-option value="4">已撤销</a-select-option>
                <a-select-option value="5">已退款</a-select-option>
                <a-select-option value="6">订单关闭</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.divisionState" placeholder="分账状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">未发生分账</a-select-option>
                <a-select-option value="1">等待分账任务处理</a-select-option>
                <a-select-option value="2">分账处理中</a-select-option>
                <a-select-option value="3">分账任务已结束（状态请看分账记录）</a-select-option>
              </a-select>
            </a-form-item>
            <ag-text-up v-if="isShowMore" :placeholder="'门店ID'" :msg="searchData.storeId" v-model="searchData.storeId"/>
            <ag-text-up v-if="isShowMore" :placeholder="'门店名称'" :msg="searchData.storeName" v-model="searchData.storeName"/>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">搜索</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line">
        <div class="btns" @click="isShowMore = !isShowMore">
          <div>
            {{ isShowMore ? '收起' : '更多' }}筛选 <a-icon :type="isShowMore ? 'up' : 'down'" />
          </div>
        </div>
      </div>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="true"
        :autoRefresh="true"
        :isShowAutoRefresh="true"
        :isShowDownload="true"
        :isEnableDataStatistics="true"
        :reqTableDataFunc="reqTableDataFunc"
        :reqDownloadDataFunc="reqDownloadDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="payOrderId"
        :tableRowCrossColor="true"
      >
        <template slot="dataStatisticsSlot">
          <div class="data-statistics" style="background: rgb(250, 250, 250);">
            <div class="statistics-list">
              <div class="item">
                <div class="title">实际收款金额</div>
                <div class="amount" style="color: rgb(26, 102, 255);">
                  <span class="amount-num">{{ (countData.payAmount-countData.mchFeeAmount).toFixed(2) }}</span>元
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">成交订单</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.payAmount.toFixed(2) }}</span>元
                </div>
                <div class="detail">
                  <span>{{ countData.payCount }}笔</span>
                  <span class="detail-text" @click="detailVisible = true">明细</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">手续费金额</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.mchFeeAmount.toFixed(2) }}</span>元
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">退款订单</div>
                <div class="amount" style="color: rgb(250, 173, 20);">
                  <span class="amount-num">{{ countData.refundAmount.toFixed(2) }}</span>元
                </div>
                <div class="detail">
                  <span>{{ countData.refundCount }}笔</span>
                </div>
              </div>
            </div>
          </div>
        </template>
        <template slot="amountSlot" slot-scope="{record}"><b>￥{{ record.amount/100 }}</b></template> <!-- 自定义插槽 -->
        <template slot="refundAmountSlot" slot-scope="{record}">￥{{ record.refundAmount/100 }}</template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-tag
            :key="record.state"
            :color="record.state === 0?'blue':record.state === 1?'orange':record.state === 2?'green':record.state === 6?'':'volcano'"
          >
            {{ record.state === 0?'订单生成':record.state === 1?'支付中':record.state === 2?'支付成功':record.state === 3?'支付失败':record.state === 4?'已撤销':record.state === 5?'已退款':record.state === 6?'订单关闭':'未知' }}
          </a-tag>
        </template>

        <template slot="divisionStateSlot" slot-scope="{record}">
          <span v-if="record.divisionState == 0">-</span>
          <a-tag color="orange" v-else-if="record.divisionState == 1">待分账</a-tag>
          <a-tag color="red" v-else-if="record.divisionState == 2">分账处理中</a-tag>
          <a-tag color="green" v-else-if="record.divisionState == 3">任务已结束</a-tag>
          <span v-else>未知</span>
        </template>

        <template slot="orderSlot" slot-scope="{record}">
          <div class="order-list">
            <p><span style="color:#729ED5;background:#e7f5f7">支付</span>{{ record.payOrderId }}</p>
            <p style="margin-bottom: 0">
              <span style="color:#56cf56;background:#d8eadf">商户</span>
              <a-tooltip placement="bottom" style="font-weight: normal;" v-if="record.mchOrderNo.length > record.payOrderId.length">
                <template slot="title">
                  <span>{{ record.mchOrderNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.mchOrderNo, record.payOrderId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.mchOrderNo }}</span>
            </p>
            <p v-if="record.channelOrderNo" style="margin-bottom: 0;margin-top: 10px">
              <span style="color:#fff;background:#E09C4D">渠道</span>
              <a-tooltip placement="bottom" style="font-weight: normal;" v-if="record.channelOrderNo.length > record.payOrderId.length">
                <template slot="title">
                  <span>{{ record.channelOrderNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.channelOrderNo, record.payOrderId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.channelOrderNo }}</span>
            </p>
          </div>
        </template>

        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_PAY_ORDER_VIEW')" @click="detailFunc(record.payOrderId)">详情</a-button>
            <a-button type="link" v-if="$access('ENT_PAY_ORDER_REFUND')" style="color: red" v-show="(record.state === 2 && record.refundState !== 2)" @click="openFunc(record, record.payOrderId)">退款</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 退款弹出框 -->
    <refund-modal ref="refundModalInfo" :callbackFunc="searchFunc"></refund-modal>
    <!-- 成交订单详细 -->
    <a-modal :visible="detailVisible" footer="" @cancel="detailVisible = false">
      <div class="modal-title">成交订单详细</div>
      <div class="modal-describe">创建订单金额/笔数 = 成交订单金额/笔数 + 未付款订单金额/笔数</div>
      <div class="statistics-list" style="padding-bottom: 55px;">
        <div class="item">
          <div class="title">创建订单</div>
          <a-tooltip placement="top">
            <template #title>
              <span>
                <span class="amount-num">{{ countData.allPayAmount.toFixed(2) }}</span>
                <span>元</span>
              </span>
            </template>
            <div class="amount">
              <span>
                <span class="amount-num">{{ countData.allPayAmount.toFixed(2) }}</span>
                <span>元</span>
              </span>
            </div>
          </a-tooltip>
          <div class="detail">
            <span>{{ countData.allPayCount }}笔</span>
          </div>
        </div>
        <div class="item">
          <div class="line"></div>
          <div class="title"></div>
        </div>
        <div class="item">
          <div class="title">成交订单</div>
          <a-tooltip placement="top">
            <template #title>
              <span>
                <span class="amount-num">{{ countData.payAmount.toFixed(2) }}</span>
                <span>元</span>
              </span>
            </template>
            <div class="amount">
              <span>
                <span class="amount-num">{{ countData.payAmount.toFixed(2) }}</span>
                <span>元</span>
              </span>
            </div>
          </a-tooltip>
          <div class="detail">
            <span>{{ countData.payCount }}笔</span>
          </div>
        </div>
        <div class="item">
          <div class="line"></div>
          <div class="title"></div>
        </div>
        <div class="item">
          <div class="title">未付款订单</div>
          <a-tooltip placement="top">
            <template #title>
              <span>
                <span class="amount-num">{{ countData.failPayAmount.toFixed(2) }}</span>
                <span>元</span>
              </span>
            </template>
            <div class="amount">
              <span>
                <span class="amount-num">{{ countData.failPayAmount.toFixed(2) }}</span>
                <span>元</span>
              </span>
            </div>
          </a-tooltip>
          <div class="detail">
            <span>{{ countData.failPayCount }}笔</span>
          </div>
        </div>
      </div>
      <div class="close">
        <a-button type="primary" @click="detailVisible = false">知道了</a-button>
      </div>
    </a-modal>
    <!-- 日志详情抽屉 -->
    <template>
      <a-drawer
        width="50%"
        placement="right"
        :closable="true"
        :visible="visible"
        :title="visible === true? '订单详情':''"
        @close="onClose"
        :drawer-style="{ overflow: 'hidden' }"
        :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
      >
        <a-row justify="space-between" type="flex">
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="服务商号">
                {{ detailData.isvNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付订单号">
                <a-tag color="purple">
                  {{ detailData.payOrderId }}
                </a-tag>
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
              <a-descriptions-item label="商户订单号">
                {{ detailData.mchOrderNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商户名称">
                {{ detailData.mchName }}
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
              <a-descriptions-item label="订单状态">
                <a-tag :color="detailData.state === 0?'blue':detailData.state === 1?'orange':detailData.state === 2?'green':detailData.state === 6?'':'volcano'">
                  {{ detailData.state === 0?'订单生成':detailData.state === 1?'支付中':detailData.state === 2?'支付成功':detailData.state === 3?'支付失败':detailData.state === 4?'已撤销':detailData.state === 5?'已退款':detailData.state === 6?'订单关闭':'未知' }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付金额">
                <a-tag color="green">
                  {{ detailData.amount/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions><a-descriptions-item label="手续费"><a-tag color="pink">{{ detailData.mchFeeAmount/100 }}</a-tag></a-descriptions-item></a-descriptions>
          </a-col>
          <a-col :sm="12">
<!--            <a-descriptions><a-descriptions-item label="商家费率">{{ (detailData.mchFeeRate*100).toFixed(2) }}%</a-descriptions-item></a-descriptions>-->
            <a-descriptions><a-descriptions-item label="商家费率">{{ detailData.mchFeeRateDesc }}</a-descriptions-item></a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付错误码">
                {{ detailData.errCode }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付错误描述">
                {{ detailData.errMsg }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="订单失效时间">
                {{ detailData.expiredTime }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付成功时间">
                {{ detailData.successTime }}
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
          <a-divider />
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商品标题">
                {{ detailData.subject }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商品描述">
                {{ detailData.body }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="接口代码">
                {{ detailData.ifCode }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="货币代码">
                {{ detailData.currency }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付方式">
                {{ detailData.wayCode }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="客户端IP">
                {{ detailData.clientIp }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="用户标识">
                {{ detailData.channelUser }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="渠道订单号">
                {{ detailData.channelOrderNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="用户支付凭证交易单号">
                {{ detailData.platformOrderNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="用户支付凭证商户单号">
                {{ detailData.platformMchOrderNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="异步通知地址">
                {{ detailData.notifyUrl }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="页面跳转地址">
                {{ detailData.returnUrl }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="退款次数">
                {{ detailData.refundTimes }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="退款总额">
                <a-tag color="cyan" v-if="detailData.refundAmount">
                  {{ detailData.refundAmount/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>

          <a-divider />
          <a-col :sm="12">
            <a-descriptions><a-descriptions-item label="订单分账模式">
              <span v-if="detailData.divisionMode == 0">该笔订单不允许分账</span>
              <span v-else-if="detailData.divisionMode == 1">支付成功按配置自动完成分账</span>
              <span v-else-if="detailData.divisionMode == 2">商户手动分账(解冻商户金额)</span>
              <span v-else>未知</span>
            </a-descriptions-item></a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions><a-descriptions-item label="分账状态">
              <a-tag color="blue" v-if="detailData.divisionState == 0">未发生分账</a-tag>
              <a-tag color="orange" v-else-if="detailData.divisionState == 1">待分账</a-tag>
              <a-tag color="red" v-else-if="detailData.divisionState == 2">分账处理中</a-tag>
              <a-tag color="green" v-else-if="detailData.divisionState == 3">任务已结束</a-tag>
              <a-tag color="#f50" v-else>未知</a-tag>
            </a-descriptions-item></a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions><a-descriptions-item label="最新分账发起时间">{{ detailData.divisionLastTime }}</a-descriptions-item></a-descriptions>
          </a-col>
        </a-row>
        <a-divider />
        <a-row justify="start" type="flex">
          <a-col :sm="24">
            <a-form-model-item label="扩展参数">
              <a-input
                type="textarea"
                disabled="disabled"
                style="height: 100px;color: black"
                v-model="detailData.extParam"
              />
            </a-form-model-item>
          </a-col>
        </a-row>
      </a-drawer>
    </template>
  </page-header-wrapper>
</template>
<script>
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import RefundModal from './RefundModal' // 退款弹出框
import { API_URL_PAY_ORDER_LIST, API_URL_PAYWAYS_LIST, req } from '@/api/manage'
import moment from 'moment'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'orderNo', title: '订单号', width: 210, fixed: 'left', scopedSlots: { customRender: 'orderSlot' } },
  // { key: 'payOrderId', dataIndex: 'payOrderId', title: '支付订单号' },
  // { key: 'mchOrderNo', dataIndex: 'mchOrderNo', title: '商户订单号' },
  { key: 'amount', title: '支付金额', width: 100, scopedSlots: { customRender: 'amountSlot' } },
  { key: 'refundAmount', title: '退款金额', width: 100, scopedSlots: { customRender: 'refundAmountSlot' } },
  { key: 'mchFeeAmount', dataIndex: 'mchFeeAmount', title: '手续费', width: 100, customRender: (text) => '￥' + (text / 100).toFixed(2) },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式', width: 150 },
  { key: 'state', title: '支付状态', width: 100, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'divisionState', title: '分账状态', width: 100, align: 'center', scopedSlots: { customRender: 'divisionStateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'IsvListPage',
  components: { AgTable, AgTableColumns, AgDateRangePicker, AgTextUp, RefundModal },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      orderNoType: 'payOrderId',
      searchData: {
        queryDateRange: 'today'
      },
      countData: {
        mchFeeAmount: 0.00,
        failPayAmount: 0.00,
        failPayCount: 0,
        refundCount: 0,
        allPayAmount: 0.00,
        allPayCount: 0,
        payAmount: 0.00,
        payCount: 0,
        refundAmount: 0.00
      },
      createdStart: '', // 选择开始时间
      createdEnd: '', // 选择结束时间
      visible: false,
      detailVisible: false,
      detailData: {},
      payWayList: []
    }
  },
  computed: {
  },
  mounted () {
    if (this.$access('ENT_PAY_ORDER_SEARCH_PAY_WAY')) {
      this.initPayWay()
    }
    this.countFunc()
  },
  methods: {
    queryFunc () {
      this.btnLoading = true
      this.countFunc()
      this.$refs.infoTable.refTable(true)
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_PAY_ORDER_LIST, params)
    },
    reqDownloadDataFunc: (params) => {
      req.export(API_URL_PAY_ORDER_LIST, 'excel', params).then(res => {
        // 将响应体中的二进制数据转换为Blob对象
        const blob = new Blob([res])
        const fileName = '订单列表.xlsx' // 要保存的文件名称
        if ('download' in document.createElement('a')) {
          // 非IE下载
          // 创建一个a标签，设置download属性和href属性，并触发click事件下载文件
          const elink = document.createElement('a')
          elink.download = fileName
          elink.style.display = 'none'
          elink.href = URL.createObjectURL(blob) // 创建URL.createObjectURL(blob) URL，并将其赋值给a标签的href属性
          document.body.appendChild(elink)
          elink.click()
          URL.revokeObjectURL(elink.href) // 释放URL 对象
          document.body.removeChild(elink)
        } else {
          // IE10+下载
          navigator.msSaveBlob(blob, fileName)
        }
      }).catch((error) => {
        console.error(error)
      })
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    countFunc: function () {
      const that = this
      req.count(API_URL_PAY_ORDER_LIST, this.searchData).then(res => {
        that.countData = res
      })
    },
    // 打开退款弹出框
    openFunc (record, recordId) {
      if (record.refundState === 2) {
        return this.$infoBox.modalError('订单无可退款金额', '')
      }
      this.$refs.refundModalInfo.show(recordId)
    },
    detailFunc: function (recordId) {
      const that = this
      req.getById(API_URL_PAY_ORDER_LIST, recordId).then(res => {
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
    initPayWay: function () {
      const that = this
      req.list(API_URL_PAYWAYS_LIST, { 'pageSize': -1 }).then(res => { // 支付方式下拉列表
        that.payWayList = res.records
      })
    },
    changeStr2ellipsis (orderNo, baseLength) {
      const halfLengh = parseInt(baseLength / 2)
      return orderNo.substring(0, halfLengh - 1) + '...' + orderNo.substring(orderNo.length - halfLengh, orderNo.length)
    },
    orderNoTypeChange () {
      this.searchData.payOrderId = null
      this.searchData.mchOrderNo = null
      this.searchData.channelOrderNo = null
      this.searchData.platformOrderNo = null
      this.searchData.platformMchOrderNo = null
    }
  }
}
</script>
<style lang="less" scoped>
  ///deep/ .ant-table-fixed{
  //  tr{
  //    th{
  //      padding: 0px 0px;
  //    }
  //  }
  //  }

  .order-list {
    -webkit-text-size-adjust:none;
    font-size: 12px;
    display: flex;
    flex-direction: column;

    p {
      white-space:nowrap;
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

  .modal-title,.modal-describe {
    text-align: center;
    margin-bottom: 15px
  }

  .modal-title {
    margin-bottom: 20px;
    text-align: center;
    font-size: 18px;
    font-weight: 600
  }

  .close {
    position: absolute;
    left: 0;
    bottom: 0;
    width: 100%;
    border-top: 1px solid #EFEFEF;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 10px 0
  }

  .data-statistics {
    margin: 0 30px 10px;
    padding: 28px 0 32px;
    border-radius: 3px;
    border: 1px solid #ebebeb;
    transform: translateY(-10px)
  }

  .statistics-list {
    display: flex;
    flex-direction: row;
    justify-content: space-around
  }

  .statistics-list .item .title {
    color: gray;
    margin-bottom: 10px
  }

  .statistics-list .item .amount {
    margin-bottom: 10px
  }

  .statistics-list .item .amount .amount-num {
    padding-right: 3px;
    font-weight: 600;
    font-size: 20px
  }

  .statistics-list .item .symbol {
    padding-right: 3px
  }

  .statistics-list .item .detail-text {
    color: rgb(26, 102, 255);
    padding-left: 5px;
    cursor: pointer
  }

  .statistics-list .line {
    width: 1px;
    height: 100%;
    border-right: 1px solid #efefef
  }
</style>

<template>
  <div>
    <a-card>
      <div v-if="$access('ENT_DIVISION_RECEIVER_LIST')" class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <!-- <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" /> -->
            <a-form-item label="" class="table-head-layout">
              <ag-select
                v-model="searchData.mchNo"
                :api="searchMch"
                value-field="mchNo"
                label-field="mchName"
                placeholder="商户号(支持按商户名称搜索)"
              />
            </a-form-item>
            <ag-input v-model="searchData.appId" placeholder="应用ID[精确]" />
            <ag-input v-model="searchData.receiverId" placeholder="收款账户ID[精确]" />
            <ag-input v-model="searchData.receiverAlias" placeholder="收款账户别名[模糊]" />
            <ag-input v-model="searchData.receiverGroupId" placeholder="分组ID[精确]" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="账户状态(系统默认)" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="1">正常可用</a-select-option>
                <a-select-option value="0">暂停使用</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.ifCode" placeholder="支付接口">
                <a-select-option value="">全部</a-select-option>
                <a-select-option v-for="item in ifDefineList" :key="item.ifCode">
                  <span class="icon-style" :style="{ backgroundColor: item.bgColor }"
                    ><img class="icon" :src="item.icon" alt=""
                  /></span>
                  {{ item.ifName }}[{{ item.ifCode }}]
                </a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" :loading="btnLoading" @click="searchFunc">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => (searchData = {})">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line" />
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="receiverId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_ADD')"
              type="primary"
              icon="plus"
              class="mg-b-30"
              @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #receiverIdSlot="{ record }">
          <b v-if="!$access('ENT_DIVISION_RECEIVER_VIEW')">{{ record.receiverId }}</b>
          <a v-if="$access('ENT_DIVISION_RECEIVER_VIEW')" @click="detailFunc(record.receiverId)"
            ><b>{{ record.receiverId }}</b></a
          >
        </template>
        <!-- 支付接口 -->
        <template #ifCodeSlot="{ record }">
          <a-tooltip placement="bottom" style="font-weight: normal">
            <template #title>
              <span
                class="icon-style"
                :style="{ backgroundColor: ifDefineList.find((f) => f.ifCode === record.ifCode).bgColor }"
              >
                <img class="icon" :src="ifDefineList.find((f) => f.ifCode === record.ifCode).icon" alt="" />
              </span>
              {{ ifDefineList.find((f) => f.ifCode === record.ifCode).ifName }}[{{
                ifDefineList.find((f) => f.ifCode === record.ifCode).ifCode
              }}]
            </template>
            <span v-if="record.ifCode">
              <span
                class="icon-style"
                :style="{ backgroundColor: ifDefineList.find((f) => f.ifCode === record.ifCode).bgColor }"
              >
                <img class="icon" :src="ifDefineList.find((f) => f.ifCode === record.ifCode).icon" alt="" />
              </span>
              {{ ifDefineList.find((f) => f.ifCode === record.ifCode).ifName }}[{{
                ifDefineList.find((f) => f.ifCode === record.ifCode).ifCode
              }}]
            </span>
          </a-tooltip>
        </template>
        <!-- 状态(系统默认) -->
        <template #stateSlot="{ record }">
          <a-badge
            :status="record.state === 0 ? 'error' : 'processing'"
            :text="record.state === 0 ? '暂停使用' : '正常可用'"
          />
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_DIVISION_RECEIVER_EDIT')" type="link" @click="editFunc(record.receiverId)"
              >编辑</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
      <!-- 新增收款账户页面  -->
      <ReceiverAdd
        v-if="showReceiverAdd"
        ref="receiverAdd"
        :callback-func="searchFunc"
        @close="showReceiverAdd = false"
      />
      <!-- 编辑 页面弹窗  -->
      <ReceiverEdit
        v-if="showReceiverEdit"
        ref="receiverEdit"
        :callback-func="searchFunc"
        @close="showReceiverEdit = false"
      />
      <Detail v-if="showDetail" ref="recordDetail" @close="showDetail = false" />
    </a-card>
  </div>
</template>
<script setup>
import { ref, reactive, onMounted, nextTick, defineAsyncComponent } from 'vue'
import { AgTable, AgTableActions, AgSelect, AgInput } from '@/components'
import { API_URL_DIVISION_RECEIVER, API_URL_IFDEFINES_LIST, API_URL_MCH_LIST, req } from '@/api/manage'

// 动态导入组件
const ReceiverAdd = defineAsyncComponent(() => import('./receiver-add.vue'))
const ReceiverEdit = defineAsyncComponent(() => import('./receiver-edit.vue'))
const Detail = defineAsyncComponent(() => import('./detail.vue'))

// 表格列配置
const tableColumns = [
  { key: 'receiverId', title: '收款账户ID', width: 125, scopedSlots: { customRender: 'receiverIdSlot' } },
  { key: 'receiverAlias', dataIndex: 'receiverAlias', title: '账户别名', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'appId', dataIndex: 'appId', title: '应用ID', width: 225 },
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '分组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '分组名称', width: 140 },
  { key: 'ifCode', title: '支付接口', width: 200, scopedSlots: { customRender: 'ifCodeSlot' } },
  { key: 'accNo', dataIndex: 'accNo', title: '收款账户账号', width: 200 },
  { key: 'accName', dataIndex: 'accName', title: '收款账户账号名称', width: 260 },
  { key: 'channelAccNo', dataIndex: 'channelAccNo', title: '渠道账号', width: 230 },
  { key: 'relationTypeName', dataIndex: 'relationTypeName', title: '收款关系类型', width: 140 },
  {
    key: 'state',
    dataIndex: 'state',
    title: '状态',
    width: 120,
    scopedSlots: { customRender: 'stateSlot' },
    align: 'center'
  },
  {
    key: 'divisionProfit',
    dataIndex: 'divisionProfit',
    title: '默认分账比例',
    width: 160,
    customRender: (text, record, index) => (text * 100).toFixed(2) + '%'
  },
  { key: 'bindSuccessTime', dataIndex: 'bindSuccessTime', title: '绑定成功时间', width: 200 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 响应式数据
const infoTable = ref(null)
const receiverAdd = ref(null)
const receiverEdit = ref(null)
const recordDetail = ref(null)
const searchData = reactive({ appId: '' })
const btnLoading = ref(false)
const showReceiverAdd = ref(false)
const showReceiverEdit = ref(false)
const showDetail = ref(false)
const ifDefineList = ref([])

// 搜索商户
const searchMch = (params) => {
  return req.list(API_URL_MCH_LIST, params)
}

// 对接table接口函数
const reqTableDataFunc = (params) => {
  return req.list(API_URL_DIVISION_RECEIVER, params)
}

// 查询支付接口定义列表
const reqIfDefineListFunc = () => {
  req.list(API_URL_IFDEFINES_LIST, { state: 1 }).then((res) => {
    ifDefineList.value = res
  })
}

// 搜索函数
const searchFunc = () => {
  // 点击查询按钮事件
  btnLoading.value = true // 开启查询按钮上的loading
  infoTable.value.loadData()
}

// 新增函数
const addFunc = () => {
  // 业务通道.收款账户 新增
  // 打开新增
  showReceiverAdd.value = true
}

// 详情函数
const detailFunc = (recordId) => {
  showDetail.value = true
  // 延迟获取ref对象并show方法确保弹窗已经渲染
  nextTick(() => {
    if (recordDetail.value) {
      recordDetail.value.show(recordId)
    }
  })
}

// 编辑函数
const editFunc = (recordId) => {
  // 业务通道.收款账户 编辑
  showReceiverEdit.value = true
  // 延迟获取ref对象并show方法确保弹窗已经渲染
  nextTick(() => {
    if (receiverEdit.value) {
      receiverEdit.value.show(recordId)
    }
  })
}

// 组件挂载时
onMounted(() => {
  reqIfDefineListFunc()
})
</script>
<style lang="less">
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
</style>

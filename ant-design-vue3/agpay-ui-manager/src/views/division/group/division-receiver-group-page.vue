<template>
  <div>
    <a-card>
      <div v-if="$access('ENT_DIVISION_RECEIVER_GROUP_LIST')" class="table-page-search-wrapper">
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
            <ag-input v-model="searchData.receiverGroupId" placeholder="分组ID" />
            <ag-input v-model="searchData.receiverGroupName" placeholder="分组名称" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.autoDivisionFlag" placeholder="是否自动分账" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="1">是</a-select-option>
                <a-select-option value="0">否</a-select-option>
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
        row-key="receiverGroupId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_GROUP_ADD')"
              type="primary"
              icon="plus"
              class="mg-b-30"
              @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_GROUP_EDIT')"
              type="link"
              @click="editFunc(record.receiverGroupId)"
              >编辑</a-button
            >
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_GROUP_DELETE')"
              type="link"
              style="color: red"
              @click="delFunc(record.receiverGroupId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { ref, reactive } from 'vue'
import { AgTable, AgTableActions, AgSelect, AgInput } from '@/components'
import { API_URL_DIVISION_RECEIVER_GROUP, API_URL_MCH_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'

// 表格列配置
const tableColumns = [
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '分组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '分组名称', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  {
    key: 'autoDivisionFlag',
    dataIndex: 'autoDivisionFlag',
    title: '自动分账',
    width: 120,
    customRender: (text, record, index) => (text === 1 ? '是' : '否')
  },
  { key: 'createdBy', dataIndex: 'createdBy', title: '创建人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 响应式数据
const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const searchData = reactive({})
const btnLoading = ref(false)

// 搜索商户
const searchMch = (params) => {
  return req.list(API_URL_MCH_LIST, params)
}

// 对接table接口函数
const reqTableDataFunc = (params) => {
  return req.list(API_URL_DIVISION_RECEIVER_GROUP, params)
}

// 搜索函数
const searchFunc = () => {
  // 点击查询按钮事件
  btnLoading.value = true // 开启查询按钮上的loading
  infoTable.value.loadData()
}

// 新增函数
const addFunc = () => {
  // 业务通道.分组管理 新增
  infoAddOrEdit.value.show()
}

// 编辑函数
const editFunc = (recordId) => {
  // 业务通道.分组管理 编辑
  infoAddOrEdit.value.show(recordId)
}

// 删除函数
const delFunc = (recordId) => {
  // 业务通道.分组管理 删除
  window.$infoBox.confirmDanger('确定删除吗', '', () => {
    // 需要加按钮的loading 返回 promise对象 否则要直接返回null
    return req.delById(API_URL_DIVISION_RECEIVER_GROUP, recordId).then((res) => {
      window.$message.success('删除成功')
      infoTable.value.loadData()
    })
  })
}
</script>

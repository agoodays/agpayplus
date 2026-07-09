<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.wayCode" label="支付方式代码" placeholder="支付方式代码" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.wayName" label="支付方式名称" placeholder="支付方式名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="支付类型"
                placeholder="请选择支付类型"
                allow-clear
                :options="[
                  { value: 'WECHAT', label: '微信' },
                  { value: 'ALIPAY', label: '支付宝' },
                  { value: 'YSFPAY', label: '云闪付' },
                  { value: 'UNIONPAY', label: '银联' },
                  { value: 'DCEPPAY', label: '数字人民币' },
                  { value: 'OTHER', label: '其他' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
        row-key="wayCode"
        @btn-load-close="btnLoading = false"
      >
        <template #toolbar-left>
          <div>
            <a-button v-if="true" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template #wayCodeSlot="{record}"><b>{{ record.wayCode }}</b></template> <!-- 自定义插槽 -->
        <template #wayTypeSlot="{record}">
          <a-tag
            :key="record.wayType"
            :color="record.wayType === 'WECHAT' ? 'rgb(4, 190, 2)' :
              record.wayType === 'ALIPAY' ? 'rgb(23, 121, 255)' :
              record.wayType === 'YSFPAY' ? '#f5222d' :
              record.wayType === 'UNIONPAY' ? '#00508e' :
              record.wayType === 'DCEPPAY' ? '#d12c2c' : '#fa8c16'">
            {{ record.wayType === 'WECHAT' ? '微信' :
              record.wayType === 'ALIPAY' ? '支付宝' :
              record.wayType === 'YSFPAY' ? '云闪付' :
              record.wayType === 'UNIONPAY' ? '银联' :
              record.wayType === 'DCEPPAY' ? '数字人民币' : '其他' }}
          </a-tag>
        </template>
        <template #opSlot="{record}">  <!-- 操作列插槽 -->
          <ag-table-actions>
            <a-button type="link" v-if="true" @click="editFunc(record.wayCode)">修改</a-button>
            <a-button type="link" style="color: red" v-if="true" @click="delFunc(record.wayCode)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增页面组件  -->
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="searchFunc"/>
  </div>

</template>
<script setup>
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { reactive, ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'
import { message } from 'ant-design-vue'

const tableColumns = [
  { key: 'wayCode', fixed: 'left', title: '支付方式代码', width: 180, customRender: 'wayCodeSlot' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称', width: 180 },
  { key: 'wayType', title: '支付类型', width: 120, align: 'center', customRender: 'wayTypeSlot' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const btnLoading = ref(false)
const searchData = reactive({})

const reqTableDataFunc = (params) => {
  return payConfigApi.queryPayWayList(params)
}

const searchFunc = () => {
  btnLoading.value = true
  infoTable.value?.reload()
}

const addFunc = () => {
  infoAddOrEdit.value.show()
}

const editFunc = (wayCode) => {
  infoAddOrEdit.value.show(wayCode)
}

const delFunc = (wayCode) => {
  window.$infoBox.confirmDanger('确认删除？', '', () => {
    payConfigApi.delPayWayById(wayCode).then(() => {
      message.success('删除成功！')
      infoTable.value?.reload()
    })
  })
}
</script>
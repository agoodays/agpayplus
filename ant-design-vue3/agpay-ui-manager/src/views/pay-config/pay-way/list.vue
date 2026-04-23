<template>
  <div>
    <a-card>
      <ag-search
        :search-data="searchData"
        :open-is-show-more="false"
        :is-show-more="isShowMore"
        :btn-loading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc">
        <template #formItem>
          <ag-input :placeholder="'支付方式代码'" v-model:value="searchData.wayCode" />
          <ag-input :placeholder="'支付方式名称'" v-model:value="searchData.wayName" />
          <a-form-item label="" class="table-head-layout">
            <a-select v-model:value="searchData.wayType" placeholder="支付类型" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="WECHAT">微信</a-select-option>
              <a-select-option value="ALIPAY">支付宝</a-select-option>
              <a-select-option value="YSFPAY">云闪付</a-select-option>
              <a-select-option value="UNIONPAY">银联</a-select-option>
              <a-select-option value="DCEPPAY">数字人民币</a-select-option>
              <a-select-option value="OTHER">其他</a-select-option>
            </a-select>
          </a-form-item>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        @btn-load-close="btnLoading=false"
        ref="infoTable"
        :init-data="true"
        :req-table-data-func="reqTableDataFunc"
        :table-columns="tableColumns"
        :search-data="searchData"
        row-key="wayCode"
      >
        <template #topLeftSlot>
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
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="queryFunc"/>
  </div>

</template>
<script setup>
import { ref, reactive } from 'vue'
import { API_URL_PAYWAYS_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'

const tableColumns = [
  { key: 'wayCode', fixed: 'left', title: '支付方式代码', scopedSlots: { customRender: 'wayCodeSlot' } },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
  { key: 'wayType', title: '支付类型', align: 'center', scopedSlots: { customRender: 'wayTypeSlot' } },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const isShowMore = ref(false)
const btnLoading = ref(false)
const searchData = reactive({})

const handleSearchFormData = (data) => {
  Object.assign(searchData, data)
}

const setIsShowMore = (value) => {
  isShowMore.value = value
}

// 请求table接口数据
const reqTableDataFunc = (params) => {
  return req.list(API_URL_PAYWAYS_LIST, params)
}

const queryFunc = () => { // 点击【查询】按钮点击事件
  btnLoading.value = true
  infoTable.value?.refTable(true)
}

const addFunc = () => { // 业务通用【新增】 函数
  infoAddOrEdit.value.show()
}

const editFunc = (wayCode) => { // 业务通用【修改】 函数
  infoAddOrEdit.value.show(wayCode)
}

const delFunc = (wayCode) => {
  import('ant-design-vue').then(({ message }) => {
    import('@/utils/info-box').then(({ infoBox }) => {
      infoBox.confirmDanger('确认删除？', '', () => {
        return req.delById(API_URL_PAYWAYS_LIST, wayCode).then(res => {
          message.success('删除成功！')
          infoTable.value?.refTable(false)
        })
      })
    })
  })
}
</script>
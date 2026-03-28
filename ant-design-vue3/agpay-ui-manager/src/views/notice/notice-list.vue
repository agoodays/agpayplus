<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :btn-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <ag-input v-model="searchData.articleId" placeholder="公告ID" />
          <ag-input v-model="searchData.title" placeholder="公告标题" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="articleId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_NOTICE_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_NOTICE_EDIT')" type="link" @click="editFunc(record.articleId)">编辑</a-button>
            <a-button v-if="$access('ENT_NOTICE_VIEW')" type="link" @click="detailFunc(record.articleId)"
              >详情</a-button
            >
            <a-button v-if="$access('ENT_NOTICE_DEL')" type="link" style="color: red" @click="delFunc(record.articleId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 详情页面弹窗  -->
    <info-detail ref="infoDetail" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { AgSearch, AgTable, AgTableActions, AgDateRangePicker, AgInput } from '@/components'
import { API_URL_ARTICLE_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'
import InfoDetail from './detail.vue'
import moment from 'moment'

// 表格列配置
const tableColumns = [
  { key: 'articleId', dataIndex: 'articleId', title: '公告ID', width: 80, fixed: 'left' },
  { key: 'title', dataIndex: 'title', title: '公告标题', width: 200 },
  { key: 'subtitle', dataIndex: 'subtitle', title: '副标题', width: 200 },
  { key: 'publisher', dataIndex: 'publisher', title: '发布人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 默认查询参数对象模板
const defaultSearchData = {
  articleType: 1 // 文章类型: 1-公告
}

// 响应式数据
const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const infoDetail = ref(null)
const btnLoading = ref(false)
const searchData = reactive({ ...defaultSearchData })

// 查询函数
const queryFunc = () => {
  btnLoading.value = true
  infoTable.value.loadData()
}

// 对接table接口函数
const reqTableDataFunc = (params) => {
  return req.list(API_URL_ARTICLE_LIST, params)
}

// 搜索函数
const searchFunc = () => {
  // 点击查询按钮事件
  infoTable.value.loadData()
}

// 新增函数
const addFunc = () => {
  // 业务通道.公告管理 新增
  infoAddOrEdit.value.show()
}

// 编辑函数
const editFunc = (recordId) => {
  // 业务通道.公告管理 编辑
  infoAddOrEdit.value.show(recordId)
}

// 详情函数
const detailFunc = (recordId) => {
  // 查看详情页面
  infoDetail.value.show(recordId)
}

// 删除公告
const delFunc = (recordId) => {
  window.$infoBox.confirmDanger('确定删除吗', '', () => {
    reqLoad.delById(API_URL_ARTICLE_LIST, recordId).then((res) => {
      infoTable.value.loadData()
      window.$message.success('删除成功')
    })
  })
}

// 日期选择变化
const onChange = (date, dateString) => {
  searchData.createdStart = dateString[0] // 开始时间
  searchData.createdEnd = dateString[1] // 结束时间
}

// 禁用日期
const disabledDate = (current) => {
  // 今天之后的日期不可选
  return current && current > moment().endOf('day')
}

// 组件挂载时
onMounted(() => {
  // 组件初始化时将默认数据赋值给 searchData
  Object.assign(searchData, defaultSearchData)
})
</script>

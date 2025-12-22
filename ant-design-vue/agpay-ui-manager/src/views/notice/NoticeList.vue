<template>
  <div>
    <a-card>
      <AgSearchForm
        :searchData="searchData"
        :openIsShowMore="false"
        :isShowMore="isShowMore"
        :btnLoading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc">
        <template slot="formItem">
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
          </a-form-item>
          <ag-text-up :placeholder="'公告ID'" :msg="searchData.articleId" v-model="searchData.articleId"/>
          <ag-text-up :placeholder="'公告标题'" :msg="searchData.title" v-model="searchData.title"/>
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
        rowKey="articleId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_NOTICE_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_NOTICE_EDIT')" @click="editFunc(record.articleId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_NOTICE_VIEW')" @click="detailFunc(record.articleId)">详情</a-button>
            <a-button type="link" v-if="$access('ENT_NOTICE_DEL')" style="color: red" @click="delFunc(record.articleId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 新增页面组件  -->
    <InfoDetail ref="infoDetail" :callbackFunc="searchFunc"/>
  </div>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_ARTICLE_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InfoDetail from './Detail'
import moment from 'moment'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'articleId', dataIndex: 'articleId', title: '公告ID', width: 80, fixed: 'left' },
  { key: 'title', dataIndex: 'title', title: '公告标题', width: 200 },
  { key: 'subtitle', dataIndex: 'subtitle', title: '公告副标题', width: 200 },
  { key: 'publisher', dataIndex: 'publisher', title: '发布人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'NoticePage',
  components: { AgSearchForm, AgTable, AgDateRangePicker, AgTextUp, AgTableColumns, InfoAddOrEdit, InfoDetail },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      defaultSearchData: {
        articleType: 1 // 文章类型: 1-公告
      },
      searchData: defaultSearchData
    }
  },
  mounted () {
  },
  methods: {
    handleSearchFormData (searchData) {
      // 如果是空对象或者为null/undefined
      if (!searchData || Object.keys(searchData).length === 0) {
        this.searchData = { ...this.defaultSearchData }
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
      return req.list(API_URL_ARTICLE_LIST, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId)
    },
    detailFunc: function (recordId) { // 公告详情页
      this.$refs.infoDetail.show(recordId)
    },
    // 删除公告
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        reqLoad.delById(API_URL_ARTICLE_LIST, recordId).then(res => {
          that.$refs.infoTable.refTable(true)
          this.$message.success('删除成功')
        })
      })
    },
    moment,
    onChange (date, dateString) {
      this.searchData.createdStart = dateString[0] // 开始时间
      this.searchData.createdEnd = dateString[1] // 结束时间
    },
    disabledDate (current) { // 今日之后日期不可选
      return current && current > moment().endOf('day')
    }
  }
}
</script>

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
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <ag-text-up :placeholder="'公告ID'" :msg="searchData.articleId" v-model="searchData.articleId" />
          <ag-text-up :placeholder="'公告标题'" :msg="searchData.title" v-model="searchData.title" />
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
        rowKey="articleId">
        <template slot="opSlot" slot-scope="{record}">
          <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_ARTICLE_NOTICEINFO')" @click="detailFunc(record.articleId)">详情</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 详细页面组件  -->
    <InfoDetail ref="infoDetail"/>
  </div>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_ARTICLE_LIST, req } from '@/api/manage'
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

// 默认查询条件数据
const defaultSearchData = {
  articleType: 1 // 文章类型: 1-公告
}

export default {
  name: 'NoticePage',
  components: { AgSearchForm, AgTable, AgDateRangePicker, AgTextUp, AgTableColumns, InfoDetail },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: defaultSearchData
    }
  },
  mounted () {
  },
  methods: {
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
      return req.list(API_URL_ARTICLE_LIST, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    detailFunc: function (recordId) { // 公告详情页
      this.$refs.infoDetail.show(recordId)
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

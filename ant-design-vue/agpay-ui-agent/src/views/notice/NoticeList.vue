<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout" style="max-width:350px;min-width:300px">
              <a-range-picker
                  @change="onChange"
                  :show-time="{ format: 'HH:mm:ss' }"
                  format="YYYY-MM-DD HH:mm:ss"
                  :disabled-date="disabledDate"
                  :ranges="{
                  今天: [moment().startOf('day'), moment()],
                  昨天: [moment().startOf('day').subtract(1,'days'), moment().endOf('day').subtract(1, 'days')],
                  最近三天: [moment().startOf('day').subtract(2, 'days'), moment().endOf('day')],
                  最近一周: [moment().startOf('day').subtract(1, 'weeks'), moment()],
                  本月: [moment().startOf('month'), moment()],
                  本年: [moment().startOf('year'), moment()]
                }"
              >
                <a-icon slot="suffixIcon" type="sync" />
              </a-range-picker>
            </a-form-item>
            <ag-text-up :placeholder="'公告ID'" :msg="searchData.articleId" v-model="searchData.articleId"/>
            <ag-text-up :placeholder="'公告标题'" :msg="searchData.title" v-model="searchData.title"/>
            <span class="table-page-search-submitButtons" style="flex-grow: 0; flex-shrink: 0;">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>

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
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_ARTICLE_NOTICEINFO')" @click="detailFunc(record.articleId)">详情</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 详细页面组件  -->
    <InfoDetail ref="infoDetail"/>
  </page-header-wrapper>
</template>
<script>
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_ARTICLE_LIST, req } from '@/api/manage'
import InfoDetail from './Detail'
import moment from 'moment'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'articleId', fixed: 'left', width: '80px', title: '公告ID', dataIndex: 'articleId' },
  { key: 'title', title: '公告标题', width: '200px', dataIndex: 'title' },
  { key: 'subtitle', title: '公告副标题', width: '200px', dataIndex: 'subtitle' },
  { key: 'publisher', title: '发布人', width: '120px', dataIndex: 'publisher' },
  { key: 'createdAt', dataIndex: 'createdAt', width: '200px', title: '创建日期' },
  { key: 'op', title: '操作', width: '260px', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'NoticePage',
  components: { AgTable, AgTextUp, AgTableColumns, InfoDetail },
  data () {
    return {
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {
        articleType: 1 // 文章类型: 1-公告
      }
    }
  },
  mounted () {
  },
  methods: {
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

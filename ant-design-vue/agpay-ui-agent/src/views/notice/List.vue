<template>
  <a-card class="card">
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
    <a-list item-layout="vertical" size="large" :pagination="pagination" :data-source="listData">
      <div slot="header"><div class="title">全部公告</div></div>
      <div slot="footer"><div class="footer"><b></b></div></div>
      <a-list-item class="list-item" slot="renderItem" key="item.title" slot-scope="item">
        <a-list-item-meta :description="item.subtitle">
          <a slot="title" type="link" v-if="$access('ENT_ARTICLE_NOTICEINFO')" @click="detailFunc(item.articleId)">{{ item.title }}</a>
          <a slot="title" type="link" v-else>{{ item.title }}</a>
          <a-avatar slot="avatar" :src="item.avatar || 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'" />
        </a-list-item-meta>
        <!-- {{ item.content }} -->
        <div class="content" v-html="item.content"></div>
        <img
          slot="extra"
          width="272"
          alt="logo"
          :src="item.logo || 'https://gw.alipayobjects.com/zos/rmsportal/mqaQswcyDLcXyDKnZfES.png'"
        />
        <template slot="actions">
          <span v-for="{ type, key } in actions" :key="type">
            <a-icon :type="type" style="margin-right: 8px" />
            {{ item[key] }}
          </span>
        </template>
      </a-list-item>
    </a-list>
    <!-- 详细页面组件  -->
    <InfoDetail ref="infoDetail"/>
  </a-card>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import { API_URL_ARTICLE_LIST, req } from '@/api/manage'
import InfoDetail from './Detail'
export default {
  components: { AgSearchForm, AgTable, AgDateRangePicker, AgTextUp, InfoDetail },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      searchData: {
        articleType: 1 // 文章类型: 1-公告
      },
      listData: [],
      pagination: {
        onChange: page => {
          this.handlePageChange(page)
        },
        current: 1,
        pageSize: 10,
        total: 0
      },
      actions: [
        // { type: 'star-o', key: 'star' },
        // { type: 'like-o', key: 'like' },
        // { type: 'message', key: 'message' },
        { type: 'user', key: 'publisher' },
        { type: 'history', key: 'createdAt' }
      ]
    }
  },
  mounted () {
    this.refListData(true)
  },
  methods: {
    handleSearchFormData (searchData) {
      // 如果是空对象或者为null/undefined
      if (!searchData || Object.keys(searchData).length === 0) {
        this.searchData = { articleType: 1 }
      } else {
        this.searchData = { ...searchData }
      }
    },
    setIsShowMore (isShowMore) {
      this.isShowMore = isShowMore
    },
    refListData (isToFirst = false) {
      if (isToFirst) {
        this.pagination.current = 1
      }
      const params = {
        ...this.searchData,
        pageNumber: this.pagination.current,
        pageSize: this.pagination.pageSize
      }
      this.reqListDataFunc(params)
        .then(res => {
          this.listData = res.records || []
          this.pagination.total = res.total || 0
          this.btnLoading = false
        })
        .catch(err => {
          console.error('AgCard 加载失败:', err)
          this.listData = []
          this.pagination.total = 0
          this.btnLoading = false
        })
    },
    queryFunc () {
      this.btnLoading = true
      this.refListData(true)
    },
    // 请求接口数据
    reqListDataFunc: (params) => {
      return req.list(API_URL_ARTICLE_LIST, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.refListData(true)
    },
    detailFunc: function (recordId) { // 公告详情页
      this.$refs.infoDetail.show(recordId)
    },
    /**
    * 分页切换
    */
    handlePageChange (page) {
      this.pagination.current = page
      this.refCardList()
    }
  }
}
</script>
<style>
.ant-list-pagination {
  margin-bottom: 10px;
  margin-right: 20px
}

.card {
  background-color: #fff;
  overflow: hidden;
  border-radius: 12px
}

.card .list-item {
  padding: 16px 24px;
  font-weight: 400;
  font-size: 13px;
  color: #666;
  min-height: 60px
}

.card .list-item span:nth-child(1) {
  cursor: pointer
}

.card .list-item span:hover {
  color: #2691ff
}

.card .title {
  font-weight: 700;
  font-size: 14px;
  color: #333;
  padding-left: 24px;
}

.card .content {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 3; /* 显示的行数 */
  overflow: hidden;
  /* min-width: 575px; */
  height: 60px; /* 固定高度 = 行高 × 行数 */
  line-height: 20px; /* 设置行高 */
  text-overflow: ellipsis;
}

.card .footer {
  padding-left: 24px;
}
</style>

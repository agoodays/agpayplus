<!--
  AgPay 通用表格, 支持基础分页， 检索

  @author terrfly
  @site https://www.agpay.vip
  @date 2021/5/8 07:18
-->
<template>
  <div>
    <div class="ag-table-top-row">
      <div class="ag-table-top-left">
        <a-button icon="area-chart" class="statistics" v-if="isEnableDataStatistics" @click="isShowDataStatistics = !isShowDataStatistics">
          {{ isShowDataStatistics ? "关闭" : "数据" }}统计
        </a-button>
        <slot name="topLeftSlot"></slot>
      </div>
      <div class="operation-icons">
        <span class="pd-0-20" v-if="isShowAutoRefresh">
          <span style="margin-right: 10px; color: rgb(169, 179, 177);">自动刷新：
            <span style="margin-right: 5px; color: rgb(0, 0, 0);">{{ countdown }}s</span>
          </span>
          <a-switch v-model="enableAutoRefresh" />
        </span>
        <a-tooltip placement="top">
          <template #title>
            <span>数据导出</span>
          </template>
          <span v-if="isShowDownload" @click="downloadData" class="anticon anticon-download pd-0-20" style="cursor: pointer; font-size: 16px;color: #000;">
            <a-icon type="download" />
          </span>
        </a-tooltip>
<!--        <span class="pd-0-20" style="cursor: pointer; font-size: 16px;color: #000;"><a-icon type="column-height" /></span>-->
        <a-dropdown :trigger="['click']">
          <a-tooltip placement="top">
            <template #title>
              <span>表格密度</span>
            </template>
            <i class="bi bi-distribute-vertical pd-0-20" style="cursor: pointer; font-size: 16px;color: #000;" @click.prevent/>
          </a-tooltip>
          <template v-slot:overlay>
            <a-menu class="ant-pro-drop-down menu">
              <a-menu-item :key="0" @click="size='default'">
                默认
              </a-menu-item>
              <a-menu-item :key="1" @click="size='middle'">
                宽松
              </a-menu-item>
              <a-menu-item :key="2" @click="size='small'">
                紧促
              </a-menu-item>
            </a-menu>
          </template>
        </a-dropdown>
        <a-dropdown :trigger="['click']">
          <a-tooltip placement="top">
            <template #title>
              <span>列设置</span>
            </template>
            <i class="bi bi-layout-sidebar-inset-reverse ant-dropdown-trigger pd-0-20" style="cursor: pointer; font-size: 16px;color: #000;"/>
          </a-tooltip>
<!--          <a-menu slot="overlay">
            <a-checkbox-group v-model="visibleColumns">
              <a-menu-item v-for="column in allColumns" :key="column.key">
                <a-checkbox is-group="" :value="column.key" :key="column.key">{{ column.title }}</a-checkbox>
              </a-menu-item>
            </a-checkbox-group>
          </a-menu>-->
          <template v-slot:overlay>
            <a-menu class="ant-pro-drop-down menu">
              <a-checkbox-group v-model="visibleColumns">
                <a-menu-item v-for="column in allColumns" :key="column.key">
                  <a-checkbox is-group="" :value="column.key" :key="column.key">{{ column.title }}</a-checkbox>
                </a-menu-item>
              </a-checkbox-group>
            </a-menu>
          </template>
        </a-dropdown>
      </div>
    </div>
    <slot name="dataStatisticsSlot" v-if="isShowDataStatistics"></slot>
    <a-table
      :columns="displayedColumns"
      :size="size"
      :data-source="apiResData.records"
      :pagination="pagination"
      :loading="showLoading"
      @change="handleTableChange"
      :row-selection="rowSelection"
      :rowKey="rowKey"
      :scroll="{ x: scrollX }"
      :customRow="(record, index) => {
        if(!tableRowCrossColor){
          return {};
        }
        return { style: { 'background-color': index % 2 == 0 ? '#FCFCFC' : '#FFFFFF'} }
      }"
    >
      <!-- 自定义列插槽， 参考：https://github.com/feseed/admin-antd-vue/blob/master/src/components/ShTable.vue  -->
      <!--  eslint-disable-next-line -->
      <template v-for="colCustom in columnsCustomSlots" :slot="colCustom.customRender" slot-scope="record">
        <slot :name="colCustom.customRender" :record="record"></slot>
      </template>
    </a-table>
  </div>
</template>
<script>

export default {
  name: 'AgTable', // 定义组件名称

  // 传递数据参数 ( 父-->子 参数 )
  props: {
    defaultCountdown: { type: Number, default: 180 },
    autoRefresh: { type: Boolean, default: false }, // 自动刷新， 默认false
    isShowAutoRefresh: { type: Boolean, default: false }, // 是否显示自动刷新， 默认false
    isShowDownload: { type: Boolean, default: false }, // 是否显示自动刷新， 默认false
    isEnableDataStatistics: { type: Boolean, default: false }, // 是否显示自动刷新， 默认false
    initData: { type: Boolean, default: true }, // 初始化列表数据， 默认true
    tableColumns: { type: Array, default: null }, // 表格数组列
    reqTableDataFunc: { type: Function, default: () => () => ({}) }, // 请求列表数据
    reqDownloadDataFunc: { type: Function, default: () => () => ({}) }, // 请求列表数据
    currentChange: { type: Function, default: (v1, v2) => {} }, // 更新当前选择行事件， 默认空函数
    searchData: { type: Object, default: null }, // 搜索条件参数
    pageSize: { type: Number, default: 10 }, // 默认每页条数
    rowSelection: { type: Object, default: null }, // checkbox选择
    rowKey: { type: [String, Function], default: 'id' }, // 定义rowKey 如果不定义将会出现（树状结构出问题， checkbox不消失等）
    scrollX: { type: Number, default: 500 }, // 表格显示滚动条的宽度
    tableRowCrossColor: { type: Boolean, default: false } // 是隔行换色
  },

  data () {
    return {
      allColumns: this.tableColumns,
      visibleColumns: this.tableColumns.map(column => column.key),
      apiResData: { total: 0, records: [] }, // 接口返回数据
      iPage: { pageNumber: 1, pageSize: this.pageSize }, // 默认table 分页/排序请求后端格式
      pagination: { total: 0, current: 1, pageSize: this.pageSize, showSizeChanger: true, showTotal: total => `共${total}条` }, // ATable 分页配置项
      countdown: this.defaultCountdown,
      enableAutoRefresh: this.autoRefresh,
      isShowDataStatistics: false,
      showLoading: false,
      size: 'default'
    }
  },
  // 计算属性
  computed: {
    columnsCustomSlots () { // 自定义列插槽  1. 过滤器仅获取到包含slot属性的元素， 2. 返回slot数组
      return this.tableColumns.filter(item => item.scopedSlots).map(item => item.scopedSlots)
    },
    displayedColumns () {
      return this.allColumns.filter(column => this.visibleColumns.includes(column.key))
    }
  },
  mounted () {
    if (this.initData) { // 是否自动加载数据
      this.refTable(true)
    }
    this.startCountdown()
  },
  methods: {
    startCountdown () {
      const that = this
      // const timer =
      setInterval(() => {
        if (this.enableAutoRefresh) { // 判断是否启用自动刷新
          that.countdown--
          if (this.countdown === 0) {
            that.countdown = that.defaultCountdown
            // clearInterval(timer)
            that.refTable(false)
          }
        }
      }, 1000)
    },

    handleTableChange (pagination, filters, sorter) { // 分页、排序、筛选变化时触发
      this.pagination = pagination
      this.iPage = {
        pageSize: pagination.pageSize, // 每页条数
        pageNumber: pagination.current, // 当前页码
        sortField: sorter.columnKey, // 排序字段
        sortOrder: sorter.order, // 排序顺序
        ...filters // 过滤数据
      }
      this.refTable()
    },

    // 查询数据
    refTable (isToFirst = false) {
      const that = this
      if (isToFirst) {
        this.iPage.pageNumber = 1
        this.pagination.current = 1
      }
      // 更新检索数据
      this.showLoading = true
      this.reqTableDataFunc(Object.assign({}, this.iPage, this.searchData)).then(resData => {
        this.pagination.total = resData.total // 更新总数量
        this.apiResData = resData // 列表数据更新
        this.showLoading = false // 关闭loading

        // 数据为0 ，并且为当前页面没有在第一页则需要自动跳转到上一页（解决，删除第二页数据全部删除后无数据的情况 ）
        if (resData.records.length === 0 && this.iPage.pageNumber > 1) {
          that.$nextTick(() => {
            // 最大页码
            const maxPageNumber = (resData.total / this.iPage.pageSize) + ((resData.total % this.iPage.pageSize) === 0 ? 0 : 1)
            if (maxPageNumber === 0) { // 已经无数据
              return false
            }

            // 跳转到的页码
            const toPageSize = (this.iPage.pageNumber - 1) > maxPageNumber ? maxPageNumber : (this.iPage.pageNumber - 1)

            this.iPage.pageNumber = toPageSize
            this.pagination.current = toPageSize
            that.refTable(false)
          })
        }
        // 请求成功后，关闭查询按钮的loading
        that.$emit('btnLoadClose')
      }).catch(res => {
        this.showLoading = false
        that.$emit('btnLoadClose')
      }) // 关闭loading
    },

    // 下载数据
    downloadData () {
      this.iPage.pageNumber = 1
      this.iPage.pageSize = -1
      this.reqDownloadDataFunc(Object.assign({}, this.iPage, this.searchData))
    }
  }
}
</script>
<style lang="less">
  //// 调整antdv 的table默认padding高度
  //.ant-table-fixed{
  //  tr{
  //    th{
  //      padding: 8px 8px !important;
  //    }
  //    th:first-child{ // 第一个表格 左填充16， 其他为8
  //      padding-left: 16px !important;
  //    }
  //    td{
  //      padding: 8px 8px !important;
  //    }
  //    td:first-child{
  //      padding-left: 16px !important;
  //    }
  //  }
  //}

  .ant-table-wrapper {
    margin: 0 30px 15px;
  }

  .ag-table-top-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 30px 30px 10px
  }

  .ag-table-top-row .ag-table-top-left {
    display: flex;
    align-items: center
  }

  .ag-table-top-row .ant-btn {
    margin-right: 10px;
    margin-bottom: 10px
  }

  .ag-table-top-row .operation-icons {
    display: flex;
    justify-content: space-between;
    align-items: center
  }

  .ag-table-top-row .operation-icons .pd-0-20 {
    padding: 0 10px
  }

  .ant-dropdown-menu {
    padding: 10px 10px 5px!important;
    box-sizing: border-box;
    backdrop-filter: blur(15px);
    box-shadow: 0 10px 30px #35414d26!important
  }

  .ant-dropdown-menu-item, .undefined-item {
    padding: 0 12px!important;
    line-height: 40px!important;
  }

  .ant-dropdown-menu-item:hover {
    border-radius: 3px;
    background: #2691ff26!important;
  }

  .undefined-item:hover {
    border-radius: 3px;
    background: #2691ff26!important;
  }

  .statistics {
    display: inline-flex;
    justify-content: center;
    align-items: center;
    margin-top: 1px;
    background: #2691ff26!important;
    border: none;
    color: rgb(26, 102, 255)
  }

  @font-face {
    font-family: bootstrap-icons;
    src: url(//jeequan.oss-cn-beijing.aliyuncs.com/jeepay/cdn/s.jeepay.com/manager/assets/bootstrap-icons.c874e14c.woff2?524846017b983fc8ded9325d94ed40f3) format("woff2"),url(//jeequan.oss-cn-beijing.aliyuncs.com/jeepay/cdn/s.jeepay.com/manager/assets/bootstrap-icons.92f8082b.woff?524846017b983fc8ded9325d94ed40f3) format("woff")
  }

  .bi {
    display: flex;
    justify-content: center;
    align-items: center;
    transition: .3s ease;
  }

  .bi-distribute-vertical:before {
    content: "\f304"
  }

  .bi-layout-sidebar-inset-reverse:before {
    content: "\f45c";
  }

  .bi:before,[class^=bi-]:before,[class*=" bi-"]:before {
    display: inline-block;
    font-family: bootstrap-icons !important;
    font-style: normal;
    font-weight: 400 !important;
    font-variant: normal;
    text-transform: none;
    line-height: 1;
    vertical-align: -.125em;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale
  }
</style>

<template>
  <page-header-wrapper>
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
            <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
          </a-form-item>
          <ag-text-up :placeholder="'用户ID'" :msg="searchData.userId" v-model="searchData.userId" />
          <ag-text-up :placeholder="'用户名'" :msg="searchData.userName" v-model="searchData.userName" />
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.sysType" placeholder="所属系统" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="MGR">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商系统</a-select-option>
              <a-select-option value="MCH">商户系统</a-select-option>
            </a-select>
          </a-form-item>
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
        :rowSelection="rowSelection"
        rowKey="sysLogId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button icon="delete" v-if="$access('ENT_SYS_LOG_DEL')" type="danger" @click="delFunc" class="mg-b-30">删除</a-button>
          </div>
        </template>
        <template slot="userNameSlot" slot-scope="{record}"><b>{{ record.userName }}</b></template> <!-- 自定义插槽 -->
        <template slot="sysTypeSlot" slot-scope="{record}">
          <a-tag
            :key="record.sysType"
            :color="record.sysType === 'MGR' ? 'green' :
              record.sysType === 'AGENT' ? 'cyan' :
              record.sysType === 'MCH' ? 'geekblue' : 'loser'">
            {{ record.sysType === 'MGR' ? '运营平台' :
              record.sysType === 'AGENT' ? '代理商系统' :
              record.sysType === 'MCH' ? '商户系统' : '其他' }}
          </a-tag>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_SYS_LOG_VIEW')" @click="detailFunc(record.sysLogId)">详情</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 日志详情抽屉 -->
    <template>
      <a-drawer
        placement="right"
        :closable="true"
        :visible="visible"
        :title="visible === true? '日志详情':''"
        @close="onClose"
        :drawer-style="{ overflow: 'hidden' }"
        :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
        width="40%"
      >
        <a-row justify="space-between" type="flex">
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="用户ID">
                {{ detailData.userId }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="用户IP">
                {{ detailData.userIp }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="用户名">
                <b>
                  {{ detailData.userName }}
                </b>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="所属系统">
                <a-tag
                  :key="detailData.sysType"
                  :color="detailData.sysType === 'MGR' ? 'green' :
                    detailData.sysType === 'AGENT' ? 'cyan' :
                    detailData.sysType === 'MCH' ? 'geekblue' : 'loser'">
                  {{ detailData.sysType === 'MGR' ? '运营平台' :
                    detailData.sysType === 'AGENT' ? '代理商系统' :
                    detailData.sysType === 'MCH' ? '商户系统' : '其他' }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
        </a-row>
        <a-divider />
        <a-row justify="space-between" type="flex">
          <a-col :sm="24">
            <a-descriptions>
              <a-descriptions-item label="操作描述">
                {{ detailData.methodRemark }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="24">
            <a-descriptions>
              <a-descriptions-item label="请求方法">
                {{ detailData.methodName }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="24">
            <a-descriptions>
              <a-descriptions-item label="请求地址">
                {{ detailData.reqUrl }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
        </a-row>
        <a-row justify="start" type="flex">
          <a-col :sm="24">
            <a-form-model-item label="请求参数">
              <a-input
                type="textarea"
                disabled="disabled"
                style="background-color: black;color: #FFFFFF;height: 100px"
                v-model="detailData.optReqParam"
              />
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="start" type="flex">
          <a-col :sm="24">
            <a-form-model-item label="响应参数">
              <a-input
                type="textarea"
                disabled="disabled"
                style="background-color: black;color: #FFFFFF;height: 150px"
                v-model="detailData.optResInfo"
              />
            </a-form-model-item>
          </a-col>
        </a-row>
      </a-drawer>
    </template>
  </page-header-wrapper>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import { API_URL_SYS_LOG, req } from '@/api/manage'
import moment from 'moment'
import { message, Modal } from 'ant-design-vue'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'userName', width: 120, title: '用户名', fixed: 'left', scopedSlots: { customRender: 'userNameSlot' } },
  { key: 'userId', dataIndex: 'userId', width: 120, title: '用户ID' },
  { key: 'userIp', dataIndex: 'userIp', width: 120, title: '用户IP' },
  { key: 'sysType', width: 120, title: '所属系统', scopedSlots: { customRender: 'sysTypeSlot' } },
  { key: 'methodRemark', dataIndex: 'methodRemark', width: 200, title: '操作描述', ellipsis: true },
  { key: 'createdAt', dataIndex: 'createdAt', width: 200, title: '创建日期' },
  { key: 'op', title: '操作', width: '100px', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'IsvListPage',
  components: { AgSearchForm, AgTable, AgTableColumns, AgDateRangePicker, AgTextUp },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: {},
      selectedIds: [], // 选中的数据
      visible: false,
      detailData: {},
      isShowMore: false,
      btnLoading: false
    }
  },
  computed: {
    rowSelection () {
      const that = this
      return {
        onChange: (selectedRowKeys, selectedRows) => {
          that.selectedIds = [] // 清空选中数组
          selectedRows.forEach(function (data) { // 赋值选中参数
            that.selectedIds.push(data.sysLogId)
          })
        }
      }
    }
  },
  mounted () {
  },
  methods: {
    handleSearchFormData (searchData) {
      this.searchData = searchData
    },
    setIsShowMore (isShowMore) {
      this.isShowMore = isShowMore
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_SYS_LOG, params)
    },
    queryFunc () {
      this.btnLoading = true
      this.$refs.infoTable.refTable(true)
    },
    delFunc: function () {
      const that = this
      if (that.selectedIds.length === 0) {
        message.error('请选择要删除的日志')
        return false
      }
      Modal.confirm({
        title: '确认删除' + that.selectedIds.length + '条日志吗？',
        okType: 'danger',
        onOk () {
          req.delById(API_URL_SYS_LOG, that.selectedIds).then(res => {
            that.selectedIds = [] // 清空选中数组
            that.$refs.infoTable.refTable(true)
            that.$message.success('删除成功')
          })
        },
        onCance () {
        }
      })
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    detailFunc: function (recordId) {
      const that = this
      req.getById(API_URL_SYS_LOG, recordId).then(res => {
        that.detailData = res
      })
      this.visible = true
    },
    moment,
    onChange (date, dateString) {
      this.searchData.createdStart = dateString[0] // 开始时间
      this.searchData.createdEnd = dateString[1] // 结束时间
    },
    disabledDate (current) { // 今日之后日期不可选
      return current && current > moment().endOf('day')
    },
    onClose () {
      this.visible = false
    }
  }
}
</script>

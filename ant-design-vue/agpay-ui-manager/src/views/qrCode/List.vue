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
            <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
          </a-form-item>
          <ag-text-up :placeholder="'代理商号'" :msg="searchData.agentNo" v-model="searchData.agentNo" />
          <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" />
          <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId" />
          <ag-text-up :placeholder="'码牌ID'" :msg="searchData.qrcId" v-model="searchData.qrcId" />
        </template>
      </AgSearchForm>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="false"
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="qrcId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_DEVICE_QRC_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">创建空码</a-button>
          </div>
        </template>
        <template slot="qrcIdSlot" slot-scope="{record}">
          <span>
            <a-icon type="qrcode" v-if="$access('ENT_DEVICE_QRC_VIEW')" @click="onPreview(record.qrcId)"/>
            {{ record.qrcId }}
          </span>
        </template> <!-- 自定义插槽 -->
        <template slot="bindInfoSlot" slot-scope="{record}">
          <span v-if="record.mchNo">
            <p>{{ record.mchNo }}</p>
            <p>{{ record.appId }}</p>
            <p>{{ record.storeId }}</p>
          </span>
          <span v-else><a-icon type="exclamation-circle"/>未绑定</span>
        </template>
        <template slot="entryPageSlot" slot-scope="{record}">
          <span>{{ record.entryPage === 'default' ? '默认':(record.entryPage === 'h5' ? 'H5':(record.entryPage === 'lite' ? '小程序':'')) }}</span>
        </template>
        <template slot="stateSlot" slot-scope="{record}">
          <AgTableColState :state="record.state" :showSwitchType="$access('ENT_DEVICE_QRC_EDIT')" :onChange="(state) => { return updateState(record.qrcId, state)}"/>
        </template>
        <template slot="fixedPayAmountSlot" slot-scope="{record}">
          <span>{{ record.fixedFlag === 0 ? '任意金额': record.fixedPayAmount }}</span>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_DEVICE_QRC_VIEW')" @click="onPreview(record.qrcId)">详情</a-button>
<!--            <a-button type="link" v-if="$access('ENT_DEVICE_QRC_EDIT')" @click="editFunc(record.qrcId)">修改</a-button>-->
            <a-button type="link" style="color: red" v-if="$access('ENT_DEVICE_QRC_DEL')" @click="delFunc(record.qrcId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="queryFunc"/>
  </div>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgTableColState from '@/components/AgTable/AgTableColState'
import { API_URL_QRC_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'

const tableColumns = [
  { key: 'qrcId', fixed: 'left', title: '码牌ID', width: 180, scopedSlots: { customRender: 'qrcIdSlot' } },
  { key: 'batchId', dataIndex: 'batchId', title: '批次号', width: 135 },
  { key: 'bindInfo', title: '绑定商户信息', width: 280, scopedSlots: { customRender: 'bindInfoSlot' } },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'entryPage', title: '扫码页面', width: 140, scopedSlots: { customRender: 'entryPageSlot' } },
  { key: 'state', title: '状态', width: 80, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'fixedPayAmount', title: '固定金额', width: 120, scopedSlots: { customRender: 'fixedPayAmountSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'PayWayPage',
  components: { AgSearchForm, AgTable, AgTableColumns, AgTableColState, InfoAddOrEdit, AgTextUp, AgDateRangePicker },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {}
    }
  },
  mounted () {
    this.searchData.mchNo = this.$route.query.mchNo
    this.queryFunc()
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
      return req.list(API_URL_QRC_LIST, params)
    },
    queryFunc () { // 点击【查询】按钮点击事件
      this.btnLoading = true
      this.searchFunc(true)
    },
    searchFunc (isToFirst = false) { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(isToFirst)
    },
    onPreview (recordId) {
      const that = this
      req.get(API_URL_QRC_LIST + '/view/' + recordId).then(res => {
        that.$viewerApi({
          images: [res],
          options: {
            initialViewIndex: 0
          }
        })
      })
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (qrcId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(qrcId)
    },
    delFunc: function (qrcId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        req.delById(API_URL_QRC_LIST, qrcId).then(res => {
          that.$message.success('删除成功！')
          that.$refs.infoTable.refTable(false)
        })
      })
    },
    updateState: function (recordId, state) { // 【更新状态】
      const that = this
      const title = state === 1 ? '确认[启用]？' : '确认[停用]？'

      return new Promise((resolve, reject) => {
        that.$infoBox.confirmDanger(title, '', () => {
              return reqLoad.updateById(API_URL_QRC_LIST, recordId, { state: state }).then(res => {
                that.searchFunc()
                resolve()
              }).catch(err => reject(err))
            },
            () => {
              reject(new Error())
            })
      })
    }
  }
}
</script>

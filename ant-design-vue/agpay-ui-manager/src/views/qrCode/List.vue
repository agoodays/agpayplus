<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline"class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
            </a-form-item>
            <ag-text-up :placeholder="'代理商号'" :msg="searchData.agentNo" v-model="searchData.agentNo" />
            <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" />
            <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId" />
            <ag-text-up :placeholder="'码牌ID'" :msg="searchData.qrcId" v-model="searchData.qrcId" />
            <span class="table-page-search-submitButtons">
              <a-button type="primary" @click="searchFunc(true)" icon="search" :loading="btnLoading">查询</a-button>
              <a-button style="margin-left: 8px;" @click="() => this.searchData = {}" icon="reload">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line"/>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="true"
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
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'禁用':'启用'" />
        </template>
        <template slot="fixedPayAmountSlot" slot-scope="{record}">
          <span>{{ record.fixedFlag === 0 ? '任意金额': record.fixedPayAmount }}</span>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a v-if="$access('ENT_DEVICE_QRC_VIEW')" @click="onPreview(record.qrcId)">详情</a>
<!--            <a v-if="$access('ENT_DEVICE_QRC_EDIT')" @click="editFunc(record.qrcId)">修改</a>-->
            <a style="color: red" v-if="$access('ENT_DEVICE_QRC_DEL')" @click="delFunc(record.qrcId)">删除</a>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>

    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
  </page-header-wrapper>

</template>
<script>
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_QRC_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'

const tableColumns = [
  { key: 'qrcId', fixed: 'left', title: '码牌ID', scopedSlots: { customRender: 'qrcIdSlot' }, width: 180 },
  { key: 'batchId', dataIndex: 'batchId', title: '批次号', width: 135 },
  { key: 'bindInfo', title: '绑定商户信息', scopedSlots: { customRender: 'bindInfoSlot' }, width: 280 },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'entryPage', title: '扫码页面', scopedSlots: { customRender: 'entryPageSlot' }, width: 140 },
  { key: 'state', title: '状态', scopedSlots: { customRender: 'stateSlot' }, width: 80 },
  { key: 'fixedPayAmount', title: '固定金额', scopedSlots: { customRender: 'fixedPayAmountSlot' }, width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 180 },
  { key: 'op', title: '操作', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' }, width: 200 }
]

export default {
  name: 'PayWayPage',
  components: { AgTable, AgTableColumns, InfoAddOrEdit, AgTextUp, AgDateRangePicker },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: {},
      btnLoading: false
    }
  },
  methods: {
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_QRC_LIST, params)
    },

    searchFunc (isToFirst = false) { // 点击【查询】按钮点击事件
      this.btnLoading = true
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
    }
  }
}
</script>

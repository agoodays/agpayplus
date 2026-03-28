<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :btn-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <!-- <ag-text-up :placeholder="'代理商号'" :msg="searchData.agentNo" v-model="searchData.agentNo" />
          <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo"/> -->
          <a-form-item label="" class="table-head-layout">
            <ag-select
              v-model="searchData.agentNo"
              :api="searchAgent"
              value-field="agentNo"
              label-field="agentName"
              placeholder="代理商号(支持按代理商名称搜索)"
            />
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <ag-select
              v-model="searchData.mchNo"
              :api="searchMch"
              value-field="mchNo"
              label-field="mchName"
              placeholder="商户号(支持按商户名称搜索)"
            />
          </a-form-item>
          <ag-input v-model="searchData.appId" placeholder="应用AppId" />
          <ag-input v-model="searchData.qrcId" placeholder="二维码ID" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="false"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="qrcId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_DEVICE_QRC_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >生成二维码</a-button
            >
          </div>
        </template>
        <template #qrcIdSlot="{ record }">
          <span>
            <a-icon v-if="$access('ENT_DEVICE_QRC_VIEW')" type="qrcode" @click="onPreview(record.qrcId)" />
            {{ record.qrcId }}
          </span>
        </template>
        <!-- 自定义列 -->
        <template #bindInfoSlot="{ record }">
          <span v-if="record.bindState === 1 && record.mchNo">
            <p>已绑定商户：{{ record.mchName }}[{{ record.mchNo }}]</p>
            <p>应用：{{ record.appName }}[{{ record.appId }}]</p>
            <p>门店：{{ record.storeName }}[{{ record.storeId }}]</p>
          </span>
          <span v-else><a-icon type="exclamation-circle" />未绑定</span>
        </template>
        <template #entryPageSlot="{ record }">
          <span>{{
            record.entryPage === 'default'
              ? '默认'
              : record.entryPage === 'h5'
                ? 'H5'
                : record.entryPage === 'lite'
                  ? '小程序'
                  : ''
          }}</span>
        </template>
        <template #stateSlot="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch-type="$access('ENT_DEVICE_QRC_EDIT')"
            :on-change="
              (state) => {
                return updateState(record.qrcId, state)
              }
            "
          />
        </template>
        <template #fixedPayAmountSlot="{ record }">
          <span>{{ record.fixedFlag === 0 ? '不固定' : (record.fixedPayAmount / 100).toFixed(2) }}</span>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_DEVICE_QRC_VIEW')" type="link" @click="onPreview(record.qrcId)">预览</a-button>
            <a-button v-if="$access('ENT_DEVICE_QRC_EDIT')" type="link" @click="editFunc(record.qrcId)">编辑</a-button>
            <a-button v-if="$access('ENT_DEVICE_QRC_EDIT')" type="link" @click="bindFunc(record.qrcId)">绑定</a-button>
            <a-button
              v-if="$access('ENT_DEVICE_QRC_EDIT') && record.bindState === 1"
              type="link"
              @click="unbindFunc(record.qrcId)"
              >解绑</a-button
            >
            <a-button v-if="$access('ENT_DEVICE_QRC_DEL')" type="link" style="color: red" @click="delFunc(record.qrcId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="queryFunc" />
    <Bind ref="bind" :callback-func="queryFunc" />
  </div>
</template>
<script>
import { AgSearch, AgTable, AgTableActions, AgSelect, AgStateSwitch, AgInput, AgDateRangePicker } from '@/components'
import { API_URL_QRC_LIST, API_URL_AGENT_LIST, API_URL_MCH_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'
import Bind from './bind.vue'

const tableColumns = [
  { key: 'qrcId', fixed: 'left', title: '二维码ID', width: 180, scopedSlots: { customRender: 'qrcIdSlot' } },
  { key: 'batchId', dataIndex: 'batchId', title: '批次号', width: 135 },
  { key: 'bindInfo', title: '绑定商户信息', width: 360, scopedSlots: { customRender: 'bindInfoSlot' } },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'entryPage', title: '扫码页面', width: 140, scopedSlots: { customRender: 'entryPageSlot' } },
  { key: 'state', title: '状态', width: 80, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'fixedPayAmount', title: '固定金额', width: 120, scopedSlots: { customRender: 'fixedPayAmountSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'PayWayPage',
  components: {
    'ag-search': AgSearch,
    'ag-table': AgTable,
    'ag-table-actions': AgTableActions,
    'ag-state-switch': AgStateSwitch,
    'ag-select': AgSelect,
    'ag-input': AgInput,
    'ag-date-range-picker': AgDateRangePicker,
    InfoAddOrEdit,
    Bind
  },
  data() {
    return {
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {}
    }
  },
  mounted() {
    this.searchData.mchNo = this.$route.query.mchNo
    this.queryFunc()
  },
  methods: {
    searchAgent(params) {
      return req.list(API_URL_AGENT_LIST, params)
    },
    searchMch(params) {
      return req.list(API_URL_MCH_LIST, params)
    },

    // 对接table接口函数
    reqTableDataFunc: (params) => {
      return req.list(API_URL_QRC_LIST, params)
    },
    queryFunc() {
      // 点击查询按钮事件
      this.btnLoading = true
      this.searchFunc(true)
    },
    searchFunc(isToFirst = false) {
      // 点击查询按钮事件
      this.$refs.infoTable.loadData()
    },
    onPreview(recordId) {
      const that = this
      req.get(API_URL_QRC_LIST + '/view/' + recordId).then((res) => {
        that.$viewerApi({
          images: [res],
          options: {
            initialViewIndex: 0
          }
        })
      })
    },
    addFunc: function () {
      // 业务通道.二维码管理 新增
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (qrcId) {
      // 业务通道.二维码管理 编辑
      this.$refs.infoAddOrEdit.show(qrcId)
    },
    bindFunc: function (qrcId) {
      this.$refs.bind.show(qrcId)
    },
    delFunc: function (qrcId) {
      const that = this
      this.$infoBox.confirmDanger('确定删除吗', '', () => {
        req.delById(API_URL_QRC_LIST, qrcId).then((res) => {
          that.$message.success('删除成功')
          that.$refs.infoTable.loadData()
        })
      })
    },
    unbindFunc: function (recordId) {
      // 二维码解绑
      const that = this
      return new Promise((resolve, reject) => {
        that.$infoBox.confirmDanger(
          '确认解绑',
          '解绑后商户将无法使用该二维码',
          () => {
            return reqLoad
              .updateById(API_URL_QRC_LIST + '/unbind', recordId, {})
              .then((res) => {
                that.searchFunc()
                resolve()
              })
              .catch((err) => reject(err))
          },
          () => {
            reject(new Error())
          }
        )
      })
    },
    updateState: function (recordId, state) {
      // 二维码状态更新
      const that = this
      const title = state === 1 ? '确认[启用]吗' : '确认[停用]吗'

      return new Promise((resolve, reject) => {
        that.$infoBox.confirmDanger(
          title,
          '',
          () => {
            return reqLoad
              .updateById(API_URL_QRC_LIST, recordId, { state: state })
              .then((res) => {
                that.searchFunc()
                resolve()
              })
              .catch((err) => reject(err))
          },
          () => {
            reject(new Error())
          }
        )
      })
    }
  }
}
</script>

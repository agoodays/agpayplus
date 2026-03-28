<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :collapsible="false">
        <template #default>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" placeholder="服务商号" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.isvName" placeholder="服务商名称" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <a-select v-model="searchData.state" placeholder="服务商状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">禁用</a-select-option>
                <a-select-option value="1">启用</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        row-key="isvNo"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_ISV_INFO_ADD')" icon="plus" type="primary" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #isvNameSlot="{ record }"
          ><b :title="record.isvName">{{ record.isvName }}</b></template
        >
        <!-- 自定义列 -->
        <template #stateSlot="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_ISV_INFO_EDIT')" type="link" @click="editFunc(record.isvNo)">编辑</a-button>
            <a-button
              v-if="$access('ENT_ISV_OAUTH2_CONFIG_VIEW')"
              type="link"
              @click="payOauth2ConfigFunc(record.isvNo)"
              >Oauth2配置</a-button
            >
            <a-button v-if="$access('ENT_ISV_PAY_CONFIG_LIST')" type="link" @click="payConfigFunc(record.isvNo)"
              >支付配置</a-button
            >
            <a-button v-if="$access('ENT_ISV_PAY_CONFIG_LIST')" type="link" @click="showPayIfConfigList(record.isvNo)"
              >支付配置(新)</a-button
            >
            <a-button v-if="$access('ENT_ISV_INFO_DEL')" type="link" style="color: red" @click="delFunc(record.isvNo)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 支付配置弹窗  -->
    <ag-pay-config-drawer ref="payConfig" :perm-code="'ENT_ISV_PAY_CONFIG_ADD'" :config-mode="'mgrIsv'" />
    <!-- Oauth2配置弹窗  -->
    <ag-pay-oauth2-config-drawer
      ref="payOauth2Config"
      :perm-code="'ENT_ISV_OAUTH2_CONFIG_ADD'"
      :config-mode="'mgrIsv'"
    />
    <!-- 支付接口配置列表页面弹窗  -->
    <IsvPayIfConfigList ref="isvPayIfConfigList" />
  </div>
</template>
<script>
import { AgSearch, AgTable, AgTableActions, AgInput } from '@/components'
import AgPayConfig from '@/components/ag-pay-config'
import AgPayOauth2Config from '@/components/ag-pay-oauth2-config'
import { API_URL_ISV_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'
import IsvPayIfConfigList from './isv-pay-if-config-list.vue'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  {
    key: 'isvName',
    title: '服务商名称',
    width: 160,
    fixed: 'left',
    ellipsis: true,
    customRender: 'isvNameSlot'
  },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'state', title: '服务商状态', width: 140, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

export default {
  name: 'IsvListPage',
  components: {
    'ag-search': AgSearch,
    'ag-table': AgTable,
    'ag-table-actions': AgTableActions,
    'ag-input': AgInput,
    'ag-pay-config': AgPayConfig,
    'ag-pay-oauth2-config': AgPayOauth2Config,
    InfoAddOrEdit,
    IsvPayIfConfigList
  },
  data() {
    return {
      isShowMore: false,
      tableColumns: tableColumns,
      searchData: {}
    }
  },
  mounted() {},
  methods: {
    // 对接table接口函数
    reqTableDataFunc: (params) => {
      return req.list(API_URL_ISV_LIST, params)
    },
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确定删除吗', '确定删除该服务商及其所有关联商户', () => {
        req.delById(API_URL_ISV_LIST, recordId).then((res) => {
          that.$refs.infoTable.reload()
          this.$message.success('删除成功')
        })
      })
    },
    addFunc: function () {
      // 业务通道.服务商管理 新增
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (recordId) {
      // 业务通道.服务商管理 编辑
      this.$refs.infoAddOrEdit.show(recordId)
    },
    payConfigFunc: function (recordId) {
      // 支付配置
      this.$refs.payConfig.show(recordId)
    },
    payOauth2ConfigFunc: function (recordId) {
      // 支付配置
      this.$refs.payOauth2Config.show(recordId)
    },
    showPayIfConfigList: function (recordId) {
      // 支付接口配置
      this.$refs.isvPayIfConfigList.show(recordId)
    }
  }
}
</script>

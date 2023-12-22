<template>
  <div>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId"/>
            <ag-text-up :placeholder="'应用名称'" :msg="searchData.appName" v-model="searchData.appName"/>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="状态" default-value="">
                <a-select-option value="">全部</a-select-option>

                <a-select-option value="0">禁用</a-select-option>
                <a-select-option value="1">启用</a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons" style="flex-grow: 0; flex-shrink: 0;">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
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
        rowKey="appId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_MCH_APP_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="appIdSlot" slot-scope="{record}">
          <b>{{ record.appId }}</b>
        </template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'禁用':'启用'" />
        </template>
        <template slot="defaultFlagSlot" slot-scope="{record}">
          <a-badge :status="record.defaultFlag === 0?'error':'processing'" :text="record.defaultFlag === 0?'否':'是'" />
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_MCH_APP_EDIT')" @click="editFunc(record.appId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_OAUTH2_CONFIG_VIEW') && record.mchType===1" @click="payOauth2ConfigFunc(record.appId, record.mchType)">Oauth2配置</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_PAY_CONFIG_LIST')" @click="payConfigFunc(record.appId, record.mchType)">支付配置</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_PAY_CONFIG_LIST')" @click="showPayIfConfigList(record.appId)">支付配置(旧版)</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_PAY_TEST')">
              <router-link :to="{name:'ENT_MCH_PAY_TEST', params:{appId:record.appId}}">
                支付测试
              </router-link>
            </a-button>
            <a-button type="link" v-if="$access('ENT_MCH_TRANSFER')">
              <router-link :to="{name:'ENT_MCH_TRANSFER', params:{appId:record.appId}}">
                发起转账
              </router-link>
            </a-button>
            <a-button type="link" v-if="$access('ENT_MCH_APP_DEL')" style="color: red" @click="delFunc(record.appId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增应用  -->
    <MchAppAddOrEdit ref="mchAppAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 支付配置组件  -->
    <AgPayConfigDrawer ref="payConfig" :perm-code="'ENT_MCH_PAY_CONFIG_ADD'" :config-mode="configMode" />
    <!-- Oauth2配置组件  -->
    <AgPayOauth2ConfigDrawer ref="payOauth2Config" :perm-code="'ENT_MCH_OAUTH2_CONFIG_ADD'" :config-mode="configMode" />
    <!-- 支付参数配置页面组件  -->
    <MchPayIfConfigList ref="mchPayIfConfigList" />
  </div>
</template>

<script>
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgPayConfigDrawer from '@/components/AgPayConfig/AgPayConfigDrawer'
import AgPayOauth2ConfigDrawer from '@/components/AgPayOauth2Config/AgPayOauth2ConfigDrawer'
import { API_URL_MCH_APP, req } from '@/api/manage'
import MchAppAddOrEdit from './AddOrEdit'
import MchPayIfConfigList from './MchPayIfConfigList'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'appId', title: '应用AppId', width: 320, fixed: 'left', scopedSlots: { customRender: 'appIdSlot' } },
  { key: 'appName', dataIndex: 'appName', title: '应用名称', width: 200 },
  { key: 'state', title: '状态', width: 80, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'defaultFlag', title: '默认', width: 80, scopedSlots: { customRender: 'defaultFlagSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'MchAppPage',
  components: { AgTable, AgTableColumns, AgTextUp, AgPayConfigDrawer, AgPayOauth2ConfigDrawer, MchAppAddOrEdit, MchPayIfConfigList },
  data () {
    return {
      btnLoading: false,
      configMode: 'mchSelfApp1',
      tableColumns: tableColumns,
      searchData: {}
    }
  },
  methods: {
    queryFunc () {
      this.btnLoading = true
      this.$refs.infoTable.refTable(true)
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_MCH_APP, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.mchAppAddOrEdit.show()
    },
    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.mchAppAddOrEdit.show(recordId)
    },
    delFunc (appId) {
      const that = this

      this.$infoBox.confirmDanger('确认删除？', '', () => {
        req.delById(API_URL_MCH_APP, appId).then(res => {
          that.$message.success('删除成功！')
          that.searchFunc()
        })
      })
    },
    payConfigFunc: function (recordId, mchType) { // 支付配置
      this.configMode = `mchSelfApp${mchType}`
      // 更新 configMode 后，使用 $nextTick() 方法来确保 payConfig 组件已经被更新完毕
      this.$nextTick(() => {
        // DOM 更新周期结束后执行该回调函数
        this.$refs.payConfig.show(recordId, mchType === 2)
      })
    },
    payOauth2ConfigFunc: function (recordId, mchType) { // 支付配置
      this.configMode = `mchSelfApp${mchType}`
      // 更新 configMode 后，使用 $nextTick() 方法来确保 payConfig 组件已经被更新完毕
      this.$nextTick(() => {
        // DOM 更新周期结束后执行该回调函数
        this.$refs.payOauth2Config.show(recordId, mchType === 2)
      })
    },
    showPayIfConfigList: function (recordId) { // 支付参数配置
      this.$refs.mchPayIfConfigList.show(recordId)
    }
  }
}
</script>

<style lang="less" scoped>
</style>

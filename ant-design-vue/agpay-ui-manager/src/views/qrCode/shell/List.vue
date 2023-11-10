<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'模板别名'" :msg="searchData.shellAlias" v-model="searchData.shellAlias" />
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
        rowKey="id"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_DEVICE_QRC_SHELL_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a v-if="$access('ENT_DEVICE_QRC_SHELL_VIEW')" @click="onPreview(record.id)">详情</a>
            <a v-if="$access('ENT_DEVICE_QRC_SHELL_EDIT')" @click="editFunc(record.id)">修改</a>
            <a style="color: red" v-if="$access('ENT_DEVICE_QRC_SHELL_DEL')" @click="delFunc(record.id)">删除</a>
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
import { API_URL_QRC_SHELL_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件

const tableColumns = [
  { key: 'shellAlias', dataIndex: 'shellAlias', title: '模板别名' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'QrCodeShellPage',
  components: { AgTable, AgTableColumns, InfoAddOrEdit, AgTextUp },
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
      return req.list(API_URL_QRC_SHELL_LIST, params)
    },

    searchFunc (isToFirst = false) { // 点击【查询】按钮点击事件
      this.btnLoading = true
      this.$refs.infoTable.refTable(isToFirst)
    },

    onPreview (recordId) {
      const that = this
      req.get(API_URL_QRC_SHELL_LIST + '/view/' + recordId).then(res => {
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

    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId)
    },

    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        req.delById(API_URL_QRC_SHELL_LIST, recordId).then(res => {
          that.$message.success('删除成功！')
          that.$refs.infoTable.refTable(false)
        })
      })
    }
  }
}
</script>

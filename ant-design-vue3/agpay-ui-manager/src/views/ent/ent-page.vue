<template>
  <div>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="querySysType" placeholder="选择系统菜单" class="table-head-layout" @change="refTable">
                <a-select-option value="MGR">显示菜单-运营平台</a-select-option>
                <a-select-option value="AGENT">显示菜单-代理商系统</a-select-option>
                <a-select-option value="MCH">显示菜单-商户系统</a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button v-if="$access('ENT_UR_ROLE_ENT_EDIT')" type="primary" @click="setFunc"
                >设置权限匹配规则</a-button
              >
            </span>
          </div>
        </a-form>
      </div>
      <a-table
        row-key="entId"
        :columns="tableColumns"
        :data-source="dataSource"
        :loading="loading"
        :pagination="false"
        :scroll="{ x: 1450 }"
      >
        <template #stateSlot="record">
          <ag-state-switch
            :state="record.state"
            :show-switch-type="$access('ENT_UR_ROLE_ENT_EDIT')"
            :on-change="
              (state) => {
                return updateState(record.entId, state)
              }
            "
          />
        </template>
        <template #opSlot="record">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_UR_ROLE_ENT_EDIT')" type="link" @click="editFunc(record.entId)">编辑</a-button>
          </ag-table-actions>
        </template>
      </a-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="refTable" />
    <!-- 设置权限匹配规则 页面弹窗  -->
    <SetEntMatchRule ref="setEntMatchRule" :callback-func="refTable" />
  </div>
</template>
<script>
import { getEntTree, API_URL_ENT_LIST, reqLoad } from '@/api/manage'
import { AgTableActions, AgStateSwitch } from '@/components'
import InfoAddOrEdit from './add-or-edit.vue'
import SetEntMatchRule from './set-ent-match-rule.vue'

const tableColumns = [
  { key: 'entId', dataIndex: 'entId', title: '资源权限ID', width: 380 }, // key为关键字段，用于标识列的唯一
  { key: 'entName', dataIndex: 'entName', title: '资源名称', width: 200 },
  { key: 'menuIcon', dataIndex: 'menuIcon', title: '图标' },
  { key: 'menuUri', dataIndex: 'menuUri', title: '路径' },
  { key: 'componentName', dataIndex: 'componentName', title: '组件名称' },
  { key: 'entType', dataIndex: 'entType', title: '类型', width: 60 },
  { key: 'state', title: '状态', align: 'center', scopedSlots: { customRender: 'stateSlot' } },
  { key: 'entSort', dataIndex: 'entSort', title: '排序', width: 60 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '修改时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'EntPage',
  components: {
    'ag-state-switch': AgStateSwitch,
    'ag-table-actions': AgTableActions,
    InfoAddOrEdit,
    SetEntMatchRule
  },
  data() {
    return {
      querySysType: 'MGR', // 默认查询运营平台
      tableColumns: tableColumns,
      dataSource: [],
      loading: false
    }
  },
  mounted() {
    this.refTable() // 刷新页面
  },
  methods: {
    refTable: function () {
      const that = this
      that.loading = true
      getEntTree(that.querySysType).then((res) => {
        that.dataSource = res
        that.loading = false
      })
    },

    updateState: function (recordId, state) {
      const that = this
      return reqLoad
        .updateById(API_URL_ENT_LIST, recordId, { state: state, sysType: that.querySysType })
        .then((res) => {
          that.$message.success('更新成功')
          that.refTable() // 刷新页面
        })
    },

    setFunc: function () {
      this.$refs.setEntMatchRule.show()
    },

    editFunc: function (recordId) {
      // 业务通道.编辑. 权限
      this.$refs.infoAddOrEdit.show(recordId, this.querySysType)
    }
  }
}
</script>

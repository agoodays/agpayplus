<template>
  <div class="ent-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <div style="margin-bottom: 16px">
        <ag-search
          v-model:model-value="searchForm"
          :collapsible="false"
          @search="onSearch"
          @reset="onReset"
        >
          <template #base="{ colSpan }">
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.sysType"
                  label="系统类型"
                  placeholder="选择系统菜单"
                  :options="[
                    { value: 'MGR', label: '显示菜单-运营平台' },
                    { value: 'AGENT', label: '显示菜单-代理商系统' },
                    { value: 'MCH', label: '显示菜单-商户系统' }
                  ]"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <a-button
                  v-if="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
                  type="primary"
                  @click="setFunc"
                >
                  设置权限匹配规则
                </a-button>
              </a-form-item>
            </a-col>
          </template>
        </ag-search>
      </div>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="columns"
        :show-auto-refresh="true"
        :on-load="reqTableDataFunc"
        :search-data="searchForm"
        :pagination="false"
        :scroll-x="1450"
        state-key="ent_table_columns"
      >
        <template #state="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch-type="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
            :on-change="(state) => updateState(record.entId, state)"
          />
        </template>
        <template #actions="{ record }">
          <ag-table-actions :max-show-num="3">
            <a-button
              v-if="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
              type="link"
              size="small"
              @click="editFunc(record.entId)"
            >
              编辑
            </a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增 / 编辑 页面弹窗 -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="refTable" />
    <!-- 设置权限匹配规则 页面弹窗 -->
    <SetEntMatchRule ref="setEntMatchRule" :callback-func="refTable" />
  </div>
</template>

<script setup>
import { entApi } from '@/api/business/ent/ent-api'
import { AgSearch, AgSelect, AgStateSwitch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { onMounted, reactive, ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'
import SetEntMatchRule from './set-ent-match-rule.vue'

const { hasPermission } = usePermission()

// State
const tableRef = ref(null)
const infoAddOrEdit = ref(null)
const setEntMatchRule = ref(null)

// 搜索表单
const searchForm = reactive({
  sysType: 'MGR'
})

// 表格列定义
const columns = [
  {
    key: 'entId',
    dataIndex: 'entId',
    title: '资源权限ID',
    width: 380
  },
  {
    key: 'entName',
    dataIndex: 'entName',
    title: '资源名称',
    width: 200
  },
  {
    key: 'menuIcon',
    dataIndex: 'menuIcon',
    title: '图标'
  },
  {
    key: 'menuUri',
    dataIndex: 'menuUri',
    title: '路径'
  },
  {
    key: 'componentName',
    dataIndex: 'componentName',
    title: '组件名称'
  },
  {
    key: 'entType',
    dataIndex: 'entType',
    title: '类型',
    width: 60
  },
  {
    key: 'state',
    title: '状态',
    align: 'center',
    width: 100,
    customRender: 'state'
  },
  {
    key: 'entSort',
    dataIndex: 'entSort',
    title: '排序',
    width: 60
  },
  {
    key: 'updatedAt',
    dataIndex: 'updatedAt',
    title: '修改时间',
    width: 200
  },
  {
    key: 'actions',
    title: '操作',
    width: 100,
    fixed: 'right',
    align: 'center',
    customRender: 'actions'
  }
]

/**
 * 初始化
 */
onMounted(() => {
})

// 请求表格数据函数
const reqTableDataFunc = (params) => {
  return entApi.queryEntTree(searchForm.sysType).then(res => {
    return {
      records: res,
      total: res.length
    }
  })
}

/**
 * 刷新表格
 */
const refTable = () => {
  tableRef.value?.reload()
}

/**
 * 更新状态
 */
const updateState = (recordId, state) => {
  return entApi.updateStateById(recordId, state, searchForm.sysType).then(() => {
    window.$message.success('更新成功')
    refTable()
  })
}

/**
 * 设置权限匹配规则
 */
const setFunc = () => {
  setEntMatchRule.value?.show()
}

/**
 * 编辑
 */
const editFunc = (recordId) => {
  infoAddOrEdit.value?.show(recordId, searchForm.sysType)
}

/**
 * 搜索
 */
function onSearch() {
  tableRef.value?.reload()
}

/**
 * 重置
 */
function onReset() {
  searchForm.sysType = 'MGR'
  tableRef.value?.reload()
}
</script>

<style lang="less" scoped>
.ent-page {
  width: 100%;
  height: 100%;
  padding: 0;
  margin: 0;
}
</style>
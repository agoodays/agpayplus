<template>
  <div class="ent-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="false"
        @search="searchFunc"
        @reset="onReset"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.sysType"
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

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="columns"
        :show-auto-refresh="true"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        :pagination="false"
        :scroll-x="1450"
        state-key="ent_table_columns"
      >
        <!-- 状态列自定义渲染 -->
        <template #state="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch-type="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
            :on-change="(state) => updateState(record.entId, state)"
          />
        </template>

        <!-- 操作列 -->
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

    <!-- 新增/编辑弹窗 -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="refTable" />

    <!-- 设置权限匹配规则弹窗 -->
    <SetEntMatchRule ref="setEntMatchRule" :callback-func="refTable" />
  </div>
</template>

<script setup>
/**
 * 资源权限列表页面组件
 * 功能：展示资源权限列表、搜索、编辑、状态切换、设置权限匹配规则等操作
 */

import { entApi } from '@/api/business/ent/ent-api'
import { AgSearch, AgSelect, AgStateSwitch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { onMounted, reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import InfoAddOrEdit from './add-or-edit.vue'
import SetEntMatchRule from './set-ent-match-rule.vue'

// 权限检查
const { hasPermission } = usePermission()

// 组件引用
const tableRef = ref(null)
const infoAddOrEdit = ref(null)
const setEntMatchRule = ref(null)

/**
 * 搜索表单数据
 */
const searchData = reactive({
  sysType: 'MGR'
})

/**
 * 表格列配置
 */
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

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  const res = await entApi.queryEntTree(searchData.sysType)
  return {
    records: res,
    total: res.length
  }
}

/**
 * 刷新表格
 */
const refTable = () => {
  tableRef.value?.reload()
}

/**
 * 更新状态
 * @param {string} recordId - 资源权限ID
 * @param {number} state - 状态值
 */
const updateState = async (recordId, state) => {
  await entApi.updateStateById(recordId, state, searchData.sysType)
  message.success('更新成功')
  refTable()
}

/**
 * 设置权限匹配规则
 */
const setFunc = () => {
  setEntMatchRule.value?.show()
}

/**
 * 编辑
 * @param {string} recordId - 资源权限ID
 */
const editFunc = (recordId) => {
  infoAddOrEdit.value?.show(recordId, searchData.sysType)
}

/**
 * 搜索回调函数
 */
const searchFunc = () => {
  tableRef.value?.reload()
}

/**
 * 重置回调函数
 */
const onReset = () => {
  searchData.sysType = 'MGR'
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

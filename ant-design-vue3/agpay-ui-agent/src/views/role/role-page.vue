<template>
  <div class="role-page">
    <a-card :bordered="false">
      <ag-search
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :collapsible="false"
        :default-collapsed="true"
        :search-loading="searchLoading"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.roleId" label="角色ID" placeholder="请输入角色ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.roleName" label="角色名称" placeholder="请输入角色名称" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="tableRef"
        row-key="roleId"
        state-key="role"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <template #title="{ record }">
          <b>{{ record.roleId }}</b>
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button type="link" @click="editFunc(record.roleId)">修改</a-button>
            <a-popconfirm title="确认删除？" ok-text="删除" ok-type="danger" @confirm="delFunc(record.roleId)">
              <a-button type="link" danger>删除</a-button>
            </a-popconfirm>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <a-drawer
      v-model:open="drawerOpen"
      :title="isAdd ? '新增角色' : '修改角色'"
      width="30%"
      :mask-closable="false"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    >
      <a-form ref="formRef" :model="saveObject" :label-col="{ span: 4 }" :wrapper-col="{ span: 15 }" :rules="rules">
        <a-form-item label="角色名称" name="roleName">
          <a-input v-model:value="saveObject.roleName" placeholder="请输入角色名称" />
        </a-form-item>
      </a-form>

      <role-dist ref="roleDistRef" sys-type="AGENT" />

      <div class="drawer-footer">
        <a-button style="margin-right: 8px" @click="drawerOpen = false">取消</a-button>
        <a-button type="primary" :loading="confirmLoading" @click="handleOkFunc">保存</a-button>
      </div>
    </a-drawer>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { AgSearch, AgTable, AgTableActions, AgInput } from '@/components'
import RoleDist from './role-dist.vue'
import { roleApi } from '@/api/business/role/role-api'
import { useCrudTablePage } from '@/composables/useCrudTablePage'

const { tableRef, searchData, defaultSearchData, searchFunc, searchLoading } = useCrudTablePage({
  searchDefaults: { roleId: '', roleName: '' }
})

const drawerOpen = ref(false)
const isAdd = ref(true)
const confirmLoading = ref(false)
const formRef = ref(null)
const roleDistRef = ref(null)
const recordId = ref(null)

const saveObject = reactive({})
const rules = {
  roleName: [{ required: true, message: '请输入角色名称', trigger: 'blur' }]
}

const tableColumns = [
  { key: 'roleId', title: '角色ID', width: 130, customRender: 'titleSlot' },
  { key: 'roleName', dataIndex: 'roleName', title: '角色名称', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const loadDataFunc = async (params) => {
  return await roleApi.queryPage(params)
}

const addFunc = () => {
  isAdd.value = true
  Object.assign(saveObject, { roleName: '' })
  recordId.value = null
  formRef.value?.resetFields()
  drawerOpen.value = true
  roleDistRef.value?.initTree(null)
}

const editFunc = async (id) => {
  isAdd.value = false
  recordId.value = id
  const res = await roleApi.getById(id)
  Object.assign(saveObject, res || {})
  formRef.value?.resetFields()
  drawerOpen.value = true
  roleDistRef.value?.initTree(id)
}

const delFunc = async (id) => {
  await roleApi.delById(id)
  message.success('删除成功')
  searchFunc()
}

const handleOkFunc = async () => {
  try {
    await formRef.value.validate()
  } catch { return }

  confirmLoading.value = true
  try {
    const selectedEntIdList = roleDistRef.value?.getSelectedEntIdList() || []
    saveObject.entIds = selectedEntIdList

    if (isAdd.value) {
      await roleApi.add({ ...saveObject })
      message.success('新增成功')
    } else {
      await roleApi.updateById(recordId.value, { ...saveObject })
      message.success('修改成功')
    }
    drawerOpen.value = false
    searchFunc()
  } catch {
  } finally {
    confirmLoading.value = false
  }
}

onMounted(() => {
  searchFunc()
})

defineExpose({ addFunc, editFunc })
</script>

<style lang="less" scoped>
.drawer-footer {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 10px 16px;
  border-top: 1px solid #f0f0f0;
  text-align: right;
  background: #fff;
}
</style>

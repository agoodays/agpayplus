<template>
  <div class="sysuser-page">
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
              <ag-input v-model="searchData.sysUserId" label="用户ID" placeholder="请输入用户ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.realname" label="用户姓名" placeholder="请输入用户姓名" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select v-model="searchData.userType" label="操作员类型" :options="userTypeOptions" placeholder="全部" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="tableRef"
        row-key="sysUserId"
        state-key="sys-user"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <template #avatarSlot="{ record }">
          <a-avatar :src="record.avatarUrl" />
        </template>

        <template #realnameSlot="{ record }">
          <span>{{ record.realname }}<a-tag v-if="record.initUser" color="green" style="margin-left:4px">初始</a-tag></span>
        </template>

        <template #userTypeSlot="{ record }">
          <span>{{ getUserTypeName(record.userType) }}</span>
        </template>

        <template #inviteCodeSlot="{ record }">
          <template v-if="record.inviteCode">
            <b>{{ record.inviteCode }}</b>
            <a-button type="link" size="small" @click="copyFunc(record.inviteCode)">复制</a-button>
            <a-button type="link" size="small" @click="showInviteCode(record.inviteCode, record.sysType)">查看</a-button>
          </template>
          <span v-else>-</span>
        </template>

        <template #stateSlot="{ record }">
          <ag-state-switch
            :checked="record.state === 1"
            :loading="false"
            @update:checked="(val) => updateState(record.sysUserId, val ? 1 : 0)"
          />
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button type="link" v-if="record.userType === 2" @click="showRoleDist(record.sysUserId)">变更角色</a-button>
            <a-button type="link" @click="editFunc(record.sysUserId)">修改</a-button>
            <a-popconfirm title="确认解除？" ok-text="解除" @confirm="relieveFunc(record.sysUserId)">
              <a-button type="link" danger>解除限制</a-button>
            </a-popconfirm>
            <a-popconfirm title="确认删除？" ok-text="删除" ok-type="danger" @confirm="delFunc(record.sysUserId)">
              <a-button type="link" danger>删除</a-button>
            </a-popconfirm>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <a-drawer
      v-model:open="userDrawerOpen"
      :title="isAdd ? '新增用户' : '修改用户'"
      width="38%"
      :mask-closable="false"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    >
      <a-form ref="formRef" :model="saveObject" :label-col="{ span: 5 }" :wrapper-col="{ span: 18 }" :rules="formRules">
        <a-form-item label="用户姓名" name="realname">
          <a-input v-model:value="saveObject.realname" placeholder="请输入用户姓名" />
        </a-form-item>
        <a-form-item label="手机号" name="telphone">
          <a-input v-model:value="saveObject.telphone" placeholder="请输入手机号" maxlength="11" />
        </a-form-item>
        <a-form-item label="登录账号" name="loginUsername">
          <a-input v-model:value="saveObject.loginUsername" :disabled="!isAdd" placeholder="请输入登录账号" />
        </a-form-item>
        <a-form-item v-if="isAdd" label="登录密码" name="password">
          <a-input-password v-model:value="saveObject.password" placeholder="请输入登录密码" />
        </a-form-item>
        <a-form-item label="性别" name="sex">
          <a-radio-group v-model:value="saveObject.sex">
            <a-radio :value="1">男</a-radio>
            <a-radio :value="2">女</a-radio>
            <a-radio :value="0">未知</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="操作员类型" name="userType">
          <a-select v-model:value="saveObject.userType" :options="userTypeOptions.filter(o => o.value !== '')" placeholder="请选择操作员类型" />
        </a-form-item>
        <a-form-item label="状态" name="state">
          <a-radio-group v-model:value="saveObject.state">
            <a-radio :value="1">启用</a-radio>
            <a-radio :value="0">停用</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-form>
      <div class="drawer-footer">
        <a-button style="margin-right: 8px" @click="userDrawerOpen = false">取消</a-button>
        <a-button type="primary" :loading="confirmLoading" @click="handleSave">保存</a-button>
      </div>
    </a-drawer>

    <a-drawer
      v-model:open="roleDrawerOpen"
      title="分配角色"
      width="30%"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    >
      <div style="padding-bottom: 20px">
        <p>请选择角色：</p>
        <a-tree
          :tree-data="roleTreeData"
          :field-names="{ key: 'roleId', title: 'roleName' }"
          v-model:checked-keys="roleCheckedKeys"
          :checkable="true"
        />
      </div>
      <div class="drawer-footer">
        <a-button style="margin-right: 8px" @click="roleDrawerOpen = false">取消</a-button>
        <a-button type="primary" :loading="roleLoading" @click="saveRoleDist">保存</a-button>
      </div>
    </a-drawer>

    <a-modal
      v-model:open="inviteCodeOpen"
      title="邀请码"
      :footer="null"
      width="420px"
    >
      <div v-if="inviteCodeData">
        <a-descriptions :column="1" bordered size="small">
          <a-descriptions-item label="邀请码">{{ inviteCodeData.inviteCode }}</a-descriptions-item>
          <a-descriptions-item label="所属系统">{{ inviteCodeData.sysType }}</a-descriptions-item>
          <a-descriptions-item label="状态">{{ inviteCodeData.state === 1 ? '有效' : '无效' }}</a-descriptions-item>
          <a-descriptions-item label="创建时间">{{ inviteCodeData.createdAt }}</a-descriptions-item>
          <a-descriptions-item label="有效期至">{{ inviteCodeData.expireTime || '长期有效' }}</a-descriptions-item>
        </a-descriptions>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { AgSearch, AgTable, AgTableActions, AgInput, AgSelect, AgStateSwitch } from '@/components'
import { sysUserApi } from '@/api/business/sys-user/sys-user-api'
import { roleApi } from '@/api/business/role/role-api'
import { useCrudTablePage } from '@/composables/useCrudTablePage'

const { tableRef, searchData, defaultSearchData, searchFunc, searchLoading } = useCrudTablePage({
  searchDefaults: { sysUserId: '', realname: '', userType: '' }
})

const tableColumns = [
  { key: 'avatar', title: '头像', width: 65, customRender: 'avatarSlot' },
  { key: 'realname', title: '姓名', width: 135, fixed: 'left', customRender: 'realnameSlot' },
  { key: 'sysUserId', dataIndex: 'sysUserId', title: '用户ID', width: 180 },
  { key: 'sex', title: '性别', width: 65, customRender: ({ record }) => record.sex === 1 ? '男' : record.sex === 2 ? '女' : '未知' },
  { key: 'telphone', dataIndex: 'telphone', title: '手机号', width: 140 },
  { key: 'userType', title: '操作员类型', width: 120, customRender: 'userTypeSlot' },
  { key: 'inviteCode', title: '邀请码', width: 180, align: 'center', customRender: 'inviteCodeSlot' },
  { key: 'state', title: '状态', width: 100, align: 'center', customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 220, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const userTypeList = [
  { userTypeName: '全部', userType: '', value: '' },
  { userTypeName: '超级管理员', userType: 1, value: 1 },
  { userTypeName: '普通操作员', userType: 2, value: 2 },
  { userTypeName: '店长', userType: 11, value: 11 },
  { userTypeName: '店员', userType: 12, value: 12 }
]
const userTypeOptions = userTypeList.filter((o) => o.value !== '')

const loadDataFunc = async (params) => await sysUserApi.queryPage(params)
const getUserTypeName = (userType) => userTypeList.find((f) => f.userType === userType)?.userTypeName || '-'

const copyFunc = (text) => {
  if (navigator.clipboard) {
    navigator.clipboard.writeText(text).then(() => message.success('邀请码已复制'))
  }
}

const userDrawerOpen = ref(false)
const isAdd = ref(true)
const confirmLoading = ref(false)
const formRef = ref(null)
const editId = ref(null)
const saveObject = reactive({ realname: '', telphone: '', loginUsername: '', password: '', sex: 1, userType: 2, state: 1 })
const formRules = {
  realname: [{ required: true, message: '请输入用户姓名', trigger: 'blur' }],
  telphone: [{ required: true, message: '请输入手机号', trigger: 'blur' }],
  loginUsername: [{ required: true, message: '请输入登录账号', trigger: 'blur' }],
  userType: [{ required: true, message: '请选择操作员类型', trigger: 'change' }]
}

const addFunc = () => {
  isAdd.value = true
  Object.assign(saveObject, { realname: '', telphone: '', loginUsername: '', password: '', sex: 1, userType: 2, state: 1 })
  editId.value = null
  userDrawerOpen.value = true
}

const editFunc = async (id) => {
  isAdd.value = false
  editId.value = id
  const res = await sysUserApi.getById(id)
  Object.assign(saveObject, res || {})
  userDrawerOpen.value = true
}

const handleSave = async () => {
  try { await formRef.value.validate() } catch { return }
  confirmLoading.value = true
  try {
    if (isAdd.value) {
      await sysUserApi.add({ ...saveObject })
      message.success('新增成功')
    } else {
      await sysUserApi.updateById(editId.value, { ...saveObject })
      message.success('修改成功')
    }
    userDrawerOpen.value = false
    searchFunc()
  } catch {
  } finally {
    confirmLoading.value = false
  }
}

const delFunc = async (id) => {
  await sysUserApi.delById(id)
  message.success('删除成功')
  searchFunc()
}

const relieveFunc = async (id) => {
  await sysUserApi.relieveLoginLimit(id)
  message.success('解除成功')
  searchFunc()
}

const updateState = async (id, state) => {
  const title = state === 1 ? '确认[启用]该用户？' : '确认[停用]该用户？'
  await new Promise((resolve, reject) => {
    import('ant-design-vue').then(({ Modal }) => {
      Modal.confirm({
        title,
        content: state === 1 ? '启用后用户可进行登陆等一系列操作' : '停用后该用户将立即退出系统并不可再次登陆',
        okText: '确认', cancelText: '取消',
        onOk: () => resolve(),
        onCancel: () => reject()
      })
    })
  })
  await sysUserApi.updateStateById(id, { state })
  message.success('更新成功')
  searchFunc()
}

const roleDrawerOpen = ref(false)
const roleTreeData = ref([])
const roleCheckedKeys = ref([])
const roleUserId = ref(null)
const roleLoading = ref(false)

const showRoleDist = async (userId) => {
  roleUserId.value = userId
  const res = await roleApi.queryPage({ pageSize: -1 })
  roleTreeData.value = (res?.records || []).map((r) => ({ roleId: r.roleId, roleName: r.roleName }))
  const relaRes = await sysUserApi.queryUserRoleRelaPage({ sysUserId: userId, pageSize: -1 })
  roleCheckedKeys.value = (relaRes?.records || []).map((r) => r.roleId)
  roleDrawerOpen.value = true
}

const saveRoleDist = async () => {
  roleLoading.value = true
  try {
    await sysUserApi.updateUserRoleRela(roleUserId.value, roleCheckedKeys.value)
    message.success('保存成功')
    roleDrawerOpen.value = false
  } catch {
  } finally {
    roleLoading.value = false
  }
}

const inviteCodeOpen = ref(false)
const inviteCodeData = ref(null)
const showInviteCode = (inviteCode, sysType) => {
  inviteCodeData.value = { inviteCode, sysType, state: 1, createdAt: '-', expireTime: '-' }
  inviteCodeOpen.value = true
}

onMounted(() => searchFunc())
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

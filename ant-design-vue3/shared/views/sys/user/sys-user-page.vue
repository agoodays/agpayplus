<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索区域 -->
      <ag-search
        v-if="hasPermission('ENT_UR_USER_SEARCH')"
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :search-loading="searchLoading"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.sysType"
                label="所属系统"
                placeholder="请选择所属系统"
                allow-clear
                :options="sysTypeOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.belongInfoId" label="所属代理商/商户" placeholder="请输入所属代理商/商户" />
            </a-form-item>
          </a-col>
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
              <ag-select
                v-model="searchData.userType"
                label="用户类型"
                placeholder="请选择用户类型"
                allow-clear
                :options="userTypeOptions"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="sysUserId"
        state-key="sys_user"
        :on-load="loadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <!-- 工具栏左侧 -->
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_UR_USER_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>

        <!-- 头像列 -->
        <template #avatarSlot="{ record }">
          <a-avatar size="default" :src="record.avatarUrl" />
        </template>

        <!-- 姓名列 -->
        <template #realnameSlot="{ record }">
          <span>
            {{ record.realname }}
            <a-tag v-if="record.initUser" :color="'green'">初始</a-tag>
          </span>
        </template>

        <!-- 性别列 -->
        <template #sexSlot="{ record }">
          {{ record.sex === 1 ? '男' : record.sex === 2 ? '女' : '未知' }}
        </template>

        <!-- 所属系统列 -->
        <template #sysTypeSlot="{ record }">
          <a-tag :color="getSysTypeColor(record.sysType)">
            {{ getSysTypeText(record.sysType) }}
          </a-tag>
        </template>

        <!-- 用户类型列 -->
        <template #userTypeSlot="{ record }">
          <span>{{ getUserTypeName(record.userType) }}</span>
        </template>

        <!-- 邀请码列 -->
        <template #inviteCodeSlot="{ record }">
          <b>{{ record.inviteCode }}</b>
          <span v-if="record.inviteCode">
            <a-button type="link" @click="copyFunc(record.inviteCode)">
              <template #icon><CopyOutlined /></template>
            </a-button>
            <a-button type="link" @click="openInviteCode(record.inviteCode, record.sysType)">
              <template #icon><InfoCircleOutlined /></template>
            </a-button>
          </span>
        </template>

        <!-- 状态列 -->
        <template #stateSlot="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch="hasPermission('ENT_UR_USER_EDIT')"
            :on-change="(state) => updateState(record.sysUserId, state)"
          />
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button
              v-if="hasPermission('ENT_UR_USER_EDIT')"
              type="link"
              @click="editFunc(record.sysUserId, record.sysType, record.belongInfoId)"
            >
              修改
            </a-button>
            <a-button
              v-if="hasPermission('ENT_UR_USER_UPD_ROLE') && record.userType === 2"
              type="link"
              @click="openRoleDist(record.sysUserId, record.sysType, record.belongInfoId)"
            >
              变更角色
            </a-button>
            <a-button
              v-if="hasPermission('ENT_UR_USER_LOGIN_LIMIT_DELETE')"
              type="link"
              style="color: red"
              @click="relieveFunc(record.sysUserId)"
            >
              解除登录限制
            </a-button>
            <a-button
              v-if="hasPermission('ENT_UR_USER_DELETE')"
              type="link"
              style="color: red"
              @click="delFunc(record.sysUserId)"
            >
              删除
            </a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑抽屉 -->
    <add-or-edit
      v-model:open="modalOpen"
      :record-id="currentRecordId"
      :sys-type="searchData.sysType"
      :belong-info-id="currentBelongInfoId"
      @success="handleModalSuccess"
    />

    <!-- 邀请码弹窗 -->
    <invite-code v-model:open="inviteCodeOpen" :invite-code="currentInviteCode" :sys-type="currentSysType" />

    <!-- 分配角色抽屉 -->
    <role-dist
      v-model:open="roleDistOpen"
      :record-id="currentRoleDistId"
      :sys-type="currentRoleDistSysType"
      :belong-info-id="currentRoleDistBelongInfoId"
      @success="handleRoleDistSuccess"
    />
  </div>
</template>

<script setup>
/**
 * 系统用户列表页面组件
 * 功能：展示系统用户列表、搜索、新增、编辑、删除、状态切换、分配角色等操作
 */
import { sysUserApi } from '@/api/business/sys-user/sys-user-api'
import { AgInput, AgSearch, AgSelect, AgStateSwitch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getStateInfo, getStateOptions, getSysTypeOptions } from '@/constants/common-const'
import { infoBox } from '@/utils/info-box'
import { CopyOutlined, InfoCircleOutlined, PlusOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import AddOrEdit from './add-or-edit.vue'
import InviteCode from './invite-code.vue'
import RoleDist from './role-dist.vue'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))
const sysTypeOptions = computed(() => getSysTypeOptions(t))

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 用户类型列表
 */
const userTypeList = [
  { userTypeName: '超级管理员', userType: 1 },
  { userTypeName: '普通操作员', userType: 2 },
  { userTypeName: '商户拓展员', userType: 3 },
  { userTypeName: '店长', userType: 11 },
  { userTypeName: '店员', userType: 12 }
]

/**
 * 用户类型选项
 */
const userTypeOptions = userTypeList.map((item) => ({
  label: item.userTypeName,
  value: item.userType
}))

/** 当前所属信息ID（用于编辑） */
const currentBelongInfoId = ref('')

/** 邀请码弹窗状态 */
const inviteCodeOpen = ref(false)
const currentInviteCode = ref('')
const currentSysType = ref('')

/** 角色分配弹窗状态 */
const roleDistOpen = ref(false)
const currentRoleDistId = ref('')
const currentRoleDistSysType = ref('')
const currentRoleDistBelongInfoId = ref('')

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  defaultSearchData,
  searchLoading,
  modalOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => sysUserApi.delById(recordId),
  deleteConfirmTitle: '确认删除？',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功！',
  searchDefaults: {
    sysType: 'MGR',
    belongInfoId: '',
    sysUserId: '',
    realname: '',
    userType: ''
  }
})

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'avatar', title: '头像', width: 65, fixed: 'left', customRender: 'avatarSlot' },
  { key: 'realname', title: '姓名', width: 135, fixed: 'left', customRender: 'realnameSlot' },
  { key: 'sysUserId', dataIndex: 'sysUserId', title: '用户ID', width: 120, fixed: 'left' },
  { key: 'sex', dataIndex: 'sex', title: '性别', width: 65, customRender: 'sexSlot' },
  { key: 'userNo', dataIndex: 'userNo', title: '编号', width: 125 },
  { key: 'telphone', dataIndex: 'telphone', title: '手机号', width: 160 },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'userType', title: '操作员类型', width: 120, customRender: 'userTypeSlot' },
  { key: 'teamName', dataIndex: 'teamName', title: '团队', width: 160 },
  { key: 'inviteCode', title: '邀请码', width: 160, customRender: 'inviteCodeSlot', align: 'center' },
  { key: 'state', title: '状态', width: 100, customRender: 'stateSlot', align: 'center' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '修改时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 获取系统类型颜色
 * @param {string} sysType - 系统类型
 * @returns {string} 颜色值
 */
const getSysTypeColor = (sysType) => {
  const colorMap = {
    MGR: 'green',
    AGENT: 'cyan',
    MCH: 'geekblue'
  }
  return colorMap[sysType] || 'default'
}

/**
 * 获取系统类型文本
 * @param {string} sysType - 系统类型
 * @returns {string} 文本值
 */
const getSysTypeText = (sysType) => {
  const textMap = {
    MGR: '运营平台',
    AGENT: '代理商系统',
    MCH: '商户系统'
  }
  return textMap[sysType] || '其他'
}

/**
 * 复制邀请码到剪贴板
 * @param {string} text - 邀请码
 */
const copyFunc = async (text) => {
  try {
    await navigator.clipboard.writeText(text)
    message.success('邀请码已复制')
  } catch (err) {
    const el = document.createElement('input')
    el.setAttribute('value', text)
    document.body.appendChild(el)
    el.select()
    document.execCommand('copy')
    document.body.removeChild(el)
    message.success('邀请码已复制')
  }
}

/**
 * 打开邀请码详情窗口
 * @param {string} inviteCodeValue - 邀请码
 * @param {string} sysTypeValue - 系统类型
 */
const openInviteCode = (inviteCodeValue, sysTypeValue) => {
  currentInviteCode.value = inviteCodeValue
  currentSysType.value = sysTypeValue
  inviteCodeOpen.value = true
}

/**
 * 获取用户类型名称
 * @param {number} userType - 用户类型
 * @returns {string} 用户类型名称
 */
const getUserTypeName = (userType) => {
  return userTypeList.find((f) => f.userType === userType)?.userTypeName || ''
}

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
  return await sysUserApi.queryPage(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => {
  reloadTable()
}

/**
 * 打开新增抽屉
 */
const addFunc = () => {
  currentBelongInfoId.value = ''
  openCreate()
}

/**
 * 打开编辑抽屉
 * @param {string} recordId - 用户ID
 * @param {string} sysType - 系统类型
 * @param {string} belongInfoId - 所属信息ID
 */
const editFunc = (recordId, sysType, belongInfoId) => {
  currentBelongInfoId.value = belongInfoId
  openEdit(recordId)
}

/**
 * 解除登录限制
 * @param {string} recordId - 用户ID
 */
const relieveFunc = async (recordId) => {
  infoBox.confirmPrimary('确认解除吗？', '', async () => {
    await sysUserApi.relieveLoginLimit(recordId)
    message.success('解除成功！')
    reloadTable()
  })
}

/**
 * 删除用户
 * @param {string} recordId - 用户ID
 */
const delFunc = (recordId) => {
  confirmDelete(recordId)
}

/**
 * 打开分配角色抽屉
 * @param {string} recordId - 用户ID
 * @param {string} sysType - 系统类型
 * @param {string} belongInfoId - 所属信息ID
 */
const openRoleDist = (recordId, sysType, belongInfoId) => {
  currentRoleDistId.value = recordId
  currentRoleDistSysType.value = sysType
  currentRoleDistBelongInfoId.value = belongInfoId
  roleDistOpen.value = true
}

/**
 * 更新用户状态
 * @param {string} recordId - 用户ID
 * @param {number} state - 状态值
 */
const updateState = async (recordId, state) => {
  const stateInfo = getStateInfo(state)
  const title = `确认[${stateInfo.desc}]该用户？`
  const content =
    stateInfo === ENABLE_ENUM.ENABLE ? '启用后用户可进行登陆等一系列操作' : '停用后该用户将立即退出系统并不可再次登陆'

  infoBox.confirmPrimary(title, content, async () => {
    await sysUserApi.updateStateById(recordId, { state })
    reloadTable()
  })
}

/**
 * 处理新增/编辑成功
 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}

/**
 * 处理角色分配成功
 */
const handleRoleDistSuccess = () => {
  roleDistOpen.value = false
  reloadTable()
}
</script>

<style scoped></style>

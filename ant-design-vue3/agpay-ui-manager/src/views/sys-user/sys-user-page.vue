<template>
  <div>
    <a-card>
      <!-- 搜索区域 -->
      <ag-search v-if="hasPermission('ENT_UR_USER_SEARCH')" v-model="searchData" :search-loading="btnLoading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.sysType"
                label="所属系统"
                placeholder="请选择所属系统"
                allow-clear
                :options="[
                  { value: 'MGR', label: '运营平台' },
                  { value: 'AGENT', label: '代理商' },
                  { value: 'MCH', label: '商户' }
                ]"
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
                v-model:value="searchData.userType"
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
        ref="infoTable"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
        row-key="sysUserId"
        @btn-load-close="btnLoading = false"
      >
        <!-- 工具栏左侧 -->
        <template #toolbar-left>
          <div>
            <a-button v-if="hasPermission('ENT_UR_USER_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
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
        <template #inviteCodeSlot="{ record }" v-if="record.inviteCode">
          <b>{{ record.inviteCode }}</b>
          <a-button icon="copy" type="link" @click="copyFunc(record.inviteCode)"/>
          <a-button icon="info-circle" type="link" @click="inviteCodeFunc(record.inviteCode, record.sysType)"/>
        </template>

        <!-- 状态列 -->
        <template #stateSlot="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch-type="hasPermission('ENT_UR_USER_EDIT')"
            :on-change="(state) => updateState(record.sysUserId, state)"
          />
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button
              v-if="hasPermission('ENT_UR_USER_UPD_ROLE') && record.userType === 2"
              type="link"
              @click="roleDist(record.sysUserId, record.sysType, record.belongInfoId)"
            >
              变更角色
            </a-button>
            <a-button
              v-if="hasPermission('ENT_UR_USER_EDIT')"
              type="link"
              @click="editFunc(record.sysUserId, record.sysType, record.belongInfoId)"
            >
              修改
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

    <!-- 新增/编辑弹窗 -->
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="searchFunc" />

    <!-- 邀请码窗口 -->
    <invite-code ref="inviteCodeRef" />

    <!-- 分配角色弹窗 -->
    <role-dist ref="roleDistRef" />
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
import { onMounted, reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import InfoAddOrEdit from './add-or-edit.vue'
import InviteCode from './invite-code.vue'
import RoleDist from './role-dist.vue'

// 权限检查
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
 * 默认查询条件
 */
const defaultSearchData = {
  userType: '',
  sysType: 'MGR'
}

/**
 * 组件引用
 */
const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const inviteCodeRef = ref(null)
const roleDistRef = ref(null)

/**
 * 搜索数据
 */
const searchData = reactive({ ...defaultSearchData })

/**
 * 用户类型选项
 */
const userTypeOptions = userTypeList

/**
 * 加载状态
 */
const btnLoading = ref(false)

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'avatar', title: '头像', width: 65, fixed: 'left', customRender: 'avatarSlot' },
  { key: 'realname', title: '姓名', width: 135, fixed: 'left', customRender: 'realnameSlot' },
  { key: 'sysUserId', dataIndex: 'sysUserId', title: '用户ID', width: 120, fixed: 'left' },
  { key: 'sex', dataIndex: 'sex', title: '性别', width: 65, customRender: (text, record) => (record.sex === 1 ? '男' : record.sex === 2 ? '女' : '未知') },
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
  { key: 'op', title: '操作', width: 180, fixed: 'right', align: 'center', customRender: 'opSlot' }
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
const copyFunc = (text) => {
  const el = document.createElement('input')
  el.setAttribute('value', text)
  document.body.appendChild(el)
  el.select()
  document.execCommand('copy')
  document.body.removeChild(el)
  message.success('邀请码已复制')
}

/**
 * 打开邀请码详情窗口
 * @param {string} inviteCodeValue - 邀请码
 * @param {string} sysType - 系统类型
 */
const inviteCodeFunc = (inviteCodeValue, sysType) => {
  inviteCodeRef.value.show(inviteCodeValue, sysType)
}

/**
 * 获取用户类型名称
 * @param {number} userType - 用户类型
 * @returns {string} 用户类型名称
 */
const getUserTypeName = (userType) => {
  return userTypeList.find(f => f.userType === userType)?.userTypeName || ''
}

/**
 * 处理搜索表单数据
 * @param {Object} data - 搜索数据
 */
const handleSearchFormData = (data) => {
  if (!data || Object.keys(data).length === 0) {
    Object.assign(searchData, defaultSearchData)
  } else {
    Object.assign(searchData, data)
  }
}

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await sysUserApi.queryPage(params)
}

/**
 * 搜索函数
 * @param {boolean} isToFirst - 是否跳转到第一页
 */
const searchFunc = (isToFirst = false) => {
  infoTable.value?.reload(isToFirst)
}

/**
 * 打开新增弹窗
 */
const addFunc = () => {
  infoAddOrEdit.value.show()
}

/**
 * 打开编辑弹窗
 * @param {string} recordId - 用户ID
 * @param {string} sysType - 系统类型
 * @param {string} belongInfoId - 所属信息ID
 */
const editFunc = (recordId, sysType, belongInfoId) => {
  infoAddOrEdit.value.show(recordId, sysType, belongInfoId)
}

/**
 * 解除登录限制
 * @param {string} recordId - 用户ID
 */
const relieveFunc = async (recordId) => {
  const { infoBox } = await import('@/utils/info-box')
  infoBox.confirmDanger('确认解除吗？', '', async () => {
    await sysUserApi.relieveLoginLimit(recordId)
    message.success('解除成功！')
    infoTable.value?.reload(false)
  })
}

/**
 * 删除用户
 * @param {string} recordId - 用户ID
 */
const delFunc = async (recordId) => {
  const { infoBox } = await import('@/utils/info-box')
  infoBox.confirmDanger('确认删除？', '', async () => {
    await sysUserApi.delById(recordId)
    message.success('删除成功！')
    infoTable.value?.reload(false)
  })
}

/**
 * 分配角色
 * @param {string} recordId - 用户ID
 * @param {string} sysType - 系统类型
 * @param {string} belongInfoId - 所属信息ID
 */
const roleDist = (recordId, sysType, belongInfoId) => {
  roleDistRef.value.show(recordId, sysType, belongInfoId)
}

/**
 * 更新用户状态
 * @param {string} recordId - 用户ID
 * @param {number} state - 状态值
 */
const updateState = async (recordId, state) => {
  const { infoBox } = await import('@/utils/info-box')
  const title = state === 1 ? '确认[启用]该用户？' : '确认[停用]该用户？'
  const content = state === 1 ? '启用后用户可进行登陆等一系列操作' : '停用后该用户将立即退出系统并不可再次登陆'

  return new Promise((resolve, reject) => {
    infoBox.confirmDanger(title, content, async () => {
      try {
        await sysUserApi.updateStateById(recordId, { state })
        searchFunc()
        resolve()
      } catch (err) {
        reject(err)
      }
    }, () => {
      reject(new Error())
    })
  })
}

/**
 * 初始化
 */
onMounted(() => {
})
</script>

<style scoped>
</style>

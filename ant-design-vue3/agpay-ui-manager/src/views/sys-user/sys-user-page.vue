<template>
  <div>
    <a-card>
      <ag-search
        v-if="$access('ENT_UR_USER_SEARCH')"
        :search-data="searchData"
        :open-is-show-more="false"
        :is-show-more="isShowMore"
        :btn-loading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model:value="searchData.sysType" placeholder="所属系统" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="MGR">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商</a-select-option>
              <a-select-option value="MCH">商户</a-select-option>
            </a-select>
          </a-form-item>
          <ag-input :placeholder="'所属代理商/商户'" v-model:value="searchData.belongInfoId" />
          <ag-input :placeholder="'用户ID'" v-model:value="searchData.sysUserId" />
          <ag-input :placeholder="'用户姓名'" v-model:value="searchData.realname" />
          <a-form-item label="" class="table-head-layout">
            <a-select v-model:value="searchData.userType" placeholder="请选择用户类型">
              <a-select-option v-for="d in userTypeOptions" :value="d.userType" :key="d.userType">
                {{ d.userTypeName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        @btn-load-close="btnLoading=false"
        ref="infoTable"
        :init-data="true"
        :req-table-data-func="reqTableDataFunc"
        :table-columns="tableColumns"
        :search-data="searchData"
        row-key="sysUserId"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_UR_USER_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>

        <template #avatarSlot="{record}">
          <a-avatar size="default" :src="record.avatarUrl" />
        </template>

        <template #realnameSlot="{record}">
          <span>
            {{ record.realname }}
            <a-tag v-if="record.initUser" :color="'green'">初始</a-tag>
          </span>
        </template>

        <template #sysTypeSlot="{record}">
          <a-tag
            :key="record.sysType"
            :color="record.sysType === 'MGR' ? 'green' :
              record.sysType === 'AGENT' ? 'cyan' :
              record.sysType === 'MCH' ? 'geekblue' : 'loser'">
            {{ record.sysType === 'MGR' ? '运营平台' :
              record.sysType === 'AGENT' ? '代理商系统' :
              record.sysType === 'MCH' ? '商户系统' : '其他' }}
          </a-tag>
        </template>

        <template #userTypeSlot="{record}">
          <span>{{ getUserTypeName(record.userType) }}</span>
        </template>

        <template #inviteCodeSlot="{record}" v-if="record.inviteCode">
          <b>{{ record.inviteCode }}</b>
          <a-button icon="copy" type="link" @click="copyFunc(record.inviteCode)"/>
          <a-button icon="info-circle" type="link" @click="inviteCodeFunc(record.inviteCode, record.sysType)"/>
        </template>

        <template #stateSlot="{record}">
          <ag-state-switch :state="record.state" :show-switch-type="$access('ENT_UR_USER_EDIT')" :on-change="(state) => { return updateState(record.sysUserId, state)}"/>
        </template>

        <template #opSlot="{record}">  <!-- 操作列插槽 -->
          <ag-table-actions>
            <a-button type="link" v-if="$access('ENT_UR_USER_UPD_ROLE') && record.userType===2" @click="roleDist(record.sysUserId, record.sysType, record.belongInfoId)" >变更角色</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_EDIT')" @click="editFunc(record.sysUserId, record.sysType, record.belongInfoId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_LOGIN_LIMIT_DELETE')" style="color: red" @click="relieveFunc(record.sysUserId)">解除登录限制</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_DELETE')" style="color: red" @click="delFunc(record.sysUserId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 修改 页面组件  -->
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="queryFunc"/>
    <!-- 邀请码窗口  -->
    <invite-code ref="inviteCode"/>
    <!-- 分配角色 页面组件  -->
    <role-dist ref="roleDistRef"/>
  </div>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { API_URL_SYS_USER_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'
import InviteCode from './invite-code.vue'
import RoleDist from './role-dist.vue'

const tableColumns = [
  { key: 'avatar', title: '头像', width: 65, fixed: 'left', scopedSlots: { customRender: 'avatarSlot' } },
  { key: 'realname', title: '姓名', width: 135, fixed: 'left', scopedSlots: { customRender: 'realnameSlot' } },
  { key: 'sysUserId', dataIndex: 'sysUserId', title: '用户ID', width: 120, fixed: 'left' },
  { key: 'sex', dataIndex: 'sex', title: '性别', width: 65, customRender: (text, record, index) => { return record.sex === 1 ? '男' : record.sex === 2 ? '女' : '未知' } },
  { key: 'userNo', dataIndex: 'userNo', title: '编号', width: 125 },
  { key: 'telphone', dataIndex: 'telphone', title: '手机号', width: 160 },
  { key: 'sysType', title: '所属系统', width: 120, scopedSlots: { customRender: 'sysTypeSlot' } },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'userType', title: '操作员类型', width: 120, scopedSlots: { customRender: 'userTypeSlot' } },
  { key: 'teamName', dataIndex: 'teamName', title: '团队', width: 160 },
  { key: 'inviteCode', title: '邀请码', width: 160, scopedSlots: { customRender: 'inviteCodeSlot' }, align: 'center' },
  { key: 'state', title: '状态', width: 100, scopedSlots: { customRender: 'stateSlot' }, align: 'center' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '修改时间', width: 200 },
  { key: 'op', title: '操作', width: 180, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

const userTypeList = [
  { userTypeName: '全部', userType: '' },
  { userTypeName: '超级管理员', userType: 1 },
  { userTypeName: '普通操作员', userType: 2 },
  { userTypeName: '商户拓展员', userType: 3 },
  { userTypeName: '店长', userType: 11 },
  { userTypeName: '店员', userType: 12 }
]

// 默认查询条件数据对象
const defaultSearchData = {
  userType: '',
  sysType: 'MGR'
}

const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const inviteCode = ref(null)
const roleDistRef = ref(null)
const searchData = reactive({ ...defaultSearchData })
const userTypeOptions = userTypeList
const isShowMore = ref(false)
const btnLoading = ref(false)

const copyFunc = (text) => {
  // text是复制文本
  // 创建input元素
  const el = document.createElement('input')
  // 给input元素赋值需要复制的文本
  el.setAttribute('value', text)
  // 将input元素插入页面
  document.body.appendChild(el)
  // 选中input元素的文本
  el.select()
  // 复制内容到剪贴板
  document.execCommand('copy')
  // 删除input元素
  document.body.removeChild(el)
  import('ant-design-vue').then(({ message }) => {
    message.success('邀请码已复制')
  })
}

const inviteCodeFunc = (inviteCode, sysType) => {
  inviteCode.value.show(inviteCode, sysType)
}

const getUserTypeName = (userType) => {
  return userTypeList.find(f => f.userType === userType)?.userTypeName || ''
}

const handleSearchFormData = (data) => {
  // 如果是空对象或者为null/undefined
  if (!data || Object.keys(data).length === 0) {
    Object.assign(searchData, defaultSearchData)
  } else {
    Object.assign(searchData, data)
  }
}

const setIsShowMore = (value) => {
  isShowMore.value = value
}

// 请求table接口数据
const reqTableDataFunc = (params) => {
  return req.list(API_URL_SYS_USER_LIST, params)
}

const queryFunc = () => { // 点击【查询】按钮点击事件
  btnLoading.value = true // 打开查询按钮的loading
  searchFunc(true)
}

const searchFunc = (isToFirst = false) => { // 点击【查询】按钮点击事件
  infoTable.value?.refTable(isToFirst)
}

const addFunc = () => { // 业务通用【新增】 函数
  infoAddOrEdit.value.show()
}

const editFunc = (recordId, sysType, belongInfoId) => { // 业务通用【修改】 函数
  infoAddOrEdit.value.show(recordId, sysType, belongInfoId)
}

const relieveFunc = (recordId) => { // 业务通用【解除登录限制】 函数
  import('ant-design-vue').then(({ message }) => {
    import('@/utils/info-box').then(({ infoBox }) => {
      infoBox.confirmDanger('确认解除吗？', '', () => {
        return req.delById(API_URL_SYS_USER_LIST + '/loginLimit', recordId).then(res => {
          message.success('解除成功！')
          infoTable.value?.refTable(false)
        })
      })
    })
  })
}

const delFunc = (recordId) => { // 业务通用【删除】 函数
  import('ant-design-vue').then(({ message }) => {
    import('@/utils/info-box').then(({ infoBox }) => {
      infoBox.confirmDanger('确认删除？', '', () => {
        return req.delById(API_URL_SYS_USER_LIST, recordId).then(res => {
          message.success('删除成功！')
          infoTable.value?.refTable(false)
        })
      })
    })
  })
}

const roleDist = (recordId, sysType, belongInfoId) => { // 【分配权限】 按钮点击事件
  roleDistRef.value.show(recordId, sysType, belongInfoId)
}

const updateState = (recordId, state) => { // 【更新状态】
  import('ant-design-vue').then(({ message }) => {
    import('@/utils/info-box').then(({ infoBox }) => {
      const title = state === 1 ? '确认[启用]该用户？' : '确认[停用]该用户？'
      const content = state === 1 ? '启用后用户可进行登陆等一系列操作' : '停用后该用户将立即退出系统并不可再次登陆'

      return new Promise((resolve, reject) => {
        infoBox.confirmDanger(title, content, () => {
          return reqLoad.updateById(API_URL_SYS_USER_LIST, recordId, { state: state }).then(res => {
            searchFunc()
            resolve()
          }).catch(err => reject(err))
        },
          () => {
          reject(new Error())
        })
      })
    })
  })
}

onMounted(() => {
})
</script>

<style scoped>
</style>
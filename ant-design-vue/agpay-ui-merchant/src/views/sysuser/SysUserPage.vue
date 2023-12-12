<template>
  <page-header-wrapper>
    <a-card>
      <div v-if="$access('ENT_UR_USER_SEARCH')" class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'用户ID'" :msg="searchData.sysUserId" v-model="searchData.sysUserId" />
            <ag-text-up :placeholder="'用户姓名'" :msg="searchData.realname" v-model="searchData.realname" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.userType" placeholder="请选择用户类型">
                <a-select-option v-for="d in userTypeOptions" :value="d.userType" :key="d.userType">
                  {{ d.userTypeName }}
                </a-select-option>
              </a-select>
            </a-form-item>

            <span class="table-page-search-submitButtons">
              <a-button type="primary" @click="searchFunc" icon="search" :loading="btnLoading">查询</a-button>
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
        rowKey="sysUserId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_UR_USER_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>

        <template slot="avatarSlot" slot-scope="{record}">
          <a-avatar size="default" :src="record.avatarUrl" />
        </template>

        <template slot="realnameSlot" slot-scope="{record}">
          <span>
            {{ record.realname }}
            <a-tag v-if="record.initUser" :color="'green'">初始</a-tag>
          </span>
        </template>

        <template slot="userTypeSlot" slot-scope="{record}">
          <span>{{ getUserTypeName(record.userType) }}</span>
        </template>

        <template slot="inviteCodeSlot" slot-scope="{record}" v-if="record.inviteCode">
          <b>{{ record.inviteCode }}</b>
          <a-button icon="copy" type="link" @click="copyFunc(record.inviteCode)"/>
          <a-button icon="info-circle" type="link" @click="inviteCodeFunc(record.inviteCode, record.sysType)"/>
        </template>

        <template slot="stateSlot" slot-scope="{record}">
          <AgTableColState :state="record.state" :showSwitchType="$access('ENT_UR_USER_EDIT')" :onChange="(state) => { return updateState(record.sysUserId, state)}"/>
        </template>

        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_UR_USER_UPD_ROLE') && record.userType===2" @click="roleDist(record.sysUserId)" >变更角色</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_EDIT')" @click="editFunc(record.sysUserId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_LOGIN_LIMIT_DELETE')" style="color: red" @click="relieveFunc(record.sysUserId)">解除登录限制</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_DELETE')" style="color: red" @click="delFunc(record.sysUserId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>

    <!-- 新增 / 修改 页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>

    <!-- 邀请码窗口  -->
    <InviteCode ref="inviteCode"/>

    <!-- 分配角色 页面组件  -->
    <RoleDist ref="roleDist"/>

  </page-header-wrapper>

</template>
<script>
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgTableColState from '@/components/AgTable/AgTableColState'
import { API_URL_SYS_USER_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InviteCode from './InviteCode'
import RoleDist from './RoleDist'

const tableColumns = [
  { key: 'avatar', title: '头像', width: 65, fixed: 'left', scopedSlots: { customRender: 'avatarSlot' } },
  { key: 'realname', title: '姓名', width: 135, fixed: 'left', scopedSlots: { customRender: 'realnameSlot' } },
  { key: 'sysUserId', dataIndex: 'sysUserId', title: '用户ID', width: 120, fixed: 'left' },
  { key: 'sex', dataIndex: 'sex', title: '性别', width: 65, customRender: (text, record, index) => { return record.sex === 1 ? '男' : record.sex === 2 ? '女' : '未知' } },
  { key: 'userNo', dataIndex: 'userNo', title: '编号', width: 125 },
  { key: 'telphone', dataIndex: 'telphone', title: '手机号', width: 160 },
  { key: 'userType', title: '操作员类型', width: 120, scopedSlots: { customRender: 'userTypeSlot' } },
  { key: 'inviteCode', title: '邀请码', width: 160, align: 'center', scopedSlots: { customRender: 'inviteCodeSlot' } },
  { key: 'state', title: '状态', width: 100, align: 'center', scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '修改时间', width: 200 },
  { key: 'op', title: '操作', width: 180, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

const userTypeList = [
  { userTypeName: '全部', userType: null },
  { userTypeName: '超级管理员', userType: 1 },
  { userTypeName: '普通操作员', userType: 2 },
  // { userTypeName: '商户拓展员', userType: 3 },
  { userTypeName: '店长', userType: 11 },
  { userTypeName: '店员', userType: 12 }
]

export default {
  components: { AgTable, AgTableColumns, InfoAddOrEdit, InviteCode, RoleDist, AgTableColState, AgTextUp },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: {
        userType: null
      },
      userTypeOptions: userTypeList,
      btnLoading: false
    }
  },
  mounted () {
  },
  methods: {
    copyFunc (text) {
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
      this.$message.success('邀请码已复制')
    },
    inviteCodeFunc: function (inviteCode, sysType) {
      this.$refs.inviteCode.show(inviteCode, sysType)
    },
    getUserTypeName: (userType) => {
      return userTypeList.find(f => f.userType === userType).userTypeName
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_SYS_USER_LIST, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.btnLoading = true // 打开查询按钮的loading
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },
    relieveFunc: function (recordId) { // 业务通用【解除登录限制】 函数
      const that = this
      this.$infoBox.confirmDanger('确认解除吗？', '', () => {
        return req.delById(API_URL_SYS_USER_LIST + '/loginLimit', recordId).then(res => {
          that.$message.success('解除成功！')
          that.$refs.infoTable.refTable(false)
        })
      })
    },
    delFunc: function (recordId) { // 业务通用【删除】 函数
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        return req.delById(API_URL_SYS_USER_LIST, recordId).then(res => {
          that.$message.success('删除成功！')
          that.$refs.infoTable.refTable(false)
        })
      })
    },
    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId)
    },
    roleDist: function (recordId) { // 【分配权限】 按钮点击事件
      this.$refs.roleDist.show(recordId)
    },
    updateState: function (recordId, state) { // 【更新状态】
      const that = this
      const title = state === 1 ? '确认[启用]该用户？' : '确认[停用]该用户？'
      const content = state === 1 ? '启用后用户可进行登陆等一系列操作' : '停用后该用户将立即退出系统并不可再次登陆'

      return new Promise((resolve, reject) => {
        that.$infoBox.confirmDanger(title, content, () => {
          return reqLoad.updateById(API_URL_SYS_USER_LIST, recordId, { state: state }).then(res => {
            that.searchFunc()
            resolve()
          }).catch(err => reject(err))
        },
          () => {
          reject(new Error())
        })
      })
    }
  }
}
</script>

<style scoped>
</style>

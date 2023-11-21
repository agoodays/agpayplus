<template>
  <page-header-wrapper>
    <a-card>
      <AgSearchForm
        v-if="$access('ENT_UR_USER_SEARCH')"
        :searchData="searchData"
        :openIsShowMore="false"
        :isShowMore="isShowMore"
        :btnLoading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc">
        <template slot="formItem">
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.sysType" placeholder="所属系统" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="MGR">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商</a-select-option>
              <a-select-option value="MCH">商户</a-select-option>
            </a-select>
          </a-form-item>
          <ag-text-up :placeholder="'所属代理商/商户'" :msg="searchData.belongInfoId" v-model="searchData.belongInfoId" />
          <ag-text-up :placeholder="'用户ID'" :msg="searchData.sysUserId" v-model="searchData.sysUserId" />
          <ag-text-up :placeholder="'用户姓名'" :msg="searchData.realname" v-model="searchData.realname" />
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.userType" placeholder="请选择用户类型">
              <a-select-option v-for="d in userTypeOptions" :value="d.userType" :key="d.userType">
                {{ d.userTypeName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </template>
      </AgSearchForm>
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

        <template slot="sysTypeSlot" slot-scope="{record}">
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
            <a-button type="link" v-if="$access('ENT_UR_USER_UPD_ROLE')" @click="roleDist(record.sysUserId, record.sysType, record.belongInfoId)" >变更角色</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_EDIT')" @click="editFunc(record.sysUserId, record.sysType, record.belongInfoId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_UR_USER_DELETE')" style="color: red" @click="delFunc(record.sysUserId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>

    <!-- 新增 / 修改 页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="queryFunc"/>

    <!-- 邀请码窗口  -->
    <InviteCode ref="inviteCode"/>

    <!-- 分配角色 页面组件  -->
    <RoleDist ref="roleDist"/>

  </page-header-wrapper>

</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgTableColState from '@/components/AgTable/AgTableColState'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import { API_URL_SYS_USER_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InviteCode from './InviteCode'
import RoleDist from './RoleDist'

const tableColumns = [
  { key: 'avatar', title: '头像', width: 65, fixed: 'left', scopedSlots: { customRender: 'avatarSlot' } },
  { key: 'realname', dataIndex: 'realname', title: '姓名', width: 120, fixed: 'left' },
  { key: 'sysUserId', dataIndex: 'sysUserId', title: '用户ID', width: 120, fixed: 'left' },
  { key: 'sex', dataIndex: 'sex', title: '性别', width: 65, customRender: (text, record, index) => { return record.sex === 1 ? '男' : record.sex === 2 ? '女' : '未知' } },
  { key: 'userNo', dataIndex: 'userNo', title: '编号', width: 120 },
  { key: 'telphone', dataIndex: 'telphone', title: '手机号', width: 160 },
  { key: 'sysType', title: '所属系统', width: 120, scopedSlots: { customRender: 'sysTypeSlot' } },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'isAdmin', dataIndex: 'isAdmin', title: '超管', width: 65, customRender: (text, record, index) => { return record.isAdmin === 1 ? '是' : '否' } },
  { key: 'userType', title: '操作员类型', width: 120, scopedSlots: { customRender: 'userTypeSlot' } },
  { key: 'teamName', dataIndex: 'teamName', title: '团队', width: 160 },
  { key: 'inviteCode', title: '邀请码', width: 160, scopedSlots: { customRender: 'inviteCodeSlot' }, align: 'center' },
  { key: 'state', title: '状态', width: 100, scopedSlots: { customRender: 'stateSlot' }, align: 'center' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '修改时间', width: 200 },
  { key: 'op', title: '操作', width: 180, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

const userTypeList = [
  { userTypeName: '全部', userType: 0 },
  { userTypeName: '超级管理员', userType: 1 },
  { userTypeName: '普通操作员', userType: 2 },
  { userTypeName: '商户拓展员', userType: 3 },
  { userTypeName: '店长', userType: 11 },
  { userTypeName: '店员', userType: 12 }
]

export default {
  components: { AgSearchForm, AgTable, AgTableColumns, AgTableColState, AgTextUp, InfoAddOrEdit, InviteCode, RoleDist },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: {
        userType: 0,
        sysType: 'MGR'
      },
      userTypeOptions: userTypeList,
      isShowMore: false,
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
    handleSearchFormData (searchData) {
      this.searchData = searchData
    },
    setIsShowMore (isShowMore) {
      this.isShowMore = isShowMore
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_SYS_USER_LIST, params)
    },
    queryFunc: function () { // 点击【查询】按钮点击事件
      this.btnLoading = true // 打开查询按钮的loading
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (recordId, sysType, belongInfoId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId, sysType, belongInfoId)
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
    roleDist: function (recordId, sysType, belongInfoId) { // 【分配权限】 按钮点击事件
      this.$refs.roleDist.show(recordId, sysType, belongInfoId)
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

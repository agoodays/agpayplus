<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title="'设置权限匹配规则'"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
    class="drawer-width">
    <a-row>
      <a-col span="24">
        <a-form layout="inline">
          <a-form-item label="">
            <a-select v-model="sysType" placeholder="选择系统菜单" @change="entTree" class="table-head-layout">
              <a-select-option value="MGR">显示菜单：运营平台</a-select-option>
              <a-select-option value="AGENT">显示菜单：代理商系统</a-select-option>
              <a-select-option value="MCH">显示菜单：商户系统</a-select-option>
            </a-select>
          </a-form-item>
        </a-form>
      </a-col>
      <a-col span="10">
        <p v-if="hasEnt">请选择权限： </p>
        <!-- 树状结构 -->
        <a-tree :tree-data="treeData" :replaceFields="replaceFields" v-model="checkedKeys" :checkable="true"/>
      </a-col>
      <a-col span="14">
        <p v-if="hasEnt">请选择匹配规则： </p>
        <a-form-model ref="infoFormModel" :model="matchRule" layout="vertical">
          <a-form-model-item v-if="sysType!=='MCH'" label="" prop="epUserEnt">
            <a-checkbox @change="onEpUserEntChange">拓展员权限</a-checkbox>
          </a-form-model-item>
          <a-form-model-item v-if="sysType==='MCH'" label="" prop="userEntRules">
            <a-checkbox-group v-model="matchRule.userEntRules">
              <a-checkbox value="USER_TYPE_11_INIT">店长默认权限</a-checkbox>
              <a-checkbox value="USER_TYPE_12_INIT">店员默认权限</a-checkbox>
              <a-checkbox value="STORE">门店管理权限</a-checkbox>
              <a-checkbox value="QUICK_PAY">快捷收银权限</a-checkbox>
              <a-checkbox value="REFUND">退款权限</a-checkbox>
              <a-checkbox value="DEVICE">设备管理权限</a-checkbox>
              <a-checkbox value="STATS">统计报表权限</a-checkbox>
            </a-checkbox-group>
          </a-form-model-item>
          <a-form-model-item v-if="sysType==='MCH'" prop="mchType">
            <a-checkbox :checked="matchRule.mchType === 1" @change="onMchTypeChange(1)">普通商户特有权限</a-checkbox>
            <a-checkbox :checked="matchRule.mchType === 2" @change="onMchTypeChange(2)">特约商户(服务商模式)特有权限</a-checkbox>
          </a-form-model-item>
          <a-form-model-item v-if="sysType==='MCH'" prop="mchLevelArray">
            <a-checkbox-group v-model="matchRule.mchLevelArray">
              <a-checkbox value="M0">M0商户特有权限</a-checkbox>
              <a-checkbox value="M1">M1商户特有权限</a-checkbox>
            </a-checkbox-group>
          </a-form-model-item>
        </a-form-model>
      </a-col>
    </a-row>
    <div class="drawer-btn-center" >
      <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose" style="margin-right:8px">
        取消
      </a-button>
      <a-button type="primary" :style="{ marginRight: '8px' }" icon="check" @click="handleOkFunc('add')" :loading="addLoading">
        添加匹配规则
      </a-button>
      <a-button type="danger" icon="delete" @click="handleOkFunc('delete')" :loading="deleteLoading">
        删除匹配规则
      </a-button>
    </div>
  </a-drawer>
</template>

<script>
import { getEntTree, API_URL_ENT_LIST, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      sysType: 'MGR', // 默认查询运营平台
      addLoading: false,
      deleteLoading: false,
      hasEnt: this.$access('ENT_UR_ROLE_DIST'),
      treeData: [],
      replaceFields: { key: 'entId', title: 'entName' }, // 配置替换字段
      checkedKeys: [], // 已选中的节点
      allEntList: {}, // 由于antd vue关联操作，无法直接获取到父ID, 需要内部自行维护一套数据结构 {entId: {pid, children}}
      matchRule: {}
    }
  },
  methods: {
    show: function () { // 弹层打开事件
      this.entTree(this.sysType)
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    onEpUserEntChange: function (e) {
      const that = this
      if (e.target.checked) {
        that.matchRule.epUserEnt = true
      } else {
        that.matchRule.epUserEnt = null
      }
    },
    onMchTypeChange (value) {
      if (this.matchRule.mchType === value) {
        this.matchRule.mchType = null
      } else {
        this.matchRule.mchType = value
      }
      this.$forceUpdate()
    },
    handleOkFunc: function (opType) { // 点击【确认】按钮事件
      const that = this
      // 显示loading
      if (opType === 'add') {
        that.addLoading = true
      } else {
        that.deleteLoading = true
      }
      // 请求接口
      const selectedEntIdList = that.getSelectedEntIdList()
      const matchRule = this.matchRule
      console.log(matchRule)
      req.updateById(API_URL_ENT_LIST, 'setMatchRule', {
        sysType: that.sysType,
        opType: opType,
        entIds: selectedEntIdList,
        matchRule: matchRule
      }).then(res => {
        that.$message.success(opType === 'add' ? '添加成功' : '删除成功')
        if (opType === 'add') {
          that.addLoading = false
        } else {
          that.deleteLoading = false
        }
        that.isShow = false
        that.callbackFunc() // 刷新列表
      }).catch(res => {
        if (opType === 'add') {
          that.addLoading = false
        } else {
          that.deleteLoading = false
        }
      })
    },
    entTree: function (sysType) { // 弹层打开事件
      const that = this

      // 判断是否有权限访问
      if (!this.hasEnt) {
        return false
      }

      // 重置数据
      that.checkedKeys = []
      that.treeData = []
      that.allEntList = {}

      sysType = sysType?.length > 0 ? sysType : 'MGR'
      // 获取全部权限的树状结构
      getEntTree(sysType).then(res => {
        that.treeData = res

        // 存储所有的菜单权限集合
        this.recursionTreeData(res, (item) => {
          that.allEntList[item.entId] = { pid: item.pid, children: item.children || [] }
        })
      })
    },
    getSelectedEntIdList: function () { // 获取已选择的列表集合
      // 判断是否有权限访问
      if (!this.hasEnt) {
        return false
      }
      const that = this
      const reqData = []

      this.checkedKeys.map(item => {
        const pidList = [] // 当前权限的所有的父节点IDList
        that.getAllPid(item, pidList)
        pidList.map(pid => {
          if (reqData.indexOf(pid) < 0) {
            reqData.push(pid)
          }
        })
      })
      return reqData
    },
    // 递归遍历树状结构数据
    recursionTreeData (entTreeData, func) {
      for (let i = 0; i < entTreeData.length; i++) {
        const thisEnt = entTreeData[i]
        if (thisEnt.children && thisEnt.children.length > 0) {
          this.recursionTreeData(thisEnt.children, func)
        }
        func(thisEnt)
      }
    },
    getAllPid (entId, array) { // 获取所有的PID
      if (this.allEntList[entId] && entId !== 'ROOT') {
        array.push(entId)
        this.getAllPid(this.allEntList[entId].pid, array)
      }
    }
  }
}
</script>

<style scoped>
  ::v-deep(.ant-checkbox-wrapper + .ant-checkbox-wrapper) {
    margin-left: 0px;
  }
</style>

<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="'设置权限匹配规则'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
    class="drawer-width"
    @close="onClose"
  >
    <a-row>
      <a-col span="24">
        <a-form layout="inline">
          <a-form-item label="">
            <a-select v-model="sysType" placeholder="选择系统菜单" class="table-head-layout" @change="entTree">
              <a-select-option value="MGR">显示菜单：运营平台</a-select-option>
              <a-select-option value="AGENT">显示菜单：代理商系统</a-select-option>
              <a-select-option value="MCH">显示菜单：商户系统</a-select-option>
            </a-select>
          </a-form-item>
        </a-form>
      </a-col>
      <a-col span="10">
        <p v-if="hasEnt">请选择权限：</p>
        <!-- 树状结构 -->
        <a-tree v-model="checkedKeys" :tree-data="treeData" :replace-fields="replaceFields" :checkable="true" />
      </a-col>
      <a-col span="14">
        <p v-if="hasEnt">请选择匹配规则：</p>
        <a-form-model ref="infoFormModel" :model="matchRule" layout="vertical">
          <a-form-model-item v-if="sysType !== 'MCH'" label="" prop="epUserEnt">
            <a-checkbox @change="onEpUserEntChange">拓展员权限</a-checkbox>
          </a-form-model-item>
          <a-form-model-item v-if="sysType === 'MCH'" label="" prop="userEntRules">
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
          <a-form-model-item v-if="sysType === 'MCH'" prop="mchType">
            <a-checkbox :checked="matchRule.mchType === 1" @change="onMchTypeChange(1)">普通商户特有权限</a-checkbox>
            <a-checkbox :checked="matchRule.mchType === 2" @change="onMchTypeChange(2)"
              >特约商户(服务商模式)特有权限</a-checkbox
            >
          </a-form-model-item>
          <a-form-model-item v-if="sysType === 'MCH'" prop="mchLevelArray">
            <a-checkbox-group v-model="matchRule.mchLevelArray">
              <a-checkbox value="M0">M0商户特有权限</a-checkbox>
              <a-checkbox value="M1">M1商户特有权限</a-checkbox>
            </a-checkbox-group>
          </a-form-model-item>
        </a-form-model>
      </a-col>
    </a-row>
    <div class="drawer-btn-center">
      <a-button icon="close" :style="{ marginRight: '8px' }" style="margin-right: 8px" @click="onClose">
        取消
      </a-button>
      <a-button
        type="primary"
        :style="{ marginRight: '8px' }"
        icon="check"
        :loading="addLoading"
        @click="handleOkFunc('add')"
      >
        添加匹配规则
      </a-button>
      <a-button type="danger" icon="delete" :loading="deleteLoading" @click="handleOkFunc('delete')">
        删除匹配规则
      </a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { entApi } from '@/api/business/ent/ent-api'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const visible = ref(false)
const sysType = ref('MGR')
const addLoading = ref(false)
const deleteLoading = ref(false)
const hasEnt = window.$access?.('ENT_UR_ROLE_DIST') ?? false
const treeData = ref([])
const replaceFields = { key: 'entId', title: 'entName' }
const checkedKeys = ref([])
const allEntList = ref({})
const matchRule = ref({})

const show = () => {
  entTree(sysType.value)
  visible.value = true
}

const onClose = () => {
  visible.value = false
}

const onEpUserEntChange = (e) => {
  if (e.target.checked) {
    matchRule.value.epUserEnt = true
  } else {
    matchRule.value.epUserEnt = null
  }
}

const onMchTypeChange = (value) => {
  if (matchRule.value.mchType === value) {
    matchRule.value.mchType = null
  } else {
    matchRule.value.mchType = value
  }
}

const handleOkFunc = (opType) => {
  if (opType === 'add') {
    addLoading.value = true
  } else {
    deleteLoading.value = true
  }

  const selectedEntIdList = getSelectedEntIdList()
  entApi
    .setMatchRule({
      sysType: sysType.value,
      opType,
      entIds: selectedEntIdList,
      matchRule: matchRule.value
    })
    .then(() => {
      window.$message.success(opType === 'add' ? '添加成功' : '删除成功')
      if (opType === 'add') {
        addLoading.value = false
      } else {
        deleteLoading.value = false
      }
      visible.value = false
      props.callbackFunc()
    })
    .catch(() => {
      if (opType === 'add') {
        addLoading.value = false
      } else {
        deleteLoading.value = false
      }
    })
}

const entTree = (currentSysType) => {
  if (!hasEnt) {
    return false
  }

  checkedKeys.value = []
  treeData.value = []
  allEntList.value = {}

  const resolvedSysType = currentSysType?.length > 0 ? currentSysType : 'MGR'
  entApi.queryEntTree(resolvedSysType).then((res) => {
    treeData.value = res
    recursionTreeData(res, (item) => {
      allEntList.value[item.entId] = { pid: item.pid, children: item.children || [] }
    })
  })
}

const getSelectedEntIdList = () => {
  if (!hasEnt) {
    return false
  }

  const reqData = []
  checkedKeys.value.map((item) => {
    const pidList = []
    getAllPid(item, pidList)
    pidList.map((pid) => {
      if (reqData.indexOf(pid) < 0) {
        reqData.push(pid)
      }
    })
  })
  return reqData
}

const recursionTreeData = (entTreeData, func) => {
  for (let i = 0; i < entTreeData.length; i++) {
    const thisEnt = entTreeData[i]
    if (thisEnt.children && thisEnt.children.length > 0) {
      recursionTreeData(thisEnt.children, func)
    }
    func(thisEnt)
  }
}

const getAllPid = (entId, array) => {
  if (allEntList.value[entId] && entId !== 'ROOT') {
    array.push(entId)
    getAllPid(allEntList.value[entId].pid, array)
  }
}

defineExpose({ show })
</script>

<style scoped>
::v-deep(.ant-checkbox-wrapper + .ant-checkbox-wrapper) {
  margin-left: 0px;
}
</style>

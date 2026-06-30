<template>
  <div style="padding-bottom: 50px">
    <p v-if="hasEnt">请选择权限：</p>
    <!-- 树状结构 -->
    <a-tree v-model="checkedKeys" :tree-data="treeData" :replace-fields="replaceFields" :checkable="true" />
  </div>
</template>

<script setup>
import { roleApi } from '@/api/business/role/role-api'
import { reactive, ref } from 'vue'

const hasEnt = window.$access('ENT_UR_ROLE_DIST')
const recordId = ref(null)
const treeData = ref([])
const replaceFields = { key: 'entId', title: 'entName' }
const checkedKeys = ref([])
const allEntList = reactive({})

const recursionTreeData = (entTreeData, func) => {
  for (let i = 0; i < entTreeData.length; i++) {
    const thisEnt = entTreeData[i]
    if (thisEnt.children && thisEnt.children.length > 0) {
      recursionTreeData(thisEnt.children, func)
    }
    func(thisEnt)
  }
}

const clearAllEntList = () => {
  Object.keys(allEntList).forEach((key) => {
    delete allEntList[key]
  })
}

const initTree = async (currentRecordId, sysType) => {
  if (!hasEnt) {
    return false
  }

  checkedKeys.value = []
  treeData.value = []
  clearAllEntList()

  recordId.value = currentRecordId

  const currentSysType = sysType?.length > 0 ? sysType : 'MGR'
  const entTree = await roleApi.queryEntTree(currentSysType)
  treeData.value = entTree

  recursionTreeData(entTree, (item) => {
    allEntList[item.entId] = { pid: item.pid, children: item.children || [] }
  })

  const relaRes = await roleApi.queryRoleEntRelaList(currentRecordId)
  const checkedEntIdList = []

  relaRes.records.forEach((item) => {
    if (allEntList[item.entId] && allEntList[item.entId].children.length <= 0) {
      checkedEntIdList.push(item.entId)
    }
  })

  checkedKeys.value = checkedEntIdList
  return true
}

const getAllPid = (entId, array) => {
  if (allEntList[entId] && entId !== 'ROOT') {
    array.push(entId)
    getAllPid(allEntList[entId].pid, array)
  }
}

const getSelectedEntIdList = () => {
  if (!hasEnt) {
    return false
  }
  const reqData = []

  checkedKeys.value.forEach((item) => {
    const pidList = []
    getAllPid(item, pidList)
    pidList.forEach((pid) => {
      if (reqData.indexOf(pid) < 0) {
        reqData.push(pid)
      }
    })
  })

  return reqData
}

defineExpose({
  initTree,
  getSelectedEntIdList
})
</script>

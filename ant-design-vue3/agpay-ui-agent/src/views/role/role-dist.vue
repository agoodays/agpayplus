<template>
  <div style="padding-bottom: 20px">
    <p v-if="hasEnt">请选择权限：</p>
    <a-tree v-if="hasEnt" :tree-data="treeData" :field-names="fieldNames" v-model:checked-keys="checkedKeys" :checkable="true" />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { roleApi } from '@/api/business/role/role-api'

const props = defineProps({
  sysType: { type: String, default: 'AGENT' }
})

const treeData = ref([])
const checkedKeys = ref([])
const allEntList = ref({})

const fieldNames = { key: 'entId', title: 'entName' }
const hasEnt = ref(true)

const recursionTreeData = (entTreeData, func) => {
  for (const thisEnt of entTreeData) {
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

const initTree = async (recordId) => {
  if (!hasEnt.value) return

  checkedKeys.value = []
  treeData.value = []
  allEntList.value = {}

  const res = await roleApi.queryEntTree(props.sysType || 'AGENT')
  treeData.value = res || []

  recursionTreeData(treeData.value, (item) => {
    allEntList.value[item.entId] = { pid: item.pid, children: item.children || [] }
  })

  if (recordId) {
    const res2 = await roleApi.queryRoleEntRelaList(recordId)
    const checkedEntIdList = []
    ;(res2?.records || []).forEach((item) => {
      if (allEntList.value[item.entId] && allEntList.value[item.entId].children.length <= 0) {
        checkedEntIdList.push(item.entId)
      }
    })
    checkedKeys.value = checkedEntIdList
  }
}

const getSelectedEntIdList = () => {
  if (!hasEnt.value) return []
  const reqData = []
  checkedKeys.value.forEach((item) => {
    const pidList = []
    getAllPid(item, pidList)
    pidList.forEach((pid) => {
      if (!reqData.includes(pid)) reqData.push(pid)
    })
  })
  return reqData
}

defineExpose({ initTree, getSelectedEntIdList })
</script>

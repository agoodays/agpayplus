<template>
  <ag-drawer
    v-model:open="localOpen"
    :title="'设置权限匹配规则'"
    width="60%"
    class="drawer-width"
    @close="handleClose"
  >
    <a-row>
      <a-col span="24">
        <a-form layout="inline">
          <a-form-item label="">
            <a-select v-model:value="sysType" placeholder="选择系统菜单" class="table-head-layout" @change="entTree">
              <a-select-option value="MGR">显示菜单：运营平台</a-select-option>
              <a-select-option value="AGENT">显示菜单：代理商系统</a-select-option>
              <a-select-option value="MCH">显示菜单：商户系统</a-select-option>
            </a-select>
          </a-form-item>
        </a-form>
      </a-col>
      <a-col span="10">
        <p v-if="hasEnt">请选择权限：</p>
        <a-tree v-model="checkedKeys" :tree-data="treeData" :replace-fields="replaceFields" :checkable="true" />
      </a-col>
      <a-col span="14">
        <p v-if="hasEnt">请选择匹配规则：</p>
        <a-form ref="infoForm" :model="matchRule" layout="vertical">
          <a-form-item v-if="sysType !== 'MCH'" label="" name="epUserEnt">
            <a-checkbox @change="onEpUserEntChange">拓展员权限</a-checkbox>
          </a-form-item>
          <a-form-item v-if="sysType === 'MCH'" label="" name="userEntRules">
            <a-checkbox-group v-model="matchRule.userEntRules">
              <a-checkbox value="USER_TYPE_11_INIT">店长默认权限</a-checkbox>
              <a-checkbox value="USER_TYPE_12_INIT">店员默认权限</a-checkbox>
              <a-checkbox value="STORE">门店管理权限</a-checkbox>
              <a-checkbox value="QUICK_PAY">快捷收银权限</a-checkbox>
              <a-checkbox value="REFUND">退款权限</a-checkbox>
              <a-checkbox value="DEVICE">设备管理权限</a-checkbox>
              <a-checkbox value="STATS">统计报表权限</a-checkbox>
            </a-checkbox-group>
          </a-form-item>
          <a-form-item v-if="sysType === 'MCH'" name="mchType">
            <a-checkbox :checked="matchRule.mchType === 1" @change="onMchTypeChange(1)">普通商户特有权限</a-checkbox>
            <a-checkbox :checked="matchRule.mchType === 2" @change="onMchTypeChange(2)">特约商户(服务商模式)特有权限</a-checkbox>
          </a-form-item>
          <a-form-item v-if="sysType === 'MCH'" name="mchLevelArray">
            <a-checkbox-group v-model="matchRule.mchLevelArray">
              <a-checkbox value="M0">M0商户特有权限</a-checkbox>
              <a-checkbox value="M1">M1商户特有权限</a-checkbox>
            </a-checkbox-group>
          </a-form-item>
        </a-form>
      </a-col>
    </a-row>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" @click="handleClose">
        <template #icon><CloseOutlined /></template>
        取消
      </a-button>
      <a-button type="primary" :style="{ marginRight: '8px' }" :loading="addLoading" @click="handleOkFunc('add')">
        <template #icon><CheckOutlined /></template>
        添加匹配规则
      </a-button>
      <a-button type="danger" :loading="deleteLoading" @click="handleOkFunc('delete')">
        <template #icon><DeleteOutlined /></template>
        删除匹配规则
      </a-button>
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 设置权限匹配规则组件
 * 功能：配置系统菜单的权限匹配规则
 */
import { entApi } from '@/api/business/ent/ent-api'
import { AgDrawer } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { CheckOutlined, CloseOutlined, DeleteOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 组件属性
 */
const props = defineProps({
  open: { type: Boolean, default: false }
})

/**
 * 组件事件
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 系统类型
 */
const sysType = ref('MGR')

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 添加加载状态
 */
const addLoading = ref(false)

/**
 * 删除加载状态
 */
const deleteLoading = ref(false)

/**
 * 是否有角色分配权限
 */
const hasEnt = hasPermission('ENT_UR_ROLE_DIST')

/**
 * 树数据
 */
const treeData = ref([])

/**
 * 树字段替换配置
 */
const replaceFields = { key: 'entId', title: 'entName' }

/**
 * 选中的节点键
 */
const checkedKeys = ref([])

/**
 * 所有权限点列表
 */
const allEntList = ref({})

/**
 * 匹配规则
 */
const matchRule = ref({})

/**
 * 监听open变化
 */
watch(() => props.open, (newVal) => {
  localOpen.value = newVal
  if (newVal) {
    entTree(sysType.value)
  }
})

/**
 * 监听本地open变化，同步emit
 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 关闭弹窗
 */
const handleClose = () => {
  emit('update:open', false)
}

/**
 * 拓展员权限变更处理
 * @param {Event} e - 事件对象
 */
const onEpUserEntChange = (e) => {
  if (e.target.checked) {
    matchRule.value.epUserEnt = true
  } else {
    matchRule.value.epUserEnt = null
  }
}

/**
 * 商户类型变更处理
 * @param {number} value - 商户类型值
 */
const onMchTypeChange = (value) => {
  if (matchRule.value.mchType === value) {
    matchRule.value.mchType = null
  } else {
    matchRule.value.mchType = value
  }
}

/**
 * 处理确认操作
 * @param {string} opType - 操作类型：add 或 delete
 */
const handleOkFunc = async (opType) => {
  if (opType === 'add') {
    addLoading.value = true
  } else {
    deleteLoading.value = true
  }

  try {
    const selectedEntIdList = getSelectedEntIdList()
    await entApi.setMatchRule({
      sysType: sysType.value,
      opType,
      entIds: selectedEntIdList,
      matchRule: matchRule.value
    })
    message.success(opType === 'add' ? '添加成功' : '删除成功')
    emit('update:open', false)
    emit('success')
  } catch (error) {
    console.error('设置匹配规则失败:', error)
  } finally {
    addLoading.value = false
    deleteLoading.value = false
  }
}

/**
 * 获取权限树
 * @param {string} currentSysType - 当前系统类型
 */
const entTree = async (currentSysType) => {
  if (!hasEnt) {
    return false
  }

  checkedKeys.value = []
  treeData.value = []
  allEntList.value = {}

  const resolvedSysType = currentSysType?.length > 0 ? currentSysType : 'MGR'
  const res = await entApi.queryEntTree(resolvedSysType)
  treeData.value = res
  recursionTreeData(res, (item) => {
    allEntList.value[item.entId] = { pid: item.pid, children: item.children || [] }
  })
}

/**
 * 获取选中的权限ID列表
 * @returns {Array|boolean} 选中的权限ID列表或false
 */
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

/**
 * 递归处理树数据
 * @param {Array} entTreeData - 树数据
 * @param {Function} func - 处理函数
 */
const recursionTreeData = (entTreeData, func) => {
  for (let i = 0; i < entTreeData.length; i++) {
    const thisEnt = entTreeData[i]
    if (thisEnt.children && thisEnt.children.length > 0) {
      recursionTreeData(thisEnt.children, func)
    }
    func(thisEnt)
  }
}

/**
 * 获取所有父级ID
 * @param {string} entId - 当前ID
 * @param {Array} array - 结果数组
 */
const getAllPid = (entId, array) => {
  if (allEntList.value[entId] && entId !== 'ROOT') {
    array.push(entId)
    getAllPid(allEntList.value[entId].pid, array)
  }
}
</script>

<style scoped>
::v-deep(.ant-checkbox-wrapper + .ant-checkbox-wrapper) {
  margin-left: 0px;
}
</style>
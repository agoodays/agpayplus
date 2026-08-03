<template>
  <ag-drawer
    title="分配角色"
    width="30%"
    :open="localOpen"
    :mask-closable="true"
    :show-confirm="true"
    :confirm-loading="loading"
    @update:open="handleUpdateOpen"
    @confirm="handleConfirm"
    @close="handleClose">
    <div>
      <div :style="{ paddingBottom: '20px', borderBottom: '1px solid #E9E9E9' }">
        <a-checkbox
          :indeterminate="checkedVal.length != 0 && allRoleList.length != checkedVal.length"
          :checked="checkedVal.length != 0 && allRoleList.length === checkedVal.length"
          @change="onCheckAllChange">
          全选
        </a-checkbox>
      </div>
      <br />
      <a-checkbox-group v-model:value="checkedVal" :options="allRoleList"/>
    </div>
  </ag-drawer>

</template>

<script setup>
/**
 * 分配角色抽屉组件
 * 功能：为用户分配角色权限，支持全选和单选
 */
import { sysUserApi } from '@/api/business/sys-user/sys-user-api'
import { AgDrawer } from '@/components'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

/**
 * 组件属性定义
 */
const props = defineProps({
  open: { type: Boolean, default: false },
  recordId: { type: String, default: '' },
  sysType: { type: String, default: 'MGR' },
  belongInfoId: { type: String, default: '' }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 确认按钮加载状态
 */
const confirmLoading = ref(false)

/**
 * 角色列表
 */
const allRoleList = ref([])

/**
 * 已选中的角色ID列表
 */
const checkedVal = ref([])

/**
 * 加载角色列表数据
 * @returns {void}
 */
const loadRoles = async () => {
  allRoleList.value = []
  checkedVal.value = []
  confirmLoading.value = false

  const roleRes = await sysUserApi.queryRolePageWithLoading({
    pageSize: -1,
    sysType: props.sysType,
    belongInfoId: props.belongInfoId
  })

  if (roleRes.total <= 0) {
    message.error('当前暂无角色，请先行添加')
    return
  }

  allRoleList.value = roleRes.records.map(role => ({
    label: role.roleName,
    value: role.roleId
  }))

  const relaRes = await sysUserApi.queryUserRoleRelaPage({
    pageSize: -1,
    userId: props.recordId
  })

  checkedVal.value = relaRes.records.map(rela => rela.roleId)
}

/**
 * 处理确认按钮点击
 * @returns {void}
 */
const handleConfirm = async () => {
  confirmLoading.value = true
  try {
    await sysUserApi.updateUserRoleRela(props.recordId, checkedVal.value)
    message.success('更新成功！')
    emit('success')
  } catch (error) {
    console.error('更新失败:', error)
  } finally {
    confirmLoading.value = false
  }
}

/**
 * 处理关闭按钮点击
 * @returns {void}
 */
const handleClose = () => {
  emit('update:open', false)
}

/**
 * 处理open更新事件
 */
const handleUpdateOpen = (val) => {
  localOpen.value = val
  emit('update:open', val)
}

/**
 * 全选/取消全选处理
 * @param {Event} e - 复选框事件
 * @returns {void}
 */
const onCheckAllChange = (e) => {
  checkedVal.value = []
  if (e.target.checked) {
    allRoleList.value.forEach(role => {
      checkedVal.value.push(role.value)
    })
  }
}

/**
 * 监听 open 属性变化，加载角色数据
 */
watch(() => props.open, (newVal) => {
  localOpen.value = newVal
  if (newVal) {
    loadRoles()
  }
}, { immediate: true })
</script>
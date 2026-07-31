<template>
  <ag-drawer
    v-model:open="localOpen"
    :title="isAdd ? '新增角色' : '修改角色'"
    width="40%"
    :mask-closable="false"
    @close="handleClose"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
  >
    <a-form ref="infoForm" :model="saveObject" :label-col="{ span: 4 }" :rules="rules">
      <a-form-item label="角色名称：" name="roleName">
        <a-input v-model:value="saveObject.roleName" />
      </a-form-item>
    </a-form>

    <!-- 角色权限分配 -->
    <role-dist ref="roleDist" />
  </ag-drawer>
</template>

<script setup>
/**
 * 角色新增/编辑组件
 * 功能：新增或编辑角色配置，分配角色权限
 */
import { roleApi } from '@/api/business/role/role-api'
import { AgDrawer } from '@/components'
import { message } from 'ant-design-vue'
import { nextTick, ref, watch } from 'vue'
import RoleDist from './role-dist.vue'

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  },
  sysType: {
    type: String,
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const roleDist = ref(null)

const loading = ref(false)
const isAdd = ref(true)
const localOpen = ref(false)
const saveObject = ref({})

const rules = {
  roleName: [{ required: true, message: '请输入角色名称', trigger: 'blur' }]
}

/** 监听 open 属性变化 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val) {
      await initForm()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 初始化表单 */
const initForm = async () => {
  isAdd.value = !props.recordId
  saveObject.value = {}
  loading.value = false

  infoForm.value?.resetFields?.()

  await nextTick()
  roleDist.value?.initTree(props.recordId, props.sysType)

  if (!isAdd.value) {
    saveObject.value = await roleApi.getById(props.recordId)
  }
}

const validateForm = async () => {
  if (!infoForm.value?.validate) {
    return true
  }
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

const handleConfirm = async () => {
  const valid = await validateForm()
  if (!valid) return

  loading.value = true

  try {
    const selectedEntIdList = roleDist.value?.getSelectedEntIdList?.() || []
    saveObject.value.entIds = selectedEntIdList

    if (isAdd.value) {
      await roleApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await roleApi.updateById(props.recordId, saveObject.value)
      message.success('修改成功')
    }

    localOpen.value = false
    emit('success')
  } finally {
    loading.value = false
  }
}

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
}
</script>

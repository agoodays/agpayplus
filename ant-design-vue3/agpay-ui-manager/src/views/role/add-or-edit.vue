<template>
  <a-drawer
    :visible="isShow"
    :title="isAdd ? '新增角色' : '修改角色'"
    width="600"
    :mask-closable="false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    @close="isShow = false"
  >
    <a-form-model ref="infoFormModel" :model="saveObject" :label-col="{ span: 4 }" :rules="rules">
      <a-form-model-item label="角色名称：" prop="roleName">
        <a-input v-model="saveObject.roleName" />
      </a-form-model-item>
    </a-form-model>

    <!-- 角色权限分配 -->
    <RoleDist ref="roleDist" />

    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" icon="close" @click="isShow = false">取消</a-button>
      <a-button type="primary" :loading="confirmLoading" icon="check" @click="handleOkFunc">保存</a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { roleApi } from '@/api/business/role/role-api'
import { message } from 'ant-design-vue'
import { nextTick, ref } from 'vue'
import RoleDist from './role-dist.vue'
const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoFormModel = ref(null)
const roleDist = ref(null)

const confirmLoading = ref(false)
const isAdd = ref(true)
const isShow = ref(false)
const saveObject = ref({})
const recordId = ref(null)

const rules = {
  roleName: [{ required: true, message: '请输入角色名称', trigger: 'blur' }]
}

const show = async (currentRecordId, sysType) => {
  isAdd.value = !currentRecordId
  saveObject.value = {}
  confirmLoading.value = false

  infoFormModel.value?.resetFields?.()

  await nextTick()
  roleDist.value?.initTree(currentRecordId, sysType)

  if (!isAdd.value) {
    recordId.value = currentRecordId
    saveObject.value = await roleApi.getById(currentRecordId)
  }

  isShow.value = true
}

const validateForm = () => {
  return new Promise((resolve) => {
    if (!infoFormModel.value?.validate) {
      resolve(true)
      return
    }
    infoFormModel.value.validate((valid) => resolve(valid))
  })
}

const handleOkFunc = async () => {
  const valid = await validateForm()
  if (!valid) return

  confirmLoading.value = true

  try {
    const selectedEntIdList = roleDist.value?.getSelectedEntIdList?.() || []
    saveObject.value.entIds = selectedEntIdList

    if (isAdd.value) {
      await roleApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await roleApi.updateById(recordId.value, saveObject.value)
      message.success('修改成功')
    }

    isShow.value = false
    props.callbackFunc()
  } finally {
    confirmLoading.value = false
  }
}

defineExpose({
  show
})
</script>

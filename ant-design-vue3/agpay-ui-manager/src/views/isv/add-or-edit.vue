<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="isAdd ? '新增服务商' : '修改服务商'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    @close="onClose"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="服务商名称" name="isvName">
            <a-input v-model:value="saveObject.isvName" placeholder="请输入服务商名称" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="服务商简称" name="isvShortName">
            <a-input v-model:value="saveObject.isvShortName" placeholder="请输入服务商简称" />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="联系人姓名" name="contactName">
            <a-input v-model:value="saveObject.contactName" placeholder="请输入联系人姓名" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="联系人手机号" name="contactTel">
            <a-input v-model:value="saveObject.contactTel" placeholder="请输入联系人手机号"> </a-input>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="联系人邮箱" name="contactEmail">
            <a-input v-model:value="saveObject.contactEmail" placeholder="请输入联系人邮箱"> </a-input>
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state" :default-value="1">
              <a-radio :value="1"> 启用 </a-radio>
              <a-radio :value="0"> 禁用 </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入备注" type="textarea" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <div class="drawer-btn-center">
      <a-button icon="close" style="margin-right: 8px" @click="onClose"> 取消 </a-button>
      <a-button type="primary" style="margin-right: 8px" icon="check" :loading="btnLoading" @click="handleOkFunc">
        保存
      </a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { isvApi } from '@/api/business/isv/isv-api'
import { message } from 'ant-design-vue'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const btnLoading = ref(false)
const isAdd = ref(true)
const saveObject = ref({})
const recordId = ref(null)
const visible = ref(false)

const rules = {
  isvName: [{ required: true, message: '请输入服务商名称', trigger: 'blur' }],
  isvShortName: [{ required: true, message: '请输入服务商简称', trigger: 'blur' }],
  contactEmail: [
    {
      required: false,
      pattern: /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/,
      message: '请输入正确的邮箱地址',
      trigger: 'blur'
    }
  ],
  contactTel: [{ required: false, pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }]
}

async function show(currentRecordId) {
  isAdd.value = !currentRecordId
  saveObject.value = { state: 1 }
  recordId.value = currentRecordId || null
  infoForm.value?.resetFields?.()
  visible.value = true

  if (!isAdd.value && recordId.value) {
    const res = await isvApi.getById(recordId.value)
    saveObject.value = res || { state: 1 }
  }
}

async function validateForm() {
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

async function handleOkFunc() {
  if (btnLoading.value) return
  const valid = await validateForm()
  if (!valid) return

  btnLoading.value = true
  try {
    if (isAdd.value) {
      await isvApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await isvApi.updateById(recordId.value, saveObject.value)
      message.success('修改成功')
    }
    visible.value = false
    props.callbackFunc()
  } finally {
    btnLoading.value = false
  }
}

function onClose() {
  visible.value = false
}

defineExpose({
  show
})
</script>

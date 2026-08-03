<template>
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    :mask-closable="false"
    :title="isAdd ? '新增服务商' : '修改服务商'"
    :show-confirm="true"
    :confirm-loading="loading"
    @close="handleClose"
    @confirm="handleConfirm"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
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
      <a-row :gutter="16">
        <a-col :span="10">
          <a-form-item label="联系人姓名" name="contactName">
            <a-input v-model:value="saveObject.contactName" placeholder="请输入联系人姓名" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="联系人手机号" name="contactTel">
            <a-input v-model:value="saveObject.contactTel" placeholder="请输入联系人手机号" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="10">
          <a-form-item label="联系人邮箱" name="contactEmail">
            <a-input v-model:value="saveObject.contactEmail" placeholder="请输入联系人邮箱" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-textarea v-model:value="saveObject.remark" placeholder="请输入备注" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>

<script setup>
import { isvApi } from '@/api/business/isv/isv-api'
import { AgDrawer } from '@/components'
import { getStateOptions } from '@/constants/common-const'
import { message } from 'ant-design-vue'
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const localOpen = ref(false)
const loading = ref(false)
const isAdd = ref(true)
const saveObject = ref({})

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

async function initForm(currentRecordId) {
  isAdd.value = !currentRecordId
  saveObject.value = { state: 1 }
  infoForm.value?.resetFields?.()

  if (!isAdd.value && currentRecordId) {
    try {
      const res = await isvApi.getById(currentRecordId)
      saveObject.value = res || { state: 1 }
    } catch (error) {
      console.error('加载服务商信息失败:', error)
      message.error(error?.msg || '加载服务商信息失败')
    }
  }
}

async function handleConfirm() {
  if (loading.value) return

  try {
    await infoForm.value.validate()

    loading.value = true
    if (isAdd.value) {
      await isvApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await isvApi.updateById(props.recordId, saveObject.value)
      message.success('修改成功')
    }
    localOpen.value = false
    emit('success')
  } catch (error) {
    if (!error.errorFields) {
      message.error('操作失败')
    }
  } finally {
    loading.value = false
  }
}

function handleClose() {
  localOpen.value = false
}

watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val) {
      await initForm(props.recordId)
    }
  }
)

watch(localOpen, (val) => {
  emit('update:open', val)
})
</script>

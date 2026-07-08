<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="'绑定码牌'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    class="drawer-width"
    @close="onClose"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 18 }"
      :rules="rules"
    >
      <a-form-item label="商户号" name="mchNo">
        <ag-select
          v-model:value="saveObject.mchNo"
          :api="searchMch"
          value-field="mchNo"
          label-field="mchName"
          placeholder="商户号（搜索商户名称）"
          @change="mchNoChange"
        />
      </a-form-item>
      <a-form-item label="应用" name="appId">
        <a-select v-model:value="saveObject.appId" placeholder="请选择应用">
          <a-select-option key="" value="">请选择应用</a-select-option>
          <a-select-option v-for="d in appList" :key="d.appId" :value="d.appId">
            {{ d.appName + ' [ AppId: ' + d.appId + ' ]' }}
          </a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item label="门店" name="storeId">
        <a-select v-model:value="saveObject.storeId" placeholder="请选择门店">
          <a-select-option key="" value="">请选择门店</a-select-option>
          <a-select-option v-for="d in storeList" :key="d.storeId" :value="d.storeId">
            {{ d.storeName + ' [ ID: ' + d.storeId + ' ]' }}
          </a-select-option>
        </a-select>
      </a-form-item>
    </a-form>
    <div class="drawer-btn-center">
      <a-button icon="close" :style="{ marginRight: '8px' }" style="margin-right: 8px" @click="onClose">
        取消
      </a-button>
      <a-button type="primary" icon="check" :loading="btnLoading" @click="handleOkFunc"> 保存 </a-button>
    </div>
  </a-drawer>
</template>
<script setup>
import { qrcApi } from '@/api/business/qr-code/qrc-api'
import AgSelect from '@/components/ag-select'
import { message } from 'ant-design-vue'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const visible = ref(false)
const btnLoading = ref(false)
const recordId = ref(null)
const saveObject = ref({})
const appList = ref(null)
const storeList = ref(null)

const rules = {
  mchNo: [{ required: true, message: '请选择商户', trigger: 'blur' }],
  appId: [{ required: true, message: '请选择应用', trigger: 'blur' }],
  storeId: [{ required: true, message: '请选择门店', trigger: 'blur' }]
}

async function show(currentRecordId) {
  recordId.value = currentRecordId
  const res = await qrcApi.getById(currentRecordId)
  saveObject.value = res
  if (res.mchNo) {
    await mchNoChange()
  }
  visible.value = true
}

function onClose() {
  visible.value = false
}

function searchMch(params) {
  return qrcApi.searchMch(params)
}

async function mchNoChange() {
  if (saveObject.value.mchNo) {
    const [appRes, storeRes] = await Promise.all([
      qrcApi.listMchApps({ mchNo: saveObject.value.mchNo, pageSize: -1, state: 1 }),
      qrcApi.listMchStores({ mchNo: saveObject.value.mchNo, pageSize: -1, state: 1 })
    ])
    appList.value = appRes.records
    storeList.value = storeRes.records
    return
  }
  appList.value = null
  storeList.value = null
  saveObject.value.appId = null
  saveObject.value.storeId = null
}

async function validateForm() {
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

async function handleOkFunc() {
  const valid = await validateForm()
  if (!valid) return
  await qrcApi.bindById(recordId.value, saveObject.value)
  message.success('绑定成功')
  visible.value = false
  props.callbackFunc()
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less"></style>

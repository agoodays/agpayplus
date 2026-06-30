<template>
  <a-modal
    v-model="isShow"
    :title="isAdd ? '新增账号组' : '修改账号组'"
    :confirm-loading="confirmLoading"
    @ok="handleOkFunc"
  >
    <a-form-model
      ref="infoFormModel"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 15 }"
      :rules="rules"
    >
      <a-form-model-item label="商户号" prop="mchNo">
        <ag-select
          v-model="saveObject.mchNo"
          :api="searchMch"
          value-field="mchNo"
          label-field="mchName"
          placeholder="商户号（搜索商户名称）"
          :disabled="!isAdd"
        />
      </a-form-model-item>
      <a-form-model-item label="组名称：" prop="receiverGroupName">
        <a-input v-model="saveObject.receiverGroupName" />
      </a-form-model-item>
      <a-form-model-item label="自动分账组" prop="autoDivisionFlag">
        <a-radio-group v-model="saveObject.autoDivisionFlag">
          <a-radio :value="1">是</a-radio> <a-radio :value="0">否</a-radio>
        </a-radio-group>
        <div class="agpay-tip-text">
          <p style="line-height: 20px">
            1. 自动分账组: 当订单分账模式为自动分账，该组下的所有正常分账状态的账号将作为订单分账对象
          </p>
          <p style="line-height: 20px">2. 每个商户仅有一个默认分账组， 当该组更新为自动分账时，其他组将改为否</p>
        </div>
      </a-form-model-item>
    </a-form-model>
  </a-modal>
</template>

<script setup>
import { divisionGroupApi } from '@/api/business/division/division-group-api'
import AgSelect from '@/components/ag-select'
import { message } from 'ant-design-vue'
import { ref } from 'vue'

defineOptions({ components: { AgSelect } })

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoFormModel = ref(null)
const confirmLoading = ref(false)
const isAdd = ref(true)
const isShow = ref(false)
const saveObject = ref({ autoDivisionFlag: 0 })
const recordId = ref(null)

const rules = {
  receiverGroupName: [{ required: true, message: '请输入组名称', trigger: 'blur' }]
}

const show = async (currentRecordId) => {
  isAdd.value = !currentRecordId
  saveObject.value = { autoDivisionFlag: 0 }
  confirmLoading.value = false
  infoFormModel.value?.resetFields?.()

  if (!isAdd.value) {
    recordId.value = currentRecordId
    const res = await divisionGroupApi.getById(currentRecordId)
    saveObject.value = res
  }
  isShow.value = true
}

const searchMch = (params) => divisionGroupApi.listMch(params)

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
    if (isAdd.value) {
      await divisionGroupApi.add(saveObject.value)
      message.success('添加成功')
    } else {
      await divisionGroupApi.updateById(recordId.value, saveObject.value)
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
<style lang="less">
.agpay-tip-text:before {
  content: '';
  width: 0;
  height: 0;
  border: 10px solid transparent;
  border-bottom-color: #ffeed8;
  position: absolute;
  top: -20px;
  left: 30px;
}
.agpay-tip-text {
  font-size: 12px !important;
  border-radius: 5px;
  background: #ffeed8;
  color: #c57000 !important;
  padding: 5px 10px;
  display: inline-block;
  max-width: 100%;
  position: relative;
  margin-top: 15px;
  line-height: 1.5715;
}
</style>

<template>
  <a-drawer
    :visible="isShow"
    title="修改分账用户信息"
    width="30%"
    :mask-closable="false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    @close="onClose"
  >
    <a-form-model
      ref="infoFormModel"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 15 }"
      :rules="rules"
    >
      <a-form-model-item label="账号别名：" prop="receiverAlias">
        <a-input v-model="saveObject.receiverAlias" />
      </a-form-model-item>

      <a-form-model-item label="默认分账比例：" prop="divisionProfit">
        <a-input v-model="saveObject.divisionProfit" style="width: 100px" /> %
      </a-form-model-item>

      <a-form-model-item label="状态" prop="state">
        <a-radio-group v-model="saveObject.state">
          <a-radio :value="1">正常分账</a-radio> <a-radio :value="0">暂停分账</a-radio>
        </a-radio-group>
      </a-form-model-item>

      <a-form-model-item label="分组变更：" prop="receiverGroupId">
        <a-select v-model="saveObject.receiverGroupId" style="width: 210px" placeholder="账号分组">
          <a-select-option v-for="item in allReceiverGroup" :key="item.receiverGroupId" :value="item.receiverGroupId">{{
            item.receiverGroupName
          }}</a-select-option>
        </a-select>
      </a-form-model-item>
    </a-form-model>

    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" icon="close" @click="onClose">取消</a-button>
      <a-button type="primary" :loading="confirmLoading" icon="check" @click="handleOkFunc">保存</a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { divisionReceiverApi } from '@/api/business/division/division-receiver-api'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const emit = defineEmits(['close'])

const infoFormModel = ref()
const confirmLoading = ref(false)
const isShow = ref(false)
const saveObject = ref({})
const recordId = ref(null)
const allReceiverGroup = ref([])

const rules = {
  receiverAlias: [{ required: true, message: '请输入别名', trigger: 'blur' }],
  receiverGroupId: [{ required: true, message: '请选择分组', trigger: 'blur' }],
  divisionProfit: [{ required: true, message: '请录入默认分账比例', trigger: 'blur' }],
  state: [{ required: true, message: '请选择状态', trigger: 'blur' }]
}

const show = (id) => {
  saveObject.value = {}
  confirmLoading.value = false
  infoFormModel.value?.resetFields?.()
  recordId.value = id

  divisionReceiverApi.getById(id).then((res) => {
    const current = res || {}
    current.divisionProfit = (current.divisionProfit * 100).toFixed(2)
    saveObject.value = current
  })

  divisionReceiverApi.listReceiverGroup({ pageSize: -1 }).then((res) => {
    allReceiverGroup.value = res.records || []
  })

  isShow.value = true
}

const handleOkFunc = () => {
  infoFormModel.value.validate((valid) => {
    if (valid) {
      confirmLoading.value = true
      const reqObject = {
        receiverAlias: saveObject.value.receiverAlias,
        receiverGroupId: saveObject.value.receiverGroupId,
        divisionProfit: saveObject.value.divisionProfit,
        state: saveObject.value.state
      }

      divisionReceiverApi
        .updateById(recordId.value, reqObject)
        .then(() => {
          window.$message.success('修改成功')
          isShow.value = false
          props.callbackFunc()
        })
        .catch(() => {
          confirmLoading.value = false
        })
    }
  })
}

const onClose = () => {
  isShow.value = false
  emit('close')
}

defineExpose({ show })
</script>

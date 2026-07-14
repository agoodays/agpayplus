<template>
  <a-modal
    v-model:open="localOpen"
    :title="isAdd ? '新增账号组' : '修改账号组'"
    :confirm-loading="confirmLoading"
    @ok="handleOkFunc"
    @cancel="handleClose"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 15 }"
      :rules="rules"
    >
      <a-form-item label="商户号" name="mchNo">
        <ag-select-infinite
          v-model="saveObject.mchNo"
          placeholder="商户号（搜索商户名称）"
          search-field="mchName"
          :fetch-data="searchMch"
          :field-names="{ label: 'mchName', value: 'mchNo' }"
          :disabled="!isAdd"
        />
      </a-form-item>
      <a-form-item label="组名称：" name="receiverGroupName">
        <a-input v-model:value="saveObject.receiverGroupName" />
      </a-form-item>
      <a-form-item label="自动分账组" name="autoDivisionFlag">
        <a-radio-group v-model:value="saveObject.autoDivisionFlag">
          <a-radio :value="1">是</a-radio> <a-radio :value="0">否</a-radio>
        </a-radio-group>
        <div class="agpay-tip-text">
          <p style="line-height: 20px">
            1. 自动分账组: 当订单分账模式为自动分账，该组下的所有正常分账状态的账号将作为订单分账对象
          </p>
          <p style="line-height: 20px">2. 每个商户仅有一个默认分账组， 当该组更新为自动分账时，其他组将改为否</p>
        </div>
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
/**
 * 分账接收方分组新增/编辑弹窗组件
 * 功能：新增或编辑分账接收方分组信息
 */
import { divisionGroupApi } from '@/api/business/division/division-group-api'
import { AgSelectInfinite } from '@/components'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

defineOptions({ components: { AgSelectInfinite } })

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: [String, Number],
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const confirmLoading = ref(false)
const isAdd = ref(true)
const localOpen = ref(false)
const saveObject = ref({ autoDivisionFlag: 0 })

const rules = {
  receiverGroupName: [{ required: true, message: '请输入组名称', trigger: 'blur' }]
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
  saveObject.value = { autoDivisionFlag: 0 }
  confirmLoading.value = false
  infoForm.value?.resetFields?.()

  if (!isAdd.value) {
    const res = await divisionGroupApi.getById(props.recordId)
    saveObject.value = res
  }
}

const searchMch = (params) => divisionGroupApi.listMch(params)

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

const handleOkFunc = async () => {
  const valid = await validateForm()
  if (!valid) return

  confirmLoading.value = true
  try {
    if (isAdd.value) {
      await divisionGroupApi.add(saveObject.value)
      message.success('添加成功')
    } else {
      await divisionGroupApi.updateById(props.recordId, saveObject.value)
      message.success('修改成功')
    }
    localOpen.value = false
    emit('success')
  } finally {
    confirmLoading.value = false
  }
}

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
}
</script>
<style lang="less">
</style>

<template>
  <ag-drawer
    v-model:open="localOpen"
    title="修改分账用户信息"
    width="30%"
    :mask-closable="false"
    @close="handleClose"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 15 }"
      :rules="rules"
    >
      <a-form-item label="账号别名：" name="receiverAlias">
        <a-input v-model:value="saveObject.receiverAlias" />
      </a-form-item>

      <a-form-item label="默认分账比例：" name="divisionProfit">
        <a-input v-model:value="saveObject.divisionProfit" style="width: 100px" /> %
      </a-form-item>

      <a-form-item label="状态" name="state">
        <a-radio-group v-model:value="saveObject.state">
          <a-radio :value="1">正常分账</a-radio> <a-radio :value="0">暂停分账</a-radio>
        </a-radio-group>
      </a-form-item>

      <a-form-item label="分组变更：" name="receiverGroupId">
        <a-select v-model:value="saveObject.receiverGroupId" style="width: 210px" placeholder="账号分组">
          <a-select-option v-for="item in allReceiverGroup" :key="item.receiverGroupId" :value="item.receiverGroupId">{{
            item.receiverGroupName
          }}</a-select-option>
        </a-select>
      </a-form-item>
    </a-form>

    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" @click="onClose">
        <template #icon><CloseOutlined /></template>
        取消
      </a-button>
      <a-button type="primary" :loading="confirmLoading" @click="handleOkFunc">
        <template #icon><CheckOutlined /></template>
        保存
      </a-button>
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 分账接收者编辑组件
 * 功能：修改分账接收者信息
 */
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'
import { AgDrawer } from '@/components'
import { divisionReceiverApi } from '@/api/business/division/division-receiver-api'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

/** Props 定义 */
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

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const confirmLoading = ref(false)
const localOpen = ref(false)
const saveObject = ref({})
const allReceiverGroup = ref([])

const rules = {
  receiverAlias: [{ required: true, message: '请输入别名', trigger: 'blur' }],
  receiverGroupId: [{ required: true, message: '请选择分组', trigger: 'blur' }],
  divisionProfit: [{ required: true, message: '请录入默认分账比例', trigger: 'blur' }],
  state: [{ required: true, message: '请选择状态', trigger: 'blur' }]
}

/** 监听 open 属性变化 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val && props.recordId) {
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
  saveObject.value = {}
  confirmLoading.value = false
  infoForm.value?.resetFields?.()

  const [res, groupRes] = await Promise.all([
    divisionReceiverApi.getById(props.recordId),
    divisionReceiverApi.listReceiverGroup({ pageSize: -1 })
  ])
  
  const current = res || {}
  current.divisionProfit = (current.divisionProfit * 100).toFixed(2)
  saveObject.value = current
  
  allReceiverGroup.value = groupRes.records || []
}

const handleOkFunc = async () => {
  try {
    await infoForm.value.validate()
  } catch {
    return
  }

  confirmLoading.value = true
  try {
    const reqObject = {
      receiverAlias: saveObject.value.receiverAlias,
      receiverGroupId: saveObject.value.receiverGroupId,
      divisionProfit: saveObject.value.divisionProfit,
      state: saveObject.value.state
    }

    await divisionReceiverApi.updateById(props.recordId, reqObject)
    message.success('修改成功')
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

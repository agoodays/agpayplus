<template>
  <a-modal
    v-model:open="visible"
    title="订单退款"
    :width="600"
    @ok="handleSubmit"
    @cancel="handleClose"
  >
    <a-form
      ref="formRef"
      :model="formState"
      :rules="rules"
      layout="vertical"
    >
      <a-alert
        message="退款说明"
        description="请谨慎操作，退款成功后资金将原路返回到用户账户"
        type="warning"
        show-icon
        style="margin-bottom: 24px"
      />

      <a-descriptions :column="1" bordered size="small" style="margin-bottom: 24px">
        <a-descriptions-item label="支付订单号">
          <a-typography-text copyable>{{ payOrder?.payOrderId }}</a-typography-text>
        </a-descriptions-item>

        <a-descriptions-item label="订单金额">
          <span style="color: #1890ff; font-weight: 500">
            ¥{{ (payOrder?.amount / 100).toFixed(2) }}
          </span>
        </a-descriptions-item>

        <a-descriptions-item label="商户名称">
          {{ payOrder?.mchName }}
        </a-descriptions-item>
      </a-descriptions>

      <a-form-item label="退款金额" name="refundAmount">
        <a-input-number
          v-model:value="formState.refundAmount"
          :min="0.01"
          :max="payOrder?.amount / 100"
          :precision="2"
          :step="0.01"
          style="width: 100%"
          placeholder="请输入退款金额"
        >
          <template #addonBefore>
            ¥
          </template>
        </a-input-number>
        <div style="margin-top: 8px; color: rgba(0, 0, 0, 0.45)">
          最大可退款金额：¥{{ (payOrder?.amount / 100).toFixed(2) }}
        </div>
      </a-form-item>

      <a-form-item label="退款原因" name="refundReason">
        <a-select
          v-model:value="formState.refundReason"
          placeholder="请选择退款原因"
        >
          <a-select-option value="用户申请退款">用户申请退款</a-select-option>
          <a-select-option value="订单异常">订单异常</a-select-option>
          <a-select-option value="商品缺货">商品缺货</a-select-option>
          <a-select-option value="其他">其他</a-select-option>
        </a-select>
      </a-form-item>

      <a-form-item label="退款备注" name="remark">
        <a-textarea
          v-model:value="formState.remark"
          placeholder="请输入退款备注（选填）"
          :rows="4"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, watch, nextTick } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { API_URL_REFUND_ORDER, req } from '/@/api/manage'

// Props & Emits
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  payOrder: {
    type: Object,
    default: () => null
  }
})

const emit = defineEmits(['update:visible', 'success'])

// State
const formRef = ref()
const loading = ref(false)
const visible = ref(false)

// 表单数据
const formState = reactive({
  refundAmount: 0,
  refundReason: '',
  remark: ''
})

// 表单验证规则
const rules = {
  refundAmount: [
    { required: true, message: '请输入退款金额', trigger: 'blur' },
    { 
      validator: (rule, value) => {
        if (value <= 0) {
          return Promise.reject('退款金额必须大于0')
        }
        if (props.payOrder && value > props.payOrder.amount / 100) {
          return Promise.reject('退款金额不能大于订单金额')
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  refundReason: [
    { required: true, message: '请选择退款原因', trigger: 'change' }
  ]
}

// 监听 props.visible 变化
watch(() => props.visible, (val) => {
  visible.value = val
  if (val) {
    initForm()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:visible', val)
})

/**
 * 初始化表单
 */
const initForm = () => {
  // 默认退款全额
  formState.refundAmount = props.payOrder ? props.payOrder.amount / 100 : 0
  formState.refundReason = ''
  formState.remark = ''
  
  nextTick(() => {
    formRef.value?.clearValidate()
  })
}

/**
 * 提交表单
 */
const handleSubmit = () => {
  formRef.value.validate().then(async () => {
    Modal.confirm({
      title: '确认退款？',
      content: `将退款 ¥${formState.refundAmount.toFixed(2)} 到用户账户，此操作不可撤销`,
      okText: '确定',
      cancelText: '取消',
      onOk: async () => {
        try {
          loading.value = true
          
          const data = {
            payOrderId: props.payOrder.payOrderId,
            refundAmount: Math.round(formState.refundAmount * 100), // 转换为分
            refundReason: formState.refundReason,
            remark: formState.remark
          }
          
          await req.add(API_URL_REFUND_ORDER, data)
          message.success('退款申请已提交')
          
          handleClose()
          emit('success')
        } catch (error) {
          console.error('退款失败:', error)
          message.error(error.msg || '退款失败')
        } finally {
          loading.value = false
        }
      }
    })
  })
}

/**
 * 关闭弹窗
 */
const handleClose = () => {
  visible.value = false
}
</script>

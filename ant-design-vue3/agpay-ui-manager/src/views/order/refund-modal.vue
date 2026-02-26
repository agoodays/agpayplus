<template>
  <a-modal
    v-model:open="visible"
    :title="t('refund.modalTitle')"
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
        :message="t('refund.noticeTitle')"
        :description="t('refund.noticeDesc')"
        type="warning"
        show-icon
        style="margin-bottom: 24px"
      />

      <a-descriptions :column="1" bordered size="small" style="margin-bottom: 24px">
        <a-descriptions-item :label="t('refund.payOrderId')">
          <a-typography-text copyable>{{ payOrder?.payOrderId }}</a-typography-text>
        </a-descriptions-item>

        <a-descriptions-item :label="t('refund.orderAmount')">
          <span style="color: #1890ff; font-weight: 500">
            ¥{{ (payOrder?.amount / 100).toFixed(2) }}
          </span>
        </a-descriptions-item>

        <a-descriptions-item :label="t('refund.mchName')">
          {{ payOrder?.mchName }}
        </a-descriptions-item>
      </a-descriptions>

      <a-form-item :label="t('refund.refundAmount')" name="refundAmount">
        <a-input-number
          v-model:value="formState.refundAmount"
          :min="0.01"
          :max="payOrder?.amount / 100"
          :precision="2"
          :step="0.01"
          style="width: 100%"
          :placeholder="t('refund.pleaseInputRefundAmount')"
        >
          <template #addonBefore>
            ¥
          </template>
        </a-input-number>
        <div style="margin-top: 8px; color: rgba(0, 0, 0, 0.45)">
          {{ t('refund.maxRefundAmount') }}：¥{{ (payOrder?.amount / 100).toFixed(2) }}
        </div>
      </a-form-item>

      <a-form-item :label="t('refund.refundReason')" name="refundReason">
        <a-select
          v-model:value="formState.refundReason"
          :placeholder="t('refund.pleaseSelectRefundReason')"
        >
          <a-select-option :value="t('refund.reasonUserRequest')">{{ t('refund.reasonUserRequest') }}</a-select-option>
          <a-select-option :value="t('refund.reasonOrderException')">{{ t('refund.reasonOrderException') }}</a-select-option>
          <a-select-option :value="t('refund.reasonOutOfStock')">{{ t('refund.reasonOutOfStock') }}</a-select-option>
          <a-select-option :value="t('refund.reasonOther')">{{ t('refund.reasonOther') }}</a-select-option>
        </a-select>
      </a-form-item>

      <a-form-item :label="t('refund.remark')" name="remark">
        <a-textarea
          v-model:value="formState.remark"
          :placeholder="t('refund.pleaseInputRemarkOptional')"
          :rows="4"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, watch, nextTick } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import { API_URL_REFUND_ORDER, req } from '@/api/manage'

const { t } = useI18n()

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
    { required: true, message: t('refund.pleaseInputRefundAmount'), trigger: 'blur' },
    { 
      validator: (rule, value) => {
        if (value <= 0) {
          return Promise.reject(t('refund.amountMustGtZero'))
        }
        if (props.payOrder && value > props.payOrder.amount / 100) {
          return Promise.reject(t('refund.amountCannotExceedOrder'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  refundReason: [
    { required: true, message: t('refund.pleaseSelectRefundReason'), trigger: 'change' }
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
      title: t('refund.confirmTitle'),
      content: t('refund.confirmContent', { amount: formState.refundAmount.toFixed(2) }),
      okText: t('common.confirm'),
      cancelText: t('common.cancel'),
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
          message.success(t('refund.submitSuccess'))
          
          handleClose()
          emit('success')
        } catch (error) {
          console.error('退款失败:', error)
          message.error(error.msg || t('refund.submitFailed'))
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

<template>
  <!-- 退款弹窗组件 -->
  <a-modal
    v-model:open="localOpen"
    title="退款"
    :width="600"
    :confirm-loading="confirmLoading"
    @ok="handleSubmit"
    @cancel="handleClose"
    :mask-closable="false"
    :closable="false"
  >
    <!-- 订单信息展示 -->
    <a-descriptions :column="1" :bordered="false" style="margin-bottom: 24px">
      <a-descriptions-item label="支付订单号">
        <a-tag color="purple">
          <a-typography-text copyable>{{ detailData.payOrderId }}</a-typography-text>
        </a-tag>
      </a-descriptions-item>
      <a-descriptions-item label="支付金额">
        <a-tag color="green">
          ¥{{ (detailData.amount / 100).toFixed(2) }}
        </a-tag>
      </a-descriptions-item>
      <a-descriptions-item label="可退金额">
        <a-tag color="pink">
          ¥{{ nowRefundAmount.toFixed(2) }}
        </a-tag>
      </a-descriptions-item>
    </a-descriptions>

    <!-- 退款表单 -->
    <a-form ref="infoForm" :model="saveObject" :rules="rules" layout="vertical">
      <a-form-item label="退款金额" name="refundAmount">
        <a-input-number
          v-model:value="saveObject.refundAmount"
          :precision="2"
          :step="0.01"
          :min="0.01"
          :max="nowRefundAmount"
          style="width: 100%"
          placeholder="请输入退款金额"
        >
          <template #addonBefore>¥</template>
        </a-input-number>
        <div style="margin-top: 8px; color: rgba(0, 0, 0, 0.45)">
          退款金额不能小于0.01，或者大于可退金额
        </div>
      </a-form-item>

      <a-form-item label="退款原因" name="refundReason">
        <a-textarea
          v-model:value="saveObject.refundReason"
          :rows="3"
          placeholder="请输入退款原因，最长不超过256个字符"
          :maxlength="256"
          show-count
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { orderApi } from '@/api/business/order/order-api'
import { message, Modal } from 'ant-design-vue'
import { nextTick, reactive, ref, watch, computed } from 'vue'

/**
 * 组件属性定义
 * @param {boolean} open - 弹窗打开状态
 * @param {string} payOrderId - 支付订单ID
 * @param {object} payOrder - 支付订单对象
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  payOrderId: {
    type: String,
    default: ''
  },
  payOrder: {
    type: Object,
    default: null
  }
})

/**
 * 组件事件定义
 * @event update:open - 通知父组件弹窗状态变化
 * @event success - 退款成功回调
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 响应式状态定义
 */
const infoForm = ref(null)
const confirmLoading = ref(false)
const localOpen = ref(false)

/**
 * 订单详情数据
 */
const detailData = reactive({
  payOrderId: '',
  amount: 0,
  refundAmount: 0
})

/**
 * 表单数据
 */
const saveObject = reactive({
  refundAmount: 0,
  refundReason: ''
})

/**
 * 计算可退金额
 */
const nowRefundAmount = computed(() => {
  return (detailData.amount - detailData.refundAmount) / 100
})

/**
 * 表单验证规则
 */
const rules = {
  refundReason: [
    { min: 0, max: 256, required: true, trigger: 'blur', message: '请输入退款原因，最长不超过256个字符' }
  ],
  refundAmount: [
    { required: true, message: '请输入金额', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value < 0.01 || value > nowRefundAmount.value) {
          return Promise.reject('退款金额不能小于0.01，或者大于可退金额')
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}

/**
 * 监听props.open变化
 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && (props.payOrderId || props.payOrder)) {
      initForm()
    }
  }
)

/**
 * 监听localOpen变化，同步通知父组件
 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 初始化表单数据
 */
async function initForm() {
  try {
    if (props.payOrder) {
      Object.assign(detailData, props.payOrder)
    } else if (props.payOrderId) {
      const res = await orderApi.getPayOrderById(props.payOrderId)
      Object.assign(detailData, res)
    }
    saveObject.refundAmount = nowRefundAmount.value
    saveObject.refundReason = ''
    nextTick(() => {
      infoForm.value?.clearValidate()
    })
  } catch (error) {
    console.error('加载订单信息失败:', error)
    message.error(error.msg || '加载订单信息失败')
  }
}

/**
 * 提交退款请求
 */
async function handleSubmit() {
  try {
    await infoForm.value.validate()
    confirmLoading.value = true

    const res = await orderApi.createRefund({
      payOrderId: props.payOrderId,
      refundAmount: Math.round(saveObject.refundAmount * 100),
      refundReason: saveObject.refundReason
    })

    handleClose()
    confirmLoading.value = false

    if (res.state === 0 || res.state === 3) {
      Modal.error({
        title: '退款失败',
        content: buildModalContent(res),
        onOk: () => {
          emit('success')
        }
      })
    } else if (res.state === 1) {
      Modal.warning({
        title: '退款中',
        content: buildModalContent(res),
        onOk: () => {
          emit('success')
        }
      })
    } else if (res.state === 2) {
      message.success('退款成功')
      emit('success')
    } else {
      Modal.warning({
        title: '退款状态未知',
        content: buildModalContent(res),
        onOk: () => {
          emit('success')
        }
      })
    }
  } catch (error) {
    console.error('退款失败:', error)
    confirmLoading.value = false
    if (error.msg) {
      message.error(error.msg)
    }
  }
}

/**
 * 构建弹窗内容
 * @param {object} res - 退款结果
 * @returns {string} - 弹窗内容
 */
function buildModalContent(res) {
  let content = ''
  if (res.errCode) {
    content += `<div>错误码：${res.errCode}</div>`
  }
  if (res.errMsg) {
    content += `<div>错误信息：${res.errMsg}</div>`
  }
  content += '<div>请到退款列表中查看详细信息</div>'
  return content
}

/**
 * 关闭弹窗
 */
function handleClose() {
  emit('update:open', false)
}
</script>

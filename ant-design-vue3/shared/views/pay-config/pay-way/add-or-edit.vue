<template>
  <a-modal
    v-model:open="localOpen"
    :title="isAdd ? '新增支付方式' : '修改支付方式'"
    :width="700"
    @ok="handleOk"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      :label-col="{ span: 4 }"
      :wrapper-col="{ span: 18 }"
      :rules="rules"
    >
      <a-form-item label="支付方式代码：" name="wayCode">
        <a-input v-model:value="saveObject.wayCode" :disabled="!isAdd" />
      </a-form-item>
      <a-form-item label="支付方式名称：" name="wayName">
        <a-input v-model:value="saveObject.wayName" />
      </a-form-item>
      <a-form-item label="产品类型" name="productType">
        <a-radio-group v-model:value="saveObject.productType">
          <a-radio-button value="PAY">支付产品</a-radio-button>
          <a-radio-button value="TRANSFER">转账产品</a-radio-button>
          <a-radio-button value="DIVISION">分账产品</a-radio-button>
        </a-radio-group>
      </a-form-item>
      <a-form-item label="支付类型：" name="wayType">
        <a-radio-group v-model:value="saveObject.wayType">
          <a-radio-button v-if="saveObject.productType === 'PAY'" value="WECHAT">微信</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'PAY'" value="ALIPAY">支付宝</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'PAY'" value="YSFPAY">云闪付</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'PAY'" value="UNIONPAY">银联</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'PAY'" value="DCEPPAY">数字人民币</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'PAY'" value="OTHER">其他</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'DIVISION'" value="DIVISION">分账</a-radio-button>
          <a-radio-button v-if="saveObject.productType === 'TRANSFER'" value="TRANSFER">转账</a-radio-button>
        </a-radio-group>
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
/**
 * 支付方式新增/编辑组件
 * 功能：新增或修改支付方式配置
 */
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { reactive, ref, watch } from 'vue'
import { message } from 'ant-design-vue'

/**
 * 组件属性定义
 * @param {boolean} modelValue - 弹窗显示状态
 * @param {string} recordId - 支付方式代码，为空则为新增
 */
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

/** 组件事件 */
const emit = defineEmits(['update:open', 'success'])

/** 表单引用 */
const infoForm = ref(null)

/**
 * 是否为新增操作
 */
const isAdd = ref(true)

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 保存表单数据
 */
const saveObject = reactive({})

/**
 * 表单验证规则
 */
const rules = reactive({
  wayCode: [
    { required: true, message: '请输入支付方式代码', trigger: 'blur' }
  ],
  wayName: [
    { required: true, message: '请输入支付方式名称', trigger: 'blur' }
  ],
  productType: [
    { required: true, message: '请选择产品类型', trigger: 'blur' }
  ],
  wayType: [
    { required: true, message: '请选择支付类型', trigger: 'blur' }
  ]
})

/**
 * 监听 open 属性变化
 */
watch(() => props.open, (val) => {
  localOpen.value = val
  if (val) {
    initForm(props.recordId)
  }
})

/**
 * 监听本地 open 变化，同步 emit
 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 初始化表单
 * @param {string} wayCodeParam - 支付方式代码
 */
const initForm = async (wayCodeParam) => {
  isAdd.value = !wayCodeParam
  Object.assign(saveObject, {})

  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value) {
    const res = await payConfigApi.getPayWayById(wayCodeParam)
    Object.assign(saveObject, res)
  }
}

/**
 * 处理确认操作
 */
const handleOk = async () => {
  try {
    await infoForm.value.validate()
    loading.value = true

    if (isAdd.value) {
      await payConfigApi.addPayWay(saveObject)
      message.success('新增成功')
    } else {
      await payConfigApi.updatePayWayById(props.recordId, saveObject)
      message.success('修改成功')
    }

    emit('success')
    localOpen.value = false
  } catch (error) {
    console.error('操作失败:', error)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <a-modal :title="isAdd ? '新增支付方式' : '修改支付方式'" :open="isShow" :width="700" @ok="handleOkFunc" @cancel="isShow = false">
    <a-form
      ref="infoForm"
      :model="saveObject"
      :label-col="{span: 6}"
      :wrapper-col="{span: 15}"
      :rules="rules">
      <a-form-item label="支付方式代码：" name="wayCode">
        <a-input v-model:value="saveObject.wayCode" :disabled="!isAdd" />
      </a-form-item>
      <a-form-item label="支付方式名称：" name="wayName">
        <a-input v-model:value="saveObject.wayName" />
      </a-form-item>
      <a-form-item label="支付类型：" name="wayType">
        <a-radio-group v-model:value="saveObject.wayType" size="small" button-style="solid">
          <a-radio-button value="WECHAT">微信</a-radio-button>
          <a-radio-button value="ALIPAY">支付宝</a-radio-button>
          <a-radio-button value="YSFPAY">云闪付</a-radio-button>
          <a-radio-button value="UNIONPAY">银联</a-radio-button>
          <a-radio-button value="DCEPPAY">数字人民币</a-radio-button>
          <a-radio-button value="OTHER">其他</a-radio-button>
        </a-radio-group>
      </a-form-item>
    </a-form>
  </a-modal>
</template>
<script setup>
import { ref, reactive, defineProps } from 'vue'
import { API_URL_PAYWAYS_LIST, req } from '@/api/manage'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const isAdd = ref(true)
const isShow = ref(false)
const wayCode = ref(null)

const saveObject = reactive({})

const rules = reactive({
  wayCode: [
    { required: true, message: '请输入支付方式代码', trigger: 'blur' }
  ],
  wayName: [
    { required: true, message: '请输入支付方式名称', trigger: 'blur' }
  ],
  wayType: [
    { required: true, message: '请选择支付类型', trigger: 'blur' }
  ]
})

const show = (wayCodeParam) => {
  isAdd.value = !wayCodeParam
  Object.assign(saveObject, {}) // 数据清空

  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value) { // 修改信息 延迟展示弹层
    wayCode.value = wayCodeParam
    req.getById(API_URL_PAYWAYS_LIST, wayCodeParam).then(res => { 
      Object.assign(saveObject, res) 
    })
    isShow.value = true
  } else {
    isShow.value = true // 立马展示弹层信息
  }
}

const handleOkFunc = () => {
  infoForm.value.validate().then(() => {
    if (isAdd.value) {
      req.add(API_URL_PAYWAYS_LIST, saveObject).then(res => {
        import('ant-design-vue').then(({ message }) => {
          message.success('新增成功')
          isShow.value = false
          props.callbackFunc() // 刷新列表
        })
      })
    } else {
      req.updateById(API_URL_PAYWAYS_LIST, wayCode.value, saveObject).then(res => {
        import('ant-design-vue').then(({ message }) => {
          message.success('修改成功')
          isShow.value = false
          props.callbackFunc() // 刷新列表
        })
      })
    }
  }).catch(error => {
    console.error('验证失败:', error)
  })
}
</script>
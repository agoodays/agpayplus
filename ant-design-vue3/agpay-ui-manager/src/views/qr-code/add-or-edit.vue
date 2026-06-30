<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="isAdd ? '新增码牌' : '修改码牌'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    class="drawer-width"
    @close="onClose"
  >
    <a-form-model
      ref="infoFormModel"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 18 }"
      :rules="rules"
    >
      <a-form-model-item v-if="isAdd" label="批次号：" prop="batchId">
        <a-input-number v-model="saveObject.batchId" style="width: 70%; margin-right: 20px" />
        <a-button type="primary" size="small" @click="onToday">今天</a-button>
        <p class="agpay-tip-text">( 数字格式， 二维码编号的前缀， 建议采用： YYYYMMDD+次数表示 )</p>
      </a-form-model-item>
      <a-form-model-item v-if="isAdd" label="创建数量：" prop="addNum">
        <a-input-number v-model="saveObject.addNum" :min="1" :max="500" />
      </a-form-model-item>
      <a-form-model-item v-if="isAdd" label="选择模板" prop="qrcShellId">
        <a-select v-model="saveObject.qrcShellId" placeholder="请选择模板">
          <a-select-option key="" value="">无</a-select-option>
          <a-select-option v-for="d in shellList" :key="d.id" :value="d.id">
            <a-tooltip placement="left">
              <template #title>
                <span
                  ><img
                    :style="{ width: '100%', height: '100%', cursor: 'pointer' }"
                    :src="d.shellImgViewUrl"
                    alt=""
                    @click="onPreview(d.shellImgViewUrl)"
                /></span>
              </template>
              <span class="icon-style"><img class="icon" :src="d.shellImgViewUrl" alt="" /></span>
            </a-tooltip>
            {{ d.shellAlias }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
      <a-form-model-item label="状态" prop="state">
        <a-radio-group v-model="saveObject.state">
          <a-radio :value="1"> 启用 </a-radio>
          <a-radio :value="0"> 禁用 </a-radio>
        </a-radio-group>
      </a-form-model-item>
      <a-form-model-item label="固定金额" prop="fixedFlag">
        <a-radio-group v-model="saveObject.fixedFlag">
          <a-radio :value="0"> 任意金额 </a-radio>
          <a-radio :value="1"> 固定金额 </a-radio>
        </a-radio-group>
        <!--<a-input v-if="saveObject.fixedFlag===1" v-model="saveObject.fixedPayAmount" type="number" addon-after="元" style="width: 150px"/>-->
        <span v-if="saveObject.fixedFlag === 1"
          ><a-input-number v-model="saveObject.fixedPayAmount" addon-after="元" />元</span
        >
      </a-form-model-item>
      <a-form-model-item v-if="isAdd" prop="entryPage">
        <template #label>
          <span>
            <label title="选择页面类型" style="margin-right: 4px">扫码后页面类型</label>
            <!-- 选择页面类型 气泡弹窗 -->
            <!-- title可省略，就不显示 -->
            <a-popover placement="top">
              <template #content>
                <p>谨慎选择， 一经填写不可变更。</p>
              </template>
              <a-icon type="question-circle" />
            </a-popover>
          </span>
        </template>
        <a-radio-group v-model="saveObject.entryPage">
          <a-radio :value="'default'">
            默认
            <a-popover placement="top">
              <template #content>
                <p>未指定，取决于二维码是否绑定到微信侧</p>
              </template>
              <a-icon type="question-circle" />
            </a-popover>
          </a-radio>
          <a-radio :value="'h5'"> 固定H5页面 </a-radio>
          <a-radio :value="'lite'"> 固定小程序页面 </a-radio>
        </a-radio-group>
        <p class="agpay-tip-text">选择[默认/H5/小程序]任意一种后不可修改，请谨慎选择。</p>
      </a-form-model-item>
      <a-form-model-item prop="alipayWayCode">
        <template #label>
          <span>
            <label title="支付宝支付方式" style="margin-right: 4px">支付宝支付方式</label>
            <!-- 支付宝支付方式 气泡弹窗 -->
            <!-- title可省略，就不显示 -->
            <a-popover placement="top">
              <template #content>
                <p>仅H5呈现时生效</p>
              </template>
              <a-icon type="question-circle" />
            </a-popover>
          </span>
        </template>
        <a-radio-group v-model="saveObject.alipayWayCode">
          <a-radio :value="'ALI_JSAPI'"> ALI_JSAPI </a-radio>
          <a-radio :value="'ALI_WAP'"> ALI_WAP </a-radio>
        </a-radio-group>
        <br />
        <p class="agpay-tip-text">仅H5呈现时生效</p>
      </a-form-model-item>
    </a-form-model>
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
import { message } from 'ant-design-vue'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

function createDefaultSaveObject() {
  return {
    batchId: null,
    addNum: 1,
    state: 1,
    fixedFlag: 0,
    entryPage: 'default',
    alipayWayCode: 'ALI_JSAPI'
  }
}

const infoFormModel = ref(null)
const isAdd = ref(true)
const visible = ref(false)
const btnLoading = ref(false)
const shellList = ref(null)
const recordId = ref(null)
const saveObject = ref(createDefaultSaveObject())

const rules = {
  batchId: [{ required: true, message: '请输入批次号', trigger: 'blur' }],
  addNum: [
    { required: true, message: '请输入创建数量', trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        if (value < 1 || value > 500) {
          callback('数量请介于1-500之间')
          return
        }
        callback()
      },
      trigger: 'blur'
    }
  ]
}

async function show(currentRecordId) {
  isAdd.value = !currentRecordId
  saveObject.value = createDefaultSaveObject()
  infoFormModel.value?.resetFields?.()

  if (isAdd.value) {
    const [shellRes, batchIdRes] = await Promise.all([
      qrcApi.listShells({ pageSize: -1, state: 1 }),
      qrcApi.getBatchIdDistinctCount()
    ])
    shellList.value = shellRes.records
    saveObject.value.batchId = +batchIdRes
    visible.value = true
    return
  }

  recordId.value = currentRecordId
  const res = await qrcApi.getById(currentRecordId)
  saveObject.value = { ...res, fixedPayAmount: (res.fixedPayAmount / 100).toFixed(2) }
  visible.value = true
}

function onClose() {
  visible.value = false
}

function onToday() {
  const today = new Date()
  const year = today.getFullYear().toString()
  const month = (today.getMonth() + 1).toString().padStart(2, '0')
  const day = today.getDate().toString().padStart(2, '0')
  saveObject.value.batchId = +`${year}${month}${day}00`
}

function validateForm() {
  return new Promise((resolve) => {
    if (!infoFormModel.value?.validate) {
      resolve(true)
      return
    }
    infoFormModel.value.validate((valid) => resolve(valid))
  })
}

async function handleOkFunc() {
  const valid = await validateForm()
  if (!valid) return

  const params = { ...saveObject.value, fixedPayAmount: (saveObject.value.fixedPayAmount || 0) * 100 }
  if (isAdd.value) {
    await qrcApi.add(params)
    message.success('新增成功')
  } else {
    await qrcApi.updateById(recordId.value, params)
    message.success('修改成功')
  }
  visible.value = false
  props.callbackFunc()
}

function onPreview(url) {
  window.$viewerApi({
    images: [url],
    options: {
      initialViewIndex: 0
    }
  })
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less">
.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
}

.icon {
  width: 18.37px;
  height: 26px;
  margin-bottom: 3px;
}

.agpay-tip-text:before {
  content: '';
  width: 0;
  height: 0;
  border: 10px solid transparent;
  border-bottom-color: var(--warning-color);
  position: absolute;
  top: -20px;
  left: 30px;
}
.agpay-tip-text {
  font-size: 12px !important;
  border-radius: 5px;
  background: var(--warning-color);
  color: var(--text-on-primary) !important;
  padding: 5px 10px;
  display: inline-block;
  max-width: 100%;
  position: relative;
  margin-top: 15px;
  line-height: 1.5715;
}
</style>

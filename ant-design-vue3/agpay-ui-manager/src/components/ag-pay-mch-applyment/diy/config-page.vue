<template>
  <div>
    <div v-if="showCard" class="card">
      <div class="content-box">
        <a-form ref="infoForm" :model="saveObject">
          <a-row :gutter="24">
            <a-col v-for="(item, key) in formItems" :key="key" :span="item.span || 6" class="form-item">
              <a-form-item :label="item.label" :required="item.required" :rules="item.rules" :name="item.key">
                <a-input
                  v-if="item.type === 'text'"
                  v-model:value="saveObject[item.key]"
                  :disabled="item.readonly"
                  :placeholder="item.placeholder"
                />
                <a-textarea
                  v-else-if="item.type === 'textarea'"
                  v-model:value="saveObject[item.key]"
                  :disabled="item.readonly"
                  :placeholder="item.placeholder"
                />
                <a-select
                  v-else-if="item.type === 'select'"
                  v-model:value="saveObject[item.key]"
                  :disabled="item.readonly"
                  :placeholder="item.placeholder"
                >
                  <a-select-option v-for="(option, optionKey) in item.options" :key="optionKey" :value="option.value">
                    {{ option.label }}
                  </a-select-option>
                </a-select>
                <a-switch
                  v-else-if="item.type === 'switch'"
                  v-model:checked="saveObject[item.key]"
                  :disabled="item.readonly"
                />
                <ag-upload
                  v-else-if="item.type === 'upload'"
                  v-model:value="saveObject[item.key]"
                  :disabled="item.readonly"
                  :max-size="item.maxSize || 2"
                  :max-count="item.maxCount || 1"
                  :is-multiple="item.isMultiple || false"
                  :preview-mode="item.previewMode || 'file'"
                  :action="item.action"
                  :data="item.data"
                  :accept="item.accept"
                  :file-list="item.fileList"
                  :remove="item.remove"
                  :before-upload="item.beforeUpload"
                  :on-success="item.onSuccess"
                  :on-error="item.onError"
                  :on-progress="item.onProgress"
                  :on-change="item.onChange"
                  :custom-request="item.customRequest"
                  :list-type="item.listType || 'text'"
                  :show-upload-list="item.showUploadList !== false"
                  :auto-upload="item.autoUpload !== false"
                />
              </a-form-item>
            </a-col>
          </a-row>
          <a-form-item>
            <a-button type="primary" :loading="loading" @click="onSubmit"> 保存 </a-button>
          </a-form-item>
        </a-form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgUpload } from '@/components'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

const props = defineProps({
  infoId: {
    type: String,
    default: null
  },
  infoType: {
    type: String,
    default: null
  },
  ifDefine: {
    type: Object,
    default: null
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  callbackFunc: {
    type: Function,
    default: () => {}
  }
})

// State
const showCard = ref(false)
const loading = ref(false)
const infoForm = ref(null)
const saveObject = reactive({})
const formItems = ref([])

/**
 * 获取支付配置数据
 */
const getConfig = async () => {
  if (!props.ifDefine) return

  loading.value = true
  try {
    const res = await payConfigApi.getPayInterfaceSavedConfigs(props.configMode, props.infoId, props.ifDefine.ifCode)
    showCard.value = true
    if (res) {
      Object.assign(saveObject, res)
      saveObject.oauth2InfoId = res.oauth2InfoId || ''
      saveObject.cashoutParams = typeof res.cashoutParams === 'string' ? JSON.parse(res.cashoutParams || '{}') : res.cashoutParams || {}
    }
  } catch (error) {
    console.error('获取支付配置失败:', error)
  } finally {
    loading.value = false
  }
}

/**
 * 重置表单数据
 */
const reset = () => {
  showCard.value = false
  formItems.value = []
  Object.keys(saveObject).forEach((key) => {
    delete saveObject[key]
  })
}

/**
 * 提交表单
 */
const onSubmit = async () => {
  try {
    await infoForm.value.validate()
    loading.value = true
    const reqParams = {
      infoId: saveObject.infoId || props.infoId,
      infoType: saveObject.infoType || props.infoType,
      ifCode: saveObject.ifCode || props.ifDefine.ifCode,
      ifRate: saveObject.ifRate,
      state: saveObject.state,
      settHoldDay: saveObject.settHoldDay,
      isOpenApplyment: saveObject.isOpenApplyment,
      isOpenCashout: saveObject.isOpenCashout,
      cashoutParams: typeof saveObject.cashoutParams === 'string' ? saveObject.cashoutParams : JSON.stringify(saveObject.cashoutParams),
      isOpenCheckBill: saveObject.isOpenCheckBill,
      ignoreCheckBillMchNos: saveObject.ignoreCheckBillMchNos,
      remark: saveObject.remark,
      ifParams: '{}'
    }
    await payConfigApi.saveOrUpdatePayInterfaceConfig(reqParams)
    message.success('保存成功')
    props.callbackFunc()
  } catch (error) {
    console.error('保存支付配置失败:', error)
  } finally {
    loading.value = false
  }
}

// Watch
watch(
  () => props.ifDefine,
  (newVal) => {
    if (newVal) {
      getConfig()
    } else {
      reset()
    }
  },
  { immediate: true }
)

// Expose methods
defineExpose({
  getConfig,
  reset,
  onSubmit
})
</script>

<style scoped>
.card {
  margin: 0 20px 20px 0;
  min-height: 700px;
}

.content-box {
  padding: 30px 50px;
}

.form-item {
  margin-bottom: 24px;
}
</style>

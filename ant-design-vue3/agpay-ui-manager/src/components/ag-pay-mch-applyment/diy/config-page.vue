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
/**
 * 参数配置页面
 *
 * 根据后端返回的支付接口配置动态渲染表单，支持多种控件类型：
 * text、textarea、select、switch、upload。
 *
 * 通过 `defineExpose` 向上暴露 getConfig / reset / onSubmit 命令式方法，
 * 保存成功后通过 `success` 事件通知父组件。
 */
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgUpload } from '@/components'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

const props = defineProps({
  /** 信息 ID（如服务商/商户 ID） */
  infoId: {
    type: String,
    default: null
  },
  /** 信息类型 */
  infoType: {
    type: String,
    default: null
  },
  /** 渠道定义对象，包含 ifCode 等 */
  ifDefine: {
    type: Object,
    default: null
  },
  /** 权限编码 */
  permCode: {
    type: String,
    default: ''
  },
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['success'])

/** 是否展示卡片 */
const showCard = ref(false)
/** 保存按钮加载状态 */
const loading = ref(false)
/** 表单实例引用 */
const infoForm = ref(null)
/** 表单数据（需要整体替换，使用 ref） */
const saveObject = ref({})
/** 动态表单项配置列表 */
const formItems = ref([])

/**
 * 构建保存请求参数
 * @param {Object} data - 表单数据
 * @returns {Object} 请求参数
 */
const buildRequestParams = (data) => ({
  infoId: data.infoId || props.infoId,
  infoType: data.infoType || props.infoType,
  ifCode: data.ifCode || props.ifDefine.ifCode,
  ifRate: data.ifRate,
  state: data.state,
  settHoldDay: data.settHoldDay,
  isOpenApplyment: data.isOpenApplyment,
  isOpenCashout: data.isOpenCashout,
  cashoutParams: typeof data.cashoutParams === 'string' ? data.cashoutParams : JSON.stringify(data.cashoutParams),
  isOpenCheckBill: data.isOpenCheckBill,
  ignoreCheckBillMchNos: data.ignoreCheckBillMchNos,
  remark: data.remark,
  ifParams: '{}'
})

/**
 * 获取支付配置数据
 *
 * 根据配置模式、信息 ID 和渠道编码从后端拉取已保存的配置，
 * 并填充到表单数据对象中。
 */
const getConfig = async () => {
  if (!props.ifDefine) return

  loading.value = true
  try {
    const res = await payConfigApi.getPayInterfaceSavedConfigs(props.configMode, props.infoId, props.ifDefine.ifCode)
    showCard.value = true
    if (res) {
      saveObject.value = { ...res }
      saveObject.value.oauth2InfoId = res.oauth2InfoId || ''
      saveObject.value.cashoutParams =
        typeof res.cashoutParams === 'string' ? JSON.parse(res.cashoutParams || '{}') : res.cashoutParams || {}
    }
  } catch (error) {
    console.error('获取支付配置失败:', error)
  } finally {
    loading.value = false
  }
}

/**
 * 重置表单数据
 *
 * 隐藏卡片并清空表单数据与表单项配置。
 */
const reset = () => {
  showCard.value = false
  formItems.value = []
  saveObject.value = {}
}

/**
 * 提交表单
 *
 * 校验表单通过后，将表单数据组装为请求参数提交到后端。
 * 保存成功后触发 `success` 事件。
 */
const onSubmit = async () => {
  try {
    await infoForm.value.validate()
  } catch {
    return
  }

  loading.value = true
  try {
    const reqParams = buildRequestParams(saveObject.value)
    await payConfigApi.saveOrUpdatePayInterfaceConfig(reqParams)
    message.success('保存成功')
    emit('success')
  } catch (error) {
    console.error('保存支付配置失败:', error)
  } finally {
    loading.value = false
  }
}

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

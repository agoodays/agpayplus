<template>
  <div>
    <div v-if="showCard" class="card">
      <div class="content-box">
        <a-form v-bind="formItemLayout" ref="infoForm" :model="saveObject">
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
 * 支付配置 - 通用配置页面组件
 * 功能：根据支付接口定义动态渲染配置表单，支持文本、文本域、下拉选择、开关、上传等类型
 */
import { ref, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgUpload } from '@/components'

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

/** 是否显示卡片 */
const showCard = ref(false)
/** 加载状态 */
const loading = ref(false)
/** 表单引用 */
const infoForm = ref(null)
/** 表单数据对象 */
const saveObject = reactive({})
/** 表单字段配置 */
const formItems = ref([])

/** 表单布局配置 */
const formItemLayout = {
  labelCol: {
    xs: { span: 24 },
    sm: { span: 6 }
  },
  wrapperCol: {
    xs: { span: 24 },
    sm: { span: 18 }
  }
}

/**
 * 获取支付配置数据
 */
const getConfig = async () => {
  if (!props.ifDefine) return

  loading.value = true
  try {
    const res = await payConfigApi.getPayConfigById(props.infoId, props.ifDefine.ifCode)
    showCard.value = true
    formItems.value = res.configItems
    formItems.value.forEach((item) => {
      saveObject[item.key] = item.value
    })
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
    const params = {
      infoId: props.infoId,
      infoType: props.infoType,
      ifCode: props.ifDefine.ifCode,
      configItems: formItems.value.map((item) => ({
        key: item.key,
        value: saveObject[item.key]
      }))
    }
    await payConfigApi.addPayConfig(params)
    message.success('保存成功')
    props.callbackFunc()
  } catch (error) {
    // 表单验证失败或接口调用失败
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
  reset
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

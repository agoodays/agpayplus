<template>
  <div>
    <div v-if="showCard" class="card">
      <div class="content-box">
        <a-form v-bind="formItemLayout" ref="formRef" :model="formState">
          <a-row :gutter="24">
            <a-col v-for="(item, key) in formItems" :key="key" :span="item.span || 6" class="form-item">
              <a-form-item :label="item.label" :required="item.required" :rules="item.rules" :name="item.key">
                <a-input
                  v-if="item.type === 'text'"
                  v-model:value="formState[item.key]"
                  :disabled="item.readonly"
                  :placeholder="item.placeholder"
                />
                <a-textarea
                  v-else-if="item.type === 'textarea'"
                  v-model:value="formState[item.key]"
                  :disabled="item.readonly"
                  :placeholder="item.placeholder"
                />
                <a-select
                  v-else-if="item.type === 'select'"
                  v-model:value="formState[item.key]"
                  :disabled="item.readonly"
                  :placeholder="item.placeholder"
                >
                  <a-select-option v-for="(option, optionKey) in item.options" :key="optionKey" :value="option.value">
                    {{ option.label }}
                  </a-select-option>
                </a-select>
                <a-switch
                  v-else-if="item.type === 'switch'"
                  v-model:checked="formState[item.key]"
                  :disabled="item.readonly"
                />
                <ag-upload
                  v-else-if="item.type === 'upload'"
                  v-model:value="formState[item.key]"
                  :disabled="item.readonly"
                  :max-size="item.maxSize || 2"
                  :max-count="item.maxCount || 1"
                  :is-multiple="item.isMultiple || false"
                  :preview-mode="item.previewMode || 'file'"
                  :action="item.action"
                  :data="item.data"
                  :headers="item.headers"
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
            <a-button type="primary" :loading="btnLoading" @click="onSubmit"> 保存 </a-button>
          </a-form-item>
        </a-form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { message } from 'ant-design-vue'
import { API_URL_PAYCONFIGS_LIST, req } from '@/api/manage'
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

// State
const showCard = ref(false)
const btnLoading = ref(false)
const formRef = ref(null)
const formState = reactive({})
const formItems = ref([])

// Layout
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

// Methods
const getConfig = () => {
  if (props.ifDefine) {
    btnLoading.value = true
    req
      .get(API_URL_PAYCONFIGS_LIST, {
        infoId: props.infoId,
        infoType: props.infoType,
        ifCode: props.ifDefine.ifCode
      })
      .then((res) => {
        showCard.value = true
        formItems.value = res.configItems
        formItems.value.forEach((item) => {
          formState[item.key] = item.value
        })
        btnLoading.value = false
      })
  }
}

const reset = () => {
  showCard.value = false
  formItems.value = []
  Object.keys(formState).forEach((key) => {
    delete formState[key]
  })
}

const onSubmit = () => {
  formRef.value.validate((errors) => {
    if (!errors) {
      btnLoading.value = true
      const params = {
        infoId: props.infoId,
        infoType: props.infoType,
        ifCode: props.ifDefine.ifCode,
        configItems: formItems.value.map((item) => ({
          key: item.key,
          value: formState[item.key]
        }))
      }
      req.add(API_URL_PAYCONFIGS_LIST, params).then((res) => {
        message.success('保存成功')
        props.callbackFunc()
        btnLoading.value = false
      })
    }
  })
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

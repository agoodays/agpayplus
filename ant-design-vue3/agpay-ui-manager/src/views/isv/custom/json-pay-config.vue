<template>
  <ag-drawer
    title="支付参数配置"
    width="40%"
    :closable="true"
    v-model:open="localOpen"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    :mask-closable="false"
    @close="handleClose"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="支付接口费率" name="ifRate">
            <a-input v-model:value="saveObject.ifRate" placeholder="请输入" suffix="%" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入" type="textarea" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <a-divider orientation="left">
      <a-tag color="var(--error-color)"> {{ saveObject.ifCode }} 服务商参数配置 </a-tag>
    </a-divider>
    <a-form ref="isvParamForm" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16">
        <a-col v-for="(item, key) in isvParams" :key="key" :span="item.type === 'text' ? 12 : 24">
          <a-form-item
            v-if="item.type === 'text' || item.type === 'textarea'"
            :label="item.desc"
            :name="item.name"
          >
            <a-input
              v-if="item.star === '1'"
              v-model:value="ifParams[item.name]"
              :placeholder="ifParams[item.name + '_ph']"
              :type="item.type"
            />
            <a-input v-else v-model:value="ifParams[item.name]" placeholder="请输入" :type="item.type" />
          </a-form-item>
          <a-form-item v-else-if="item.type === 'radio'" :label="item.desc" :name="item.name">
            <a-radio-group v-model:value="ifParams[item.name]">
              <a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
                {{ radioItem.title }}
              </a-radio>
            </a-radio-group>
          </a-form-item>
          <a-form-item v-else-if="item.type === 'file'" :label="item.desc" :name="item.name">
            <ag-upload
              :action="action"
              :bind-name="item.name"
              :urls="[ifParams[item.name]]"
              list-type="picture"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" @click="handleClose">
        <template #icon><CloseOutlined /></template>
        取消
      </a-button>
      <a-button type="primary" :loading="loading" @click="onSubmit">
        <template #icon><CheckOutlined /></template>
        保存
      </a-button>
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 服务商 JSON 动态渲染支付配置组件
 * 功能：根据后端返回的配置定义动态渲染表单
 */
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import { AgDrawer, AgUpload } from '@/components'
import { usePayConfigDrawer } from '@/composables/usePayConfigDrawer'
import { getStateOptions } from '@/constants/common-const'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const stateOptions = computed(() => getStateOptions(t))

const icons = { LoadingOutlined, UploadOutlined }

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  isvNo: {
    type: String,
    default: ''
  },
  record: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const isvParamForm = ref(null)
const action = isvPayConfigApi.certUploadAction
const isvParams = ref([])
const saveObject = ref({})
const ifParams = ref({})
const ifParamsRules = ref({})

const rules = {
  infoId: [{ required: true, trigger: 'blur' }],
  ifCode: [{ required: true, trigger: 'blur' }],
  ifRate: [
    {
      required: false,
      pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/,
      message: '请输入0-100之间的数字，最多四位小数',
      trigger: 'blur'
    }
  ]
}

function parseJsonArray(rawValue) {
  if (Array.isArray(rawValue)) return rawValue
  if (!rawValue) return []
  try {
    const parsed = JSON.parse(rawValue)
    return Array.isArray(parsed) ? parsed : []
  } catch (_error) {
    return []
  }
}

function parseJsonObject(rawValue) {
  if (rawValue && typeof rawValue === 'object' && !Array.isArray(rawValue)) return rawValue
  if (!rawValue) return {}
  try {
    const parsed = JSON.parse(rawValue)
    return parsed && typeof parsed === 'object' ? parsed : {}
  } catch (_error) {
    return {}
  }
}

function buildRadioItems(item) {
  if (item.type !== 'radio') return []
  const valueItems = (item.values || '').split(',')
  const titleItems = (item.titles || '').split(',')
  return valueItems.map((rawValue, index) => {
    let value = rawValue
    if (value !== '' && !Number.isNaN(Number(rawValue))) {
      value = Number(rawValue)
    }
    return {
      value,
      title: titleItems[index]
    }
  })
}

function generateRules() {
  const rulesMap = {}
  isvParams.value.forEach((item) => {
    if (item.verify === 'required' && item.star !== '1') {
      rulesMap[item.name] = [
        {
          required: true,
          message: '请输入' + item.desc,
          trigger: 'blur'
        }
      ]
    }
  })
  ifParamsRules.value = rulesMap
}

async function loadConfigData() {
  infoForm.value?.resetFields?.()
  isvParamForm.value?.resetFields?.()

  saveObject.value = {
    infoId: props.isvNo,
    ifCode: props.record.ifCode,
    state: props.record.ifConfigState === 0 ? 0 : 1
  }
  ifParams.value = {}
  isvParams.value = []

  const res = await isvPayConfigApi.getUnique(saveObject.value.infoId, saveObject.value.ifCode)
  if (res?.ifParams) {
    saveObject.value = res
    ifParams.value = parseJsonObject(res.ifParams)
  }

  const parsedItems = parseJsonArray(props.record?.isvParams).map((item) => {
    if (item.star === '1') {
      const currentValue = ifParams.value[item.name]
      ifParams.value[item.name + '_ph'] = currentValue || '请输入'
      if (currentValue) {
        ifParams.value[item.name] = ''
      }
    }

    return {
      name: item.name,
      desc: item.desc,
      type: item.type,
      verify: item.verify,
      values: buildRadioItems(item),
      star: item.star
    }
  })

  isvParams.value = parsedItems
  generateRules()
}

const { localOpen, loading, submit, uploadSuccess: handleUploadSuccess, handleClose: closeDrawer } = usePayConfigDrawer({
  props,
  emit,
  infoForm,
  paramForm: isvParamForm,
  saveObject,
  ifParams,
  initialSaveObject: () => ({
    infoId: props.isvNo,
    ifCode: props.record.ifCode,
    state: props.record.ifConfigState === 0 ? 0 : 1,
    remark: ''
  }),
  initialIfParams: () => ({}),
  loadConfig: async () => {
    await loadConfigData()
  },
  buildSubmitPayload: ({ saveObject: currentSaveObject, ifParams: currentIfParams }) => {
    const submitParams = { ...currentIfParams }
    isvParams.value.forEach((item) => {
      if (item.star === '1' && submitParams[item.name] === '') {
        submitParams[item.name] = undefined
      }
      submitParams[item.name + '_ph'] = undefined
    })

    return {
      infoId: currentSaveObject.infoId,
      ifCode: currentSaveObject.ifCode,
      ifRate: currentSaveObject.ifRate,
      state: currentSaveObject.state,
      remark: currentSaveObject.remark,
      ifParams: JSON.stringify(submitParams)
    }
  },
  saveConfig: async (reqParams) => {
    await isvPayConfigApi.save(reqParams)
  },
  shouldInit: (propsData) => Boolean(propsData.isvNo && propsData.record?.ifCode)
})

const onSubmit = submit
const uploadSuccess = handleUploadSuccess
const handleClose = closeDrawer
</script>

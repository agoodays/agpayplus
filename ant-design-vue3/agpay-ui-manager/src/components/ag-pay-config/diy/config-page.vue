<template>
  <div>
    <BasePage ref="infoFormRef" :form-data="saveObject" :diy-list="diyList" />
    <a-divider orientation="left" v-if="ifDefineArray.length && saveObject.infoType !== 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} {{ saveObject.infoType === 'ISV' ? '服务商' : saveObject.infoType === 'MCH_APP' ? '商户' : saveObject.infoType === 'AGENT' ? '代理商' : '' }}参数配置
      </a-tag>
    </a-divider>
    <a-form v-if="saveObject.infoType !== 'AGENT'" ref="paramFormRef" :model="ifParams" layout="vertical">
      <a-row :gutter="16">
        <a-col v-for="(item, key) in ifDefineArray" :key="key" :span="item.type === 'text' ? 12 : 24">
          <a-form-item :label="item.desc" :name="item.name" :rules="getItemRules(item)">
            <a-input
              v-if="item.type === 'text' || item.type === 'textarea'"
              v-model:value="ifParams[item.name]"
              :placeholder="ifParams[item.name + '_ph'] || '请输入' + item.desc"
              :type="item.type"
            />
            <a-radio-group v-else-if="item.type === 'radio'" v-model:value="ifParams[item.name]">
              <a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
                {{ radioItem.title }}
              </a-radio>
            </a-radio-group>
            <ag-upload
              v-else-if="item.type === 'file'"
              :action="uploadAction"
              :bind-name="item.name"
              :urls="[ifParams[item.name]]"
              :list-type="'picture'"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? LoadingOutlined : UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
</template>

<script setup>
import { ref, reactive, watch, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { payOauth2Api } from '@/api/business/pay-oauth2/pay-oauth2-api'
import { upload } from '@/lib/ag-axios'
import BasePage from './base-page.vue'
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
  diyList: {
    type: Array,
    default: () => []
  },
  callbackFunc: {
    type: Function,
    default: () => {}
  }
})

const emit = defineEmits(['success'])

const infoFormRef = ref(null)
const paramFormRef = ref(null)
const uploadAction = ref(upload.cert)

const saveObject = reactive({
  infoId: props.infoId,
  infoType: props.infoType,
  ifCode: props.ifDefine?.ifCode || '',
  state: props.ifDefine?.ifConfigState === 0 ? 0 : 1,
  ifRate: null,
  settHoldDay: null,
  isOpenApplyment: 0,
  isOpenCashout: 0,
  cashoutParams: {
    isOpenMchOrderCashout: 0,
    isOpenMchTaskCashout: 0,
    minCashoutAmount: null,
    maxCashoutAmount: null,
    startTime: null,
    endTime: null
  },
  isOpenCheckBill: 0,
  ignoreCheckBillMchNos: null,
  isSupportApplyment: props.ifDefine?.isSupportApplyment === 1 ? 1 : 0,
  isSupportCashout: props.ifDefine?.isSupportCashout === 1 ? 1 : 0,
  isSupportCheckBill: props.ifDefine?.isSupportCheckBill === 1 ? 1 : 0,
  remark: '',
  oauth2InfoId: ''
})

const ifParams = reactive({})
const ifDefineArray = ref([])

const getItemRules = (item) => {
  const rules = []
  if (item.verify === 'required' && (item.star !== '1' || !ifParams[item.name + '_ph'])) {
    rules.push({
      required: true,
      message: '请输入' + item.desc,
      trigger: 'blur'
    })
  }
  return rules
}

const getPayConfig = async () => {
  if (!props.ifDefine) return

  try {
    const params = {
      configMode: props.configMode,
      infoId: saveObject.infoId,
      ifCode: saveObject.ifCode
    }
    const res = await payConfigApi.getPayInterfaceSavedConfigs(params.configMode, params.infoId, params.ifCode)

    if (res) {
      Object.assign(saveObject, res)
      saveObject.oauth2InfoId = res.oauth2InfoId || ''
      saveObject.cashoutParams = typeof res.cashoutParams === 'string' ? JSON.parse(res.cashoutParams || '{}') : res.cashoutParams || {}
      const parsedIfParams = typeof res.ifParams === 'string' ? JSON.parse(res.ifParams || '{}') : res.ifParams || {}
      Object.assign(ifParams, parsedIfParams)
    }

    const newItems = []
    const paramsData = props.ifDefine.mchType
      ? props.ifDefine.mchType === 1
        ? props.ifDefine.normalMchParams
        : props.ifDefine.isvsubMchParams
      : props.ifDefine.isvParams

    JSON.parse(paramsData || '[]').forEach(item => {
      const radioItems = []
      if (item.type === 'radio') {
        const valueItems = item.values.split(',')
        const titleItems = item.titles.split(',')
        for (const i in valueItems) {
          let radioVal = valueItems[i]
          if (!isNaN(radioVal)) {
            radioVal = Number(radioVal)
          }
          radioItems.push({
            value: radioVal,
            title: titleItems[i]
          })
        }
      }

      if (item.star === '1') {
        ifParams[item.name + '_ph'] = ifParams[item.name] || '请输入' + item.desc
        if (ifParams[item.name]) {
          ifParams[item.name] = ''
        }
      }

      newItems.push({
        name: item.name,
        desc: item.desc,
        type: item.type,
        verify: item.verify,
        values: radioItems,
        star: item.star
      })
    })

    ifDefineArray.value = newItems
  } catch (error) {
    console.error('获取支付配置失败:', error)
  }
}

const onSubmit = async () => {
  try {
    if (infoFormRef.value) {
      await infoFormRef.value.validate()
    }

    if (paramFormRef.value && ifDefineArray.value.length > 0) {
      await paramFormRef.value.validate()
    }

    if (Object.keys(ifParams).length === 0) {
      message.error('参数不能为空！')
      return
    }

    const ifParamsCopy = JSON.parse(JSON.stringify(ifParams) || '{}')
    ifDefineArray.value.forEach(item => {
      if (item.star === '1' && !ifParamsCopy[item.name]) {
        ifParamsCopy[item.name] = undefined
      }
      ifParamsCopy[item.name + '_ph'] = undefined
    })

    await submitRequest(JSON.stringify(ifParamsCopy))
  } catch (error) {
    console.error('保存支付配置失败:', error)
    throw error
  }
}

const submitRequest = async (ifParamsData = '{}') => {
  const reqParams = {
    infoId: saveObject.infoId,
    infoType: saveObject.infoType,
    ifCode: saveObject.ifCode,
    ifRate: saveObject.ifRate,
    state: saveObject.state,
    settHoldDay: saveObject.settHoldDay,
    isOpenApplyment: saveObject.isOpenApplyment,
    isOpenCashout: saveObject.isOpenCashout,
    cashoutParams: typeof saveObject.cashoutParams === 'string' ? saveObject.cashoutParams : JSON.stringify(saveObject.cashoutParams),
    isOpenCheckBill: saveObject.isOpenCheckBill,
    ignoreCheckBillMchNos: saveObject.ignoreCheckBillMchNos,
    remark: saveObject.remark,
    ifParams: ifParamsData
  }

  await payConfigApi.saveOrUpdatePayInterfaceConfig(reqParams)
  props.callbackFunc()
  emit('success')
}

const uploadSuccess = (name, fileList) => {
  const [firstItem] = fileList
  ifParams[name] = firstItem?.url
}

const hasPermission = (permCode) => {
  return true
}

watch(
  () => props.ifDefine,
  () => {
    getPayConfig()
  },
  { immediate: true }
)

defineExpose({
  onSubmit
})
</script>

<style scoped>
.drawer-btn-center {
  position: fixed;
  width: 90%;
}
</style>
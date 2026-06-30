<template>
  <a-drawer
    :visible="visible"
    title="支付参数列表"
    :closable="true"
    :drawer-style="{ overflow: 'hidden', backgroundColor: 'var(--layout-bg)' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    @close="onClose"
  >
    <ag-card ref="infoCard" :req-card-list-func="reqCardListFunc" :span="agpayCard.span" :height="agpayCard.height">
      <template #cardContentSlot="{ record }">
        <div>
          <div :style="{ height: agpayCard.height + 'px' }" class="ag-card-content">
            <!-- 卡片自定义样式 -->
            <div
              class="ag-card-content-header"
              :style="{ backgroundColor: record.bgColor, height: agpayCard.height / 2 + 'px' }"
            >
              <img v-if="record.icon" :src="record.icon" :style="{ height: agpayCard.height / 5 + 'px' }" />
            </div>
            <div class="ag-card-content-body" :style="{ height: agpayCard.height / 2 - 50 + 'px' }">
              <div class="title">
                {{ record.ifName }}
              </div>
              <a-badge
                :status="record.ifConfigState === 1 ? 'processing' : 'error'"
                :text="record.ifConfigState === 1 ? '启用' : '未开通'"
              ></a-badge>
            </div>
            <!-- 卡片底部操作栏 -->
            <div class="ag-card-ops">
              <a v-if="$access('ENT_ISV_PAY_CONFIG_ADD')" @click="editPayIfConfigFunc(record)"
                >填写参数 <a-icon key="right" type="right"></a-icon
              ></a>
              <a v-else>暂无操作</a>
            </div>
          </div>
        </div>
      </template>
    </ag-card>
    <a-drawer
      title="支付参数配置"
      width="40%"
      :closable="true"
      :visible="childrenVisible"
      :drawer-style="{ overflow: 'hidden' }"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
      :mask-closable="false"
      @close="onChildrenDrawerClose"
    >
      <a-form-model ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-model-item label="支付接口费率" prop="ifRate">
              <a-input v-model="saveObject.ifRate" placeholder="请输入" suffix="%" />
            </a-form-model-item>
          </a-col>
          <a-col :span="12">
            <a-form-model-item label="状态" prop="state">
              <a-radio-group v-model="saveObject.state">
                <a-radio :value="1"> 启用 </a-radio>
                <a-radio :value="0"> 停用 </a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
          <a-col :span="24">
            <a-form-model-item label="备注" prop="remark">
              <a-input v-model="saveObject.remark" placeholder="请输入" type="textarea" />
            </a-form-model-item>
          </a-col>
        </a-row>
      </a-form-model>
      <a-divider orientation="left">
        <a-tag color="var(--error-color)"> {{ saveObject.ifCode }} 服务商参数配置 </a-tag>
      </a-divider>
      <a-form-model ref="isvParamFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
        <a-row :gutter="16">
          <a-col v-for="(item, key) in isvParams" :key="key" :span="item.type === 'text' ? 12 : 24">
            <a-form-model-item
              v-if="item.type === 'text' || item.type === 'textarea'"
              :label="item.desc"
              :prop="item.name"
            >
              <a-input
                v-if="item.star === '1'"
                v-model="ifParams[item.name]"
                :placeholder="ifParams[item.name + '_ph']"
                :type="item.type"
              />
              <a-input v-else v-model="ifParams[item.name]" placeholder="请输入" :type="item.type" />
            </a-form-model-item>
            <a-form-model-item v-else-if="item.type === 'radio'" :label="item.desc" :prop="item.name">
              <a-radio-group v-model="ifParams[item.name]">
                <a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
                  {{ radioItem.title }}
                </a-radio>
              </a-radio-group>
            </a-form-model-item>
            <a-form-model-item v-else-if="item.type === 'file'" :label="item.desc" :prop="item.name">
              <ag-upload
                :action="action"
                :bind-name="item.name"
                :urls="[ifParams[item.name]]"
                list-type="picture"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-model-item>
          </a-col>
        </a-row>
      </a-form-model>
      <div class="drawer-btn-center">
        <a-button :style="{ marginRight: '8px' }" icon="close" @click="onChildrenDrawerClose"> 取消 </a-button>
        <a-button type="primary" :loading="btnLoading" icon="check" @click="onSubmit"> 保存 </a-button>
      </div>
    </a-drawer>
    <!-- 支付参数配置页面组件  -->
    <wxpay-pay-config ref="wxpayPayConfig" :callback-func="refCardList" />
    <!-- 支付参数配置页面组件  -->
    <alipay-pay-config ref="alipayPayConfig" :callback-func="refCardList" />
  </a-drawer>
</template>

<script setup>
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import AgCard from '@/components/ag-card'
import AgUpload from '@/components/ag-upload'
import { message } from 'ant-design-vue'
import { ref } from 'vue'
import AlipayPayConfig from './custom/alipay-pay-config.vue'
import WxpayPayConfig from './custom/wxpay-pay-config.vue'

const infoCard = ref(null)
const infoFormModel = ref(null)
const isvParamFormModel = ref(null)
const wxpayPayConfig = ref(null)
const alipayPayConfig = ref(null)

const btnLoading = ref(false)
const isvNo = ref(null)
const action = isvPayConfigApi.certUploadAction
const visible = ref(false)
const childrenVisible = ref(false)
const isvParams = ref([])
const saveObject = ref({})
const ifParams = ref({})
const ifParamsRules = ref({})

const agpayCard = {
  height: 300,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }
}

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

function show(currentIsvNo) {
  isvNo.value = currentIsvNo
  visible.value = true
  refCardList()
}

function reqCardListFunc() {
  return isvPayConfigApi.queryCardList(isvNo.value)
}

function refCardList() {
  infoCard.value?.refCardList?.()
}

async function editPayIfConfigFunc(record) {
  if (record.configPageType === 1) {
    infoFormModel.value?.resetFields?.()
    isvParamFormModel.value?.resetFields?.()

    childrenVisible.value = true
    saveObject.value = {
      infoId: isvNo.value,
      ifCode: record.ifCode,
      state: record.ifConfigState === 0 ? 0 : 1
    }
    ifParams.value = {}
    isvParams.value = []

    const res = await isvPayConfigApi.getUnique(saveObject.value.infoId, saveObject.value.ifCode)
    if (res?.ifParams) {
      saveObject.value = res
      ifParams.value = parseJsonObject(res.ifParams)
    }

    const parsedItems = parseJsonArray(record?.isvParams).map((item) => {
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
    return
  }

  if (record.configPageType === 2) {
    const customRefName = `${record.ifCode}PayConfig`
    const customRefMap = {
      wxpayPayConfig,
      alipayPayConfig
    }
    customRefMap[customRefName]?.value?.show(isvNo.value, record)
  }
}

function validateForm(formRef) {
  return new Promise((resolve) => {
    if (!formRef.value?.validate) {
      resolve(true)
      return
    }
    formRef.value.validate((valid) => {
      resolve(valid)
    })
  })
}

async function onSubmit() {
  const valid = await validateForm(infoFormModel)
  const valid2 = await validateForm(isvParamFormModel)
  if (!valid || !valid2) return

  btnLoading.value = true
  try {
    const reqParams = {
      infoId: saveObject.value.infoId,
      ifCode: saveObject.value.ifCode,
      ifRate: saveObject.value.ifRate,
      state: saveObject.value.state,
      remark: saveObject.value.remark
    }

    if (Object.keys(ifParams.value).length === 0) {
      message.error('参数不能为空！')
      return
    }

    const submitParams = { ...ifParams.value }
    isvParams.value.forEach((item) => {
      if (item.star === '1' && submitParams[item.name] === '') {
        submitParams[item.name] = undefined
      }
      submitParams[item.name + '_ph'] = undefined
    })

    reqParams.ifParams = JSON.stringify(submitParams)
    await isvPayConfigApi.save(reqParams)
    message.success('保存成功')
    childrenVisible.value = false
    refCardList()
  } finally {
    btnLoading.value = false
  }
}

function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  ifParams.value[name] = firstItem?.url
}

function onClose() {
  visible.value = false
}

function onChildrenDrawerClose() {
  childrenVisible.value = false
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
  border-radius: 6px;
  overflow: hidden;
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: var(--base-bg-color);
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid var(--layout-bg);
  position: absolute;
  bottom: 0;
}
.ag-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}
.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  align-items: center;
}
.title {
  font-size: 16px;
  font-family:
    PingFang SC,
    PingFang SC-Bold;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}
</style>

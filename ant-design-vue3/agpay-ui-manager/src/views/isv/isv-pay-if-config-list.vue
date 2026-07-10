<template>
  <ag-drawer
    v-model:open="localOpen"
    title="支付参数列表"
    :closable="true"
    :drawer-style="{ overflow: 'hidden', backgroundColor: 'var(--layout-bg)' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    @close="handleClose"
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
              <a v-if="hasPermission('ENT_ISV_PAY_CONFIG_ADD')" @click="editPayIfConfigFunc(record)">填写参数 <icons.RightOutlined /></a>
              <a v-else>暂无操作</a>
            </div>
          </div>
        </div>
      </template>
    </ag-card>
    <ag-drawer
      title="支付参数配置"
      width="40%"
      :closable="true"
      v-model:open="childrenVisible"
      :drawer-style="{ overflow: 'hidden' }"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
      :mask-closable="false"
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
              <a-radio-group v-model:value="saveObject.state">
                <a-radio :value="1"> 启用 </a-radio>
                <a-radio :value="0"> 停用 </a-radio>
              </a-radio-group>
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
        <a-button :style="{ marginRight: '8px' }" @click="onChildrenDrawerClose">
          <template #icon><CloseOutlined /></template>
          取消
        </a-button>
        <a-button type="primary" :loading="loading" @click="onSubmit">
          <template #icon><CheckOutlined /></template>
          保存
        </a-button>
      </div>
    </ag-drawer>
    <!-- 支付参数配置页面组件  -->
    <wxpay-pay-config ref="wxpayPayConfig" :callback-func="refCardList" />
    <!-- 支付参数配置页面组件  -->
    <alipay-pay-config ref="alipayPayConfig" :callback-func="refCardList" />
  </ag-drawer>
</template>

<script setup>
/**
 * ISV支付接口配置列表组件
 * 功能：展示ISV支付接口配置卡片列表，支持填写参数配置
 */
import { CheckOutlined, CloseOutlined, LoadingOutlined, RightOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { AgDrawer, AgCard, AgUpload } from '@/components'
import { usePermission } from '@/composables/useCommon'
const icons = { LoadingOutlined, RightOutlined, UploadOutlined }
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'
import AlipayPayConfig from './custom/alipay-pay-config.vue'
import WxpayPayConfig from './custom/wxpay-pay-config.vue'

// 权限检查
const { hasPermission } = usePermission()

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  isvNo: {
    type: String,
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open'])

const infoCard = ref(null)
const infoForm = ref(null)
const isvParamForm = ref(null)
const wxpayPayConfig = ref(null)
const alipayPayConfig = ref(null)

const loading = ref(false)
const action = isvPayConfigApi.certUploadAction
const localOpen = ref(false)
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

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.isvNo) {
      refCardList()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

function reqCardListFunc() {
  return isvPayConfigApi.queryCardList(props.isvNo)
}

function refCardList() {
  infoCard.value?.refCardList?.()
}

async function editPayIfConfigFunc(record) {
  if (record.configPageType === 1) {
    infoForm.value?.resetFields?.()
    isvParamForm.value?.resetFields?.()

    childrenVisible.value = true
    saveObject.value = {
      infoId: props.isvNo,
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
    customRefMap[customRefName]?.value?.show(props.isvNo, record)
  }
}

async function validateForm(formRef) {
  if (!formRef.value?.validate) {
    return true
  }
  try {
    await formRef.value.validate()
    return true
  } catch {
    return false
  }
}

async function onSubmit() {
  const valid = await validateForm(infoForm)
  const valid2 = await validateForm(isvParamForm)
  if (!valid || !valid2) return

  loading.value = true
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
    loading.value = false
  }
}

function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  ifParams.value[name] = firstItem?.url
}

/** 处理关闭 */
function handleClose() {
  localOpen.value = false
}

function onChildrenDrawerClose() {
  childrenVisible.value = false
}
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

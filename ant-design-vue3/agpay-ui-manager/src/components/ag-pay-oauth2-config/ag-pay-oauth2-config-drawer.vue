<template>
  <ag-drawer
    :visible="visible"
    :title="true ? 'Oauth2配置' : ''"
    :drawer-style="{ overflow: 'hidden', backgroundColor: '#f0f2f5' }"
    :body-style="{ padding: '24px', overflowY: 'auto' }"
    width="80%"
    @close="onClose"
  >
    <div v-show="configMode === 'mgrIsv'">
      <div style="margin-bottom: 20px">
        <label>选择配置的条目：</label>
        <a-select
          v-model:value="diyListSelectedInfoId"
          placeholder=""
          style="width: 380px; margin-right: 20px"
          @change="getSavedConfigs"
        >
          <a-select-option :value="infoId">默认</a-select-option>
          <a-select-option v-for="(item, key) in diyList" :key="key" :value="item.infoId">
            {{ item.remark + ' [ ID: ' + item.infoId + ' ]' }}
          </a-select-option>
        </a-select>
        <a-button v-show="diyAddMode === 'init'" type="primary" icon="plus" @click="diyAddMode = 'adding'"
          >创建</a-button
        >
      </div>
      <div v-show="diyAddMode === 'adding'">
        <label>输入名称：</label>
        <a-input v-model:value="addDiyListName" placeholder="" style="width: 160px" />
        <a-checkbox
          style="margin-left: 20px"
          :checked="addDiyListIsCopyCurrentFlag"
          @change="addDiyListIsCopyCurrentFlag = !addDiyListIsCopyCurrentFlag"
        >
          复制当前参数
        </a-checkbox>
        <a-popover placement="top">
          <template #content>
            <p>勾选： 新创建的oauth2参数将来源自当前选择条目的记录值。且创建副本，互不干扰。</p>
            <p>不勾选： 创建全新的记录， 所有的参数需要重新填入。</p>
          </template>
          <template #title>
            <span>复制当前参数</span>
          </template>
          <icons.QuestionCircleOutlined />
        </a-popover>
        <a-button type="danger" icon="check" :style="{ marginLeft: '20px' }" @click="onSave">保存</a-button>
        <a-button type="primary" icon="close" :style="{ marginLeft: '8px' }" @click="diyAddMode = 'init'"
          >取消</a-button
        >
      </div>
      <a-divider />
    </div>
    <a-tabs v-model:activeKey="currentIfCode" type="card" @change="getSavedConfigs">
      <a-tab-pane v-for="item in tabData" :key="item.code" :tab="item.name" />
    </a-tabs>
    <a-card style="padding: 30px">
      <component
        :is="currentComponent"
        ref="currentComponentRef"
        :config-mode="configMode"
        :form-data="ifParams"
        @update-if-params="handleUpdateIfParams"
      />
      <div style="display: flex; justify-content: space-around; flex-direction: row">
        <a-button type="primary" icon="check" :loading="btnLoading" @click="onSubmit">保存</a-button>
      </div>
    </a-card>
  </ag-drawer>
</template>

<script setup>
import { QuestionCircleOutlined } from '@ant-design/icons-vue'
const icons = { QuestionCircleOutlined }
import { ref, nextTick, markRaw } from 'vue'
import { message } from 'ant-design-vue'
import { AgDrawer } from '@/components'
import { payOauth2Api } from '@/api/business/pay-oauth2/pay-oauth2-api'

const props = defineProps({
  configMode: { type: String, default: null }
})

const visible = ref(false)
const infoId = ref(null)
const btnLoading = ref(false)
const isIsvSubMch = ref(false)
const diyListSelectedInfoId = ref('')
const diyList = ref([])
const diyAddMode = ref('init')
const addDiyListName = ref('')
const addDiyListIsCopyCurrentFlag = ref(true)
const currentIfCode = ref('wxpay')
const tabData = ref([
  { code: 'wxpay', name: '微信' },
  { code: 'alipay', name: '支付宝' }
])
const currentComponent = ref(null)
const saveObject = ref({})
const ifParams = ref({})
const currentComponentRef = ref(null)

const show = (infoIdValue, isIsvSubMchValue) => {
  infoId.value = infoIdValue
  diyListSelectedInfoId.value = infoIdValue
  isIsvSubMch.value = isIsvSubMchValue
  visible.value = true
  if (props.configMode === 'mgrIsv') {
    getDiyList()
  }
  nextTick(() => {
    getSavedConfigs()
  })
}

const onClose = () => {
  visible.value = false
  infoId.value = null
  isIsvSubMch.value = false
  diyListSelectedInfoId.value = ''
  diyList.value = []
  diyAddMode.value = 'init'
  addDiyListName.value = ''
  addDiyListIsCopyCurrentFlag.value = true
  currentIfCode.value = 'wxpay'
  saveObject.value = {}
  currentComponent.value = null
}

const getCurrentComponent = () => {
  const suffix = isIsvSubMch.value ? 'IsvSubMch' : ''
  switch (currentIfCode.value) {
    case 'wxpay':
      return import(`./diy/wxpay/${suffix}Oauth2ConfigPage.vue`)
    case 'alipay':
      return import(`./diy/alipay/${suffix}Oauth2ConfigPage.vue`)
    default:
      return Promise.reject(new Error('Unknown variable dynamic import: ' + currentIfCode.value))
  }
}

const getDiyList = async () => {
  const res = await payOauth2Api.queryDiyList({ configMode: props.configMode, infoId: infoId.value })
  diyList.value = res
}

const getSavedConfigs = async () => {
  currentComponent.value = null
  const params = Object.assign(
    {},
    { configMode: props.configMode, infoId: diyListSelectedInfoId.value, ifCode: currentIfCode.value }
  )
  const res = await payOauth2Api.querySavedConfigs(params)
  if (res) {
    saveObject.value = res
    ifParams.value = JSON.parse(res.ifParams || '{}')
    if (currentIfCode.value === 'alipay') {
      ifParams.value.liteParams = ifParams.value.liteParams || {}
    }
    if (isIsvSubMch.value) {
      ifParams.value.isUseSubmchAccount = ifParams.value.isUseSubmchAccount || 0
    }
  }
  await nextTick()
  try {
    const module = await getCurrentComponent()
    currentComponent.value = markRaw(module.default || module)
  } catch {
    currentComponent.value = null
    message.error('当前渠道不支持Oauth2配置！')
  }
}

const handleUpdateIfParams = (params) => {
  ifParams.value = params
}

const onSave = async () => {
  if (!addDiyListName.value) {
    message.error('请输入名称')
    return
  }
  window.$infoBox.confirmPrimary('确认新增该服务商的配置条目？', '新建后不支持修改/删除，请谨慎操作', async () => {
    const params = Object.assign(
      {},
      {
        infoId: infoId.value,
        configMode: props.configMode,
        remark: addDiyListName.value,
        copySourceInfoId: diyListSelectedInfoId.value
      }
    )
    await payOauth2Api.createDiyList(params)
    message.success('保存成功')
    await getDiyList()
  })
}

const onSubmit = async () => {
  try {
    await currentComponentRef.value.validate()
  } catch {
    return
  }
  if (Object.keys(ifParams.value).length === 0) {
    message.error('参数不能为空！')
    return
  }
  const params = currentComponentRef.value.handleStarParams()
  saveObject.value.ifParams = JSON.stringify(params)
  btnLoading.value = true
  try {
    await payOauth2Api.saveConfigParams(saveObject.value)
    message.success('保存成功')
  } finally {
    btnLoading.value = false
  }
}

defineExpose({ show })
</script>

<style scoped>
::v-deep(.ant-tabs-bar) {
  border-bottom: 1px solid #f0f2f5;
}
::v-deep(.ant-tabs.ant-tabs-card .ant-tabs-card-bar .ant-tabs-tab-active) {
  border-color: #fff;
}
::v-deep(.ant-collapse-borderless) {
  background-color: #ffffff;
}
::v-deep(.ant-collapse-borderless > .ant-collapse-item) {
  border-bottom: 0px solid #ffffff;
}
</style>
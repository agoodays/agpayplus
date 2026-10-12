<template>
  <ag-drawer
    v-model:open="localOpen"
    width="50%"
    :title="t(isAdd ? 'payConfig.ifDefine.addTitle' : 'payConfig.ifDefine.editTitle')"
    :mask-closable="false"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
    @close="handleClose"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      layout="vertical"
      :rules="rules"
    >
      <a-row :gutter="16">
        <a-col :span="24">
          <p class="ag-drawer-title">{{ t('payConfig.ifDefine.basicConfig') }}</p>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.ifCode')" name="ifCode">
            <a-input v-model:value="saveObject.ifCode" :placeholder="t('payConfig.ifDefine.pleaseInput')" :disabled="!isAdd" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.ifName')" name="ifName">
            <a-input v-model:value="saveObject.ifName" :placeholder="t('payConfig.ifDefine.pleaseInput')" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.supportMerchantMode')" name="isMchMode">
            <a-radio-group v-model:value="saveObject.isMchMode" :options="supportStatusOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.supportIsvSubMerchantMode')" name="isIsvMode">
            <a-radio-group v-model:value="saveObject.isIsvMode" :options="supportStatusOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item :label="t('payConfig.ifDefine.configPageType')" name="configPageType">
            <a-radio-group v-model:value="saveObject.configPageType">
              <a-radio :value="1">{{ t('payConfig.ifDefine.renderFromDefinition') }}</a-radio>
              <a-radio :value="2">{{ t('payConfig.ifDefine.customPage') }}</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.supportApplyment')" name="isSupportApplyment">
            <a-radio-group v-model:value="saveObject.isSupportApplyment" :options="supportStatusOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.openApplyment')" name="isOpenApplyment">
            <a-radio-group v-model:value="saveObject.isOpenApplyment" :disabled="!saveObject.isSupportApplyment" :options="openStatusOptions"/>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.supportReconciliation')" name="isSupportCheckBill">
            <a-radio-group v-model:value="saveObject.isSupportCheckBill" :options="supportStatusOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.openReconciliation')" name="isOpenCheckBill">
            <a-radio-group v-model:value="saveObject.isOpenCheckBill" :disabled="!saveObject.isSupportCheckBill" :options="openStatusOptions"/>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.supportCashout')" name="isSupportCashout">
            <a-radio-group v-model:value="saveObject.isSupportCashout" :options="supportStatusOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.openCashout')" name="isOpenCashout">
            <a-radio-group v-model:value="saveObject.isOpenCashout" :disabled="!saveObject.isSupportCashout" :options="openStatusOptions"/>
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isIsvMode == 1 && saveObject.configPageType === 1">
          <a-form-item :label="t('payConfig.ifDefine.isvParams')" name="isvParams">
            <a-textarea v-model:value="saveObject.isvParams" :placeholder="t('payConfig.ifDefine.pleaseInput')" :rows="4" />
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isIsvMode == 1 && saveObject.configPageType === 1">
          <a-form-item :label="t('payConfig.ifDefine.isvSubMerchantParams')" name="isvsubMchParams">
            <a-textarea v-model:value="saveObject.isvsubMchParams" :placeholder="t('payConfig.ifDefine.pleaseInput')" :rows="4" />
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isMchMode == 1 && saveObject.configPageType === 1">
          <a-form-item :label="t('payConfig.ifDefine.normalMerchantParams')" name="normalMchParams">
            <a-textarea v-model:value="saveObject.normalMchParams" :placeholder="t('payConfig.ifDefine.pleaseInput')" :rows="4" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.state')" name="state">
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.remark')" name="remark">
            <a-input v-model:value="saveObject.remark" :placeholder="t('payConfig.ifDefine.pleaseInput')" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left" />
        </a-col>
        <a-col :span="24">
          <p class="ag-drawer-title">{{ t('payConfig.ifDefine.paymentMethods') }}</p>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item :label="t('payConfig.ifDefine.supportedPaymentMethods')" name="checkedList">
            <a-checkbox-group v-model:value="checkedList">
              <div v-for="(group, index) in groupedWays" :key="index" class="pay-way-group">
                <div class="pay-way-group-title">{{ group.name }}</div>
                <a-row :gutter="8">
                  <a-col v-for="(way, i) in group.ways" :key="i">
                    <a-checkbox :value="way.wayCode">{{ way.wayName }}</a-checkbox>
                  </a-col>
                </a-row>
              </div>
            </a-checkbox-group>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left" />
        </a-col>
        <a-col :span="24">
          <p class="ag-drawer-title">{{ t('payConfig.ifDefine.pageDisplay') }}</p>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.cardIcon')" name="icon">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="icon"
              :urls="[saveObject.icon]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> {{ t('components.upload') }} </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item :label="t('payConfig.ifDefine.cardBackgroundColor')" name="bgColor">
            <color-picker
              class="ag-color-picker"
              v-model:pureColor="saveObject.bgColor"
              v-bind="{ isWidget:false, format:'hex', shape:'square', lang:'ZH-cn' }"
            />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>

<script setup>
/**
 * 支付接口定义新增/编辑组件
 * 功能：新增或修改支付接口定义配置
 */
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgDrawer, AgUpload } from '@/components'
import { getOpenStatusOptions, getStateOptions, getSupportStatusOptions } from '@/constants/common-const'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, defineEmits, defineProps, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))
const openStatusOptions = computed(() => getOpenStatusOptions(t))
const supportStatusOptions = computed(() => getSupportStatusOptions(t))

const icons = { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined }

/**
 * Props 定义
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  ifCode: {
    type: String,
    default: ''
  }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 表单引用
 */
const infoForm = ref(null)

/**
 * 本地抽屉打开状态，避免直接修改 props
 */
const localOpen = ref(false)

/**
 * 是否为新增模式
 */
const isAdd = ref(true)

/**
 * 按钮加载状态
 */
const loading = ref(false)

/**
 * 上传地址
 */
const action = payConfigApi.getIfBgUploadAction()

/**
 * 监听抽屉打开状态变化
 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val) {
      await loadData()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

const createDefaultSaveObject = () => ({
  isMchMode: 1,
  isIsvMode: 1,
  state: 1,
  configPageType: 1,
  isSupportApplyment: 0,
  isOpenApplyment: 0,
  isSupportCheckBill: 0,
  isOpenCheckBill: 0,
  isSupportCashout: 0,
  isOpenCashout: 0,
  bgColor: '#1a53ff'
})

const saveObject = ref(createDefaultSaveObject())

const groupedWays = ref([])

/**
 * 选中的支付方式列表
 */
const checkedList = ref([])

/**
 * 表单验证规则
 */
const rules = computed(() => ({
  ifCode: [{ required: true, message: t('payConfig.ifDefine.ifCodeRequired'), trigger: 'blur' }],
  ifName: [{ required: true, message: t('payConfig.ifDefine.ifNameRequired'), trigger: 'blur' }],
  normalMchParams: [{
    required: true, 
    validator: (rule, value) => {
      if (saveObject.value.isMchMode === 1 && saveObject.value.configPageType === 1 && !value) {
        return Promise.reject(new Error(t('payConfig.ifDefine.normalMerchantParamsRequired')))
      }
      return Promise.resolve()
    },
    trigger: 'blur'
  }],
  isvParams: [{
    required: true, 
    validator: (rule, value) => {
      if (saveObject.value.isIsvMode === 1 && saveObject.value.configPageType === 1 && !value) {
        return Promise.reject(new Error(t('payConfig.ifDefine.isvParamsRequired')))
      }
      return Promise.resolve()
    },
    trigger: 'blur'
  }],
  isvsubMchParams: [{
    required: true, 
    validator: (rule, value) => {
      if (saveObject.value.isIsvMode === 1 && saveObject.value.configPageType === 1 && !value) {
        return Promise.reject(new Error(t('payConfig.ifDefine.isvSubMerchantParamsRequired')))
      }
      return Promise.resolve()
    },
    trigger: 'blur'
  }],
  checkedList: [{
    required: true,
    validator: (rule, value) => {
      if (checkedList.value.length <= 0) {
        return Promise.reject(new Error(t('payConfig.ifDefine.paymentMethodRequired')))
      }
      return Promise.resolve()
    },
    trigger: 'blur'
  }]
}))

/**
 * 加载表单数据
 * 新增时初始化默认数据，编辑时从接口获取数据
 */
async function loadData() {
  isAdd.value = !props.ifCode

  saveObject.value = createDefaultSaveObject()

  /** 重置表单 */
  if (infoForm.value) {
    infoForm.value.resetFields()
    infoForm.value.clearValidate?.()
  }

  checkedList.value = []

  if (!isAdd.value) {
    const res = await payConfigApi.getIfDefineById(props.ifCode)
    saveObject.value = { ...createDefaultSaveObject(), ...res }
    const newItems = []
    res.wayCodes.forEach(item => {
      newItems.push(item.wayCode)
    })
    checkedList.value = newItems
  } else {
    checkedList.value = []
  }
}

/** 处理关闭 */
const handleClose = () => {
  saveObject.value = createDefaultSaveObject()
  checkedList.value = []
  if (infoForm.value) {
    infoForm.value.resetFields()
    infoForm.value.clearValidate?.()
  }
  localOpen.value = false
}

/**
 * 表单提交
 */
const handleConfirm = async () => {
  try {
    await infoForm.value.validate()
    saveObject.value.wayCodeStrs = checkedList.value.join(',')

    if (isAdd.value) {
      await payConfigApi.addIfDefine(saveObject.value)
      message.success(t('common.addSuccess'))
    } else {
      await payConfigApi.updateIfDefineById(props.ifCode, saveObject.value)
      message.success(t('common.editSuccess'))
    }

    localOpen.value = false
    emit('success')
  } catch (error) {
    console.error('操作失败:', error)
  }
}

/**
 * 按指定键分组列表
 * @param {Array} list - 列表数据
 * @param {string} key - 分组键
 * @returns {Object} 分组结果
 */
const groupBy = (list, key) => {
  return list.reduce((acc, item) => {
    (acc[item[key]] = acc[item[key]] || []).push(item)
    return acc
  }, {})
}

/**
 * 获取分组名称
 * @param {string} wayType - 支付方式类型
 * @returns {string} 分组名称
 */
const getGroupName = (wayType) => {
  switch (wayType) {
    case 'WECHAT':
      return t('common.wayType.wechat')
    case 'ALIPAY':
      return t('common.wayType.alipay')
    case 'YSFPAY':
      return t('common.wayType.ysfpay')
    case 'UNIONPAY':
      return t('common.wayType.unionpay')
    case 'DCEPPAY':
      return t('common.wayType.dceppay')
    default:
      return t('common.wayType.other')
  }
}

/**
 * 加载支付方式列表
 */
const payWayList = async () => {
  const res = await payConfigApi.queryPayWayList({ pageSize: -1 })
  const ways = res.records

  const groupedWaysData = groupBy(ways, 'wayType')

  const order = ['WECHAT', 'ALIPAY', 'YSFPAY', 'UNIONPAY', 'DCEPPAY', 'OTHER']
  const sortedGroupedWays = Object.fromEntries(
    Object.entries(groupedWaysData)
      .sort(([keyA], [keyB]) => {
        const indexA = order.indexOf(keyA)
        const indexB = order.indexOf(keyB)
        return indexA - indexB
      })
  )

  groupedWays.value = []
  for (const wayType in sortedGroupedWays) {
    const group = {
      name: getGroupName(wayType),
      ways: []
    }
    for (const way of sortedGroupedWays[wayType]) {
      group.ways.push({ wayCode: way.wayCode, wayName: way.wayName })
    }
    groupedWays.value.push(group)
  }
}

/**
 * 上传文件成功回调方法
 * @param {string} name - 字段名
 * @param {Array} fileList - 文件列表
 */
const uploadSuccess = (name, fileList) => {
  const [firstItem] = fileList
  saveObject.value[name] = firstItem?.url
}

/**
 * 组件挂载时加载支付方式列表
 */
onMounted(() => {
  payWayList()
})
</script>

<style lang="less" scoped>
.ag-drawer-title {
  font-size: 16px;
  font-weight: 600;
  width: 100%;
  margin-bottom: 15px;
}

.pay-way-group {
  margin-bottom: 16px;

  &:last-child {
    margin-bottom: 0;
  }
}

.pay-way-group-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-color);
  margin-bottom: 8px;
  padding-left: 4px;
}
</style>

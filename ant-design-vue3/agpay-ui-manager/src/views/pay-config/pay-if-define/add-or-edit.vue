<template>
  <ag-drawer
    width="50%"
    v-model:open="localOpen"
    :mask-closable="false"
    :title="isAdd ? '新增支付接口' : '修改支付接口'"
    :show-confirm="true"
    :confirm-loading="loading"
    @close="handleClose"
    @confirm="handleConfirm"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      layout="vertical"
      :rules="rules"
    >
      <a-row :gutter="16">
        <a-col :span="24">
          <p class="ag-drawer-title">基本配置</p>
        </a-col>
        <a-col :span="12">
          <a-form-item label="接口代码" name="ifCode">
            <a-input v-model:value="saveObject.ifCode" placeholder="请输入" :disabled="!isAdd" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="接口名称" name="ifName">
            <a-input v-model:value="saveObject.ifName" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持普通商户模式" name="isMchMode">
            <a-radio-group v-model:value="saveObject.isMchMode">
              <a-radio :value="1">支持</a-radio>
              <a-radio :value="0">不支持</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持服务商子商户模式" name="isIsvMode">
            <a-radio-group v-model:value="saveObject.isIsvMode">
              <a-radio :value="1">支持</a-radio>
              <a-radio :value="0">不支持</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="支付参数配置页面类型" name="configPageType">
            <a-radio-group v-model:value="saveObject.configPageType">
              <a-radio :value="1">根据接口配置定义描述渲染页面</a-radio>
              <a-radio :value="2">自定义页面</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持进件" name="isSupportApplyment">
            <a-radio-group v-model:value="saveObject.isSupportApplyment">
              <a-radio :value="1">支持</a-radio>
              <a-radio :value="0">不支持</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否开启进件" name="isOpenApplyment">
            <a-radio-group v-model:value="saveObject.isOpenApplyment" :disabled="!saveObject.isSupportApplyment">
              <a-radio :value="1">开启</a-radio>
              <a-radio :value="0">关闭</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持对账" name="isSupportCheckBill">
            <a-radio-group v-model:value="saveObject.isSupportCheckBill">
              <a-radio :value="1">支持</a-radio>
              <a-radio :value="0">不支持</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否开启对账" name="isOpenCheckBill">
            <a-radio-group v-model:value="saveObject.isOpenCheckBill" :disabled="!saveObject.isSupportCheckBill">
              <a-radio :value="1">开启</a-radio>
              <a-radio :value="0">关闭</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持提现" name="isSupportCashout">
            <a-radio-group v-model:value="saveObject.isSupportCashout">
              <a-radio :value="1">支持</a-radio>
              <a-radio :value="0">不支持</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否开启提现" name="isOpenCashout">
            <a-radio-group v-model:value="saveObject.isOpenCashout" :disabled="!saveObject.isSupportCashout">
              <a-radio :value="1">开启</a-radio>
              <a-radio :value="0">关闭</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isIsvMode == 1 && saveObject.configPageType === 1">
          <a-form-item label="服务商接口配置定义描述" name="isvParams">
            <a-textarea v-model:value="saveObject.isvParams" placeholder="请输入" :rows="4" />
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isIsvMode == 1 && saveObject.configPageType === 1">
          <a-form-item label="特约商户接口配置定义描述" name="isvsubMchParams">
            <a-textarea v-model:value="saveObject.isvsubMchParams" placeholder="请输入" :rows="4" />
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isMchMode == 1 && saveObject.configPageType === 1">
          <a-form-item label="普通商户接口配置定义描述" name="normalMchParams">
            <a-textarea v-model:value="saveObject.normalMchParams" placeholder="请输入" :rows="4" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left" />
        </a-col>
        <a-col :span="24">
          <p class="ag-drawer-title">支付方式</p>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="支持的支付方式" name="checkedList">
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
          <p class="ag-drawer-title">页面展示</p>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="卡片icon" name="icon">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="icon"
              :urls="[saveObject.icon]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="卡片背景色" name="bgColor">
            <color-picker v-model="saveObject.bgColor" locale="zh-CN" />
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
import { AgDrawer, AgUpload } from '@/components'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { onMounted, reactive, ref, watch, computed } from 'vue'
import { message } from 'ant-design-vue'
import { getStateOptions } from '@/constants/common-const'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

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

const saveObject = reactive({})

const groupedWays = ref([])

/**
 * 选中的支付方式列表
 */
const checkedList = ref([])

/**
 * 表单验证规则
 */
const rules = reactive({
  ifCode: [{ required: true, message: '请输入接口代码', trigger: 'blur' }],
  ifName: [{ required: true, message: '请输入接口名称', trigger: 'blur' }],
  normalMchParams: [{
    validator: (rule, value, callback) => {
      if (saveObject.isMchMode === 1 && saveObject.configPageType === 1 && !value) {
        callback(new Error('请输入普通商户接口配置定义描述'))
      }
      callback()
    },
    trigger: 'blur'
  }],
  isvParams: [{
    validator: (rule, value, callback) => {
      if (saveObject.isIsvMode === 1 && saveObject.configPageType === 1 && !value) {
        callback(new Error('请输入服务商接口配置定义描述'))
      }
      callback()
    },
    trigger: 'blur'
  }],
  isvsubMchParams: [{
    validator: (rule, value, callback) => {
      if (saveObject.isIsvMode === 1 && saveObject.configPageType === 1 && !value) {
        callback(new Error('请输入特约商户接口配置定义描述'))
      }
      callback()
    },
    trigger: 'blur'
  }],
  checkedList: [{
    required: true,
    validator: (rule, value, callback) => {
      if (checkedList.value.length <= 0) {
        callback(new Error('请选择支付方式'))
      }
      callback()
    },
    trigger: 'blur'
  }]
})

/**
 * 加载表单数据
 * 新增时初始化默认数据，编辑时从接口获取数据
 */
async function loadData() {
  isAdd.value = !props.ifCode

  /** 初始化表单默认值 */
  Object.assign(saveObject, {
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

  /** 重置表单 */
  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value) {
    const res = await payConfigApi.getIfDefineById(props.ifCode)
    Object.assign(saveObject, res)
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
  localOpen.value = false
}

/**
 * 表单提交
 */
const handleConfirm = async () => {
  try {
    await infoForm.value.validate()
    saveObject.wayCodeStrs = checkedList.value.join(',')

    if (isAdd.value) {
      await payConfigApi.addIfDefine(saveObject)
      message.success('新增成功')
    } else {
      await payConfigApi.updateIfDefineById(props.ifCode, saveObject)
      message.success('修改成功')
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
      return '微信'
    case 'ALIPAY':
      return '支付宝'
    case 'YSFPAY':
      return '云闪付'
    case 'UNIONPAY':
      return '银联'
    case 'DCEPPAY':
      return '数字人民币'
    default:
      return '其他'
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
  saveObject[name] = firstItem?.url
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

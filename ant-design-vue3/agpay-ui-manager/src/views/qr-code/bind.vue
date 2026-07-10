<template>
  <ag-drawer
    v-model:open="localOpen"
    :mask-closable="false"
    title="绑定码牌"
    width="40%"
    @close="handleClose"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      layout="vertical"
      :rules="rules"
    >
      <a-row :gutter="16">
        <a-col :span="10">
          <a-form-item label="商户号" name="mchNo">
            <ag-select
              v-model:value="saveObject.mchNo"
              :api="searchMch"
              value-field="mchNo"
              label-field="mchName"
              placeholder="商户号（搜索商户名称）"
              @change="mchNoChange"
            />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="应用" name="appId">
            <a-select v-model:value="saveObject.appId" placeholder="请选择应用">
              <a-select-option key="" value="">请选择应用</a-select-option>
              <a-select-option v-for="d in appList" :key="d.appId" :value="d.appId">
                {{ d.appName + ' [ AppId: ' + d.appId + ' ]' }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="门店" name="storeId">
            <a-select v-model:value="saveObject.storeId" placeholder="请选择门店">
              <a-select-option key="" value="">请选择门店</a-select-option>
              <a-select-option v-for="d in storeList" :key="d.storeId" :value="d.storeId">
                {{ d.storeName + ' [ ID: ' + d.storeId + ' ]' }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>
<script setup>
/**
 * 二维码绑定组件
 * 功能：将二维码绑定到商户、应用和门店
 */
import { AgDrawer, AgSelect } from '@/components'
import { qrcApi } from '@/api/business/qr-code/qrc-api'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

/**
 * 组件属性定义
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: [String, Number],
    default: null
  }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 本地打开状态
 */
const localOpen = ref(props.open)

/**
 * 监听外部打开状态变化
 */
watch(() => props.open, (val) => {
  localOpen.value = val
})

/**
 * 表单引用
 */
const infoForm = ref(null)

/**
 * 按钮加载状态
 */
const loading = ref(false)

/**
 * 表单保存数据对象
 */
const saveObject = ref({})

/**
 * 应用列表
 */
const appList = ref(null)

/**
 * 门店列表
 */
const storeList = ref(null)

/**
 * 表单验证规则
 */
const rules = {
  mchNo: [{ required: true, message: '请选择商户', trigger: 'blur' }],
  appId: [{ required: true, message: '请选择应用', trigger: 'blur' }],
  storeId: [{ required: true, message: '请选择门店', trigger: 'blur' }]
}

/**
 * 加载数据
 */
const loadData = async () => {
  if (!props.recordId) return
  const res = await qrcApi.getById(props.recordId)
  saveObject.value = res
  if (res.mchNo) {
    await mchNoChange()
  }
}

/**
 * 监听打开状态，加载数据
 */
watch(() => props.open, async (val) => {
  if (val) {
    await loadData()
  }
})

/**
 * 关闭抽屉
 */
const handleClose = () => {
  emit('update:open', false)
}

/**
 * 搜索商户
 * @param {Object} params - 搜索参数
 * @returns {Promise<Object>} 商户列表
 */
const searchMch = (params) => qrcApi.searchMch(params)

/**
 * 商户号变更时加载应用和门店列表
 */
const mchNoChange = async () => {
  if (saveObject.value.mchNo) {
    const [appRes, storeRes] = await Promise.all([
      qrcApi.listMchApps({ mchNo: saveObject.value.mchNo, pageSize: -1, state: 1 }),
      qrcApi.listMchStores({ mchNo: saveObject.value.mchNo, pageSize: -1, state: 1 })
    ])
    appList.value = appRes.records
    storeList.value = storeRes.records
    return
  }
  appList.value = null
  storeList.value = null
  saveObject.value.appId = null
  saveObject.value.storeId = null
}

/**
 * 验证表单
 * @returns {boolean} 验证结果
 */
const validateForm = async () => {
  if (!infoForm.value?.validate) {
    return true
  }
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

/**
 * 确认绑定
 */
const handleConfirm = async () => {
  const valid = await validateForm()
  if (!valid) return

  loading.value = true
  try {
    await qrcApi.bindById(props.recordId, saveObject.value)
    message.success('绑定成功')
    emit('success')
    emit('update:open', false)
  } finally {
    loading.value = false
  }
}
</script>

<style lang="less"></style>
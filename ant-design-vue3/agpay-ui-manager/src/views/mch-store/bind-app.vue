<template>
  <a-modal
    v-model:open="localOpen"
    :title="t('mchStore.bindAppTitle')"
    :width="500"
    @ok="handleSubmit"
    @cancel="handleClose"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical">
      <a-form-item :label="t('mchStore.pleaseSelectBindApp')" name="bindAppId">
        <a-select v-model:value="saveObject.bindAppId" :placeholder="t('mchStore.pleaseSelectApp')" :loading="loading">
          <a-select-option value="">{{ t('mchStore.emptyOption') }}</a-select-option>
          <a-select-option v-for="item in appList" :key="item.appId" :value="item.appId">
            {{ item.appName }} [{{ item.appId }}]
          </a-select-option>
        </a-select>
      </a-form-item>

      <a-alert
        v-if="appList.length === 0 && !loading"
        :message="t('mchStore.noAvailableApp')"
        type="warning"
        show-icon
      />
    </a-form>
  </a-modal>
</template>

<script setup>
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { mchStoreApi } from '@/api/business/mch-store/mch-store-api'
import { message, Modal } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// Props & Emits
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  storeId: {
    type: [String, Number],
    default: ''
  },
  bindAppId: {
    type: String,
    default: ''
  },
  mchNo: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open', 'success'])

// State
const infoForm = ref(null)
const loading = ref(false)
const localOpen = ref(false)
const appList = ref([])

// 表单数据
const saveObject = reactive({
  bindAppId: ''
})

// 监听 props.open 变化
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val) {
      initForm()
    }
  }
)

// 监听 localOpen 变化
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 初始化表单
 */
const initForm = async () => {
  // 设置当前绑定的应用
  saveObject.bindAppId = props.bindAppId || ''

  // 加载应用列表
  await loadAppList()
}

/**
 * 加载应用列表
 */
const loadAppList = async () => {
  try {
    loading.value = true
    const res = await mchAppApi.queryByMchNo(props.mchNo)
    appList.value = res.records || []
  } catch (error) {
    console.error('加载应用列表失败:', error)
    message.error(t('mchStore.loadAppListFailed'))
  } finally {
    loading.value = false
  }
}

/**
 * 提交表单
 */
const handleSubmit = () => {
  Modal.confirm({
    title: t('mchStore.confirmBindTitle'),
    content: t('mchStore.confirmBindContent'),
    okText: t('common.confirm'),
    cancelText: t('common.cancel'),
    onOk: async () => {
      try {
        loading.value = true

        const data = {
          bindAppId: saveObject.bindAppId || null
        }

        await mchStoreApi.updateById(props.storeId, data)
        message.success(t('mchStore.bindAppSuccess'))

        handleClose()
        emit('success')
      } catch (error) {
        console.error('绑定失败:', error)
        message.error(error.msg || t('mchStore.bindAppFailed'))
      } finally {
        loading.value = false
      }
    }
  })
}

/**
 * 关闭弹窗
 */
const handleClose = () => {
  emit('update:open', false)
}
</script>

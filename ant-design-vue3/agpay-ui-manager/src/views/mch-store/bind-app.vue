<template>
  <a-modal
    v-model:open="open"
    :title="t('mchStore.bindAppTitle')"
    :width="500"
    @ok="handleSubmit"
    @cancel="handleClose"
  >
    <a-form
      ref="formRef"
      :model="formState"
      layout="vertical"
    >
      <a-form-item :label="t('mchStore.pleaseSelectBindApp')" name="bindAppId">
        <a-select
          v-model:value="formState.bindAppId"
          :placeholder="t('mchStore.pleaseSelectApp')"
          :loading="loading"
        >
          <a-select-option value="">{{ t('mchStore.emptyOption') }}</a-select-option>
          <a-select-option
            v-for="item in appList"
            :key="item.appId"
            :value="item.appId"
          >
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
import { ref, reactive, watch } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import { API_URL_MCH_APP, API_URL_MCH_STORE, req } from '@/api/manage'

const { t } = useI18n()

// Props & Emits
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  storeId: {
    type: String,
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
const formRef = ref()
const loading = ref(false)
const open = ref(false)
const appList = ref([])

// 表单数据
const formState = reactive({
  bindAppId: ''
})

// 监听 props.open 变化
watch(() => props.open, (val) => {
  open.value = val
  if (val) {
    initForm()
  }
})

// 监听 open 变化
watch(open, (val) => {
  emit('update:open', val)
})

/**
 * 初始化表单
 */
const initForm = async () => {
  // 设置当前绑定的应用
  formState.bindAppId = props.bindAppId || ''
  
  // 加载应用列表
  await loadAppList()
}

/**
 * 加载应用列表
 */
const loadAppList = async () => {
  try {
    loading.value = true
    const res = await req.list(API_URL_MCH_APP, {
      pageSize: -1,
      mchNo: props.mchNo
    })
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
          bindAppId: formState.bindAppId || null
        }
        
        await req.updateById(API_URL_MCH_STORE, props.storeId, data)
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
  open.value = false
}
</script>

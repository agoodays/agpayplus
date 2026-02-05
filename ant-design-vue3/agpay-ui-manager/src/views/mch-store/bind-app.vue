<template>
  <a-modal
    v-model:open="visible"
    title="应用分配"
    :width="500"
    @ok="handleSubmit"
    @cancel="handleClose"
  >
    <a-form
      ref="formRef"
      :model="formState"
      layout="vertical"
    >
      <a-form-item label="请选择要绑定的应用" name="bindAppId">
        <a-select
          v-model:value="formState.bindAppId"
          placeholder="请选择应用"
          :loading="loading"
        >
          <a-select-option value="">（空）</a-select-option>
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
        message="该商户暂无可用应用"
        type="warning"
        show-icon
      />
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { API_URL_MCH_APP, API_URL_MCH_STORE, req } from '/@/api/manage'

// Props & Emits
const props = defineProps({
  visible: {
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

const emit = defineEmits(['update:visible', 'success'])

// State
const formRef = ref()
const loading = ref(false)
const visible = ref(false)
const appList = ref([])

// 表单数据
const formState = reactive({
  bindAppId: ''
})

// 监听 props.visible 变化
watch(() => props.visible, (val) => {
  visible.value = val
  if (val) {
    initForm()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:visible', val)
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
    message.error('加载应用列表失败')
  } finally {
    loading.value = false
  }
}

/**
 * 提交表单
 */
const handleSubmit = () => {
  Modal.confirm({
    title: '确认保存应用吗？',
    content: '请确认是否绑定选中的应用',
    okText: '确定',
    cancelText: '取消',
    onOk: async () => {
      try {
        loading.value = true
        
        const data = {
          bindAppId: formState.bindAppId || null
        }
        
        await req.updateById(API_URL_MCH_STORE, props.storeId, data)
        message.success('绑定应用成功')
        
        handleClose()
        emit('success')
      } catch (error) {
        console.error('绑定失败:', error)
        message.error(error.msg || '绑定失败')
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
  visible.value = false
}
</script>

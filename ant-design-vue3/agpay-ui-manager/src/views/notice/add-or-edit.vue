<template>
  <ag-drawer
    v-model:open="localOpen"
    :mask-closable="false"
    :title="isAdd ? '新增公告' : '修改公告'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    :width="drawerWidth"
    class="drawer-width"
    @close="handleClose"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="公告标题" name="title">
            <a-input v-model:value="saveObject.title" placeholder="请输入公告标题" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="公告副标题" name="subtitle">
            <a-input v-model:value="saveObject.subtitle" placeholder="请输入公告副标题" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="公告范围" name="articleRange">
            <a-checkbox-group v-model:value="saveObject.articleRange" :options="articleRangeOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="发布人" name="publisher">
            <a-input v-model:value="saveObject.publisher" placeholder="请输入发布人" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="公告内容" name="content">
            <ag-editor v-model:modelValue="saveObject.content" :height="438"></ag-editor>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>

<script setup>
/**
 * 公告新增/编辑抽屉组件
 * 功能：公告信息的新增和编辑
 */
import { AgDrawer, AgEditor } from '@/components'
import { noticeApi } from '@/api/business/notice/notice-api'
import { message } from 'ant-design-vue'
import { computed, onBeforeUnmount, onMounted, reactive, ref, watch } from 'vue'

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

/** 表单引用 */
const infoForm = ref(null)

/** 本地状态 */
const localOpen = ref(false)
const loading = ref(false)
const isAdd = ref(true)
const viewportWidth = ref(window.innerWidth)

/** 抽屉宽度（响应式） */
const drawerWidth = computed(() => (viewportWidth.value < 1200 ? '94%' : '60%'))

/** 公告范围选项 */
const articleRangeOptions = [
  { label: '商户', value: 'MCH' },
  { label: '代理商', value: 'AGENT' }
]

/** 保存对象 */
const saveObject = reactive({
  title: '',
  subtitle: '',
  publisher: '',
  articleRange: [],
  content: ''
})

/** 窗口大小变化处理 */
const onResize = () => {
  viewportWidth.value = window.innerWidth
}

onMounted(() => {
  window.addEventListener('resize', onResize)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', onResize)
})

/** 表单验证规则 */
const rules = {
  title: [{ required: true, message: '请输入公告标题', trigger: 'blur' }],
  subtitle: [{ required: true, message: '请输入公告副标题', trigger: 'blur' }],
  publisher: [{ required: true, message: '请填写发布人', trigger: 'blur' }],
  articleRange: [
    {
      required: true,
      validator: (_rule, value) => {
        if (!value?.length) {
          return Promise.reject(new Error('请选择公告范围'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}

/** 获取默认保存对象 */
function getDefaultSaveObject() {
  return {
    title: '',
    subtitle: '',
    publisher: '',
    articleRange: [],
    content: ''
  }
}

/** 初始化表单 */
async function initForm(currentRecordId) {
  isAdd.value = !currentRecordId
  Object.assign(saveObject, getDefaultSaveObject())

  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value && currentRecordId) {
    try {
      const res = await noticeApi.getById(currentRecordId)
      Object.assign(saveObject, res || {})
    } catch (error) {
      console.error('加载公告信息失败:', error)
      message.error('加载公告信息失败，请重试')
    }
  }
}

/** 监听 open 属性变化 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val) {
      await initForm(props.recordId)
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 验证表单 */
async function validateForm() {
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

/** 确认提交 */
async function handleConfirm() {
  if (loading.value) return

  const valid = await validateForm()
  if (!valid) return

  loading.value = true
  try {
    if (isAdd.value) {
      await noticeApi.add(saveObject)
      message.success('新增成功')
    } else {
      await noticeApi.updateById(props.recordId, saveObject)
      message.success('修改成功')
    }
    localOpen.value = false
    emit('success')
  } catch (error) {
    if (!error.errorFields) {
      message.error('操作失败')
    }
  } finally {
    loading.value = false
  }
}

/** 处理关闭 */
function handleClose() {
  localOpen.value = false
}
</script>

<style lang="less">
.upload-list-inline .ant-btn {
  height: 66px;
}
</style>

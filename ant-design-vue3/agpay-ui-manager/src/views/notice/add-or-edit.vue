<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="isAdd ? '新增公告' : '修改公告'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    :width="drawerWidth"
    class="drawer-width"
    @close="onClose"
  >
    <a-form v-if="visible" ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
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
            <!--vue2父组件的v-model，相当于-->
            <!--<ag-editor :value="saveObject.content" @input="saveObject.content = $event"></ag-editor>-->
            <!--vue3父组件的v-model，相当于-->
            <!--<ag-editor :height="438" :modelValue="saveObject.content" @update:modelValue="saveObject.content = $event"></ag-editor>-->
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" style="margin-right: 8px" @click="onClose">
        <template #icon><close-outlined /></template>
        取消
      </a-button>
      <a-button type="primary" :loading="btnLoading" @click="onSubmit">
        <template #icon><check-circle-outlined /></template>
        保存
      </a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { noticeApi } from '@/api/business/notice/notice-api'
import AgEditor from '@/components/ag-editor'
import { CheckCircleOutlined, CloseOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const btnLoading = ref(false)
const isAdd = ref(true)
const saveObject = ref({})
const recordId = ref(null)
const visible = ref(false)
const viewportWidth = ref(window.innerWidth)
const drawerWidth = computed(() => (viewportWidth.value < 1200 ? '94%' : '60%'))

const articleRangeOptions = [
  { label: '商户', value: 'MCH' },
  { label: '代理商', value: 'AGENT' }
]

const onResize = () => {
  viewportWidth.value = window.innerWidth
}

onMounted(() => {
  window.addEventListener('resize', onResize)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', onResize)
})

const checkArticleRange = (_rule, value, callback) => {
  if (!value?.length) {
    callback(new Error('请选择公告范围'))
    return
  }
  callback()
}

const rules = {
  title: [{ required: true, message: '请输入公告标题', trigger: 'blur' }],
  subtitle: [{ required: true, message: '请输入公告副标题', trigger: 'blur' }],
  publisher: [{ required: true, message: '请填写发布人', trigger: 'blur' }],
  articleRange: [{ required: true, validator: checkArticleRange, trigger: 'blur' }]
}

async function show(id) {
  isAdd.value = !id
  saveObject.value = {}
  recordId.value = id || null
  infoForm.value?.resetFields?.()
  visible.value = true

  if (!isAdd.value && recordId.value) {
    try {
      const res = await noticeApi.getById(recordId.value)
      saveObject.value = res || {}
    } catch (_e) {
      message.error('加载公告信息失败，请重试')
    }
  }
}

async function validateForm() {
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

async function onSubmit() {
  if (btnLoading.value) return

  const valid = await validateForm()
  if (!valid) return

  btnLoading.value = true
  try {
    if (isAdd.value) {
      await noticeApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await noticeApi.updateById(recordId.value, saveObject.value)
      message.success('修改成功')
    }
    visible.value = false
    props.callbackFunc()
  } finally {
    btnLoading.value = false
  }
}

function onClose() {
  visible.value = false
}

defineExpose({ show })
</script>

<style lang="less">
.upload-list-inline .ant-btn {
  height: 66px;
}
</style>

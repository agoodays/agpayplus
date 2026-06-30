<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="isAdd ? '新增团队' : '修改团队'"
    class="drawer-width"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    :width="drawerWidth"
    @close="onClose"
  >
    <a-form-model v-if="visible" ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="团队名称" prop="teamName">
            <a-input v-model="saveObject.teamName" placeholder="请输入团队名称" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="团队编号" prop="teamNo">
            <a-input v-model="saveObject.teamNo" placeholder="请输入团队编号" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="团队编号" prop="statRangeType">
            <a-select v-model="saveObject.statRangeType" placeholder="统计周期" default-value="year">
              <a-select-option value="year">年</a-select-option>
              <a-select-option value="quarter">季度</a-select-option>
              <a-select-option value="month">月</a-select-option>
              <a-select-option value="week">周</a-select-option>
            </a-select>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" style="margin-right: 8px" @click="onClose">
        <template #icon><close-outlined /></template>
        取消
      </a-button>
      <a-button type="primary" :loading="btnLoading" @click="onSubmit">
        <template #icon><check-outlined /></template>
        保存
      </a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { teamApi } from '@/api/business/sys-user-team/team-api'
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoFormModel = ref()
const btnLoading = ref(false)
const isAdd = ref(true)
const saveObject = ref({})
const recordId = ref(null)
const visible = ref(false)
const viewportWidth = ref(window.innerWidth)
const drawerWidth = computed(() => (viewportWidth.value < 992 ? '92%' : '40%'))

const onResize = () => {
  viewportWidth.value = window.innerWidth
}

onMounted(() => {
  window.addEventListener('resize', onResize)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', onResize)
})

const checkStatRangeType = (_rule, value, callback) => {
  if (isAdd.value && !value) {
    callback(new Error('请选择统计周期'))
    return
  }
  callback()
}

const rules = {
  teamName: [{ required: true, message: '请输入团队名称', trigger: 'blur' }],
  teamNo: [{ required: true, message: '请输入团队编号', trigger: 'blur' }],
  statRangeType: [{ required: true, validator: checkStatRangeType, trigger: 'blur' }]
}

async function show(id) {
  isAdd.value = !id
  saveObject.value = { statRangeType: 'year' }
  recordId.value = id || null
  infoFormModel.value?.resetFields?.()
  visible.value = true

  if (!isAdd.value && recordId.value) {
    try {
      const res = await teamApi.getById(recordId.value)
      saveObject.value = res || { statRangeType: 'year' }
    } catch (_e) {
      message.error('加载团队信息失败，请重试')
    }
  }
}

function validateForm() {
  return new Promise((resolve) => {
    infoFormModel.value?.validate((valid) => {
      resolve(valid)
    })
  })
}

async function onSubmit() {
  if (btnLoading.value) return

  const valid = await validateForm()
  if (!valid) return

  btnLoading.value = true
  try {
    if (isAdd.value) {
      await teamApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await teamApi.updateById(recordId.value, saveObject.value)
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

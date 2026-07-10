<template>
  <ag-drawer
    v-model:open="localOpen"
    :title="isAdd ? '新增团队' : '修改团队'"
    class="drawer-width"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    :width="drawerWidth"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
    @close="handleClose"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="10">
          <a-form-item label="团队名称" name="teamName">
            <a-input v-model:value="saveObject.teamName" placeholder="请输入团队名称" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="团队编号" name="teamNo">
            <a-input v-model:value="saveObject.teamNo" placeholder="请输入团队编号" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="统计周期" name="statRangeType">
            <a-select v-model:value="saveObject.statRangeType" placeholder="统计周期" default-value="year">
              <a-select-option value="year">年</a-select-option>
              <a-select-option value="quarter">季度</a-select-option>
              <a-select-option value="month">月</a-select-option>
              <a-select-option value="week">周</a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>

<script setup>
/**
 * 用户团队新增/编辑弹窗组件
 * 功能：支持新增和编辑团队信息
 */
import { AgDrawer } from '@/components'
import { teamApi } from '@/api/business/sys-user-team/team-api'
import { message } from 'ant-design-vue'
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'

const props = defineProps({
  open: { type: Boolean, default: false },
  recordId: { type: String, default: '' }
})

const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const loading = ref(false)
const localOpen = ref(false)
const isAdd = ref(true)
const saveObject = ref({})
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

watch(() => props.open, async (val) => {
  localOpen.value = val
  if (val) {
    await initForm()
  }
})

watch(localOpen, (val) => {
  emit('update:open', val)
})

const initForm = async () => {
  isAdd.value = !props.recordId
  saveObject.value = { statRangeType: 'year' }
  infoForm.value?.resetFields?.()

  if (!isAdd.value && props.recordId) {
    try {
      const res = await teamApi.getById(props.recordId)
      saveObject.value = res || { statRangeType: 'year' }
    } catch (_e) {
      message.error('加载团队信息失败，请重试')
    }
  }
}

const validateForm = async () => {
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

const handleConfirm = async () => {
  if (loading.value) return

  const valid = await validateForm()
  if (!valid) return

  loading.value = true
  try {
    if (isAdd.value) {
      await teamApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await teamApi.updateById(props.recordId, saveObject.value)
      message.success('修改成功')
    }
    localOpen.value = false
    emit('success')
  } finally {
    loading.value = false
  }
}

const handleClose = () => {
  localOpen.value = false
}
</script>

<style lang="less">
.upload-list-inline .ant-btn {
  height: 66px;
}
</style>

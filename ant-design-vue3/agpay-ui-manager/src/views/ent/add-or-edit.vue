<template>
  <a-modal
    :open="localOpen"
    :title="isAdd ? '新增菜单' : '修改菜单'"
    :confirm-loading="confirmLoading"
    @ok="handleOkFunc"
    @cancel="handleCancel"
    @update:open="handleUpdateOpen"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 15 }"
      :rules="rules"
    >
      <a-form-item label="资源名称：" name="entName">
        <a-input v-model:value="saveObject.entName" />
      </a-form-item>

      <a-form-item label="路径地址：" name="menuUri">
        <a-input v-model:value="saveObject.menuUri" />
      </a-form-item>

      <a-form-item label="排序（正序显示）：" name="entSort">
        <a-input-number v-model:value="saveObject.entSort" />
      </a-form-item>

      <a-form-item label="快速开始：" name="quickJump">
        <a-radio-group v-model:value="saveObject.quickJump" :disabled="saveObject.menuType == 'PB' || !saveObject.menuUri">
          <a-radio :value="1">是</a-radio>
          <a-radio :value="0">否</a-radio>
        </a-radio-group>
      </a-form-item>

      <a-form-item label="状态：" name="state">
        <a-radio-group v-model:value="saveObject.state">
          <a-radio :value="1">启用</a-radio>
          <a-radio :value="0">停用</a-radio>
        </a-radio-group>
      </a-form-item>
      <div>
        <!-- 匹配规则板块 -->
        <a-row justify="space-between" type="flex">
          <a-col :span="24">
            <a-divider orientation="left">
              <a-tag color="#FF4B33"> 匹配规则 </a-tag>
            </a-divider>
          </a-col>
        </a-row>

        <a-form-item v-if="sysType !== 'MCH'" label="" name="matchRule.epUserEnt">
          <a-checkbox v-model:checked="saveObject.matchRule.epUserEnt">拓展员权限</a-checkbox>
        </a-form-item>
        <a-form-item v-if="sysType === 'MCH'" label="" name="matchRule.userEntRules">
          <a-checkbox-group v-model:value="saveObject.matchRule.userEntRules">
            <a-checkbox value="USER_TYPE_11_INIT">店长默认权限</a-checkbox>
            <a-checkbox value="USER_TYPE_12_INIT">店员默认权限</a-checkbox>
            <a-checkbox value="STORE">门店管理权限</a-checkbox>
            <a-checkbox value="QUICK_PAY">快捷收银权限</a-checkbox>
            <a-checkbox value="REFUND">退款权限</a-checkbox>
            <a-checkbox value="DEVICE">设备管理权限</a-checkbox>
            <a-checkbox value="STATS">统计报表权限</a-checkbox>
          </a-checkbox-group>
        </a-form-item>
        <a-form-item v-if="sysType === 'MCH'" name="matchRule.mchType">
          <a-checkbox :checked="saveObject.matchRule.mchType === 1" @change="onMchTypeChange(1)"
            >普通商户特有权限</a-checkbox
          >
          <a-checkbox :checked="saveObject.matchRule.mchType === 2" @change="onMchTypeChange(2)"
            >特约商户(服务商模式)特有权限</a-checkbox
          >
        </a-form-item>
        <a-form-item v-if="sysType === 'MCH'" name="matchRule.mchLevelArray">
          <a-checkbox-group v-model:value="saveObject.matchRule.mchLevelArray">
            <a-checkbox value="M0">M0商户特有权限</a-checkbox>
            <a-checkbox value="M1">M1商户特有权限</a-checkbox>
          </a-checkbox-group>
        </a-form-item>
      </div>
    </a-form>
  </a-modal>
</template>

<script setup>
/**
 * 资源权限编辑弹窗组件
 * 功能：新增/编辑资源权限信息，配置匹配规则
 */
import { entApi } from '@/api/business/ent/ent-api'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

/**
 * 组件属性
 */
const props = defineProps({
  open: { type: Boolean, default: false },
  recordId: { type: String, default: '' },
  sysType: { type: String, default: 'MGR' }
})

/**
 * 组件事件
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 表单引用
 */
const infoForm = ref(null)

/**
 * 确认加载状态
 */
const confirmLoading = ref(false)

/**
 * 是否新增
 */
const isAdd = ref(true)

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 当前记录ID
 */
const currentRecordId = ref('')

/**
 * 当前系统类型
 */
const currentSysType = ref('MGR')

/**
 * 表单验证规则
 */
const rules = {
  entName: [{ required: true, message: '请输入资源名称', trigger: 'blur' }]
}

/**
 * 创建默认保存对象
 * @returns {Object} 默认保存对象
 */
const createDefaultSaveObject = () => ({
  matchRule: {
    epUserEnt: null,
    userEntRules: null,
    mchType: null,
    mchLevelArray: null
  }
})

/**
 * 保存对象
 */
const saveObject = ref(createDefaultSaveObject())

/**
 * 监听open变化
 */
watch(() => props.open, async (newVal) => {
  localOpen.value = newVal
  if (newVal) {
    await initForm()
  }
})

/**
 * 处理open更新事件
 */
const handleUpdateOpen = (val) => {
  localOpen.value = val
  emit('update:open', val)
}

/**
 * 初始化表单
 */
const initForm = async () => {
  isAdd.value = !props.recordId
  currentRecordId.value = props.recordId || ''
  currentSysType.value = props.sysType || 'MGR'
  saveObject.value = createDefaultSaveObject()
  confirmLoading.value = false

  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value) {
    const res = await entApi.getBySysType(currentRecordId.value, currentSysType.value)
    const current = res || {}
    if (!current.matchRule) {
      current.matchRule = createDefaultSaveObject().matchRule
    }
    saveObject.value = current
  }
}

/**
 * 拓展员权限变更处理
 * @param {Event} e - 事件对象
 */
const onEpUserEntChange = (e) => {
  saveObject.value.matchRule.epUserEnt = e.target.checked ? true : null
}

/**
 * 商户类型变更处理
 * @param {number} value - 商户类型值
 */
const onMchTypeChange = (value) => {
  if (saveObject.value.matchRule.mchType === value) {
    saveObject.value.matchRule.mchType = null
  } else {
    saveObject.value.matchRule.mchType = value
  }
}

/**
 * 处理确认操作
 */
const handleOkFunc = async () => {
  try {
    await infoForm.value.validate()
  } catch {
    return
  }

  confirmLoading.value = true
  try {
    if (isAdd.value) {
      await entApi.add(saveObject.value)
      message.success('新增成功')
    } else {
      await entApi.updateById(currentRecordId.value, saveObject.value)
      message.success('修改成功')
    }
    emit('update:open', false)
    emit('success')
  } finally {
    confirmLoading.value = false
  }
}

/**
 * 处理取消操作
 */
const handleCancel = () => {
  emit('update:open', false)
}
</script>

<style scoped>
::v-deep(.ant-checkbox-wrapper + .ant-checkbox-wrapper) {
  margin-left: 0px;
}
</style>

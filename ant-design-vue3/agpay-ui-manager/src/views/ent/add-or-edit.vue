<template>
  <a-modal
    v-model="isShow"
    :title="isAdd ? '新增菜单' : '修改菜单'"
    :confirm-loading="confirmLoading"
    @ok="handleOkFunc"
  >
    <a-form-model
      ref="infoFormModel"
      :model="saveObject"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 15 }"
      :rules="rules"
    >
      <a-form-model-item label="资源名称：" prop="entName">
        <a-input v-model="saveObject.entName" />
      </a-form-model-item>

      <a-form-model-item label="路径地址：" prop="menuUri">
        <a-input v-model="saveObject.menuUri" />
      </a-form-model-item>

      <a-form-model-item label="排序（正序显示）：" prop="entSort">
        <a-input v-model="saveObject.entSort" />
      </a-form-model-item>

      <a-form-model-item label="快速开始：" prop="quickJump">
        <a-radio-group v-model="saveObject.quickJump" :disabled="saveObject.menuType == 'PB' || !saveObject.menuUri">
          <a-radio :value="1">是</a-radio>
          <a-radio :value="0">否</a-radio>
        </a-radio-group>
      </a-form-model-item>

      <a-form-model-item label="状态：" prop="state">
        <a-radio-group v-model="saveObject.state">
          <a-radio :value="1">启用</a-radio>
          <a-radio :value="0">停用</a-radio>
        </a-radio-group>
      </a-form-model-item>
      <div>
        <!-- 匹配规则板块 -->
        <a-row justify="space-between" type="flex">
          <a-col :span="24">
            <a-divider orientation="left">
              <a-tag color="#FF4B33"> 匹配规则 </a-tag>
            </a-divider>
          </a-col>
        </a-row>

        <a-form-model-item v-if="sysType !== 'MCH'" label="" prop="matchRule.epUserEnt">
          <a-checkbox :checked="saveObject.matchRule.epUserEnt" @change="onEpUserEntChange">拓展员权限</a-checkbox>
        </a-form-model-item>
        <a-form-model-item v-if="sysType === 'MCH'" label="" prop="matchRule.userEntRules">
          <a-checkbox-group v-model="saveObject.matchRule.userEntRules">
            <a-checkbox value="USER_TYPE_11_INIT">店长默认权限</a-checkbox>
            <a-checkbox value="USER_TYPE_12_INIT">店员默认权限</a-checkbox>
            <a-checkbox value="STORE">门店管理权限</a-checkbox>
            <a-checkbox value="QUICK_PAY">快捷收银权限</a-checkbox>
            <a-checkbox value="REFUND">退款权限</a-checkbox>
            <a-checkbox value="DEVICE">设备管理权限</a-checkbox>
            <a-checkbox value="STATS">统计报表权限</a-checkbox>
          </a-checkbox-group>
        </a-form-model-item>
        <a-form-model-item v-if="sysType === 'MCH'" prop="matchRule.mchType">
          <a-checkbox :checked="saveObject.matchRule.mchType === 1" @change="onMchTypeChange(1)"
            >普通商户特有权限</a-checkbox
          >
          <a-checkbox :checked="saveObject.matchRule.mchType === 2" @change="onMchTypeChange(2)"
            >特约商户(服务商模式)特有权限</a-checkbox
          >
        </a-form-model-item>
        <a-form-model-item v-if="sysType === 'MCH'" prop="matchRule.mchLevelArray">
          <a-checkbox-group v-model="saveObject.matchRule.mchLevelArray">
            <a-checkbox value="M0">M0商户特有权限</a-checkbox>
            <a-checkbox value="M1">M1商户特有权限</a-checkbox>
          </a-checkbox-group>
        </a-form-model-item>
      </div>
    </a-form-model>
  </a-modal>
</template>

<script setup>
import { entApi } from '@/api/business/ent/ent-api'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoFormModel = ref()
const confirmLoading = ref(false)
const isAdd = ref(true)
const isShow = ref(false)
const recordId = ref(null)
const sysType = ref('MGR')
const rules = {
  entName: [{ required: true, message: '请输入资源名称', trigger: 'blur' }]
}

const createDefaultSaveObject = () => ({
  matchRule: {
    epUserEnt: null,
    userEntRules: null,
    mchType: null,
    mchLevelArray: null
  }
})

const saveObject = ref(createDefaultSaveObject())

const show = (id, currentSysType) => {
  isAdd.value = !id
  sysType.value = currentSysType
  saveObject.value = createDefaultSaveObject()
  confirmLoading.value = false

  if (infoFormModel.value !== undefined) {
    infoFormModel.value.resetFields()
  }

  if (!isAdd.value) {
    recordId.value = id
    entApi.getBySysType(id, currentSysType).then((res) => {
      const current = res || {}
      if (!current.matchRule) {
        current.matchRule = createDefaultSaveObject().matchRule
      }
      saveObject.value = current
    })
    isShow.value = true
  } else {
    isShow.value = true
  }
}

const onEpUserEntChange = (e) => {
  if (e.target.checked) {
    saveObject.value.matchRule.epUserEnt = true
  } else {
    saveObject.value.matchRule.epUserEnt = null
  }
}

const onMchTypeChange = (value) => {
  if (saveObject.value.matchRule.mchType === value) {
    saveObject.value.matchRule.mchType = null
  } else {
    saveObject.value.matchRule.mchType = value
  }
}

const handleOkFunc = () => {
  infoFormModel.value.validate((valid) => {
    if (valid) {
      confirmLoading.value = true
      if (isAdd.value) {
        confirmLoading.value = false
      } else {
        entApi
          .updateById(recordId.value, saveObject.value)
          .then(() => {
            window.$message.success('修改成功')
            isShow.value = false
            props.callbackFunc()
          })
          .catch(() => {
            confirmLoading.value = false
          })
      }
    }
  })
}

defineExpose({ show })
</script>

<style scoped>
::v-deep(.ant-checkbox-wrapper + .ant-checkbox-wrapper) {
  margin-left: 0px;
}
</style>

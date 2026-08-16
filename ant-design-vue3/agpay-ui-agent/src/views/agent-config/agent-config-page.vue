<template>
  <div>
    <a-card :bordered="false">      
      <a-tabs v-model:active-key="groupKey" :animated="false">
        <a-tab-pane key="1" tab="安全管理">
          <div v-if="groupKey === '1'">
            <a-row :gutter="16">
              <a-col :md="16" :lg="16">
                <a-form ref="pwdFormRef" :model="updateObject" :label-col="{ span: 9 }" :wrapper-col="{ span: 10 }" :rules="rulesPass">
                  <a-form-item label="原支付密码" prop="originalPwd" v-if="hasSipwValidate">
                    <a-input-password :maxlength="6" v-model:value="updateObject.originalPwd" placeholder="请输入原支付密码" />
                  </a-form-item>
                  <a-form-item label="新支付密码" prop="newPwd">
                    <a-input-password :maxlength="6" v-model:value="updateObject.newPwd" placeholder="请输入新支付密码" />
                  </a-form-item>
                  <a-form-item label="确认新支付密码" prop="confirmPwd">
                    <a-input-password :maxlength="6" v-model:value="updateObject.confirmPwd" placeholder="确认新支付密码" />
                  </a-form-item>
                </a-form>
                <a-form-item style="display: flex; justify-content: center">
                  <a-button type="primary" @click="setSipw" :loading="btnLoading">确认更改</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </div>
        </a-tab-pane>
      </a-tabs>
    </a-card>
  </div>
</template>

<script setup>
import { agentConfigApi } from '@/api/business/agent-config/agent-config-api'
import { infoBox } from '@/utils/info-box'
import { Base64 } from 'js-base64'
import { onMounted, reactive, ref } from 'vue'

const btnLoading = ref(false)
const groupKey = ref('1')
const hasSipwValidate = ref(false)
const pwdFormRef = ref(null)

const updateObject = reactive({
  originalPwd: '',
  newPwd: '',
  confirmPwd: ''
})

const rulesPass = reactive({
  originalPwd: [
    { min: 6, max: 6, required: true, message: '请输入原支付密码(6位数字格式)', trigger: 'blur' },
    { pattern: /^\d{6}$/, message: '请输入原支付密码(6位数字格式)', trigger: 'blur' }
  ],
  newPwd: [
    { min: 6, max: 6, required: true, message: '请输入新支付密码(6位数字格式)', trigger: 'blur' },
    { pattern: /^\d{6}$/, message: '请输入新支付密码(6位数字格式)', trigger: 'blur' }
  ],
  confirmPwd: [
    { min: 6, max: 6, required: true, message: '请输入确认新支付密码', trigger: 'blur' },
    {
      validator: (rule, value) => {
        if (value === updateObject.newPwd) return Promise.resolve()
        return Promise.reject(new Error('新密码与确认密码不一致'))
      },
      trigger: 'blur'
    }
  ]
})

const setHasSipwValidate = async () => {
  try {
    const res = await agentConfigApi.getConfig('hasSipwValidate')
    hasSipwValidate.value = !!res
  } catch (err) {
    console.error(err)
  }
}

const setSipw = () => {
  pwdFormRef.value?.validate().then(() => {
    infoBox.confirmPrimary('确认更新支付密码吗？', '', async () => {
      btnLoading.value = true
      try {
        const originalPwd = Base64.encode(updateObject.originalPwd)
        const confirmPwd = Base64.encode(updateObject.confirmPwd)
        await agentConfigApi.updateConfig('agentSipw', { originalPwd, confirmPwd })
        infoBox.modalWarning('提示', '更新成功！')
        updateObject.originalPwd = ''
        updateObject.newPwd = ''
        updateObject.confirmPwd = ''
        await setHasSipwValidate()
      } catch (err) {
        console.error(err)
      } finally {
        btnLoading.value = false
      }
    })
  }).catch(() => {})
}

onMounted(() => {
  setHasSipwValidate()
})
</script>

<style lang="less" scoped>
</style>

<template>
  <div style="background: #fff;border-radius:10px">
    <a-tabs v-model="groupKey" @change="selectTabs" :animated="false">
      <a-tab-pane key="1" tab="安全管理">
        <div class="account-settings-info-view">
          <a-row :gutter="16">
            <a-col :md="16" :lg="16">
              <a-form-model ref="pwdFormModel" :model="updateObject" :label-col="{span: 9}" :wrapper-col="{span: 10}" :rules="rulesPass">
                <a-form-model-item label="原支付密码：" prop="originalPwd" v-if="hasSipwValidate">
                  <a-input-password :maxlength="6" v-model="updateObject.originalPwd" placeholder="请输入原支付密码" />
                </a-form-model-item>
                <a-form-model-item label="新支付密码：" prop="newPwd">
                  <a-input-password :maxlength="6" v-model="updateObject.newPwd" placeholder="请输入新支付密码"/>
                </a-form-model-item>
                <a-form-model-item label="确认新支付密码：" prop="confirmPwd">
                  <a-input-password :maxlength="6" v-model="updateObject.confirmPwd" placeholder="确认新支付密码"/>
                </a-form-model-item>
              </a-form-model>
              <a-form-item style="display:flex;justify-content:center">
                <a-button type="primary" icon="safety-certificate" @click="setSipw($event, '提示', '更新成功！')" :loading="btnLoading">确认更改</a-button>
              </a-form-item>
            </a-col>
          </a-row>
        </div>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>

<script>
import { API_URL_AGENT_CONFIG, req, getAgentConfigs } from '@/api/manage'
import { Base64 } from 'js-base64'
export default {
  components: {},
  data () {
    return {
      btnLoading: false,
      groupKey: '1',
      hasSipwValidate: false,
      updateObject: {
        originalPwd: '', // 原密码
        newPwd: '', //  新密码
        confirmPwd: '' //  确认密码
      },
      rulesPass: {
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
            validator: (rule, value, callBack) => {
              this.updateObject.newPwd === value ? callBack() : callBack('新密码与确认密码不一致')
            }
          }
        ]
      }
    }
  },
  created () {
    this.setHasSipwValidate()
  },
  methods: {
    setHasSipwValidate () {
      getAgentConfigs('hasSipwValidate').then(res => {
        this.hasSipwValidate = res
      })
    },
    setSipw (e, title, content) {
      const that = this
      this.$refs.pwdFormModel.validate(valid => {
        if (valid) { // 验证通过
          this.$infoBox.confirmPrimary('确认更新支付密码吗？', '', () => {
            // 请求接口
            that.btnLoading = true // 打开按钮上的 loading
            const originalPwd = Base64.encode(that.updateObject.originalPwd)
            const confirmPwd = Base64.encode(that.updateObject.confirmPwd)
            req.updateById(API_URL_AGENT_CONFIG, 'agentSipw', { originalPwd, confirmPwd }).then(res => {
              that.$infoBox.modalWarning(title, content)
              that.btnLoading = false
              that.setHasSipwValidate()
            }).catch(res => {
              that.btnLoading = false
            })
          })
        }
      })
    }
  }
}
</script>
<style lang="less">
</style>

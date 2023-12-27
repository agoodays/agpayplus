<template>
  <div class="main">
    <a-form-model class="user-layout-login" ref="infoFormModel" :model="saveObject" :rules="rules">
      <!-- 错误提示信息 -->
      <a-alert v-if="showForgetErrorInfo" type="error" showIcon style="margin-bottom: 24px;" :message="showForgetErrorInfo" />

      <a-form-model-item prop="phone">
        <a-input size="large" type="text" placeholder="请输入手机号" v-model="saveObject.phone"/>
      </a-form-model-item>
      <div class="code">
        <a-form-model-item prop="code">
          <a-input class="code-input" size="large" type="text" placeholder="请输入验证码" v-model="saveObject.code"/>
        </a-form-model-item>
        <div style="position: relative;">
          <a-button
            size="large"
            type="primary"
            class="login-button"
            @click="sendCode()"
            :disabled="this.codeExpireTime > 0"
          >
            {{ this.codeExpireTime > 0 ? `${this.codeExpireTime}秒后重新发送` : '发送短信验证码' }}
          </a-button>
        </div>
      </div>

      <a-form-model-item prop="password">
        <a-input-password size="large" placeholder="请输入新密码" v-model="saveObject.password"/>
      </a-form-model-item>

      <a-form-model-item prop="confirmPwd">
        <a-input-password size="large" placeholder="请输入确认新密码" v-model="saveObject.confirmPwd"/>
      </a-form-model-item>

      <a-form-model-item>
        <a class="forge-password" style="float: right;" href="/login" >去登录 >></a>
      </a-form-model-item>

      <a-form-model-item class="submit">
        <a-button
          size="large"
          type="primary"
          class="login-button"
          @click="handleSubmit"
          :loading="forgetBtnLoadingFlag"
        >找回密码</a-button>
      </a-form-model-item>
    </a-form-model>
  </div>
</template>

<script>
// import Initializer from '@/core/bootstrap'
import { mapActions } from 'vuex'
import { message } from 'ant-design-vue'
import { timeFix } from '@/utils/util'
import { sendcode, forget } from '@/api/login'

export default {
  components: {
  },
  data () {
    return {
      forgetBtnLoadingFlag: false, // 登录按钮是否显示 加载状态
      showForgetErrorInfo: '', // 是否显示登录错误面板信息
      codeExpireTime: 0,
      saveObject: {}, // 数据对象
      rules: {
        phone: [{ required: true, pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }],
        code: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
        password: [{ required: false, trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            if (!this.saveObject.password.length) {
              callBack('请输入新密码')
            }
            if (this.saveObject.password.length < 6 || this.saveObject.password.length > 12) {
              callBack('密码不符合规则，请输入6-12位密码')
            }
            // if (this.saveObject.password !== this.saveObject.confirmPwd) {
            //   callBack('两次输入密码不一致')
            // }
            callBack()
          }
        }], // 新密码
        confirmPwd: [{ required: false, trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            if (!this.saveObject.confirmPwd.length) {
              callBack('请输入确认新密码')
            }
            if (this.saveObject.password !== this.saveObject.confirmPwd) {
              callBack('两次输入密码不一致')
            }
            callBack()
          }
        }] // 确认新密码
      }
    }
  },
  mounted () {
  },
  methods: {
    ...mapActions(['Login', 'Logout']),
    // handler
    handleSubmit (e) {
      e.preventDefault() // 通知 Web 浏览器不要执行与事件关联的默认动作
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          console.log(that.saveObject)
          const forgetParams = {
            phone: that.saveObject.phone,
            code: that.saveObject.code,
            confirmPwd: that.saveObject.confirmPwd
          }
          forget(forgetParams).then((res) => {
            this.retrieveSuccess(res)
          }).catch(err => {
            that.showForgetErrorInfo = (err.msg || JSON.stringify(err))
            that.forgetBtnLoadingFlag = false
          })
        }
      })
    },
    retrieveSuccess (res) {
      this.$router.push({ path: '/' })
      // 延迟 1 秒显示欢迎信息
      setTimeout(() => {
        this.$notification.success({
          message: '欢迎',
          description: `${timeFix()}，欢迎回来`
        })
      }, 1000)
      this.showForgetErrorInfo = ''
    },
    sendCode () { // 发送验证码
      const that = this
      if (!that.saveObject.phone.length) {
        message.error('请输入手机号！')
        return false
      }
      const phoneReg = /^1[3-9]\d{9}$/
      if (!phoneReg.test(that.saveObject.phone)) {
        message.error('请输入正确的手机号！')
        return false
      }
      // 获取图形验证码
      sendcode({ phone: that.saveObject.phone, smsType: 'retrieve' }).then(res => {
        that.codeExpireTime = 60
        if (this.timer) clearInterval(this.timer) // 如果多次点击则清除已有的定时器
        // 超过60秒提示过期刷新
        this.timer = setInterval(() => {
          that.codeExpireTime--
          if (that.codeExpireTime <= 0) {
            clearInterval(this.timer)
          }
        }, 1000)
      })
    }
  }
}
</script>

<style lang="less" scoped>
.user-layout-content .main .desc {
  margin-bottom: 50px;
}
.user-layout-login {
  label {
    font-size: 14px;
  }

  .forge-password {
    font-size: 14px;
    color: @ag-theme;
  }

  button.login-button {
    padding: 0 15px;
    font-size: 16px;
    height: 40px;
    width: 100%;
  }

  .user-login-other {
    text-align: left;
    margin-top: 24px;
    line-height: 22px;

    .item-icon {
      font-size: 24px;
      color: rgba(0, 0, 0, 0.2);
      margin-left: 16px;
      vertical-align: middle;
      cursor: pointer;
      transition: color 0.3s;

      &:hover {
        color: #1890ff;
      }
    }

    .register {
      float: right;
    }
  }
  .code {
    display: flex;
    justify-content: space-between;
    .code-input {
      width: 216px;
    }
    .code-img {
      width: 137px;
      height: 40px;
      background-color: #ddd;
      img{
        width: 137px;
        height: 40px;
      }
    }
  }
  .submit {
    margin-top: 50px;
  }
}
.vercode-mask {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background: #000;
  opacity: 0.8;
  text-align:center;
  line-height: 40px;
  color:#fff;
  &:hover {
    cursor: pointer;
  }
}
</style>

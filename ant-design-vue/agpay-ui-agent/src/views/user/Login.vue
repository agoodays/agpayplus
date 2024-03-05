<template>
  <div class="main">
    <div class="tab-box">
      <div class="desc">代理商登录</div>
      <div class="tab">
        <a class="operation-class" v-if="loginMethod !== 'password'" @click="loginMethod = 'password'">密码登录</a>
        <span style="margin: 0px 10px;" v-if="loginMethod=== 'qrcode'">|</span>
        <a class="operation-class" v-if="loginMethod !== 'message'" @click="loginMethod = 'message'">短信登录</a>
        <span style="margin: 0px 10px;" v-if="loginMethod !== 'qrcode'">|</span>
        <a class="operation-class" v-if="loginMethod !== 'qrcode'" @click="loginMethod = 'qrcode'">扫码登录</a>
      </div>
    </div>
    <div v-if="loginMethod === 'qrcode'" class="qr-wrapper">
      <vue-qr text="AGPAY_LOGIN_QR_75c86a33f88b472e8bda6b8e783fba1f" :size="200" margin="0"></vue-qr>
      <div class="qr-tips">二维码过期请刷新
        <div class="tips-img">
          <img src="@/assets/svg/refresh.svg">
        </div>
      </div>
      <div class="qr-mantle"></div>
    </div>
    <div v-if="loginMethod === 'qrcode'" class="qr-footer">请使用展业宝APP扫码登录</div>
    <a-form v-if="loginMethod !== 'qrcode'" class="user-layout-login" ref="formLogin" :form="form" @submit="handleSubmit">
      <!-- 错误提示信息 -->
      <a-alert v-if="showLoginErrorInfo" type="error" showIcon style="margin-bottom: 24px;" :message="showLoginErrorInfo" />
      <a-form-item v-if="loginMethod === 'password'">
        <a-input
          @focus="usernameIcon = require('@/assets/svg/select-user.svg')"
          @blur="usernameIcon = require('@/assets/svg/user.svg')"
          size="large"
          type="text"
          placeholder="登录名/手机号"
          v-decorator="[
            'username',
            {rules: [{ required: true, message: '请输入登录名/手机号' }], validateTrigger: 'change'}
          ]"
        >
          <img :src="usernameIcon" slot="prefix" class="user" alt="user" >
        </a-input>
      </a-form-item>
      <a-form-item v-if="loginMethod === 'password'">
        <a-input-password
          @focus="passwordIcon = require('@/assets/svg/select-lock.svg')"
          @blur="passwordIcon = require('@/assets/svg/lock.svg')"
          size="large"
          placeholder="密码"
          v-decorator="[
            'password',
            {rules: [{ required: true, message: '请输入密码' }], validateTrigger: 'change'}
          ]"
        >
          <!-- <a-icon src="../../assets/svg/user.svg" alt=""> -->
          <img :src="passwordIcon" slot="prefix" class="user" alt="user">
        </a-input-password>
      </a-form-item>

      <a-form-item v-if="loginMethod === 'message'">
        <a-input
          size="large"
          type="mobile"
          placeholder="手机号"
          v-decorator="[
            'mobile',
            {rules: [{ required: true, pattern: /^1[34578]\d{9}$/, message: '请输入正确的手机号！' }], validateTrigger: 'blur'}
          ]"/>
      </a-form-item>

      <div class="code-body">
        <div class="code-layout">
          <div class="code code-layout-item">
            <a-form-item>
              <a-input
                @focus="vercodeIcon = require('@/assets/svg/select-code.svg')"
                @blur="vercodeIcon = require('@/assets/svg/code.svg')"
                class="code-input"
                size="large"
                :placeholder="(loginMethod === 'password'?'图形':'')+'验证码'"
                v-decorator="[
                  'usercode',
                  {rules: [{ required: true, message: '请输入验证码' }], validateTrigger: 'blur'}
                ]"
              >
                <img v-if="loginMethod === 'password'" :src="vercodeIcon" slot="prefix" class="user" alt="user" />
              </a-input>
            </a-form-item>
            <div v-if="loginMethod === 'password'" class="code-img" style="position: relative;background:#ddd">
              <img v-show="vercodeImgSrc" :src="vercodeImgSrc" @click="refVercode()"/>
              <div class="vercode-mask" v-show="isOverdue" @click="refVercode()">已过期 请刷新</div>
            </div>
            <div v-if="loginMethod === 'message'" style="position: relative;">
              <a-button
                type="primary"
                @click="sendCode()"
                style="height: 40px; margin-left: 10px;"
                :disabled="this.codeExpireTime > 0"
              >
                {{ this.codeExpireTime > 0 ? `${this.codeExpireTime}秒后重新发送` : '发送短信验证码' }}
              </a-button>
            </div>
          </div>
        </div>
      </div>

      <a-form-item>
        <!-- 自动登录 -->
        <!-- <a-checkbox v-decorator="['rememberMe', { valuePropName: 'checked' }]">自动登录</a-checkbox> -->
        <a-checkbox v-model="isAutoLogin">自动登录</a-checkbox>
        <!-- 忘记密码 -->
        <a class="forge-password" style="float: right;" href="/forget">忘记密码?</a>
        <a class="forge-password" style="float: right;margin-right: 20px;" href="/register">注册</a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button
          size="large"
          type="primary"
          htmlType="submit"
          class="login-button"
          :loading="loginBtnLoadingFlag"
        >登录
        </a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script>
// import Initializer from '@/core/bootstrap'
import { mapActions } from 'vuex'
import vueQr from 'vue-qr'
import { timeFix } from '@/utils/util'
import { sendcode, vercode } from '@/api/login'

export default {
  components: {
    vueQr
  },
  data () {
    return {
      loginMethod: 'password',
      codeExpireTime: 0,
      isOverdue: false, // 设置过期样式
      isAutoLogin: true, // 是否是自动登录
      loginBtnLoadingFlag: false, // 登录按钮是否显示 加载状态
      showLoginErrorInfo: '', // 是否显示登录错误面板信息
      form: this.$form.createForm(this),
      usernameIcon: require('@/assets/svg/user.svg'), // 三个icon图标
      passwordIcon: require('@/assets/svg/lock.svg'),
      vercodeIcon: require('@/assets/svg/code.svg'),
      vercodeImgSrc: '', // 验证码图片
      vercodeToken: '' // 验证码验证token
    }
  },
  mounted () {
    this.refVercode()
  },
  methods: {
    ...mapActions(['Login', 'Logout']),
    // handler
    handleSubmit (e) {
      e.preventDefault() // 通知 Web 浏览器不要执行与事件关联的默认动作
      const that = this
      that.form.validateFields({ force: true }, (err, values) => {
        if (!err) {
          const loginParams = { ...values }
          loginParams.loginMethod = that.loginMethod
          loginParams.username = values.username
          loginParams.password = values.password
          loginParams.mobile = values.mobile
          loginParams.vercode = values.usercode
          loginParams.vercodeToken = that.vercodeToken
          that.loginBtnLoadingFlag = true // 登录按钮显示加载loading
          that.Login({ loginParams: loginParams, isSaveStorage: that.isAutoLogin }) // 打开自动登录将保存在localStorage中，否则保存在内存中。
            .then((res) => {
              this.loginSuccess(res)
            })
            .catch(err => {
              that.showLoginErrorInfo = (err.msg || JSON.stringify(err))
              that.loginBtnLoadingFlag = false
            })
        }
      })
    },
    loginSuccess (res) {
      const redirect = this.$route.query.redirect
      this.$router.push({ path: '/', query: { redirect: redirect } })
      // 延迟 1 秒显示欢迎信息
      setTimeout(() => {
        this.$notification.success({
          message: '欢迎',
          description: `${timeFix()}，欢迎回来${(res.lastLoginTime ? `\n上次登录时间：${res.lastLoginTime}` : '')}`,
          style: {
            whiteSpace: 'pre-wrap'
          }
        })
      }, 1000)
      this.showLoginErrorInfo = ''
    },
    sendCode () { // 发送验证码
      const { form: { validateFields } } = this
      const that = this
      validateFields(['mobile'], { force: true }, (err, values) => {
        if (!err) {
          // 获取图形验证码
          sendcode({ phone: values.mobile, smsType: 'auth' }).then(res => {
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
      })
    },
    refVercode () { // 刷新图片验证码
      const that = this
      // 获取图形验证码
      vercode().then(res => {
        that.vercodeImgSrc = res.imageBase64Data
        that.vercodeToken = res.vercodeToken

        this.isOverdue = false
        if (this.timer) clearInterval(this.timer) // 如果多次点击则清除已有的定时器
        // 超过60秒提示过期刷新
        this.timer = setInterval(() => {
          res.expireTime--
          if (res.expireTime <= 0) {
            that.isOverdue = true
            clearInterval(this.timer)
          }
        }, 1000)
      })
    }
  }
}
</script>

<style lang="less" scoped>
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
      // width: 216px;
    }
    .code-img {
      width: 120px;
      height: 40px;
      margin-left: 10px;
      background-color: #ddd;
      img{
        width: 120px;
        height: 40px;
      }
    }
  }
  .submit {
    margin-bottom: 0;
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

.qr-wrapper {
  width: 200px;
  height: 200px;
  position: relative;
  border-radius: 5px;
  overflow: hidden
}

.qr-wrapper .qr-tips {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%,-50%);
  z-index: 9999999999;
  font-size: 16px;
  white-space: nowrap;
  letter-spacing: 2px;
  padding: 10px 15px;
  border-radius: 5px;
  color: #fff
}

.qr-wrapper .qr-tips .tips-img {
  width: 30px;
  height: 30px;
  margin: 15px auto
}

.qr-wrapper .qr-tips .tips-img img {
  width: 100%;
  height: 100%
}

.qr-wrapper .qr-mantle {
  position: absolute;
  top: 0;
  left: 0;
  z-index: 999999;
  width: 100%;
  height: 100%;
  background-color: #000000b3
}

.qr-footer {
  margin: 15px 0;
  font-size: 16px;
  color: @ag-theme;
  text-align: center
}
</style>

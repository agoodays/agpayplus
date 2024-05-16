<script setup>
import { reactive, ref } from 'vue';

const loading = ref(false);
const codeExpireTime = ref(0);

const saveObject = reactive({
  phone: '',
  code: '',
  password: '',
  confirmPwd: ''
});

const passwordRules = {
  regexpRules: '',
  errTips: ''
};

const rules = {
  phone: [{ required: true, pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }],
  code: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
  password: [{ required: false, trigger: 'blur' }, {
    validator: (rule, value, callBack) => {
      if (!saveObject.password.length) {
        callBack('请输入新密码');
      }
      if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
        const regex = new RegExp(passwordRules.regexpRules);
        const isMatch = regex.test(saveObject.password);
        if (!isMatch) {
          callBack(passwordRules.errTips);
        }
      }
      // if (saveObject.password !== saveObject.confirmPwd) {
      //   callBack('两次输入密码不一致');
      // }
      callBack();
    }
  }], // 新密码
  confirmPwd: [{ required: false, trigger: 'blur' }, {
    validator: (rule, value, callBack) => {
      if (!saveObject.confirmPwd.length) {
        callBack('请输入确认新密码');
      }
      if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
        const regex = new RegExp(passwordRules.regexpRules);
        const isMatch = regex.test(saveObject.confirmPwd);
        if (!isMatch) {
          callBack(passwordRules.errTips);
        }
      }
      if (saveObject.password !== saveObject.confirmPwd) {
        callBack('两次输入密码不一致');
      }
      callBack();
    }
  }] // 确认新密码
};

async function sendCode() {

}

async function handleSubmit() {
  console.log(saveObject);
}

</script>

<template>
  <a-alert class="login-error-message" message="错误提示信息" type="error" show-icon />
  <div class="main">
    <div class="desc">找回密码</div>
    <a-form class="user-layout-login" ref="infoForm" :model="saveObject" :rules="rules">
      <a-form-item name="phone">
        <a-input size="large" type="text" placeholder="请输入手机号" v-model:value="saveObject.phone"/>
      </a-form-item>
      <div class="code-body">
        <div class="code-layout">
          <div class="code code-layout-item">
            <a-form-item name="code">
              <a-input class="code-input" size="large" type="text" placeholder="请输入验证码" v-model:value="saveObject.code"/>
            </a-form-item>
            <div style="position: relative;">
              <a-button
                  type="primary"
                  @click="sendCode()"
                  style="height: 40px; margin-left: 10px;"
                  :disabled="codeExpireTime > 0"
              >
                {{ codeExpireTime > 0 ? `${codeExpireTime}秒后重新发送` : '发送短信验证码' }}
              </a-button>
            </div>
          </div>
        </div>
      </div>
      <a-form-item name="password">
        <a-input-password size="large" placeholder="请输入新密码" v-model:value="saveObject.password"/>
      </a-form-item>
      <a-form-item name="confirmPwd">
        <a-input-password size="large" placeholder="请输入确认新密码" v-model:value="saveObject.confirmPwd"/>
      </a-form-item>
      <a-form-item>
        <a class="forge-password" style="float: right;" href="/login" >去登录 >></a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" class="login-button" @click="handleSubmit" :loading="loading">找回密码</a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<style lang="less" scoped>
.user-layout-login {
  label {
    font-size: 14px;
  }

  .forge-password {
    //font-size: 14px;
    color: @primary-color;
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
      //width: 216px;
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
</style>

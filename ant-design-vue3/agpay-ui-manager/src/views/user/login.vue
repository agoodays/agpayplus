<script setup>
import { reactive, ref, onMounted } from 'vue';

const loginForm = ref();
const loading = ref(false);
const isOverdue = ref(false);
const vercodeImgSrc = ref('');
const isAutoLogin = ref(false);
const loginErrorInfo = ref('');

const loginObject = reactive({
  loginMethod: 'password',
  username: '',
  password: '',
  vercode: '',
  vercodeToken: ''
});

const rules = {
  username: [{ required: true, message: '请输入登录名/手机号', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  vercode: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
};

async function refVercode() {
  console.log('Vercode');
}

async function handleSubmit() {
  loginForm.value.validate().then(async () => {
    console.log(loginObject);
  });
}

// 生命周期钩子
onMounted(() => {
  refVercode()
})
</script>

<template>
  <a-alert class="login-error-message" v-if="loginErrorInfo" :message="loginErrorInfo" type="error" show-icon />
  <div class="main">
    <div class="desc">运营平台登录</div>
    <a-form class="user-layout-login" ref="loginForm" :model="loginObject" :rules="rules" @submit="handleSubmit">
      <a-form-item name="username">
        <a-input size="large" type="text" placeholder="登录名/手机" v-model:value="loginObject.username"/>
      </a-form-item>
      <a-form-item name="password">
        <a-input-password size="large" placeholder="密码" v-model:value="loginObject.password"/>
      </a-form-item>
      <div class="code-body">
        <div class="code-layout">
          <div class="code code-layout-item">
            <a-form-item name="vercode">
              <a-input v-model:value="loginObject.vercode" class="code-input" size="large" type="text" placeholder="图形验证码"/>
            </a-form-item>
            <div class="code-img" style="position: relative;background:#ddd">
              <img v-show="vercodeImgSrc" :src="vercodeImgSrc" @click="refVercode()"/>
              <div class="vercode-mask" v-show="isOverdue" @click="refVercode()">已过期 请刷新</div>
            </div>
          </div>
        </div>
      </div>
      <a-form-item>
        <!-- 自动登录 -->
        <!-- <a-checkbox v-decorator="['rememberMe', { valuePropName: 'checked' }]">自动登录</a-checkbox> -->
        <a-checkbox v-model:value="isAutoLogin">自动登录</a-checkbox>
        <!-- 忘记密码 -->
        <a class="forget-password" style="float: right;" href="/forget">忘记密码?</a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" htmlType="submit" class="login-button" :loading="loading" >登录</a-button>
      </a-form-item>
    </a-form>
  </div>
  <div class="footer"></div>
</template>

<style lang="less" scoped>
.user-layout-login {
  label {
    font-size: 14px;
  }
  .forget-password {
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
</style>

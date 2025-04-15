<template>
  <a-alert class="login-error-message" v-if="loginErrorInfo" :message="loginErrorInfo" type="error" show-icon />
  <div class="main">
    <div class="desc">运营平台登录</div>
    <a-form class="user-layout-login" ref="loginForm" :model="loginObject" :rules="rules" @finish="onFinish" @finishFailed="onFinishFailed">
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
      <a-form-item name="isAutoLogin">
        <!-- 自动登录 -->
        <!-- <a-checkbox v-decorator="['rememberMe', { valuePropName: 'checked' }]">自动登录</a-checkbox> -->
        <a-checkbox v-model:checked="loginObject.isAutoLogin">自动登录</a-checkbox>
        <!-- 忘记密码 -->
        <a class="forget-password" style="float: right;" href="/forget">忘记密码?</a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" html-type="submit" class="login-button" :loading="loading" >登录</a-button>
      </a-form-item>
    </a-form>
  </div>
  <div class="footer"></div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { notification } from "ant-design-vue";
import { timeFix } from '/@/utils/time-util'
import { loginApi } from '/@/api/system/login-api';
import { ACCESS_TOKEN_NAME } from '/@/constants/system/token-const';
import { LOGIN_METHOD_ENUM } from '/@/constants/system/login-const.js';
import { useUserStore } from '/@/store/modules/system/user';

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

const loginForm = ref();
const loading = ref(false);
const isOverdue = ref(false);
const vercodeImgSrc = ref('');
const loginErrorInfo = ref('');

const loginObject = reactive({
  loginMethod: LOGIN_METHOD_ENUM.PASSWORD,
  username: '',
  password: '',
  vercode: '',
  vercodeToken: '',
  isAutoLogin: false
});

const rules = {
  username: [{ required: true, message: '请输入登录名/手机号', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  vercode: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
};

let timer = null;

// const refVercode = async () => {
//   let res = await loginApi.getVercode();
//   vercodeImgSrc.value = res.imageBase64Data;
//   loginObject.vercodeToken = res.vercodeToken;
//
//   isOverdue.value = false;
//   if (timer.value) clearInterval(timer.value); // 如果多次点击则清除已有的定时器
//   // 默认超过60秒提示过期刷新
//   timer.value = setInterval(() => {
//     res.expireTime--;
//     if (res.expireTime <= 0) {
//       isOverdue.value = true;
//       clearInterval(timer.value);
//     }
//   }, 1000);
// }

const refVercode = () => {
  loginApi.getVercode().then(res => {
    vercodeImgSrc.value = res.imageBase64Data;
    loginObject.vercodeToken = res.vercodeToken;

    isOverdue.value = false;
    if (timer) clearInterval(timer); // 如果多次点击则清除已有的定时器
    // 默认超过60秒提示过期刷新
    timer = setInterval(() => {
      res.expireTime--;
      if (res.expireTime <= 0) {
        isOverdue.value = true;
        clearInterval(timer);
      }
    }, 1000);
  }).catch(error => {
    // 处理 Promise 的错误情况
    console.error(error);
    // 可以在这里进行错误处理，例如显示错误信息给用户
  });
}

const onFinish = values => {
  loading.value = true // 登录按钮显示加载loading
  loginApi.login(values).then(res => {
    userStore.setToken(res[ACCESS_TOKEN_NAME], loginObject.isAutoLogin);
    loginSuccess(res);
  }).catch(error => {
    console.error(error);
    // 处理 Promise 的错误情况
    // 可以在这里进行错误处理，例如显示错误信息给用户
    loading.value = false;
    loginErrorInfo.value = (error.msg || JSON.stringify(error));
  });
};

const loginSuccess = res => {
  const redirect = route.query.redirect;
  console.log('loginSuccess', redirect);
  // 清除验证码
  router.push({ path: redirect || '/' });
  // 延迟 1 秒显示欢迎信息
  setTimeout(() => {
    notification.success({
      message: '欢迎',
      // description: `<p>${timeFix()}，欢迎回来</p>${(res.lastLoginTime ? `<p>上次登录时间：${res.lastLoginTime}</p>` : '')}`,
      description: `${timeFix()}，欢迎回来${(res.lastLoginTime ? `\n上次登录时间：${res.lastLoginTime}` : '')}`,
      style: {
        whiteSpace: 'pre-wrap'
      }
    });
  }, 1000);
  loginErrorInfo.value = '';
};

const onFinishFailed = errorInfo => {
  console.log('Failed:', errorInfo);
};

// 生命周期钩子
onMounted(() => {
  refVercode()
})
</script>

<style lang="less" scoped>
.user-layout-login {
  label {
    font-size: 14px;
  }
  .forget-password {
    //font-size: 14px;
    color: var(--ant-primary-color);
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

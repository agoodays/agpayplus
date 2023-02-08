// with polyfills
import 'core-js/stable'
import 'regenerator-runtime/runtime'

import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store/'
import ProLayout, { PageHeaderWrapper } from '@ant-design-vue/pro-layout'

import bootstrap from './core/bootstrap'
import './core/lazy_use' // use lazy load components
import './permission' // permission control  路由守卫
import './utils/filter' // global filter
import './global.less' // global style
import 'ant-design-vue/dist/antd.less'
import infoBox from '@/utils/infoBox'
import VueClipboard from 'vue-clipboard2' // 复制插件

Vue.config.productionTip = false

// mount axios to `Vue.$http` and `this.$http`
// use pro-layout components
Vue.component('pro-layout', ProLayout)
Vue.component('page-container', PageHeaderWrapper)
Vue.component('page-header-wrapper', PageHeaderWrapper)
Vue.use(VueClipboard) // 复制插件

/**
 * @description 全局注册权限验证
 */
Vue.prototype.$access = (entId) => {
  // eslint-disable-next-line eqeqeq
  return store.state.user.accessList.some(item => item == entId)
}

Vue.prototype.$infoBox = infoBox

window._AMapSecurityConfig = {
  // securityJsCode: '7fec782d86662766f46d8d92e4651154'
  securityJsCode: 'dccbb5a56d2a1850eda2b6e67f8f2f13'
}

new Vue({
  router,
  store,
  // init localstorage, vuex
  created: bootstrap,
  render: h => h(App)
}).$mount('#app')

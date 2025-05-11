import App from './App'

// #ifndef VUE3
import Vue from 'vue'
import CustomHeader from './components/CustomHeader.vue' // 路径根据实际项目结构调整
import './uni.promisify.adaptor'
Vue.component('CustomHeader', CustomHeader)
Vue.config.productionTip = false
App.mpType = 'app'
const app = new Vue({
	...App
})
app.$mount()
// #endif

// #ifdef VUE3
import {
	createSSRApp
} from 'vue'
import CustomHeader from './components/CustomHeader.vue'
export function createApp() {
	const app = createSSRApp(App)
	app.component('CustomHeader', CustomHeader) // 注册全局组件
	return {
		app
	}
}
// #endif
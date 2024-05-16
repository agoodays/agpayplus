import { createApp } from 'vue'
import Antd, { message } from 'ant-design-vue';
import './theme/index.less'
import App from './App.vue'
import { router } from '/@/router';
import { store } from '/@/store';

const app = createApp(App);

app.use(router).use(store).use(Antd)

app.mount('#app')

import { createApp } from 'vue'
import Antd, { message } from 'ant-design-vue';
import * as antIcons from '@ant-design/icons-vue';
import './theme/index.less'
import App from './App.vue'
import { router } from '/@/router';
import { store } from '/@/store';

const app = createApp(App);

app.use(router).use(store).use(Antd)

// 注册图标组件
Object.keys(antIcons).forEach((key) => {
    app.component(key, antIcons[key]);
});

app.mount('#app')

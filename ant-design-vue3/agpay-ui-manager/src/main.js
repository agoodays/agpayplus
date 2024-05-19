import { createApp } from 'vue'
import Antd, { message } from 'ant-design-vue';
import * as antIcons from '@ant-design/icons-vue';
import './theme/index.less'
import App from './App.vue'
import { router } from '/@/router';
import { store } from '/@/store';
import { localSave } from '/@/utils/local-util';
import { useAppConfigStore } from '/@/store/modules/system/app-config';
import localStorageKeyConst from '/@/constants/local-storage-key-const';

const theme = 'default'

// 动态加载适当的样式文件
if (theme === 'blue') {
    import('./theme/style-blue.less');
} else if (theme === 'red') {
    import('./theme/style-red.less');
}else {
    import('./theme/style-default.less');
}

const app = createApp(App);

app.use(router).use(store).use(Antd)

const appConfigStore = useAppConfigStore();
appConfigStore.theme = theme

localSave(localStorageKeyConst.APP_CONFIG, JSON.stringify({theme}));

// 注册图标组件
Object.keys(antIcons).forEach((key) => {
    app.component(key, antIcons[key]);
});

app.mount('#app')

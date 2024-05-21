import { createApp } from 'vue'
import Antd, { message } from 'ant-design-vue';
import * as antIcons from '@ant-design/icons-vue';
import './theme/index.less'
import App from './App.vue'
import { router } from '/@/router';
import { store } from '/@/store'
import('./theme/style-blue.less');
// import { basicApi } from '/@/api/system/basic-api';
// import { localSave } from '/@/utils/local-util';
// import localStorageKeyConst from '/@/constants/local-storage-key-const';

// basicApi.getSiteInfos().then((res) => {
//     console.log(res);
//     localSave(localStorageKeyConst.APP_CONFIG, JSON.stringify(res));
//
//     const theme = 'default'
//
//     // 动态加载适当的样式文件
//     if (theme === 'blue') {
//         import('./theme/style-blue.less');
//     } else if (theme === 'red') {
//         import('./theme/style-red.less');
//     }else {
//         import('./theme/style-default.less');
//     }
// })

const app = createApp(App);

app.use(router).use(store).use(Antd)

// 注册图标组件
Object.keys(antIcons).forEach((key) => {
    app.component(key, antIcons[key]);
});

app.mount('#app')

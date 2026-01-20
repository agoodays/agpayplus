import { createApp } from 'vue'
import Antd from 'ant-design-vue';
import * as antIcons from '@ant-design/icons-vue';
import './theme/index.less'
import App from './App.vue'
import { router } from '/@/router';
import { store } from '/@/store';
import Initializer from './bootstrap'
import { infoBox } from './utils/info-box';

// 在启动时从后端获取站点配置信息，并保存到 localStorage，随后按配置动态加载主题样式
import themeService from '/@/utils/themeService';

/**
 * 尝试在应用挂载前获取站点配置（如果可用），并根据返回的 theme 字段动态加载主题样式。
 * 保存结果到 localStorage（key: localStorageKeyConst.APP_CONFIG），以便 `useAppConfigStore` 在初始化时读取。
 */
// 使用 themeService.loadAndApplyTheme 在挂载前加载并应用主题
async function fetchAndApplySiteConfig() {
    try {
        await themeService.loadAndApplyTheme();
    } catch (e) {
        console.warn('fetch site infos failed:', e);
    }
}

const app = createApp(App);

// 初始化本地存储或其他逻辑
Initializer();

// 在挂载前尝试获取站点配置并应用（theme、其他 runtime 配置会写入 localStorage）
fetchAndApplySiteConfig().then(() => {
    app.use(router).use(store).use(Antd);

    // 注册全局方法 infoBox
    app.config.globalProperties.$infoBox = infoBox;

    // 注册图标组件
    Object.keys(antIcons).forEach((key) => {
        app.component(key, antIcons[key]);
    });

    app.mount('#app');
});

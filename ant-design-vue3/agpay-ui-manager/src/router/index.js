import { createRouter, createWebHistory } from 'vue-router';
import UserLayout from '/@/layouts/user-layout.vue'
import AgLayout from '/@/layouts/index.vue'
import nProgress from 'nprogress';
import 'nprogress/nprogress.css';

const routes = [
    // 定义你的路由配置
    {
        path: '/',
        name: 'user',
        component: UserLayout,
        children: [
            { path: 'login', name: 'login', component: import('/@/views/user/login.vue') },
            { path: 'forget', name: 'forget', component: () => import('/@/views/user/forget.vue') }
        ]
    },
    {
        path: '/main',
        name: 'index',
        component: AgLayout
    },
];

export const router = createRouter({
    history: createWebHistory(),
    routes,
});

// 获取到第一个uri (递归查找)
function getOneUri (item) {
    let result = ''
    for (let i = 0; i < item.length; i++) {
        if (item[i].menuUri && item[i].entType === 'ML') {
            return item[i].menuUri
        }

        if (item[i].children) {
            result = getOneUri(item[i].children)
            if (result) {
                return result
            }
        }
    }
    return result
}

router.beforeEach((to, from, next) => {
    // 进度条开启
    nProgress.start();


})

router.afterEach(() => {
    nProgress.done();
});

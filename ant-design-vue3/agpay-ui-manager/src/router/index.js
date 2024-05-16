import { createRouter, createWebHistory } from 'vue-router';
import UserLayout from '/@/layouts/user-layout.vue'

const routes = [
    // 定义你的路由配置
    {
        path: '/',
        component: UserLayout,
        children: [
            { path: 'login', name: 'login', component: import('/@/views/user/login.vue') },
            { path: 'forget', name: 'forget', component: () => import('/@/views/user/forget.vue') }
        ]
    },
];

export const router = createRouter({
    history: createWebHistory(),
    routes,
});

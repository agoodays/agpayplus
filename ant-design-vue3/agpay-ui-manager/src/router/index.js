import nProgress from 'nprogress';
import 'nprogress/nprogress.css';
import { createRouter, createWebHistory } from 'vue-router';
import UserLayout from '/@/layouts/user-layout.vue'
import AgLayout from '/@/layouts/index.vue'
import { useUserStore } from '/@/store/modules/system/user';
import { localClear, localRead } from '/@/utils/local-util';
import { setDocumentTitle } from '/@/utils/domUtil'
import { PAGE_PATH_404, PAGE_PATH_LOGIN } from '/@/constants/common-const';
import localStorageKeyConst from '/@/constants/local-storage-key-const';
import { loginApi } from '/@/api/system/login-api';
import { asyncRouteDefine } from '/@/config/app-config'

const routes = [
    // 定义你的路由配置
    {
        path: '/',
        name: 'user',
        component: UserLayout,
        children: [
            { path: 'login', name: 'login', component: () => import('/@/views/user/login.vue') },
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
    strict: true,
    scrollBehavior: () => ({ left: 0, top: 0 }),
});

const allowList = ['login', 'forget', 'register', 'registerResult'] // no redirect allowList

// 封装跳转到指定路由的函数
function redirectToTargetRoute (path, next) {
    next(path === '/' ? redirectFunc() : undefined)
}

// 动态跳转路径 func
function redirectFunc () {
    let mainPageUri = ''
    useUserStore().allMenuRouteTree.forEach(item => {
        if (item.entId === 'ENT_C_MAIN') { // 当前用户是否拥有主页权限， 如果有直接跳转到该路径
            mainPageUri = item.menuUri
            return false
        }
    })

    if (mainPageUri) {
        return mainPageUri
    }

    return getOneUri(useUserStore().allMenuRouteTree)
}

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

    to.meta && (typeof to.meta.title !== 'undefined' && setDocumentTitle(`${to.meta.title} - ${import.meta.env.VITE_APP_TITLE}`)); // 设置浏览器标题

    // 公共页面，任何时候都可以跳转
    if (to.path === PAGE_PATH_404) {
        next();
        return;
    }
    // 如果在免登录页面则直接放行
    if (allowList.includes(to.name)) {
        // 在免登录名单，直接进入
        next();
        return;
    }

    // 验证登录
    const token = localRead(localStorageKeyConst.USER_TOKEN);
    if (!token) {
        localClear();
        next({ path: PAGE_PATH_LOGIN, query: { redirect: to.fullPath }  });
        return;
    }

    // 以下为包含Token的情况
    // 如果用户信息不存在， 则重新获取 [用户登录成功 & 强制刷新浏览器时 会执行该函数]
    if (!useUserStore().userId) {
        // request login userInfo

        loginApi.getCurrentInfo().then(bizData => {
            useUserStore().setUserLoginInfo(bizData); // 调用vuex设置用户基本信息

            // 动态添加路由
            const menuRouterList = generator(useUserStore().allMenuRouteTree);
            buildRoutes(menuRouterList);

            // 判断是否存在路由参数 redirect
            if (to.query.redirect) {
                next(to.query.redirect) ;// 跳转到指定的路由
            } else {
                redirectToTargetRoute(to.path, next);
            }
        }).catch(() => {
            // 失败时，获取用户信息失败时，调用登出，来清空历史保留信息
            useUserStore().logout();
            next({ path: PAGE_PATH_LOGIN, query: { redirect: to.fullPath }  });
        });
    } else {
        redirectToTargetRoute(to.path, next);
    }
    next();
})

router.afterEach(() => {
    nProgress.done();
});

/**
 * 绑定路由
 * @param menuRouterList
 */
function buildRoutes(menuRouterList) {
    router.addRoute({
        path: '/',
        meta: {},
        component: AgLayout,
        redirect: redirectFunc, // 根页面【/】默认跳转 地址
        children: menuRouterList,
    });
}

/**
 * 格式化树形结构数据 生成 vue-router 层级路由表
 *
 * @param routerMap
 * @returns {*}
 */
const generator = (allMenuRouteTreeArray) => {
    const menuResult = [];

    // 1、构建整个路由信息
    allMenuRouteTreeArray.map(item => {
        const defComponent = null;

        // 找不到组件 || 其他菜单
        if (!defComponent) {
            return;
        }

        // 跳转uri
        let path = item.menuUri || defComponent.defaultPath;

        // 没有配置path, 如果为目录则允许为空， 否则不在加载此配置
        if (!path) {
            if (item.children && item.children.length > 0) {
                path = `/${item.entId}`;
            } else {
                return; // 不再加载此配置项
            }
        }

        const currentRouter = {
            // 如果路由设置了 path，则作为默认 path，否则 路由地址 为默认配置
            path: path,
            // 路由名称，建议唯一
            name: item.entId,
            // 该路由对应页面的 组件 :方案2 (动态加载)
            component: ((defComponent && defComponent.component) || (() => import(`/@/views/${item.componentName}`))),
            // meta: 页面标题, 菜单图标, 页面权限(供指令权限用，可去掉)
            meta: {
                title: item.entName,
                icon: item.menuIcon,
                keepAlive: false,
            },
            hidden: item.entType === 'MO', // 当其他菜单时需要隐藏显示
        };
        // 是否有子菜单，并递归处理
        if (item.children && item.children.length > 0) {
            // Recursion
            currentRouter.children = generator(item.children);
        }
        menuResult.push(currentRouter);
    });

    return menuResult;
}

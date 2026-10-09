/*
 * 应用默认配置
 *
 * 公共配置（appDefaultConfig / defaultThemeConfig / defaultLayoutConfig）
 * 已迁移至 shared/config/common-config.js 统一维护。
 */
export { appDefaultConfig, defaultLayoutConfig, defaultThemeConfig } from '../../../shared/config/common-config.js'

/**
 * 与后端开发人员的路由名称及配置项
 * 组件名称 ：{ 默认跳转路径（如果后端配置则已动态配置为准）， 组件渲染 }
 * */
export const asyncRouteDefine = {
  CurrentUserInfo: { defaultPath: '/current/userinfo', component: () => import('@/views/current/user-info-page.vue') },
  MainPage: { defaultPath: '/main', component: () => import('@/views/main/main-page.vue') },
  NoticeInfoPage: { defaultPath: '/notices', component: () => import('@/views/sys/notice/notice-info-page.vue') }, // 公告管理
  // 账户中心
  StatisticsPage: { defaultPath: '/statistic', component: () => import('@/views/account/statistics-page.vue') },
  PayConfigPage: { defaultPath: '/passageConfig', component: () => import('@/views/account/pay-config-page.vue') },
  // 商户管理
  MchPage: { defaultPath: '/mch', component: () => import('@/views/mch/mch-page.vue') },
  MchAppPage: { defaultPath: '/apps', component: () => import('@/views/mch-app/mch-app-page.vue') },
  MchStorePage: { defaultPath: '/store', component: () => import('@/views/mch-store/mch-store-page.vue') },
  // 代理商管理
  AgentPage: { defaultPath: '/agent', component: () => import('@/views/agent/agent-page.vue') },
  // 订单管理
  PayOrderPage: { defaultPath: '/payOrder', component: () => import('@/views/order/pay/pay-order-page.vue') },
  RefundOrderPage: {
    defaultPath: '/refundOrder',
    component: () => import('@/views/order/refund/refund-order-page.vue')
  },
  // 系统管理
  SysUserPage: { defaultPath: '/users', component: () => import('@/views/sys/user/sys-user-page.vue') },
  SysUserTeamPage: { defaultPath: '/sysUserTeam', component: () => import('@/views/sys/team/team-page.vue') },
  RolePage: { defaultPath: '/role', component: () => import('@/views/sys/role/role-page.vue') },
  AgentConfigPage: {
    defaultPath: '/agentConfig',
    component: () => import('@/views/agent-config/agent-config-page.vue')
  }
}

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
  MchInfoPage: { defaultPath: '/mchInfo', component: () => import('@/views/mch/mch-info-page.vue') },
  MchAppPage: { defaultPath: '/apps', component: () => import('@/views/mch-app/mch-app-page.vue') },
  MchStorePage: { defaultPath: '/store', component: () => import('@/views/mch-store/mch-store-page.vue') },
  PayOrderPage: { defaultPath: '/payOrder', component: () => import('@/views/order/pay/pay-order-page.vue') },
  RefundOrderPage: { defaultPath: '/refundOrder', component: () => import('@/views/order/refund/refund-order-page.vue') },
  TransferOrderPage: { defaultPath: '/transferOrder', component: () => import('@/views/order/transfer/transfer-order-page.vue') },
  DivisionReceiverGroupPage: { defaultPath: '/divisionReceiverGroup', component: () => import('@/views/division/group/division-receiver-group-page.vue') },
  DivisionReceiverPage: { defaultPath: '/divisionReceiver', component: () => import('@/views/division/receiver/division-receiver-page.vue') },
  DivisionRecordPage: { defaultPath: '/divisionRecord', component: () => import('@/views/division/record/division-record-page.vue') },
  MchConfigPage: { defaultPath: '/mchConfig', component: () => import('@/views/mch-config/mch-config-page.vue') },
  NoticeInfoPage: { defaultPath: '/notices', component: () => import('@/views/sys/notice/notice-info-page.vue') }, // 公告管理
  MchTransferPage: { defaultPath: '/transfer', component: () => import('@/views/transfer/mch-transfer-page.vue') },
  RolePage: { defaultPath: '/role', component: () => import('@/views/sys/role/role-page.vue') },
  SysUserPage: { defaultPath: '/users', component: () => import('@/views/sys/user/sys-user-page.vue') },
  PayTestPage: { defaultPath: '/payTest', component: () => import('@/views/pay-test/pay-test-page.vue') }
}

/*
 * 应用默认配置
 *
 */
export const appDefaultConfig = {
  // i18n 语言选择
  language: 'zh_CN',
  // 布局: side 或者 side-expand 或者 top
  layout: 'side',
  // 侧边菜单宽度 ， 默认为260px
  sideMenuWidth: 260,
  // 菜单主题
  sideMenuTheme: 'dark',
  // 主题颜色索引
  colorIndex: 0,
  // 顶部菜单页面宽度
  pageWidth: '99%',
  // 圆角
  borderRadius: 6,
  // 标签页
  pageTagFlag: true,
  // 面包屑
  breadCrumbFlag: true,
  // 页脚
  footerFlag: true,
  // 帮助文档
  helpDocFlag: true,
  // 水印
  watermarkFlag: true,
  // 主题颜色
  // 主题颜色（作为回退值）。优先由运行时 CSS 变量 `--primary-color` 覆盖。
  primaryColor: '#1677ff',
  // 紧凑
  compactFlag: false
}

/**
 * 应用级默认主题配置
 * 说明：
 * - 供 store/theme 工具复用，避免多处重复定义
 */
export const defaultThemeConfig = {
  primaryColor: appDefaultConfig.primaryColor,
  darkMode: true,
  grayMode: false,
  colorWeakMode: false,
  compactTheme: appDefaultConfig.compactFlag,
  borderRadius: appDefaultConfig.borderRadius
}

/**
 * 应用级默认布局配置
 */
export const defaultLayoutConfig = {
  layoutMode: 'classic',
  menuSplit: false
}

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
  NoticeInfoPage: { defaultPath: '/notices', component: () => import('@/views/notice/notice-list-page.vue') },
  MchTransferPage: { defaultPath: '/transfer', component: () => import('@/views/transfer/mch-transfer-page.vue') },
  RolePage: { defaultPath: '/role', component: () => import('@/views/role/role-page.vue') },
  SysUserPage: { defaultPath: '/sysUser', component: () => import('@/views/sysuser/sys-user-page.vue') },
  PayTestPage: { defaultPath: '/payTest', component: () => import('@/views/pay-test/pay-test-page.vue') }
}

export const devUserInfo = {
  sysUserId: 'dev-mch-001',
  realname: '商户开发者',
  loginUsername: 'merchant_dev',
  avatarUrl: '',
  telphone: '13800138000',
  sex: 1,
  state: 1,
  isAdmin: true,
  sysType: 'MCH',
  belongInfoId: 'dev-mch-info-001',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString()
}
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
  CurrentUserInfo: { defaultPath: '/current/userinfo', component: () => import('@/views/current/user-info-page.vue') }, // 用户设置
  MainPage: { defaultPath: '/main', component: () => import('@/views/main/main-page.vue') },
  SysUserTeamPage: { defaultPath: '/teams', component: () => import('@/views/sys-user-team/team-list.vue') },
  RolePage: { defaultPath: '/roles', component: () => import('@/views/role/role-page.vue') },
  EntPage: { defaultPath: '/ents', component: () => import('@/views/ent/ent-page.vue') },
  PayWayPage: { defaultPath: '/payways', component: () => import('@/views/pay-config/pay-config-page.vue') },
  QrCodePage: { defaultPath: '/qrc', component: () => import('@/views/qr-code/list.vue') },
  QrCodeShellPage: { defaultPath: '/shell', component: () => import('@/views/qr-code/shell/list.vue') },
  IsvListPage: { defaultPath: '/isv', component: () => import('@/views/isv/isv-list.vue') }, // 服务商列表
  AgentListPage: { defaultPath: '/agent', component: () => import('@/views/agent/agent-list.vue') }, // 代理商列表
  AccountBillPage: { defaultPath: '/accountBill', component: () => import('@/views/account-bill/account-bill-page.vue') }, // 代理商列表
  MchListPage: { defaultPath: '/mch', component: () => import('@/views/mch/mch-list-page.vue') }, // 商户列表
  MchAppPage: { defaultPath: '/apps', component: () => import('@/views/mch-app/mch-app-page.vue') }, // 商户应用列表
  MchStorePage: { defaultPath: '/store', component: () => import('@/views/mch-store/mch-store-page.vue') }, // 商户门店列表
  PayOrderListPage: { defaultPath: '/payOrder', component: () => import('@/views/order/pay-order-page.vue') }, // 支付订单列表
  RefundOrderListPage: { defaultPath: '/refundOrder', component: () => import('@/views/order/refund-order-page.vue') }, // 退款订单列表
  TransferOrderListPage: { defaultPath: '/transferOrder', component: () => import('@/views/order/transfer-order-page.vue') }, // 转账订单
  TransactionPage: { defaultPath: '/statistic/transaction', component: () => import('@/views/statistic/transaction/transaction-page.vue') }, // 交易报表
  MchCountPage: { defaultPath: '/statistic/mch', component: () => import('@/views/statistic/mch/mch-count-page.vue') }, // 商户统计
  AgentCountPage: { defaultPath: '/statistic/agent', component: () => import('@/views/statistic/agent/agent-count-page.vue') }, // 代理商统计
  IsvCountPage: { defaultPath: '/statistic/isv', component: () => import('@/views/statistic/isv/isv-count-page.vue') }, // 服务商统计
  ChannelCountPage: { defaultPath: '/statistic/channel', component: () => import('@/views/statistic/channel/channel-count-page.vue') }, // 通道统计
  DivisionReceiverGroupPage: { defaultPath: '/divisionReceiverGroup', component: () => import('@/views/division/group/division-receiver-group-page.vue') }, // 分账账号组管理
  DivisionReceiverPage: { defaultPath: '/divisionReceiver', component: () => import('@/views/division/receiver/division-receiver-page.vue') }, // 分账账号管理
  DivisionRecordPage: { defaultPath: '/divisionRecord', component: () => import('@/views/division/record/division-record-page.vue') }, // 分账记录
  SysConfigPage: { defaultPath: '/config', component: () => import('@/views/sys/config/sys-config.vue') }, // 系统配置
  NoticeInfoPage: { defaultPath: '/notices', component: () => import('@/views/notice/notice-list.vue') } // 公告管理
}

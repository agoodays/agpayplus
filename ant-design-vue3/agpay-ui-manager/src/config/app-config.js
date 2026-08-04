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
  // MainPage: { defaultPath: '/main', component: () => import('@/views/main/main-page.vue') },
  MainPage: { defaultPath: '/main', component: () => import('@/views/dashboard/analysis-page.vue') },
  // 商户管理
  MchPage: { defaultPath: '/mch', component: () => import('@/views/mch/mch-page.vue') }, // 商户列表
  MchAppPage: { defaultPath: '/apps', component: () => import('@/views/mch-app/mch-app-page.vue') }, // 商户应用列表
  MchStorePage: { defaultPath: '/store', component: () => import('@/views/mch-store/mch-store-page.vue') }, // 商户门店列表
  // 代理商管理
  AgentPage: { defaultPath: '/agent', component: () => import('@/views/agent/agent-page.vue') }, // 代理商列表
  // 服务商管理
  IsvPage: { defaultPath: '/isv', component: () => import('@/views/isv/isv-page.vue') }, // 服务商列表
  // 佣金管理
  PlatformProfitPage: { defaultPath: '/platformProfits', component: () => import('@/views/statistic/agent/agent-count-page.vue') }, // 平台佣金统计
  AccountBillPage: { defaultPath: '/accountBill', component: () => import('@/views/account-bill/account-bill-page.vue') }, // 钱包流水
  // 订单管理
  PayOrderPage: { defaultPath: '/payOrder', component: () => import('@/views/order/pay/pay-order-page.vue') }, // 支付订单列表
  RefundOrderPage: { defaultPath: '/refundOrder', component: () => import('@/views/order/refund/refund-order-page.vue') }, // 退款订单列表
  TransferOrderPage: { defaultPath: '/transferOrder', component: () => import('@/views/order/transfer/transfer-order-page.vue') }, // 转账订单
  MchNotifyPage: { defaultPath: '/notify', component: () => import('@/views/order/notify/mch-notify-page.vue') }, // 商户通知列表
  // 数据统计
  TransactionPage: { defaultPath: '/statistic/transaction', component: () => import('@/views/statistic/transaction/transaction-page.vue') }, // 交易报表
  MchCountPage: { defaultPath: '/statistic/mch', component: () => import('@/views/statistic/mch/mch-count-page.vue') }, // 商户统计
  AgentCountPage: { defaultPath: '/statistic/agent', component: () => import('@/views/statistic/agent/agent-count-page.vue') }, // 代理商统计
  IsvCountPage: { defaultPath: '/statistic/isv', component: () => import('@/views/statistic/isv/isv-count-page.vue') }, // 服务商统计
  ChannelCountPage: { defaultPath: '/statistic/channel', component: () => import('@/views/statistic/channel/channel-count-page.vue') }, // 通道统计
  // 分账管理
  DivisionReceiverGroupPage: { defaultPath: '/divisionReceiverGroup', component: () => import('@/views/division/group/division-receiver-group-page.vue') }, // 分账账号组管理
  DivisionReceiverPage: { defaultPath: '/divisionReceiver', component: () => import('@/views/division/receiver/division-receiver-page.vue') }, // 分账账号管理
  DivisionRecordPage: { defaultPath: '/divisionRecord', component: () => import('@/views/division/record/division-record-page.vue') }, // 分账记录
  // 支付配置
  IfDefinePage: { defaultPath: '/ifdefines', component: () => import('@/views/pay-config/pay-if-define/if-define-page.vue') },
  PayWayPage: { defaultPath: '/payways', component: () => import('@/views/pay-config/pay-way/pay-way-page.vue') },
  // 设备配置
  QrCodePage: { defaultPath: '/qrc', component: () => import('@/views/qr-code/qr-code-page.vue') },
  QrCodeShellPage: { defaultPath: '/shell', component: () => import('@/views/qr-code/shell/qr-code-shell-page.vue') },
  // 系统管理
  SysUserPage: { defaultPath: '/users', component: () => import('@/views/sys/user/sys-user-page.vue') },
  RolePage: { defaultPath: '/roles', component: () => import('@/views/sys/role/role-page.vue') },
  EntPage: { defaultPath: '/ents', component: () => import('@/views/sys/ent/ent-page.vue') },
  SysUserTeamPage: { defaultPath: '/teams', component: () => import('@/views/sys/team/team-page.vue') },
  SysConfigPage: { defaultPath: '/config', component: () => import('@/views/sys/config/sys-config.vue') }, // 系统配置
  SysLogPage: { defaultPath: '/log', component: () => import('@/views/sys/log/sys-log.vue') },
  NoticeInfoPage: { defaultPath: '/notices', component: () => import('@/views/sys/notice/notice-info-page.vue') }, // 公告管理
}

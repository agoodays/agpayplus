/**
 * 开发模式菜单配置（Agent）
 */
export const devMenuTree = [
  {
    entId: 'ENT_HOME',
    entName: '首页',
    menuUri: '/main',
    componentName: 'MainPage',
    entType: 'ML',
    menuIcon: 'HomeOutlined'
  },
  {
    entId: 'ENT_AGENT',
    entName: '代理商管理',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'BankOutlined',
    children: [
      { entId: 'ENT_AGENT_LIST', entName: '代理商列表', menuUri: '/agent', componentName: 'AgentPage', entType: 'ML', menuIcon: 'ClusterOutlined' }
    ]
  },
  {
    entId: 'ENT_MCH',
    entName: '商户管理',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'ShopOutlined',
    children: [
      { entId: 'ENT_MCH_LIST', entName: '商户列表', menuUri: '/mch', componentName: 'MchPage', entType: 'ML', menuIcon: 'ShoppingOutlined' },
      { entId: 'ENT_MCH_APP', entName: '商户应用', menuUri: '/apps', componentName: 'MchAppPage', entType: 'ML', menuIcon: 'AppstoreOutlined' },
      { entId: 'ENT_MCH_STORE', entName: '商户门店', menuUri: '/store', componentName: 'MchStorePage', entType: 'ML', menuIcon: 'EnvironmentOutlined' }
    ]
  },
  {
    entId: 'ENT_ORDER',
    entName: '订单管理',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'FileTextOutlined',
    children: [
      { entId: 'ENT_PAY_ORDER', entName: '支付订单', menuUri: '/payOrder', componentName: 'PayOrderPage', entType: 'ML', menuIcon: 'DollarOutlined' },
      { entId: 'ENT_REFUND_ORDER', entName: '退款订单', menuUri: '/refundOrder', componentName: 'RefundOrderPage', entType: 'ML', menuIcon: 'RollbackOutlined' }
    ]
  },
  {
    entId: 'ENT_DIVISION',
    entName: '分账管理',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'ShareAltOutlined',
    children: [
      { entId: 'ENT_DIV_GROUP', entName: '分账账号组', menuUri: '/divisionReceiverGroup', componentName: 'DivisionReceiverGroupPage', entType: 'ML', menuIcon: 'TeamOutlined' },
      { entId: 'ENT_DIV_RECEIVER', entName: '分账账号', menuUri: '/divisionReceiver', componentName: 'DivisionReceiverPage', entType: 'ML', menuIcon: 'UserOutlined' },
      { entId: 'ENT_DIV_RECORD', entName: '分账记录', menuUri: '/divisionRecord', componentName: 'DivisionRecordPage', entType: 'ML', menuIcon: 'OrderedListOutlined' }
    ]
  },
  {
    entId: 'ENT_STAT',
    entName: '数据统计',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'BarChartOutlined',
    children: [
      { entId: 'ENT_STAT_TX', entName: '交易报表', menuUri: '/statistic/transaction', componentName: 'TransactionPage', entType: 'ML', menuIcon: 'LineChartOutlined' },
      { entId: 'ENT_STAT_AGENT', entName: '代理商统计', menuUri: '/statistic/agent', componentName: 'AgentCountPage', entType: 'ML', menuIcon: 'FundOutlined' },
      { entId: 'ENT_STAT_MCH', entName: '商户统计', menuUri: '/statistic/mch', componentName: 'MchCountPage', entType: 'ML', menuIcon: 'PieChartOutlined' }
    ]
  },
  {
    entId: 'ENT_BILL',
    entName: '钱包流水',
    menuUri: '/accountBill',
    componentName: 'AccountBillPage',
    entType: 'ML',
    menuIcon: 'BookOutlined'
  },
  {
    entId: 'ENT_AGENT_CONFIG',
    entName: '账户设置',
    menuUri: '/agentConfig',
    componentName: 'AgentConfigPage',
    entType: 'ML',
    menuIcon: 'SettingOutlined'
  },
  {
    entId: 'ENT_AGENT_ACCOUNT',
    entName: '代理商账户',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'WalletOutlined',
    children: [
      { entId: 'ENT_AGENT_SELF_PAY_CONFIG', entName: '分账配置', menuUri: '/account/payConfig', componentName: 'PayConfigPage', entType: 'ML', menuIcon: 'BankOutlined' },
      { entId: 'ENT_AGENT_SELF_STATISTICS', entName: '经营统计', menuUri: '/account/statistics', componentName: 'StatisticsPage', entType: 'ML', menuIcon: 'BarChartOutlined' }
    ]
  },
  {
    entId: 'ENT_ROLE',
    entName: '角色管理',
    menuUri: '/role',
    componentName: 'RolePage',
    entType: 'ML',
    menuIcon: 'TeamOutlined'
  },
  {
    entId: 'ENT_SYS_USER',
    entName: '子用户管理',
    menuUri: '/sysUser',
    componentName: 'SysUserPage',
    entType: 'ML',
    menuIcon: 'UserOutlined'
  },
  {
    entId: 'ENT_ARTICLE_NOTICEINFO',
    entName: '公告列表',
    menuUri: '/notice/table',
    componentName: 'NoticeInfoPage',
    entType: 'ML',
    menuIcon: 'NotificationOutlined'
  },
  {
    entId: 'ENT_ARTICLE_NOTICELIST',
    entName: '公告浏览',
    menuUri: '/notice/list',
    componentName: 'NoticeListPage',
    entType: 'ML',
    menuIcon: 'ReadOutlined'
  }
]

export const devUserInfo = {
  sysUserId: 'dev-agent-001',
  realname: '代理商开发者',
  loginUsername: 'agent_dev',
  avatarUrl: '',
  telphone: '13800138000',
  sex: 1,
  state: 1,
  isAdmin: true,
  sysType: 'AGENT',
  belongInfoId: 'dev-agent-info-001',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString()
}

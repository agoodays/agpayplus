/**
 * 开发模式菜单配置（Merchant）
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
    entId: 'ENT_MCH',
    entName: '商户信息',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'BankOutlined',
    children: [
      { entId: 'ENT_MCH_INFO', entName: '商户基本信息', menuUri: '/mchInfo', componentName: 'MchInfoPage', entType: 'ML', menuIcon: 'IdcardOutlined' },
      { entId: 'ENT_MCH_APP', entName: '商户应用', menuUri: '/apps', componentName: 'MchAppPage', entType: 'ML', menuIcon: 'AppstoreOutlined' },
      { entId: 'ENT_MCH_STORE', entName: '商户门店', menuUri: '/store', componentName: 'MchStorePage', entType: 'ML', menuIcon: 'ShopOutlined' }
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
      { entId: 'ENT_REFUND_ORDER', entName: '退款订单', menuUri: '/refundOrder', componentName: 'RefundOrderPage', entType: 'ML', menuIcon: 'RollbackOutlined' },
      { entId: 'ENT_TRANSFER_ORDER', entName: '转账订单', menuUri: '/transferOrder', componentName: 'TransferOrderPage', entType: 'ML', menuIcon: 'SwapOutlined' },
      { entId: 'ENT_NOTIFY', entName: '商户通知', menuUri: '/notify', componentName: 'MchNotifyPage', entType: 'ML', menuIcon: 'BellOutlined' }
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
    entId: 'ENT_MCH_CONFIG',
    entName: '商户配置',
    menuUri: '/mchConfig',
    componentName: 'MchConfigPage',
    entType: 'ML',
    menuIcon: 'SettingOutlined'
  },
  {
    entId: 'ENT_TRANSFER',
    entName: '商户转账',
    menuUri: '/transfer',
    componentName: 'MchTransferPage',
    entType: 'ML',
    menuIcon: 'SwapOutlined'
  },
  {
    entId: 'ENT_NOTICE',
    entName: '公告信息',
    menuUri: '/noticeList',
    componentName: 'NoticePage',
    entType: 'ML',
    menuIcon: 'NotificationOutlined'
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
    entId: 'ENT_PAY_TEST',
    entName: '支付测试',
    menuUri: '/payTest',
    componentName: 'PayTestPage',
    entType: 'ML',
    menuIcon: 'ExperimentOutlined'
  }
]

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

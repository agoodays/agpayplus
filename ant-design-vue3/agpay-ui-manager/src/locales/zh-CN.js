export default {
  app: {
    title: 'AgPay运营平台',
    startupSuccess: '应用启动成功',
    startupFailed: '应用启动失败',
    themeColor: '当前主题色'
  },
  menu: {
    devDemo: {
      root: '组件示例',
      index: '组件总览',
      searchTable: '搜索表格',
      basicTable: '基础表格',
      stateSwitch: '状态切换',
      form: '表单组件',
      floatLabel: '浮动标签组件',
      selectInfinite: '分页下拉选择',
      card: '卡片组件',
      upload: '文件上传',
      editor: '富文本编辑',
      container: '容器组件'
    }
  },
  auth: {
    loginTitle: '运营平台登录',
    loginNameOrPhone: '登录名/手机',
    password: '密码',
    captcha: '图形验证码',
    captchaExpiredRefresh: '已过期 请刷新',
    autoLogin: '自动登录',
    forgotPassword: '忘记密码?',
    login: '登录',
    pleaseInputLoginNameOrPhone: '请输入登录名/手机号',
    pleaseInputPassword: '请输入密码',
    pleaseInputCaptcha: '请输入验证码',
    loginFailedRetry: '登录失败，请重试',
    welcome: '欢迎',
    welcomeBack: '{greet}，欢迎回来{name}',
    lastLoginTime: '上次登录时间：{time}',
    forgetTitle: '找回密码',
    pleaseInputPhone: '请输入手机号',
    pleaseInputValidPhone: '请输入正确的手机号',
    pleaseInputSmsCode: '请输入验证码',
    resendInSeconds: '{seconds}秒后重新发送',
    sendSmsCode: '发送短信验证码',
    pleaseInputNewPassword: '请输入新密码',
    pleaseInputConfirmPassword: '请输入确认新密码',
    goLogin: '去登录 >>',
    retrievePassword: '找回密码',
    passwordNotMatch: '两次输入密码不一致',
    smsCodeSent: '验证码已发送，请注意查收',
    sendSmsCodeFailed: '发送验证码失败',
    retrieveFailedRetry: '找回密码失败，请重试',
    passwordResetSuccess: '密码重置成功，请使用新密码登录'
  },
  layout: {
    refreshPage: '刷新页面',
    userCenter: '个人中心',
    accountSetting: '账户设置',
    language: '语言',
    languageZhCN: '简体中文',
    languageEnUS: 'English',
    logout: '退出登录',
    confirmLogoutTitle: '确认退出',
    confirmLogoutContent: '你好 {name}，确认退出登录吗？',
    footerCopyright: 'Copyright ©2023-{year} AgPay | 吉日科技'
  },
  userLayout: {
    footerCopyright: 'Copyright ©2023-{year} 吉日科技 版权所有',
    icpRecord: 'ICP备案：鄂ICP备19941223号-9',
    pcacRecord: '中国支付清算协会备案编码：W2016091300000019',
    telecomPermit: '电信增值业务许可证编号：鄂A2-20160913'
  },
  components: {
    upload: '上传',
    more: '更多',
    view: '查看',
    edit: '编辑',
    delete: '删除',
    confirmDelete: '确认删除？',
    scrollLoadMore: '滚动加载更多',
    searching: '搜索中...'
  },
  common: {
    success: '成功',
    error: '错误',
    warning: '警告',
    tip: '提示',
    confirm: '确定',
    cancel: '取消',
    reset: '重置',
    done: '完成',
    backPrevious: '返回上一页',
    comingSoon: '此功能正在开发中，敬请期待',
    featureDescription: '功能说明',
    plannedFeatures: '计划功能',
    networkError: '网络发生错误',
    confirmDeleteTitle: '确认删除',
    confirmDeleteContent: '确定要删除吗？',
    deleteSuccess: '删除成功',
    exportSuccess: '导出成功',
    exportFailed: '导出失败',
    exportInDevelopment: '导出功能开发中',
    addSuccess: '新增成功',
    editSuccess: '修改成功',
    editFailed: '修改失败',
    operationFailed: '操作失败',
    uploadSuccess: '上传成功',
    uploadFailed: '上传失败',
    loadDataFailed: '加载数据失败'
  },
  main: {
    welcomeDesc: '欢迎使用 AgPay 运营管理平台',
    todayAmount: '今日交易金额',
    yuan: '元',
    todayCount: '今日交易笔数',
    countUnit: '笔',
    totalMch: '商户总数',
    totalAgent: '代理商总数',
    itemUnit: '个',
    quickEntry: '快速入口',
    defaultUser: '用户',
    greeting: '{greet}，{name}！'
  },
  time: {
    greetingEarlyMorning: '早上好',
    greetingMorning: '上午好',
    greetingNoon: '中午好',
    greetingAfternoon: '下午好',
    greetingEvening: '晚上好'
  },
  exception500: {
    title: '服务器错误',
    desc: '抱歉，服务器出错了',
    backHome: '返回首页'
  },
  payConfig: {
    comingSoonTitle: '支付配置功能',
    featureIntro: '商户支付配置功能允许为不同商户配置支付接口参数，包括支付宝、微信支付等第三方支付渠道。',
    features: {
      apiConfig: {
        title: '支付接口配置',
        desc: '支持配置支付宝、微信支付、云闪付等主流支付接口'
      },
      paramManage: {
        title: '参数管理',
        desc: '管理商户的支付参数，如商户号、密钥、证书等'
      },
      apiTest: {
        title: '接口测试',
        desc: '提供接口测试功能，快速验证配置是否正确'
      },
      statusMonitor: {
        title: '状态监控',
        desc: '实时监控支付接口状态，及时发现和处理异常'
      },
      auditLog: {
        title: '日志记录',
        desc: '记录配置变更日志，便于追溯和审计'
      }
    }
  },
  transferOrder: {
    comingSoonTitle: '转账订单管理',
    featureIntro: '转账订单管理功能允许查看和管理商户的转账订单，包括商户提现、代付等场景。',
    features: {
      orderList: {
        title: '转账订单列表',
        desc: '查看所有转账订单，支持多维度筛选和搜索'
      },
      orderDetail: {
        title: '转账详情',
        desc: '查看转账订单的详细信息，包括收款人、金额、状态等'
      },
      orderReview: {
        title: '转账审核',
        desc: '对转账订单进行审核，确保资金安全'
      },
      batchTransfer: {
        title: '批量转账',
        desc: '支持批量转账功能，提高操作效率'
      },
      transferStats: {
        title: '转账统计',
        desc: '提供转账数据统计分析功能'
      }
    }
  },
  refund: {
    modalTitle: '订单退款',
    noticeTitle: '退款说明',
    noticeDesc: '请谨慎操作，退款成功后资金将原路返回到用户账户',
    payOrderId: '支付订单号',
    orderAmount: '订单金额',
    mchName: '商户名称',
    refundAmount: '退款金额',
    pleaseInputRefundAmount: '请输入退款金额',
    maxRefundAmount: '最大可退款金额',
    refundReason: '退款原因',
    pleaseSelectRefundReason: '请选择退款原因',
    reasonUserRequest: '用户申请退款',
    reasonOrderException: '订单异常',
    reasonOutOfStock: '商品缺货',
    reasonOther: '其他',
    remark: '退款备注',
    pleaseInputRemarkOptional: '请输入退款备注（选填）',
    amountMustGtZero: '退款金额必须大于0',
    amountCannotExceedOrder: '退款金额不能大于订单金额',
    confirmTitle: '确认退款？',
    confirmContent: '将退款 ¥{amount} 到用户账户，此操作不可撤销',
    submitSuccess: '退款申请已提交',
    submitFailed: '退款失败'
  },
  mchStore: {
    bindAppTitle: '应用分配',
    pleaseSelectBindApp: '请选择要绑定的应用',
    pleaseSelectApp: '请选择应用',
    emptyOption: '（空）',
    noAvailableApp: '该商户暂无可用应用',
    loadAppListFailed: '加载应用列表失败',
    confirmBindTitle: '确认保存应用吗？',
    confirmBindContent: '请确认是否绑定选中的应用',
    bindAppSuccess: '绑定应用成功',
    bindAppFailed: '绑定失败',
    onlyImageAllowed: '只能上传图片文件！',
    imageMax2m: '图片大小不能超过 2MB！'
  },
  mchApp: {
    oauth2ComingSoon: 'Oauth2配置功能开发中...',
    payConfigComingSoon: '支付配置功能开发中...',
    secretGenerated: '密钥已生成'
  },
  mch: {
    confirmDeleteMchTitle: '确认删除该商户吗？',
    confirmDeleteMchContent: '该操作将删除商户下所有配置及用户信息',
    randomPasswordGenerated: '随机密码: {password}'
  },
  current: {
    confirmUpdateInfoTitle: '确认更新信息吗？',
    confirmUpdatePasswordTitle: '确认更新密码吗？',
    updatePasswordNeedRelogin: '更新密码后需要重新登录',
    editSuccessRelogin: '修改成功，请重新登录',
    safeWordEmpty: '信息内容不可为空',
    onlyJpgPngAllowed: '只能上传 JPG/PNG 格式的图片!',
    imageMax10m: '图片大小不能超过 10MB!',
    avatarUpdated: '头像更新成功'
  },
  agTable: {
    autoRefresh: '自动刷新',
    closeStatistics: '关闭统计',
    statistics: '数据统计',
    densityCompact: '紧凑',
    densityDefault: '默认',
    densityLoose: '宽松',
    tableDensity: '表格密度',
    dataExport: '数据导出',
    columnSettings: '列设置',
    selectAll: '全选',
    visibleCount: '显示: {visible} / {total}',
    notFixed: '不固定',
    fixedLeft: '左固定',
    fixedRight: '右固定',
    quickWidth: '常用宽度快速设置',
    autoWidth: '自动宽度',
    preset: '预设',
    moveUp: '上移',
    moveDown: '下移',
    noStatisticsData: '暂无统计数据',
    totalItems: '共 {total} 条',
    resetToDefaultSuccess: '已重置为默认配置',
    downloadNotConfigured: '未配置下载功能',
    exportTriggered: '导出任务已触发',
    exportFailed: '导出失败'
  }
}

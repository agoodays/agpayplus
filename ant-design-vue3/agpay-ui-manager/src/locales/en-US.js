export default {
  app: {
    title: 'AgPay Management Platform',
    startupSuccess: 'Application started successfully',
    startupFailed: 'Application failed to start',
    themeColor: 'Current theme color'
  },
  menu: {
    devDemo: {
      root: 'Component Demos',
      index: 'Component Overview',
      searchTable: 'Search Table',
      basicTable: 'Basic Table',
      stateSwitch: 'State Switch',
      form: 'Form Components',
      floatLabel: 'Float Label',
      selectInfinite: 'Infinite Select',
      card: 'Card Component',
      upload: 'File Upload',
      editor: 'Rich Text Editor',
      container: 'Container Component'
    }
  },
  auth: {
    loginTitle: 'Operations Platform Login',
    loginNameOrPhone: 'Login Name/Phone',
    password: 'Password',
    captcha: 'Captcha',
    captchaExpiredRefresh: 'Expired, click to refresh',
    autoLogin: 'Auto Login',
    forgotPassword: 'Forgot Password?',
    login: 'Login',
    pleaseInputLoginNameOrPhone: 'Please enter login name or phone',
    pleaseInputPassword: 'Please enter password',
    pleaseInputCaptcha: 'Please enter captcha',
    loginFailedRetry: 'Login failed, please try again',
    welcome: 'Welcome',
    welcomeBack: '{greet}, welcome back {name}',
    lastLoginTime: 'Last login time: {time}',
    forgetTitle: 'Retrieve Password',
    pleaseInputPhone: 'Please enter phone number',
    pleaseInputValidPhone: 'Please enter a valid phone number',
    pleaseInputSmsCode: 'Please enter verification code',
    resendInSeconds: 'Resend in {seconds}s',
    sendSmsCode: 'Send SMS Code',
    pleaseInputNewPassword: 'Please enter new password',
    pleaseInputConfirmPassword: 'Please confirm new password',
    goLogin: 'Go to Login >>',
    retrievePassword: 'Retrieve Password',
    passwordNotMatch: 'The two passwords do not match',
    smsCodeSent: 'Verification code sent successfully',
    sendSmsCodeFailed: 'Failed to send verification code',
    retrieveFailedRetry: 'Password retrieval failed, please try again',
    passwordResetSuccess: 'Password reset successful, please login with the new password'
  },
  layout: {
    refreshPage: 'Refresh Page',
    userCenter: 'User Center',
    accountSetting: 'Account Settings',
    language: 'Language',
    languageZhCN: '简体中文',
    languageEnUS: 'English',
    logout: 'Logout',
    confirmLogoutTitle: 'Confirm Logout',
    confirmLogoutContent: 'Hi {name}, are you sure you want to log out?',
    footerCopyright: 'Copyright ©2023-{year} AgPay | Agooday'
  },
  userLayout: {
    footerCopyright: 'Copyright ©2023-{year} Agooday. All rights reserved.',
    icpRecord: 'ICP Record: 鄂ICP备19941223号-9',
    pcacRecord: 'PCAC Filing Code: W2016091300000019',
    telecomPermit: 'Telecom Value-added Service Permit: 鄂A2-20160913'
  },
  components: {
    upload: 'Upload',
    more: 'More',
    view: 'View',
    edit: 'Edit',
    delete: 'Delete',
    confirmDelete: 'Confirm deletion?',
    scrollLoadMore: 'Scroll to load more',
    searching: 'Searching...'
  },
  common: {
    success: 'Success',
    error: 'Error',
    warning: 'Warning',
    tip: 'Notice',
    confirm: 'Confirm',
    cancel: 'Cancel',
    reset: 'Reset',
    done: 'Done',
    backPrevious: 'Back',
    comingSoon: 'This feature is under development. Stay tuned.',
    featureDescription: 'Feature Description',
    plannedFeatures: 'Planned Features',
    networkError: 'Network error occurred',
    confirmDeleteTitle: 'Confirm Delete',
    confirmDeleteContent: 'Are you sure you want to delete?',
    deleteSuccess: 'Deleted successfully',
    exportSuccess: 'Export succeeded',
    exportFailed: 'Export failed',
    exportInDevelopment: 'Export feature is under development',
    addSuccess: 'Added successfully',
    editSuccess: 'Updated successfully',
    editFailed: 'Update failed',
    operationFailed: 'Operation failed',
    uploadSuccess: 'Uploaded successfully',
    uploadFailed: 'Upload failed',
    loadDataFailed: 'Failed to load data'
  },
  main: {
    welcomeDesc: 'Welcome to AgPay Operations Management Platform',
    todayAmount: "Today's Transaction Amount",
    yuan: 'CNY',
    todayCount: "Today's Transaction Count",
    countUnit: 'orders',
    totalMch: 'Total Merchants',
    totalAgent: 'Total Agents',
    itemUnit: 'items',
    quickEntry: 'Quick Access',
    defaultUser: 'User',
    greeting: '{greet}, {name}!'
  },
  time: {
    greetingEarlyMorning: 'Good early morning',
    greetingMorning: 'Good morning',
    greetingNoon: 'Good noon',
    greetingAfternoon: 'Good afternoon',
    greetingEvening: 'Good evening'
  },
  exception500: {
    title: 'Server Error',
    desc: 'Sorry, something went wrong on the server',
    backHome: 'Back to Home'
  },
  payConfig: {
    comingSoonTitle: 'Payment Configuration',
    featureIntro:
      'Merchant payment configuration supports setting payment interface parameters for different merchants, including Alipay, WeChat Pay, and other channels.',
    features: {
      apiConfig: {
        title: 'Payment API Configuration',
        desc: 'Supports Alipay, WeChat Pay, UnionPay and other mainstream payment interfaces'
      },
      paramManage: {
        title: 'Parameter Management',
        desc: 'Manage merchant payment parameters such as merchant ID, keys, and certificates'
      },
      apiTest: {
        title: 'API Testing',
        desc: 'Provide API testing capabilities to quickly verify configuration correctness'
      },
      statusMonitor: {
        title: 'Status Monitoring',
        desc: 'Monitor payment interface status in real time to detect and handle issues promptly'
      },
      auditLog: {
        title: 'Audit Logs',
        desc: 'Record configuration change logs for traceability and auditing'
      }
    }
  },
  transferOrder: {
    comingSoonTitle: 'Transfer Order Management',
    featureIntro:
      'Transfer order management allows viewing and managing merchant transfer orders, including merchant withdrawal and payout scenarios.',
    features: {
      orderList: {
        title: 'Transfer Order List',
        desc: 'View all transfer orders with multi-dimensional filtering and search'
      },
      orderDetail: {
        title: 'Transfer Details',
        desc: 'View detailed transfer order information, including payee, amount, and status'
      },
      orderReview: {
        title: 'Transfer Review',
        desc: 'Review transfer orders to ensure fund security'
      },
      batchTransfer: {
        title: 'Batch Transfer',
        desc: 'Support batch transfer operations to improve efficiency'
      },
      transferStats: {
        title: 'Transfer Statistics',
        desc: 'Provide transfer data statistics and analysis'
      }
    }
  },
  refund: {
    modalTitle: 'Order Refund',
    noticeTitle: 'Refund Notice',
    noticeDesc:
      'Please proceed with caution. After a successful refund, funds will be returned to the user via the original route.',
    payOrderId: 'Pay Order ID',
    orderAmount: 'Order Amount',
    mchName: 'Merchant Name',
    refundAmount: 'Refund Amount',
    pleaseInputRefundAmount: 'Please enter refund amount',
    maxRefundAmount: 'Maximum refundable amount',
    refundReason: 'Refund Reason',
    pleaseSelectRefundReason: 'Please select refund reason',
    reasonUserRequest: 'User requested refund',
    reasonOrderException: 'Order exception',
    reasonOutOfStock: 'Out of stock',
    reasonOther: 'Other',
    remark: 'Refund Remark',
    pleaseInputRemarkOptional: 'Please enter refund remark (optional)',
    amountMustGtZero: 'Refund amount must be greater than 0',
    amountCannotExceedOrder: 'Refund amount cannot exceed order amount',
    confirmTitle: 'Confirm refund?',
    confirmContent: 'A refund of ¥{amount} will be sent back to the user account. This action cannot be undone.',
    submitSuccess: 'Refund request submitted',
    submitFailed: 'Refund failed'
  },
  mchStore: {
    bindAppTitle: 'App Assignment',
    pleaseSelectBindApp: 'Please select an app to bind',
    pleaseSelectApp: 'Please select app',
    emptyOption: '(None)',
    noAvailableApp: 'No available apps for this merchant',
    loadAppListFailed: 'Failed to load app list',
    confirmBindTitle: 'Confirm app binding?',
    confirmBindContent: 'Please confirm binding the selected app',
    bindAppSuccess: 'App bound successfully',
    bindAppFailed: 'Binding failed',
    onlyImageAllowed: 'Only image files are allowed!',
    imageMax2m: 'Image size cannot exceed 2MB!'
  },
  mchApp: {
    oauth2ComingSoon: 'OAuth2 configuration feature is under development...',
    payConfigComingSoon: 'Payment configuration feature is under development...',
    secretGenerated: 'Secret generated'
  },
  mch: {
    confirmDeleteMchTitle: 'Confirm deleting this merchant?',
    confirmDeleteMchContent: 'This operation will delete all merchant configurations and user information',
    randomPasswordGenerated: 'Random password: {password}'
  },
  current: {
    confirmUpdateInfoTitle: 'Confirm updating information?',
    confirmUpdatePasswordTitle: 'Confirm updating password?',
    updatePasswordNeedRelogin: 'You need to log in again after updating the password',
    editSuccessRelogin: 'Updated successfully, please log in again',
    safeWordEmpty: 'Reserved information cannot be empty',
    onlyJpgPngAllowed: 'Only JPG/PNG images are allowed!',
    imageMax10m: 'Image size cannot exceed 10MB!',
    avatarUpdated: 'Avatar updated successfully'
  },
  agTable: {
    autoRefresh: 'Auto Refresh',
    closeStatistics: 'Hide Statistics',
    statistics: 'Statistics',
    densityCompact: 'Compact',
    densityDefault: 'Default',
    densityLoose: 'Loose',
    tableDensity: 'Table Density',
    dataExport: 'Export Data',
    columnSettings: 'Column Settings',
    selectAll: 'Select All',
    visibleCount: 'Visible: {visible} / {total}',
    notFixed: 'Not Fixed',
    fixedLeft: 'Fix Left',
    fixedRight: 'Fix Right',
    quickWidth: 'Quick Width Presets',
    autoWidth: 'Auto Width',
    preset: 'Preset',
    moveUp: 'Move Up',
    moveDown: 'Move Down',
    noStatisticsData: 'No statistics data',
    totalItems: 'Total {total} items',
    resetToDefaultSuccess: 'Reset to default configuration',
    downloadNotConfigured: 'Download is not configured',
    exportTriggered: 'Export task has been triggered',
    exportFailed: 'Export failed'
  }
}

const defaultConfig = {
  views: {
    // hiddenColumns、hiddenSearchFields、hiddenFormFields、hiddenFields 均填写页面字段 key。
    // ---- 商户门店列表 ----
    mchStorePage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 商户门店新增/编辑 ----
    mchStoreAddOrEdit: { hiddenFormFields: [] },
    // ---- 商户门店详情 ----
    mchStoreDetail: { hiddenFields: [] },
    // ---- 商户应用列表 ----
    mchAppPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 商户应用新增/编辑 ----
    mchAppAddOrEdit: { hiddenFormFields: [] },
    // ---- 商户列表 ----
    mchPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 支付订单列表 ----
    payOrderPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      refundPermission: 'ENT_PAY_ORDER_REFUND',
      refundButtonVisibility: 'eligible-only',
      exportStatus: 'enabled'
    },
    // ---- 退款订单列表 ----
    refundOrderPage: { hiddenColumns: [], hiddenSearchFields: [], exportStatus: 'enabled' },
    // ---- 转账订单列表 ----
    transferOrderPage: { hiddenColumns: [], hiddenSearchFields: [], exportStatus: 'enabled' },
    // ---- 角色管理列表 ----
    sysRolePage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 系统用户列表 ----
    sysUserPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 用户邀请码弹窗 ----
    sysUserInviteCode: { showAgentMerchantIntro: true },
    // ---- 团队管理列表 ----
    sysTeamPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 代理商列表 ----
    agentPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 商户进件/申请列表 ----
    mchApplyPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 支付二维码列表 ----
    qrCodePage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 商户信息页 ----
    mchInfoPage: { hiddenFields: [] },
    // ---- 分账记录列表 ----
    divisionRecordPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 分账接收方列表 ----
    divisionReceiverPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 分账接收方详情 ----
    divisionReceiverDetail: { hiddenFields: [] },
    // ---- 分账接收组列表 ----
    divisionReceiverGroupPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 商户回调通知列表 ----
    mchNotifyPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 支付订单详情 ----
    orderDetailDrawer: { hiddenFields: [] },
    // ---- 转账订单详情 ----
    transferDetailDrawer: { hiddenFields: [] },
    // ---- 退款订单详情 ----
    refundDetailDrawer: { hiddenFields: [] },
    // ---- 回调通知详情 ----
    notifyDetailDrawer: { hiddenFields: [] },
    // ---- 商户统计列表 ----
    statisticMchPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 交易统计列表 ----
    statisticTransactionPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 代理商统计列表 ----
    statisticAgentCountPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 服务商统计列表 ----
    statisticIsvCountPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 商户进件详情 ----
    mchApplyDetail: { hiddenFields: [] },
    // ---- 对账详情 ----
    accountBillDetail: { hiddenFields: [] },
    // ---- 应用支付接口配置列表 ----
    payIfConfigListPage: { hiddenColumns: [], hiddenSearchFields: [] },
    // ---- 分账接收方新增/编辑 ----
    divisionReceiverAdd: { hiddenFormFields: [] },
    // ---- 商户新增/编辑 ----
    mchAddOrEdit: { hiddenFormFields: [] },
    // ---- 代理商新增/编辑 ----
    agentAddOrEdit: { hiddenFormFields: [] },
    // ---- 二维码绑定 ----
    qrCodeBind: { hiddenFormFields: [] },
    // ---- 商户详情 ----
    mchDetail: { hiddenFields: [] },
    // ---- 代理商详情 ----
    agentDetail: { hiddenFields: [] }
  }
}

export function createProjectConfig(overrides = {}) {
  return deepMerge(defaultConfig, overrides)
}

// 递归合并对象，保留同一视图中未被项目覆盖的默认选项。
function deepMerge(base, override) {
  const result = { ...base }
  for (const key of Object.keys(override)) {
    const baseValue = base[key]
    const overrideValue = override[key]
    result[key] = isPlainObject(baseValue) && isPlainObject(overrideValue)
      ? deepMerge(baseValue, overrideValue)
      : overrideValue
  }
  return result
}

function isPlainObject(value) {
  return value !== null && typeof value === 'object' && !Array.isArray(value)
}
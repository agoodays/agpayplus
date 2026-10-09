import { createProjectConfig } from '../../../shared/config/project-config-core'

// 只声明 Merchant 相对 shared 默认配置的差异，其余配置继承默认值。
const projectConfig = createProjectConfig({
  views: {
    // ---- 角色管理列表 ----
    // Merchant 用户只属于当前商户，无需按系统类型或主体归属筛选。
    sysRolePage: {
      hiddenColumns: ['sysType', 'belongInfoId'],
      hiddenSearchFields: ['sysType', 'belongInfoId']
    },
    // ---- 系统用户列表 ----
    // Merchant 用户只属于当前商户，无需按系统类型或主体归属筛选。
    sysUserPage: {
      hiddenColumns: ['sysType', 'belongInfoId'],
      hiddenSearchFields: ['sysType', 'belongInfoId']
    },
    // ---- 团队管理列表 ----
    // Merchant 用户只属于当前商户，无需按系统类型或主体归属筛选。
    sysTeamPage: {
      hiddenColumns: ['sysType', 'belongInfoId'],
      hiddenSearchFields: ['sysType', 'belongInfoId']
    },
    // ---- 用户邀请码弹窗 ----
    sysUserInviteCode: {
      showAgentMerchantIntro: false
    },
    // ---- 支付订单列表 ----
    // 商户端不需要支付接口筛选，也不重复显示上级主体和当前商户标识。
    payOrderPage: {
      hiddenColumns: ['ifCode', 'agentNo', 'isvNo', 'mchNo', 'mchName'],
      hiddenSearchFields: ['ifCode', 'agentNo', 'isvNo', 'mchNo']
    },
    // ---- 退款订单列表 ----
    // 商户端只处理当前商户退款，不展示商户及上级主体标识。
    refundOrderPage: {
      hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'],
      hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo']
    },
    // ---- 转账订单列表 ----
    // 商户端只处理当前商户转账，不展示商户及上级主体标识。
    transferOrderPage: {
      hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'],
      hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo']
    },
    // ---- 支付二维码列表 ----
    // 商户账号按当前商户限定数据范围，不需要代理商号或商户号筛选。
    qrCodePage: {
      hiddenColumns: ['agentNo'],
      hiddenSearchFields: ['agentNo', 'mchNo']
    },
    // ---- 商户门店列表、新增/编辑和详情 ----
    // 门店隶属当前登录商户；服务端从登录态确定商户号，页面不提供选择器或重复展示标识。
    mchStorePage: {
      hiddenColumns: ['mchNo', 'mchName'],
      hiddenSearchFields: ['mchNo']
    },
    mchStoreAddOrEdit: { hiddenFormFields: ['mchNo'] },
    mchStoreDetail: {
      hiddenFields: ['mchNo', 'mchName', 'agentNo', 'isvNo']
    },
    // ---- 商户应用列表、新增/编辑 ----
    // 应用隶属当前登录商户；服务端从登录态确定商户号，页面不提供筛选或编辑项。
    mchAppPage: {
      hiddenColumns: ['mchNo'],
      hiddenSearchFields: ['mchNo']
    },
    mchAppAddOrEdit: { hiddenFormFields: ['mchNo'] },
    // ---- 支付/退款/转账订单和通知详情 ----
    // 详情只展示当前商户业务信息，不重复展示上级主体或当前商户标识。
    orderDetailDrawer: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
    transferDetailDrawer: { hiddenFields: ['isvNo', 'mchNo', 'mchName'] },
    refundDetailDrawer: { hiddenFields: ['agentNo', 'isvNo', 'mchNo'] },
    notifyDetailDrawer: { hiddenFields: ['isvNo', 'mchNo'] },
    // ---- 商户详情 ----
    // 当前商户无需在详情中重复查看自身编号和名称。
    mchDetail: { hiddenFields: ['mchNo', 'mchName'] },
    // ---- 商户信息页 ----
    // 商户资料页隐藏固定的主体标识。
    mchInfoPage: { hiddenFields: ['isvNo', 'mchNo', 'mchName'] },
    // ---- 分账记录列表 ----
    // 分账记录已限定当前商户范围，不需要商户号搜索项。
    divisionRecordPage: { hiddenSearchFields: ['mchNo'] },
    // ---- 分账接收方列表 ----
    // 当前商户无需在接收方列表中重复筛选或查看商户号/名称。
    divisionReceiverPage: { hiddenSearchFields: ['mchNo'], hiddenColumns: ['mchNo', 'mchName'] },
    divisionReceiverGroupPage: { hiddenSearchFields: ['mchNo'], hiddenColumns: ['mchNo', 'mchName'] },
    // ---- 分账接收方详情 ----
    divisionReceiverDetail: { hiddenFields: ['isvNo', 'mchNo'] },
    // ---- 商户通知列表 ----
    mchNotifyPage: { hiddenSearchFields: ['isvNo'] },
    // ---- 代理商/服务商统计列表 ----
    // 商户通常不会访问这些主体统计页面，保留隐藏项以避免暴露主体编号。
    statisticAgentCountPage: { hiddenSearchFields: ['agentNo', 'isvNo'], hiddenColumns: ['agentNo'] },
    statisticIsvCountPage: { hiddenSearchFields: ['isvNo'], hiddenColumns: ['isvNo'] },
    // ---- 商户进件详情 ----
    mchApplyDetail: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
    // ---- 对账详情 ----
    accountBillDetail: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
    // ---- 应用支付接口配置列表 ----
    payIfConfigListPage: {
      hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo'],
      hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName']
    },
    // ---- 分账接收方新增/编辑 ----
    divisionReceiverAdd: { hiddenFormFields: ['mchNo'] },
    // ---- 商户新增/编辑 ----
    mchAddOrEdit: { hiddenFormFields: ['agentNo', 'isvNo'] },
    // ---- 代理商新增/编辑 ----
    agentAddOrEdit: { hiddenFormFields: ['agentNo', 'isvNo'] },
    // ---- 二维码绑定 ----
    qrCodeBind: { hiddenFormFields: ['agentNo', 'mchNo'] }
  }
})

export function getProjectConfig() {
  return projectConfig
}

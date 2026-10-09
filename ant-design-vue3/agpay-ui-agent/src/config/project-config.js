import { createProjectConfig } from '../../../shared/config/project-config-core'

// 只声明 Agent 相对 shared 默认配置的差异，其余配置继承默认值。
const projectConfig = createProjectConfig({
  views: {
    // ---- 角色管理列表 ----
    // Agent 用户归属当前代理商，无需按系统类型或主体归属筛选。
    sysRolePage: {
      hiddenColumns: ['sysType', 'belongInfoId'],
      hiddenSearchFields: ['sysType', 'belongInfoId']
    },
    // ---- 系统用户列表 ----
    // Agent 用户归属当前代理商，无需按系统类型或主体归属筛选。
    sysUserPage: {
      hiddenColumns: ['sysType', 'belongInfoId'],
      hiddenSearchFields: ['sysType', 'belongInfoId']
    },
    // ---- 团队管理列表 ----
    // Agent 用户归属当前代理商，无需按系统类型或主体归属筛选。
    sysTeamPage: {
      hiddenColumns: ['sysType', 'belongInfoId'],
      hiddenSearchFields: ['sysType', 'belongInfoId']
    },
    // ---- 用户邀请码弹窗 ----
    // Agent 内邀请码默认绑定当前代理商，不展示代理商/商户绑定说明。
    sysUserInviteCode: {
      showAgentMerchantIntro: false
    },
    // ---- 商户列表 ----
    // Agent 已是当前列表上下文，不重复展示代理商/服务商编号。
    mchPage: {
      hiddenColumns: ['agentNo', 'isvNo'],
      hiddenSearchFields: ['agentNo', 'isvNo']
    },
    // ---- 支付订单列表 ----
    // Agent 查看自身范围订单时不重复展示代理商编号。
    payOrderPage: {
      hiddenColumns: ['agentNo'],
      hiddenSearchFields: ['agentNo']
    },
    // ---- 退款订单列表 ----
    // Agent 查看自身范围退款时不重复展示代理商编号。
    refundOrderPage: {
      hiddenColumns: ['agentNo'],
      hiddenSearchFields: ['agentNo']
    },
    // ---- 转账订单列表 ----
    // Agent 查看自身范围转账时不重复展示代理商编号。
    transferOrderPage: {
      hiddenColumns: ['agentNo'],
      hiddenSearchFields: ['agentNo']
    },
    // ---- 支付二维码列表 ----
    // Agent 上下文无需再次筛选代理商或商户编号。
    qrCodePage: {
      hiddenColumns: ['agentNo'],
      hiddenSearchFields: ['agentNo', 'mchNo']
    },
    // ---- 商户进件/申请列表 ----
    // 代理商提交商户申请时不需要展示服务商名称。
    mchApplyPage: {
      hiddenColumns: ['isvName']
    },
    // ---- 商户门店列表 ----
    // 隐藏门店列表中真实存在的商户号/名称列及商户号搜索项。
    mchStorePage: {
      hiddenColumns: ['mchNo', 'mchName'],
      hiddenSearchFields: ['mchNo']
    },
    // ---- 商户门店详情 ----
    // 门店详情仅保留门店自身信息，不展示商户身份及其上级主体。
    mchStoreDetail: {
      hiddenFields: ['mchNo', 'mchName']
    },
    // ---- 商户详情 ----
    // 隐藏商户所属代理商/服务商的编号和名称，保留商户自身信息。
    mchDetail: { hiddenFields: ['agentNo', 'agentName', 'isvNo', 'isvName'] },
    // ---- 订单详情 ----
    // Agent 上下文已明确数据归属，无需再次展示代理商编号。
    orderDetailDrawer: { hiddenFields: ['agentNo'] },
    refundDetailDrawer: { hiddenFields: ['agentNo'] },
    // ---- 代理商详情 ----
    // 代理商查看自身资料时不重复展示代理商编号。
    agentDetail: { hiddenFields: ['agentNo'] },
    // ---- 商户通知列表 ----
    // 当前页面保留真实存在的服务商号搜索项，并按 Agent 场景隐藏。
    mchNotifyPage: { hiddenSearchFields: ['isvNo'] },
    // ---- 代理商/服务商统计列表 ----
    // 代理商统计不需要额外按服务商编号筛选。
    statisticAgentCountPage: { hiddenSearchFields: ['isvNo'] },
    statisticIsvCountPage: { hiddenSearchFields: ['isvNo'], hiddenColumns: ['isvNo'] },
    // ---- 商户进件详情 ----
    // 详情不重复展示代理商和服务商编号。
    mchApplyDetail: { hiddenFields: ['agentNo', 'isvNo'] },
    // ---- 对账详情 ----
    // 详情不重复展示代理商和服务商编号。
    accountBillDetail: { hiddenFields: ['agentNo', 'isvNo'] },
    // ---- 应用支付接口配置列表 ----
    // Agent 上下文不需要代理商号筛选和列。
    payIfConfigListPage: { hiddenSearchFields: ['agentNo'], hiddenColumns: ['agentNo'] },
    // ---- 分账接收方新增/编辑 ----
    divisionReceiverAdd: { hiddenFormFields: ['mchNo'] },
    // ---- 商户新增/编辑 ----
    // Agent 代管商户时无需选择代理商或服务商归属。
    mchAddOrEdit: { hiddenFormFields: ['agentNo', 'isvNo'] },
    // ---- 代理商新增/编辑 ----
    // 新代理商无需在当前 Agent 系统中选择服务商。
    agentAddOrEdit: { hiddenFormFields: ['isvNo'] },
    // ---- 二维码绑定 ----
    qrCodeBind: { hiddenFormFields: ['agentNo', 'mchNo'] }
  }
})

export function getProjectConfig() {
  return projectConfig
}

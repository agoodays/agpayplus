/**
 * 项目差异配置（Project Overrides）
 *
 * 三项目共享同一套视图组件，但个别页面存在细微差异。
 * 这些差异统一在本文件声明，视图通过 __APP_ID__（Vite define 注入）读取当前项目的覆盖配置。
 *
 * 使用方式：
 *   import { getProjectConfig } from '@/config/project-config'  // shared 内部用相对路径
 *   const cfg = getProjectConfig(__APP_ID__)
 *   const visibleColumns = ALL_COLUMNS.filter(c => !cfg.views.payOrderPage.hiddenColumns.includes(c.key))
 *
 * 新增项目差异时：
 *   1. 在 defaultConfig 的 views.<页面key> 下加默认值
 *   2. 在 projectOverrides.<appId> 下只写与默认值不同的字段
 */

const defaultConfig = {
  views: {
    // ---- 商户门店列表 ----
    mchStorePage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 商户门店详情 ----
    mchStoreDetail: {
      hiddenFields: [] // descriptions-item 不显示的字段
    },
    // ---- 商户列表 ----
    mchPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: [],
      // advancedConfig 点击行为：'drawer'（打开抽屉，agent/merchant）| 'route'（路由跳转，manager）
      advancedConfigMode: 'drawer',
      advancedConfigRoutePath: '/mchAdvancedConfig' // manager 模式下跳转的路径
    },
    // ---- 支付订单列表 ----
    payOrderPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: [],
      // 是否显示"支付接口"搜索条件和表格列（merchant 不需要）
      showPayInterfaceFilter: true,
      // 退款按钮权限点（为空表示不做 v-if 权限检查）
      refundPermission: 'ENT_PAY_ORDER_REFUND',
      // 退款按钮可见性 v-show 条件（为空表示不做 v-show 限制）
      refundVisibleWhen: "record.state === 2 && record.refundState !== 2",
      // 导出功能：'enabled' | 'disabled'（禁用时显示"开发中"提示）
      exportStatus: 'enabled'
    },
    // ---- 退款订单列表 ----
    refundOrderPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: [],
      exportStatus: 'enabled'
    },
    // ---- 转账订单列表 ----
    transferOrderPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: [],
      exportStatus: 'enabled'
    },
    // ---- 角色管理列表 ----
    sysRolePage: {
      hiddenColumns: [],        // 'sysType' | 'belongInfoId'
      hiddenSearchFields: [],   // 'sysType' | 'belongInfoId'
      hiddenActions: [],        // 'edit' | 'delete'
      // 新增/编辑时表单内隐藏字段（add-or-edit 中如果有的话，这里作为默认值）
      hiddenFormFields: []
    },
    // ---- 系统用户列表 ----
    sysUserPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: [],
      hiddenFormFields: []
    },
    // ---- 用户邀请码弹窗（sysUser 内） ----
    sysUserInviteCode: {
      // 是否展示"邀请码绑定代理商/商户"说明（当 sysType 非 MCH 时）
      showAgentMerchantIntro: true
    },
    // ---- 角色分配弹窗（sysUser 内） ----
    sysUserRoleDist: {
      hiddenFormFields: []
    },
    // ---- 用户新增/编辑（sysUser 内） ----
    sysUserAddOrEdit: {
      hiddenFormFields: []
    },
    // ---- 团队管理列表 ----
    sysTeamPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: [],
      hiddenFormFields: []
    },
    // ---- 团队详情（Descriptions） ----
    sysTeamDetail: {
      hiddenFields: []  // 'sysType' | 'belongInfoId'
    },
    // ---- 代理商列表 ----
    agentPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 商户进件/申请列表 ----
    mchApplyPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 支付二维码(QR Code)列表 ----
    qrCodePage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 服务商(ISV)列表 ----
    isvPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 商户应用列表 ----
    mchAppPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 商户信息页(Descriptions + 表单, mch-info-page) ----
    mchInfoPage: {
      hiddenFields: []  // 'isvNo' | 'agentNo'
    },
    // ---- 分账: 分账记录列表 ----
    divisionRecordPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 分账: 分账接收方列表 ----
    divisionReceiverPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 分账: 分账接收方详情(Descriptions) ----
    divisionReceiverDetail: {
      hiddenFields: []
    },
    // ---- 分账: 分账接收组列表 ----
    divisionReceiverGroupPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 回调通知: 商户通知列表 ----
    mchNotifyPage: {
      hiddenColumns: [],
      hiddenSearchFields: [],
      hiddenActions: []
    },
    // ---- 订单详情抽屉(Descriptions) ----
    orderDetailDrawer: {
      hiddenFields: []  // 'agentNo' | 'isvNo' | 'mchNo' | 'mchName'
    },
    // ---- 转账订单详情抽屉(Descriptions) ----
    transferDetailDrawer: {
      hiddenFields: []
    },
    // ---- 退款订单详情抽屉(Descriptions) ----
    refundDetailDrawer: {
      hiddenFields: []
    },
    // ---- 回调通知详情抽屉(Descriptions) ----
    notifyDetailDrawer: {
      hiddenFields: []
    },
    // ---- 统计: 代理商统计 ----
    statisticAgentPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 统计: 商户统计 ----
    statisticMchPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 统计: 服务商统计 ----
    statisticIsvPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 统计: 交易统计 ----
    statisticTransactionPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 统计: 代理商统计(列表) ----
    statisticAgentCountPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 统计: 服务商统计(列表) ----
    statisticIsvCountPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 商户进件详情(Descriptions) ----
    mchApplyDetail: {
      hiddenFields: []  // 'agentNo' | 'isvNo'
    },
    // ---- 对账详情(Descriptions) ----
    accountBillDetail: {
      hiddenFields: []  // 'agentNo' | 'isvNo' | 'mchNo'
    },
    // ---- 应用权限点列表(isv-pay-if-config-list.vue / mch-pay-if-config-list.vue) ----
    payIfConfigListPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 二维码外壳列表(qr-shell/qr-code-shell-page.vue) ----
    qrCodeShellPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    // ---- 企业(ent)列表 & 表单 ----
    sysEntPage: {
      hiddenColumns: [],
      hiddenSearchFields: []
    },
    sysEntAddOrEdit: {
      hiddenFormFields: []
    },
    // ---- 角色表单(sys/role/add-or-edit.vue & role-dist.vue) ----
    sysRoleAddOrEdit: {
      hiddenFormFields: []  // 'sysType' | 'belongInfoId'
    },
    sysRoleRoleDist: {
      hiddenFormFields: []  // 'sysType' | 'belongInfoId'
    },
    // ---- 角色管理里角色分配(sys/user/role-dist.vue) ----
    sysUserRoleDist: {
      hiddenFormFields: []  // 冗余 key 已声明
    },
    // ---- 分账:分组/接收方 表单 ----
    divisionGroupAddOrEdit: {
      hiddenFormFields: []  // 'mchNo'
    },
    divisionReceiverAdd: {
      hiddenFormFields: []  // 'mchNo' | 'agentNo'
    },
    // ---- 实体新增/编辑:商户/门店/代理商/商户应用 ----
    mchAddOrEdit: {
      hiddenFormFields: []  // 'agentNo' | 'isvNo' | 'mchNo'
    },
    mchStoreAddOrEdit: {
      hiddenFormFields: []  // 'agentNo' | 'isvNo' | 'mchNo'
    },
    agentAddOrEdit: {
      hiddenFormFields: []  // 'isvNo'
    },
    mchAppAddOrEdit: {
      hiddenFormFields: []  // 'agentNo' | 'isvNo' | 'mchNo'
    },
    // ---- 二维码绑定 ----
    qrCodeBind: {
      hiddenFormFields: []  // 'agentNo' | 'mchNo'
    },
    // ---- 商户详情(Descriptions) ----
    mchDetail: {
      hiddenFields: []  // 'agentNo' | 'isvNo'
    },
    // ---- 代理商详情(Descriptions) ----
    agentDetail: {
      hiddenFields: []  // 'isvNo'
    }
  }
}

/**
 * 各项目覆盖默认配置
 * 只写与 defaultConfig 不同的字段，其余继承 defaultConfig
 */
const projectOverrides = {
  agent: {
    views: {
      // Agent 系统中:用户只属于当前 Agent 系统,无需"所属系统"和"所属代理商"筛选
      sysRolePage: {
        hiddenColumns: ['sysType', 'belongInfoId'],
        hiddenSearchFields: ['sysType', 'belongInfoId']
      },
      sysUserPage: {
        hiddenColumns: ['sysType', 'belongInfoId'],
        hiddenSearchFields: ['sysType', 'belongInfoId']
      },
      sysTeamPage: {
        hiddenColumns: ['sysType', 'belongInfoId'],
        hiddenSearchFields: ['sysType', 'belongInfoId']
      },
      sysTeamDetail: {
        hiddenFields: ['sysType', 'belongInfoId']
      },
      sysUserInviteCode: {
        // Agent 内邀请码必然绑定当前 Agent,不需要展示"绑定代理商/商户"文字
        showAgentMerchantIntro: false
      },
      // Agent 自身就是代理商,商户列表中"代理商号"是多余
      mchPage: {
        hiddenColumns: ['agentNo'],
        hiddenSearchFields: ['agentNo']
      },
      // Agent 看订单:自己的上下文,代理商号/服务商号一般不需要
      payOrderPage: {
        hiddenColumns: ['agentNo'],
        hiddenSearchFields: ['agentNo']
      },
      refundOrderPage: {
        hiddenColumns: ['agentNo'],
        hiddenSearchFields: ['agentNo']
      },
      transferOrderPage: {
        hiddenColumns: ['agentNo'],
        hiddenSearchFields: ['agentNo']
      },
      qrCodePage: {
        hiddenColumns: ['agentNo'],
        hiddenSearchFields: ['agentNo', 'mchNo']
      },
      // Agent 申请商户:服务商字段可能不需要(本项目仅二级代理架构下隐藏)
      mchApplyPage: {
        hiddenColumns: ['isvName']
        // 注:当前版本页面没有 isvNo 搜索项,不再写 hiddenSearchFields: ['isvNo'] 避免"配置写了但 UI 不变"的假配置
      },
      // Agent 侧门店:真实存在且应隐藏的字段(仅 mchNo/mchName 列、mchNo 搜索)
      mchStorePage: {
        hiddenColumns: ['mchNo', 'mchName'],
        hiddenSearchFields: ['mchNo']
      },
      mchStoreDetail: {
        hiddenFields: ['mchNo', 'mchName']
      },
      // Agent: 所有详情抽屉隐藏 agentNo
      orderDetailDrawer: { hiddenFields: ['agentNo'] },
      transferDetailDrawer: { hiddenFields: ['agentNo'] },
      refundDetailDrawer: { hiddenFields: ['agentNo'] },
      notifyDetailDrawer: { hiddenFields: ['agentNo'] },
      // 商户详情: 代理商号固定
      mchDetail: { hiddenFields: ['agentNo'] },
      // 代理商详情: 自己不用看自己的代理商号
      agentDetail: { hiddenFields: ['agentNo'] },
      // 商户信息页: 代理商号固定
      mchInfoPage: { hiddenFields: ['agentNo'] },
      // 分账(3 列表页:当前页面 columns 中无 agentNo/isvNo/mchNo/mchName 独立列,搜索区也没有对应搜索项,不再写假 hidden;只在 receiver detail 保留隐藏逻辑)
      divisionReceiverDetail: { hiddenFields: [] },
      // 回调通知:页面搜索含 isvNo,表格无 agentNo/isvNo/mchNo/mchName 列,故只保留 isvNo + isvNo 列(如未来新增)
      mchNotifyPage: { hiddenSearchFields: ['isvNo'] },
      // 商户应用:真实仅 mchNo 列存在;mchNo 搜索存在,但 Agent 侧场景下商户号搜索/列都应隐藏
      mchAppPage: { hiddenSearchFields: ['mchNo'], hiddenColumns: ['mchNo'] },
      // ============ S5 新增接入 ============
      // 统计:代理商/服务商统计列表
      statisticAgentCountPage: { hiddenSearchFields: ['isvNo'], hiddenColumns: ['isvNo'] },
      statisticIsvCountPage:   { hiddenSearchFields: ['isvNo'], hiddenColumns: ['isvNo'] },
      // 商户进件详情
      mchApplyDetail: { hiddenFields: ['agentNo', 'isvNo'] },
      // 对账详情
      accountBillDetail: { hiddenFields: ['agentNo', 'isvNo'] },
      // 应用权限点列表
      payIfConfigListPage: { hiddenSearchFields: ['agentNo'], hiddenColumns: ['agentNo'] },
      // 二维码外壳列表
      qrCodeShellPage: { hiddenSearchFields: ['mchNo', 'isvNo'], hiddenColumns: ['mchNo', 'mchName', 'isvNo'] },
      // 企业(ent)
      sysEntPage:     { hiddenSearchFields: ['agentNo', 'isvNo'], hiddenColumns: ['agentNo', 'isvNo'] },
      sysEntAddOrEdit:{ hiddenFormFields: ['agentNo', 'isvNo'] },
      // 角色表单
      sysRoleAddOrEdit:   { hiddenFormFields: ['sysType', 'belongInfoId'] },
      sysRoleRoleDist:    { hiddenFormFields: ['sysType'] },
      sysUserAddOrEdit:   { hiddenFormFields: ['sysType', 'belongInfoId'] },
      sysUserRoleDist:    { hiddenFormFields: ['sysType'] },
      // 分账:分组/接收人表单
      divisionGroupAddOrEdit: { hiddenFormFields: ['mchNo'] },
      divisionReceiverAdd:    { hiddenFormFields: ['mchNo', 'agentNo'] },
      // 商户/门店/代理商/商户应用 新增编辑
      mchAddOrEdit:       { hiddenFormFields: ['agentNo', 'isvNo'] },
      mchStoreAddOrEdit:  { hiddenFormFields: ['agentNo', 'isvNo', 'mchNo'] },
      agentAddOrEdit:     { hiddenFormFields: ['isvNo'] },
      mchAppAddOrEdit:    { hiddenFormFields: ['agentNo', 'isvNo'] },
      // 二维码绑定
      qrCodeBind:         { hiddenFormFields: ['agentNo', 'mchNo'] }
    }
  },
  merchant: {
    views: {
      // Merchant 系统中:用户只属于当前 Merchant 系统,无需"所属系统"和"所属商户"筛选
      sysRolePage: {
        hiddenColumns: ['sysType', 'belongInfoId'],
        hiddenSearchFields: ['sysType', 'belongInfoId']
      },
      sysUserPage: {
        hiddenColumns: ['sysType', 'belongInfoId'],
        hiddenSearchFields: ['sysType', 'belongInfoId']
      },
      sysTeamPage: {
        hiddenColumns: ['sysType', 'belongInfoId'],
        hiddenSearchFields: ['sysType', 'belongInfoId']
      },
      sysTeamDetail: {
        hiddenFields: ['sysType', 'belongInfoId']
      },
      sysUserInviteCode: {
        showAgentMerchantIntro: false
      },
      // 支付订单:商户侧隐藏"支付接口"(重复/无意义)、代理商号、服务商号、商户号/名
      payOrderPage: {
        showPayInterfaceFilter: false,
        hiddenColumns: ['ifCode', 'agentNo', 'isvNo', 'mchNo', 'mchName'],
        hiddenSearchFields: ['ifCode', 'agentNo', 'isvNo', 'mchNo']
      },
      refundOrderPage: {
        hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'],
        hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo']
      },
      transferOrderPage: {
        hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'],
        hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo']
      },
      qrCodePage: {
        hiddenColumns: ['agentNo'],
        hiddenSearchFields: ['agentNo', 'mchNo']
      },
      // 商户看门店:商户号/商户名固定(页面不存在 agentNo/isvNo 独立列/搜索项,不再写假 hidden)
      mchStorePage: {
        hiddenColumns: ['mchNo', 'mchName'],
        hiddenSearchFields: ['mchNo']
      },
      mchStoreDetail: {
        hiddenFields: ['mchNo', 'agentNo', 'isvNo']
      },
      // 详情抽屉: 商户号/名、代理商号、服务商号 均固定
      orderDetailDrawer: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      transferDetailDrawer: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      refundDetailDrawer: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      notifyDetailDrawer: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      mchDetail: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      // 商户信息页: 信息固定
      mchInfoPage: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      // 商户进件: 商户不可见,但保持默认不额外配置
      // 分账(3 列表页 + 详情):仅保留页面真实存在的维度,删除 agentNo/isvNo 等无对映 UI 的假 hidden
      divisionRecordPage: {
        hiddenSearchFields: ['mchNo']
        // 注:当前页面 columns 中没有 agentNo/isvNo/mchNo/mchName 独立列,不再写假 hiddenColumns
      },
      divisionReceiverPage: {
        hiddenSearchFields: ['mchNo'],
        hiddenColumns: ['mchNo', 'mchName']
      },
      divisionReceiverGroupPage: {
        hiddenSearchFields: ['mchNo'],
        hiddenColumns: ['mchNo', 'mchName']
      },
      divisionReceiverDetail: { hiddenFields: ['isvNo', 'mchNo'] },
      // 回调通知:仅高级搜索含 isvNo;表格无 agentNo/isvNo/mchNo/mchName 独立列
      mchNotifyPage: {
        hiddenSearchFields: ['isvNo']
      },
      // 商户应用:真实仅 mchNo 列存在;mchNo 搜索存在
      mchAppPage: {
        hiddenSearchFields: ['mchNo'],
        hiddenColumns: ['mchNo']
      },
      // ============ S5 新增接入 ============
      // 统计:代理商/服务商统计列表(商户一般不会进入对应模块,只写保险值)
      statisticAgentCountPage: { hiddenSearchFields: ['agentNo', 'isvNo'], hiddenColumns: ['agentNo', 'isvNo'] },
      statisticIsvCountPage:   { hiddenSearchFields: ['isvNo'], hiddenColumns: ['isvNo'] },
      // 商户进件详情
      mchApplyDetail: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      // 对账详情
      accountBillDetail: { hiddenFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      // 应用权限点列表
      payIfConfigListPage: { hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo'], hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      // 二维码外壳列表
      qrCodeShellPage: { hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo'], hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      // 企业(ent)
      sysEntPage:     { hiddenSearchFields: ['agentNo', 'isvNo', 'mchNo'], hiddenColumns: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      sysEntAddOrEdit:{ hiddenFormFields: ['agentNo', 'isvNo', 'mchNo'] },
      // 角色表单
      sysRoleAddOrEdit:   { hiddenFormFields: ['sysType', 'belongInfoId'] },
      sysRoleRoleDist:    { hiddenFormFields: ['sysType'] },
      sysUserAddOrEdit:   { hiddenFormFields: ['sysType', 'belongInfoId'] },
      sysUserRoleDist:    { hiddenFormFields: ['sysType'] },
      // 分账:分组/接收人表单
      divisionGroupAddOrEdit: { hiddenFormFields: ['mchNo'] },
      divisionReceiverAdd:    { hiddenFormFields: ['mchNo', 'agentNo'] },
      // 商户/门店/代理商/商户应用 新增编辑
      mchAddOrEdit:       { hiddenFormFields: ['agentNo', 'isvNo', 'mchNo', 'mchName'] },
      mchStoreAddOrEdit:  { hiddenFormFields: ['agentNo', 'isvNo', 'mchNo'] },
      agentAddOrEdit:     { hiddenFormFields: ['agentNo', 'isvNo'] },
      mchAppAddOrEdit:    { hiddenFormFields: ['agentNo', 'isvNo', 'mchNo'] },
      // 二维码绑定
      qrCodeBind:         { hiddenFormFields: ['agentNo', 'mchNo'] }
    }
  },
  manager: {
    views: {
      mchStoreDetail: {
        hiddenFields: ['agentNo', 'isvNo']
      },
      mchPage: {
        advancedConfigMode: 'route'
      },
      payOrderPage: {
        refundPermission: '',
        refundVisibleWhen: '',
        exportStatus: 'disabled'
      },
      refundOrderPage: {
        exportStatus: 'disabled'
      },
      transferOrderPage: {
        exportStatus: 'disabled'
      },
      // Manager 通常显示完整信息,此处保留空覆盖(便于后续扩展)
      mchInfoPage: { hiddenFields: [] }
    }
  }
}

/**
 * 获取当前项目的完整配置
 * @param {'agent'|'manager'|'merchant'} appId - 由 Vite define 注入的 __APP_ID__
 * @returns {Object} 合并后的项目配置
 */
export function getProjectConfig(appId) {
  const override = projectOverrides[appId] || {}
  return deepMerge(defaultConfig, override)
}

/**
 * 深合并（只合并 plain object，数组直接覆盖）
 */
function deepMerge(base, override) {
  const result = { ...base }
  for (const key of Object.keys(override)) {
    const bv = base[key]
    const ov = override[key]
    if (isPlainObject(bv) && isPlainObject(ov)) {
      result[key] = deepMerge(bv, ov)
    } else {
      result[key] = ov
    }
  }
  return result
}

function isPlainObject(v) {
  return v !== null && typeof v === 'object' && !Array.isArray(v)
}

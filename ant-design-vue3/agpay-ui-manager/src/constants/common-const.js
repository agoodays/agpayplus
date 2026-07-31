/*
 * 通用常量
 */

export const PAGE_SIZE = 10
export const PAGE_SIZE_OPTIONS = ['5', '10', '15', '20', '30', '40', '50', '75', '100', '150', '200', '300', '500']

// 路由路径
export const PAGE_PATH_LOGIN = '/login'
export const PAGE_PATH_404 = '/404'

// 分页总数显示 (需传入 t 函数)
export const showTableTotal = (t, total) => {
  return t('pagination.total', { total })
}

// ================= 枚举定义 (使用 descKey) =================

export const FLAG_NUMBER_ENUM = {
  TRUE: { value: 1, descKey: 'common.yes' },
  FALSE: { value: 0, descKey: 'common.no' }
}

export const GENDER_ENUM = {
  UNKNOWN: { value: 0, descKey: 'common.gender.unknown' },
  MAN: { value: 1, descKey: 'common.gender.man' },
  WOMAN: { value: 2, descKey: 'common.gender.woman' }
}

export const USER_TYPE_ENUM = {
  ADMIN_EMPLOYEE: { value: 1, descKey: 'common.userType.employee' }
}

export const DATA_TYPE_ENUM = {
  NORMAL: { value: 1, descKey: 'common.dataType.normal' },
  ENCRYPT: { value: 10, descKey: 'common.dataType.encrypt' }
}

export const STAT_RANGE_TYPE_ENUM = {
  YEAR: { value: 'year', descKey: 'common.statRange.year' },
  QUARTER: { value: 'quarter', descKey: 'common.statRange.quarter' },
  MONTH: { value: 'month', descKey: 'common.statRange.month' },
  WEEK: { value: 'week', descKey: 'common.statRange.week' }
}

export const SYS_TYPE_ENUM = {
  MGR: { value: 'MGR', descKey: 'common.sysType.mgr' },
  AGENT: { value: 'AGENT', descKey: 'common.sysType.agent' },
  MCH: { value: 'MCH', descKey: 'common.sysType.mch' }
}

export const STATE_ENUM = {
  DISABLED: { value: 0, descKey: 'common.state.disabled', color: 'volcano' },
  ENABLED: { value: 1, descKey: 'common.state.enabled', color: 'green' }
}

export const FLAG_ENUM = {
  NO: { value: 0, descKey: 'common.no', color: 'volcano' },
  YES: { value: 1, descKey: 'common.yes', color: 'green' }
}

export const OPEN_STATUS_ENUM = {
  OPEN: { value: 1, descKey: 'common.open', color: 'green' },   // 开启显示绿色
  CLOSED: { value: 0, descKey: 'common.closed', color: 'volcano' } // 关闭显示红色
}

// 是否支持状态枚举（如：支持对账、支持退款等）
export const SUPPORT_STATUS_ENUM = {
  SUPPORT: { value: 1, descKey: 'common.support', color: 'green' },      // 支持
  NOT_SUPPORT: { value: 0, descKey: 'common.notSupport', color: 'volcano' } // 不支持
}

export const AGENT_TYPE_ENUM = {
  INDIVIDUAL: { value: 1, descKey: 'common.agentType.individual', labelKey: 'common.label.legalPerson' },
  ENTERPRISE: { value: 2, descKey: 'common.agentType.enterprise', labelKey: 'common.label.contactPerson' }
}

export const SETT_ACCOUNT_TYPE_ENUM = {
  WX_CASH: { value: 'WX_CASH', descKey: 'common.settAccount.wx', labelKey: 'common.settAccount.wxNo' },
  ALIPAY_CASH: { value: 'ALIPAY_CASH', descKey: 'common.settAccount.alipay', labelKey: 'common.settAccount.alipayNo' },
  BANK_PRIVATE: { value: 'BANK_PRIVATE', descKey: 'common.settAccount.private', labelKey: 'common.settAccount.privateNo' },
  BANK_PUBLIC: { value: 'BANK_PUBLIC', descKey: 'common.settAccount.public', labelKey: 'common.settAccount.publicNo' }
}

export const MCH_TYPE_ENUM = {
  NORMAL: { value: 1, descKey: 'common.mchType.normal', color: 'green' },
  SPECIAL: { value: 2, descKey: 'common.mchType.special', color: 'orange' }
}

export const MCH_LEVEL_ENUM = {
  M0: { value: 'M0', descKey: 'common.mchLevel.m0', tipsKey: 'common.mchLevel.m0Tips' },
  M1: { value: 'M1', descKey: 'common.mchLevel.m1', tipsKey: 'common.mchLevel.m1Tips' }
}

export const REFUND_MODE_ENUM = {
  PLAT: { value: 'plat', descKey: 'common.refundMode.plat', color: 'blue' },
  API: { value: 'api', descKey: 'common.refundMode.api', color: 'green' }
}

export const CASH_OUT_FEE_TYPE_ENUM = {
  FIX: { value: 'FIX', descKey: 'common.cashOutFee.fix' },
  SINGLE: { value: 'SINGLE', descKey: 'common.cashOutFee.single' },
  FIXANDRATE: { value: 'FIXANDRATE', descKey: 'common.cashOutFee.fixAndRate' }
}

// ================= 动态选项生成函数 (接收 t) =================

const generateOptions = (enumObj, t) => {
  return Object.values(enumObj).map(item => ({
    value: item.value,
    label: t(item.descKey)
  }))
}

export const getStatRangeTypeOptions = (t) => generateOptions(STAT_RANGE_TYPE_ENUM, t)
export const getSysTypeOptions = (t) => generateOptions(SYS_TYPE_ENUM, t)
export const getStateOptions = (t) => generateOptions(STATE_ENUM, t)
export const getFlagOptions = (t) => generateOptions(FLAG_ENUM, t)
export const getOpenStatusOptions = (t) => generateOptions(OPEN_STATUS_ENUM, t)
export const getSupportStatusOptions = (t) => generateOptions(SUPPORT_STATUS_ENUM, t)
export const getAgentTypeOptions = (t) => generateOptions(AGENT_TYPE_ENUM, t)
export const getSettAccountTypeOptions = (t) => generateOptions(SETT_ACCOUNT_TYPE_ENUM, t)
export const getMchTypeOptions = (t) => generateOptions(MCH_TYPE_ENUM, t)
export const getMchLevelOptions = (t) => generateOptions(MCH_LEVEL_ENUM, t)
export const getRefundModeOptions = (t) => generateOptions(REFUND_MODE_ENUM, t)
export const getCashOutFeeTypeOptions = (t) => generateOptions(CASH_OUT_FEE_TYPE_ENUM, t)
export const getUserTypeOptions = (t) => generateOptions(USER_TYPE_ENUM, t)

// ================= 状态获取工具函数 =================
/**
 * 通用枚举信息获取函数
 * @param {object} enumObj - 枚举对象 (如 STATE_ENUM)
 * @param {number|string} value - 当前状态值
 * @param {function} t - i18n 翻译函数
 */
export const getEnumInfo = (enumObj, value, t) => {
  // 遍历枚举找到匹配项，找不到则取第一个作为兜底
  const current = Object.values(enumObj).find(item => item.value === value) || Object.values(enumObj)[0]

  // 基础返回对象（包含所有原始属性，如 value, color, tipsKey 等）
  const result = {
    ...current,
    desc: t(current.descKey), // descKey 是必须存在的
    text: t(current.descKey)
  }

  // 只有当存在 color 时，才映射给 status（兼容 a-badge / a-tag）
  if (current.color) {
    result.status = current.color
  }

  // 只有当存在 labelKey 时，才进行翻译并挂载
  if (current.labelKey) {
    result.label = t(current.labelKey)
  }

  return result
}

// 使用时极其简洁：
// const stateInfo = getEnumInfo(STATE_ENUM, record.state, t)
// const mchInfo = getEnumInfo(MCH_TYPE_ENUM, record.mchType, t)

export const getFlagInfo = (flag, t) => getEnumInfo(FLAG_ENUM, flag, t)
export const getStateInfo = (state, t) => getEnumInfo(STATE_ENUM, state, t)
export const getOpenStatusInfo = (openState, t) => getEnumInfo(OPEN_STATUS_ENUM, openState, t)
export const getSupportStatusInfo = (supportState, t) => getEnumInfo(SUPPORT_STATUS_ENUM, supportState, t)
export const getAgentTypeInfo = (type, t) => getEnumInfo(AGENT_TYPE_ENUM, type, t)
export const getSettAccountTypeInfo = (type, t) => getEnumInfo(SETT_ACCOUNT_TYPE_ENUM, type, t)
export const getMchTypeInfo = (type, t) => getEnumInfo(MCH_TYPE_ENUM, type, t)
export const getMchLevelInfo = (level, t) => getEnumInfo(MCH_LEVEL_ENUM, level, t)
export const getRefundModeInfo = (mode, t) => getEnumInfo(REFUND_MODE_ENUM, mode, t)
export const getCashOutFeeTypeInfo = (type, t) => getEnumInfo(CASH_OUT_FEE_TYPE_ENUM, type, t)

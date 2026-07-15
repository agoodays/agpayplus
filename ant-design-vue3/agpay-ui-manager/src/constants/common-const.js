/*
 * 通用常量
 *
 */

export const PAGE_SIZE = 10

export const PAGE_SIZE_OPTIONS = ['5', '10', '15', '20', '30', '40', '50', '75', '100', '150', '200', '300', '500']

//登录页面名字
export const PAGE_PATH_LOGIN = '/login'

//404页面名字
export const PAGE_PATH_404 = '/404'

export const showTableTotal = function (total) {
  return `共${total}条`
}

export const FLAG_NUMBER_ENUM = {
  TRUE: {
    value: 1,
    desc: '是'
  },
  FALSE: {
    value: 0,
    desc: '否'
  }
}

export const GENDER_ENUM = {
  UNKNOWN: {
    value: 0,
    desc: '未知'
  },
  MAN: {
    value: 1,
    desc: '男'
  },
  WOMAN: {
    value: 2,
    desc: '女'
  }
}

export const USER_TYPE_ENUM = {
  ADMIN_EMPLOYEE: {
    value: 1,
    desc: '员工'
  }
}

export const DATA_TYPE_ENUM = {
  NORMAL: {
    value: 1,
    desc: '普通'
  },
  ENCRYPT: {
    value: 10,
    desc: '加密'
  }
}

export const STAT_RANGE_TYPE_ENUM = {
  YEAR: {
    value: 'year',
    desc: '年'
  },
  QUARTER: {
    value: 'quarter',
    desc: '季度'
  },
  MONTH: {
    value: 'month',
    desc: '月'
  },
  WEEK: {
    value: 'week',
    desc: '周'
  }
}

export const STAT_RANGE_TYPE_OPTIONS = Object.values(STAT_RANGE_TYPE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const SYS_TYPE_ENUM = {
  MGR: {
    value: 'MGR',
    desc: '运营平台'
  },
  AGENT: {
    value: 'AGENT',
    desc: '代理商系统'
  },
  MCH: {
    value: 'MCH',
    desc: '商户系统'
  }
}

export const SYS_TYPE_OPTIONS = Object.values(SYS_TYPE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const STATE_ENUM = {
  DISABLED: {
    value: 0,
    desc: '禁用',
    color: 'volcano'
  },
  ENABLED: {
    value: 1,
    desc: '启用',
    color: 'green'
  }
}

export const STATE_OPTIONS = Object.values(STATE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const FLAG_ENUM = {
  NO: {
    value: 0,
    desc: '否',
    color: 'volcano'
  },
  YES: {
    value: 1,
    desc: '是',
    color: 'green'
  }
}

export const FLAG_OPTIONS = Object.values(FLAG_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const AGENT_TYPE_ENUM = {
  INDIVIDUAL: {
    value: 1,
    desc: '个人'
  },
  ENTERPRISE: {
    value: 2,
    desc: '企业'
  }
}

export const AGENT_TYPE_OPTIONS = Object.values(AGENT_TYPE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const SETT_ACCOUNT_TYPE_ENUM = {
  WX_CASH: {
    value: 'WX_CASH',
    desc: '个人微信',
    noLabel: '个人微信号'
  },
  ALIPAY_CASH: {
    value: 'ALIPAY_CASH',
    desc: '个人支付宝',
    noLabel: '支付宝账号'
  },
  BANK_PRIVATE: {
    value: 'BANK_PRIVATE',
    desc: '对私账户',
    noLabel: '收款银行卡号'
  },
  BANK_PUBLIC: {
    value: 'BANK_PUBLIC',
    desc: '对公账户',
    noLabel: '对公账号'
  }
}

export const SETT_ACCOUNT_TYPE_OPTIONS = Object.values(SETT_ACCOUNT_TYPE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const MCH_TYPE_ENUM = {
  NORMAL: {
    value: 1,
    desc: '普通商户',
    color: 'green'
  },
  SPECIAL: {
    value: 2,
    desc: '特约商户',
    color: 'orange'
  }
}

export const MCH_TYPE_OPTIONS = Object.values(MCH_TYPE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const MCH_LEVEL_ENUM = {
  M0: {
    value: 'M0',
    desc: 'M0',
    tips: '简单模式（页面简洁，仅基础收款功能）'
  },
  M1: {
    value: 'M1',
    desc: 'M1',
    tips: '高级模式（支持API调用，支持配置应用及分账、转账功能）'
  }
}

export const MCH_LEVEL_OPTIONS = Object.values(MCH_LEVEL_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const REFUND_MODE_ENUM = {
  PLAT: {
    value: 'plat',
    desc: '平台退款'
  },
  API: {
    value: 'api',
    desc: '接口退款'
  }
}

export const REFUND_MODE_OPTIONS = Object.values(REFUND_MODE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

export const CASH_OUT_FEE_TYPE_ENUM = {
  FIX: {
    value: 'FIX',
    desc: '单笔固定'
  },
  SINGLE: {
    value: 'SINGLE',
    desc: '单笔费率'
  },
  FIXANDRATE: {
    value: 'FIXANDRATE',
    desc: '固定+费率'
  }
}

export const CASH_OUT_FEE_TYPE_OPTIONS = Object.values(CASH_OUT_FEE_TYPE_ENUM).map(item => ({
  value: item.value,
  label: item.desc
}))

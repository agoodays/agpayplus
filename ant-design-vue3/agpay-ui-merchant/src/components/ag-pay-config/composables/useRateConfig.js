import { ref, reactive } from 'vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { infoBox } from '@/utils/info-box'
import { message } from 'ant-design-vue'

/** 费率类型编码常量 */
const FEE_TYPE_CODES = {
  MAIN_FEE: 'mainFee',
  AGENT_DEF_FEE: 'agentdefFee',
  MCH_APPLY_DEF_FEE: 'mchapplydefFee',
  READONLY_ISV_COST: 'readonlyIsvCost',
  READONLY_PARENT_AGENT: 'readonlyParentAgent',
  READONLY_PARENT_DEF_RATE: 'readonlyParentDefRate'
}

/** 阶梯费率模式 */
const LEVEL_MODE = {
  NORMAL: 'NORMAL',
  UNIONPAY: 'UNIONPAY'
}

/** 银行卡类型 */
const BANK_CARD_TYPE = {
  DEBIT: 'DEBIT',
  CREDIT: 'CREDIT'
}

/**
 * 创建空的费率配置结构
 * @returns {Object} 包含所有费率类型的空配置对象
 */
const createEmptyRateConfig = () => ({
  [FEE_TYPE_CODES.MAIN_FEE]: {},
  [FEE_TYPE_CODES.AGENT_DEF_FEE]: {},
  [FEE_TYPE_CODES.MCH_APPLY_DEF_FEE]: {},
  [FEE_TYPE_CODES.READONLY_ISV_COST]: {},
  [FEE_TYPE_CODES.READONLY_PARENT_AGENT]: {},
  [FEE_TYPE_CODES.READONLY_PARENT_DEF_RATE]: {}
})

/**
 * 创建费率分组初始结构
 * @param {Object} def - 分组定义
 * @returns {Object} 带有空费率数据的分组对象
 */
const createFeeGroup = (def) => ({
  ...def,
  mainFee: {},
  agentdefFee: {},
  mchapplydefFee: {},
  isMergeMode: false,
  selectedPayWayList: [],
  readonlyIsvCost: null,
  readonlyParentAgent: null,
  readonlyParentDefRate: null
})

/**
 * 创建重置后的费率分组结构（保留定义，清空数据）
 * @param {Object} item - 现有费率分组
 * @returns {Object} 重置后的分组对象
 */
const resetFeeGroup = (item) => ({
  ...item,
  selectedPayWayList: [],
  mainFee: {},
  agentdefFee: {},
  mchapplydefFee: {},
  readonlyIsvCost: null,
  readonlyParentAgent: null,
  readonlyParentDefRate: null,
  isMergeMode: false
})

/** 费率分组定义（按支付方式类型分组） */
const FEE_GROUP_DEFINITIONS = [
  {
    key: 'WECHAT1',
    name: '微信线下',
    filter: f => f.wayType === 'WECHAT' && ['WX_BAR', 'WX_JSAPI', 'WX_LITE'].indexOf(f.wayCode) >= 0
  },
  {
    key: 'WECHAT2',
    name: '微信线上',
    filter: f => f.wayType === 'WECHAT' && ['WX_BAR', 'WX_JSAPI', 'WX_LITE'].indexOf(f.wayCode) < 0
  },
  {
    key: 'ALIPAY1',
    name: '支付宝线下',
    filter: f => f.wayType === 'ALIPAY' && ['ALI_BAR', 'ALI_JSAPI', 'ALI_LITE', 'ALI_QR'].indexOf(f.wayCode) >= 0
  },
  {
    key: 'ALIPAY2',
    name: '支付宝线上',
    filter: f => f.wayType === 'ALIPAY' && ['ALI_BAR', 'ALI_JSAPI', 'ALI_LITE', 'ALI_QR'].indexOf(f.wayCode) < 0
  },
  {
    key: 'YSFPAY',
    name: '云闪付',
    filter: f => f.wayType === 'YSFPAY'
  },
  {
    key: 'UNIONPAY',
    name: '银联',
    filter: f => f.wayType === 'UNIONPAY'
  },
  {
    key: 'OTHER',
    name: '其他',
    filter: f => f.wayType === 'OTHER'
  }
]

/**
 * 费率配置 Composable
 * 负责支付渠道费率配置的加载、编辑、验证、保存等核心逻辑。
 * 支持单一费率、阶梯费率、合并模式等多种费率配置方式。
 *
 * @param {Object} props - 组件 props（需包含 ifCode、configMode、infoId）
 * @param {string} props.ifCode - 渠道编码
 * @param {string} props.configMode - 配置模式
 * @param {string|number} props.infoId - 信息 ID
 * @param {Function} [emit] - 组件 emit 函数，保存成功后触发 success 事件
 * @returns {Object} 费率配置相关状态与方法
 */
export function useRateConfig(props, emit) {
  /** 费率配置加载状态 */
  const loading = ref(false)
  /** 当前渠道编码 */
  const currentChannelCode = ref(props.ifCode)
  /** 只读费率类型列表（如服务商底价、上级代理商费率） */
  const readonlyFeeTypes = ref([])
  /** 可编辑费率类型列表 */
  const editableFeeTypes = ref([])
  /** 全部支付方式列表 */
  const allPayWayList = ref([])
  /** 支付方式映射表（wayCode -> payWay） */
  const allPayWayMap = ref({})
  /** 跳过校验标志（用于二次确认跳过校验） */
  const skipValidationFlag = ref(0)
  /** 原始已保存的支付方式编码列表（用于删除检测） */
  const originSavedList = ref([])

  /** 费率配置对象（按费率类型分组，每类为 wayCode -> config 的映射） */
  const rateConfig = reactive(createEmptyRateConfig())

  /** 费率分组列表（按支付方式类型分组，支持合并模式） */
  const feeGroups = reactive(FEE_GROUP_DEFINITIONS.map(def => createFeeGroup(def)))

  /**
   * 创建单条费率配置
   * @param {string} wayCode - 支付方式编码
   * @returns {Object} 费率配置对象
   */
  const createRateConfig = (wayCode) => ({
    wayCode,
    state: 0,
    applymentSupport: 0,
    feeType: 'SINGLE'
  })

  /**
   * 创建普通阶梯费率结构
   * @param {number} id1 - 第一档 ID
   * @param {number} id2 - 第二档 ID
   * @returns {Array} 阶梯费率数组
   */
  const createNormalLevel = (id1, id2) => [{
    minFee: 0,
    maxFee: 99999,
    levelList: [
      { id: id1, minAmount: 0, maxAmount: 1000, feeRate: null },
      { id: id2, minAmount: 1000, maxAmount: 999999.99, feeRate: null }
    ]
  }]

  /**
   * 创建银联阶梯费率结构（借记 + 贷记）
   * @param {number} id1 - 第一档 ID
   * @param {number} id2 - 第二档 ID
   * @returns {Array} 银联阶梯费率数组
   */
  const createUnionpayLevel = (id1, id2) => [
    {
      minFee: 0,
      maxFee: 99999,
      bankCardType: BANK_CARD_TYPE.DEBIT,
      levelList: [
        { id: id1, minAmount: 0, maxAmount: 1000, feeRate: null },
        { id: id2, minAmount: 1000, maxAmount: 999999.99, feeRate: null }
      ]
    },
    {
      minFee: 0,
      maxFee: 99999,
      bankCardType: BANK_CARD_TYPE.CREDIT,
      levelList: [
        { id: id1, minAmount: 0, maxAmount: 1000, feeRate: null },
        { id: id2, minAmount: 1000, maxAmount: 999999.99, feeRate: null }
      ]
    }
  ]

  /**
   * 创建阶梯费率区间项
   * @param {number} id - 区间 ID
   * @returns {Object} 阶梯区间项
   */
  const createLevelItem = (id) => ({
    id,
    minAmount: null,
    maxAmount: null,
    fee: null
  })

  /**
   * 检查多个费率配置是否可以合并为同一配置模式
   * @param {Array} rateConfigs - 费率配置数组
   * @returns {Object|boolean} 可合并时返回基准配置对象，不可合并返回 false
   */
  const isSameConfigMode = (rateConfigs) => {
    let rateConfigTemp = null
    for (const i in rateConfigs) {
      const rateConfigItem = JSON.parse(JSON.stringify(rateConfigs[i]))
      if (rateConfigItem.state === 1) {
        rateConfigItem.wayCode = null
        if (rateConfigTemp === null) {
          rateConfigTemp = rateConfigItem
          continue
        }
        if (JSON.stringify(rateConfigTemp) !== JSON.stringify(rateConfigItem)) {
          return false
        }
      }
    }
    return rateConfigTemp
  }

  /**
   * 将接口返回的费率配置数据转换并填充到 rateConfig 中
   * @param {string} key - 费率类型键
   * @param {Object} feeRateConfig - 接口返回的费率配置映射
   */
  const transformRateConfig = (key, feeRateConfig) => {
    Object.values(rateConfig[key]).forEach(item => {
      item.feeType = 'SINGLE'
      delete item.feeRate
      delete item.minFee
      delete item.maxFee
      const payWayConfig = feeRateConfig[item.wayCode] || {}
      Object.assign(item, payWayConfig)
      item.state = feeRateConfig[item.wayCode] ? 1 : 0
      feeRateConfig[item.wayCode] && feeRateConfig[item.wayCode].state === 0 && (item.state = 0)
      typeof item.feeRate === 'number' && (item.feeRate = Number.parseFloat((item.feeRate * 100).toFixed(6)))
      if (item.levelMode && item[item.levelMode]) {
        for (const i in item[item.levelMode]) {
          typeof item[item.levelMode][i].maxFee === 'number' && (item[item.levelMode][i].maxFee = Number.parseFloat((item[item.levelMode][i].maxFee / 100).toFixed(2)))
          typeof item[item.levelMode][i].minFee === 'number' && (item[item.levelMode][i].minFee = Number.parseFloat((item[item.levelMode][i].minFee / 100).toFixed(2)))
          let id = 1
          item[item.levelMode][i].levelList && item[item.levelMode][i].levelList.forEach(s => {
            s.id = id
            typeof s.feeRate === 'number' && (s.feeRate = Number.parseFloat((s.feeRate * 100).toFixed(6)))
            typeof s.maxAmount === 'number' && (s.maxAmount = Number.parseFloat((s.maxAmount / 100).toFixed(2)))
            typeof s.minAmount === 'number' && (s.minAmount = Number.parseFloat((s.minAmount / 100).toFixed(2)))
            id++
          })
        }
      }
    })
  }

  /**
   * 将只读费率配置应用到可编辑费率上（保留 feeRate，覆盖其他配置）
   * @param {Object} fee - 可编辑费率对象
   * @param {Object} readonlyFee - 只读费率对象
   * @returns {Object} 合并后的费率对象
   */
  const applyFeeConfig = (fee, readonlyFee) => {
    if (fee.feeType === readonlyFee.feeType) {
      return fee
    }
    const { state, feeRate, ...readonlyFeeWithoutFeeRate } = readonlyFee
    
    if (readonlyFee.feeType !== 'SINGLE') {
      const updatedItems = readonlyFeeWithoutFeeRate[readonlyFee.levelMode].map((item) => ({
        ...item,
        levelList: item.levelList.map((levelItem) => ({
          ...levelItem,
          feeRate: null
        }))
      }))

      if (readonlyFee.levelMode === LEVEL_MODE.NORMAL) {
        return Object.assign(fee, { ...readonlyFeeWithoutFeeRate, NORMAL: updatedItems })
      }
      if (readonlyFee.levelMode === LEVEL_MODE.UNIONPAY) {
        return Object.assign(fee, { ...readonlyFeeWithoutFeeRate, UNIONPAY: updatedItems })
      }
    }
    return Object.assign(fee, readonlyFeeWithoutFeeRate)
  }

  /**
   * 根据配置模式和费率类型获取显示名称
   * @param {string} feeType - 费率类型编码
   * @returns {string} 费率类型显示名称
   */
  const getFeeTypeName = (feeType) => {
    if (props.configMode === 'mgrIsv') {
      if (feeType === FEE_TYPE_CODES.MAIN_FEE) return '服务商底价'
      if (feeType === FEE_TYPE_CODES.AGENT_DEF_FEE) return '代理商默认'
      if (feeType === FEE_TYPE_CODES.MCH_APPLY_DEF_FEE) return '商户进件默认'
    }
    if (props.configMode === 'mgrMch') {
      if (feeType === FEE_TYPE_CODES.READONLY_ISV_COST) return '服务商底价'
      if (feeType === FEE_TYPE_CODES.READONLY_PARENT_AGENT) return '上级代理商'
    }
    if (props.configMode === 'mgrAgent') {
      if (feeType === FEE_TYPE_CODES.MAIN_FEE) return '当前代理商'
      if (feeType === FEE_TYPE_CODES.AGENT_DEF_FEE) return '下级代理商默认'
      if (feeType === FEE_TYPE_CODES.MCH_APPLY_DEF_FEE) return '代理商子商户进件默认'
      if (feeType === FEE_TYPE_CODES.READONLY_ISV_COST) return '服务商底价'
      if (feeType === FEE_TYPE_CODES.READONLY_PARENT_AGENT) return '上级代理商'
    }
    if (props.configMode === 'agentSelf') {
      if (feeType === FEE_TYPE_CODES.MAIN_FEE) return '我的代理'
      if (feeType === FEE_TYPE_CODES.AGENT_DEF_FEE) return '下级代理商默认'
      if (feeType === FEE_TYPE_CODES.MCH_APPLY_DEF_FEE) return '商户进件默认'
    }
    if (props.configMode === 'agentSubagent') {
      if (feeType === FEE_TYPE_CODES.MAIN_FEE) return '当前代理商'
      if (feeType === FEE_TYPE_CODES.AGENT_DEF_FEE) return '下级代理商默认'
      if (feeType === FEE_TYPE_CODES.MCH_APPLY_DEF_FEE) return '商户进件默认'
      if (feeType === FEE_TYPE_CODES.READONLY_PARENT_AGENT) return '我的代理'
    }
    if ((props.configMode === 'mgrMch' || props.configMode === 'agentMch') && feeType === FEE_TYPE_CODES.MAIN_FEE) return '商户'
    if ((props.configMode === 'mgrApplyment' || props.configMode === 'agentApplyment') && feeType === FEE_TYPE_CODES.MAIN_FEE) return '进件'
    if (props.configMode === 'mchSelfApp1' && feeType === FEE_TYPE_CODES.MAIN_FEE) return '接口'
    return ''
  }

  /**
   * 读取默认费率（将只读费率配置应用到 mainFee）
   * @param {boolean} isMergeModeVal - 是否为合并模式
   * @param {string} key - 分组键或支付方式编码
   */
  const readDefaultFeeRate = (isMergeModeVal, key) => {
    if (!isMergeModeVal) {
      const mainFee = rateConfig.mainFee[key]
      const readonlyFee = rateConfig.readonlyParentDefRate[key] || rateConfig.readonlyParentAgent[key] || rateConfig.readonlyIsvCost[key]
      if (readonlyFee) {
        const { state, ...readonlyFeeWithoutState } = readonlyFee
        rateConfig.mainFee[key] = Object.assign(mainFee, readonlyFeeWithoutState)
      }
    } else {
      const feeGroup = feeGroups[key]
      if (!feeGroup) return
      const mainFee = feeGroup.mainFee
      const readonlyFee = feeGroup.readonlyParentDefRate || feeGroup.readonlyParentAgent || feeGroup.readonlyIsvCost
      if (readonlyFee) {
        const { state, ...readonlyFeeWithoutState } = readonlyFee
        feeGroup.mainFee = Object.assign(mainFee, readonlyFeeWithoutState)
      }
    }
  }

  /**
   * 切换支付方式 / 分组的开通状态
   * @param {string} wayCode - 支付方式编码（分组模式下为空）
   * @param {boolean|Event} checked - 开关状态或事件对象
   * @param {Object} [feeGroup] - 费率分组对象（分组模式下传入）
   */
  const onStateChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked

    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        if (isChecked && !rateConfig[item][wayCode]) {
          rateConfig[item][wayCode] = createRateConfig(wayCode)
        } else {
          rateConfig[item][wayCode].state = +isChecked
        }
      })
    }

    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        if (isChecked && !feeGroup[item]) {
          feeGroup[item] = createRateConfig(wayCode)
        } else {
          feeGroup[item].state = +isChecked
        }
      })
    }
  }

  /**
   * 切换是否支持进件
   * @param {string} wayCode - 支付方式编码
   * @param {boolean|Event} checked - 开关状态或事件对象
   * @param {Object} [feeGroup] - 费率分组对象
   */
  const onApplymentSupportChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked

    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        rateConfig[item][wayCode].applymentSupport = +isChecked
      })
    }

    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        if (feeGroup[item]) {
          feeGroup[item].applymentSupport = +isChecked
        }
      })
    }
  }

  /**
   * 切换费率类型（单一费率 / 阶梯费率）
   * @param {string} wayCode - 支付方式编码
   * @param {boolean|Event} checked - 是否切换为阶梯费率
   * @param {Object} [feeGroup] - 费率分组对象
   */
  const onFeeTypeChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked
    const currentTime = new Date()
    const id1 = currentTime.getTime()
    currentTime.setSeconds(currentTime.getSeconds() + 1)
    const id2 = currentTime.getTime()

    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        if (isChecked) {
          rateConfig[item][wayCode].feeType = 'LEVEL'
          rateConfig[item][wayCode].levelMode = LEVEL_MODE.NORMAL
          if (!rateConfig[item][wayCode][LEVEL_MODE.NORMAL]) {
            rateConfig[item][wayCode][LEVEL_MODE.NORMAL] = createNormalLevel(id1, id2)
          }
        } else {
          rateConfig[item][wayCode].feeType = 'SINGLE'
          delete rateConfig[item][wayCode].levelMode
        }
      })
    }

    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        if (isChecked) {
          feeGroup[item].feeType = 'LEVEL'
          feeGroup[item].levelMode = LEVEL_MODE.NORMAL
          if (!feeGroup[item][LEVEL_MODE.NORMAL]) {
            feeGroup[item][LEVEL_MODE.NORMAL] = createNormalLevel(id1, id2)
          }
        } else {
          feeGroup[item].feeType = 'SINGLE'
          delete feeGroup[item].levelMode
        }
      })
    }
  }

  /**
   * 切换阶梯费率模式（普通 / 银联）
   * @param {string} wayCode - 支付方式编码
   * @param {boolean|Event} checked - 是否切换为银联模式
   * @param {Object} [feeGroup] - 费率分组对象
   */
  const onLevelModeChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked
    const currentTime = new Date()
    const id1 = currentTime.getTime()
    currentTime.setSeconds(currentTime.getSeconds() + 1)
    const id2 = currentTime.getTime()

    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        rateConfig[item][wayCode].levelMode = isChecked ? LEVEL_MODE.UNIONPAY : LEVEL_MODE.NORMAL
        if (isChecked && !rateConfig[item][wayCode][LEVEL_MODE.UNIONPAY]) {
          rateConfig[item][wayCode][LEVEL_MODE.UNIONPAY] = createUnionpayLevel(id1, id2)
        }
        if (!isChecked && !rateConfig[item][wayCode][LEVEL_MODE.NORMAL]) {
          rateConfig[item][wayCode][LEVEL_MODE.NORMAL] = createNormalLevel(id1, id2)
        }
      })
    }

    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        feeGroup[item].levelMode = isChecked ? LEVEL_MODE.UNIONPAY : LEVEL_MODE.NORMAL
        if (isChecked && !feeGroup[item][LEVEL_MODE.UNIONPAY]) {
          feeGroup[item][LEVEL_MODE.UNIONPAY] = createUnionpayLevel(id1, id2)
        }
        if (!isChecked && !feeGroup[item][LEVEL_MODE.NORMAL]) {
          feeGroup[item][LEVEL_MODE.NORMAL] = createNormalLevel(id1, id2)
        }
      })
    }
  }

  /**
   * 阶梯费率金额输入处理
   * @param {string} wayCode - 支付方式编码
   * @param {string} flag - 金额类型标记（min / max）
   * @param {number} id - 阶梯区间 ID
   * @param {number} amount - 输入金额
   * @param {Object} [feeGroup] - 费率分组对象
   */
  const onAmountInput = (wayCode, flag, id, amount, feeGroup) => {
    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
      })
    }
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        feeGroup[item][LEVEL_MODE.NORMAL][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
      })
    }
  }

  /**
   * 更新单个支付方式的阶梯费率值
   * @param {string} feeType - 费率类型
   * @param {string} wayCode - 支付方式编码
   * @param {string} bankCardType - 银行卡类型
   * @param {number} levelKey - 阶梯层级索引
   * @param {number} value - 费率值
   */
  const updateFeeRate = (feeType, wayCode, bankCardType, levelKey, value) => {
    const target = rateConfig[feeType][wayCode]?.[rateConfig.mainFee[wayCode]?.levelMode]
      ?.find(f => f.bankCardType === bankCardType)?.levelList[levelKey]
    if (target) target.feeRate = value
  }

  /**
   * 更新单个支付方式的最低费用
   * @param {string} feeType - 费率类型
   * @param {string} wayCode - 支付方式编码
   * @param {number} levelModeKey - 阶梯层级索引
   * @param {number} value - 最低费用值
   */
  const updateMinFee = (feeType, wayCode, levelModeKey, value) => {
    const target = rateConfig[feeType][wayCode]?.[rateConfig.mainFee[wayCode]?.levelMode]?.[levelModeKey]
    if (target) target.minFee = value
  }

  /**
   * 更新单个支付方式的最高费用
   * @param {string} feeType - 费率类型
   * @param {string} wayCode - 支付方式编码
   * @param {number} levelModeKey - 阶梯层级索引
   * @param {number} value - 最高费用值
   */
  const updateMaxFee = (feeType, wayCode, levelModeKey, value) => {
    const target = rateConfig[feeType][wayCode]?.[rateConfig.mainFee[wayCode]?.levelMode]?.[levelModeKey]
    if (target) target.maxFee = value
  }

  /**
   * 更新分组的阶梯费率值
   * @param {string} feeType - 费率类型
   * @param {Object} feeGroup - 费率分组对象
   * @param {string} bankCardType - 银行卡类型
   * @param {number} levelKey - 阶梯层级索引
   * @param {number} value - 费率值
   */
  const updateGroupFeeRate = (feeType, feeGroup, bankCardType, levelKey, value) => {
    const target = feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]
      ?.find(f => f.bankCardType === bankCardType)?.levelList[levelKey]
    if (target) target.feeRate = value
  }

  /**
   * 更新分组的最低费用
   * @param {string} feeType - 费率类型
   * @param {Object} feeGroup - 费率分组对象
   * @param {number} levelModeKey - 阶梯层级索引
   * @param {number} value - 最低费用值
   */
  const updateGroupMinFee = (feeType, feeGroup, levelModeKey, value) => {
    const target = feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]?.[levelModeKey]
    if (target) target.minFee = value
  }

  /**
   * 更新分组的最高费用
   * @param {string} feeType - 费率类型
   * @param {Object} feeGroup - 费率分组对象
   * @param {number} levelModeKey - 阶梯层级索引
   * @param {number} value - 最高费用值
   */
  const updateGroupMaxFee = (feeType, feeGroup, levelModeKey, value) => {
    const target = feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]?.[levelModeKey]
    if (target) target.maxFee = value
  }

  /**
   * 更新分组的单一费率值
   * @param {string} feeType - 费率类型
   * @param {Object} feeGroup - 费率分组对象
   * @param {number} value - 费率值
   */
  const updateGroupSingleFeeRate = (feeType, feeGroup, value) => {
    if (feeGroup[feeType]) feeGroup[feeType].feeRate = value
  }

  /**
   * 添加阶梯费率区间项
   * @param {string} wayCode - 支付方式编码
   * @param {Object} [feeGroup] - 费率分组对象
   */
  const addLevelItem = (wayCode, feeGroup) => {
    const id = new Date().getTime()
    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList.push(createLevelItem(id))
      })
    }
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        feeGroup[item][LEVEL_MODE.NORMAL][0].levelList.push(createLevelItem(id))
      })
    }
  }

  /**
   * 删除阶梯费率区间项
   * @param {string} wayCode - 支付方式编码
   * @param {number} id - 区间 ID
   * @param {Object} [feeGroup] - 费率分组对象
   */
  const deleteLevelItem = (wayCode, id, feeGroup) => {
    if (wayCode) {
      editableFeeTypes.value.forEach(item => {
        rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList =
            rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList.filter(i => i.id !== id)
      })
    }
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.forEach(item => {
        feeGroup[item][LEVEL_MODE.NORMAL][0].levelList =
            feeGroup[item][LEVEL_MODE.NORMAL][0].levelList.filter(i => i.id !== id)
      })
    }
  }

  /**
   * 检查阶梯区间是否存在重叠
   * @param {Array} limits - 阶梯区间列表
   * @returns {boolean} 存在重叠返回 true
   */
  const checkOverlap = (limits) => {
    for (let i = 0; i < limits.length; i++) {
      const { minAmount: min1, maxAmount: max1 } = limits[i]
      for (let j = i + 1; j < limits.length; j++) {
        const { minAmount: min2, maxAmount: max2 } = limits[j]
        if (min1 <= max2 && min2 < max1) {
          return true
        }
      }
    }
    return false
  }

  /**
   * 阶梯费率校验（校验保底费用、封顶费用、区间重叠、费率值等）
   * @param {Object} fee - 待填充的费率对象
   * @param {Object} rateConfigItem - 原始费率配置项
   * @returns {boolean} 校验通过返回 true
   */
  const levelValidate = (fee, rateConfigItem) => {
    const levelFees = rateConfigItem[rateConfigItem.levelMode]
    for (const i in levelFees) {
      const levelFee = levelFees[i]
      if (isNaN(+levelFee.minFee) || levelFee.minFee === '') {
        message.error('阶梯费率请填入保底费用')
        return false
      }
      if (isNaN(+levelFee.maxFee) || levelFee.maxFee === '') {
        message.error('阶梯费率请填入封顶费用')
        return false
      }
      levelFees[i].minFee = Number.parseInt(+levelFee.minFee * 100 + '')
      levelFees[i].maxFee = Number.parseInt(+levelFee.maxFee * 100 + '')

      if (levelFee.levelList.length <= 0) {
        message.error('阶梯费率请至少包含一个价格区间')
        return false
      }

      if (checkOverlap(levelFee.levelList)) {
        message.error('阶梯费率请填入正确的金额区间值，存在重叠区间')
        return false
      }

      for (const k in levelFee.levelList) {
        const levelItem = levelFee.levelList[k]
        if (isNaN(+levelItem.feeRate) || levelItem.feeRate === '' || +levelItem.feeRate <= 0) {
          message.error('请录入阶梯费率')
          return false
        }
        if (isNaN(+levelItem.minAmount) || levelItem.minAmount === '' ||
            isNaN(+levelItem.maxAmount) || levelItem.maxAmount === '') {
          message.error('阶梯费率请填入金额区间值')
          return false
        }
        if (+levelItem.minAmount > +levelItem.maxAmount) {
          message.error('阶梯费率请填入正确的金额区间值')
          return false
        }
        levelFees[i].levelList[k].feeRate = Number.parseFloat((+levelItem.feeRate / 100).toFixed(6))
        levelFees[i].levelList[k].minAmount = Number.parseInt(+levelItem.minAmount * 100 + '')
        levelFees[i].levelList[k].maxAmount = Number.parseInt(+levelItem.maxAmount * 100 + '')
      }
    }
    fee.levelMode = rateConfigItem.levelMode
    fee[rateConfigItem.levelMode] = levelFees
    return true
  }

  /**
   * 单一费率校验
   * @param {Object} fee - 费率对象
   * @returns {boolean} 校验通过返回 true
   */
  const singleValidate = (fee) => {
    if (isNaN(+fee.feeRate) || fee.feeRate === '' || +fee.feeRate <= 0) {
      message.error('请录入费率')
      return false
    }
    fee.feeRate = Number.parseFloat((+fee.feeRate / 100).toFixed(6))
    return true
  }

  /**
   * 根据支付方式编码查找所属费率分组
   * @param {string} wayCode - 支付方式编码
   * @returns {Array} [feeGroup, checked] - 分组对象与是否选中
   */
  const getFeeGroupByWayCode = (wayCode) => {
    for (const i in feeGroups) {
      const feeGroup = feeGroups[i]
      for (const k in feeGroup.selectedPayWayList) {
        const payWay = feeGroup.selectedPayWayList[k]
        if (payWay.wayCode === wayCode) {
          return [feeGroup, payWay.checked]
        }
      }
    }
    return [null, false]
  }

  /**
   * 从费率配置中提取并校验指定类型的费率列表
   * @param {string} key - 费率类型键
   * @param {Array} rateConfigs - 费率配置数组
   * @returns {Array|boolean} 校验通过返回费率数组，失败返回 false
   */
  const getFees = (key, rateConfigs) => {
    const fees = []
    for (const i in rateConfigs) {
      let rateConfigItem = rateConfigs[i]
      const wayCode = rateConfigItem.wayCode
      const [feeGroup, checked] = getFeeGroupByWayCode(wayCode)
      
      if (feeGroup == null ||
          (feeGroup.isMergeMode && feeGroup.mainFee.state !== 1) ||
          (feeGroup.isMergeMode && !checked) ||
          (!feeGroup.isMergeMode && rateConfigItem.state !== 1)
      ) {
        continue
      }
      
      if (feeGroup.isMergeMode) {
        rateConfigItem = JSON.parse(JSON.stringify(feeGroup[key]))
        rateConfigItem.wayCode = wayCode
      }

      const fee = {}
      fee.wayCode = rateConfigItem.wayCode
      fee.feeType = rateConfigItem.feeType
      fee.state = rateConfigItem.state
      fee.applymentSupport = rateConfigItem.applymentSupport
      
      if (rateConfigItem.feeType === 'SINGLE') {
        if (isNaN(+rateConfigItem.feeRate) || rateConfigItem.feeRate === '' || +rateConfigItem.feeRate < 0) {
          message.error('费率值不可小于0')
          return false
        }
        fee.feeRate = Number.parseFloat((+rateConfigItem.feeRate / 100).toFixed(6))
      } else {
        if (!levelValidate(fee, rateConfigItem)) {
          return false
        }
      }
      fees.push(fee)
    }
    return fees
  }

  /**
   * 汇总生成提交保存的费率配置对象
   * 按配置模式组装不同结构的费率配置，校验合并模式选中状态。
   * @returns {Object|boolean} 校验通过返回费率配置对象，失败返回 false
   */
  const getFeeRateConfig = () => {
    for (const i in feeGroups) {
      const feeGroup = feeGroups[i]
      if (
        feeGroup.isMergeMode &&
        feeGroup.selectedPayWayList.length > 0 &&
        feeGroup.selectedPayWayList.filter(f => f.checked).length <= 0 &&
        feeGroup.mainFee.state === 1
      ) {
        message.error(`【${feeGroup.name}】合并模式为开通状态但没有选择任何产品， 请点击关闭或勾选产品！`)
        return false
      }
    }
    
    const mainFee = getFees(FEE_TYPE_CODES.MAIN_FEE, Object.values(rateConfig.mainFee))
    if (typeof mainFee !== 'object') return false
    
    let agentdefFee = null
    let mchapplydefFee = null
    
    if (props.configMode === 'mgrIsv' || props.configMode === 'mgrAgent' || props.configMode === 'agentSubagent' || props.configMode === 'agentSelf') {
      agentdefFee = getFees(FEE_TYPE_CODES.AGENT_DEF_FEE, Object.values(rateConfig.agentdefFee))
      if (typeof agentdefFee !== 'object') return false
      mchapplydefFee = getFees(FEE_TYPE_CODES.MCH_APPLY_DEF_FEE, Object.values(rateConfig.mchapplydefFee), true)
      if (typeof mchapplydefFee !== 'object') return false
    }
    
    if (props.configMode === 'mgrIsv') {
      return { ISVCOST: mainFee, AGENTDEF: agentdefFee, MCHAPPLYDEF: mchapplydefFee }
    }
    if (props.configMode === 'mgrAgent' || props.configMode === 'agentSubagent' || props.configMode === 'agentSelf') {
      return { AGENTRATE: mainFee, AGENTDEF: agentdefFee, MCHAPPLYDEF: mchapplydefFee }
    }
    if (props.configMode === 'mgrMch' || props.configMode === 'agentMch') {
      return { MCHRATE: mainFee }
    }
    if (props.configMode === 'mgrApplyment' || props.configMode === 'agentApplyment') {
      return { MCHAPPLYDEF: mainFee }
    }
    if (props.configMode === 'mchSelfApp1') {
      return { MCHRATE: mainFee }
    }
    if (props.configMode === 'mchApplyment') {
      return { MCHAPPLYDEF: mainFee }
    }
    return { AGENTRATE: mainFee }
  }

  /**
   * 加载费率配置数据
   * 请求已保存的费率映射和支付方式列表，并初始化费率配置结构。
   * @param {string} [currentIfCodeVal] - 当前渠道编码（可选，未传则使用 currentChannelCode）
   */
  const getRateConfig = async (currentIfCodeVal) => {
    if (loading.value) return
    loading.value = true

    try {
      if (currentIfCodeVal) {
        currentChannelCode.value = currentIfCodeVal
      }

      editableFeeTypes.value = []
      readonlyFeeTypes.value = []
      allPayWayList.value = []
      allPayWayMap.value = {}

      // 重置费率配置（整体替换为空结构）
      Object.assign(rateConfig, createEmptyRateConfig())

      originSavedList.value = []

      // 重置费率分组
      feeGroups.forEach((item, index) => {
        Object.assign(item, resetFeeGroup(item))
      })

      const params = { configMode: props.configMode, infoId: props.infoId, ifCode: currentChannelCode.value }

      let mapData = {}
      try {
        mapData = await payConfigApi.queryRateConfigList('/savedMapData', params)
      } catch (error) {
        console.error('获取费率配置映射数据失败:', error)
        return
      }

      Object.assign(params, { pageSize: -1 })
      try {
        const res = await payConfigApi.queryRateConfigList('/payways', params)
        res.records.forEach(payWay => {
          payWay.checked = false
          allPayWayList.value.push(payWay)
          allPayWayMap.value[payWay.wayCode] = payWay

          rateConfig[FEE_TYPE_CODES.MAIN_FEE][payWay.wayCode] = createRateConfig(payWay.wayCode)
          rateConfig[FEE_TYPE_CODES.AGENT_DEF_FEE][payWay.wayCode] = createRateConfig(payWay.wayCode)
          rateConfig[FEE_TYPE_CODES.MCH_APPLY_DEF_FEE][payWay.wayCode] = createRateConfig(payWay.wayCode)
          rateConfig[FEE_TYPE_CODES.READONLY_ISV_COST][payWay.wayCode] = mapData && mapData.READONLYISVCOST ? createRateConfig(payWay.wayCode) : null
          rateConfig[FEE_TYPE_CODES.READONLY_PARENT_AGENT][payWay.wayCode] = mapData && mapData.READONLYPARENTAGENT ? createRateConfig(payWay.wayCode) : null
          rateConfig[FEE_TYPE_CODES.READONLY_PARENT_DEF_RATE][payWay.wayCode] = mapData && mapData.READONLYPARENTDEFRATE ? createRateConfig(payWay.wayCode) : null
        })

        feeGroups.forEach(item => {
          item.mainFee = createRateConfig(null)
          item.agentdefFee = createRateConfig(null)
          item.mchapplydefFee = createRateConfig(null)
          item.readonlyIsvCost = mapData && mapData.READONLYISVCOST ? createRateConfig(null) : null
          item.readonlyParentAgent = mapData && mapData.READONLYPARENTAGENT ? createRateConfig(null) : null
          item.readonlyParentDefRate = mapData && mapData.READONLYPARENTDEFRATE ? createRateConfig(null) : null

          allPayWayList.value.filter(item.filter).forEach(payWay => {
            item.selectedPayWayList.push({
              wayCode: payWay.wayCode,
              wayName: payWay.wayName,
              checked: false
            })
          })
        })

        if (mapData && mapData.ISVCOST) {
          transformRateConfig(FEE_TYPE_CODES.MAIN_FEE, mapData.ISVCOST)
          originSavedList.value = JSON.parse(JSON.stringify(Object.keys(mapData.ISVCOST)))
        }
        if (mapData && mapData.AGENTRATE) {
          originSavedList.value = JSON.parse(JSON.stringify(Object.keys(mapData.AGENTRATE)))
          transformRateConfig(FEE_TYPE_CODES.MAIN_FEE, mapData.AGENTRATE)
        }
        mapData && mapData.MCHRATE && transformRateConfig(FEE_TYPE_CODES.MAIN_FEE, mapData.MCHRATE)
        mapData && mapData.AGENTDEF && transformRateConfig(FEE_TYPE_CODES.AGENT_DEF_FEE, mapData.AGENTDEF)
        mapData && mapData.MCHAPPLYDEF && transformRateConfig(FEE_TYPE_CODES.MCH_APPLY_DEF_FEE, mapData.MCHAPPLYDEF)

        if (mapData && mapData.READONLYISVCOST) {
          readonlyFeeTypes.value.push(FEE_TYPE_CODES.READONLY_ISV_COST)
          transformRateConfig(FEE_TYPE_CODES.READONLY_ISV_COST, mapData.READONLYISVCOST)
        }
        if (mapData && mapData.READONLYPARENTAGENT) {
          readonlyFeeTypes.value.push(FEE_TYPE_CODES.READONLY_PARENT_AGENT)
          transformRateConfig(FEE_TYPE_CODES.READONLY_PARENT_AGENT, mapData.READONLYPARENTAGENT)
        }
        if (mapData && mapData.READONLYPARENTDEFRATE) {
          transformRateConfig(FEE_TYPE_CODES.READONLY_PARENT_DEF_RATE, mapData.READONLYPARENTDEFRATE)
        }

        mapData && (mapData.ISVCOST || mapData.AGENTRATE || mapData.MCHRATE) && editableFeeTypes.value.push(FEE_TYPE_CODES.MAIN_FEE)
        mapData && mapData.AGENTDEF && editableFeeTypes.value.push(FEE_TYPE_CODES.AGENT_DEF_FEE)
        mapData && mapData.MCHAPPLYDEF && editableFeeTypes.value.push(FEE_TYPE_CODES.MCH_APPLY_DEF_FEE)
      } catch (error) {
        console.error('获取支付产品列表失败:', error)
        return
      }

      feeGroups.forEach(item => {
        item.isMergeMode = false
        const payWays = []
        item.selectedPayWayList.forEach(c => payWays.push(c.wayCode))

        const mainFee = isSameConfigMode(Object.values(rateConfig.mainFee).filter(f => payWays.indexOf(f.wayCode) >= 0))
        const agentdefFee = isSameConfigMode(Object.values(rateConfig.agentdefFee).filter(f => payWays.indexOf(f.wayCode) >= 0))
        const mchapplydefFee = isSameConfigMode(Object.values(rateConfig.mchapplydefFee).filter(f => payWays.indexOf(f.wayCode) >= 0))
        const readonlyIsvCost = mapData && mapData.READONLYISVCOST ? isSameConfigMode(Object.values(rateConfig.readonlyIsvCost).filter(f => payWays.indexOf(f.wayCode) >= 0)) : null
        const readonlyParentAgent = mapData && mapData.READONLYPARENTAGENT ? isSameConfigMode(Object.values(rateConfig.readonlyParentAgent).filter(f => payWays.indexOf(f.wayCode) >= 0)) : null
        const readonlyParentDefRate = mapData && mapData.READONLYPARENTDEFRATE ? isSameConfigMode(Object.values(rateConfig.readonlyParentDefRate).filter(f => payWays.indexOf(f.wayCode) >= 0)) : null

        if (typeof mainFee === 'object' && typeof agentdefFee === 'object' && typeof mchapplydefFee === 'object') {
          if (mainFee) item.mainFee = mainFee
          if (agentdefFee) item.agentdefFee = agentdefFee
          if (mchapplydefFee) item.mchapplydefFee = mchapplydefFee

          if (readonlyIsvCost) {
            item.readonlyIsvCost = readonlyIsvCost
            const { state, feeRate, ...readonlyFeeWithoutFeeRateAndState } = readonlyIsvCost
            item.mainFee = applyFeeConfig(item.mainFee, readonlyFeeWithoutFeeRateAndState)
            item.agentdefFee = applyFeeConfig(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
            item.mchapplydefFee = applyFeeConfig(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
          }
          if (readonlyParentAgent) {
            item.readonlyParentAgent = readonlyParentAgent
            const { state, feeRate, ...readonlyFeeWithoutFeeRateAndState } = readonlyParentAgent
            item.mainFee = applyFeeConfig(item.mainFee, readonlyFeeWithoutFeeRateAndState)
            item.agentdefFee = applyFeeConfig(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
            item.mchapplydefFee = applyFeeConfig(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
          }
          if (readonlyParentDefRate) {
            item.readonlyParentDefRate = readonlyParentDefRate
          }

          item.selectedPayWayList.forEach(c => {
            c.checked = rateConfig.mainFee[c.wayCode] != null && !!rateConfig.mainFee[c.wayCode].state
          })
          item.isMergeMode = true
        }
      })
    } finally {
      // 统一在 finally 中复位加载状态，避免多个 return 点遗漏
      loading.value = false
    }
  }

  /**
   * 提交保存费率配置
   * 校验费率数据后通过二次确认弹窗提交保存。
   * 保存成功后触发 emit('success') 通知父组件。
   */
  const onSubmit = async () => {
    const feeRateConfig = getFeeRateConfig()
    if (typeof feeRateConfig !== 'object') {
      return
    }

    const getDeletedWayCodes = (configList) => {
      const wayCodes = []
      originSavedList.value.forEach(wayCode => {
        if (configList.filter(f => f.wayCode === wayCode).length <= 0) {
          wayCodes.push(wayCode)
        }
      })
      return wayCodes
    }

    let deletedPayWayCodes = []
    let originSavedListResult = null

    if (props.configMode === 'mgrIsv') {
      deletedPayWayCodes = getDeletedWayCodes(feeRateConfig.ISVCOST)
      originSavedListResult = []
      feeRateConfig.ISVCOST.forEach(s => {
          originSavedListResult.push(s.wayCode)
      })
    } else {
        if (props.configMode === 'mgrAgent' || props.configMode === 'agentSubagent') {
          deletedPayWayCodes = getDeletedWayCodes(feeRateConfig.AGENTRATE)
          originSavedListResult = []
          feeRateConfig.AGENTRATE.forEach(s => {
              originSavedListResult.push(s.wayCode)
          })
        }
    }

    let content = ''
    if (deletedPayWayCodes.length > 0) {
      content = '系统检测到关闭了' + deletedPayWayCodes.length + '个支付产品：【'
      deletedPayWayCodes.forEach(wayCode => {
        allPayWayMap.value[wayCode] ? content += `${allPayWayMap.value[wayCode].wayName}(${wayCode});` : content += `${wayCode}(已禁用);`
      })
      content += '】，点击确定将同时关闭操作对象的下级代理商和商户的配置！'
    }

    await new Promise((resolve, reject) => {
      infoBox.confirmPrimary('确认操作？', content, async () => {
        try {
          const params = {
            infoId: props.infoId,
            ifCode: props.ifCode,
            configMode: props.configMode,
            noCheckRuleFlag: skipValidationFlag.value,
            delPayWayCodes: deletedPayWayCodes
          }
          Object.assign(params, feeRateConfig)
          await payConfigApi.addRateConfig(params)
          if (typeof originSavedListResult === 'object') {
            originSavedList.value = originSavedListResult
          }
          // 通过 emit 通知父组件
          if (emit) {
            emit('success')
          }
          resolve()
        } catch (error) {
          reject(error)
        }
      }, () => {
        reject(new Error('用户取消'))
      })
    })
  }

  return {
    loading,
    currentChannelCode,
    readonlyFeeTypes,
    editableFeeTypes,
    allPayWayList,
    allPayWayMap,
    skipValidationFlag,
    rateConfig,
    feeGroups,
    getRateConfig,
    readDefaultFeeRate,
    onStateChange,
    onApplymentSupportChange,
    onFeeTypeChange,
    onLevelModeChange,
    onAmountInput,
    updateFeeRate,
    updateMinFee,
    updateMaxFee,
    updateGroupFeeRate,
    updateGroupMinFee,
    updateGroupMaxFee,
    updateGroupSingleFeeRate,
    addLevelItem,
    deleteLevelItem,
    getFeeTypeName,
    onSubmit
  }
}
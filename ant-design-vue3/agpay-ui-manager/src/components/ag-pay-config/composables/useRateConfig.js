import { ref, reactive } from 'vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { infoBox } from '@/utils/info-box'
import { message } from 'ant-design-vue'

const FEE_TYPE_CODES = {
  MAIN_FEE: 'mainFee',
  AGENT_DEF_FEE: 'agentdefFee',
  MCH_APPLY_DEF_FEE: 'mchapplydefFee',
  READONLY_ISV_COST: 'readonlyIsvCost',
  READONLY_PARENT_AGENT: 'readonlyParentAgent',
  READONLY_PARENT_DEF_RATE: 'readonlyParentDefRate'
}

const LEVEL_MODE = {
  NORMAL: 'NORMAL',
  UNIONPAY: 'UNIONPAY'
}

const BANK_CARD_TYPE = {
  DEBIT: 'DEBIT',
  CREDIT: 'CREDIT'
}

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

export function useRateConfig(props) {
  const loading = ref(false)
  const currentChannelCode = ref(props.ifCode)
  const readonlyFeeTypes = ref([])
  const editableFeeTypes = ref([])
  const allPayWayList = ref([])
  const allPayWayMap = ref({})
  const skipValidationFlag = ref(0)
  const originSavedList = ref([])

  const rateConfig = reactive({
    [FEE_TYPE_CODES.MAIN_FEE]: {},
    [FEE_TYPE_CODES.AGENT_DEF_FEE]: {},
    [FEE_TYPE_CODES.MCH_APPLY_DEF_FEE]: {},
    [FEE_TYPE_CODES.READONLY_ISV_COST]: {},
    [FEE_TYPE_CODES.READONLY_PARENT_AGENT]: {},
    [FEE_TYPE_CODES.READONLY_PARENT_DEF_RATE]: {}
  })

  const feeGroups = reactive(FEE_GROUP_DEFINITIONS.map(def => ({
    ...def,
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedPayWayList: [],
    readonlyIsvCost: null,
    readonlyParentAgent: null,
    readonlyParentDefRate: null
  })))

  const createRateConfig = (wayCode) => ({
    wayCode,
    state: 0,
    applymentSupport: 0,
    feeType: 'SINGLE'
  })

  const createNormalLevel = (id1, id2) => [{
    minFee: 0,
    maxFee: 99999,
    levelList: [
      { id: id1, minAmount: 0, maxAmount: 1000, feeRate: null },
      { id: id2, minAmount: 1000, maxAmount: 999999.99, feeRate: null }
    ]
  }]

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

  const createLevelItem = (id) => ({
    id,
    minAmount: null,
    maxAmount: null,
    fee: null
  })

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

  const onStateChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked
    
    if (wayCode) {
      editableFeeTypes.value.map(item => {
        if (isChecked && !rateConfig[item][wayCode]) {
          rateConfig[item][wayCode] = createRateConfig(wayCode)
        } else {
          rateConfig[item][wayCode].state = +isChecked
        }
      })
    }
    
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.map(item => {
        if (isChecked && !feeGroup[item]) {
          feeGroup[item] = createRateConfig(wayCode)
        } else {
          feeGroup[item].state = +isChecked
        }
      })
    }
  }

  const onApplymentSupportChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked
    
    if (wayCode) {
      editableFeeTypes.value.map(item => {
        rateConfig[item][wayCode].applymentSupport = +isChecked
      })
    }
    
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.map(item => {
        if (feeGroup[item]) {
          feeGroup[item].applymentSupport = +isChecked
        }
      })
    }
  }

  const onFeeTypeChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked
    const currentTime = new Date()
    const id1 = currentTime.getTime()
    currentTime.setSeconds(currentTime.getSeconds() + 1)
    const id2 = currentTime.getTime()
    
    if (wayCode) {
      editableFeeTypes.value.map(item => {
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
      editableFeeTypes.value.map(item => {
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

  const onLevelModeChange = (wayCode, checked, feeGroup) => {
    const isChecked = typeof checked === 'boolean' ? checked : checked.target?.checked
    const currentTime = new Date()
    const id1 = currentTime.getTime()
    currentTime.setSeconds(currentTime.getSeconds() + 1)
    const id2 = currentTime.getTime()
    
    if (wayCode) {
      editableFeeTypes.value.map(item => {
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
      editableFeeTypes.value.map(item => {
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

  const onAmountInput = (wayCode, flag, id, amount, feeGroup) => {
    if (wayCode) {
      editableFeeTypes.value.map(item => {
        rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
      })
    }
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.map(item => {
        feeGroup[item][LEVEL_MODE.NORMAL][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
      })
    }
  }

  const updateFeeRate = (feeType, wayCode, bankCardType, levelKey, value) => {
    const target = rateConfig[feeType][wayCode]?.[rateConfig.mainFee[wayCode]?.levelMode]
      ?.find(f => f.bankCardType === bankCardType)?.levelList[levelKey]
    if (target) target.feeRate = value
  }

  const updateMinFee = (feeType, wayCode, levelModeKey, value) => {
    const target = rateConfig[feeType][wayCode]?.[rateConfig.mainFee[wayCode]?.levelMode]?.[levelModeKey]
    if (target) target.minFee = value
  }

  const updateMaxFee = (feeType, wayCode, levelModeKey, value) => {
    const target = rateConfig[feeType][wayCode]?.[rateConfig.mainFee[wayCode]?.levelMode]?.[levelModeKey]
    if (target) target.maxFee = value
  }

  const updateGroupFeeRate = (feeType, feeGroup, bankCardType, levelKey, value) => {
    const target = feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]
      ?.find(f => f.bankCardType === bankCardType)?.levelList[levelKey]
    if (target) target.feeRate = value
  }

  const updateGroupMinFee = (feeType, feeGroup, levelModeKey, value) => {
    const target = feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]?.[levelModeKey]
    if (target) target.minFee = value
  }

  const updateGroupMaxFee = (feeType, feeGroup, levelModeKey, value) => {
    const target = feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]?.[levelModeKey]
    if (target) target.maxFee = value
  }

  const updateGroupSingleFeeRate = (feeType, feeGroup, value) => {
    if (feeGroup[feeType]) feeGroup[feeType].feeRate = value
  }

  const addLevelItem = (wayCode, feeGroup) => {
    const id = new Date().getTime()
    if (wayCode) {
      editableFeeTypes.value.map(item => {
        rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList.push(createLevelItem(id))
      })
    }
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.map(item => {
        feeGroup[item][LEVEL_MODE.NORMAL][0].levelList.push(createLevelItem(id))
      })
    }
  }

  const deleteLevelItem = (wayCode, id, feeGroup) => {
    if (wayCode) {
      editableFeeTypes.value.map(item => {
        rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList =
            rateConfig[item][wayCode][LEVEL_MODE.NORMAL][0].levelList.filter(item => item.id !== id)
      })
    }
    if (!wayCode && feeGroup) {
      editableFeeTypes.value.map(item => {
        feeGroup[item][LEVEL_MODE.NORMAL][0].levelList =
            feeGroup[item][LEVEL_MODE.NORMAL][0].levelList.filter(item => item.id !== id)
      })
    }
  }

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

  const singleValidate = (fee) => {
    if (isNaN(+fee.feeRate) || fee.feeRate === '' || +fee.feeRate <= 0) {
      message.error('请录入费率')
      return false
    }
    fee.feeRate = Number.parseFloat((+fee.feeRate / 100).toFixed(6))
    return true
  }

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

  const getRateConfig = async (currentIfCodeVal) => {
    if (loading.value) return
    loading.value = true
    
    if (currentIfCodeVal) {
      currentChannelCode.value = currentIfCodeVal
    }
    
    editableFeeTypes.value = []
    readonlyFeeTypes.value = []
    allPayWayList.value = []
    allPayWayMap.value = {}
    
    Object.keys(rateConfig).forEach(key => {
      rateConfig[key] = {}
    })
    
    originSavedList.value = []
    
    feeGroups.forEach(item => {
      item.selectedPayWayList = []
      item.mainFee = {}
      item.agentdefFee = {}
      item.mchapplydefFee = {}
      item.readonlyIsvCost = null
      item.readonlyParentAgent = null
      item.readonlyParentDefRate = null
      item.isMergeMode = false
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
    
    loading.value = false
  }

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
          props.callbackFunc()
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
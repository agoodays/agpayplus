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
    }
  }
}

/**
 * 各项目覆盖默认配置
 * 只写与 defaultConfig 不同的字段，其余继承 defaultConfig
 */
const projectOverrides = {
  agent: {},
  merchant: {
    views: {
      mchStorePage: {
        hiddenColumns: ['mchNo', 'mchName']
      },
      payOrderPage: {
        showPayInterfaceFilter: false,
        hiddenColumns: ['ifCode']
      }
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
      }
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

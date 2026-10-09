import { createProjectConfig } from '../../../shared/config/project-config-core'

// 只声明 Manager 相对 shared 默认配置的差异，其余配置继承默认值。
const projectConfig = createProjectConfig({
  views: {
    // ---- 商户门店详情 ----
    // 运营端保留商户信息，只隐藏代理商和服务商归属字段。
    mchStoreDetail: {
      hiddenFields: ['agentNo', 'isvNo']
    },
    // ---- 支付订单列表 ----
    // 运营端暂不开放导出；退款按钮不额外限定权限点。
    payOrderPage: {
      refundPermission: '',
      exportStatus: 'disabled'
    },
    // ---- 退款订单列表 ----
    // 运营端暂不开放导出。
    refundOrderPage: {
      exportStatus: 'disabled'
    },
    // ---- 转账订单列表 ----
    // 运营端暂不开放导出。
    transferOrderPage: {
      exportStatus: 'disabled'
    }
  }
})

export function getProjectConfig() {
  return projectConfig
}

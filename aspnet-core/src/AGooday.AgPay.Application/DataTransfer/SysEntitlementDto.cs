namespace AGooday.AgPay.Application.DataTransfer
{
    public class SysEntitlementDto
    {
        /// <summary>
        /// 权限ID[ENT_功能模块_子模块_操作], eg: ENT_ROLE_LIST_ADD
        /// </summary>
        public string EntId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string EntName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIcon { get; set; }

        /// <summary>
        /// 菜单uri/路由地址
        /// </summary>
        public string MenuUri { get; set; }

        /// <summary>
        /// 组件Name（前后端分离使用）
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// 权限类型 ML-左侧显示菜单, MO-其他菜单, PB-页面/按钮
        /// </summary>
        public string EntType { get; set; }

        /// <summary>
        /// 快速开始菜单 0-否, 1-是
        /// </summary>
        public byte? QuickJump { get; set; }

        /// <summary>
        /// 权限匹配规则
        /// </summary>
        public EntMatchRule MatchRule { get; set; }

        /// <summary>
        /// 状态 0-停用, 1-启用
        /// </summary>
        public byte? State { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 排序字段, 规则：正序
        /// </summary>
        public int? EntSort { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 匹配规则
        /// </summary>
        public class EntMatchRule { 
        
            /// <summary>
            /// 拓展员
            /// </summary>
            public bool EpUserEnt { get; set; }

            /// <summary>
            /// 用户权限规则
            /// ["USER_TYPE_11_INIT","USER_TYPE_12_INIT"]
            /// USER_TYPE_11_INIT-店长默认权限
            /// STORE-门店管理权限
            /// USER_TYPE_12_INIT-店员默认权限
            /// QUICK_PAY-快捷收银权限
            /// REFUND-退款权限
            /// DEVICE-设备管理权限
            /// STATS-统计报表权限
            /// </summary>
            public List<string> UserEntRules { get; set; }

            /// <summary>
            /// 便捷收银台配置
            /// </summary>
            public bool MchSelfCashierEnt { get; set; }

            /// <summary>
            /// 商户类型: 1-普通商户, 2-特约商户(服务商模式)
            /// </summary>
            public byte MchType { get; set; }

            /// <summary>
            /// 商户级别
            /// ["M1"]
            /// M0商户-简单模式（页面简洁，仅基础收款功能）
            /// M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）
            /// </summary>
            public List<string> MchLevel { get; set; }
        }
    }
}

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 代理商信息表
    /// </summary>
    public class AgentInfoUpdateDto
    {
        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 代理商简称
        /// </summary>
        public string AgentShortName { get; set; }

        /// <summary>
        /// 代理商类型: 1-个人, 2-企业
        /// </summary>
        public byte AgentType { get; set; }

        /// <summary>
        /// 等级: 1-一级, 2-二级, 3-三级 ...
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// 上级代理商号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// 是否允许发展下级: 0-否, 1-是
        /// </summary>
        public byte AddAgentFlag { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-正常
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 支付密码
        /// </summary>
        public string Sipw { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 账户类型: ALIPAY_CASH-支付宝转账, WX_CASH-微信零钱, BANK_CARD-银行卡
        /// </summary>
        public string SettAccountType { get; set; }

        /// <summary>
        /// 结算账户名
        /// </summary>
        public string SettAccountName { get; set; }

        /// <summary>
        /// 结算人手机号
        /// </summary>
        public string SettAccountTelphone { get; set; }

        /// <summary>
        /// 结算账号
        /// </summary>
        public string SettAccountNo { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string SettAccountBank { get; set; }

        /// <summary>
        /// 提现配置类型: 1-使用系统默认, 2-自定义
        /// </summary>
        public byte CashoutFeeRuleType { get; set; }

        /// <summary>
        /// 提现手续费规则
        /// </summary>
        public string CashoutFeeRule { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        public string LicenseImg { get; set; }

        /// <summary>
        /// 开户许可证照片
        /// </summary>
        public string PermitImg { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        public string Idcard1Img { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        public string Idcard2Img { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        public string IdcardInHandImg { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        public string BankCardImg { get; set; }
    }
}

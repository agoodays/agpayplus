namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 账户帐单表
    /// </summary>
    public class AccountBillDto
    {
        /// <summary>
        /// 帐单单号
        /// </summary>
        public string BillId { get; set; }

        /// <summary>
        /// PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 运营平台, 代理商名称
        /// </summary>
        public string InfoName { get; set; }

        /// <summary>
        /// PLATFORM-运营平台, AGENT-代理商
        /// </summary>
        public string InfoType { get; set; }

        /// <summary>
        /// 变动前余额,单位分
        /// </summary>
        public long BeforeBalance { get; set; }

        /// <summary>
        /// 变动金额,单位分
        /// </summary>
        public long ChangeAmount { get; set; }

        /// <summary>
        /// 变动后余额,单位分
        /// </summary>
        public long AfterBalance { get; set; }

        /// <summary>
        /// 业务类型: 1-订单佣金计算, 2-退款轧差, 3-佣金提现, 4-人工调账
        /// </summary>
        public byte BizType { get; set; }

        /// <summary>
        /// 账户类型: 1-钱包账户, 2-在途账户
        /// </summary>
        public byte AccountType { get; set; }

        /// <summary>
        /// 关联订单类型: 1-支付订单, 2-退款订单, 3-提现申请订单
        /// </summary>
        public byte RelaBizOrderType { get; set; }

        /// <summary>
        /// 关联订单号
        /// </summary>
        public string RelaBizOrderId { get; set; }

        /// <summary>
        /// 帐单备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}

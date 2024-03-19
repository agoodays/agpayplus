using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 账户帐单表
    /// </summary>
    public class AccountBillQueryDto : DatePageQuery
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
        [BindNever]
        public string InfoName { get; set; }

        /// <summary>
        /// PLATFORM-运营平台, AGENT-代理商
        /// </summary>
        public string InfoType { get; set; }

        /// <summary>
        /// 业务类型: 1-订单佣金计算, 2-退款轧差, 3-佣金提现, 4-人工调账
        /// </summary>
        public byte? BizType { get; set; }

        /// <summary>
        /// 账户类型: 1-钱包账户, 2-在途账户
        /// </summary>
        public byte? AccountType { get; set; }

        /// <summary>
        /// 关联订单类型: 1-支付订单, 2-退款订单, 3-提现申请订单
        /// </summary>
        [BindNever]
        public byte? RelaBizOrderType { get; set; }

        /// <summary>
        /// 关联订单号
        /// </summary>
        public string RelaBizOrderId { get; set; }

        /// <summary>
        /// 帐单备注
        /// </summary>
        [BindNever]
        public string Remark { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 支付订单分润表
    /// </summary>
    [Comment("支付订单分润表")]
    [Table("t_pay_order_profit")]
    public class PayOrderProfit : AbstractTrackableTimestamps
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("id", TypeName = "bigint(20)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号
        /// </summary>
        [Comment("PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号")]
        [Required, Column("info_id", TypeName = "varchar(64)")]
        public string InfoId { get; set; }

        /// <summary>
        /// 运营平台, 代理商名称
        /// </summary>
        [Comment("运营平台, 代理商名称")]
        [Required, Column("info_name", TypeName = "varchar(64)")]
        public string InfoName { get; set; }

        /// <summary>
        /// PLATFORM-运营平台, AGENT-代理商
        /// </summary>
        [Comment("PLATFORM-运营平台, AGENT-代理商")]
        [Required, Column("info_type", TypeName = "varchar(20)")]
        public string InfoType { get; set; }

        /// <summary>
        /// 支付订单号（与t_pay_order对应）
        /// </summary>
        [Comment("支付订单号（与t_pay_order对应）")]
        [Required, Column("pay_order_id", TypeName = "varchar(30)")]
        public string PayOrderId { get; set; }

        /// <summary>
        /// 费率快照
        /// </summary>
        [Comment("费率快照")]
        [Required, Column("fee_rate", TypeName = "decimal(20,6)")]
        public decimal FeeRate { get; set; }

        /// <summary>
        /// 费率快照描述
        /// </summary>
        [Comment("费率快照描述")]
        [Column("fee_rate_desc", TypeName = "varchar(128)")]
        public string FeeRateDesc { get; set; }

        /// <summary>
        /// 手续费(实际手续费),单位分
        /// </summary>
        [Comment("手续费(实际手续费),单位分")]
        [Required, Column("fee_amount", TypeName = "bigint(20)")]
        public long FeeAmount { get; set; }

        /// <summary>
        /// 收单手续费,单位分
        /// </summary>
        [Comment("收单手续费,单位分")]
        [Required, Column("order_fee_amount", TypeName = "bigint(20)")]
        public long OrderFeeAmount { get; set; }

        /// <summary>
        /// 分润点数（利润率）
        /// </summary>
        [Required, Column("profit_rate", TypeName = "decimal(20,6)")]
        public decimal ProfitRate { get; set; }

        /// <summary>
        /// 分润金额(实际分润),单位分
        /// </summary>
        [Comment("分润金额(实际分润),单位分")]
        [Required, Column("profit_amount", TypeName = "bigint(20)")]
        public long ProfitAmount { get; set; }

        /// <summary>
        /// 收单分润金额,单位分
        /// </summary>
        [Comment("收单分润金额,单位分")]
        [Required, Column("order_profit_amount", TypeName = "bigint(20)")]
        public long OrderProfitAmount { get; set; }
    }
}

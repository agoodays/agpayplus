using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 支付费率阶梯配置表
    /// </summary>
    [Comment("支付费率阶梯配置表")]
    [Table("t_pay_rate_level_config")]
    public class PayRateLevelConfig : AbstractTrackableTimestamps
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("id", TypeName = "bigint(20)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 支付费率配置ID
        /// </summary>
        [Comment("支付费率配置ID")]
        [Required, Column("rate_config_id", TypeName = "bigint(20)")]
        public long RateConfigId { get; set; }

        /// <summary>
        /// 银行卡类型: DEBIT-借记卡（储蓄卡）, CREDIT-贷记卡（信用卡）
        /// </summary>
        [Comment("银行卡类型: DEBIT-借记卡（储蓄卡）, CREDIT-贷记卡（信用卡）")]
        [Column("bank_card_type", TypeName = "varchar(20)")]
        public string BankCardType { get; set; }

        /// <summary>
        /// 最小金额: 计算时大于此值
        /// </summary>
        [Comment("最小金额: 计算时大于此值")]
        [Required, Column("min_amount", TypeName = "int(11)")]
        public int MinAmount { get; set; }

        /// <summary>
        /// 最大金额: 计算时小于或等于此值
        /// </summary>
        [Comment("最大金额: 计算时小于或等于此值")]
        [Required, Column("max_amount", TypeName = "int(11)")]
        public int MaxAmount { get; set; }

        /// <summary>
        /// 保底费用
        /// </summary>
        [Comment("保底费用")]
        [Required, Column("min_fee", TypeName = "int(11)")]
        public int MinFee { get; set; }

        /// <summary>
        /// 封顶费用
        /// </summary>
        [Comment("封顶费用")]
        [Required, Column("max_fee", TypeName = "int(11)")]
        public int MaxFee { get; set; }

        /// <summary>
        /// 支付方式费率
        /// </summary>
        [Comment("支付方式费率")]
        [Column("fee_rate", TypeName = "decimal(20,6)")]
        public decimal? FeeRate { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Comment("状态: 0-停用, 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }
    }
}

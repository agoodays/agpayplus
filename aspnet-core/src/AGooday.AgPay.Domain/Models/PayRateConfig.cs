using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 支付费率配置表
    /// </summary>
    [Comment("支付费率配置表")]
    [Table("t_pay_rate_config")]
    public class PayRateConfig
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 配置类型: ISVCOST-服务商低价, AGENTRATE-代理商费率, AGENTDEF-代理商默认费率, MCHAPPLYDEF-商户进件默认费率, MCHRATE-商户费率
        /// </summary>
        [Comment("配置类型: ISVCOST-服务商低价, AGENTRATE-代理商费率, AGENTDEF-代理商默认费率, MCHAPPLYDEF-商户进件默认费率, MCHRATE-商户费率")]
        [Required, Column("config_type", TypeName = "varchar(20)")]
        public string ConfigType { get; set; }

        /// <summary>
        /// 账号类型:ISV-服务商, ISV_OAUTH2-服务商oauth2, AGENT-代理商, MCH_APP-商户应用, MCH_APP_OAUTH2-商户应用oauth2
        /// </summary>
        [Comment("账号类型:ISV-服务商, ISV_OAUTH2-服务商oauth2, AGENT-代理商, MCH_APP-商户应用, MCH_APP_OAUTH2-商户应用oauth2")]
        [Required, Column("info_type", TypeName = "varchar(20)")]
        public string InfoType { get; set; }

        /// <summary>
        /// 服务商号/代理商号/商户号/应用ID
        /// </summary>
        [Comment("服务商号/代理商号/商户号/应用ID")]
        [Required, Column("info_id", TypeName = "varchar(64)")]
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        [Comment("支付接口")]
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [Comment("支付方式")]
        [Required, Column("way_code", TypeName = "varchar(20)")]
        public string WayCode { get; set; }

        /// <summary>
        /// 费率类型:SINGLE-单笔费率, LEVEL-阶梯费率
        /// </summary>
        [Comment("费率类型:SINGLE-单笔费率, LEVEL-阶梯费率")]
        [Required, Column("fee_type", TypeName = "varchar(20)")]
        public string FeeType { get; set; }

        /// <summary>
        /// 阶梯模式: 模式: NORMAL-普通模式, UNIONPAY-银联模式
        /// </summary>
        [Comment("阶梯模式: 模式: NORMAL-普通模式, UNIONPAY-银联模式")]
        [Column("level_mode", TypeName = "varchar(20)")]
        public string LevelMode { get; set; }

        /// <summary>
        /// 支付方式费率
        /// </summary>
        [Comment("支付方式费率")]
        [Column("fee_rate", TypeName = "decimal(20,6)")]
        public decimal? FeeRate { get; set; }

        /// <summary>
        /// 是否支持进件: 0-不支持, 1-支持
        /// </summary>
        [Comment("是否支持进件: 0-不支持, 1-支持")]
        [Required, Column("applyment_support", TypeName = "tinyint(6)")]
        public byte ApplymentSupport { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Comment("状态: 0-停用, 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Comment("商户支付通道表")]
    [Table("t_mch_pay_passage")]
    public class MchPayPassage
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID")]
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

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
        /// 支付方式费率
        /// </summary>
        [Comment("支付方式费率")]
        [Required, Column("rate", TypeName = "decimal(20,6)")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 风控数据
        /// </summary>
        [Comment("风控数据")]
        [Column("risk_config", TypeName = "json")]
        public string RiskConfig { get; set; }

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

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
    /// 商户支付通道表
    /// </summary>
    [Table("t_mch_pay_passage")]
    public class MchPayPassage
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, Required, Column("id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [Required, Column("way_codes", TypeName = "varchar(20)")]
        public string WayCode { get; set; }

        /// <summary>
        /// 支付方式费率
        /// </summary>
        [Required, Column("rate", TypeName = "decimal(20,6)")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 风控数据
        /// </summary>
        [Column("risk_config", TypeName = "json")]
        public JsonObject RiskConfig { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Required, Column("state", TypeName = "tinyint")]
        public byte State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required, Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Required, Column("updated_at", TypeName = "timestamp")]
        public DateTime UpdatedAt { get; set; }
    }
}

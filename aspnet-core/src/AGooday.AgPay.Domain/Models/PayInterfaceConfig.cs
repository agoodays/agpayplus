using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 支付接口配置参数表
    /// </summary>
    [Comment("支付接口配置参数表")]
    [Table("t_pay_interface_config")]
    public class PayInterfaceConfig
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 账号类型:1-服务商 2-商户
        /// </summary>
        [Comment("账号类型:1-服务商 2-商户")]
        [Required, Column("info_type", TypeName = "tinyint(6)")]
        public byte InfoType { get; set; }

        /// <summary>
        /// 服务商或商户No
        /// </summary>
        [Comment("服务商或商户No")]
        [Required, Column("info_id", TypeName = "varchar(64)")]
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        [Comment("支付接口")]
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 接口配置参数,json字符串
        /// </summary>
        [Comment("接口配置参数,json字符串")]
        [Required, Column("if_params", TypeName = "varchar(4096)")]
        public string IfParams { get; set; }

        /// <summary>
        /// 支付接口费率
        /// </summary>
        [Comment("支付接口费率")]
        [Column("if_rate", TypeName = "decimal(20,6)")]
        public decimal? IfRate { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Comment("状态: 0-停用, 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新者用户ID
        /// </summary>
        [Comment("更新者用户ID")]
        [Column("updated_uid", TypeName = "bigint")]
        public long? UpdatedUid { get; set; }

        /// <summary>
        /// 更新者姓名
        /// </summary>
        [Comment("更新者姓名")]
        [Column("updated_by", TypeName = "varchar(64)")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}

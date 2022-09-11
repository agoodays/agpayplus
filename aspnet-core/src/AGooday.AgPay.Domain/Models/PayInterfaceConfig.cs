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
    [Table("t_pay_interface_config")]
    public class PayInterfaceConfig
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, Required, Column("sys_user_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 账号类型:1-服务商 2-商户
        /// </summary>
        [Required, Column("info_type", TypeName = "tinyint")]
        public byte InfoType { get; set; }

        /// <summary>
        /// 服务商或商户No
        /// </summary>
        [Required, Column("info_id", TypeName = "varchar(64)")]
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 接口配置参数,json字符串
        /// </summary>
        [Required, Column("if_params", TypeName = "varchar(4096)")]
        public string IfParams { get; set; }

        /// <summary>
        /// 支付接口费率
        /// </summary>
        [Required, Column("if_rate", TypeName = "decimal(20,6)")]
        public decimal IfRate { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Required, Column("state", TypeName = "tinyint")]
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required, Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新者用户ID
        /// </summary>
        [Column("updated_uid", TypeName = "bigint")]
        public long? UpdatedUid { get; set; }

        /// <summary>
        /// 更新者姓名
        /// </summary>
        [Column("updated_by", TypeName = "varchar(64)")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Required, Column("updated_at", TypeName = "timestamp")]
        public DateTime UpdatedAt { get; set; }
    }
}

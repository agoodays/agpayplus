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
    /// 商户分账接收者账号绑定关系表
    /// </summary>
    [Table("t_mch_division_receiver")]
    public class MchDivisionReceiver
    {
        /// <summary>
        /// 分账接收者ID
        /// </summary>
        [Key, Required, Column("receiver_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long ReceiverId { get; set; }

        /// <summary>
        /// 接收者账号别名
        /// </summary>
        [Required, Column("receiver_alias", TypeName = "varchar(64)")]
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 组ID（便于商户接口使用）
        /// </summary>
        [Required, Column("receiver_group_id", TypeName = "bigint")]
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Required, Column("receiver_group_name", TypeName = "varchar(64)")]
        public string ReceiverGroupName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用iD
        /// </summary>
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 分账接收账号类型: 0-个人(对私) 1-商户(对公)
        /// </summary>
        [Required, Column("acc_type", TypeName = "tinyint")]
        public byte AccType { get; set; }

        /// <summary>
        /// 分账接收账号
        /// </summary>
        [Required, Column("acc_no", TypeName = "varchar(50)")]
        public string AccNo { get; set; }

        /// <summary>
        /// 分账接收账号名称
        /// </summary>
        [Required, Column("acc_name", TypeName = "varchar(30)")]
        public string AccName { get; set; }

        /// <summary>
        /// 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等
        /// </summary>
        [Required, Column("relation_type", TypeName = "varchar(30)")]
        public string RelationType { get; set; }

        /// <summary>
        /// 当选择自定义时，需要录入该字段。 否则为对应的名称
        /// </summary>
        [Required, Column("relation_type_name", TypeName = "varchar(30)")]
        public string RelationTypeName { get; set; }

        /// <summary>
        /// 分账比例
        /// </summary>
        [Column("division_profit", TypeName = "decimal(20,6)")]
        public decimal? DivisionProfit { get; set; }

        /// <summary>
        /// 分账状态（本系统状态，并不调用上游关联关系）: 1-正常分账, 0-暂停分账
        /// </summary>
        [Required, Column("state", TypeName = "tinyint")]
        public byte State { get; set; }

        /// <summary>
        /// 上游绑定返回信息，一般用作查询绑定异常时的记录
        /// </summary>
        [Required, Column("channel_bind_result", TypeName = "text")]
        public string ChannelBindResult { get; set; }

        /// <summary>
        /// 渠道特殊信息
        /// </summary>
        [Required, Column("channel_ext_info", TypeName = "text")]
        public string ChannelExtInfo { get; set; }

        /// <summary>
        /// 绑定成功时间
        /// </summary>
        [Column("bind_success_time", TypeName = "datetime")]
        public DateTime? BindSuccessTime { get; set; }

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

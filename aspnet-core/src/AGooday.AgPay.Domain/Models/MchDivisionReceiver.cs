using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户分账接收者账号绑定关系表
    /// </summary>
    [Table("t_mch_division_receiver")]
    [Comment("商户分账接收者账号绑定关系表")]
    public class MchDivisionReceiver : AbstractTrackableTimestamps
    {
        /// <summary>
        /// 分账接收者ID
        /// </summary>
        [Comment("分账接收者ID")]
        [Key, Required, Column("receiver_id", TypeName = "bigint(20)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long ReceiverId { get; set; }

        /// <summary>
        /// 接收者账号别名
        /// </summary>
        [Comment("接收者账号别名")]
        [Required, Column("receiver_alias", TypeName = "varchar(64)")]
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 组ID（便于商户接口使用）
        /// </summary>
        [Comment("组ID（便于商户接口使用）")]
        [Required, Column("receiver_group_id", TypeName = "bigint(20)")]
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Comment("组名称")]
        [Required, Column("receiver_group_name", TypeName = "varchar(64)")]
        public string ReceiverGroupName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID")]
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Comment("支付接口代码")]
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 分账接收账号类型: 0-个人(对私) 1-商户(对公)
        /// </summary>
        [Comment("分账接收账号类型: 0-个人(对私) 1-商户(对公)")]
        [Required, Column("acc_type", TypeName = "tinyint(6)")]
        public byte AccType { get; set; }

        /// <summary>
        /// 分账接收账号
        /// </summary>
        [Comment("分账接收账号")]
        [Required, Column("acc_no", TypeName = "varchar(50)")]
        public string AccNo { get; set; }

        /// <summary>
        /// 分账接收账号名称
        /// </summary>
        [Comment("分账接收账号名称")]
        [Required, Column("acc_name", TypeName = "varchar(30)")]
        public string AccName { get; set; }

        /// <summary>
        /// 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等
        /// </summary>
        [Comment("分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等")]
        [Required, Column("relation_type", TypeName = "varchar(30)")]
        public string RelationType { get; set; }

        /// <summary>
        /// 当选择自定义时，需要录入该字段。 否则为对应的名称
        /// </summary>
        [Comment("当选择自定义时，需要录入该字段。 否则为对应的名称")]
        [Required, Column("relation_type_name", TypeName = "varchar(30)")]
        public string RelationTypeName { get; set; }

        /// <summary>
        /// 分账比例
        /// </summary>
        [Comment("分账比例")]
        [Column("division_profit", TypeName = "decimal(20,6)")]
        public decimal? DivisionProfit { get; set; }

        /// <summary>
        /// 分账状态（本系统状态，并不调用上游关联关系）: 1-正常分账, 0-暂停分账
        /// </summary>
        [Comment("分账状态（本系统状态，并不调用上游关联关系）: 1-正常分账, 0-暂停分账")]
        [Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 渠道账号信息
        /// </summary>
        [Comment("渠道账号信息")]
        [Column("channel_acc_no", TypeName = "text")]
        public string ChannelAccNo { get; set; }

        /// <summary>
        /// 上游绑定返回信息，一般用作查询绑定异常时的记录
        /// </summary>
        [Comment("上游绑定返回信息，一般用作查询绑定异常时的记录")]
        [Column("channel_bind_result", TypeName = "text")]
        public string ChannelBindResult { get; set; }

        /// <summary>
        /// 渠道特殊信息
        /// </summary>
        [Comment("渠道特殊信息")]
        [Column("channel_ext_info", TypeName = "text")]
        public string ChannelExtInfo { get; set; }

        /// <summary>
        /// 绑定成功时间
        /// </summary>
        [Comment("绑定成功时间")]
        [Column("bind_success_time", TypeName = "datetime")]
        public DateTime? BindSuccessTime { get; set; }
    }
}

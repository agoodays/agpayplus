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
    /// 分账记录表
    /// </summary>
    [Comment("分账接收者ID")]
    [Table("分账记录表")]
    public class PayOrderDivisionRecord
    {
        /// <summary>
        /// 分账记录ID
        /// </summary>
[Comment("分账记录ID")]
        [Key, Required, Column("record_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long RecordId { get; set; }

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
        /// 商户名称
        /// </summary>
[Comment("商户名称")]
        [Required, Column("mch_name", TypeName = "varchar(30)")]
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
[Comment("类型: 1-普通商户, 2-特约商户(服务商模式)")]
        [Required, Column("mch_type", TypeName = "tinyint(6)")]
        public byte MchType { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
[Comment("支付接口代码")]
        [Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
[Comment("支付订单号")]
        [Required, Column("pay_order_id", TypeName = "varchar(30)")]
        public string PayOrderId { get; set; }

        /// <summary>
        /// 支付订单渠道支付订单号
        /// </summary>
[Comment("支付订单渠道支付订单号")]
        [Column("pay_order_channel_order_no", TypeName = "varchar(64)")]
        public string PayOrderChannelOrderNo { get; set; }

        /// <summary>
        /// 订单金额,单位分
        /// </summary>
[Comment("订单金额,单位分")]
        [Required, Column("pay_order_amount", TypeName = "bigint")]
        public long PayOrderAmount { get; set; }

        /// <summary>
        /// 订单实际分账金额, 单位：分（订单金额 - 商户手续费 - 已退款金额）
        /// </summary>
[Comment("订单实际分账金额, 单位：分（订单金额 - 商户手续费 - 已退款金额）")]
        [Required, Column("pay_order_division_amount", TypeName = "bigint")]
        public long PayOrderDivisionAmount { get; set; }

        /// <summary>
        /// 系统分账批次号
        /// </summary>
[Comment("系统分账批次号")]
        [Required, Column("batch_order_id", TypeName = "varchar(30)")]
        public string BatchOrderId { get; set; }

        /// <summary>
        /// 上游分账批次号
        /// </summary>
[Comment("上游分账批次号")]
        [Column("channel_batch_order_id", TypeName = "varchar(64)")]
        public string ChannelBatchOrderId { get; set; }

        /// <summary>
        /// 状态: 0-待分账 1-分账成功, 2-分账失败
        /// </summary>
[Comment("状态: 0-待分账 1-分账成功, 2-分账失败")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 上游返回数据包
        /// </summary>
[Comment("上游返回数据包")]
        [Column("channel_resp_result", TypeName = "text")]
        public string ChannelRespResult { get; set; }

        /// <summary>
        /// 账号快照》 分账接收者ID
        /// </summary>
[Comment("账号快照》 分账接收者ID")]
        [Required, Column("receiver_id", TypeName = "bigint")]
        public long ReceiverId { get; set; }

        /// <summary>
        /// 账号快照》 组ID（便于商户接口使用）
        /// </summary>
[Comment("账号快照》 组ID（便于商户接口使用）")]
        [Column("receiver_group_id", TypeName = "bigint")]
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 账号快照》 分账接收者别名
        /// </summary>
[Comment("账号快照》 分账接收者别名")]
        [Column("receiver_alias", TypeName = "varchar(64)")]
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 账号快照》 分账接收账号类型: 0-个人 1-商户
        /// </summary>
[Comment("账号快照》 分账接收账号类型: 0-个人 1-商户")]
        [Required, Column("acc_type", TypeName = "tinyint(6)")]
        public byte AccType { get; set; }

        /// <summary>
        /// 账号快照》 分账接收账号
        /// </summary>
[Comment("账号快照》 分账接收账号")]
        [Required, Column("acc_no", TypeName = "varchar(50)")]
        public string AccNo { get; set; }

        /// <summary>
        /// 账号快照》 分账接收账号名称
        /// </summary>
[Comment("账号快照》 分账接收账号名称")]
        [Required, Column("acc_name", TypeName = "varchar(30)")]
        public string AccName { get; set; }

        /// <summary>
        /// 账号快照》 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等
        /// </summary>
[Comment("账号快照》 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等")]
        [Required, Column("relation_type", TypeName = "varchar(30)")]
        public string RelationType { get; set; }

        /// <summary>
        /// 账号快照》 当选择自定义时，需要录入该字段。 否则为对应的名称
        /// </summary>
[Comment("账号快照》 当选择自定义时，需要录入该字段。 否则为对应的名称")]
        [Required, Column("relation_type_name", TypeName = "varchar(30)")]
        public string RelationTypeName { get; set; }

        /// <summary>
        /// 账号快照》 配置的实际分账比例
        /// </summary>
[Comment("账号快照》 配置的实际分账比例")]
        [Required, Column("division_profit", TypeName = "decimal(20,6)")]
        public decimal DivisionProfit { get; set; }

        /// <summary>
        /// 计算该接收方的分账金额,单位分
        /// </summary>
[Comment("计算该接收方的分账金额,单位分")]
        [Required, Column("cal_division_amount", TypeName = "bigint")]
        public long CalDivisionAmount { get; set; }

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

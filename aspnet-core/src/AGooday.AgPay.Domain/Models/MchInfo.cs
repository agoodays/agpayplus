using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    [Comment("商户信息表")]
    [Table("t_mch_info")]
    public class MchInfo
    {
        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Key, Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [Comment("商户名称")]
        [Required, Column("mch_name", TypeName = "varchar(64)")]
        public string MchName { get; set; }

        /// <summary>
        /// 商户简称
        /// </summary>
        [Comment("商户简称")]
        [Required, Column("mch_short_name", TypeName = "varchar(32)")]
        public string MchShortName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [Comment("类型: 1-普通商户, 2-特约商户(服务商模式)")]
        [Required, Column("type", TypeName = "tinyint(6)")]
        public byte Type { get; set; }

        /// <summary>
        /// 商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）
        /// </summary>
        [Comment("商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）")]
        [Required, Column("mch_level", TypeName = "varchar(8)")]
        public string MchLevel { get; set; }

        /// <summary>
        /// 退款方式["plat", "api"],平台退款、接口退款，平台退款方式必须包含接口退款。
        /// </summary>
        [Comment("退款方式[\"plat\", \"api\"],平台退款、接口退款，平台退款方式必须包含接口退款。")]
        [Column("refund_mode", TypeName = "json")]
        public string RefundMode { get; set; }

        /// <summary>
        /// 支付密码
        /// </summary>
        [Comment("支付密码")]
        [Column("sipw", TypeName = "varchar(128)")]
        public string Sipw { get; set; }

        /// <summary>
        /// 顶级代理商号
        /// </summary>
        [Comment("顶级代理商号")]
        [Column("top_agent_no", TypeName = "varchar(64)")]
        public string TopAgentNo { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        [Comment("代理商号")]
        [Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Comment("联系人姓名")]
        [Column("contact_name", TypeName = "varchar(32)")]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [Comment("联系人手机号")]
        [Column("contact_tel", TypeName = "varchar(32)")]
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [Comment("联系人邮箱")]
        [Column("contact_email", TypeName = "varchar(32)")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 商户状态: 0-停用, 1-正常
        /// </summary>
        [Comment("商户状态: 0-停用, 1-正常")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 商户备注
        /// </summary>
        [Comment("商户备注")]
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 初始用户ID（创建商户时，允许商户登录的用户）
        /// </summary>
        [Comment("初始用户ID（创建商户时，允许商户登录的用户）")]
        [Column("init_user_id", TypeName = "bigint")]
        public long? InitUserId { get; set; }

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
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}

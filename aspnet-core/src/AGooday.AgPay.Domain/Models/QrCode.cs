using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 码牌信息表
    /// </summary>
    [Comment("码牌信息表")]
    [Table("t_qr_code")]
    public class QrCode
    {
        /// <summary>
        /// 码牌ID
        /// </summary>
        [Comment("码牌ID")]
        [Key, Required, Column("qrc_id", TypeName = "varchar(64)")]
        public string QrcId { get; set; }

        /// <summary>
        /// 码牌模板ID
        /// </summary>
        [Comment("模板ID")]
        [Column("qrc_shell_id", TypeName = "int")]
        public long? QrcShellId { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        [Comment("批次号")]
        [Required, Column("batch_id", TypeName = "varchar(64)")]
        public string BatchId { get; set; }

        /// <summary>
        /// 是否固定金额: 0-任意金额, 1-固定金额
        /// </summary>
        [Comment("是否固定金额: 0-任意金额, 1-固定金额")]
        [Required, Column("fixed_flag", TypeName = "tinyint(6)")]
        public byte FixedFlag { get; set; }

        /// <summary>
        /// 固定金额
        /// </summary>
        [Comment("固定金额")]
        [Required, Column("fixed_pay_amount", TypeName = "int")]
        public int FixedPayAmount { get; set; }

        /// <summary>
        /// 选择页面类型: default-默认(未指定，取决于二维码是否绑定到微信侧), h5-固定H5页面, lite-固定小程序页面
        /// </summary>
        [Comment("选择页面类型: default-默认(未指定，取决于二维码是否绑定到微信侧), h5-固定H5页面, lite-固定小程序页面")]
        [Required, Column("entry_page", TypeName = "varchar(20)")]
        public string EntryPage { get; set; }

        /// <summary>
        /// 支付宝支付方式(仅H5呈现时生效)
        /// </summary>
        [Comment("支付宝支付方式(仅H5呈现时生效)")]
        [Required, Column("alipay_way_code", TypeName = "varchar(20)")]
        public string AlipayWayCode { get; set; }

        /// <summary>
        /// 码牌别名
        /// </summary>
        [Comment("码牌别名")]
        [Column("qrc_alias", TypeName = "varchar(20)")]
        public string QrcAlias { get; set; }

        /// <summary>
        /// 码牌绑定状态: 0-未绑定, 1-已绑定
        /// </summary>
        [Comment("码牌绑定状态: 0-未绑定, 1-已绑定")]
        [Required, Column("bind_state", TypeName = "tinyint(6)")]
        public byte BindState { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        [Comment("代理商号")]
        [Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID")]
        [Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [Comment("门店ID")]
        [Column("store_id", TypeName = "bigint")]
        public long? StoreId { get; set; }

        /// <summary>
        /// 二维码Url
        /// </summary>
        [Comment("二维码Url")]
        [Required, Column("qr_url", TypeName = "varchar(255)")]
        public string QrUrl { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Comment("状态: 0-停用, 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
        /// </summary>
        [Comment("所属商户ID / 代理商ID / 0(平台)")]
        [Required, Column("belong_info_id", TypeName = "varchar(64)")]
        public string BelongInfoId { get; set; }

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

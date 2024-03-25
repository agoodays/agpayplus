using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 账户帐单表
    /// </summary>
    [Comment("账户帐单表")]
    [Table("t_account_bill")]
    public class AccountBill
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [Comment("流水号")]
        [Key, Required, Column("id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

        /// <summary>
        /// 帐单单号
        /// </summary>
        [Comment("帐单单号")]
        [Required, Column("bill_id", TypeName = "varchar(30)")]
        public string BillId { get; set; }

        /// <summary>
        /// PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号
        /// </summary>
        [Comment("PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号")]
        [Required, Column("info_id", TypeName = "varchar(64)")]
        public string InfoId { get; set; }

        /// <summary>
        /// 运营平台, 代理商名称
        /// </summary>
        [Comment("运营平台, 代理商名称")]
        [Required, Column("info_name", TypeName = "varchar(64)")]
        public string InfoName { get; set; }

        /// <summary>
        /// PLATFORM-运营平台, AGENT-代理商
        /// </summary>
        [Comment("PLATFORM-运营平台, AGENT-代理商")]
        [Required, Column("info_type", TypeName = "varchar(20)")]
        public string InfoType { get; set; }

        /// <summary>
        /// 变动前余额,单位分
        /// </summary>
        [Comment("变动前余额,单位分")]
        [Required, Column("before_balance", TypeName = "bigint")]
        public long BeforeBalance { get; set; }

        /// <summary>
        /// 变动金额,单位分
        /// </summary>
        [Comment("变动金额,单位分")]
        [Required, Column("change_amount", TypeName = "bigint")]
        public long ChangeAmount { get; set; }

        /// <summary>
        /// 变动后余额,单位分
        /// </summary>
        [Comment("变动后余额,单位分")]
        [Required, Column("after_balance", TypeName = "bigint")]
        public long AfterBalance { get; set; }

        /// <summary>
        /// 业务类型: 1-订单佣金计算, 2-退款轧差, 3-佣金提现, 4-人工调账
        /// </summary>
        [Comment("业务类型: 1-订单佣金计算, 2-退款轧差, 3-佣金提现, 4-人工调账")]
        [Required, Column("biz_type", TypeName = "tinyint(6)")]
        public byte BizType { get; set; }

        /// <summary>
        /// 账户类型: 1-钱包账户, 2-在途账户
        /// </summary>
        [Comment("账户类型: 1-钱包账户, 2-在途账户")]
        [Required, Column("account_type", TypeName = "tinyint(6)")]
        public byte AccountType { get; set; }

        /// <summary>
        /// 关联订单类型: 1-支付订单, 2-退款订单, 3-提现申请订单
        /// </summary>
        [Comment("关联订单类型: 1-支付订单, 2-退款订单, 3-提现申请订单")]
        [Required, Column("rela_biz_order_type", TypeName = "tinyint(6)")]
        public byte RelaBizOrderType { get; set; }

        /// <summary>
        /// 关联订单号
        /// </summary>
        [Comment("关联订单号")]
        [Column("rela_biz_order_id", TypeName = "varchar(30)")]
        public string RelaBizOrderId { get; set; }

        /// <summary>
        /// 帐单备注
        /// </summary>
        [Comment("帐单备注")]
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

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

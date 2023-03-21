using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 支付接口定义表
    /// </summary>
    [Comment("支付接口定义表")]
    [Table("t_pay_interface_define")]
    public class PayInterfaceDefine
    {
        /// <summary>
        /// 接口代码 全小写  wxpay alipay 
        /// </summary>
        [Comment("接口代码 全小写  wxpay alipay ")]
        [Key, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        [Comment("接口名称")]
        [Required, Column("if_name", TypeName = "varchar(20)")]
        public string IfName { get; set; }

        /// <summary>
        /// 是否支持普通商户模式: 0-不支持, 1-支持
        /// </summary>
        [Comment("是否支持普通商户模式: 0-不支持, 1-支持")]
        [Required, Column("is_mch_mode", TypeName = "tinyint(6)")]
        public byte IsMchMode { get; set; }

        /// <summary>
        /// 是否支持服务商子商户模式: 0-不支持, 1-支持
        /// </summary>
        [Comment("是否支持服务商子商户模式: 0-不支持, 1-支持")]
        [Required, Column("is_isv_mode", TypeName = "tinyint(6)")]
        public byte IsIsvMode { get; set; }

        /// <summary>
        /// 支付参数配置页面类型:1-JSON渲染,2-自定义
        /// </summary>
        [Comment("支付参数配置页面类型:1-JSON渲染,2-自定义")]
        [Required, Column("config_page_type", TypeName = "tinyint(6)")]
        public byte ConfigPageType { get; set; }

        /// <summary>
        /// 是否支持进件: 0-不支持, 1-支持
        /// </summary>
        [Comment("是否支持进件: 0-不支持, 1-支持")]
        [Required, Column("is_support_applyment", TypeName = "tinyint(6)")]
        public byte IsSupportApplyment { get; set; }

        /// <summary>
        /// 是否开启进件: 0-关闭, 1-开启
        /// </summary>
        [Comment("是否开启进件: 0-关闭, 1-开启")]
        [Required, Column("is_open_applyment", TypeName = "tinyint(6)")]
        public byte IsOpenApplyment { get; set; }

        /// <summary>
        /// 是否支持对账: 0-不支持, 1-支持
        /// </summary>
        [Comment("是否支持对账: 0-不支持, 1-支持")]
        [Required, Column("is_support_check_bill", TypeName = "tinyint(6)")]
        public byte IsSupportCheckBill { get; set; }

        /// <summary>
        /// 是否开启对账: 0-关闭, 1-开启
        /// </summary>
        [Comment("是否开启对账: 0-关闭, 1-开启")]
        [Required, Column("is_open_check_bill", TypeName = "tinyint(6)")]
        public byte IsOpenCheckBill { get; set; }

        /// <summary>
        /// 是否支持提现: 0-不支持, 1-支持
        /// </summary>
        [Comment("是否支持提现: 0-不支持, 1-支持")]
        [Required, Column("is_support_cashout", TypeName = "tinyint(6)")]
        public byte IsSupportCashout { get; set; }

        /// <summary>
        /// 是否开启提现: 0-关闭, 1-开启
        /// </summary>
        [Comment("是否开启提现: 0-关闭, 1-开启")]
        [Required, Column("is_open_cashout", TypeName = "tinyint(6)")]
        public byte IsOpenCashout { get; set; }

        /// <summary>
        /// ISV接口配置定义描述,json字符串
        /// </summary>
        [Comment("ISV接口配置定义描述,json字符串")]
        [Column("isv_params", TypeName = "varchar(4096)")]
        public string IsvParams { get; set; }

        /// <summary>
        /// 特约商户接口配置定义描述,json字符串
        /// </summary>
        [Comment("特约商户接口配置定义描述,json字符串")]
        [Column("isvsub_mch_params", TypeName = "varchar(4096)")]
        public string IsvsubMchParams { get; set; }

        /// <summary>
        /// 普通商户接口配置定义描述,json字符串
        /// </summary>
        [Comment("普通商户接口配置定义描述,json字符串")]
        [Column("normal_mch_params", TypeName = "varchar(4096)")]
        public string NormalMchParams { get; set; }

        /// <summary>
        /// 支持的支付方式 ["wxpay_jsapi", "wxpay_bar"]
        /// </summary>
        [Comment("支持的支付方式 [\"wxpay_jsapi\", \"wxpay_bar\"]")]
        [Required, Column("way_codes", TypeName = "json")]
        public string WayCodes { get; set; }

        /// <summary>
        /// 页面展示：卡片-图标
        /// </summary>
        [Comment("页面展示：卡片-图标")]
        [Column("icon", TypeName = "varchar(256)")]
        public string Icon { get; set; }

        /// <summary>
        /// 页面展示：卡片-背景色
        /// </summary>
        [Comment("页面展示：卡片-背景色")]
        [Column("bg_color", TypeName = "varchar(20)")]
        public string BgColor { get; set; }

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

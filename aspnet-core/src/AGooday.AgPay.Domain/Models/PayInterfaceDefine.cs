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
    /// 支付接口定义表
    /// </summary>
    [Table("t_pay_interface_define")]
    public class PayInterfaceDefine
    {
        /// <summary>
        /// 接口代码 全小写  wxpay alipay 
        /// </summary>
        [Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        [Required, Column("if_name", TypeName = "varchar(20)")]
        public string IfName { get; set; }

        /// <summary>
        /// 是否支持普通商户模式: 0-不支持, 1-支持
        /// </summary>
        [Required, Column("is_mch_mode", TypeName = "tinyint(6)")]
        public byte IsMchMode { get; set; }

        /// <summary>
        /// 是否支持服务商子商户模式: 0-不支持, 1-支持
        /// </summary>
        [Required, Column("is_isv_mode", TypeName = "tinyint(6)")]
        public byte IsIsvMode { get; set; }

        /// <summary>
        /// 支付参数配置页面类型:1-JSON渲染,2-自定义
        /// </summary>
        [Required, Column("config_page_type", TypeName = "tinyint(6)")]
        public byte ConfigPageType { get; set; }

        /// <summary>
        /// ISV接口配置定义描述,json字符串
        /// </summary>
        [Column("isv_params", TypeName = "varchar(4096)")]
        public string IsvParams { get; set; }

        /// <summary>
        /// 特约商户接口配置定义描述,json字符串
        /// </summary>
        [Column("isvsub_mch_params", TypeName = "varchar(4096)")]
        public string IsvsubMchParams { get; set; }

        /// <summary>
        /// 普通商户接口配置定义描述,json字符串
        /// </summary>
        [Column("normal_mch_params", TypeName = "varchar(4096)")]
        public string NormalMchParams { get; set; }

        /// <summary>
        /// 支持的支付方式 ["wxpay_jsapi", "wxpay_bar"]
        /// </summary>
        [Required, Column("way_codes", TypeName = "json")]
        public JsonArray WayCodes { get; set; }

        /// <summary>
        /// 页面展示：卡片-图标
        /// </summary>
        [ Column("icon", TypeName = "varchar(256)")]
        public string Icon { get; set; }

        /// <summary>
        /// 页面展示：卡片-背景色
        /// </summary>
        [Column("bg_color", TypeName = "varchar(20)")]
        public string BgColor { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

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

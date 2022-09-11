using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    /// <summary>
    /// 支付接口定义表
    /// </summary>
    public class PayInterfaceDefineVM
    {
        /// <summary>
        /// 接口代码 全小写  wxpay alipay 
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string IfName { get; set; }

        /// <summary>
        /// 是否支持普通商户模式: 0-不支持, 1-支持
        /// </summary>
        public byte IsMchMode { get; set; }

        /// <summary>
        /// 是否支持服务商子商户模式: 0-不支持, 1-支持
        /// </summary>
        public byte IsIsvMode { get; set; }

        /// <summary>
        /// 支付参数配置页面类型:1-JSON渲染,2-自定义
        /// </summary>
        public byte ConfigPageType { get; set; }

        /// <summary>
        /// ISV接口配置定义描述,json字符串
        /// </summary>
        public string IsvParams { get; set; }

        /// <summary>
        /// 特约商户接口配置定义描述,json字符串
        /// </summary>
        public string IsvsubMchParams { get; set; }

        /// <summary>
        /// 普通商户接口配置定义描述,json字符串
        /// </summary>
        public string NormalMchParams { get; set; }

        /// <summary>
        /// 支持的支付方式 ["wxpay_jsapi", "wxpay_bar"]
        /// </summary>
        public string WayCodes { get; set; }

        /// <summary>
        /// 页面展示：卡片-图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 页面展示：卡片-背景色
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}

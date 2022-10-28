using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    public class AvailablePayInterfaceDto
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
        /// 支付参数配置页面类型:1-JSON渲染,2-自定义
        /// </summary>
        public sbyte ConfigPageType { get; set; }

        /// <summary>
        /// 页面展示：卡片-图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 页面展示：卡片-背景色
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// 接口配置参数,json字符串
        /// </summary>
        public string IfParams { get; set; }

        /// <summary>
        /// 支付接口费率
        /// </summary>
        public decimal? IfRate { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public long? PassageId { get; set; }

        /// <summary>
        /// 通道费率
        /// </summary>
        public decimal? Rate { get; set; }

        /// <summary>
        /// 通道状态: 0-停用, 1-启用
        /// </summary>
        public sbyte? State { get; set; }
    }
}

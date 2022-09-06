using AGooday.AgPay.Payment.Api.RQRS.Msg;
using System.Text.Json.Serialization;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    /// <summary>
    /// 创建订单(统一订单) 响应参数
    /// </summary>
    public class UnifiedOrderRS : AbstractRS
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string payOrderId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string mchOrderNo { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public byte orderState { get; set; }

        /// <summary>
        /// 支付参数类型  ( 无参数，  调起支付插件参数， 重定向到指定地址，  用户扫码   ) 
        /// </summary>
        public string payDataType { get; set; }

        /// <summary>
        /// 支付参数
        /// </summary>
        public string payData { get; set; }

        /// <summary>
        /// 渠道返回错误代码
        /// </summary>
        public string errCode { get; set; }

        /// <summary>
        /// 渠道返回错误信息
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// 上游渠道返回数据包 (无需JSON序列化)
        /// </summary>
        [JsonIgnore]
        public ChannelRetMsg channelRetMsg { get; set; }

        /// <summary>
        /// 生成聚合支付参数 (仅统一下单接口使用)
        /// </summary>
        /// <returns></returns>
        public string BuildPayDataType()
        {
            return "none";
        }

        /// <summary>
        /// 生成支付参数
        /// </summary>
        /// <returns></returns>
        public string BuildPayData()
        {
            return "";
        }
    }
}

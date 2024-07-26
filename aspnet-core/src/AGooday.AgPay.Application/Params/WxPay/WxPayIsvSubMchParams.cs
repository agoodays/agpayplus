﻿namespace AGooday.AgPay.Application.Params.WxPay
{
    /// <summary>
    /// 微信官方支付 配置参数
    /// </summary>
    public class WxPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 子商户ID
        /// </summary>
        public string SubMchId { get; set; }

        /// <summary>
        /// 子账户AppId
        /// </summary>
        public string SubMchAppId { get; set; }
    }
}

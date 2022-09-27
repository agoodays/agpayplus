using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.WxPay
{
    /// <summary>
    /// 微信官方支付 配置参数
    /// </summary>
    public class WxPayIsvSubMchParams : IsvSubMchParams
    {
        /** 子商户ID **/
        public string subMchId { get; set; }

        /** 子账户appID **/
        public string subMchAppId { get; set; }
    }
}

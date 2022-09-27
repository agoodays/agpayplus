using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝 特约商户参数定义
    /// </summary>
    public class AliPayIsvSubMchParams : IsvSubMchParams
    {
        public string appAuthToken { get; set; }
    }
}

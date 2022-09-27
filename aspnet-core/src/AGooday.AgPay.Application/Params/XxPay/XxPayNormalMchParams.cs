using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.XxPay
{
    /// <summary>
    /// 小新支付 普通商户参数定义
    /// </summary>
    public class XxPayNormalMchParams : NormalMchParams
    {
        /** 商户号 */
        public string mchId { get; set; }

        /** 私钥 */
        public string key { get; set; }

        /** 支付网关地址 */
        public string payUrl { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                key = StringUtil.Str2Star(key, 4, 4, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

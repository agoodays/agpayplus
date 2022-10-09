using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.PpPay
{
    public class PpPayNormalMchParams : NormalMchParams
    {
        /**
         * 是否沙箱环境
         */
        public byte Sandbox { get; set; }

        /**
         * clientId
         * 客户端 ID
         */
        public string ClientId { get; set; }

        /**
         * secret
         * 密钥
         */
        public string Secret { get; set; }

        /**
         * 支付 Webhook 通知 ID
         */
        public string NotifyWebhook { get; set; }

        /**
         * 退款 Webhook 通知 ID
         */
        public string RefundWebhook { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(Secret))
            {
                Secret = StringUtil.Str2Star(Secret, 6, 6, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

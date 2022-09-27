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
        public byte sandbox { get; set; }

        /**
         * clientId
         * 客户端 ID
         */
        public string clientId { get; set; }

        /**
         * secret
         * 密钥
         */
        public string secret { get; set; }

        /**
         * 支付 Webhook 通知 ID
         */
        public string notifyWebhook { get; set; }

        /**
         * 退款 Webhook 通知 ID
         */
        public string refundWebhook { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(secret))
            {
                secret = StringUtil.Str2Star(secret, 6, 6, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

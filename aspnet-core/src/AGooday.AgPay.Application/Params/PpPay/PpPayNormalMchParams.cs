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
        private byte sandbox { get; set; }

        /**
         * clientId
         * 客户端 ID
         */
        private string clientId { get; set; }

        /**
         * secret
         * 密钥
         */
        private string secret { get; set; }

        /**
         * 支付 Webhook 通知 ID
         */
        private string notifyWebhook { get; set; }

        /**
         * 退款 Webhook 通知 ID
         */
        private string refundWebhook { get; set; }

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

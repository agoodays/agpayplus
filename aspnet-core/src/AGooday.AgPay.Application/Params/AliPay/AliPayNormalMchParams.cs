using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝 普通商户参数定义
    /// </summary>
    public class AliPayNormalMchParams : NormalMchParams
    {
        /** 是否沙箱环境 */
        public byte sandbox { get; set; }

        /** appId */
        public string appId { get; set; }

        /** privateKey */
        public string privateKey { get; set; }

        /** alipayPublicKey */
        public string alipayPublicKey { get; set; }

        /** 签名方式 **/
        public string signType { get; set; }

        /** 是否使用证书方式 **/
        public byte useCert { get; set; }

        /** app 证书 **/
        public string appPublicCert { get; set; }

        /** 支付宝公钥证书（.crt格式） **/
        public string alipayPublicCert { get; set; }

        /** 支付宝根证书 **/
        public string alipayRootCert { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(privateKey))
            {
                privateKey = StringUtil.Str2Star(privateKey, 4, 4, 6);
            }
            if (!string.IsNullOrWhiteSpace(alipayPublicKey))
            {
                alipayPublicKey = StringUtil.Str2Star(alipayPublicKey, 6, 6, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

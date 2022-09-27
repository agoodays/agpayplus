using AGooday.AgPay.Common.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.YsfPay
{
    /// <summary>
    /// 云闪付 配置信息
    /// </summary>
    public class YsfPayIsvParams : IsvParams
    {
        /** 是否沙箱环境 */
        public byte sandbox;

        /** serProvId **/
        public string serProvId;

        /** isvPrivateCertFile 证书 **/
        public string isvPrivateCertFile;

        /** isvPrivateCertPwd **/
        public string isvPrivateCertPwd;

        /** ysfpayPublicKey **/
        public string ysfpayPublicKey;

        /** acqOrgCodeList 支付机构号 **/
        public string acqOrgCode;

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(isvPrivateCertPwd))
            {
                isvPrivateCertPwd = StringUtil.Str2Star(isvPrivateCertPwd, 0, 3, 6);
            }
            if (!string.IsNullOrWhiteSpace(ysfpayPublicKey))
            {
                ysfpayPublicKey = StringUtil.Str2Star(ysfpayPublicKey, 6, 6, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

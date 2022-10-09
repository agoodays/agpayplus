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
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte Sandbox { get; set; }

        /// <summary>
        /// serProvId
        /// </summary>
        public string SerProvId { get; set; }

        /// <summary>
        /// isvPrivateCertFile 证书
        /// </summary>
        public string IsvPrivateCertFile { get; set; }

        /// <summary>
        /// isvPrivateCertPwd
        /// </summary>
        public string IsvPrivateCertPwd { get; set; }

        /// <summary>
        /// ysfpayPublicKey
        /// </summary>
        public string YsfpayPublicKey { get; set; }

        /// <summary>
        /// acqOrgCodeList 支付机构号
        /// </summary>
        public string AcqOrgCode;

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(IsvPrivateCertPwd))
            {
                IsvPrivateCertPwd = StringUtil.Str2Star(IsvPrivateCertPwd, 0, 3, 6);
            }
            if (!string.IsNullOrWhiteSpace(YsfpayPublicKey))
            {
                YsfpayPublicKey = StringUtil.Str2Star(YsfpayPublicKey, 6, 6, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

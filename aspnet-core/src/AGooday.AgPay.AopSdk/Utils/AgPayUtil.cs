using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Utils
{
    public class AgPayUtil
    {
        /// <summary>
        /// 计算签名摘要
        /// </summary>
        /// <param name="param"></param>
        /// <param name="key">商户秘钥</param>
        /// <returns></returns>
        public static string GetSign(Dictionary<string, object> @params, string key)
        {
            return string.Empty;
        }
    }
}

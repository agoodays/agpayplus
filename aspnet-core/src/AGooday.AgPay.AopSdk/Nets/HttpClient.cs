using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// Http请求客户端
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public APIAgPayResponse Request(APIAgPayRequest request)
        {
            int responseCode = 0;
            string responseBody = string.Empty;

            return new APIAgPayResponse(responseCode, responseBody, null);
        }
    }
}

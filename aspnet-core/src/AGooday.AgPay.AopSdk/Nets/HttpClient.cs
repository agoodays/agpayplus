using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// Http请求客户端
    /// </summary>
    public abstract class HttpClient
    {
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract void Request(APIAgPayRequest request);
    }
}

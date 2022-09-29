using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// API请求
    /// </summary>
    public class APIAgPayRequest
    {
        /// <summary>
        /// 请求方法 (GET, POST, DELETE or PUT)
        /// </summary>
        private APIResource.RequestMethod Method { get; set; }
        /// <summary>
        /// 请求URL
        /// </summary>
        private string url { get; set; }
    }
}

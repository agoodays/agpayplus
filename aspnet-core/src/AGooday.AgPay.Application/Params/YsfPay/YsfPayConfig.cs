using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.YsfPay
{
    /// <summary>
    /// 云闪付 通用配置信息
    /// </summary>
    public class YsfPayConfig
    {
        /// <summary>
        /// 网关地址
        /// </summary>
        public static string PROD_SERVER_URL = "https://partner.95516.com";
        public static string SANDBOX_SERVER_URL = "http://ysf.bcbip.cn:10240";
    }
}

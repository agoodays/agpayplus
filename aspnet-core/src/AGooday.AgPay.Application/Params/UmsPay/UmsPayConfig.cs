using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.UmsPay
{
    /// <summary>
    /// 银联商务 通用配置信息
    /// </summary>
    public class UmsPayConfig
    {
        /// <summary>
        /// 网关地址
        /// </summary>
        public const string PROD_SERVER_URL = "https://api-mop.chinaums.com";
        public const string SANDBOX_SERVER_URL = "https://test-api-open.chinaums.com";
    }
}

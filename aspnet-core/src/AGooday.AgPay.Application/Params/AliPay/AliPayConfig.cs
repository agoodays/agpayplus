using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝， 通用配置信息
    /// </summary>
    public class AliPayConfig
    {
        public const string SIGN_TYPE_RSA = "RSA";
        public const string SIGN_TYPE_RSA2 = "RSA2";


        /** 网关地址 */
        public static string PROD_SERVER_URL = "https://openapi.alipay.com/gateway.do";
        public static string SANDBOX_SERVER_URL = "https://openapi.alipaydev.com/gateway.do";

        public static string PROD_OAUTH_URL = "https://openauth.alipay.com/oauth2/publicAppAuthorize.htm?app_id=%s&scope=auth_base&state=&redirect_uri=%s";
        public static string SANDBOX_OAUTH_URL = "https://openauth.alipaydev.com/oauth2/publicAppAuthorize.htm?app_id=%s&scope=auth_base&state=&redirect_uri=%s";

        /** isv获取授权商户URL地址 **/
        public static string PROD_APP_TO_APP_AUTH_URL = "https://openauth.alipay.com/oauth2/appToAppAuth.htm?app_id=%s&redirect_uri=%s&state=%s";
        public static string SANDBOX_APP_TO_APP_AUTH_URL = "https://openauth.alipaydev.com/oauth2/appToAppAuth.htm?app_id=%s&redirect_uri=%s&state=%s";


        public static string FORMAT = "json";

        public static string CHARSET = "UTF-8";
    }
}

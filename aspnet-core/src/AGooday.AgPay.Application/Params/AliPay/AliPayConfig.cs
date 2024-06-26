﻿namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝， 通用配置信息
    /// </summary>
    public class AliPayConfig
    {
        public const string SIGN_TYPE_RSA = "RSA";
        public const string SIGN_TYPE_RSA2 = "RSA2";


        /** 网关地址 */
        public const string PROD_SERVER_URL = "https://openapi.alipay.com/gateway.do";
        public const string SANDBOX_SERVER_URL = "https://openapi-sandbox.dl.alipaydev.com/gateway.do";

        public const string PROD_OAUTH_URL = "https://openauth.alipay.com/oauth2/publicAppAuthorize.htm?app_id={0}&scope=auth_base&state=&redirect_uri={1}";
        public const string SANDBOX_OAUTH_URL = "https://openauth-sandbox.dl.alipaydev.com/oauth2/publicAppAuthorize.htm?app_id={0}&scope=auth_base&state=&redirect_uri={1}";

        /** isv获取授权商户URL地址 **/
        public const string PROD_APP_TO_APP_AUTH_URL = "https://openauth.alipay.com/oauth2/appToAppAuth.htm?app_id={0}&redirect_uri={1}&state={2}";
        public const string SANDBOX_APP_TO_APP_AUTH_URL = "https://openauth-sandbox.dl.alipaydev.com/oauth2/appToAppAuth.htm?app_id={0}&redirect_uri={1}&state={2}";


        public const string FORMAT = "json";

        public const string CHARSET = "UTF-8";
    }
}

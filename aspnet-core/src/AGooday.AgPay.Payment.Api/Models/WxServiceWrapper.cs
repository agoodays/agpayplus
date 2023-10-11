using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Utils;
using SKIT.FlurlHttpClient;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Models
{
    using WechatTenpayClientOptionsV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClientOptions;
    using WechatTenpayClientOptionsV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClientOptions;
    using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
    using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

    /// <summary>
    /// wxService 包装类
    /// </summary>
    public class WxServiceWrapper
    {
        public WxPayConfig Config { get; private set; }
        public CommonClientBase Client { get; private set; }
        //public WechatTenpayClientV2 ClientV2 { get; private set; }
        //public WechatTenpayClientV3 ClientV3 { get; private set; }
        public WxServiceWrapper(CommonClientBase client, WxPayConfig config)
        {
            Client = client;
            Config = config;
        }
        //public WxServiceWrapper(string apiVersion, WechatTenpayClientV2 clientv2, WechatTenpayClientV3 clientv3)
        //{
        //    ApiVersion = apiVersion;
        //    ClientV2 = clientv2;
        //    ClientV3 = clientv3;
        //}

        public static WxServiceWrapper BuildWxServiceWrapper(string mchId, string appId, string appSecret, string mchKey, string apiVersion, string apiV3Key,
            string serialNo, string cert, string apiClientKey)
        {
            CommonClientBase client;
            var config = new WxPayConfig()
            {
                ApiVersion = apiVersion,
                MchId = mchId,
                AppId = appId,
                AppSecret = appSecret,
                MchKey = mchKey,
                ApiV3Key = apiV3Key,
            };
            // 微信API  V2
            if (CS.PAY_IF_VERSION.WX_V2.Equals(apiVersion))
            {
                var certFilePath = ChannelCertConfigKit.GetCertFilePath(cert);
                var fileStream = new FileStream(certFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] merchantCertificateBytes = new byte[fileStream.Length];
                fileStream.Read(merchantCertificateBytes, 0, merchantCertificateBytes.Length);
                fileStream.Close();
                var optionsv2 = new WechatTenpayClientOptionsV2()
                {
                    MerchantId = mchId,// 微信商户号
                    MerchantSecret = mchKey,// 微信商户 API 密钥
                    MerchantCertificateBytes = merchantCertificateBytes,// 微信商户证书内容，即 `apiclient_cert.p12` 文件内容的 Base64 编码结果
                    MerchantCertificatePassword = mchId// 微信商户证书密码，通常是商户号
                };
                //var clientv2 = new WechatTenpayClientV2(optionsv2);
                client = new WechatTenpayClientV2(optionsv2);
            }
            // 微信API  V3
            else if (CS.PAY_IF_VERSION.WX_V3.Equals(apiVersion))
            {
                var manager = new InMemoryCertificateManager();

                var certFilePath = ChannelCertConfigKit.GetCertFilePath(apiClientKey);
                string merchantCertificatePrivateKey = File.ReadAllText(certFilePath, Encoding.UTF8);
                var optionsv3 = new WechatTenpayClientOptionsV3()
                {
                    MerchantId = mchId,// 微信商户号
                    MerchantV3Secret = apiV3Key,// 微信商户 v3 API 密钥
                    MerchantCertificateSerialNumber = serialNo,// 微信商户证书序列号
                    MerchantCertificatePrivateKey = merchantCertificatePrivateKey,// -----BEGIN PRIVATE KEY-----微信商户证书私钥，即 `apiclient_key.pem` 文件内容-----END PRIVATE KEY-----
                    PlatformCertificateManager = manager // 证书管理器的具体用法请参阅下文的高级技巧与加密、验签有关的章节
                };
                config.ApiClientKey = apiClientKey;
                config.MchPrivateKey = merchantCertificatePrivateKey;
                //var clientv3 = new WechatTenpayClientV3(optionsv3);
                client = new WechatTenpayClientV3(optionsv3);
            }
            else
            {
                throw new BizException("不支持的微信支付API版本");
            }
            //return new WxServiceWrapper(apiVersion, clientv2, clientv3);
            return new WxServiceWrapper(client, config);
        }

        public static WxServiceWrapper BuildWxServiceWrapper(WxPayIsvParams wxpayParams)
        {
            //放置 wxPayService
            return BuildWxServiceWrapper(wxpayParams.MchId, wxpayParams.AppId,
                    wxpayParams.AppSecret, wxpayParams.Key, wxpayParams.ApiVersion, wxpayParams.ApiV3Key,
                    wxpayParams.SerialNo, wxpayParams.Cert, wxpayParams.ApiClientKey);
        }

        public static WxServiceWrapper BuildWxServiceWrapper(WxPayNormalMchParams wxpayParams)
        {
            //放置 wxPayService
            return BuildWxServiceWrapper(wxpayParams.MchId, wxpayParams.AppId,
                    wxpayParams.AppSecret, wxpayParams.Key, wxpayParams.ApiVersion, wxpayParams.ApiV3Key,
                    wxpayParams.SerialNo, wxpayParams.Cert, wxpayParams.ApiClientKey);
        }

        public class WxPayConfig
        {
            /// <summary>
            /// 缓存微信API版本
            /// </summary>
            public string ApiVersion { get; set; }
            /// <summary>
            /// 微信ApiId
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 微信商户号
            /// </summary>
            public string MchId { get; set; }
            /// <summary>
            /// API密钥
            /// </summary>
            public string MchKey { get; set; }
            /// <summary>
            /// 应用AppSecret
            /// </summary>
            public string AppSecret { get; set; }
            /// <summary>
            /// 私钥文件(.pem格式)
            /// </summary>
            public string ApiClientKey { get; set; }
            /// <summary>
            /// 私钥文件(.pem格式)内容 商户私钥
            /// </summary>
            public string MchPrivateKey { get; set; }
            /// <summary>
            /// API V3秘钥
            /// </summary>
            public string ApiV3Key { get; set; }
        }
    }
}

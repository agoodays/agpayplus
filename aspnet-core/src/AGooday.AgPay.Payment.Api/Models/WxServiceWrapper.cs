using AGooday.AgPay.Application.Params.WxPay;
using SKIT.FlurlHttpClient;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Utils;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Models
{
    using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
    using WechatTenpayClientOptionsV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClientOptions;
    using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;
    using WechatTenpayClientOptionsV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClientOptions;
    public class WxServiceWrapper
    {
        /// <summary>
        /// 缓存微信API版本
        /// </summary>
        public string ApiVersion { get; private set; }
        public string AppId { get; private set; }
        public string MchKey { get; private set; }
        public CommonClientBase Client { get; private set; }
        //public WechatTenpayClientV2 ClientV2 { get; private set; }
        //public WechatTenpayClientV3 ClientV3 { get; private set; }
        public WxServiceWrapper(string apiVersion, string appId, string mchKey, CommonClientBase client)
        {
            ApiVersion = apiVersion;
            AppId = appId;
            MchKey = mchKey;
            Client = client;
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
                //var clientv3 = new WechatTenpayClientV3(optionsv3);
                client = new WechatTenpayClientV3(optionsv3);
            }
            else
            {
                throw new BizException("不支持的微信支付API版本");
            }
            //return new WxServiceWrapper(apiVersion, clientv2, clientv3);
            return new WxServiceWrapper(apiVersion, appId, mchKey, client);
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
    }
}

using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Exceptions;
using AGooday.AgPay.Components.Third.Utils;
using Aop.Api;

namespace AGooday.AgPay.Components.Third.Models
{
    /// <summary>
    /// 支付宝Client 包装类
    /// </summary>
    public class AliPayClientWrapper
    {
        /// <summary>
        /// 默认为 不使用证书方式
        /// </summary>
        public byte? UseCert { get; private set; } = CS.NO;

        /// <summary>
        /// 缓存支付宝client 对象
        /// </summary>
        public IAopClient AlipayClient { get; private set; }

        /// <summary>
        /// 封装支付宝接口调用函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public T Execute<T>(IAopRequest<T> request) where T : AopResponse
        {
            try
            {
                T alipayResp = null;
                //证书加密方式
                if (UseCert == CS.YES)
                {
                    alipayResp = AlipayClient.CertificateExecute(request);
                }
                //key 或者 空都为默认普通加密方式
                else
                {
                    alipayResp = AlipayClient.Execute(request);
                }

                return alipayResp;
            }
            // 调起接口前出现异常，如私钥问题。  调起后出现验签异常等。
            catch (AopException e)
            {
                LogUtil<AgHttpClient>.Error("调起支付宝Execute[AopException]异常！", e);
                //如果数据返回出现验签异常，则需要抛出： UNKNOWN 异常。
                throw ChannelException.SysError(e.Message);
            }
            catch (Exception e)
            {
                LogUtil<AgHttpClient>.Error("调起支付宝Execute[Exception]异常！", e);
                throw ChannelException.SysError($"调用支付宝client服务异常：{e.Message}");
            }
        }
        public AliPayClientWrapper(byte? useCert, IAopClient alipayClient)
        {
            this.UseCert = useCert;
            this.AlipayClient = alipayClient;
        }

        /// <summary>
        /// 构建支付宝client 包装类
        /// </summary>
        /// <param name="useCert">是否使用证书方式</param>
        /// <param name="sandbox">是否沙箱环境</param>
        /// <param name="appId">appId</param>
        /// <param name="privateKey">privateKey</param>
        /// <param name="alipayPublicKey">alipayPublicKey</param>
        /// <param name="signType">签名方式</param>
        /// <param name="appCert">商户应用证书路径</param>
        /// <param name="alipayPublicCert">支付宝公钥证书路径（.crt格式）</param>
        /// <param name="alipayRootCert">支付宝根证书路径</param>
        /// <returns></returns>
        public static AliPayClientWrapper BuildAlipayClientWrapper(byte? useCert, byte? sandbox, string appId, string privateKey,
            string alipayPublicKey, string signType, string appCert,
            string alipayPublicCert, string alipayRootCert)
        {
            //避免空值
            sandbox ??= CS.NO;

            IAopClient alipayClient;
            //证书的方式
            if (useCert == CS.YES)
            {
                //设置证书相关参数
                CertParams certParams = new CertParams
                {
                    AlipayPublicCertPath = ChannelCertConfigKit.GetCertFilePath(alipayPublicCert),
                    AppCertPath = ChannelCertConfigKit.GetCertFilePath(appCert),
                    RootCertPath = ChannelCertConfigKit.GetCertFilePath(alipayRootCert)
                };
                alipayClient = new DefaultAopClient(sandbox == CS.YES ? AliPayConfig.SANDBOX_SERVER_URL : AliPayConfig.PROD_SERVER_URL,
                    appId, privateKey, AliPayConfig.FORMAT, "1.0", signType, AliPayConfig.CHARSET, false, certParams);

                //AlipayConfig alipayConfig = new AlipayConfig();
                //alipayConfig.ServerUrl = sandbox == CS.YES ? AliPayConfig.SANDBOX_SERVER_URL : AliPayConfig.PROD_SERVER_URL;
                //alipayConfig.AppId = appId;
                //alipayConfig.PrivateKey = privateKey;
                //alipayConfig.Format = AliPayConfig.FORMAT;
                //alipayConfig.Charset = AliPayConfig.CHARSET;
                //alipayConfig.SignType = signType;
                //alipayConfig.AlipayPublicCertPath = alipayPublicCert;
                //alipayConfig.AppCertPath = appCert;
                //alipayConfig.RootCertPath = alipayRootCert;
                //alipayClient = new DefaultAopClient(alipayConfig);
            }
            else
            {
                // RSA2 加密算法默认生成格式为 PKCS8（Java 适用），如需 PKCS1 格式（非 Java 适用），可使用格式转换。C# 需使用 PKCS1 格式。
                alipayClient = new DefaultAopClient(sandbox == CS.YES ? AliPayConfig.SANDBOX_SERVER_URL : AliPayConfig.PROD_SERVER_URL,
                    appId, privateKey, AliPayConfig.FORMAT, "1.0", signType, alipayPublicKey, AliPayConfig.CHARSET,
                        false);
            }
            return new AliPayClientWrapper(useCert, alipayClient);
        }

        public static AliPayClientWrapper BuildAlipayClientWrapper(AliPayIsvParams alipayParams)
        {
            return BuildAlipayClientWrapper(alipayParams.UseCert, alipayParams.Sandbox, alipayParams.AppId, alipayParams.PrivateKey,
                alipayParams.AlipayPublicKey, alipayParams.SignType, alipayParams.AppPublicCert,
                alipayParams.AlipayPublicCert, alipayParams.AlipayRootCert);
        }

        public static AliPayClientWrapper BuildAlipayClientWrapper(AliPayNormalMchParams alipayParams)
        {
            return BuildAlipayClientWrapper(alipayParams.UseCert, alipayParams.Sandbox, alipayParams.AppId, alipayParams.PrivateKey,
                alipayParams.AlipayPublicKey, alipayParams.SignType, alipayParams.AppPublicCert,
                alipayParams.AlipayPublicCert, alipayParams.AlipayRootCert);
        }

        public static AliPayClientWrapper BuildAlipayClientWrapper(AliPayIsvOauth2Params alipayOauth2Params)
        {
            return BuildAlipayClientWrapper(alipayOauth2Params.UseCert, alipayOauth2Params.Sandbox, alipayOauth2Params.AppId, alipayOauth2Params.PrivateKey,
                alipayOauth2Params.AlipayPublicKey, alipayOauth2Params.SignType, alipayOauth2Params.AppPublicCert,
                alipayOauth2Params.AlipayPublicCert, alipayOauth2Params.AlipayRootCert);
        }

        public static AliPayClientWrapper BuildAlipayClientWrapper(AliPayNormalMchOauth2Params alipayOauth2Params)
        {
            return BuildAlipayClientWrapper(alipayOauth2Params.UseCert, alipayOauth2Params.Sandbox, alipayOauth2Params.AppId, alipayOauth2Params.PrivateKey,
                alipayOauth2Params.AlipayPublicKey, alipayOauth2Params.SignType, alipayOauth2Params.AppPublicCert,
                alipayOauth2Params.AlipayPublicCert, alipayOauth2Params.AlipayRootCert);
        }

        public static AliPayClientWrapper BuildAlipayClientWrapper(AliLiteParams alipayOauth2Params)
        {
            return BuildAlipayClientWrapper(alipayOauth2Params.UseCert, alipayOauth2Params.Sandbox, alipayOauth2Params.AppId, alipayOauth2Params.PrivateKey,
                alipayOauth2Params.AlipayPublicKey, alipayOauth2Params.SignType, alipayOauth2Params.AppPublicCert,
                alipayOauth2Params.AlipayPublicCert, alipayOauth2Params.AlipayRootCert);
        }
    }
}

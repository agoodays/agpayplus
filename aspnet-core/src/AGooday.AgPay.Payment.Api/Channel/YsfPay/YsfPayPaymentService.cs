using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.YsfPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.YsfPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay
{
    public class YsfPayPaymentService : AbstractPaymentService
    {
        private static ILog logger = LogManager.GetLogger(typeof(YsfPayPaymentService));

        public YsfPayPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPaywayService(this, payOrder.WayCode).Pay(bizRQ, payOrder, mchAppConfigContext);
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPaywayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }

        /// <summary>
        /// 封装参数 & 统一请求
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="reqParams"></param>
        /// <param name="logPrefix"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public JObject PackageParamAndReq(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext) {

            YsfPayIsvParams isvParams = (YsfPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());
            
            if (isvParams.SerProvId == null)
            {
                LogUtil<YsfPayPaymentService>.Error($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                throw new BizException("服务商配置为空。");
            }

            reqParams.Add("serProvId", isvParams.SerProvId); //云闪付服务商标识
            YsfPayIsvSubMchParams isvsubMchParams = (YsfPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqParams.Add("merId", isvsubMchParams.MerId); // 商户号

            //签名
            string isvPrivateCertFile = ChannelCertConfigKit.GetCertFilePath(isvParams.IsvPrivateCertFile);
            string isvPrivateCertPwd = isvParams.IsvPrivateCertPwd;
            reqParams.Add("signature", YsfSignUtil.SignBy256(reqParams, isvPrivateCertFile, isvPrivateCertPwd)); //RSA 签名串

            // 调起上游接口
            logger.Info($"{logPrefix} reqJSON={reqParams}");
            string resText = YsfHttpUtil.DoPostJson(GetYsfpayHost4env(isvParams) + apiUri, null, reqParams);
            logger.Info($"{logPrefix} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }
            return JObject.Parse(resText);
        }

        /// <summary>
        /// 获取云闪付正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetYsfpayHost4env(YsfPayIsvParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? YsfPayConfig.SANDBOX_SERVER_URL : YsfPayConfig.PROD_SERVER_URL;
        }

        /// <summary>
        /// 云闪付 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public static void JsapiParamsSet(JObject reqParams, PayOrderDto payOrder, String notifyUrl, String returnUrl)
        {
            string orderType = YsfHttpUtil.GetOrderTypeByJSapi(payOrder.WayCode);
            reqParams.Add("orderType", orderType); //订单类型： alipayJs-支付宝， wechatJs-微信支付， upJs-银联二维码
            YsfPublicParams(reqParams, payOrder);
            reqParams.Add("backUrl", notifyUrl); //交易通知地址
            reqParams.Add("frontUrl", returnUrl); //前台通知地址
        }

        /// <summary>
        /// 云闪付 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder)
        {
            string orderType = YsfHttpUtil.GetOrderTypeByBar(payOrder.WayCode);
            reqParams.Add("orderType", orderType); //订单类型： alipay-支付宝， wechat-微信支付， -unionpay银联二维码
            YsfPublicParams(reqParams, payOrder);
            // TODO 终端编号暂时写死
            reqParams.Add("termId", "01727367"); // 终端编号
        }

        /// <summary>
        /// 云闪付公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void YsfPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            //获取订单类型
            reqParams.Add("orderNo", payOrder.PayOrderId); //订单号
            reqParams.Add("orderTime", $"{DateTime.Now:yyyyMMddHHmmss}"); //订单时间 如：20180702142900
            reqParams.Add("txnAmt", payOrder.Amount); //交易金额 单位：分，不带小数点
            reqParams.Add("currencyCode", "156"); //交易币种 不出现则默认为人民币-156
            reqParams.Add("orderInfo", payOrder.Subject); //订单信息 订单描述信息，如：京东生鲜食品
        }
    }
}

using System.Diagnostics;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.YsfPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Channel.YsfPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.YsfPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.YsfPay
{
    /// <summary>
    /// 云闪付下单
    /// </summary>
    public class YsfPayPaymentService : AbstractPaymentService
    {
        public YsfPayPaymentService(ILogger<YsfPayPaymentService> logger, IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public YsfPayPaymentService()
            : base()
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

        public override Task<AbstractRS> PayAsync(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PayAsync(bizRQ, payOrder, mchAppConfigContext);
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PreCheckAsync(bizRQ, payOrder, mchAppConfigContext);
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
        public async Task<JObject> PackageParamAndReqAsync(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            YsfPayIsvParams isvParams = (YsfPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            if (isvParams.SerProvId == null)
            {
                _logger.LogError("服务商配置为空: isvParams={isvParams}", JsonConvert.SerializeObject(isvParams));
                //_logger.LogError($"服务商配置为空: isvParams={JsonConvert.SerializeObject(isvParams)}");
                throw new BizException("服务商配置为空。");
            }

            reqParams.Add("serProvId", isvParams.SerProvId); //云闪付服务商标识
            YsfPayIsvSubMchParams isvsubMchParams = (YsfPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqParams.Add("merId", isvsubMchParams.MerId); // 商户号

            //签名
            string isvPrivateCertFile = ChannelCertConfigKit.GetCertFilePath(isvParams.IsvPrivateCertFile);
            string isvPrivateCertPwd = isvParams.IsvPrivateCertPwd;
            reqParams.Add("signature", YsfPaySignUtil.SignBy256(reqParams, isvPrivateCertFile, isvPrivateCertPwd)); //RSA 签名串

            // 调起上游接口
            string url = GetHost4env(isvParams) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            var stopwatch = new Stopwatch();
            var reqJsonData = JsonConvert.SerializeObject(reqParams);
            _logger.LogInformation("{logPrefix} unionId={unionId} url={url} reqData={reqData}", logPrefix, unionId, url, reqJsonData);
            //_logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqData={reqJsonData}");
            stopwatch.Restart();
            string resText = await YsfPayHttpUtil.DoPostJsonAsync(url, reqParams);
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("{logPrefix} unionId={unionId} url={url} reqData={reqData} resData={resData} time={time}", logPrefix, unionId, url, reqJsonData, resText, time);
            //_logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqData={reqJsonData} resData={resText} time={time}");

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
        public static string GetHost4env(YsfPayIsvParams isvParams)
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
        public static void JsapiParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            string orderType = YsfPayEnum.GetOrderTypeByJSapi(payOrder.WayCode);
            reqParams.Add("orderType", orderType); //订单类型： alipayJs-支付宝， wechatJs-微信支付， upJs-银联二维码
            PublicParams(reqParams, payOrder);
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
            string orderType = YsfPayEnum.GetOrderTypeByBar(payOrder.WayCode);
            reqParams.Add("orderType", orderType); //订单类型： alipay-支付宝， wechat-微信支付， -unionpay银联二维码
            PublicParams(reqParams, payOrder);
            // TODO 终端编号暂时写死
            reqParams.Add("termId", "01727367"); // 终端编号
        }

        /// <summary>
        /// 云闪付公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void PublicParams(JObject reqParams, PayOrderDto payOrder)
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

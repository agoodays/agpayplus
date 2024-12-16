using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.UmsPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.UmsPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay
{
    /// <summary>
    /// 银联商务下单
    /// </summary>
    public class UmsPayPaymentService : AbstractPaymentService
    {
        public UmsPayPaymentService(ILogger<UmsPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public UmsPayPaymentService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override Task<AbstractRS> PayAsync(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PayAsync(bizRQ, payOrder, mchAppConfigContext);
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }

        /// <summary>
        /// 获取正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetHost4env(UmsPayIsvParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? UmsPayConfig.SANDBOX_SERVER_URL : UmsPayConfig.PROD_SERVER_URL;
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
        public async Task<JObject> PackageParamAndReqAsync(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext, bool isBarPay = false)
        {
            UmsPayIsvParams isvParams = (UmsPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            if (string.IsNullOrWhiteSpace(isvParams?.AppId) || string.IsNullOrWhiteSpace(isvParams?.AppKey))
            {
                _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                throw new BizException("服务商配置为空。");
            }

            UmsPayIsvSubMchParams isvsubMchParams = (UmsPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

            reqParams.Add(isBarPay ? "merchantCode" : "mid", isvsubMchParams.Mid); //商户号
            reqParams.Add(isBarPay ? "terminalCode" : "tid", isvsubMchParams.Tid); //终端号

            // 调起上游接口
            string url = GetHost4env(isvParams) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqJSON={JsonConvert.SerializeObject(reqParams)}");
            string resText = await UmsPayHttpUtil.DoPostJsonAsync(url, isvParams.AppId, isvParams.AppKey, reqParams);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            var resParams = JObject.Parse(resText);
            return resParams;
        }

        /// <summary>
        /// 银联商务 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public static void UnifiedParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            reqParams.Add("merOrderId", payOrder.PayOrderId);
            reqParams.Add("instMid", "YUEDANDEFAULT");
            reqParams.Add("tradeType", "JSAPI");
            reqParams.Add("originalAmount", payOrder.Amount);
            reqParams.Add("totalAmount", payOrder.Amount);
            reqParams.Add("orderDesc", payOrder.Subject);
            reqParams.Add("notifyUrl", notifyUrl);
            reqParams.Add("returnUrl", returnUrl);
            reqParams.Add("clientIp", payOrder.ClientIp);
        }

        /// <summary>
        /// 银联商务 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            reqParams.Add("transactionAmount", payOrder.Amount);
            reqParams.Add("transactionCurrencyCode", "156");
            reqParams.Add("merchantOrderId", payOrder.PayOrderId);
            reqParams.Add("merchantRemark", payOrder.Subject);
            /* 支付方式
             * E_CASH – 电子现金
             * SOUNDWAVE – 声波
             * NFC – NFC
             * CODE_SCAN – 扫码
             * MANUAL – 手输
             * FACE_SCAN – 扫脸
             */
            reqParams.Add("payMode", "CODE_SCAN");
            reqParams.Add("deviceType", "11");

            /* 经度 长度⇐10。
             * 实体类终端（设备类型非01,11,12,13）：经纬度与基站信息cust2字段必传其一；
             * 非实体类终端（设备类型为01,11,12,13）：经纬度与ip字段必传其一。
             * 格式： 1 位正负号+3 位整数+1 位小数点+5 位小数；
             * 对于正负号：+表示东经， -表示西经。例如-121.48352
             */
            // reqParams.Add("longitude", "");
            /* 纬度 长度⇐10。
             * 实体类终端（设备类型非01,11,12,13）：经纬度与基站信息cust2字段必传其一；
             * 非实体类终端（设备类型为01,11,12,13）：经纬度与ip字段必传其一。
             * 格式： 1 位正负号 + 2 位整数 + 1 位小数点 + 6 位小数；
             * 对于正负号：+表示北纬， -表示南纬。例如 + 31.221345或 - 03.561345
             */
            // reqParams.Add("latitude", "");
            // 基站信息 注：实体类终端（设备类型非01,11,12,13）如无经纬度，该字段必送
            // reqParams.Add("cust2", "");
            // 终端设备IP地址 长度⇐64, 非实体类终端（设备类型为01,11,12,13）如无经纬度，该字段必送；格式如：“ip”:“172.20.11.089”
            reqParams.Add("ip", payOrder.ClientIp);
        }

        public async Task<ChannelRetMsg> BarAsync(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/v6/poslink/transaction/pay", reqParams, logPrefix, mchAppConfigContext, true);
            //请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            resJSON.TryGetString("errInfo", out string errInfo); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "00":
                        resJSON.TryGetString("orderId", out string orderId);// 20230713201936190033303567
                        resJSON.TryGetString("userId", out string userId); // oUpF8uDs4900N84XIoXWcCqzbyyo
                        resJSON.TryGetString("thirdPartyBuyerId", out string thirdPartyBuyerId); // 第三方买家Id oUpF8uDs4900N84XIoXWcCqzbyyo
                        resJSON.TryGetString("thirdPartyOrderId", out string thirdPartyOrderId);// 4200001857202307136819383689
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        channelRetMsg.ChannelOrderId = orderId;
                        channelRetMsg.ChannelUserId = userId;
                        channelRetMsg.PlatformOrderId = thirdPartyOrderId;
                        break;
                    case "0000":
                    case "SUCCESS":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }

            return channelRetMsg;
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.LcswPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LcswPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.LcswPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LcswPay
{
    public class LcswPayPaymentService : AbstractPaymentService
    {
        public LcswPayPaymentService(ILogger<LcswPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public LcswPayPaymentService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LCSWPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).Pay(bizRQ, payOrder, mchAppConfigContext);
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }

        public ChannelRetMsg LcswBar(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/pay/open/barcodepay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
            string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
            resJSON.TryGetString("merchant_no", out string merchantNo); // 商户号
            channelRetMsg.ChannelMchNo = merchantNo;
            try
            {
                if ("01".Equals(returnCode))
                {
                    resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode) || "02".Equals(resultCode) || "03".Equals(resultCode))
                    {
                        resJSON.TryGetString("out_trade_no", out string outTradeNo);// 平台唯一订单号
                        resJSON.TryGetString("channel_trade_no", out string channelTradeNo);// 微信/支付宝流水号
                        resJSON.TryGetString("channel_order_no", out string channelOrderNo);// 银行渠道订单号，微信支付时显示在支付成功页面的条码，可用作扫码查询和扫码退款时匹配
                        resJSON.TryGetString("user_id", out string userId);// 付款方用户id，服务商appid下的“微信openid”、“支付宝账户”
                        switch (resultCode)
                        {
                            case "01":
                                channelRetMsg.ChannelOrderId = outTradeNo;
                                channelRetMsg.ChannelUserId = userId;
                                channelRetMsg.PlatformOrderId = channelTradeNo;
                                channelRetMsg.PlatformMchOrderId = channelOrderNo;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                break;
                            case "02":
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = resultCode;
                                channelRetMsg.ChannelErrMsg = returnMsg;
                                break;
                            case "03":
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                channelRetMsg.IsNeedQuery = true; // 开启轮询查单;
                                break;
                        }
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = resultCode;
                        channelRetMsg.ChannelErrMsg = returnMsg;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = returnCode;
                channelRetMsg.ChannelErrMsg = returnMsg;
            }

            return channelRetMsg;
        }

        /// <summary>
        /// 获取乐刷正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetLcswPayHost4env(LcswPayNormalMchParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? LcswPayConfig.SANDBOX_SERVER_URL : LcswPayConfig.PROD_SERVER_URL;
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
        public JObject PackageParamAndReq(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            LcswPayNormalMchParams lcswParams = (LcswPayNormalMchParams)_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

            reqParams.Add("merchant_no", lcswParams.MerchantNo); // 商户号
            reqParams.Add("terminal_id", lcswParams.TerminalId); // 商户发起交易的IP地址

            // 签名
            string key = lcswParams.AccessToken;
            reqParams.Add("key_sign", LcswSignUtil.Sign(reqParams, key)); // 签名字符串

            // 调起上游接口
            string url = GetLcswPayHost4env(lcswParams) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            var reqJSON = JsonConvert.SerializeObject(reqParams);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqJSON={reqJSON}");
            string resText = LcswHttpUtil.DoPost(url, reqJSON);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!LcswSignUtil.Verify(resParams, key))
            {
                _logger.LogWarning($"{logPrefix} 验签失败！ reqJSON={reqJSON} resJSON={resText}");
            }

            return resParams;
        }

        /// <summary>
        /// 利楚扫呗 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        public static void UnifiedParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            string payType = LcswPayEnum.GetPayType(payOrder.WayCode);
            reqParams.Add("pay_ver", "201");
            reqParams.Add("pay_type", payType);
            reqParams.Add("service_id", "012");
            reqParams.Add("payType", payType);
            reqParams.Add("notify_url", notifyUrl);
            reqParams.Add("terminal_ip", payOrder.ClientIp); //商户发起交易的IP地址
            LcswPublicParams(reqParams, payOrder);
        }

        /// <summary>
        /// 利楚扫呗 小程序下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        public static void MiniPayParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            string payType = LcswPayEnum.GetPayType(payOrder.WayCode);
            reqParams.Add("pay_ver", "202");
            reqParams.Add("pay_type", payType);
            reqParams.Add("service_id", "015");
            reqParams.Add("payType", payType);
            reqParams.Add("notify_url", notifyUrl);
            reqParams.Add("terminal_ip", payOrder.ClientIp); //商户发起交易的IP地址
            LcswPublicParams(reqParams, payOrder);
        }

        /// <summary>
        /// 利楚扫呗 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder)
        {
            string payType = LcswPayEnum.GetPayType(payOrder.WayCode);
            reqParams.Add("pay_ver", "202");
            reqParams.Add("pay_type", payType);
            reqParams.Add("service_id", "010");
            reqParams.Add("terminal_ip", payOrder.ClientIp); //商户发起交易的IP地址
            LcswPublicParams(reqParams, payOrder);
        }

        /// <summary>
        /// 公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void LcswPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            reqParams.Add("terminal_trace", payOrder.PayOrderId); //终端流水号，填写商户系统的支付订单号，不可重复
            reqParams.Add("terminal_time", payOrder.CreatedAt.Value.ToString("yyyyMMddHHmmss"));
            reqParams.Add("total_fee", payOrder.Amount.ToString()); //订单总金额 金额不能为零或负数
            reqParams.Add("order_body", payOrder.Body); //商品描述,不能包含回车换行等特殊字符
            //reqParams.Add("terminal_ip", payOrder.ClientIp); //商户发起交易的IP地址
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.LesPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.LesPay.Enumerator;
using AGooday.AgPay.Payment.Api.Channel.LesPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay
{
    public class LesPayPaymentService : AbstractPaymentService
    {
        public LesPayPaymentService(ILogger<LesPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LESPAY;
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

        public ChannelRetMsg LesBar(SortedDictionary<string, string> reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/cgi-bin/lepos_pay_gateway.cgi", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string resp_code = resJSON.GetValue("resp_code").ToString(); //返回状态码
            resJSON.TryGetString("resp_msg", out string resp_msg); //返回错误信息
            try
            {
                if ("0".Equals(resp_code))
                {
                    string result_code = resJSON.GetValue("result_code").ToString(); //业务结果
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if ("0".Equals(result_code))
                    {
                        string status = resJSON.GetValue("status").ToString();
                        string leshua_order_id = resJSON.GetValue("leshua_order_id").ToString();//乐刷订单号
                        resJSON.TryGetString("sub_merchant_id", out string sub_merchant_id);//渠道商商户号
                        resJSON.TryGetString("out_transaction_id", out string out_transaction_id);//微信、支付宝等订单号
                        resJSON.TryGetString("channel_order_id", out string channel_order_id);//通道订单号
                        resJSON.TryGetString("sub_openid", out string sub_openid);//用户子标识 微信：公众号APPID下用户唯一标识；支付宝：买家的支付宝用户ID
                        var orderStatus = LesPayEnum.ConvertOrderStatus(status);
                        switch (orderStatus)
                        {
                            case LesPayEnum.OrderStatus.PaySuccess:
                                channelRetMsg.ChannelOrderId = leshua_order_id;
                                channelRetMsg.ChannelUserId = sub_openid;
                                channelRetMsg.PlatformOrderId = out_transaction_id;
                                channelRetMsg.PlatformMchOrderId = channel_order_id;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                break;
                            case LesPayEnum.OrderStatus.PayFail:
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                break;
                            case LesPayEnum.OrderStatus.Paying:
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                channelRetMsg.IsNeedQuery = true; // 开启轮询查单;
                                break;
                        }
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = error_code;
                        channelRetMsg.ChannelErrMsg = error_msg;
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
                channelRetMsg.ChannelErrCode = resp_code;
                channelRetMsg.ChannelErrMsg = resp_msg;
            }

            return channelRetMsg;
        }

        /// <summary>
        /// 获取乐刷正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetLesPayHost4env(LesPayIsvParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? LesPayConfig.SANDBOX_SERVER_URL : LesPayConfig.PROD_SERVER_URL;
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
        public JObject PackageParamAndReq(string apiUri, SortedDictionary<string, string> reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            LesPayIsvParams isvParams = (LesPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            if (isvParams.AgentId == null)
            {
                _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                throw new BizException("服务商配置为空。");
            }

            LesPayIsvSubMchParams isvsubMchParams = (LesPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqParams.Add("merchant_id", isvsubMchParams.MerchantId); // 商户号
            reqParams.Add("nonce_str", Guid.NewGuid().ToString("N"));//随机字符串

            // 签名
            string tradeKey = isvParams.TradeKey;
            reqParams.Add("sign", LesSignUtil.Sign(reqParams, tradeKey)); //RSA 签名字符串

            // 调起上游接口
            string url = GetLesPayHost4env(isvParams) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            var reqText = string.Join("&", reqParams.Select(s => $"{s.Key}={s.Value}"));
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqText={reqText}");
            string resText = LesHttpUtil.DoPost(url, reqText);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resText={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }
            string resJson = XmlUtil.ConvertToJson(resText);
            var resParams = JObject.Parse(resJson);

            return resParams;
        }

        /// <summary>
        /// 乐刷 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public void UnifiedParamsSet(SortedDictionary<string, string> reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl, MchAppConfigContext mchAppConfigContext)
        {
            LesPublicParams(reqParams, payOrder, mchAppConfigContext);
            reqParams.Add("service", "get_tdcode");
            string payWay = LesPayEnum.GetPayWay(payOrder.WayCode);
            reqParams.Add("pay_way", payWay);
            string jspayflag = LesPayEnum.GetJspayFlag(payOrder.WayCode);
            reqParams.Add("jspay_flag", jspayflag);
            reqParams.Add("notify_url", notifyUrl); //通知地址 接收乐刷通知（支付结果通知）的URL，需做UrlEncode 处理，需要绝对路径，确保乐刷能正确访问，若不需要回调请忽略
            reqParams.Add("jump_url", returnUrl); //前台跳转地址 简易支付时必填，完成后，乐刷将跳转到该页面，需做UrlEncode 处理
            reqParams.Add("front_url", returnUrl); //前端跳转地址 银联JSAPI支付时选填，支付成功时跳转
            reqParams.Add("front_fail_url", returnUrl); //支付失败前端跳转地址 银联JSAPI支付时选填，支付失败时跳转
        }

        /// <summary>
        /// 乐刷 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public void BarParamsSet(SortedDictionary<string, string> reqParams, PayOrderDto payOrder, string notifyUrl, MchAppConfigContext mchAppConfigContext)
        {
            LesPublicParams(reqParams, payOrder, mchAppConfigContext);
            reqParams.Add("service", "upload_authcode");
            reqParams.Add("notify_url", notifyUrl); //通知地址 接收乐刷通知（支付结果通知）的URL，需做UrlEncode 处理，需要绝对路径，确保乐刷能正确访问，若不需要回调请忽略
        }

        /// <summary>
        /// 乐刷公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public void LesPublicParams(SortedDictionary<string, string> reqParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            //获取订单类型
            reqParams.Add("third_order_id", payOrder.PayOrderId); //商户内部订单号 可以包含字母：确保同一个商户下唯一
            reqParams.Add("amount", payOrder.Amount.ToString()); //订单总金额 金额不能为零或负数
            reqParams.Add("body", payOrder.Body); //商品描述,不能包含回车换行等特殊字符
            reqParams.Add("client_ip", payOrder.ClientIp); //商户发起交易的IP地址

            LesPayIsvSubMchParams isvsubMchParams = (LesPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            if (isvsubMchParams.T0.HasValue)
            {
                reqParams.Add("t0", isvsubMchParams.T0.Value.ToString()); // T0交易标志
            }
            if (isvsubMchParams.LimitPay.HasValue)
            {
                reqParams.Add("limit_pay", isvsubMchParams.LimitPay.Value.ToString()); // 是否禁止信用卡（默认为不禁用）
            }
        }
    }
}

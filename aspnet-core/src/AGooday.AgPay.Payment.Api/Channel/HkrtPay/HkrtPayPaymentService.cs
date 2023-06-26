using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.HkrtPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay.Enumerator;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.HkrtPay
{
    public class HkrtPayPaymentService : AbstractPaymentService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HkrtPayPaymentService));

        public HkrtPayPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.HKRTPAY;
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

        public ChannelRetMsg HkrtBar(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/api/v1/pay/polymeric/passivepay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string return_code = resJSON.GetValue("return_code").ToString(); //返回状态码
            resJSON.TryGetString("return_msg", out string return_msg); //返回错误信息
            try
            {
                if ("10000".Equals(return_code))
                {
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if (!string.IsNullOrWhiteSpace(error_code))
                    {
                        string status = resJSON.GetValue("trade_status").ToString();
                        string type = resJSON.GetValue("trade_type").ToString();
                        string trade_no = resJSON.GetValue("trade_no").ToString();//交易订单号 SaaS平台的交易订单编号
                        string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号
                        var tradeStatus = HkrtPayEnum.ConvertTradeStatus(status);
                        switch (tradeStatus)
                        {
                            case HkrtPayEnum.TradeStatus.Success:
                                channelRetMsg.ChannelOrderId = trade_no;
                                var tradeType = HkrtPayEnum.ConvertTradeType(type);
                                var attach = GetHkrtAttach(resJSON);
                                attach.TryGetString("out_trade_no", out string out_trade_no);
                                channelRetMsg.PlatformMchOrderId = out_trade_no;
                                switch (tradeType)
                                {
                                    case HkrtPayEnum.TradeType.WX:
                                        attach.TryGetString("sub_openid", out string sub_openid);
                                        attach.TryGetString("transaction_id", out string transaction_id);
                                        channelRetMsg.ChannelUserId = sub_openid;
                                        channelRetMsg.PlatformOrderId = transaction_id;
                                        break;
                                    case HkrtPayEnum.TradeType.ALI:
                                        attach.TryGetString("buyer_user_id", out string buyer_user_id);
                                        attach.TryGetString("trade_no", out string ali_trade_no);
                                        channelRetMsg.ChannelUserId = buyer_user_id;
                                        channelRetMsg.PlatformOrderId = ali_trade_no;
                                        break;
                                    case HkrtPayEnum.TradeType.UNIONQR:
                                        break;
                                }
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                break;
                            case HkrtPayEnum.TradeStatus.Failed:
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                break;
                            case HkrtPayEnum.TradeStatus.Paying:
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
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = return_code;
                channelRetMsg.ChannelErrMsg = return_msg;
            }

            return channelRetMsg;
        }

        /// <summary>
        /// 获取海科融通正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetHkrtPayHost4env(HkrtPayIsvParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? HkrtPayConfig.SANDBOX_SERVER_URL : HkrtPayConfig.PROD_SERVER_URL;
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
            HkrtPayIsvParams isvParams = (HkrtPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            if (isvParams.AgentNo == null)
            {
                log.Error($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                throw new BizException("服务商配置为空。");
            }

            HkrtPayIsvSubMchParams isvsubMchParams = (HkrtPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqParams.Add("accessid", isvParams.AccessId);
            reqParams.Add("merch_no", isvsubMchParams.MerchNo); // 商户号

            // 签名
            string accessKey = isvParams.AccessKey;
            reqParams.Add("sign", HkrtSignUtil.Sign(reqParams, accessKey)); //RSA 签名字符串

            // 调起上游接口
            log.Info($"{logPrefix} reqJSON={reqParams}");
            string resText = HkrtHttpUtil.DoPost(GetHkrtPayHost4env(isvParams) + apiUri, reqParams);
            log.Info($"{logPrefix} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }
            string resJson = XmlUtil.ConvertToJson(resText);
            var resParams = JObject.Parse(resJson);

            return resParams;
        }

        public JObject GetHkrtAttach(JObject resParams)
        {
            try
            {
                resParams.TryGetString("attach", out string attach);
                if (string.IsNullOrWhiteSpace(attach))
                {
                    return null;
                }
                var attachjson = attach;
                do
                {
                    attachjson = attachjson.Replace("\\\"", "\"");
                } while (attachjson.Contains("\\\""));
                attachjson = attachjson.Replace("\"[", "[").Replace("]\"", "]")
                    .Replace("\"{", "{").Replace("}\"", "}");

                return JObject.Parse(attachjson);
            }
            catch (Exception)
            {
                log.Info($"海科融通解析支付宝/微信原生参数异常 resParams={resParams}");
                return null;
            }
        }

        /// <summary>
        /// 海科融通 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public static void UnifiedParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            HkrtPublicParams(reqParams, payOrder);
            string tradeType = HkrtPayEnum.GetTradeType(payOrder.WayCode);
            reqParams.Add("trade_type", tradeType); //支付方式
            reqParams.Add("notify_url", notifyUrl); //支付成功后的通知地址
            reqParams.Add("frontUrl", returnUrl); //支付成功跳转地址
            reqParams.Add("frontFailUrl", returnUrl); //支付失败跳转地址
        }

        /// <summary>
        /// 海科融通 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            HkrtPublicParams(reqParams, payOrder);
            reqParams.Add("notify_url", notifyUrl); //通知地址 接收海科融通通知（支付结果通知）的URL，需做UrlEncode 处理，需要绝对路径，确保海科融通能正确访问，若不需要回调请忽略
        }

        /// <summary>
        /// 海科融通公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void HkrtPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            //获取订单类型
            reqParams.Add("out_trade_no", payOrder.PayOrderId); //服务商交易订单号 服务商的交易订单编号（同一服务商下唯一）
            reqParams.Add("total_amount", AmountUtil.ConvertCent2Dollar(payOrder.Amount)); //订单金额 订单总金额，以元为单位
            reqParams.Add("pn", ""); //SAAS终端号
            reqParams.Add("remark", payOrder.Subject); //交易备注
        }
    }
}

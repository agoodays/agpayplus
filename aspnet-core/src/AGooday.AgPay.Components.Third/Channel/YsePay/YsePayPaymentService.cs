using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.YsePay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.YsePay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.YsePay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Web;

namespace AGooday.AgPay.Components.Third.Channel.YsePay
{
    public class YsePayPaymentService : AbstractPaymentService
    {
        public YsePayPaymentService(ILogger<YsePayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSEPAY;
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

        public ChannelRetMsg YseBar(SortedDictionary<string, string> reqParams, string notifyUrl, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            if (mchAppConfigContext.IsIsvSubMch())
            {
                YsePayIsvParams isvParams = (YsePayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.PartnerId == null)
                {
                    throw new BizException("服务商配置为空。");
                }
                reqParams.Add("business_code", isvParams.BusinessCode);
            }
            else
            {
                throw new BizException("不支持普通商户配置");
            }
            // 发送请求
            string method = "ysepay.online.barcodepay", repMethod = "ysepay_online_barcodepay_response";
            JObject resJSON = PackageParamAndReq(YsePayConfig.QRCODE_GATEWAY, method, repMethod, reqParams, notifyUrl, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            var data = resJSON.GetValue(repMethod)?.ToObject<JObject>();
            string code = data?.GetValue("code").ToString();
            string msg = data?.GetValue("msg").ToString();
            data.TryGetString("sub_code", out string subCode);
            data.TryGetString("sub_msg", out string subMsg);
            channelRetMsg.ChannelMchNo = string.Empty;
            try
            {
                if ("10000".Equals(code))
                {
                    data.TryGetString("trade_no", out string tradeNo);//银盛支付交易流水号
                    data.TryGetString("channel_recv_sn", out string channelRecvSn);//渠道返回流水号	
                    data.TryGetString("channel_send_sn", out string channelSendSn);//发往渠道流水号
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    data.TryGetString("buyer_user_id", out string buyerUserId);
                    data.TryGetString("openid", out string openid);
                    string tradeStatus = data.GetValue("trade_status").ToString();
                    var transStat = YsePayEnum.ConvertTradeStatus(tradeStatus);
                    switch (transStat)
                    {
                        case YsePayEnum.TradeStatus.WAIT_BUYER_PAY:
                        case YsePayEnum.TradeStatus.TRADE_PROCESS:
                        case YsePayEnum.TradeStatus.TRADE_ABNORMALITY:
                        case YsePayEnum.TradeStatus.TRADE_SUCCESS:
                            channelRetMsg.ChannelOrderId = tradeNo;
                            channelRetMsg.ChannelUserId = openid ?? buyerUserId;
                            channelRetMsg.PlatformOrderId = channelRecvSn;
                            channelRetMsg.PlatformMchOrderId = channelSendSn;
                            if (transStat.Equals(YsePayEnum.TradeStatus.TRADE_SUCCESS))
                            {
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            }
                            else
                            {
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                            }
                            break;
                        case YsePayEnum.TradeStatus.TRADE_FAILD:
                        case YsePayEnum.TradeStatus.TRADE_FAILED:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            break;
                    }
                }
                else if ("50000".Equals(code) || "3501".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = subCode ?? code;
                    channelRetMsg.ChannelErrMsg = subMsg ?? msg;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = subCode ?? code;
                channelRetMsg.ChannelErrMsg = subMsg ?? msg;
            }

            return channelRetMsg;
        }

        /// <summary>
        /// 封装参数 & 统一请求
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="reqData"></param>
        /// <param name="logPrefix"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public JObject PackageParamAndReq(string apiUri, string method, string repMethod, SortedDictionary<string, string> reqData, string notifyUrl, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            // 签名
            string partnerId, businessCode, privateKeyFilePath, privateKeyPassword, publicKeyFilePath;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                YsePayIsvParams isvParams = (YsePayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.PartnerId == null)
                {
                    _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }
                partnerId = isvParams.PartnerId;
                businessCode = isvParams.BusinessCode;
                privateKeyFilePath = ChannelCertConfigKit.GetCertFilePath(isvParams.PrivateKeyFile);
                privateKeyPassword = isvParams.PrivateKeyPassword;
                publicKeyFilePath = ChannelCertConfigKit.GetCertFilePath(isvParams.PublicKeyFile);
            }
            else
            {
                throw new BizException("不支持普通商户配置");
            }

            YsePayIsvSubMchParams isvsubMchParams = (YsePayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqData.Add("seller_id", isvsubMchParams.SellerId);
            reqData.Add("seller_name", isvsubMchParams.SellerName);
            //reqData.Add("business_code", businessCode);

            var reqParams = new SortedDictionary<string, string>();
            reqParams.Add("partner_id", partnerId);
            reqParams.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            reqParams.Add("charset", "utf-8");
            reqParams.Add("sign_type", "SM");
            reqParams.Add("version", "3.0");
            reqParams.Add("method", method);
            if (!string.IsNullOrWhiteSpace(notifyUrl))
            {
                reqParams.Add("notify_url", notifyUrl);
            }
            reqParams.Add("biz_content", JsonConvert.SerializeObject(reqData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None })); //业务请求参数，具体值参考API文档

            var sign = YseSignUtil.Sign(reqParams, privateKeyFilePath, privateKeyPassword); //SM 签名字符串
            reqParams.Add("sign", HttpUtility.UrlEncode(sign)); //加签结果
            reqData.TryGetValue("biz_content", out string bizContent);
            reqData["biz_content"] = HttpUtility.UrlEncode(bizContent);

            // 调起上游接口
            string url = apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            string reqText = string.Join("&", reqParams.Select(s => $"{s.Key}={s.Value}"));
            var stopwatch = new Stopwatch();
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} method={method} reqText={JsonConvert.SerializeObject(reqParams)} ");
            stopwatch.Restart();
            string resText = YseHttpUtil.DoPostFrom(url, reqText);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} method={method} resJSON={resText} time={stopwatch.ElapsedMilliseconds}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!YseSignUtil.Verify(resParams, publicKeyFilePath, repMethod))
            {
                _logger.LogWarning($"{logPrefix} 验签失败！ reqJSON={JsonConvert.SerializeObject(reqParams)} resJSON={resText}");
            }

            return resParams;
        }

        /// <summary>
        /// 随行付 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public static void UnifiedParamsSet(SortedDictionary<string, string> reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            YsePublicParams(reqParams, payOrder);
            //reqParams.Add("notify_url", notifyUrl); //交易异步通知地址，http或https开头。
        }

        /// <summary>
        /// 银盛 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(SortedDictionary<string, string> reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            YsePublicParams(reqParams, payOrder);
            //reqParams.Add("notify_url", notifyUrl); //异步通知地址
        }

        /// <summary>
        /// 银盛公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void YsePublicParams(SortedDictionary<string, string> reqParams, PayOrderDto payOrder)
        {
            reqParams.Add("shopdate", payOrder.CreatedAt.Value.ToString("yyyyMMdd")); // 请求格式：yyyyMMdd；示例值：20220905
            reqParams.Add("out_trade_no", payOrder.PayOrderId); //商户订单号（字母、数字、下划线）需保证在合作方系统中不重复
            reqParams.Add("total_amount", AmountUtil.ConvertCent2Dollar(payOrder.Amount)); //订单总金额(元)，格式：#########.##
            reqParams.Add("subject", payOrder.Subject); //订单标题
            reqParams.Add("timeout_express", $"{(payOrder.ExpiredTime.Value - payOrder.CreatedAt.Value).TotalMinutes}m"); //订单标题
        }
    }
}

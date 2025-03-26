using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.AllinPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AllinPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.AllinPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay
{
    public class AllinPayPaymentService : AbstractPaymentService
    {
        public AllinPayPaymentService(ILogger<AllinPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public AllinPayPaymentService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALLINPAY;
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

        public async Task<ChannelRetMsg> BarAsync(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/apiweb/unitorder/scanqrpay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON.GetValue("retcode").ToString(); //请求响应码
            string msg = resJSON.GetValue("retmsg").ToString(); //响应信息
            reqParams.TryGetString("cusid", out string cusid); // 商户号
            channelRetMsg.ChannelMchNo = cusid;
            //channelRetMsg.ChannelIsvNo = orgid;
            try
            {
                if ("SUCCESS".Equals(code))
                {
                    string trxstatus = resJSON.GetValue("trxstatus").ToString();
                    resJSON.TryGetString("trxid", out string trxid);
                    resJSON.TryGetString("reqsn", out string reqsn);
                    resJSON.TryGetString("chnltrxid", out string chnltrxid);
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    resJSON.TryGetString("acct", out string acct);
                    switch (trxstatus)
                    {
                        case "0000":
                            channelRetMsg.ChannelOrderId = trxid;
                            channelRetMsg.ChannelUserId = acct;
                            channelRetMsg.PlatformOrderId = chnltrxid;
                            channelRetMsg.PlatformMchOrderId = reqsn;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            break;
                        case "2008":
                        case "2000":
                            //case "3088":
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            channelRetMsg.IsNeedQuery = true; // 开启轮询查单;
                            break;
                        default:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            break;
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
                channelRetMsg.ChannelErrCode = code;
                channelRetMsg.ChannelErrMsg = msg;
            }

            return channelRetMsg;
        }

        /// <summary>
        /// 获取通联正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetHost4env(byte? sandbox)
        {
            return CS.YES == sandbox ? AllinPayConfig.SANDBOX_SERVER_URL : AllinPayConfig.PROD_SERVER_URL;
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
        public async Task<JObject> PackageParamAndReqAsync(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            byte? sandbox;
            string signType, orgid = null, cusid, appid, privateKey, publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                // 获取支付参数
                AllinPayIsvParams isvParams = (AllinPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.Orgid == null)
                {
                    _logger.LogError("服务商配置为空：isvParams：{isvParams}", JsonConvert.SerializeObject(isvParams));
                    //_logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }

                AllinPayIsvSubMchParams isvsubMchParams = (AllinPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                sandbox = isvParams.Sandbox;
                signType = isvParams.SignType;
                orgid = isvParams.Orgid;
                cusid = isvsubMchParams.Cusid;
                appid = isvParams.AppId;
                privateKey = isvParams.PrivateKey;
                publicKey = isvParams.PublicKey;
            }
            else
            {
                // 获取支付参数
                AllinPayNormalMchParams normalMchParams = (AllinPayNormalMchParams)await _configContextQueryService.QueryNormalMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                sandbox = normalMchParams.Sandbox;
                signType = normalMchParams.SignType;
                cusid = normalMchParams.Cusid;
                appid = normalMchParams.AppId;
                privateKey = normalMchParams.PrivateKey;
                publicKey = normalMchParams.PublicKey;
            }
            if (string.IsNullOrWhiteSpace(orgid))
            {
                reqParams.Add("orgid", orgid);
            }
            reqParams.Add("cusid", orgid);
            reqParams.Add("appid", appid);
            reqParams.Add("randomstr", Guid.NewGuid().ToString("N"));

            // 签名
            reqParams.Add("signtype", signType);
            reqParams.Add("sign", AllinPaySignUtil.Sign(reqParams, privateKey));

            // 调起上游接口
            string url = GetHost4env(sandbox) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation("{logPrefix} unionId={unionId} url={url} reqJSON={reqParams}", logPrefix, unionId, url, JsonConvert.SerializeObject(reqParams));
            //_logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqJSON={JsonConvert.SerializeObject(reqParams)}");
            string resText = await AllinPayHttpUtil.DoPostJsonAsync(url, reqParams);
            _logger.LogInformation("{logPrefix} unionId={unionId} url={url} resJSON={resText}", logPrefix, unionId, url, resText);
            //_logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!AllinPaySignUtil.Verify(resParams, publicKey))
            {
                _logger.LogWarning("{logPrefix} 验签失败！ reqJSON={reqParams} resJSON={resText}", logPrefix, JsonConvert.SerializeObject(reqParams), resText);
                //_logger.LogWarning($"{logPrefix} 验签失败！ reqJSON={JsonConvert.SerializeObject(reqParams)} resJSON={resText}");
            }

            return resParams;
        }

        /// <summary>
        /// 通联 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public static void UnifiedParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            PublicParams(reqParams, payOrder);
            string payType = AllinPayEnum.GetPayType(payOrder.WayCode);
            reqParams.Add("paytype", payType);
            //var maxvalidtime = payOrder.WayType.Equals(PayWayType.WECHAT.ToString()) ? 120 : 1440;
            //var validtime = (payOrder.ExpiredTime.Value - payOrder.CreatedAt.Value).TotalMinutes;
            //reqParams.Add("validtime", validtime > maxvalidtime ? maxvalidtime : validtime);
            reqParams.Add("expiretime", payOrder.ExpiredTime?.ToString("yyyyMMddHHmmss"));
            reqParams.Add("notify_url", notifyUrl);
        }

        /// <summary>
        /// 通联 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            PublicParams(reqParams, payOrder);
            reqParams.Add("notify_url", notifyUrl); //支付结果通知地址不上送则交易成功后，无异步交易结果通知
        }

        /// <summary>
        /// 通联公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void PublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            reqParams.Add("reqsn", payOrder.PayOrderId);
            reqParams.Add("trxamt", payOrder.Amount);
            reqParams.Add("body", payOrder.Subject); //订单标题
        }
    }
}

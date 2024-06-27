using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.AllinPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.AllinPay.Enumerator;
using AGooday.AgPay.Payment.Api.Channel.AllinPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.AllinPay
{
    public class AllinPayPaymentService : AbstractPaymentService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(AllinPayPaymentService));

        public AllinPayPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
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

        public override AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).Pay(bizRQ, payOrder, mchAppConfigContext);
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }

        public ChannelRetMsg AllinBar(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/apiweb/unitorder/scanqrpay", reqParams, logPrefix, mchAppConfigContext);
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
        public static string GetAllinPayHost4env(byte sandbox)
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
        public JObject PackageParamAndReq(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            byte sandbox;
            string signType, orgid = null, cusid, appid, privateKey, publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                // 获取支付参数
                AllinPayIsvParams isvParams = (AllinPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.Orgid == null)
                {
                    log.Error($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }

                AllinPayIsvSubMchParams isvsubMchParams = (AllinPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
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
                AllinPayNormalMchParams normalMchParams = (AllinPayNormalMchParams)_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
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
            reqParams.Add("sign", AllinSignUtil.Sign(reqParams, privateKey));

            // 调起上游接口
            string url = GetAllinPayHost4env(sandbox) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            log.Info($"{logPrefix} unionId={unionId} url={url} reqJSON={JsonConvert.SerializeObject(reqParams)}");
            string resText = AllinHttpUtil.DoPostJson(url, reqParams);
            log.Info($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!AllinSignUtil.Verify(resParams, publicKey))
            {
                log.Warn($"{logPrefix} 验签失败！ reqJSON={JsonConvert.SerializeObject(reqParams)} resJSON={resText}");
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
            AllinPublicParams(reqParams, payOrder);
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
            AllinPublicParams(reqParams, payOrder);
            reqParams.Add("notify_url", notifyUrl); //支付结果通知地址不上送则交易成功后，无异步交易结果通知
        }

        /// <summary>
        /// 通联公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void AllinPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            reqParams.Add("reqsn", payOrder.PayOrderId);
            reqParams.Add("trxamt", payOrder.Amount);
            reqParams.Add("body", payOrder.Subject); //订单标题
        }
    }
}

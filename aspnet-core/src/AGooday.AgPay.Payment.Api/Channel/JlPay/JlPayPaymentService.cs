using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.JlPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.JlPay.Enumerator;
using AGooday.AgPay.Payment.Api.Channel.JlPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.JlPay
{
    public class JlPayPaymentService : AbstractPaymentService
    {
        public JlPayPaymentService(ILogger<JlPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.JLPAY;
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

        public ChannelRetMsg JlBar(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/api/pay/micropay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string retCode = resJSON?.GetValue("ret_code").ToString(); //业务响应码
            string retMsg = resJSON?.GetValue("ret_msg").ToString(); //业务响应信息	
            string mchId = resJSON?.GetValue("mch_id")?.ToString();
            string orgCode = resJSON?.GetValue("org_code")?.ToString();
            channelRetMsg.ChannelMchNo = mchId;
            channelRetMsg.ChannelIsvNo = orgCode;
            try
            {
                if ("00".Equals(retCode))
                {
                    resJSON.TryGetString("transaction_id", out string transactionId);
                    resJSON.TryGetString("chn_transaction_id", out string chnTransactionId);//用户账单上的交易订单号	
                    resJSON.TryGetString("sub_openid", out string subOpenid);
                    string _status = resJSON.GetValue("status").ToString();
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Pending:
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            channelRetMsg.IsNeedQuery = true; // 开启轮询查单;
                            break;
                        case JlPayEnum.Status.Success:
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelUserId = subOpenid;
                            channelRetMsg.PlatformOrderId = chnTransactionId;
                            channelRetMsg.PlatformMchOrderId = transactionId;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            break;
                        case JlPayEnum.Status.Failure:
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
                channelRetMsg.ChannelErrCode = retCode;
                channelRetMsg.ChannelErrMsg = retMsg;
            }

            return channelRetMsg;
        }

        /// <summary>
        /// 获取嘉联正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetJlPayHost4env(byte? sandbox)
        {
            return CS.YES == sandbox ? JlPayConfig.SANDBOX_SERVER_URL : JlPayConfig.PROD_SERVER_URL;
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
        public JObject PackageParamAndReq(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext, bool isPay = true)
        {
            // 签名
            byte? sandbox;
            string orgCode, mchId, termNo, privateKey, publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                JlPayIsvParams isvParams = (JlPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.OrgCode == null)
                {
                    _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }
                JlPayIsvSubMchParams isvsubMchParams = (JlPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                sandbox = isvParams.Sandbox;
                orgCode = isvParams.OrgCode;
                mchId = isvsubMchParams.MchId;
                termNo = isvsubMchParams.TermNo;
                privateKey = isvParams.RsaPrivateKey;
                publicKey = isvParams.RsaPublicKey;
            }
            else
            {
                throw new BizException("指定通道不支持普通商户模式。");
            }

            reqParams.Add("org_code", orgCode);
            reqParams.Add("mch_id", mchId);
            if (isPay)
            {
                reqParams.Add("term_no", termNo);
            }
            reqParams.Add("nonce_str", Guid.NewGuid().ToString("N"));
            var sign = JlSignUtil.Sign(reqParams, privateKey); //RSA 签名字符串
            reqParams.Add("sign", sign); //加签结果

            // 调起上游接口
            string url = GetJlPayHost4env(sandbox) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqJSON={JsonConvert.SerializeObject(reqParams)}");
            string resText = JlHttpUtil.DoPostJson(url, reqParams);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!JlSignUtil.Verify(resParams, publicKey))
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
        public static void UnifiedParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            JlPublicParams(reqParams, payOrder);
            reqParams.Add("notify_url", notifyUrl); //交易异步通知地址，http或https开头。
        }

        /// <summary>
        /// 嘉联 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            JlPublicParams(reqParams, payOrder);
            //reqParams.Add("notify_url", notifyUrl); //异步通知地址
        }

        /// <summary>
        /// 嘉联公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void JlPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            string payType = JlPayEnum.GetPayType(payOrder.WayCode);
            reqParams.Add("pay_type", payType);
            reqParams.Add("out_trade_no", payOrder.PayOrderId); //商户订单号（字母、数字、下划线）需保证在合作方系统中不重复
            reqParams.Add("total_fee", payOrder.Amount); //订单总金额，单位为分
            reqParams.Add("body", payOrder.Subject); //订单标题
            reqParams.Add("attach", payOrder.Body); //商品描述
            reqParams.Add("remark", payOrder.SellerRemark); //备注信息
            reqParams.Add("payment_valid_time", (payOrder.ExpiredTime.Value - payOrder.CreatedAt.Value).TotalMinutes); //订单支付的有效时间，单位：分钟，默认有效时间：20分钟
            reqParams.Add("mch_create_ip", payOrder.ClientIp);
        }
    }
}

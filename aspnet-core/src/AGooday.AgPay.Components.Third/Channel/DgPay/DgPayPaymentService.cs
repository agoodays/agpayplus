using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.DgPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.DgPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.DgPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.DgPay
{
    public class DgPayPaymentService : AbstractPaymentService
    {
        public DgPayPaymentService(ILogger<DgPayPaymentService> logger, 
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.DGPAY;
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

        public ChannelRetMsg DgBar(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/trade/payment/micropay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            var data = resJSON.GetValue("data")?.ToObject<JObject>();
            string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
            string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息
            string bankCode = null, bankDesc = null, bankMessage = null;
            data?.TryGetString("bank_code", out bankCode); //外部通道返回码
            data?.TryGetString("bank_desc", out bankDesc); //外部通道返回描述
            data?.TryGetString("bank_message", out bankMessage); //外部通道返回描述
            string code = bankCode ?? respCode;
            string msg = (bankMessage ?? bankDesc) ?? respDesc;
            string huifuId = data?.GetValue("huifu_id")?.ToString();
            channelRetMsg.ChannelMchNo = huifuId;
            try
            {
                if ("00000000".Equals(respCode) || "00000100".Equals(respCode))
                {
                    data.TryGetString("hf_seq_id", out string hfSeqId);//全局流水号
                    data.TryGetString("req_seq_id", out string reqSeqId);//请求流水号
                    data.TryGetString("out_trans_id", out string outTransId);//用户账单上的交易订单号	
                    data.TryGetString("party_order_id", out string partyOrderId);//用户账单上的商户订单号	
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    data.TryGetString("wx_user_id", out string userId);
                    var wxResponse = data.GetValue("wx_response")?.ToObject<JObject>();
                    var alipayResponse = data.GetValue("alipay_response")?.ToObject<JObject>();
                    var unionpayResponse = data.GetValue("unionpay_response")?.ToObject<JObject>();
                    var subOpenid = wxResponse?.GetValue("sub_openid").ToString();
                    var buyerId = alipayResponse?.GetValue("buyer_id").ToString();
                    string _transStat = data.GetValue("trans_stat").ToString();
                    var transStat = DgPayEnum.ConvertTransStat(_transStat);
                    switch (transStat)
                    {
                        case DgPayEnum.TransStat.P:
                        case DgPayEnum.TransStat.S:
                            channelRetMsg.ChannelOrderId = hfSeqId;
                            channelRetMsg.ChannelUserId = subOpenid ?? buyerId;
                            channelRetMsg.PlatformOrderId = outTransId;
                            channelRetMsg.PlatformMchOrderId = partyOrderId;
                            if (transStat.Equals(DgPayEnum.TransStat.S))
                            {
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            }
                            else
                            {
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                            }
                            break;
                        case DgPayEnum.TransStat.F:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            break;
                    }
                }
                else if ("90000000".Equals(respCode))
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
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
        /// 封装参数 & 统一请求
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="reqData"></param>
        /// <param name="logPrefix"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public JObject PackageParamAndReq(string apiUri, JObject reqData, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            // 签名
            string sysId, productId, huifuId, privateKey, publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                DgPayIsvParams isvParams = (DgPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.SysId == null)
                {
                    _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }

                DgPayIsvSubMchParams isvsubMchParams = (DgPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                sysId = isvParams.SysId;
                productId = isvParams.ProductId;
                huifuId = isvsubMchParams.HuifuId;
                privateKey = isvParams.RsaPrivateKey;
                publicKey = isvParams.RsaPublicKey;
            }
            else
            {
                var normalMchParams = (DgPayNormalMchParams)_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                if (normalMchParams.HuifuId == null)
                {
                    _logger.LogError($"商户配置为空：normalMchParams：{JsonConvert.SerializeObject(normalMchParams)}");
                    throw new BizException("商户配置为空。");
                }

                sysId = normalMchParams.HuifuId;
                huifuId = normalMchParams.HuifuId;
                productId = normalMchParams.ProductId;
                privateKey = normalMchParams.RsaPrivateKey;
                publicKey = normalMchParams.RsaPublicKey;
            }

            //reqData.Add("req_seq_id", Guid.NewGuid().ToString("N")); //同一huifu_id下当天唯一，示例值：rQ20211213111739475651
            reqData.Add("huifu_id", huifuId); // 渠道与一级代理商的直属商户ID；示例值：6666000123123123

            var sign = DgSignUtil.Sign(reqData, privateKey); //RSA 签名字符串
            var reqParams = new JObject();
            /**
             * 渠道商/商户的huifu_id
             * （1）当主体为渠道商时，此字段填写渠道商huifu_id；
             * （2）当主体为直连商户时，此字段填写商户huifu_id；
             *  示例值：6666000123120000
             **/
            reqParams.Add("sys_id", sysId);
            reqParams.Add("product_id", productId); //汇付分配的产品号，示例值：MCS
            reqParams.Add("sign", sign); //加签结果
            reqParams.Add("data", reqData.ToString(Formatting.None)); //业务请求参数，具体值参考API文档

            // 调起上游接口
            string url = "https://api.huifu.com/v2" + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqJSON={JsonConvert.SerializeObject(reqParams)}");
            string resText = DgHttpUtil.DoPostJson(url, reqParams);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!DgSignUtil.Verify(resParams, publicKey))
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
            DgPublicParams(reqParams, payOrder);
            string tradeType = DgPayEnum.GetTransType(payOrder.WayCode);
            reqParams.Add("trade_type", tradeType);
            reqParams.Add("notify_url", notifyUrl); //交易异步通知地址，http或https开头。
        }

        /// <summary>
        /// 斗拱 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            DgPublicParams(reqParams, payOrder);
            reqParams.Add("notify_url", notifyUrl); //异步通知地址
        }

        /// <summary>
        /// 斗拱公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void DgPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            reqParams.Add("req_date", payOrder.CreatedAt.Value.ToString("yyyyMMdd")); // 请求格式：yyyyMMdd；示例值：20220905
            reqParams.Add("req_seq_id", payOrder.PayOrderId); //商户订单号（字母、数字、下划线）需保证在合作方系统中不重复
            reqParams.Add("trans_amt", AmountUtil.ConvertCent2Dollar(payOrder.Amount)); //订单总金额(元)，格式：#########.##
            reqParams.Add("goods_desc", payOrder.Subject); //订单标题
            //reqParams.Add("goods_desc", payOrder.Body); //商品描述
            reqParams.Add("time_expire", payOrder.ExpiredTime.Value.ToString("yyyyMMddHHmmss")); //订单标题
        }
    }
}

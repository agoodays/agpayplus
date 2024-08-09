using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.LklPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LklPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.LklPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LklPay
{
    public class LklPayPaymentService : AbstractPaymentService
    {
        public LklPayPaymentService(ILogger<LklPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LKLPAY;
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

        public ChannelRetMsg LklBar(JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/api/v3/labs/trans/micropay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON?.GetValue("code").ToString(); //业务响应码
            string msg = resJSON?.GetValue("msg").ToString(); //业务响应信息	
            try
            {
                if ("BBS00000".Equals(code))
                {
                    var respData = resJSON.GetValue("resp_data")?.ToObject<JObject>();
                    respData.TryGetString("merchant_no", out string merchantNo);
                    respData.TryGetString("trade_no", out string tradeNo);//全局流水号
                    respData.TryGetString("acc_trade_no", out string accTradeNo);//账户端交易流水号
                    var accRespFields = respData.GetValue("acc_resp_fields")?.ToObject<JObject>();
                    var userId = accRespFields?.GetValue("user_id").ToString();

                    channelRetMsg.ChannelMchNo = merchantNo;
                    channelRetMsg.ChannelOrderId = tradeNo;
                    channelRetMsg.ChannelUserId = userId;
                    channelRetMsg.PlatformOrderId = accTradeNo;
                    channelRetMsg.PlatformMchOrderId = tradeNo;
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                }
                else if ("BBS11112".Equals(code) || "BBS11105".Equals(code) || "BBS10000".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
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
        /// 获取拉卡拉正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetLklPayHost4env(LklPayIsvParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? LklPayConfig.SANDBOX_SERVER_URL : LklPayConfig.PROD_SERVER_URL;
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
            LklPayIsvParams isvParams = (LklPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //if (isvParams.OrgCode == null)
            //{
            //    _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
            //    throw new BizException("服务商配置为空。");
            //}

            LklPayIsvSubMchParams isvsubMchParams = (LklPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqData.Add("merchant_no", isvsubMchParams.MerchantNo); // 拉卡拉分配的商户号
            reqData.Add("term_no", isvsubMchParams.TermNo); // 拉卡拉分配的业务终端号

            var reqParams = new JObject();
            /**
             * 渠道商/商户的huifu_id
             * （1）当主体为渠道商时，此字段填写渠道商huifu_id；
             * （2）当主体为直连商户时，此字段填写商户huifu_id；
             *  示例值：6666000123120000
             **/
            reqParams.Add("req_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
            reqParams.Add("version", "3.0"); //汇付分配的产品号，示例值：MCS
            reqParams.Add("req_data", reqData); //业务请求参数，具体值参考API文档

            // 调起上游接口
            string url = GetLklPayHost4env(isvParams) + apiUri;
            string unionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} reqJSON={JsonConvert.SerializeObject(reqParams)}");
            string resText = LklHttpUtil.DoPostJson(url, isvParams.AppId, isvParams.SerialNo, isvParams.PrivateCert, reqParams, out Dictionary<string, string> headers);
            _logger.LogInformation($"{logPrefix} unionId={unionId} url={url} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            // 验签
            var resParams = JObject.Parse(resText);
            if (!LklSignUtil.Verify(headers, isvParams.AppId, resText, isvParams.PublicCert))
            {
                _logger.LogWarning($"{logPrefix} 验签失败！ reqJSON={JsonConvert.SerializeObject(reqParams)} resJSON={resText}");
            }

            return resParams;
        }

        /// <summary>
        /// 拉卡拉 jsapi下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        public static void UnifiedParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl, string returnUrl)
        {
            LklPublicParams(reqParams, payOrder);
            string accountType = LklPayEnum.GetAccountType(payOrder.WayCode);
            string transType = LklPayEnum.GetTransType(payOrder.WayCode);
            reqParams.Add("account_type", accountType);
            reqParams.Add("trans_type", transType);
            reqParams.Add("notify_url", notifyUrl); //交易异步通知地址，http或https开头。
        }

        /// <summary>
        /// 拉卡拉 bar下单请求统一发送参数
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void BarParamsSet(JObject reqParams, PayOrderDto payOrder, string notifyUrl)
        {
            LklPublicParams(reqParams, payOrder);
            reqParams.Add("notify_url", notifyUrl); //异步通知地址
        }

        /// <summary>
        /// 拉卡拉公共参数赋值
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="payOrder"></param>
        public static void LklPublicParams(JObject reqParams, PayOrderDto payOrder)
        {
            reqParams.Add("out_trade_no", payOrder.PayOrderId); //商户交易流水号	商户系统唯一，对应数据库表中外部请求流水号。
            reqParams.Add("total_amount", payOrder.Amount); //单位分，整数型字符
            reqParams.Add("subject", payOrder.Subject); //订单标题
            //reqParams.Add("remark", payOrder.Body); //商品描述
            reqParams.Add("location_info", new JObject() {
                { "request_ip", payOrder.ClientIp }
            }); // 终端信息); //订单标题
        }
    }
}

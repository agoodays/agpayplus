using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.JlPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.JlPay.PayWay
{
    /// <summary>
    /// 嘉联 支付宝 小程序支付
    /// </summary>
    public class AliLite : JlPayPaymentService
    {
        public AliLite(ILogger<AliLite> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【嘉联(alipay)小程序支付】";
            AliLiteOrderRQ bizRQ = (AliLiteOrderRQ)rq;
            JObject reqParams = new JObject();
            AliLiteOrderRS res = ApiResBuilder.BuildSuccess<AliLiteOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            //嘉联扫一扫支付， 需要传入buyerUserId参数
            /*用户号（微信openid / 支付宝userid / 银联userid）*/
            reqParams.Add("buyer_id", bizRQ.GetChannelUserId());

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/api/pay/waph5pay", reqParams, logPrefix, mchAppConfigContext);
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
                    resJSON.TryGetString("status", out string _status);
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Pending:
                            resJSON.TryGetString("pay_info", out string tradeNo);
                            res.AlipayTradeNo = tradeNo;
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            break;
                        case JlPayEnum.Status.Failure:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = retCode;
                            channelRetMsg.ChannelErrMsg = retMsg;
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
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliLiteOrderRQ bizRQ = (AliLiteOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return Task.FromResult<string>(null);
        }
    }
}

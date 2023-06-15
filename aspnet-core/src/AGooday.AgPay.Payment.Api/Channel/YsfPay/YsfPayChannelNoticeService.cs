using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.YsfPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Channel.YsfPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay
{
    /// <summary>
    /// 云闪付回调
    /// </summary>
    public class YsfPayChannelNoticeService : AbstractChannelNoticeService
    {
        public YsfPayChannelNoticeService(ILogger<YsfPayChannelNoticeService> logger, 
            ConfigContextQueryService configContextQueryService) 
            : base(logger, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = GetReqParamJson(request);
                string payOrderId = @params.GetValue("orderNo").ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
            }
            catch (Exception e)
            {
                log.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                String logPrefix = "【处理云闪付支付回调】";

                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                log.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验支付回调
                bool verifyResult = VerifyParams(jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                log.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                ContentResult okResponse = TextResp("success");
                result.ResponseEntity=okResponse;
                result.ChannelOrderId=jsonParams.GetValue("transIndex").ToString();
                return result;
            }
            catch (Exception e)
            {
                log.LogError(e,"error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public bool VerifyParams(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {

            string orderNo = jsonParams.GetValue("orderNo").ToString();       // 商户订单号
            string txnAmt = jsonParams.GetValue("txnAmt").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                log.LogInformation($"订单ID为空 [orderNo]={orderNo}" );
                return false;
            }
            if (string.IsNullOrWhiteSpace(txnAmt))
            {
                log.LogInformation($"金额参数为空 [txnAmt] :{txnAmt}");
                return false;
            }

            YsfPayIsvParams isvParams = (YsfPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string ysfpayPublicKey = isvParams.YsfpayPublicKey;

            //验签失败
            if (!YsfSignUtil.Validate(JObject.FromObject(jsonParams), ysfpayPublicKey))
            {
                log.LogInformation($"【云闪付回调】 验签失败！ 回调参数：parameter = {jsonParams}, ysfpayPublicKey={ysfpayPublicKey} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(txnAmt))
            {
                log.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, txnAmt={txnAmt}, payOrderId={orderNo}" );
                return false;
            }
            return true;
        }
    }
}

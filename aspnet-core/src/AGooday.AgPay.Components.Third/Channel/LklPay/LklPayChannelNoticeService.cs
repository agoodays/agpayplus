using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.LklPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LklPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.LklPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.LklPay
{
    /// <summary>
    /// 拉卡拉回调
    /// </summary>
    public class LklPayChannelNoticeService : AbstractChannelNoticeService
    {
        public LklPayChannelNoticeService(ILogger<LklPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public LklPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LKLPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = await GetReqParamJSONAsync();
                string payOrderId = @params.GetValue("out_trade_no").ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override async Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理拉卡拉支付回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验支付回调
                bool verifyResult = await VerifyParamsAsync(request, jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("code", "SUCCESS");
                resJSON.Add("message", "执行成功");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                jsonParams.TryGetString("merchant_no", out string merchantNo);
                string tradeState = jsonParams.GetValue("trade_state").ToString();
                string tradeNo = jsonParams.GetValue("trade_no").ToString();//拉卡拉商户订单号
                string accTradeNo = jsonParams.GetValue("acc_trade_no").ToString();//拉卡拉商户订单号
                jsonParams.TryGetString("user_id1", out string userId1);
                jsonParams.TryGetString("user_id2", out string userId2);
                var orderStatus = LklPayEnum.ConvertTradeState(tradeState);
                switch (orderStatus)
                {
                    case LklPayEnum.TradeState.SUCCESS:
                        result.ChannelMchNo = merchantNo;
                        result.ChannelOrderId = tradeNo;
                        result.ChannelUserId = userId2 ?? userId1;
                        result.PlatformOrderId = accTradeNo;
                        result.PlatformMchOrderId = tradeNo;
                        result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        break;
                    case LklPayEnum.TradeState.FAIL:
                        result.ChannelState = ChannelState.CONFIRM_FAIL;
                        break;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public async Task<bool> VerifyParamsAsync(HttpRequest request, JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string ordNo = jsonParams.GetValue("out_trade_no").ToString();       // 商户订单号
            string amt = jsonParams.GetValue("total_amount").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(ordNo))
            {
                _logger.LogInformation($"订单ID为空 [orderNo]={ordNo}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(amt))
            {
                _logger.LogInformation($"金额参数为空 [amt] :{amt}");
                return false;
            }

            LklPayIsvParams isvParams = (LklPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string publicKey = isvParams.PublicCert;

            //验签失败
            var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
            if (!LklSignUtil.Verify(headers, isvParams.AppId, jsonParams.ToString(), publicKey))
            {
                _logger.LogInformation($"【拉卡拉回调】 验签失败！ 回调参数：parameter = {jsonParams}, publicKey={publicKey} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(amt))
            {
                _logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={ordNo}");
                return false;
            }
            return true;
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.HkrtPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.HkrtPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.HkrtPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.HkrtPay
{
    /// <summary>
    /// 海科融通回调
    /// </summary>
    public class HkrtPayChannelNoticeService : AbstractChannelNoticeService
    {
        private readonly HkrtPayPaymentService _paymentService;
        public HkrtPayChannelNoticeService(ILogger<HkrtPayChannelNoticeService> logger,
            IServiceProvider serviceProvider,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
            _paymentService = ActivatorUtilities.CreateInstance<HkrtPayPaymentService>(serviceProvider);
        }

        public HkrtPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.HKRTPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                string resText = await GetReqParamFromBodyAsync();
                var resJson = XmlUtil.ConvertToJson(resText);
                var resParams = JObject.Parse(resJson);
                string payOrderId = resParams.GetValue("third_order_id").ToString();
                return new Dictionary<string, object>() { { payOrderId, resText } };
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

                string logPrefix = "【处理海科融通支付回调】";
                // 获取请求参数
                var resText = @params?.ToString();
                _logger.LogInformation("{logPrefix} 回调参数, 报文: {resText}", logPrefix, resText);
                //_logger.LogInformation($"{logPrefix} 回调参数, 报文: {resText}");
                var resJson = XmlUtil.ConvertToJson(@params.ToString());
                var resParams = JObject.Parse(resJson);

                // 校验支付回调
                bool verifyResult = await VerifyParamsAsync(resText, resParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation("{logPrefix} 验证支付通知数据及签名通过", logPrefix);
                //_logger.LogInformation($"{logPrefix} 验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("result", "SUCCESS");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                string status = resParams.GetValue("trade_status").ToString();
                string type = resJSON.GetValue("trade_type").ToString();
                string trade_no = resJSON.GetValue("trade_no").ToString();//交易订单号 SaaS平台的交易订单编号
                string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号
                resJSON.TryGetString("alipay_no", out string alipay_no);
                resJSON.TryGetString("weixin_no", out string weixin_no);
                var tradeStatus = HkrtPayEnum.ConvertTradeStatus(status);
                switch (tradeStatus)
                {
                    case HkrtPayEnum.TradeStatus.Success:
                        result.ChannelOrderId = trade_no;
                        var tradeType = HkrtPayEnum.ConvertTradeType(type);
                        var attach = _paymentService.GetAttach(resJSON);
                        attach.TryGetString("out_trade_no", out string out_trade_no);
                        result.PlatformMchOrderId = out_trade_no;
                        switch (tradeType)
                        {
                            case HkrtPayEnum.TradeType.WX:
                                attach.TryGetString("sub_openid", out string sub_openid);
                                attach.TryGetString("transaction_id", out string transaction_id);
                                result.ChannelUserId = sub_openid;
                                result.PlatformOrderId = transaction_id;
                                break;
                            case HkrtPayEnum.TradeType.ALI:
                                attach.TryGetString("sub_openid", out string buyer_user_id);
                                attach.TryGetString("trade_no", out string ali_trade_no);
                                result.ChannelUserId = buyer_user_id;
                                result.PlatformOrderId = ali_trade_no;
                                break;
                            case HkrtPayEnum.TradeType.UNIONQR:
                                break;
                        }
                        result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        break;
                    case HkrtPayEnum.TradeStatus.Failed:
                        result.ChannelState = ChannelState.CONFIRM_FAIL;
                        break;
                        //case HkrtPayEnum.TradeStatus.Paying:
                        //    result.ChannelState = ChannelState.WAITING;
                        //    result.IsNeedQuery = true; // 开启轮询查单;
                        //    break;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public async Task<bool> VerifyParamsAsync(string resText, JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string trade_no = jsonParams.GetValue("trade_no").ToString();       // 商户订单号
            string total_amount = jsonParams.GetValue("total_amount").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(trade_no))
            {
                _logger.LogInformation("订单ID为空 [trade_no]={trade_no}", trade_no);
                //_logger.LogInformation($"订单ID为空 [trade_no]={trade_no}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(total_amount))
            {
                _logger.LogInformation("金额参数为空 [total_amount]={total_amount}", total_amount);
                //_logger.LogInformation($"金额参数为空 [total_amount]={total_amount}");
                return false;
            }

            HkrtPayIsvParams isvParams = (HkrtPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string tradeKey = isvParams.AccessKey;

            //验签失败
            if (!HkrtPaySignUtil.Verify(jsonParams, tradeKey))
            {
                _logger.LogInformation("【海科融通回调】 验签失败！ 回调参数: resParams={resText}, key={tradeKey}", resText, tradeKey);
                //_logger.LogInformation($"【海科融通回调】 验签失败！ 回调参数: resParams={resText}, key={tradeKey}");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(total_amount))
            {
                _logger.LogInformation("订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={trade_no}", dbPayAmt, total_amount, trade_no);
                //_logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={total_amount}, payOrderId={trade_no}");
                return false;
            }
            return true;
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.LesPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LesPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.LesPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.LesPay
{
    /// <summary>
    /// 乐刷回调
    /// </summary>
    public class LesPayChannelNoticeService : AbstractChannelNoticeService
    {
        public LesPayChannelNoticeService(ILogger<LesPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public LesPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LESPAY;
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

                string logPrefix = "【处理乐刷支付回调】";
                // 获取请求参数
                var resText = @params?.ToString();
                _logger.LogInformation("{logPrefix} 回调参数, resParams：{resText}", logPrefix, resText);
                //_logger.LogInformation($"{logPrefix} 回调参数, resParams：{resText}");
                var resJson = XmlUtil.ConvertToJson(@params.ToString());
                var resParams = JObject.Parse(resJson);

                // 校验支付回调
                bool verifyResult = await VerifyParamsAsync(resText, resParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation("{logPrefix}验证支付通知数据及签名通过", logPrefix);
                //_logger.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                var okResponse = TextResp("000000");
                result.ResponseEntity = okResponse;

                resParams.TryGetString("error_code", out string error_code).ToString(); //错误码
                if (string.IsNullOrWhiteSpace(error_code))
                {
                    string status = resParams.GetValue("status").ToString();
                    string leshua_order_id = resParams.GetValue("leshua_order_id").ToString();//乐刷订单号
                    resParams.TryGetString("sub_merchant_id", out string sub_merchant_id);//渠道商商户号
                    resParams.TryGetString("out_transaction_id", out string out_transaction_id);//微信、支付宝等订单号
                    resParams.TryGetString("channel_order_id", out string channel_order_id);//通道订单号
                    resParams.TryGetString("sub_openid", out string sub_openid);//用户子标识 微信：公众号APPID下用户唯一标识；支付宝：买家的支付宝用户ID
                    var orderStatus = LesPayEnum.ConvertOrderStatus(status);
                    switch (orderStatus)
                    {
                        case LesPayEnum.OrderStatus.PaySuccess:
                            result.ChannelOrderId = leshua_order_id;
                            result.ChannelUserId = sub_openid;
                            result.PlatformOrderId = out_transaction_id;
                            result.PlatformMchOrderId = channel_order_id;
                            result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            break;
                        case LesPayEnum.OrderStatus.PayFail:
                            result.ChannelState = ChannelState.CONFIRM_FAIL;
                            break;
                            //case LesPayEnum.OrderStatus.Paying:
                            //    result.ChannelState = ChannelState.WAITING;
                            //    result.IsNeedQuery = true; // 开启轮询查单;
                            //    break;
                    }
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = error_code;
                    result.ChannelErrMsg = $"乐刷支付回调错误{error_code}";
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
            string third_order_id = jsonParams.GetValue("third_order_id").ToString();       // 商户订单号
            string amt = jsonParams.GetValue("amt").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(third_order_id))
            {
                _logger.LogInformation("订单ID为空 [orderNo]={third_order_id}", third_order_id);
                //_logger.LogInformation($"订单ID为空 [orderNo]={third_order_id}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(amt))
            {
                _logger.LogInformation("金额参数为空 [amt] :{amt}", amt);
                //_logger.LogInformation($"金额参数为空 [amt] :{amt}");
                return false;
            }

            LesPayIsvParams isvParams = (LesPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string noticeKey = isvParams.NoticeKey;

            //验签失败
            if (!LesPaySignUtil.Verify(jsonParams, noticeKey))
            {
                _logger.LogInformation("【乐刷回调】 验签失败！ 回调参数：resParams = {resText}, key = {noticeKey} ", resText, noticeKey);
                //_logger.LogInformation($"【乐刷回调】 验签失败！ 回调参数：resParams = {resText}, key = {noticeKey} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(amt))
            {
                _logger.LogInformation("订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={third_order_id}", dbPayAmt, amt, third_order_id);
                //_logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={third_order_id}");
                return false;
            }
            return true;
        }
    }
}

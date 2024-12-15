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
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.LesPay
{
    /// <summary>
    /// 乐刷 退款回调接口实现类
    /// </summary>
    public class LesPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public LesPayChannelRefundNoticeService(ILogger<LesPayChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public LesPayChannelRefundNoticeService()
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
                string refundOrderId = resParams.GetValue("third_order_id").ToString();
                return new Dictionary<string, object>() { { refundOrderId, resParams } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override async Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理乐刷退款回调】";
                // 获取请求参数
                var resText = @params?.ToString();
                _logger.LogInformation($"{logPrefix} 回调参数, resParams：{resText}");
                var resJson = XmlUtil.ConvertToJson(resText);
                var resParams = JObject.Parse(resJson);

                // 校验退款回调
                bool verifyResult = await VerifyParamsAsync(resText, resParams, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation($"{logPrefix}验证退款通知数据及签名通过");

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
                        case LesPayEnum.OrderStatus.RefundSuccess:
                            result.ChannelOrderId = leshua_order_id;
                            result.ChannelUserId = sub_openid;
                            result.PlatformOrderId = out_transaction_id;
                            result.PlatformMchOrderId = channel_order_id;
                            result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            break;
                        case LesPayEnum.OrderStatus.RefundFail:
                            result.ChannelState = ChannelState.CONFIRM_FAIL;
                            break;
                            //case LesPayEnum.OrderStatus.Refunding:
                            //    result.ChannelState = ChannelState.WAITING;
                            //    result.IsNeedQuery = true; // 开启轮询查单;
                            //    break;
                    }
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = error_code;
                    result.ChannelErrMsg = $"乐刷退款回调错误{error_code}";
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public async Task<bool> VerifyParamsAsync(string resText, JObject jsonParams, MchAppConfigContext mchAppConfigContext)
        {
            LesPayIsvParams isvParams = (LesPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string noticeKey = isvParams.NoticeKey;

            //验签失败
            if (!LesSignUtil.Verify(jsonParams, noticeKey))
            {
                _logger.LogInformation($"【乐刷回调】 验签失败！ 回调参数：resParams = {resText}, key = {noticeKey}");
                return false;
            }
            return true;
        }
    }
}

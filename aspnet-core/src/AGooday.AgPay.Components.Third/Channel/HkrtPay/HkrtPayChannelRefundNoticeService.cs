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
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.HkrtPay
{
    /// <summary>
    /// 海科融通 退款回调接口实现类
    /// </summary>
    public class HkrtPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public HkrtPayChannelRefundNoticeService(ILogger<HkrtPayChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public HkrtPayChannelRefundNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.HKRTPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                string resText = GetReqParamFromBody();
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

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理海科融通退款回调】";
                // 获取请求参数
                var resText = @params?.ToString();
                _logger.LogInformation($"{logPrefix} 回调参数, resParams：{resText}");
                var resJson = XmlUtil.ConvertToJson(resText);
                var resParams = JObject.Parse(resJson);

                // 校验退款回调
                bool verifyResult = VerifyParams(resText, resParams, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation($"{logPrefix}验证退款通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("result", "SUCCESS");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                string status = resParams.GetValue("trade_status").ToString();
                string type = resJSON.GetValue("trade_type").ToString();
                string trade_no = resJSON.GetValue("trade_no").ToString();//交易订单号 SaaS平台的交易订单编号
                string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号
                var refundStatus = HkrtPayEnum.ConvertRefundStatus(status);
                switch (refundStatus)
                {
                    case HkrtPayEnum.RefundStatus.Success:
                        result.ChannelOrderId = trade_no;
                        result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        break;
                    case HkrtPayEnum.RefundStatus.Failed:
                        result.ChannelState = ChannelState.CONFIRM_FAIL;
                        break;
                        //case HkrtPayEnum.RefundStatus.Refunding:
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

        public bool VerifyParams(string resText, JObject jsonParams, MchAppConfigContext mchAppConfigContext)
        {
            HkrtPayIsvParams isvParams = (HkrtPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string tradeKey = isvParams.AccessKey;

            //验签失败
            if (!HkrtSignUtil.Verify(jsonParams, tradeKey))
            {
                _logger.LogInformation($"【海科融通回调】 验签失败！ 回调参数：resParams = {resText}, key = {tradeKey}");
                return false;
            }
            return true;
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.SxfPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.SxfPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.SxfPay
{
    /// <summary>
    /// 随行付 退款回调接口实现类
    /// </summary>
    public class SxfPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public SxfPayChannelRefundNoticeService(ILogger<SxfPayChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public SxfPayChannelRefundNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.SXFPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = await GetReqParamJsonAsync();
                string refundOrderId = @params.GetValue("ordNo").ToString();
                return new Dictionary<string, object>() { { refundOrderId, @params } };
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

                string logPrefix = "【处理随行付退款回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation("{logPrefix} 回调参数, 报文: {jsonParams}", logPrefix, jsonParams);
                //_logger.LogInformation($"{logPrefix} 回调参数, 报文: {jsonParams}");

                // 校验退款回调
                bool verifyResult = await VerifyParamsAsync(jsonParams, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation("{logPrefix} 验证退款通知数据及签名通过", logPrefix);
                //_logger.LogInformation($"{logPrefix} 验证退款通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("code", "success");
                resJSON.Add("msg", "成功");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                string bizCode = jsonParams.GetValue("bizCode").ToString(); //业务响应码
                string bizMsg = jsonParams.GetValue("bizMsg").ToString(); //业务响应信息
                if ("0000".Equals(bizCode))
                {
                    string uuid = jsonParams.GetValue("uuid").ToString();//天阙平台订单号
                    /*落单号
                    仅供退款使用
                    消费者账单中的条形码订单号*/
                    jsonParams.TryGetString("sxfUuid", out string sxfUuid);
                    jsonParams.TryGetString("channelId", out string channelId);//渠道商商户号
                    jsonParams.TryGetString("transactionId", out string transactionId);//微信/支付宝流水号
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    jsonParams.TryGetString("buyerId", out string buyerId);
                    result.ChannelOrderId = uuid;
                    result.ChannelUserId = buyerId;
                    result.PlatformOrderId = transactionId;
                    result.PlatformMchOrderId = sxfUuid;
                    result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = bizCode;
                    result.ChannelErrMsg = bizMsg;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public async Task<bool> VerifyParamsAsync(JObject jsonParams, MchAppConfigContext mchAppConfigContext)
        {
            SxfPayIsvParams isvParams = (SxfPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string publicKey = isvParams.PublicKey;

            //验签失败
            if (!SxfPaySignUtil.Verify(jsonParams, publicKey))
            {
                _logger.LogInformation("【随行付回调】 验签失败！ 回调参数：parameter={jsonParams}, publicKey={publicKey}", jsonParams, publicKey);
                //_logger.LogInformation($"【随行付回调】 验签失败！ 回调参数：parameter={jsonParams}, publicKey={publicKey}");
                return false;
            }
            return true;
        }
    }
}

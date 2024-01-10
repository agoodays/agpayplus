using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.LcswPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.LcswPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Payment.Api.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Payment.Api.Channel.LcswPay
{
    /// <summary>
    /// 利楚扫呗回调
    /// </summary>
    public class LcswPayChannelNoticeService : AbstractChannelNoticeService
    {
        public LcswPayChannelNoticeService(ILogger<LcswPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LCSWPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = GetReqParamJSON();
                string payOrderId = @params.GetValue("terminal_trace").ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
            }
            catch (Exception e)
            {
                log.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理利楚扫呗支付回调】";
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
                JObject resJSON = new JObject();
                resJSON.Add("return_code", "01");
                resJSON.Add("return_msg", "success");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
                string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
                jsonParams.TryGetString("merchant_no", out string merchantNo); // 商户号
                if ("01".Equals(returnCode))
                {
                    jsonParams.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode))
                    {
                        jsonParams.TryGetString("out_trade_no", out string outTradeNo);// 平台唯一订单号
                        jsonParams.TryGetString("channel_trade_no", out string channelTradeNo);// 微信/支付宝流水号
                        jsonParams.TryGetString("channel_order_no", out string channelOrderNo);// 银行渠道订单号，微信支付时显示在支付成功页面的条码，可用作扫码查询和扫码退款时匹配
                        jsonParams.TryGetString("user_id", out string userId);// 付款方用户id，服务商appid下的“微信openid”、“支付宝账户”
                        result.ChannelMchNo = merchantNo;
                        result.ChannelOrderId = outTradeNo;
                        result.ChannelUserId = userId;
                        result.PlatformOrderId = channelTradeNo;
                        result.PlatformMchOrderId = channelOrderNo;
                        result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    }
                    else
                    {
                        result.ChannelState = ChannelState.CONFIRM_FAIL;
                        result.ChannelErrCode = resultCode;
                        result.ChannelErrMsg = returnMsg;
                    }
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = returnCode;
                    result.ChannelErrMsg = returnMsg;
                }
                return result;
            }
            catch (Exception e)
            {
                log.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public bool VerifyParams(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string terminalTrace = jsonParams.GetValue("terminal_trace").ToString();       // 商户订单号
            string totalFee = jsonParams.GetValue("total_fee").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(terminalTrace))
            {
                log.LogInformation($"订单ID为空 [terminalTrace]={terminalTrace}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(totalFee))
            {
                log.LogInformation($"金额参数为空 [totalFee] :{totalFee}");
                return false;
            }

            LcswPayNormalMchParams lcswParams = (LcswPayNormalMchParams)configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

            //验签
            string key = lcswParams.AccessToken;

            //验签失败
            if (!LcswSignUtil.Verify(jsonParams, key))
            {
                log.LogInformation($"【利楚扫呗回调】 验签失败！ 回调参数：parameter = {jsonParams}, publicKey={key} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(totalFee))
            {
                log.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, totalFee={totalFee}, payOrderId={terminalTrace}");
                return false;
            }
            return true;
        }
    }
}

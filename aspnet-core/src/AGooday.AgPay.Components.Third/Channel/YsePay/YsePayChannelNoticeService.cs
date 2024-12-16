using System.Net;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.YsePay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.YsePay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.YsePay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.YsePay
{
    /// <summary>
    /// 银盛回调
    /// </summary>
    public class YsePayChannelNoticeService : AbstractChannelNoticeService
    {
        public YsePayChannelNoticeService(ILogger<YsePayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public YsePayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSEPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = await GetReqParamJSONAsync();
                string payOrderId = @params.GetValue("out_trade_no")?.ToString();
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

                string logPrefix = "【处理银盛支付回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验支付回调
                bool verifyResult = await VerifyParamsAsync(jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
                }
                _logger.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                jsonParams.TryGetString("trade_no", out string tradeNo);//银盛支付交易流水号
                jsonParams.TryGetString("channel_recv_sn", out string channelRecvSn);//渠道返回流水号	
                jsonParams.TryGetString("channel_send_sn", out string channelSendSn);//发往渠道流水号
                string tradeStatus = jsonParams.GetValue("trade_status").ToString();
                var transStat = YsePayEnum.ConvertTradeStatus(tradeStatus);
                switch (transStat)
                {
                    case YsePayEnum.TradeStatus.TRADE_SUCCESS:
                        result.ChannelOrderId = tradeNo;
                        result.PlatformOrderId = channelRecvSn;
                        result.PlatformMchOrderId = channelSendSn;
                        result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        break;
                    case YsePayEnum.TradeStatus.TRADE_FAILD:
                    case YsePayEnum.TradeStatus.TRADE_FAILED:
                        result.ChannelState = ChannelState.CONFIRM_FAIL;
                        break;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<bool> VerifyParamsAsync(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string outTradeNo = jsonParams.GetValue("out_trade_no").ToString(); // 商户订单号
            string totalAmount = jsonParams.GetValue("total_amount").ToString();  // 支付金额
            if (string.IsNullOrWhiteSpace(outTradeNo))
            {
                _logger.LogInformation($"订单ID为空 [outTradeNo]={outTradeNo}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(totalAmount))
            {
                _logger.LogInformation($"金额参数为空 [amt] :{totalAmount}");
                return false;
            }

            //验签
            string certFilePath;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                YsePayIsvParams isvParams = (YsePayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.PartnerId == null)
                {
                    _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }
                certFilePath = ChannelCertConfigKit.GetCertFilePath(isvParams.PublicKeyFile);
            }
            else
            {
                throw new BizException("不支持普通商户配置");
            }

            //验签失败
            if (!YsePaySignUtil.Verify(jsonParams, certFilePath))
            {
                _logger.LogInformation($"【银盛回调】 验签失败！ 回调参数：parameter = {jsonParams}, certFilePath={certFilePath} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(totalAmount))
            {
                _logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={totalAmount}, payOrderId={outTradeNo}");
                return false;
            }
            return true;
        }
    }
}

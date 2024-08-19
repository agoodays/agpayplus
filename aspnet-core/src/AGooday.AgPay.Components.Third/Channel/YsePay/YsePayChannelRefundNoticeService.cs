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
using System.Net;
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.YsePay
{
    /// <summary>
    /// 银盛支付 退款回调接口实现类
    /// </summary>
    public class YsePayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public YsePayChannelRefundNoticeService(ILogger<YsePayChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public YsePayChannelRefundNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSEPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = GetReqParamJSON();
                string refundOrderId = @params.GetValue("out_trade_no")?.ToString();
                return new Dictionary<string, object>() { { refundOrderId, @params } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理银盛支付退款回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验退款回调
                bool verifyResult = VerifyParams(jsonParams, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation($"{logPrefix}验证退款通知数据及签名通过");

                //验签成功后判断上游订单状态
                jsonParams.TryGetString("trade_no", out string tradeNo);//银盛支付交易流水号
                jsonParams.TryGetString("channel_recv_sn", out string channelRecvSn);//渠道返回流水号	
                jsonParams.TryGetString("channel_send_sn", out string channelSendSn);//发往渠道流水号
                string tradeStatus = jsonParams.GetValue("trade_status").ToString();
                var transStat = YsePayEnum.ConvertTradeStatus(tradeStatus);

                switch (transStat)
                {
                    case YsePayEnum.TradeStatus.TRADE_SUCCESS:
                        result.ChannelOrderId = tradeNo;
                        result.PlatformOrderId = channelSendSn;
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

        public bool VerifyParams(JObject jsonParams, MchAppConfigContext mchAppConfigContext)
        {
            //验签
            string certFilePath;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                YsePayIsvParams isvParams = (YsePayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

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
            if (!YseSignUtil.Verify(jsonParams, certFilePath))
            {
                _logger.LogInformation($"【银盛支付回调】 验签失败！ 回调参数：parameter = {jsonParams}, certFilePath={certFilePath} ");
                return false;
            }
            return true;
        }
    }
}

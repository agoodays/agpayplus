using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.PpPay
{
    public class PpPayChannelNoticeService : AbstractChannelNoticeService
    {
        public PpPayChannelNoticeService(ILogger<PpPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public PpPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.PPPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            // 同步和异步需要不同的解析方案
            // 异步需要从 webhook 中读取，所以这里读取方式不太一样
            if (noticeTypeEnum == NoticeTypeEnum.DO_NOTIFY)
            {
                JObject paramsObj = await GetReqParamJSONAsync();
                string orderId = paramsObj.SelectToken("resource.purchase_units[0].invoice_id")?.ToString();
                return new Dictionary<string, object>() { { orderId, paramsObj } };
            }
            else
            {
                if (string.IsNullOrEmpty(urlOrderId))
                {
                    throw ResponseException.BuildText("ERROR");
                }
                try
                {
                    JObject paramsObj = JObject.Parse(GetReqParamJSONAsync().ToString());
                    return new Dictionary<string, object>() { { urlOrderId, paramsObj } };
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "error");
                    throw ResponseException.BuildText("ERROR");
                }
            }
        }

        public override Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object parameters, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                if (noticeTypeEnum == NoticeTypeEnum.DO_RETURN)
                {
                    return DoReturn(request, parameters, payOrder, mchAppConfigContext);
                }
                return DoNotify(request, parameters, payOrder, mchAppConfigContext);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public Task<ChannelRetMsg> DoReturn(HttpRequest request, object parameters, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject obj = JObject.FromObject(parameters); ;
            // 获取 Paypal 订单 ID
            string ppOrderId = obj.GetValue("token")?.ToString();
            // 统一处理订单
            return mchAppConfigContext.GetPaypalWrapper().ProcessOrderAsync(ppOrderId, payOrder);
        }

        public Task<ChannelRetMsg> DoNotify(HttpRequest request, object parameters, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject obj = JObject.FromObject(parameters);
            // 获取 Paypal 订单 ID
            string ppOrderId = obj.SelectToken("resource.id")?.ToString();
            // 统一处理订单
            return mchAppConfigContext.GetPaypalWrapper().ProcessOrderAsync(ppOrderId, payOrder, true);
        }
    }
}

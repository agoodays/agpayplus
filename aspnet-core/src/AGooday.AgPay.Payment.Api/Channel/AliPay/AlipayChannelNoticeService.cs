using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.YsfPay;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using log4net;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    public class AlipayChannelNoticeService : AbstractChannelNoticeService
    {
        private static ILog log = LogManager.GetLogger(typeof(AlipayChannelNoticeService));

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = GetReqParamJson(request);
                string payOrderId = @params.GetValue("out_trade_no").ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
            }
            catch (Exception e)
            {
                log.Error("error", e);
                throw;
            }
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    public class WxPayChannelNoticeService : AbstractChannelNoticeService
    {
        private readonly IPayOrderService payOrderService;
        private readonly ILogger<WxPayChannelNoticeService> logger;

        public WxPayChannelNoticeService(ILogger<WxPayChannelNoticeService> logger,
            ConfigContextQueryService configContextQueryService,
            IPayOrderService payOrderService)
            : base(logger, configContextQueryService)
        {
            this.logger = logger;
            this.payOrderService = payOrderService;
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                // V3接口回调
                if (!string.IsNullOrEmpty(urlOrderId))
                {

                    // 获取订单信息
                    PayOrderDto payOrder = payOrderService.GetById(urlOrderId);
                    if (payOrder == null)
                    {
                        throw new BizException("订单不存在");
                    }

                    //获取支付参数 (缓存数据) 和 商户信息
                    MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);
                    if (mchAppConfigContext == null)
                    {
                        throw new BizException("获取商户信息失败");
                    }

                    // 验签 && 获取订单回调数据

                }
                else
                {
                    // V2接口回调
                }

                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "error");
                throw new ResponseException("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using log4net;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay
{
    public class YsfPayRefundService : AbstractRefundService
    {
        private static ILog logger = LogManager.GetLogger(typeof(YsfPayRefundService));
        private YsfPayPaymentService ysfpayPaymentService;
        public YsfPayRefundService(IServiceProvider serviceProvider, 
            ISysConfigService sysConfigService, 
            ConfigContextQueryService configContextQueryService) 
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            ysfpayPaymentService = serviceProvider.GetService<YsfPayPaymentService>();
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            throw new NotImplementedException();
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }

        public override ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }
    }
}

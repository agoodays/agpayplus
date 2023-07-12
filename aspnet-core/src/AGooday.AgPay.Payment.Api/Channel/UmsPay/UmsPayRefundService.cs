using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay
{
    /// <summary>
    /// 银联商务退款
    /// </summary>
    public class UmsPayRefundService : AbstractRefundService
    {
        public UmsPayRefundService(IServiceProvider serviceProvider, 
            ISysConfigService sysConfigService, 
            ConfigContextQueryService configContextQueryService) 
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
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

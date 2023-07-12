using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay
{
    /// <summary>
    /// 银联商务下单
    /// </summary>
    public class UmsPayPaymentService : AbstractPaymentService
    {
        public UmsPayPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            throw new NotImplementedException();
        }
    }
}

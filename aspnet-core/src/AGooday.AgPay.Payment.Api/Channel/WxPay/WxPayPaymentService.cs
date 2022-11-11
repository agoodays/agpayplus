using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    public class WxPayPaymentService : AbstractPaymentService
    {
        public WxPayPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            // 微信API版本
            WxServiceWrapper wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
            string apiVersion = wxServiceWrapper.Config.ApiVersion; 
            if (CS.PAY_IF_VERSION.WX_V2.Equals(apiVersion))
            {
                return PayWayUtil.GetRealPaywayService(this, payOrder.WayCode).Pay(bizRQ, payOrder, mchAppConfigContext);
            }
            else if (CS.PAY_IF_VERSION.WX_V3.Equals(apiVersion))
            {
                return PayWayUtil.GetRealPaywayV3Service(this, payOrder.WayCode).Pay(bizRQ, payOrder, mchAppConfigContext);
            }
            else
            {
                throw new BizException("不支持的微信支付API版本");
            }
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPaywayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Models;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 支付接口： 微信官方
    /// 支付方式： 自适应
    /// </summary>
    public class WxPayPaymentService : AbstractPaymentService
    {
        public WxPayPaymentService(ILogger<WxPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public WxPayPaymentService()
            : base()
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

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            // 微信API版本
            WxServiceWrapper wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);
            string apiVersion = wxServiceWrapper.Config.ApiVersion;
            if (CS.PAY_IF_VERSION.WX_V2.Equals(apiVersion))
            {
                return await PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PayAsync(bizRQ, payOrder, mchAppConfigContext);
            }
            else if (CS.PAY_IF_VERSION.WX_V3.Equals(apiVersion))
            {
                return await PayWayUtil.GetRealPayWayV3Service(this, payOrder.WayCode).PayAsync(bizRQ, payOrder, mchAppConfigContext);
            }
            else
            {
                throw new BizException("不支持的微信支付API版本");
            }
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }

        public async Task<(CreatePayUnifiedOrderRequest request, WxServiceWrapper wxServiceWrapper)> BuildUnifiedOrderRequestAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            var wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

            // 微信统一下单请求对象
            var request = new CreatePayUnifiedOrderRequest()
            {
                TradeType = "APP",
                OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                Body = payOrder.Subject,// 订单描述
                Detail = JsonConvert.DeserializeObject<CreatePayMicroPayRequest.Types.Detail>(payOrder.Body),
                FeeType = "CNY",
                TotalFee = Convert.ToInt32(payOrder.Amount),
                ClientIp = payOrder.ClientIp,
                NotifyUrl = GetNotifyUrl(),
                //ProductId = Guid.NewGuid().ToString("N")
            };

            //订单分账， 将冻结商户资金。
            if (IsDivisionOrder(payOrder))
            {
                request.IsProfitSharing = true;
            }

            //放置isv信息
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvSubMchParams = (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                request.SubMerchantId = isvSubMchParams.SubMchId;
                // 子商户subAppId不为空
                if (!string.IsNullOrEmpty(isvSubMchParams.SubMchAppId))
                {
                    request.SubAppId = isvSubMchParams.SubMchAppId;
                }
            }

            return (request, wxServiceWrapper);
        }
    }
}

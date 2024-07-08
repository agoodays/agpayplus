using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Channel.AliPay.Kits;
using AGooday.AgPay.Payment.Api.Channel.AllinPay;
using AGooday.AgPay.Payment.Api.Channel.DgPay;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay;
using AGooday.AgPay.Payment.Api.Channel.JlPay;
using AGooday.AgPay.Payment.Api.Channel.LcswPay;
using AGooday.AgPay.Payment.Api.Channel.LesPay;
using AGooday.AgPay.Payment.Api.Channel.LklPay;
using AGooday.AgPay.Payment.Api.Channel.PpPay;
using AGooday.AgPay.Payment.Api.Channel.SxfPay;
using AGooday.AgPay.Payment.Api.Channel.UmsPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Channel.YsePay;
using AGooday.AgPay.Payment.Api.Channel.YsfPay;
using AGooday.AgPay.Payment.Api.Utils;

namespace AGooday.AgPay.Payment.Api.Channel
{
    public class ChannelNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            #region ChannelUserService
            services.AddKeyedScoped<IChannelUserService, AliPayChannelUserService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IChannelUserService, WxPayChannelUserService>(CS.IF_CODE.WXPAY);
            services.AddScoped<IChannelServiceFactory<IChannelUserService>, ChannelServiceFactory<IChannelUserService>>();
            #endregion
            #region DivisionService
            services.AddKeyedScoped<IDivisionService, AliPayDivisionService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IDivisionService, WxPayDivisionService>(CS.IF_CODE.WXPAY);
            services.AddScoped<IChannelServiceFactory<IDivisionService>, ChannelServiceFactory<IDivisionService>>();
            #endregion
            #region DivisionRecordChannelNotifyService
            services.AddKeyedScoped<AliPayDivisionRecordChannelNotifyService>(CS.IF_CODE.ALIPAY);
            services.AddScoped<IChannelServiceFactory<AliPayDivisionRecordChannelNotifyService>, ChannelServiceFactory<AliPayDivisionRecordChannelNotifyService>>();
            #endregion
            #region PaymentService
            services.AddKeyedScoped<IPaymentService, AliPayPaymentService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IPaymentService, WxPayPaymentService>(CS.IF_CODE.WXPAY);
            services.AddKeyedScoped<IPaymentService, YsfPayPaymentService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IPaymentService, PpPayPaymentService>(CS.IF_CODE.PPPAY);
            services.AddKeyedScoped<IPaymentService, SxfPayPaymentService>(CS.IF_CODE.SXFPAY);
            services.AddKeyedScoped<IPaymentService, LesPayPaymentService>(CS.IF_CODE.LESPAY);
            services.AddKeyedScoped<IPaymentService, HkrtPayPaymentService>(CS.IF_CODE.HKRTPAY);
            services.AddKeyedScoped<IPaymentService, UmsPayPaymentService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IPaymentService, LcswPayPaymentService>(CS.IF_CODE.LCSWPAY);
            services.AddKeyedScoped<IPaymentService, DgPayPaymentService>(CS.IF_CODE.DGPAY);
            services.AddKeyedScoped<IPaymentService, LklPayPaymentService>(CS.IF_CODE.LKLPAY);
            services.AddKeyedScoped<IPaymentService, YsePayPaymentService>(CS.IF_CODE.YSEPAY);
            services.AddKeyedScoped<IPaymentService, AllinPayPaymentService>(CS.IF_CODE.ALLINPAY);
            services.AddKeyedScoped<IPaymentService, JlPayPaymentService>(CS.IF_CODE.JLPAY);
            services.AddScoped<IChannelServiceFactory<IPaymentService>, ChannelServiceFactory<IPaymentService>>();
            PayWayUtil.PayWayServiceRegister<AliPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<WxPayPaymentService>(services);
            PayWayUtil.PayWayV3ServiceRegister<WxPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<YsfPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<PpPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<SxfPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<LesPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<HkrtPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<UmsPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<LcswPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<DgPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<LklPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<YsePayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<AllinPayPaymentService>(services);
            PayWayUtil.PayWayServiceRegister<JlPayPaymentService>(services);
            #endregion
            #region RefundService
            services.AddKeyedScoped<IRefundService, AliPayRefundService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IRefundService, WxPayRefundService>(CS.IF_CODE.WXPAY);
            services.AddKeyedScoped<IRefundService, YsfPayRefundService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IRefundService, PpPayRefundService>(CS.IF_CODE.PPPAY);
            services.AddKeyedScoped<IRefundService, SxfPayRefundService>(CS.IF_CODE.SXFPAY);
            services.AddKeyedScoped<IRefundService, LesPayRefundService>(CS.IF_CODE.LESPAY);
            services.AddKeyedScoped<IRefundService, HkrtPayRefundService>(CS.IF_CODE.HKRTPAY);
            services.AddKeyedScoped<IRefundService, UmsPayRefundService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IRefundService, LcswPayRefundService>(CS.IF_CODE.LCSWPAY);
            services.AddKeyedScoped<IRefundService, DgPayRefundService>(CS.IF_CODE.DGPAY);
            services.AddKeyedScoped<IRefundService, LklPayRefundService>(CS.IF_CODE.LKLPAY);
            services.AddKeyedScoped<IRefundService, YsePayRefundService>(CS.IF_CODE.YSEPAY);
            services.AddKeyedScoped<IRefundService, AllinPayRefundService>(CS.IF_CODE.ALLINPAY);
            services.AddKeyedScoped<IRefundService, JlPayRefundService>(CS.IF_CODE.JLPAY);
            services.AddScoped<IChannelServiceFactory<IRefundService>, ChannelServiceFactory<IRefundService>>();
            #endregion
            #region ChannelNoticeService
            services.AddKeyedScoped<IChannelNoticeService, AliPayChannelNoticeService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IChannelNoticeService, WxPayChannelNoticeService>(CS.IF_CODE.WXPAY);
            services.AddKeyedScoped<IChannelNoticeService, YsfPayChannelNoticeService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IChannelNoticeService, PpPayChannelNoticeService>(CS.IF_CODE.PPPAY);
            services.AddKeyedScoped<IChannelNoticeService, SxfPayChannelNoticeService>(CS.IF_CODE.SXFPAY);
            services.AddKeyedScoped<IChannelNoticeService, LesPayChannelNoticeService>(CS.IF_CODE.LESPAY);
            services.AddKeyedScoped<IChannelNoticeService, HkrtPayChannelNoticeService>(CS.IF_CODE.HKRTPAY);
            services.AddKeyedScoped<IChannelNoticeService, UmsPayChannelNoticeService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IChannelNoticeService, LcswPayChannelNoticeService>(CS.IF_CODE.LCSWPAY);
            services.AddKeyedScoped<IChannelNoticeService, DgPayChannelNoticeService>(CS.IF_CODE.DGPAY);
            services.AddKeyedScoped<IChannelNoticeService, LklPayChannelNoticeService>(CS.IF_CODE.LKLPAY);
            services.AddKeyedScoped<IChannelNoticeService, YsePayChannelNoticeService>(CS.IF_CODE.YSEPAY);
            services.AddKeyedScoped<IChannelNoticeService, AllinPayChannelNoticeService>(CS.IF_CODE.ALLINPAY);
            services.AddKeyedScoped<IChannelNoticeService, JlPayChannelNoticeService>(CS.IF_CODE.JLPAY);
            services.AddScoped<IChannelServiceFactory<IChannelNoticeService>, ChannelServiceFactory<IChannelNoticeService>>();
            #endregion
            #region ChannelRefundNoticeService
            //services.AddKeyedScoped<IChannelRefundNoticeService, AliPayChannelRefundNoticeService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, WxPayChannelRefundNoticeService>(CS.IF_CODE.WXPAY);
            //services.AddKeyedScoped<IChannelRefundNoticeService, YsfPayChannelRefundNoticeService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, PpPayChannelRefundNoticeService>(CS.IF_CODE.PPPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, SxfPayChannelRefundNoticeService>(CS.IF_CODE.SXFPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, LesPayChannelRefundNoticeService>(CS.IF_CODE.LESPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, HkrtPayChannelRefundNoticeService>(CS.IF_CODE.HKRTPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, UmsPayChannelRefundNoticeService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, YsePayChannelRefundNoticeService>(CS.IF_CODE.YSEPAY);
            services.AddKeyedScoped<IChannelRefundNoticeService, AllinPayChannelRefundNoticeService>(CS.IF_CODE.ALLINPAY);
            services.AddScoped<IChannelServiceFactory<IChannelRefundNoticeService>, ChannelServiceFactory<IChannelRefundNoticeService>>();
            #endregion
            #region CloseService
            services.AddKeyedScoped<IPayOrderCloseService, AliPayPayOrderCloseService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IPayOrderCloseService, WxPayPayOrderCloseService>(CS.IF_CODE.WXPAY);
            services.AddKeyedScoped<IPayOrderCloseService, YsfPayPayOrderCloseService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IPayOrderCloseService, UmsPayPayOrderCloseService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IPayOrderCloseService, LcswPayPayOrderCloseService>(CS.IF_CODE.LCSWPAY);
            services.AddKeyedScoped<IPayOrderCloseService, YsePayPayOrderCloseService>(CS.IF_CODE.YSEPAY);
            services.AddKeyedScoped<IPayOrderCloseService, JlPayPayOrderCloseService>(CS.IF_CODE.JLPAY);
            services.AddScoped<IChannelServiceFactory<IPayOrderCloseService>, ChannelServiceFactory<IPayOrderCloseService>>();
            #endregion
            #region QueryService
            services.AddKeyedScoped<IPayOrderQueryService, AliPayPayOrderQueryService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IPayOrderQueryService, WxPayPayOrderQueryService>(CS.IF_CODE.WXPAY);
            services.AddKeyedScoped<IPayOrderQueryService, YsfPayPayOrderQueryService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IPayOrderQueryService, PpPayPayOrderQueryService>(CS.IF_CODE.PPPAY);
            services.AddKeyedScoped<IPayOrderQueryService, SxfPayPayOrderQueryService>(CS.IF_CODE.SXFPAY);
            services.AddKeyedScoped<IPayOrderQueryService, LesPayPayOrderQueryService>(CS.IF_CODE.LESPAY);
            services.AddKeyedScoped<IPayOrderQueryService, HkrtPayPayOrderQueryService>(CS.IF_CODE.HKRTPAY);
            services.AddKeyedScoped<IPayOrderQueryService, UmsPayPayOrderQueryService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IPayOrderQueryService, LcswPayPayOrderQueryService>(CS.IF_CODE.LCSWPAY);
            services.AddKeyedScoped<IPayOrderQueryService, DgPayPayOrderQueryService>(CS.IF_CODE.DGPAY);
            services.AddKeyedScoped<IPayOrderQueryService, LklPayPayOrderQueryService>(CS.IF_CODE.LKLPAY);
            services.AddKeyedScoped<IPayOrderQueryService, YsePayPayOrderQueryService>(CS.IF_CODE.YSEPAY);
            services.AddKeyedScoped<IPayOrderQueryService, AllinPayPayOrderQueryService>(CS.IF_CODE.ALLINPAY);
            services.AddKeyedScoped<IPayOrderQueryService, JlPayPayOrderQueryService>(CS.IF_CODE.JLPAY);
            services.AddScoped<IChannelServiceFactory<IPayOrderQueryService>, ChannelServiceFactory<IPayOrderQueryService>>();
            #endregion
            #region TransferService
            services.AddKeyedScoped<ITransferService, AliPayTransferService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<ITransferService, WxPayTransferService>(CS.IF_CODE.WXPAY);
            services.AddScoped<IChannelServiceFactory<ITransferService>, ChannelServiceFactory<ITransferService>>();
            #endregion
            #region TransferNoticeService
            services.AddKeyedScoped<ITransferNoticeService, AliPayTransferNoticeService>(CS.IF_CODE.ALIPAY);
            services.AddScoped<IChannelServiceFactory<ITransferNoticeService>, ChannelServiceFactory<ITransferNoticeService>>();
            #endregion

            var serviceProvider = services.BuildServiceProvider();
            PayWayUtil.ServiceProvider = serviceProvider;
            AliPayKit.ServiceProvider = serviceProvider;
            WxPayKit.ServiceProvider = serviceProvider;

            ChannelCertConfigKit.ServiceProvider = serviceProvider;
            ChannelCertConfigKit.Initialize();
        }
    }

    public interface IChannelServiceFactory<T>
    {
        T GetService(object serviceKey);
    }

    public class ChannelServiceFactory<T> : IChannelServiceFactory<T>
    {
        private readonly IServiceProvider _serviceProvider;

        public ChannelServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetService(object serviceKey)
        {
            return _serviceProvider.GetRequiredKeyedService<T>(serviceKey);
        }
    }
}

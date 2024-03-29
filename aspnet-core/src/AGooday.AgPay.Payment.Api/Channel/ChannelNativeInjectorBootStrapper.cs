using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Channel.AliPay.Kits;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay;
using AGooday.AgPay.Payment.Api.Channel.LcswPay;
using AGooday.AgPay.Payment.Api.Channel.LesPay;
using AGooday.AgPay.Payment.Api.Channel.PpPay;
using AGooday.AgPay.Payment.Api.Channel.SxfPay;
using AGooday.AgPay.Payment.Api.Channel.UmsPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
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
            services.AddScoped(provider =>
            {
                Func<string, IChannelUserService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IChannelUserService>(ifCode);
                };
                return funcFactory;
            });
            #endregion
            #region DivisionService
            services.AddKeyedScoped<IDivisionService, AliPayDivisionService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IDivisionService, WxPayDivisionService>(CS.IF_CODE.WXPAY);
            services.AddScoped(provider =>
            {
                Func<string, IDivisionService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IDivisionService>(ifCode);
                };
                return funcFactory;
            });
            #endregion
            #region DivisionRecordChannelNotifyService
            services.AddKeyedScoped<AliPayDivisionRecordChannelNotifyService>(CS.IF_CODE.ALIPAY);
            services.AddScoped(provider =>
            {
                Func<string, AbstractDivisionRecordChannelNotifyService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<AbstractDivisionRecordChannelNotifyService>(ifCode);
                };
                return funcFactory;
            });
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
            services.AddScoped(provider =>
            {
                Func<string, IPaymentService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IPaymentService>(ifCode);
                };
                return funcFactory;
            });
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
            services.AddScoped(provider =>
            {
                Func<string, IRefundService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IRefundService>(ifCode);
                };
                return funcFactory;
            });
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
            services.AddScoped(provider =>
            {
                Func<string, IChannelNoticeService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IChannelNoticeService>(ifCode);
                };
                return funcFactory;
            });
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
            services.AddScoped(provider =>
            {
                Func<string, IChannelRefundNoticeService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IChannelRefundNoticeService>(ifCode);
                };
                return funcFactory;
            });
            #endregion
            #region CloseService
            services.AddKeyedScoped<IPayOrderCloseService, AliPayPayOrderCloseService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<IPayOrderCloseService, WxPayPayOrderCloseService>(CS.IF_CODE.WXPAY);
            services.AddKeyedScoped<IPayOrderCloseService, YsfPayPayOrderCloseService>(CS.IF_CODE.YSFPAY);
            services.AddKeyedScoped<IPayOrderCloseService, UmsPayPayOrderCloseService>(CS.IF_CODE.UMSPAY);
            services.AddKeyedScoped<IPayOrderCloseService, LcswPayPayOrderCloseService>(CS.IF_CODE.LCSWPAY);
            services.AddScoped(provider =>
            {
                Func<string, IPayOrderCloseService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IPayOrderCloseService>(ifCode);
                };
                return funcFactory;
            });
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
            services.AddScoped(provider =>
            {
                Func<string, IPayOrderQueryService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<IPayOrderQueryService>(ifCode);
                };
                return funcFactory;
            });
            #endregion
            #region TransferService
            services.AddKeyedScoped<ITransferService, AliPayTransferService>(CS.IF_CODE.ALIPAY);
            services.AddKeyedScoped<ITransferService, WxPayTransferService>(CS.IF_CODE.WXPAY);
            services.AddScoped(provider =>
            {
                Func<string, ITransferService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<ITransferService>(ifCode);
                };
                return funcFactory;
            });
            #endregion
            #region TransferNoticeService
            services.AddKeyedScoped<ITransferNoticeService, AliPayTransferNoticeService>(CS.IF_CODE.ALIPAY);
            services.AddScoped(provider =>
            {
                Func<string, ITransferNoticeService> funcFactory = ifCode =>
                {
                    return provider.GetRequiredKeyedService<ITransferNoticeService>(ifCode);
                };
                return funcFactory;
            });
            #endregion

            var serviceProvider = services.BuildServiceProvider();
            PayWayUtil.ServiceProvider = serviceProvider;
            AliPayKit.ServiceProvider = serviceProvider;
            WxPayKit.ServiceProvider = serviceProvider;
        }
    }
}

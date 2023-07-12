using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Channel.AliPay.Extensions;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay.Extensions;
using AGooday.AgPay.Payment.Api.Channel.LesPay;
using AGooday.AgPay.Payment.Api.Channel.LesPay.Extensions;
using AGooday.AgPay.Payment.Api.Channel.SxfPay;
using AGooday.AgPay.Payment.Api.Channel.SxfPay.Extensions;
using AGooday.AgPay.Payment.Api.Channel.UmsPay;
using AGooday.AgPay.Payment.Api.Channel.UmsPay.Extensions;
using AGooday.AgPay.Payment.Api.Channel.WxPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Extensions;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Channel.YsfPay;
using AGooday.AgPay.Payment.Api.Channel.YsfPay.Extensions;
using AGooday.AgPay.Payment.Api.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.Extensions
{
    public class ChannelNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            #region ChannelUserService
            services.AddSingleton<AliPayChannelUserService>();
            services.AddSingleton<WxPayChannelUserService>();
            services.AddSingleton(provider =>
            {
                Func<string, IChannelUserService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayChannelUserService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayChannelUserService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region DivisionService
            //services.AddSingleton<IDivisionService, AliPayDivisionService>();
            //services.AddSingleton<IDivisionService, WxPayDivisionService>();
            services.AddSingleton<AliPayDivisionService>();
            services.AddSingleton<WxPayDivisionService>();
            services.AddSingleton(provider =>
            {
                Func<string, IDivisionService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayDivisionService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayDivisionService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region PaymentService
            services.AddSingleton<AliPayPaymentService>();
            services.AddSingleton<WxPayPaymentService>();
            services.AddSingleton<YsfPayPaymentService>();
            services.AddSingleton<SxfPayPaymentService>();
            services.AddSingleton<LesPayPaymentService>();
            services.AddSingleton<HkrtPayPaymentService>();
            services.AddSingleton(provider =>
            {
                Func<string, IPaymentService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayPaymentService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayPaymentService>();
                        case CS.IF_CODE.YSFPAY:
                            return provider.GetService<YsfPayPaymentService>();
                        case CS.IF_CODE.SXFPAY:
                            return provider.GetService<SxfPayPaymentService>();
                        case CS.IF_CODE.LESPAY:
                            return provider.GetService<LesPayPaymentService>();
                        case CS.IF_CODE.HKRTPAY:
                            return provider.GetService<HkrtPayPaymentService>();
                        case CS.IF_CODE.UMSPAY:
                            return provider.GetService<UmsPayPaymentService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region RefundService
            services.AddSingleton<AliPayRefundService>();
            services.AddSingleton<WxPayRefundService>();
            services.AddSingleton<YsfPayRefundService>();
            services.AddSingleton<SxfPayRefundService>();
            services.AddSingleton<LesPayRefundService>();
            services.AddSingleton<HkrtPayRefundService>();
            services.AddSingleton(provider =>
            {
                Func<string, IRefundService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayRefundService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayRefundService>();
                        case CS.IF_CODE.YSFPAY:
                            return provider.GetService<YsfPayRefundService>();
                        case CS.IF_CODE.SXFPAY:
                            return provider.GetService<SxfPayRefundService>();
                        case CS.IF_CODE.LESPAY:
                            return provider.GetService<LesPayRefundService>();
                        case CS.IF_CODE.HKRTPAY:
                            return provider.GetService<HkrtPayRefundService>();
                        case CS.IF_CODE.UMSPAY:
                            return provider.GetService<UmsPayRefundService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region ChannelNoticeService
            services.AddSingleton<AliPayChannelNoticeService>();
            services.AddSingleton<WxPayChannelNoticeService>();
            services.AddSingleton<YsfPayChannelNoticeService>();
            services.AddSingleton<SxfPayChannelNoticeService>();
            services.AddSingleton<LesPayChannelNoticeService>();
            services.AddSingleton<HkrtPayChannelNoticeService>();
            services.AddSingleton(provider =>
            {
                Func<string, IChannelNoticeService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayChannelNoticeService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayChannelNoticeService>();
                        case CS.IF_CODE.YSFPAY:
                            return provider.GetService<YsfPayChannelNoticeService>();
                        case CS.IF_CODE.SXFPAY:
                            return provider.GetService<SxfPayChannelNoticeService>();
                        case CS.IF_CODE.LESPAY:
                            return provider.GetService<LesPayChannelNoticeService>();
                        case CS.IF_CODE.HKRTPAY:
                            return provider.GetService<HkrtPayChannelNoticeService>();
                        case CS.IF_CODE.UMSPAY:
                            return provider.GetService<UmsPayChannelNoticeService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region ChannelRefundNoticeService
            //services.AddSingleton<AliPayChannelRefundNoticeService>();
            //services.AddSingleton<WxPayChannelRefundNoticeService>();
            //services.AddSingleton<YsfPayChannelRefundNoticeService>();
            services.AddSingleton<SxfPayChannelNoticeService>();
            services.AddSingleton<LesPayChannelRefundNoticeService>();
            services.AddSingleton<HkrtPayChannelRefundNoticeService>();
            services.AddSingleton(provider =>
            {
                Func<string, IChannelRefundNoticeService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        //case CS.IF_CODE.ALIPAY:
                        //    return provider.GetService<AliPayChannelRefundNoticeService>();
                        //case CS.IF_CODE.WXPAY:
                        //    return provider.GetService<WxPayChannelRefundNoticeService>();
                        //case CS.IF_CODE.YSFPAY:
                        //    return provider.GetService<YsfPayChannelRefundNoticeService>();
                        case CS.IF_CODE.SXFPAY:
                            return provider.GetService<SxfPayChannelRefundNoticeService>();
                        case CS.IF_CODE.LESPAY:
                            return provider.GetService<LesPayChannelRefundNoticeService>();
                        case CS.IF_CODE.HKRTPAY:
                            return provider.GetService<HkrtPayChannelRefundNoticeService>();
                        case CS.IF_CODE.UMSPAY:
                            return provider.GetService<UmsPayChannelRefundNoticeService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region CloseService
            services.AddSingleton<AliPayPayOrderCloseService>();
            services.AddSingleton<WxPayPayOrderCloseService>();
            services.AddSingleton(provider =>
            {
                Func<string, IPayOrderCloseService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayPayOrderCloseService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayPayOrderCloseService>();
                        case CS.IF_CODE.YSFPAY:
                            return provider.GetService<YsfPayPayOrderCloseService>();
                        case CS.IF_CODE.UMSPAY:
                            return provider.GetService<UmsPayPayOrderCloseService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region QueryService
            services.AddSingleton<AliPayPayOrderQueryService>();
            services.AddSingleton<WxPayPayOrderQueryService>();
            services.AddSingleton<YsfPayPayOrderQueryService>();
            services.AddSingleton<SxfPayPayOrderQueryService>();
            services.AddSingleton<LesPayPayOrderQueryService>();
            services.AddSingleton<HkrtPayPayOrderQueryService>();
            services.AddSingleton(provider =>
            {
                Func<string, IPayOrderQueryService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayPayOrderQueryService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayPayOrderQueryService>();
                        case CS.IF_CODE.YSFPAY:
                            return provider.GetService<YsfPayPayOrderQueryService>();
                        case CS.IF_CODE.SXFPAY:
                            return provider.GetService<SxfPayPayOrderQueryService>();
                        case CS.IF_CODE.LESPAY:
                            return provider.GetService<LesPayPayOrderQueryService>();
                        case CS.IF_CODE.HKRTPAY:
                            return provider.GetService<HkrtPayPayOrderQueryService>();
                        case CS.IF_CODE.UMSPAY:
                            return provider.GetService<UmsPayPayOrderQueryService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion

            #region AliPay
            AliPayNativeInjectorBootStrapper.RegisterServices(services);
            #endregion
            #region WxPay
            WxPayNativeInjectorBootStrapper.RegisterServices(services);
            WxPayV3NativeInjectorBootStrapper.RegisterServices(services);
            #endregion
            #region YsfPay
            YsfPayNativeInjectorBootStrapper.RegisterServices(services);
            #endregion
            #region SxfPay
            SxfPayNativeInjectorBootStrapper.RegisterServices(services);
            #endregion
            #region LesPay
            LesPayNativeInjectorBootStrapper.RegisterServices(services);
            #endregion
            #region HkrtPay
            HkrtPayNativeInjectorBootStrapper.RegisterServices(services);
            #endregion
            #region UmsPay
            UmsPayNativeInjectorBootStrapper.RegisterServices(services);
            #endregion

            var serviceProvider = services.BuildServiceProvider();
            PayWayUtil.ServiceProvider = serviceProvider;
            AliPayKit.ServiceProvider = serviceProvider;
            WxPayKit.ServiceProvider = serviceProvider;
        }
    }
}

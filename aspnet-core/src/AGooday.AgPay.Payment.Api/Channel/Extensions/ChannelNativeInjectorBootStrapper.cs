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
            services.AddScoped<AliPayChannelUserService>();
            services.AddScoped<WxPayChannelUserService>();
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
            services.AddScoped<AliPayDivisionService>();
            services.AddScoped<WxPayDivisionService>();
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
            #region DivisionRecordChannelNotifyService
            services.AddScoped<AliPayDivisionRecordChannelNotifyService>();
            services.AddSingleton(provider =>
            {
                Func<string, AbstractDivisionRecordChannelNotifyService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayDivisionRecordChannelNotifyService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region PaymentService
            services.AddScoped<AliPayPaymentService>();
            services.AddScoped<WxPayPaymentService>();
            services.AddScoped<YsfPayPaymentService>();
            services.AddScoped<SxfPayPaymentService>();
            services.AddScoped<LesPayPaymentService>();
            services.AddScoped<HkrtPayPaymentService>();
            services.AddScoped<UmsPayPaymentService>();
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
            services.AddScoped<AliPayRefundService>();
            services.AddScoped<WxPayRefundService>();
            services.AddScoped<YsfPayRefundService>();
            services.AddScoped<SxfPayRefundService>();
            services.AddScoped<LesPayRefundService>();
            services.AddScoped<HkrtPayRefundService>();
            services.AddScoped<UmsPayRefundService>();
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
            services.AddScoped<AliPayChannelNoticeService>();
            services.AddScoped<WxPayChannelNoticeService>();
            services.AddScoped<YsfPayChannelNoticeService>();
            services.AddScoped<SxfPayChannelNoticeService>();
            services.AddScoped<LesPayChannelNoticeService>();
            services.AddScoped<HkrtPayChannelNoticeService>();
            services.AddScoped<UmsPayChannelNoticeService>();
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
            services.AddSingleton<WxPayChannelRefundNoticeService>();
            //services.AddSingleton<YsfPayChannelRefundNoticeService>();
            services.AddScoped<SxfPayChannelNoticeService>();
            services.AddScoped<LesPayChannelRefundNoticeService>();
            services.AddScoped<HkrtPayChannelRefundNoticeService>();
            services.AddScoped<UmsPayChannelRefundNoticeService>();
            services.AddSingleton(provider =>
            {
                Func<string, IChannelRefundNoticeService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        //case CS.IF_CODE.ALIPAY:
                        //    return provider.GetService<AliPayChannelRefundNoticeService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayChannelRefundNoticeService>();
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
            services.AddScoped<AliPayPayOrderCloseService>();
            services.AddScoped<WxPayPayOrderCloseService>();
            services.AddScoped<YsfPayPayOrderCloseService>();
            services.AddScoped<UmsPayPayOrderCloseService>();
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
            services.AddScoped<AliPayPayOrderQueryService>();
            services.AddScoped<WxPayPayOrderQueryService>();
            services.AddScoped<YsfPayPayOrderQueryService>();
            services.AddScoped<SxfPayPayOrderQueryService>();
            services.AddScoped<LesPayPayOrderQueryService>();
            services.AddScoped<HkrtPayPayOrderQueryService>();
            services.AddScoped<UmsPayPayOrderQueryService>();
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
            #region TransferService
            services.AddScoped<AliPayTransferService>();
            services.AddScoped<WxPayTransferService>();
            services.AddSingleton(provider =>
            {
                Func<string, ITransferService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayTransferService>();
                        case CS.IF_CODE.WXPAY:
                            return provider.GetService<WxPayTransferService>();
                        default:
                            return null;
                    }
                };
                return funcFactory;
            });
            #endregion
            #region TransferNoticeService
            services.AddScoped<AliPayTransferNoticeService>();
            services.AddSingleton(provider =>
            {
                Func<string, ITransferNoticeService> funcFactory = ifCode =>
                {
                    switch (ifCode)
                    {
                        case CS.IF_CODE.ALIPAY:
                            return provider.GetService<AliPayTransferNoticeService>();
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

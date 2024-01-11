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
            services.AddScoped<AliPayChannelUserService>();
            services.AddScoped<WxPayChannelUserService>();
            services.AddScoped(provider =>
            {
                Func<string, IChannelUserService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayChannelUserService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayChannelUserService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region DivisionService
            //services.AddScoped<IDivisionService, AliPayDivisionService>();
            //services.AddScoped<IDivisionService, WxPayDivisionService>();
            services.AddScoped<AliPayDivisionService>();
            services.AddScoped<WxPayDivisionService>();
            services.AddScoped(provider =>
            {
                Func<string, IDivisionService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayDivisionService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayDivisionService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region DivisionRecordChannelNotifyService
            services.AddScoped<AliPayDivisionRecordChannelNotifyService>();
            services.AddScoped(provider =>
            {
                Func<string, AbstractDivisionRecordChannelNotifyService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayDivisionRecordChannelNotifyService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region PaymentService
            services.AddScoped<AliPayPaymentService>();
            services.AddScoped<WxPayPaymentService>();
            services.AddScoped<YsfPayPaymentService>();
            services.AddScoped<PpPayPaymentService>();
            services.AddScoped<SxfPayPaymentService>();
            services.AddScoped<LesPayPaymentService>();
            services.AddScoped<HkrtPayPaymentService>();
            services.AddScoped<UmsPayPaymentService>();
            services.AddScoped<LcswPayPaymentService>();
            services.AddScoped(provider =>
            {
                Func<string, IPaymentService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayPaymentService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayPaymentService>(),
                        CS.IF_CODE.YSFPAY => provider.GetService<YsfPayPaymentService>(),
                        CS.IF_CODE.PPPAY => provider.GetService<PpPayPaymentService>(),
                        CS.IF_CODE.SXFPAY => provider.GetService<SxfPayPaymentService>(),
                        CS.IF_CODE.LESPAY => provider.GetService<LesPayPaymentService>(),
                        CS.IF_CODE.HKRTPAY => provider.GetService<HkrtPayPaymentService>(),
                        CS.IF_CODE.UMSPAY => provider.GetService<UmsPayPaymentService>(),
                        CS.IF_CODE.LCSWPAY => provider.GetService<LcswPayPaymentService>(),
                        _ => null,
                    };
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
            services.AddScoped<AliPayRefundService>();
            services.AddScoped<WxPayRefundService>();
            services.AddScoped<YsfPayRefundService>();
            services.AddScoped<PpPayRefundService>();
            services.AddScoped<SxfPayRefundService>();
            services.AddScoped<LesPayRefundService>();
            services.AddScoped<HkrtPayRefundService>();
            services.AddScoped<UmsPayRefundService>();
            services.AddScoped<LcswPayRefundService>();
            services.AddScoped(provider =>
            {
                Func<string, IRefundService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayRefundService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayRefundService>(),
                        CS.IF_CODE.YSFPAY => provider.GetService<YsfPayRefundService>(),
                        CS.IF_CODE.PPPAY => provider.GetService<PpPayRefundService>(),
                        CS.IF_CODE.SXFPAY => provider.GetService<SxfPayRefundService>(),
                        CS.IF_CODE.LESPAY => provider.GetService<LesPayRefundService>(),
                        CS.IF_CODE.HKRTPAY => provider.GetService<HkrtPayRefundService>(),
                        CS.IF_CODE.UMSPAY => provider.GetService<UmsPayRefundService>(),
                        CS.IF_CODE.LCSWPAY => provider.GetService<LcswPayRefundService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region ChannelNoticeService
            services.AddScoped<AliPayChannelNoticeService>();
            services.AddScoped<WxPayChannelNoticeService>();
            services.AddScoped<YsfPayChannelNoticeService>();
            services.AddScoped<PpPayChannelNoticeService>();
            services.AddScoped<SxfPayChannelNoticeService>();
            services.AddScoped<LesPayChannelNoticeService>();
            services.AddScoped<HkrtPayChannelNoticeService>();
            services.AddScoped<UmsPayChannelNoticeService>();
            services.AddScoped<LcswPayChannelNoticeService>();
            services.AddScoped(provider =>
            {
                Func<string, IChannelNoticeService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayChannelNoticeService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayChannelNoticeService>(),
                        CS.IF_CODE.YSFPAY => provider.GetService<YsfPayChannelNoticeService>(),
                        CS.IF_CODE.PPPAY => provider.GetService<PpPayChannelNoticeService>(),
                        CS.IF_CODE.SXFPAY => provider.GetService<SxfPayChannelNoticeService>(),
                        CS.IF_CODE.LESPAY => provider.GetService<LesPayChannelNoticeService>(),
                        CS.IF_CODE.HKRTPAY => provider.GetService<HkrtPayChannelNoticeService>(),
                        CS.IF_CODE.UMSPAY => provider.GetService<UmsPayChannelNoticeService>(),
                        CS.IF_CODE.LCSWPAY => provider.GetService<LcswPayChannelNoticeService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region ChannelRefundNoticeService
            //services.AddScoped<AliPayChannelRefundNoticeService>();
            services.AddScoped<WxPayChannelRefundNoticeService>();
            //services.AddScoped<YsfPayChannelRefundNoticeService>();
            services.AddScoped<PpPayChannelNoticeService>();
            services.AddScoped<SxfPayChannelNoticeService>();
            services.AddScoped<LesPayChannelRefundNoticeService>();
            services.AddScoped<HkrtPayChannelRefundNoticeService>();
            services.AddScoped<UmsPayChannelRefundNoticeService>();
            services.AddScoped(provider =>
            {
                Func<string, IChannelRefundNoticeService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        //CS.IF_CODE.ALIPAY => provider.GetService<AliPayChannelRefundNoticeService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayChannelRefundNoticeService>(),
                        //CS.IF_CODE.YSFPAY => provider.GetService<YsfPayChannelRefundNoticeService>(),
                        CS.IF_CODE.PPPAY => provider.GetService<PpPayChannelRefundNoticeService>(),
                        CS.IF_CODE.SXFPAY => provider.GetService<SxfPayChannelRefundNoticeService>(),
                        CS.IF_CODE.LESPAY => provider.GetService<LesPayChannelRefundNoticeService>(),
                        CS.IF_CODE.HKRTPAY => provider.GetService<HkrtPayChannelRefundNoticeService>(),
                        CS.IF_CODE.UMSPAY => provider.GetService<UmsPayChannelRefundNoticeService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region CloseService
            services.AddScoped<AliPayPayOrderCloseService>();
            services.AddScoped<WxPayPayOrderCloseService>();
            services.AddScoped<YsfPayPayOrderCloseService>();
            services.AddScoped<UmsPayPayOrderCloseService>();
            services.AddScoped<LcswPayPayOrderCloseService>();
            services.AddScoped(provider =>
            {
                Func<string, IPayOrderCloseService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayPayOrderCloseService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayPayOrderCloseService>(),
                        CS.IF_CODE.YSFPAY => provider.GetService<YsfPayPayOrderCloseService>(),
                        CS.IF_CODE.UMSPAY => provider.GetService<UmsPayPayOrderCloseService>(),
                        CS.IF_CODE.LCSWPAY => provider.GetService<LcswPayPayOrderCloseService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region QueryService
            services.AddScoped<AliPayPayOrderQueryService>();
            services.AddScoped<WxPayPayOrderQueryService>();
            services.AddScoped<YsfPayPayOrderQueryService>();
            services.AddScoped<PpPayPayOrderQueryService>();
            services.AddScoped<SxfPayPayOrderQueryService>();
            services.AddScoped<LesPayPayOrderQueryService>();
            services.AddScoped<HkrtPayPayOrderQueryService>();
            services.AddScoped<UmsPayPayOrderQueryService>();
            services.AddScoped<LcswPayPayOrderQueryService>();
            services.AddScoped(provider =>
            {
                Func<string, IPayOrderQueryService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayPayOrderQueryService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayPayOrderQueryService>(),
                        CS.IF_CODE.YSFPAY => provider.GetService<YsfPayPayOrderQueryService>(),
                        CS.IF_CODE.PPPAY => provider.GetService<PpPayPayOrderQueryService>(),
                        CS.IF_CODE.SXFPAY => provider.GetService<SxfPayPayOrderQueryService>(),
                        CS.IF_CODE.LESPAY => provider.GetService<LesPayPayOrderQueryService>(),
                        CS.IF_CODE.HKRTPAY => provider.GetService<HkrtPayPayOrderQueryService>(),
                        CS.IF_CODE.UMSPAY => provider.GetService<UmsPayPayOrderQueryService>(),
                        CS.IF_CODE.LCSWPAY => provider.GetService<LcswPayPayOrderQueryService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region TransferService
            services.AddScoped<AliPayTransferService>();
            services.AddScoped<WxPayTransferService>();
            services.AddScoped(provider =>
            {
                Func<string, ITransferService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayTransferService>(),
                        CS.IF_CODE.WXPAY => provider.GetService<WxPayTransferService>(),
                        _ => null,
                    };
                };
                return funcFactory;
            });
            #endregion
            #region TransferNoticeService
            services.AddScoped<AliPayTransferNoticeService>();
            services.AddScoped(provider =>
            {
                Func<string, ITransferNoticeService> funcFactory = ifCode =>
                {
                    return ifCode switch
                    {
                        CS.IF_CODE.ALIPAY => provider.GetService<AliPayTransferNoticeService>(),
                        _ => null,
                    };
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

using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay;

namespace AGooday.AgPay.Payment.Api.Extensions
{
    public class FuncFactory
    {
        public static IChannelUserService ChannelUserServiceFactory(IServiceProvider provider, string ifCode)
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
        }
    }
}

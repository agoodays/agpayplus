using AGooday.AgPay.Notice.Core;

namespace AGooday.AgPay.Notice.Sms
{
    public interface ISmsFactoryProvider
    {
        ISmsProvider GetProvider();
    }
}

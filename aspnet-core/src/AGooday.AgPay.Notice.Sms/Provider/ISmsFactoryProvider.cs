namespace AGooday.AgPay.Notice.Sms
{
    public interface ISmsFactoryProvider
    {
        ISmsProvider GetProvider();
    }
}

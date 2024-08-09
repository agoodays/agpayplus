namespace AGooday.AgPay.Payment.Api.Services
{
    public interface IQRCodeService
    {
        byte[] GetQRCode(string plainText);
    }
}

namespace AGooday.AgPay.Components.Third.Services
{
    public interface IQRCodeService
    {
        byte[] GetQRCode(string plainText);
    }
}

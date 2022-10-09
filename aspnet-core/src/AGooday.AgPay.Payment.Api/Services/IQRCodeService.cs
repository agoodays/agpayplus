using System.Drawing;

namespace AGooday.AgPay.Payment.Api.Services
{
    public interface IQRCodeService
    {
        Bitmap GetQRCode(string url, int pixel);
    }
}

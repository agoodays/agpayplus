using AGooday.AgPay.Common.Utils;
using System.Drawing;

namespace AGooday.AgPay.Payment.Api.Services
{
    public class QRCodeService : IQRCodeService
    {
        #region  QRCode

        public Bitmap GetQRCode(string plainText, int pixel)
        {
            var bitmap = QrCodeBuilder.Generate(plainText, pixel);

            return bitmap;
        }
        #endregion
    }
}

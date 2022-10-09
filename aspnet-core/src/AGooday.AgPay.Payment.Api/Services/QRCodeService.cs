using QRCoder;
using System.Drawing;

namespace AGooday.AgPay.Payment.Api.Services
{
    public class QRCodeService : IQRCodeService
    {
        #region  QRCode

        public Bitmap GetQRCode(string plainText, int pixel)
        {
            var generator = new QRCodeGenerator();
            var qrCodeData = generator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.L);
            var qrCode = new QRCode(qrCodeData);

            var bitmap = qrCode.GetGraphic(pixel);
            //var bitmap = qrCode.GetGraphic(pixel, Color.Black, Color.White, null, 15, 6, false);

            return bitmap;
        }
        #endregion
    }
}

﻿using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.Services
{
    public class QRCodeService : IQRCodeService
    {
        #region  QRCode

        public byte[] GetQRCode(string plainText)
        {
            var bitmap = QrCodeBuilder.Generate(plainText);

            return bitmap;
        }
        #endregion
    }
}

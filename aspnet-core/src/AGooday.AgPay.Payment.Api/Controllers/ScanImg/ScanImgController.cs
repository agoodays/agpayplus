using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;

namespace AGooday.AgPay.Payment.Api.Controllers.ScanImg
{
    /// <summary>
    /// 扫描图片生成器
    /// </summary>
    [Route("api/scan")]
    [ApiController]
    public class ScanImgController : ControllerBase
    {
        private readonly IQRCodeService _qrCode;

        public ScanImgController(IQRCodeService qrCode)
        {
            _qrCode = qrCode;
        }

        [HttpGet, Route("imgs/{aesStr}.png")]
        public IActionResult QrImgs(string aesStr, int pixel = 5)
        {
            string plainText = aesStr;
            try
            {
                plainText = AgPayUtil.AesDecode(aesStr);
            }
            catch { }
            if (string.IsNullOrEmpty(plainText))
            {
                return BadRequest("parameter is null");
            }
            if (pixel <= 0)
            {
                return BadRequest("pixel <= 0");
            }

            var bitmap = _qrCode.GetQRCode(plainText, pixel);
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);

            return File(ms.GetBuffer(), "image/png");
        }
    }
}

using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult QrImgs(string aesStr)
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

            var buffer = _qrCode.GetQRCode(plainText);
            return File(buffer, "image/png");
        }
    }
}

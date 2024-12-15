using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.OCR.Extensions;
using AGooday.AgPay.Components.OCR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AGooday.AgPay.Components.OCR.Controllers
{
    [ApiController, Authorize]
    [Route("api/ocr")]
    public class OcrController : ControllerBase
    {
        private readonly ILogger<OcrController> _logger;
        private readonly IOcrService _ocrService;

        public OcrController(ILogger<OcrController> logger, IOcrServiceFactory ocrServiceFactory)
        {
            _logger = logger;
            _ocrService = ocrServiceFactory.GetService();
        }

        /// <summary>
        /// 通用文字识别
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("recognizeText/{type}")]
        public async Task<ApiRes> RecognizeTextAsync(string imageUrl, string type)
        {
            try
            {
                var text = await _ocrService.RecognizeTextAsync(imageUrl, type);
                return ApiRes.Ok(text);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"通用文字识别异常, imageUrl = {imageUrl}");
                throw new BizException(ApiCode.SYSTEM_ERROR, e.Message);
            }
        }

        /// <summary>
        /// 卡证文字识别
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("{type}")]
        public async Task<ApiRes> RecognizeCardTextAsync(string imageUrl, string type)
        {
            try
            {
                var text = await _ocrService.RecognizeCardTextAsync(imageUrl, type);
                return ApiRes.Ok(text);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"卡证文字识别异常, imageUrl = {imageUrl}");
                throw new BizException(ApiCode.SYSTEM_ERROR, e.Message);
            }
        }
    }
}

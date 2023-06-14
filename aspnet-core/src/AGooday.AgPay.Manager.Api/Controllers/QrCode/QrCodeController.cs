using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [ApiController, Authorize]
    [Route("api/qrc")]
    public class QrCodeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<QrCodeController> _logger;
        private readonly IQrCodeService _qrCodeService;
        private readonly IQrCodeShellService _qrCodeShellService;

        public QrCodeController(IWebHostEnvironment env, ILogger<QrCodeController> logger, IQrCodeService qrCodeService, IQrCodeShellService qrCodeShellService)
        {
            _env = env;
            _logger = logger;
            _qrCodeService = qrCodeService;
            _qrCodeShellService = qrCodeShellService;
        }

        /// <summary>
        /// 码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_LIST)]
        public ApiRes List([FromQuery] QrCodeQueryDto dto)
        {
            var data = _qrCodeService.GetPaginatedData<QrCodeDto>(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新增码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增码牌")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_ADD)]
        public ApiRes Add(QrCodeDto dto)
        {
            bool result = _qrCodeService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除码牌
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除码牌")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_DEL)]
        public ApiRes Delete(string recordId)
        {
            bool result = _qrCodeService.Remove(recordId);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_DELETE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新码牌")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_EDIT)]
        public ApiRes Update(QrCodeDto dto)
        {
            bool result = _qrCodeService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看码牌
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_VIEW, PermCode.MGR.ENT_DEVICE_QRC_EDIT)]
        public ApiRes Detail(string recordId)
        {
            var qrCode = _qrCodeService.GetById(recordId);
            if (qrCode == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(qrCode);
        }

        [HttpGet, Route("view/{recordId}")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_VIEW, PermCode.MGR.ENT_DEVICE_QRC_EDIT)]
        public ApiRes View(string recordId)
        {
            var qrCode = _qrCodeService.GetById(recordId);
            Bitmap bitmap = null;
            if (qrCode.QrcShellId.HasValue)
            {
                var qrCodeShell = _qrCodeShellService.GetById(qrCode.QrcShellId.Value);
                bitmap = GetBitmap(qrCodeShell, qrCode.QrUrl, qrCode.QrcId);
            }
            else
            {
                bitmap = QrCodeBuilder.Generate(qrCode.QrUrl);
            }
            var imageBase64Data = bitmap == null ? "" : $"data:image/png;base64,{DrawQrCode.BitmapToBase64Str(bitmap)}";
            return ApiRes.Ok(imageBase64Data);
        }

        private Bitmap GetBitmap(QrCodeShellDto dto, string content, string text)
        {
            var configInfo = JsonConvert.DeserializeObject<QrCodeConfigInfo>(dto.ConfigInfo.ToString());
            var logoPath = configInfo.LogoImgUrl;

            var backgroundColor = configInfo.BgColor == "custom" ? configInfo.CustomBgColor : configInfo.BgColor;
            foreach (var item in configInfo.PayTypeList)
            {
                item.ImgUrl = string.IsNullOrWhiteSpace(item.ImgUrl) && item.Name != "custom" ? Path.Combine(_env.WebRootPath, "images", $"{item.Name}.png") : item.ImgUrl;
            }
            text = configInfo.ShowIdFlag ? text : string.Empty;
            var iconPath = configInfo.QrInnerImgUrl;
            Bitmap bitmap = null;
            switch (dto.StyleCode)
            {
                case CS.STYLE_CODE.A:
                    bitmap = DrawQrCode.GenerateStyleAImage(backgroundColor: backgroundColor, title: "", content: content, logoPath: logoPath, iconPath: iconPath, text: text, payTypes: configInfo.PayTypeList);
                    break;
                case CS.STYLE_CODE.B:
                    bitmap = DrawQrCode.GenerateStyleBImage(backgroundColor: backgroundColor, title: "", content: content, logoPath: logoPath, iconPath: iconPath, text: text, payTypes: configInfo.PayTypeList);
                    break;
                default:
                    break;
            }
            return bitmap;
        }
    }
}

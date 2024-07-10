﻿using AGooday.AgPay.Application;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [ApiController, Authorize]
    [Route("api/qrc")]
    public class QrCodeController : CommonController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IQrCodeService _qrCodeService;
        private readonly IQrCodeShellService _qrCodeShellService;
        private readonly ISysConfigService _sysConfigService;

        public QrCodeController(ILogger<QrCodeController> logger,
            IWebHostEnvironment env,
            IQrCodeService qrCodeService,
            IQrCodeShellService qrCodeShellService,
            RedisUtil client,
            IAuthService authService,
            ISysConfigService sysConfigService)
            : base(logger, client, authService)
        {
            _env = env;
            _qrCodeService = qrCodeService;
            _qrCodeShellService = qrCodeShellService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_LIST)]
        public ApiPageRes<QrCodeDto> List([FromQuery] QrCodeQueryDto dto)
        {
            var data = _qrCodeService.GetPaginatedData(dto);
            return ApiPageRes<QrCodeDto>.Pages(data);
        }

        /// <summary>
        /// 码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("batchIdDistinctCount"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_LIST)]
        public ApiRes BatchIdDistinctCount()
        {
            string data = _qrCodeService.BatchIdDistinctCount();
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 新增码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增码牌")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_ADD)]
        public ApiRes Add(QrCodeAddDto dto)
        {
            dto.SysType = string.IsNullOrWhiteSpace(dto.SysType) ? CS.SYS_TYPE.MGR : dto.SysType;
            dto.BelongInfoId = CS.BASE_BELONG_INFO_ID.MGR;
            bool result = _qrCodeService.BatchAdd(dto);
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
        public ApiRes Update(string recordId, QrCodeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.QrcId)) // 状态变更
            {
                var sysUser = _qrCodeService.GetByIdAsNoTracking(recordId);
                sysUser.State = dto.State.Value;
                CopyUtil.CopyProperties(sysUser, dto);
            }
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
            byte[] inArray;
            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            var content = $"{dbApplicationConfig.PaySiteUrl}{qrCode.QrUrl}";
            if (qrCode.QrcShellId.HasValue)
            {
                var qrCodeShell = _qrCodeShellService.GetById(qrCode.QrcShellId.Value);
                inArray = GetBitmap(qrCodeShell, content, $"No.{qrCode.QrcId}");
            }
            else
            {
                inArray = QrCodeBuilder.Generate(content);
            }
            var imageBase64Data = inArray == null ? "" : DrawQrCode.BitmapToImageBase64String(inArray);
            return ApiRes.Ok(imageBase64Data);
        }

        private byte[] GetBitmap(QrCodeShellDto dto, string content, string text)
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
            switch (dto.StyleCode)
            {
                case CS.STYLE_CODE.A:
                    return DrawQrCode.GenerateStyleAImage(backgroundColor: backgroundColor, title: "", content: content, logoPath: logoPath, iconPath: iconPath, text: text, payTypes: configInfo.PayTypeList);
                case CS.STYLE_CODE.B:
                    return DrawQrCode.GenerateStyleBImage(backgroundColor: backgroundColor, title: "", content: content, logoPath: logoPath, iconPath: iconPath, text: text, payTypes: configInfo.PayTypeList);
                default:
                    break;
            }
            return null;
        }
    }
}

using AGooday.AgPay.Application.Config;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
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
        private readonly IMchInfoService _mchInfoService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchStoreService _mchStoreService;

        public QrCodeController(ILogger<QrCodeController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IWebHostEnvironment env,
            IQrCodeService qrCodeService,
            IQrCodeShellService qrCodeShellService,
            ISysConfigService sysConfigService,
            IMchInfoService mchInfoService,
            IMchAppService mchAppService,
            IMchStoreService mchStoreService)
            : base(logger, cacheService, authService)
        {
            _env = env;
            _qrCodeService = qrCodeService;
            _qrCodeShellService = qrCodeShellService;
            _sysConfigService = sysConfigService;
            _mchInfoService = mchInfoService;
            _mchAppService = mchAppService;
            _mchStoreService = mchStoreService;
        }

        /// <summary>
        /// 码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_LIST)]
        public async Task<ApiPageRes<QrCodeDto>> ListAsync([FromQuery] QrCodeQueryDto dto)
        {
            var data = await _qrCodeService.GetPaginatedDataAsync(dto);
            var mchNos = data.Items.Select(s => s.MchNo).Distinct().ToList();
            var appIds = data.Items.Select(s => s.AppId).Distinct().ToList();
            var storeIds = data.Items.Select(s => s.StoreId).Distinct().ToList();
            var mchInfos = await _mchInfoService.GetByMchNosAsNoTrackingAsync(mchNos);
            var mchApps = await _mchAppService.GetByAppIdsAsNoTrackingAsync(appIds);
            var mchStores = await _mchStoreService.GetByStoreIdsAsNoTrackingAsync(storeIds);
            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            foreach (var item in data.Items)
            {
                item.AddExt("mchName", mchInfos?.FirstOrDefault(s => s.MchNo == item.MchNo)?.MchName);
                item.AddExt("appName", mchApps?.FirstOrDefault(s => s.AppId == item.AppId)?.AppName);
                item.AddExt("storeName", mchStores?.FirstOrDefault(s => s.StoreId == item.StoreId)?.StoreName);
                item.AddExt("payHostUrl", dbApplicationConfig.PaySiteUrl);
            }
            return ApiPageRes<QrCodeDto>.Pages(data);
        }

        /// <summary>
        /// 码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("batchIdDistinctCount"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_LIST)]
        public async Task<ApiRes> BatchIdDistinctCountAsync()
        {
            string data = await _qrCodeService.BatchIdDistinctCountAsync();
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
        public async Task<ApiRes> AddAsync(QrCodeAddDto dto)
        {
            dto.SysType = string.IsNullOrWhiteSpace(dto.SysType) ? CS.SYS_TYPE.MGR : dto.SysType;
            dto.BelongInfoId = CS.BASE_BELONG_INFO_ID.MGR;
            bool result = await _qrCodeService.BatchAddAsync(dto);
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
        public async Task<ApiRes> DeleteAsync(string recordId)
        {
            bool result = await _qrCodeService.RemoveAsync(recordId);
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
        public async Task<ApiRes> UpdateAsync(string recordId, QrCodeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.QrcId)) // 状态变更
            {
                var entity = await _qrCodeService.GetByIdAsNoTrackingAsync(recordId);
                entity.State = dto.State.Value;
                CopyUtil.CopyProperties(entity, dto);
            }
            bool result = await _qrCodeService.UpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 绑定码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("bind/{recordId}"), MethodLog("绑定码牌")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_EDIT)]
        public async Task<ApiRes> BindAsync(string recordId, QrCodeDto dto)
        {
            dto.BindState = CS.YES;
            bool result = await _qrCodeService.UpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 解绑码牌
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("unbind/{recordId}"), MethodLog("解绑码牌")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_EDIT)]
        public async Task<ApiRes> UnBindAsync(string recordId)
        {
            bool result = await _qrCodeService.UnBindAsync(recordId);
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
        public async Task<ApiRes> DetailAsync(string recordId)
        {
            var qrCode = await _qrCodeService.GetByIdAsNoTrackingAsync(recordId);
            if (qrCode == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(qrCode);
        }

        [HttpGet, Route("view/{recordId}")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_VIEW, PermCode.MGR.ENT_DEVICE_QRC_EDIT)]
        public async Task<ApiRes> ViewAsync(string recordId)
        {
            var qrCode = await _qrCodeService.GetByIdAsync(recordId);
            byte[] inArray;
            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            string payUrl = dbApplicationConfig.GenUniJsapiPayUrl(CS.GenTokenData(CS.TOKEN_DATA_TYPE.QRC_ID, qrCode.QrcId));
            if (qrCode.QrcShellId.HasValue)
            {
                var qrCodeShell = await _qrCodeShellService.GetByIdAsync(qrCode.QrcShellId.Value);
                inArray = GetBitmap(qrCodeShell, payUrl, $"No.{qrCode.QrcId}");
            }
            else
            {
                //inArray = QrCodeBuilder.Generate(payUrl);
                inArray = DrawQrCode.GenerateNoStyleImage(content: payUrl, text: $"No.{qrCode.QrcId}");
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

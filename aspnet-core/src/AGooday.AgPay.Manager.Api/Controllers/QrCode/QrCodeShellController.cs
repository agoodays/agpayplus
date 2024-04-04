using AGooday.AgPay.Application;
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
    [ApiController]
    [Route("api/qrc/shell")]
    public class QrCodeShellController : CommonController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IQrCodeShellService _qrCodeShellService;
        protected readonly ISysConfigService _sysConfigService;

        public QrCodeShellController(ILogger<QrCodeController> logger,
            IWebHostEnvironment env,
            IQrCodeShellService qrCodeShellService,
            ISysConfigService sysConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _env = env;
            _qrCodeShellService = qrCodeShellService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 码牌模板
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_LIST)]
        public ApiPageRes<QrCodeShellDto> List([FromQuery] QrCodeShellQueryDto dto)
        {
            var data = _qrCodeShellService.GetPaginatedData(dto);
            return ApiPageRes<QrCodeShellDto>.Pages(data);
        }

        /// <summary>
        /// 新增码牌模板
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增码牌模板")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_ADD)]
        public ApiRes Add(QrCodeShellDto dto)
        {
            dto.SysType = string.IsNullOrWhiteSpace(dto.SysType) ? CS.SYS_TYPE.MGR : dto.SysType;
            dto.BelongInfoId = CS.BASE_BELONG_INFO_ID.MGR;
            bool result = _qrCodeShellService.Add(dto);
            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            dto.ShellImgViewUrl = dbApplicationConfig.GenShellImgViewUrl(dto.Id.ToString());
            _qrCodeShellService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除码牌模板
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除码牌模板")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_DEL)]
        public ApiRes Delete(long recordId)
        {
            bool result = _qrCodeShellService.Remove(recordId);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_DELETE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新码牌模板
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新码牌模板")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_EDIT)]
        public ApiRes Update(long recordId, QrCodeShellDto dto)
        {
            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            dto.ShellImgViewUrl = dbApplicationConfig.GenShellImgViewUrl(recordId.ToString());
            bool result = _qrCodeShellService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看码牌模板
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_VIEW, PermCode.MGR.ENT_DEVICE_QRC_SHELL_EDIT)]
        public ApiRes Detail(long recordId)
        {
            var qrCodeShell = _qrCodeShellService.GetById(recordId);
            if (qrCodeShell == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(qrCodeShell);
        }

        [HttpPost, Route("view")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_VIEW, PermCode.MGR.ENT_DEVICE_QRC_SHELL_EDIT)]
        public ApiRes View(QrCodeShellDto dto)
        {
            var inArray = GetQrCodeShellImage(dto);
            var imageBase64Data = inArray == null ? "" : DrawQrCode.BitmapToImageBase64String(inArray);
            return ApiRes.Ok(imageBase64Data);
        }

        private byte[] GetQrCodeShellImage(QrCodeShellDto dto)
        {
            var configInfo = JsonConvert.DeserializeObject<QrCodeConfigInfo>(dto.ConfigInfo.ToString());
            var logoPath = configInfo.LogoImgUrl;

            var backgroundColor = configInfo.BgColor == "custom" ? configInfo.CustomBgColor : configInfo.BgColor;
            foreach (var item in configInfo.PayTypeList)
            {
                item.ImgUrl = string.IsNullOrWhiteSpace(item.ImgUrl) && item.Name != "custom" ? Path.Combine(_env.WebRootPath, "images", $"{item.Name}.png") : item.ImgUrl;
            }
            string text = configInfo.ShowIdFlag ? "No.220101000001" : string.Empty;
            var iconPath = configInfo.QrInnerImgUrl;
            byte[] inArray = null;
            switch (dto.StyleCode)
            {
                case CS.STYLE_CODE.A:
                    inArray = DrawQrCode.GenerateStyleAImage(backgroundColor: backgroundColor, title: "", logoPath: logoPath, iconPath: iconPath, text: text, payTypes: configInfo.PayTypeList);
                    break;
                case CS.STYLE_CODE.B:
                    inArray = DrawQrCode.GenerateStyleBImage(backgroundColor: backgroundColor, title: "", logoPath: logoPath, iconPath: iconPath, text: text, payTypes: configInfo.PayTypeList);
                    break;
                default:
                    break;
            }
            return inArray;
        }

        [HttpGet, Route("view/{recordId}")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_VIEW, PermCode.MGR.ENT_DEVICE_QRC_SHELL_EDIT)]
        public ApiRes View(long recordId)
        {
            var qrCodeShell = _qrCodeShellService.GetById(recordId);
            return View(qrCodeShell);
        }

        [HttpGet, AllowAnonymous, Route("imgview/{key}.png")]
        public ActionResult ImgView(string key)
        {
            var qrCodeShell = _qrCodeShellService.GetById(Convert.ToInt64(AgPayUtil.AesDecode(key)));
            if (qrCodeShell != null)
            {
                var buffer = GetQrCodeShellImage(qrCodeShell);
                using (var stream = new MemoryStream(buffer))
                {
                    // 返回生成的码牌图片
                    return File(stream.ToArray(), "image/png");
                }
            }
            return null;
        }

        [HttpGet, AllowAnonymous, Route("nostyle.png")]
        public IActionResult NoStyle()
        {
            // 创建二维码对象
            var buffer = QrCodeBuilder.Generate(iconPath: Path.Combine(_env.WebRootPath, "images", "avatar.png"));

            // 将位图对象转换为 PNG 格式并输出到响应流
            using (var stream = new MemoryStream(buffer))
            {
                // 返回生成的码牌图片
                return File(stream.ToArray(), "image/png");
            }
        }

        [HttpGet, AllowAnonymous, Route("stylea.png")]
        public IActionResult StyleA()
        {
            var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay.png");

            var payTypes = new List<QrCodePayType>() {
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            var buffer = DrawQrCode.GenerateStyleAImage(title: "吉日科技", logoPath: logoPath, iconPath: Path.Combine(_env.WebRootPath, "images", "avatar.png"), payTypes: payTypes);
            using (var stream = new MemoryStream(buffer))
            {
                // 返回生成的码牌图片
                return File(stream.ToArray(), "image/png");
            }
        }

        [HttpGet, AllowAnonymous, Route("styleb.png")]
        public IActionResult StyleB()
        {
            var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png");

            var payTypes = new List<QrCodePayType>() {
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            var buffer = DrawQrCode.GenerateStyleBImage(title: "吉日科技", logoPath: logoPath, iconPath: Path.Combine(_env.WebRootPath, "images", "avatar.png"), payTypes: payTypes);
            using (var stream = new MemoryStream(buffer))
            {
                // 返回生成的码牌图片
                return File(stream.ToArray(), "image/png");
            }
        }
    }
}

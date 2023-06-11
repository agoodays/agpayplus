using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [ApiController, Authorize]
    [Route("api/qrc")]
    public class QrCodeController : ControllerBase
    {
        private readonly ILogger<QrCodeController> _logger;
        private readonly IQrCodeService _qrCodeService;

        public QrCodeController(ILogger<QrCodeController> logger, IQrCodeService qrCodeService)
        {
            _logger = logger;
            _qrCodeService = qrCodeService;
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
    }
}

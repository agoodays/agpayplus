using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Division
{
    /// <summary>
    /// 商户分账接收者账号组
    /// </summary>
    [Route("api/division/records")]
    [ApiController, Authorize]
    public class PayOrderDivisionRecordController : CommonController
    {
        private readonly ILogger<PayOrderDivisionRecordController> _logger;
        private readonly IPayOrderDivisionRecordService _payOrderDivisionRecordService;

        public PayOrderDivisionRecordController(ILogger<PayOrderDivisionRecordController> logger,
            IPayOrderDivisionRecordService payOrderDivisionRecordService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _payOrderDivisionRecordService = payOrderDivisionRecordService;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECORD_LIST)]
        public ApiRes List([FromQuery] PayOrderDivisionRecordQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var data = _payOrderDivisionRecordService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECORD_VIEW)]
        public ApiRes Detail(long recordId)
        {
            var record = _payOrderDivisionRecordService.GetById(recordId, GetCurrentMchNo());
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }
    }
}

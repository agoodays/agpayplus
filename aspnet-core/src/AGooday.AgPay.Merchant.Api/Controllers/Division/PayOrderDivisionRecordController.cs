using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Division
{
    /// <summary>
    /// 商户分账接收者账号组
    /// </summary>
    [Route("api/division/records")]
    [ApiController]
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

        [HttpGet, Route("")]
        public ApiRes List([FromQuery] PayOrderDivisionRecordQueryDto dto)
        {
            dto.MchNo = GetCurrentUser().User.BelongInfoId;
            var data = _payOrderDivisionRecordService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        [HttpGet, Route("{recordId}")]
        public ApiRes Detail(long recordId)
        {
            var record = _payOrderDivisionRecordService.GetById(recordId, GetCurrentUser().User.BelongInfoId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }
    }
}

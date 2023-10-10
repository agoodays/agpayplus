using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
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
        private readonly IMQSender mqSender;
        private readonly ILogger<PayOrderDivisionRecordController> _logger;
        private readonly IPayOrderDivisionRecordService _payOrderDivisionRecordService;

        public PayOrderDivisionRecordController(ILogger<PayOrderDivisionRecordController> logger,
            IPayOrderDivisionRecordService payOrderDivisionRecordService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService, IMQSender mqSender)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _payOrderDivisionRecordService = payOrderDivisionRecordService;
            this.mqSender = mqSender;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECORD_LIST)]
        public ApiPageRes<PayOrderDivisionRecordDto> List([FromQuery] PayOrderDivisionRecordQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var data = _payOrderDivisionRecordService.GetPaginatedData(dto);
            return ApiPageRes<PayOrderDivisionRecordDto>.Pages(data);
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


        [HttpPost, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECORD_RESEND)]
        public ApiRes Resend(long recordId)
        {
            var record = _payOrderDivisionRecordService.GetById(recordId, GetCurrentMchNo());
            if (record == null)
            {
                throw new BizException(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }

            if (record.State != (byte)PayOrderDivisionRecordState.STATE_FAIL)
            {
                throw new BizException("请选择失败的分账记录");
            }

            // 更新订单状态 & 记录状态
            _payOrderDivisionRecordService.UpdateResendState(record.PayOrderId);

            // 重发到MQ
            mqSender.Send(PayOrderDivisionMQ.Build(record.PayOrderId, null, null, true));

            return ApiRes.Ok(record);
        }
    }
}

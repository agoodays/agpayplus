using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Division
{
    /// <summary>
    /// 分账记录
    /// </summary>
    [Route("api/division/records")]
    [ApiController, Authorize]
    public class PayOrderDivisionRecordController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<PayOrderDivisionRecordController> _logger;
        private readonly IMchInfoService _mchInfoService;
        private readonly IPayOrderDivisionRecordService _payOrderDivisionRecordService;

        public PayOrderDivisionRecordController(ILogger<PayOrderDivisionRecordController> logger, 
            IMchInfoService mchInfoService,
            IPayOrderDivisionRecordService payOrderDivisionRecordService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService, IMQSender mqSender)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _mchInfoService = mchInfoService;
            _payOrderDivisionRecordService = payOrderDivisionRecordService;
            this.mqSender = mqSender;
        }

        /// <summary>
        /// 分账记录列表
        /// </summary>
        /// <param name="dto">分账记录ID</param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECORD_LIST)]
        public ApiPageRes<PayOrderDivisionRecordDto> List([FromQuery] PayOrderDivisionRecordQueryDto dto)
        {
            var data = _payOrderDivisionRecordService.GetPaginatedData(dto);
            var mchNos = data.Select(s => s.MchNo).Distinct().ToList();
            var mchInfos = _mchInfoService.GetByMchNos(mchNos);
            foreach (var item in data)
            {
                item.AddExt("mchName", mchInfos?.FirstOrDefault(s => s.MchNo == item.MchNo)?.MchName);
            }
            return ApiPageRes<PayOrderDivisionRecordDto>.Pages(data);
        }

        /// <summary>
        /// 分账记录详情
        /// </summary>
        /// <param name="recordId">分账记录ID</param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECORD_VIEW)]
        public ApiRes Detail(long recordId)
        {
            var record = _payOrderDivisionRecordService.GetById(recordId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }

        /// <summary>
        /// 分账接口重试
        /// </summary>
        /// <param name="recordId">分账记录ID</param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECORD_RESEND)]
        public ApiRes Resend(long recordId)
        {
            var record = _payOrderDivisionRecordService.GetById(recordId);
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

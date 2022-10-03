using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Merchant.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Division
{
    [Route("api/divisionReceivers")]
    [ApiController]
    public class MchDivisionReceiverController : CommonController
    {
        private readonly ILogger<MchDivisionReceiverController> _logger;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;
        private readonly IMchAppService _mchAppService;
        private readonly ISysConfigService _sysConfigService;

        public MchDivisionReceiverController(ILogger<MchDivisionReceiverController> logger,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService,
            IMchAppService mchAppService,
            ISysConfigService sysConfigService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
            _mchAppService = mchAppService;
            _sysConfigService = sysConfigService;
        }

        [HttpGet, Route("")]
        public ApiRes List([FromQuery] MchDivisionReceiverQueryDto dto)
        {
            dto.MchNo = GetCurrentUser().User.BelongInfoId;
            var data = _mchDivisionReceiverService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        [HttpGet, Route("{recordId}")]
        public ApiRes Detail(long recordId)
        {
            var record = _mchDivisionReceiverService.GetById(recordId, GetCurrentUser().User.BelongInfoId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }

        [HttpPost, Route("")]
        public ApiRes Add(DivisionReceiverBindReqModel model)
        {
            var mchApp = _mchAppService.GetById(model.AppId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE || !mchApp.MchNo.Equals(GetCurrentUser().User.BelongInfoId))
            {
                throw new BizException("商户应用不存在或不可用");
            }
            DivisionReceiverBindRequest request = new DivisionReceiverBindRequest();
            request.SetBizModel(model);
            model.MchNo = GetCurrentUser().User.BelongInfoId;
            model.AppId = mchApp.AppId;

            var agPayClient = new AgPayClient(_sysConfigService.GetDBApplicationConfig().PaySiteUrl, mchApp.AppSecret);

            try
            {
                DivisionReceiverBindResponse response = agPayClient.Execute(request);
                if (response.code != 0)
                {
                    throw new BizException(response.msg);
                }
                return ApiRes.Ok(response);
            }
            catch (AgPayException e)
            {
                throw new BizException(e.Message);
            }
        }
    }
}

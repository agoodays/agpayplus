using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Division
{
    /// <summary>
    /// 商户分账接收者账号关系维护
    /// </summary>
    [Route("api/divisionReceivers")]
    [ApiController, Authorize]
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

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_LIST)]
        public ApiRes List([FromQuery] MchDivisionReceiverQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var data = _mchDivisionReceiverService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_VIEW)]
        public ApiRes Detail(long recordId)
        {
            var record = _mchDivisionReceiverService.GetById(recordId, GetCurrentMchNo());
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }

        /// <summary>
        /// 新增分账接收账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增分账接收账号")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_ADD)]
        public ApiRes Add(DivisionReceiverBindReqModel model)
        {
            var mchApp = _mchAppService.GetById(model.AppId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE || !mchApp.MchNo.Equals(GetCurrentMchNo()))
            {
                throw new BizException("商户应用不存在或不可用");
            }
            DivisionReceiverBindRequest request = new DivisionReceiverBindRequest();
            request.SetBizModel(model);
            model.MchNo = GetCurrentMchNo();
            model.AppId = mchApp.AppId;
            model.DivisionProfit = (Convert.ToDecimal(model.DivisionProfit) / 100).ToString();

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

        /// <summary>
        /// 更新分账接收账号
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPut, Route("{recordId}"), MethodLog("更新分账接收账号")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_EDIT)]
        public ApiRes Update(long recordId, MchDivisionReceiverDto record)
        {
            // 改为真实比例
            record.DivisionProfit = record.DivisionProfit / 100;
            if (record.ReceiverGroupId != null)
            {
                var groupRecord = _mchDivisionReceiverGroupService.FindByIdAndMchNo(record.ReceiverGroupId.Value, GetCurrentMchNo());
                if (record == null)
                {
                    throw new BizException("账号组不存在");
                }
                record.ReceiverGroupId = groupRecord.ReceiverGroupId;
                record.ReceiverGroupName = groupRecord.ReceiverGroupName;
            }
            var result = _mchDivisionReceiverService.Update(record);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除分账接收账号
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除分账接收账号")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_DELETE)]
        public ApiRes Delete(long recordId)
        {
            var record = _mchDivisionReceiverService.GetById(recordId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            _mchDivisionReceiverService.Remove(recordId);
            return ApiRes.Ok();
        }
    }
}

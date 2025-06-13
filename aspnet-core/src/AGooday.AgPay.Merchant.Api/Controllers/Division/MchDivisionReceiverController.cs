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
using AGooday.AgPay.Components.Cache.Services;
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
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;
        private readonly IMchAppService _mchAppService;
        private readonly ISysConfigService _sysConfigService;

        public MchDivisionReceiverController(ILogger<MchDivisionReceiverController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService,
            IMchAppService mchAppService,
            ISysConfigService sysConfigService)
            : base(logger, cacheService, authService)
        {
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
            _mchAppService = mchAppService;
            _sysConfigService = sysConfigService;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_LIST)]
        public async Task<ApiPageRes<MchDivisionReceiverDto>> ListAsync([FromQuery] MchDivisionReceiverQueryDto dto)
        {
            dto.MchNo = await GetCurrentMchNoAsync();
            var data = await _mchDivisionReceiverService.GetPaginatedDataAsync(dto);
            return ApiPageRes<MchDivisionReceiverDto>.Pages(data);
        }

        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_VIEW)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var record = await _mchDivisionReceiverService.GetByIdAsNoTrackingAsync(recordId, await GetCurrentMchNoAsync());
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
        public async Task<ApiRes> AddAsync(DivisionReceiverBindReqModel model)
        {
            var mchApp = await _mchAppService.GetByIdAsync(model.AppId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE || !mchApp.MchNo.Equals(await GetCurrentMchNoAsync()))
            {
                throw new BizException("商户应用不存在或不可用");
            }
            DivisionReceiverBindRequest request = new DivisionReceiverBindRequest();
            request.SetBizModel(model);
            model.MchNo = await GetCurrentMchNoAsync();
            model.AppId = mchApp.AppId;
            model.DivisionProfit = (Convert.ToDecimal(model.DivisionProfit) / 100).ToString();

            var agPayClient = new AgPayClient(_sysConfigService.GetDBApplicationConfig().PaySiteUrl, mchApp.AppSecret);

            try
            {
                DivisionReceiverBindResponse response = await agPayClient.ExecuteAsync(request);
                if (response.Code != 0)
                {
                    throw new BizException(response.Msg);
                }
                return ApiRes.Ok(response.Get());
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
        public async Task<ApiRes> UpdateAsync(long recordId, MchDivisionReceiverModifyDto dto)
        {
            var record = await _mchDivisionReceiverService.GetByIdAsNoTrackingAsync(recordId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            record.ReceiverAlias = dto.ReceiverAlias;
            // 改为真实比例
            record.DivisionProfit = dto.DivisionProfit / 100;
            record.State = dto.State;
            if (record.ReceiverGroupId != null)
            {
                var groupRecord = await _mchDivisionReceiverGroupService.FindByIdAndMchNoAsync(dto.ReceiverGroupId.Value, await GetCurrentMchNoAsync());
                if (groupRecord == null)
                {
                    throw new BizException("账号组不存在");
                }
                record.ReceiverGroupId = groupRecord.ReceiverGroupId;
                record.ReceiverGroupName = groupRecord.ReceiverGroupName;
            }
            var result = await _mchDivisionReceiverService.UpdateAsync(record);
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
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var record = await _mchDivisionReceiverService.GetByIdAsync(recordId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            await _mchDivisionReceiverService.RemoveAsync(recordId);
            return ApiRes.Ok();
        }
    }
}

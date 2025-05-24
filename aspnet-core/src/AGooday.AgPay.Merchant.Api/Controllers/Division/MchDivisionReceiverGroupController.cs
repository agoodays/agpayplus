using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
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
    /// 商户分账接收者账号组
    /// </summary>
    [Route("api/divisionReceiverGroups")]
    [ApiController, Authorize]
    public class MchDivisionReceiverGroupController : CommonController
    {
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverGroupController(ILogger<MchDivisionReceiverGroupController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService)
            : base(logger, cacheService, authService)
        {
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_LIST)]
        public async Task<ApiPageRes<MchDivisionReceiverGroupDto>> ListAsync([FromQuery] MchDivisionReceiverGroupQueryDto dto)
        {
            dto.MchNo = await GetCurrentMchNoAsync();
            var data = await _mchDivisionReceiverGroupService.GetPaginatedDataAsync(dto);
            return ApiPageRes<MchDivisionReceiverGroupDto>.Pages(data);
        }

        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_VIEW)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var record = _mchDivisionReceiverGroupService.GetByIdAsNoTrackingAsync(recordId, await GetCurrentMchNoAsync());
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }

        /// <summary>
        /// 新增分账账号组
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增分账账号组")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_ADD)]
        public async Task<ApiRes> AddAsync(MchDivisionReceiverGroupDto dto)
        {
            var sysUser = (await GetCurrentUserAsync()).SysUser;
            dto.MchNo = sysUser.BelongInfoId;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;

            var result = await _mchDivisionReceiverGroupService.AddAsync(dto);
            if (result)
            {
                // 更新其他组为非默认分账组
                await _mchDivisionReceiverGroupService.UpdateAutoDivisionFlagAsync(dto);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新分账账号组
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新分账账号组")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_EDIT)]
        public async Task<ApiRes> UpdateAsync(long recordId, MchDivisionReceiverGroupDto dto)
        {
            dto.MchNo = await GetCurrentMchNoAsync();
            var result = await _mchDivisionReceiverGroupService.UpdateAsync(dto);
            if (result)
            {
                // 更新其他组为非默认分账组
                await _mchDivisionReceiverGroupService.UpdateAutoDivisionFlagAsync(dto);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除分账账号组
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpDelete, Route("{recordId}"), MethodLog("删除分账账号组")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_DELETE)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var record = await _mchDivisionReceiverGroupService.GetByIdAsync(recordId);
            if (record == null || !record.MchNo.Equals(await GetCurrentMchNoAsync()))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (await _mchDivisionReceiverService.IsExistUseReceiverGroupAsync(record.ReceiverGroupId.Value))
            {
                throw new BizException("该组存在账号，无法删除");
            }
            await _mchDivisionReceiverService.RemoveAsync(recordId);
            return ApiRes.Ok();
        }
    }
}

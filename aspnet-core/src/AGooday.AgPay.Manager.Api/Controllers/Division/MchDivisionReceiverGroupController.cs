using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Division
{
    /// <summary>
    /// 商户分账接收者账号组
    /// </summary>
    [Route("api/divisionReceiverGroups")]
    [ApiController, Authorize]
    public class MchDivisionReceiverGroupController : CommonController
    {
        private readonly IMchInfoService _mchInfoService;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverGroupController(ILogger<MchDivisionReceiverGroupController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchInfoService mchInfoService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService)
            : base(logger, cacheService, authService)
        {
            _mchInfoService = mchInfoService;
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
        }

        /// <summary>
        /// 账号组列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_LIST)]
        public async Task<ApiPageRes<MchDivisionReceiverGroupDto>> ListAsync([FromQuery] MchDivisionReceiverGroupQueryDto dto)
        {
            var data = await _mchDivisionReceiverGroupService.GetPaginatedDataAsync(dto);
            var mchNos = data.Items.Select(s => s.MchNo).Distinct().ToList();
            var mchInfos = await _mchInfoService.GetByMchNosAsNoTrackingAsync(mchNos);
            foreach (var item in data.Items)
            {
                item.AddExt("mchName", mchInfos?.FirstOrDefault(s => s.MchNo == item.MchNo)?.MchName);
            }
            return ApiPageRes<MchDivisionReceiverGroupDto>.Pages(data);
        }

        /// <summary>
        /// 账号组详情
        /// </summary>
        /// <param name="recordId">账号组ID</param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_VIEW)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var record = await _mchDivisionReceiverGroupService.GetByIdAsNoTrackingAsync(recordId);
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
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_ADD)]
        public async Task<ApiRes> AddAsync(MchDivisionReceiverGroupDto dto)
        {
            if (!await _mchInfoService.IsExistMchNoAsync(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var sysUser = (await GetCurrentUserAsync()).SysUser;
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
        /// <param name="record">账号组ID</param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新分账账号组")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_EDIT)]
        public async Task<ApiRes> UpdateAsync(long recordId, MchDivisionReceiverGroupDto dto)
        {
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
        /// <param name="recordId">账号组ID</param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpDelete, Route("{recordId}"), MethodLog("删除分账账号组")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_DELETE)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var record = await _mchDivisionReceiverGroupService.GetByIdAsync(recordId);
            if (record == null)
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

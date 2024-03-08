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
    /// 商户分账接收者账号组
    /// </summary>
    [Route("api/divisionReceiverGroups")]
    [ApiController, Authorize]
    public class MchDivisionReceiverGroupController : CommonController
    {
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverGroupController(ILogger<MchDivisionReceiverGroupController> logger,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService, 
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_LIST)]
        public ApiPageRes<MchDivisionReceiverGroupDto> List([FromQuery] MchDivisionReceiverGroupQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var data = _mchDivisionReceiverGroupService.GetPaginatedData(dto);
            return ApiPageRes<MchDivisionReceiverGroupDto>.Pages(data);
        }

        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_VIEW)]
        public ApiRes Detail(long recordId)
        {
            var record = _mchDivisionReceiverGroupService.GetById(recordId, GetCurrentMchNo());
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
        public ApiRes Add(MchDivisionReceiverGroupDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.MchNo = sysUser.BelongInfoId;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;

            var result = _mchDivisionReceiverGroupService.Add(dto);
            if (result)
            {
                // 更新其他组为非默认分账组
                if (dto.AutoDivisionFlag == CS.YES)
                {
                    _mchDivisionReceiverGroupService.GetByMchNo(dto.MchNo)
                        .Where(w => !w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                        .ToList().ForEach(w =>
                        {
                            w.AutoDivisionFlag = CS.NO;
                            _mchDivisionReceiverGroupService.Update(w);
                        });
                }
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
        public ApiRes Update(long recordId, MchDivisionReceiverGroupDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var result = _mchDivisionReceiverGroupService.Update(dto);
            if (result)
            {
                // 更新其他组为非默认分账组
                if (dto.AutoDivisionFlag == CS.YES)
                {
                    _mchDivisionReceiverGroupService.GetByMchNo(dto.MchNo)
                        .Where(w => !w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                        .ToList().ForEach(w =>
                        {
                            w.AutoDivisionFlag = CS.NO;
                            _mchDivisionReceiverGroupService.Update(w);
                        });
                }
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
        public ApiRes Delete(long recordId)
        {
            var record = _mchDivisionReceiverGroupService.GetById(recordId);
            if (record == null || !record.MchNo.Equals(GetCurrentMchNo()))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (_mchDivisionReceiverService.IsExistUseReceiverGroup(record.ReceiverGroupId.Value))
            {
                throw new BizException("该组存在账号，无法删除");
            }
            _mchDivisionReceiverService.Remove(recordId);
            return ApiRes.Ok();
        }
    }
}

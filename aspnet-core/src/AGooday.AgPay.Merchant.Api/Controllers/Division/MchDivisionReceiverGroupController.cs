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
        private readonly ILogger<MchDivisionReceiverGroupController> _logger;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverGroupController(ILogger<MchDivisionReceiverGroupController> logger,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_LIST)]
        public ApiRes List([FromQuery] MchDivisionReceiverGroupQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var data = _mchDivisionReceiverGroupService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
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
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增分账账号组")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_ADD)]
        public ApiRes Add(MchDivisionReceiverGroupDto record)
        {
            var sysUser = GetCurrentUser().SysUser;
            record.MchNo = sysUser.BelongInfoId;
            record.CreatedBy = sysUser.Realname;
            record.CreatedUid = sysUser.SysUserId;

            var result = _mchDivisionReceiverGroupService.Add(record);
            if (result)
            {
                // 更新其他组为非默认分账组
                if (record.AutoDivisionFlag == CS.YES)
                {
                    _mchDivisionReceiverGroupService.GetByMchNo(record.MchNo)
                        .Where(w => !w.ReceiverGroupId.Equals(record.ReceiverGroupId))
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
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新分账账号组")]
        [PermissionAuth(PermCode.MCH.ENT_DIVISION_RECEIVER_GROUP_EDIT)]
        public ApiRes Update(long recordId, MchDivisionReceiverGroupDto record)
        {
            record.MchNo = GetCurrentMchNo();
            var result = _mchDivisionReceiverGroupService.Update(record);
            if (result)
            {
                // 更新其他组为非默认分账组
                if (record.AutoDivisionFlag == CS.YES)
                {
                    _mchDivisionReceiverGroupService.GetByMchNo(record.MchNo)
                        .Where(w => !w.ReceiverGroupId.Equals(record.ReceiverGroupId))
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
            if (_mchDivisionReceiverService.IsExistUseReceiverGroup(record.ReceiverGroupId))
            {
                throw new BizException("该组存在账号，无法删除");
            }
            _mchDivisionReceiverService.Remove(recordId);
            return ApiRes.Ok();
        }
    }
}

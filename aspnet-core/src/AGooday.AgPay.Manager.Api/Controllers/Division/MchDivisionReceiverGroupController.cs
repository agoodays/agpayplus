using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AGooday.AgPay.Manager.Api.Controllers.Division
{
    /// <summary>
    /// 商户分账接收者账号组
    /// </summary>
    [Route("api/divisionReceiverGroups")]
    [ApiController, Authorize]
    public class MchDivisionReceiverGroupController : CommonController
    {
        private readonly ILogger<MchDivisionReceiverGroupController> _logger;
        private readonly IMchInfoService _mchInfoService;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverGroupController(ILogger<MchDivisionReceiverGroupController> logger, 
            IMchInfoService mchInfoService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
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
        public ApiPageRes<MchDivisionReceiverGroupDto> List([FromQuery] MchDivisionReceiverGroupQueryDto dto)
        {
            var data = _mchDivisionReceiverGroupService.GetPaginatedData(dto);
            var mchNos = data.Select(s => s.MchNo).Distinct().ToList();
            var mchInfos = _mchInfoService.GetByMchNos(mchNos);
            foreach (var item in data)
            {
                item.AddExt("mchName", mchInfos.First(s => s.MchNo == item.MchNo).MchName);
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
        public ApiRes Detail(long recordId)
        {
            var record = _mchDivisionReceiverGroupService.GetById(recordId);
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
        public ApiRes Add(MchDivisionReceiverGroupDto dto)
        {
            if (!_mchInfoService.IsExistMchNo(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var sysUser = GetCurrentUser().SysUser;
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
        /// <param name="record">账号组ID</param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新分账账号组")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_EDIT)]
        public ApiRes Update(long recordId, MchDivisionReceiverGroupDto record)
        {
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
        /// <param name="recordId">账号组ID</param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpDelete, Route("{recordId}"), MethodLog("删除分账账号组")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_GROUP_DELETE)]
        public ApiRes Delete(long recordId)
        {
            var record = _mchDivisionReceiverGroupService.GetById(recordId);
            if (record == null)
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

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
{
    /// <summary>
    /// 服务商管理类
    /// </summary>
    [Route("api/isvInfo")]
    [ApiController, Authorize]
    public class IsvInfoController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly IIsvInfoService _isvInfoService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;

        public IsvInfoController(ILogger<IsvInfoController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMQSender mqSender,
            IIsvInfoService isvInfoService,
            IAgentInfoService agentInfoService,
            IMchInfoService mchInfoService,
            IPayInterfaceConfigService payInterfaceConfigService)
            : base(logger, cacheService, authService)
        {
            _mqSender = mqSender;
            _isvInfoService = isvInfoService;
            _agentInfoService = agentInfoService;
            _mchInfoService = mchInfoService;
            _payInterfaceConfigService = payInterfaceConfigService;
        }

        /// <summary>
        /// 查询服务商信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_LIST, PermCode.MGR.ENT_MCH_INFO_ADD, PermCode.MGR.ENT_MCH_INFO_EDIT, PermCode.MGR.ENT_MCH_INFO_VIEW)]
        public async Task<ApiPageRes<IsvInfoDto>> ListAsync([FromQuery] IsvInfoQueryDto dto)
        {
            var data = await _isvInfoService.GetPaginatedDataAsync(dto);
            return ApiPageRes<IsvInfoDto>.Pages(data);
        }

        /// <summary>
        /// 新增服务商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增服务商")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_ADD)]
        public async Task<ApiRes> AddAsync(IsvInfoDto dto)
        {
            var sysUser = (await GetCurrentUserAsync()).SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            var result = await _isvInfoService.AddAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除服务商
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpDelete, Route("{isvNo}"), MethodLog("删除服务商")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_DEL)]
        public async Task<ApiRes> DeleteAsync(string isvNo)
        {
            // 0.当前服务商是否存在
            var isvInfo = await _isvInfoService.GetByIdAsync(isvNo);
            if (isvInfo == null)
            {
                throw new BizException("该服务商不存在");
            }

            // 1.查询当前服务商下是否存在商户
            if (await _mchInfoService.IsExistMchByIsvNoAsync(isvInfo.IsvNo))
            {
                throw new BizException("该服务商下存在商户，不可删除");
            }

            // 2.查询当前服务商下是否存在代理商
            if (await _agentInfoService.IsExistAgentAsync(isvInfo.IsvNo))
            {
                throw new BizException("该服务商下存在代理商，不可删除");
            }

            // 3.删除当前服务商支付接口配置参数
            await _payInterfaceConfigService.RemoveAsync(CS.INFO_TYPE.ISV, isvInfo.IsvNo);

            // 4.删除该服务商
            var remove = await _isvInfoService.RemoveAsync(isvNo);
            if (!remove)
            {
                throw new BizException("删除服务商失败");
            }

            // 推送mq到目前节点进行更新数据
            await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, isvNo, null, null, null));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新服务商信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{isvNo}"), MethodLog("更新服务商信息")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_EDIT)]
        public async Task<ApiRes> UpdateAsync(string isvNo, IsvInfoDto dto)
        {
            await _isvInfoService.UpdateAsync(dto);

            // 推送mq到目前节点进行更新数据
            await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, dto.IsvNo, null, null, null));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看服务商信息
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet, Route("{isvNo}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_VIEW, PermCode.MGR.ENT_ISV_INFO_EDIT)]
        public async Task<ApiRes> DetailAsync(string isvNo)
        {
            var isvInfo = await _isvInfoService.GetByIdAsNoTrackingAsync(isvNo);
            if (isvInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(isvInfo);
        }
    }
}

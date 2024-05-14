using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
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
        private readonly IMQSender mqSender;
        private readonly IIsvInfoService _isvInfoService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;

        public IsvInfoController(ILogger<IsvInfoController> logger, 
            IMQSender mqSender,
            IIsvInfoService isvInfoService,
            IAgentInfoService agentInfoService,
            IMchInfoService mchInfoService,
            IPayInterfaceConfigService payInterfaceConfigService, 
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            this.mqSender = mqSender;
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
        public ApiPageRes<IsvInfoDto> List([FromQuery] IsvInfoQueryDto dto)
        {
            var data = _isvInfoService.GetPaginatedData(dto);
            return ApiPageRes<IsvInfoDto>.Pages(data);
        }

        /// <summary>
        /// 新增服务商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增服务商")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_ADD)]
        public ApiRes Add(IsvInfoDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            var result = _isvInfoService.Add(dto);
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
        public ApiRes Delete(string isvNo)
        {
            // 0.当前服务商是否存在
            var isvInfo = _isvInfoService.GetById(isvNo);
            if (isvInfo == null)
            {
                throw new BizException("该服务商不存在");
            }

            // 1.查询当前服务商下是否存在商户
            if (_mchInfoService.IsExistMchByIsvNo(isvInfo.IsvNo))
            {
                throw new BizException("该服务商下存在商户，不可删除");
            }

            // 2.查询当前服务商下是否存在代理商
            if (_agentInfoService.IsExistAgent(isvInfo.IsvNo))
            {
                throw new BizException("该服务商下存在代理商，不可删除");
            }

            // 3.删除当前服务商支付接口配置参数
            _payInterfaceConfigService.Remove(CS.INFO_TYPE.ISV, isvInfo.IsvNo);

            // 4.删除该服务商
            var remove = _isvInfoService.Remove(isvNo);
            if (!remove)
            {
                throw new BizException("删除服务商失败");
            }

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, isvNo, null, null, null));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新服务商信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{isvNo}"), MethodLog("更新服务商信息")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_EDIT)]
        public ApiRes Update(string isvNo, IsvInfoDto dto)
        {
            _isvInfoService.Update(dto);

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, dto.IsvNo, null, null, null));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看服务商信息
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet, Route("{isvNo}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_VIEW, PermCode.MGR.ENT_ISV_INFO_EDIT)]
        public ApiRes Detail(string isvNo)
        {
            var isvInfo = _isvInfoService.GetById(isvNo);
            if (isvInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(isvInfo);
        }
    }
}

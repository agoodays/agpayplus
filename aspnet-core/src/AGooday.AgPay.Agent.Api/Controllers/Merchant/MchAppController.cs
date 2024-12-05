using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Extensions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AGooday.AgPay.Agent.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户应用管理类
    /// </summary>
    [Route("api/mchApps")]
    [ApiController, Authorize]
    public class MchAppController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly SysRSA2Config _sysRSA2Config;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;

        public MchAppController(ILogger<MchAppController> logger,
            IMQSender mqSender,
            IOptions<SysRSA2Config> sysRSA2Config,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            this.mqSender = mqSender;
            _sysRSA2Config = sysRSA2Config.Value;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
        }

        /// <summary>
        /// 应用列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_APP_LIST)]
        public ApiPageRes<MchAppDto> List([FromQuery] MchAppQueryDto dto)
        {
            var data = _mchAppService.GetPaginatedData(dto, GetCurrentAgentNo());
            var mchNos = data.Select(s => s.MchNo).Distinct().ToList();
            var mchInfos = _mchInfoService.GetByMchNos(mchNos);
            //JArray records = new JArray();
            foreach (var item in data)
            {
                //var jitem = JObject.FromObject(item);
                //jitem["mchType"] = mchInfos.First(s => s.MchNo == item.MchNo).Type;
                //records.Add(jitem);
                item.AddExt("mchType", mchInfos.First(s => s.MchNo == item.MchNo).Type);
            }
            return ApiPageRes<MchAppDto>.Pages(data);
        }

        /// <summary>
        /// 新建应用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建应用")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_APP_ADD)]
        public async Task<ApiRes> AddAsync(MchAppDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            dto.AppId = SeqUtil.GenAppId();
            if (!_mchInfoService.IsExistMchNo(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }

            var result = await _mchAppService.AddAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{appId}"), MethodLog("删除应用")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_APP_DEL)]
        public async Task<ApiRes> DeleteAsync(string appId)
        {
            _mchAppService.Remove(appId);

            // 推送mq到目前节点进行更新数据
            var mchApp = await _mchAppService.GetByIdAsync(appId);
            mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, null, mchApp.MchNo, appId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新应用信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{appId}"), MethodLog("更新应用信息")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_APP_EDIT)]
        public ApiRes Update(string appId, MchAppDto dto)
        {
            var result = _mchAppService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            // 推送修改应用消息
            mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, null, dto.MchNo, dto.AppId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 应用详情
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, Route("{appId}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_APP_VIEW, PermCode.AGENT.ENT_MCH_APP_EDIT)]
        public async Task<ApiRes> DetailAsync(string appId)
        {
            var mchApp = await _mchAppService.GetByIdAsync(appId);
            if (mchApp == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            mchApp.AppSecret = mchApp.AppSecret.Mask();
            return ApiRes.Ok(mchApp);
        }

        /// <summary>
        /// 获取支付网关系统公钥
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("sysRSA2PublicKey"), AllowAnonymous, NoLog]
        public ApiRes SysRSA2PublicKey()
        {
            return ApiRes.Ok(_sysRSA2Config.PublicKey);
        }
    }
}

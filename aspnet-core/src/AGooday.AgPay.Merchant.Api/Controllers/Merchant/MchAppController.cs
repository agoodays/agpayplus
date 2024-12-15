using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Extensions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using AGooday.AgPay.Merchant.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AGooday.AgPay.Merchant.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户应用管理类
    /// </summary>
    [Route("api/mchApps")]
    [ApiController, Authorize]
    public class MchAppController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly SysRSA2Config _sysRSA2Config;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;

        public MchAppController(ILogger<MchAppController> logger,
            IOptions<SysRSA2Config> sysRSA2Config,
            IMQSender mqSender,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _mqSender = mqSender;
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
        [PermissionAuth(PermCode.MCH.ENT_MCH_APP_LIST)]
        public async Task<ApiPageRes<MchAppDto>> ListAsync([FromQuery] MchAppQueryDto dto)
        {
            var mchNo = GetCurrentMchNo();
            dto.MchNo = mchNo;
            var data = await _mchAppService.GetPaginatedDataAsync(dto);
            var mchNos = data.Select(s => s.MchNo).Distinct().ToList();
            var mchInfos = _mchInfoService.GetByMchNos(mchNos);
            //JArray records = new JArray();
            foreach (var item in data)
            {
                //var jitem = JObject.FromObject(item);
                //jitem["mchType"] = mchInfos.First(s => s.MchNo == item.MchNo).Type;
                //records.Add(jitem);
                item.AddExt("mchType", mchInfos?.FirstOrDefault(s => s.MchNo == item.MchNo)?.Type);
            }
            return ApiPageRes<MchAppDto>.Pages(data);
        }

        /// <summary>
        /// 新建应用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建应用")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_APP_ADD)]
        public async Task<ApiRes> AddAsync(MchAppDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.MchNo = sysUser.BelongInfoId;
            dto.AppId = SeqUtil.GenAppId();
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            if (!await _mchInfoService.IsExistMchNoAsync(dto.MchNo))
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
        [PermissionAuth(PermCode.MCH.ENT_MCH_APP_DEL)]
        public async Task<ApiRes> DeleteAsync(string appId)
        {
            await _mchAppService.RemoveAsync(appId);

            // 推送mq到目前节点进行更新数据
            var mchApp = await _mchAppService.GetByIdAsync(appId);
            await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, null, mchApp.MchNo, appId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新应用信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{appId}"), MethodLog("更新应用信息")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_APP_EDIT)]
        public async Task<ApiRes> UpdateAsync(string appId, MchAppDto dto)
        {
            var result = await _mchAppService.UpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            // 推送修改应用消息
            await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, null, dto.MchNo, dto.AppId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 应用详情
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, Route("{appId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_MCH_APP_VIEW, PermCode.MCH.ENT_MCH_APP_EDIT)]
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

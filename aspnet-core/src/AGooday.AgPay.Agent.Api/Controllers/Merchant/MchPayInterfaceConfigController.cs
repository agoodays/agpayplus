using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.Merchant
{
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("api/mch/payConfigs")]
    [ApiController, Authorize]
    public class MchPayInterfaceConfigController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly ISysConfigService _sysConfigService;

        public MchPayInterfaceConfigController(ILogger<MchPayInterfaceConfigController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMQSender mqSender,
            IPayInterfaceConfigService payIfConfigService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            ISysConfigService sysConfigService)
            : base(logger, cacheService, authService)
        {
            _mqSender = mqSender;
            _payIfConfigService = payIfConfigService;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 查询应用支付接口配置列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_PAY_CONFIG_LIST)]
        public async Task<ApiRes> ListAstnc(string appId)
        {
            var data = await _payIfConfigService.SelectAllPayIfConfigListByAppIdAsync(appId);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 appId、接口类型 获取应用参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("{appId}/{ifCode}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_PAY_CONFIG_VIEW)]
        public async Task<ApiRes> GetByAppIdAsync(string appId, string ifCode)
        {
            var payInterfaceConfig = await _payIfConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.MCH_APP, appId, ifCode);
            if (payInterfaceConfig != null)
            {
                // 费率转换为百分比数值
                payInterfaceConfig.IfRate = payInterfaceConfig.IfRate * 100;

                // 敏感数据脱敏
                if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                {
                    var mchApp = await _mchAppService.GetByIdAsync(appId);
                    var mchInfo = await _mchInfoService.GetByIdAsync(mchApp.MchNo);

                    // 普通商户的支付参数执行数据脱敏
                    if (mchInfo.Type == CS.MCH_TYPE_NORMAL)
                    {
                        NormalMchParams mchParams = NormalMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                        if (mchParams != null)
                        {
                            payInterfaceConfig.IfParams = mchParams.DeSenData();
                        }
                    }
                }
            }
            return ApiRes.Ok(payInterfaceConfig);
        }

        /// <summary>
        /// 应用支付接口配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("更新应用支付参数")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_PAY_CONFIG_ADD)]
        public async Task<ApiRes> SaveOrUpdateAsync(PayInterfaceConfigDto dto)
        {
            var mchApp = await _mchAppService.GetByIdAsync(dto.InfoId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            dto.InfoType = CS.INFO_TYPE.MCH_APP;
            dto.IfRate = dto.IfRate / 100;// 存入真实费率
            //添加更新者信息
            long userId = await GetCurrentUserIdAsync();
            string realName = (await GetCurrentUserAsync()).SysUser.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;
            dto.UpdatedAt = DateTime.Now;

            //根据 商户号、接口类型 获取商户参数配置
            var dbRecoed = await _payIfConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.MCH_APP, dto.InfoId, dto.IfCode);
            //若配置存在，为saveOrUpdate添加ID，第一次配置添加创建者
            if (dbRecoed != null)
            {
                dto.Id = dbRecoed.Id;
                dto.CreatedUid = dbRecoed.CreatedUid;
                dto.CreatedBy = dbRecoed.CreatedBy;
                dto.CreatedAt = dbRecoed.CreatedAt;
                // 合并支付参数
                dto.IfParams = StringUtil.Merge(dbRecoed.IfParams, dto.IfParams);
            }
            else
            {
                dto.CreatedUid = userId;
                dto.CreatedBy = realName;
                dto.CreatedAt = DateTime.Now;
            }

            var result = await _payIfConfigService.SaveOrUpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "配置失败");
            }

            // 推送mq到目前节点进行更新数据
            await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, null, mchApp.MchNo, dto.InfoId));

            return ApiRes.Ok();
        }

        [HttpGet, Route("alipayIsvsubMchAuthUrls/{mchAppId}"), AllowAnonymous, NoLog]
        public async Task<ApiRes> QueryAlipayIsvsubMchAuthUrlAsync(string mchAppId)
        {
            var mchApp = await _mchAppService.GetByIdAsync(mchAppId);
            var mchInfo = await _mchInfoService.GetByIdAsync(mchApp.MchNo);
            var dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            string authUrl = dbApplicationConfig.GenAlipayIsvsubMchAuthUrl(mchInfo.IsvNo, mchAppId);
            string authQrImgUrl = dbApplicationConfig.GenScanImgUrl(authUrl);
            return ApiRes.Ok(new { AuthUrl = authUrl, AuthQrImgUrl = authQrImgUrl });
        }
    }
}

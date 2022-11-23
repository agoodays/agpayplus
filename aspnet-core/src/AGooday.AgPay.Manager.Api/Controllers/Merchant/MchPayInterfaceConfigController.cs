using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using System.Runtime.InteropServices;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Attributes;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("/api/mch/payConfigs")]
    [ApiController, Authorize]
    public class MchPayInterfaceConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchPayInterfaceConfigController> _logger;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly ISysConfigService _sysConfigService;

        public MchPayInterfaceConfigController(IMQSender mqSender, ILogger<MchPayInterfaceConfigController> logger, RedisUtil client,
            IPayInterfaceConfigService payIfConfigService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService,
            ISysConfigService sysConfigService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
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
        [PermissionAuth(PermCode.MGR.ENT_MCH_PAY_CONFIG_LIST)]
        public ApiRes List(string appId)
        {
            var data = _payIfConfigService.SelectAllPayIfConfigListByAppId(appId);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 appId、接口类型 获取应用参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("{appId}/{ifCode}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_PAY_CONFIG_VIEW)]
        public ApiRes GetByAppId(string appId, string ifCode)
        {
            var payInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_MCH_APP, appId, ifCode);
            if (payInterfaceConfig != null)
            {
                // 费率转换为百分比数值
                payInterfaceConfig.IfRate = payInterfaceConfig.IfRate * 100;

                // 敏感数据脱敏
                if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                {
                    var mchApp = _mchAppService.GetById(appId);
                    var mchInfo = _mchInfoService.GetById(mchApp.MchNo);

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
        [PermissionAuth(PermCode.MGR.ENT_MCH_PAY_CONFIG_ADD)]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            var mchApp = _mchAppService.GetById(dto.InfoId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            dto.InfoType = CS.INFO_TYPE_MCH_APP;
            dto.IfRate = dto.IfRate / 100;// 存入真实费率
            //添加更新者信息
            long userId = GetCurrentUser().SysUser.SysUserId;
            string realName = GetCurrentUser().SysUser.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;

            //根据 商户号、接口类型 获取商户参数配置
            var dbRecoed = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_MCH_APP, dto.InfoId, dto.IfCode);
            //若配置存在，为saveOrUpdate添加ID，第一次配置添加创建者
            if (dbRecoed != null)
            {
                dto.Id = dbRecoed.Id;
            }
            else
            {
                dto.CreatedUid = userId;
                dto.CreatedBy = realName;
            }

            var result = _payIfConfigService.SaveOrUpdate(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "配置失败");
            }

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, mchApp.MchNo, dto.InfoId));

            return ApiRes.Ok();
        }

        [HttpGet, Route("alipayIsvsubMchAuthUrls/{mchAppId}"), AllowAnonymous, NoLog]
        public ApiRes QueryAlipayIsvsubMchAuthUrl(string mchAppId)
        {
            var mchApp = _mchAppService.GetById(mchAppId);
            var mchInfo = _mchInfoService.GetById(mchApp.MchNo);
            var dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();
            string authUrl = dbApplicationConfig.GenAlipayIsvsubMchAuthUrl(mchInfo.IsvNo, mchAppId);
            string authQrImgUrl = dbApplicationConfig.GenScanImgUrl(authUrl);
            return ApiRes.Ok(new { AuthUrl = authUrl, AuthQrImgUrl = authQrImgUrl });
        }
    }
}

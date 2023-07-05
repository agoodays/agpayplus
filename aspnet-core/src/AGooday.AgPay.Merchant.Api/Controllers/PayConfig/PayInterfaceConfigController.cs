using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付接口管理类
    /// </summary>
    [Route("/api/payConfig")]
    [ApiController, Authorize]
    public class PayInterfaceConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<PayInterfaceConfigController> _logger;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public PayInterfaceConfigController(IMQSender mqSender,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            IPayInterfaceConfigService payIfConfigService,
            ILogger<PayInterfaceConfigController> logger,
            RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _payIfConfigService = payIfConfigService;
            _mchInfoService = mchInfoService;
            _mchAppService = mchAppService;
        }

        /// <summary>
        /// 查询支付接口配置列表
        /// </summary>
        /// <param name="infoId"></param>
        /// <param name="configMode"></param>
        /// <param name="ifName"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("ifCodes"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_CONFIG_LIST)]
        public ApiRes List(string configMode, string infoId, string ifName, string ifCode)
        {
            string infoType = GetInfoType(configMode);
            var data = _payIfConfigService.PayIfConfigList(infoType, configMode, infoId, ifName, ifCode);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 获取商户参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("interfaceSavedConfigs"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_CONFIG_VIEW)]
        public ApiRes GetByInfoId(string configMode, string infoId, string ifCode)
        {
            string infoType = GetInfoType(configMode);
            var payInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(infoType, infoId, ifCode);
            if (payInterfaceConfig != null)
            {
                switch (infoType) {
                    case CS.INFO_TYPE.ISV:
                        // 费率转换为百分比数值
                        payInterfaceConfig.IfRate = payInterfaceConfig.IfRate * 100;
                        if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                        {
                            var isvParams = IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                            if (isvParams != null)
                            {
                                payInterfaceConfig.IfParams = isvParams.DeSenData();
                            }
                        }
                        break;
                    case CS.INFO_TYPE.MCH_APP:
                        // 费率转换为百分比数值
                        payInterfaceConfig.IfRate = payInterfaceConfig.IfRate * 100;

                        // 敏感数据脱敏
                        if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                        {
                            var mchApp = _mchAppService.GetById(infoId);
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
                        break;
                }
            }
            return ApiRes.Ok(payInterfaceConfig);
        }

        /// <summary>
        /// 支付接口参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("interfaceParams"), MethodLog("更新支付参数")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_CONFIG_ADD)]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            dto.IfRate = dto.IfRate / 100;// 存入真实费率
            //添加更新者信息
            long userId = GetCurrentUser().SysUser.SysUserId;
            string realName = GetCurrentUser().SysUser.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;
            dto.UpdatedAt = DateTime.Now;

            //根据 服务商号、接口类型 获取商户参数配置
            var dbRecoed = _payIfConfigService.GetByInfoIdAndIfCode(dto.InfoType, dto.InfoId, dto.IfCode);
            //若配置存在，为saveOrUpdate添加ID，第一次配置添加创建者
            if (dbRecoed != null)
            {
                dto.Id = dbRecoed.Id;
                dto.CreatedUid = dbRecoed.CreatedUid;
                dto.CreatedBy = dbRecoed.CreatedBy;
                dto.CreatedAt = dbRecoed.CreatedAt;
                // 合并支付参数
                dto.IfParams = StringUtil.Marge(dbRecoed.IfParams, dto.IfParams);
            }
            else
            {
                dto.CreatedUid = userId;
                dto.CreatedBy = realName;
                dto.CreatedAt = DateTime.Now;
            }
            var result = _payIfConfigService.SaveOrUpdate(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "配置失败");
            }

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, dto.InfoId, null, null));

            return ApiRes.Ok();
        }

        private static string GetInfoType(string configMode)
        {
            string infoType = string.Empty;
            switch (configMode)
            {
                case "mgrIsv":
                    infoType = CS.INFO_TYPE.ISV;
                    break;
                case "mgrAgent":
                case "agentSelf":
                case "agentSubagent":
                    infoType = CS.INFO_TYPE.AGENT;
                    break;
                case "mgrMch":
                case "agentMch":
                case "mchSelfApp1":
                case "mchSelfApp2":
                    infoType = CS.INFO_TYPE.MCH_APP;
                    break;
                default:
                    break;
            }

            return infoType;
        }
    }
}

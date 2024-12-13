using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付接口管理类
    /// </summary>
    [Route("api/payConfig")]
    [ApiController, Authorize]
    public class PayInterfaceConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IPayInterfaceDefineService _payIfDefineService;

        public PayInterfaceConfigController(ILogger<PayInterfaceConfigController> logger,
            IMQSender mqSender,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            IAgentInfoService agentInfoService,
            IPayInterfaceDefineService payIfDefineService,
            IPayInterfaceConfigService payIfConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            this.mqSender = mqSender;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _agentInfoService = agentInfoService;
            _payIfConfigService = payIfConfigService;
            _payIfDefineService = payIfDefineService;
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
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_LIST, PermCode.MGR.ENT_AGENT_PAY_CONFIG_LIST, PermCode.MGR.ENT_MCH_PAY_CONFIG_LIST)]
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
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_VIEW, PermCode.MGR.ENT_AGENT_PAY_CONFIG_VIEW, PermCode.MGR.ENT_MCH_PAY_CONFIG_VIEW)]
        public async Task<ApiRes> GetByInfoIdAsync(string configMode, string infoId, string ifCode)
        {
            string infoType = GetInfoType(configMode);
            var payInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(infoType, infoId, ifCode);
            var payIfDefine = await _payIfDefineService.GetByIdAsync(ifCode);
            payInterfaceConfig ??= new PayInterfaceConfigDto()
            {
                InfoType = infoType,
                InfoId = infoId,
                IfCode = ifCode,
                State = CS.YES
            };
            // 费率转换为百分比数值
            payInterfaceConfig.IfRate *= 100;
            List<byte> isSupportApplyments = new List<byte>() { payIfDefine.IsSupportApplyment };
            List<byte> isSupportCheckBills = new List<byte>() { payIfDefine.IsSupportCheckBill };
            List<byte> isSupportCashouts = new List<byte>() { payIfDefine.IsSupportCashout };
            switch (infoType)
            {
                case CS.INFO_TYPE.ISV:
                    if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                    {
                        var isvParams = IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                        if (isvParams != null)
                        {
                            payInterfaceConfig.IfParams = isvParams.DeSenData();
                        }
                    }
                    break;
                case CS.INFO_TYPE.AGENT:
                    var agentInfo = await _agentInfoService.GetByIdAsync(infoId);
                    var isvPayInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.ISV, agentInfo.IsvNo, ifCode);
                    isSupportApplyments.Add(isvPayInterfaceConfig.IsOpenApplyment);
                    isSupportCheckBills.Add(isvPayInterfaceConfig.IsOpenCheckBill);
                    isSupportCashouts.Add(isvPayInterfaceConfig.IsOpenCashout);
                    if (!string.IsNullOrEmpty(agentInfo.Pid))
                    {
                        var parentAgents = _agentInfoService.GetParents(agentInfo.Pid);
                        var agentNos = parentAgents.Select(o => o.AgentNo).ToList();
                        var agentPayInterfaceConfigs = _payIfConfigService.GetByInfoIdAndIfCodes(CS.INFO_TYPE.AGENT, agentNos, ifCode);
                        foreach (var item in agentPayInterfaceConfigs)
                        {
                            isSupportApplyments.Add(item.IsOpenApplyment);
                            isSupportCheckBills.Add(item.IsOpenCheckBill);
                            isSupportCashouts.Add(item.IsOpenCashout);
                        }
                    }
                    break;
                case CS.INFO_TYPE.MCH_APP:
                    var mchApp = await _mchAppService.GetByIdAsync(infoId);
                    var mchInfo = await _mchInfoService.GetByIdAsync(mchApp.MchNo);
                    if (!string.IsNullOrEmpty(mchInfo.IsvNo))
                    {
                        var mchIsvPayInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.ISV, mchInfo.IsvNo, ifCode);
                        isSupportApplyments.Add(mchIsvPayInterfaceConfig.IsOpenApplyment);
                        isSupportCheckBills.Add(mchIsvPayInterfaceConfig.IsOpenCheckBill);
                        isSupportCashouts.Add(mchIsvPayInterfaceConfig.IsOpenCashout);
                        if (!string.IsNullOrEmpty(mchInfo.AgentNo))
                        {
                            var mchParentAgents = _agentInfoService.GetParents(mchInfo.AgentNo);
                            var agentNos = mchParentAgents.Select(o => o.AgentNo).ToList();
                            var agentPayInterfaceConfigs = _payIfConfigService.GetByInfoIdAndIfCodes(CS.INFO_TYPE.AGENT, agentNos, ifCode);
                            foreach (var item in agentPayInterfaceConfigs)
                            {
                                isSupportApplyments.Add(item.IsOpenApplyment);
                                isSupportCheckBills.Add(item.IsOpenCheckBill);
                                isSupportCashouts.Add(item.IsOpenCashout);
                            }
                        }
                    }
                    // 敏感数据脱敏
                    if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                    {
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
            bool isSupportApplyment = isSupportApplyments.Any(a => a == CS.NO);
            bool isSupportCheckBill = isSupportCheckBills.Any(a => a == CS.NO);
            bool isSupportCashout = isSupportCashouts.Any(a => a == CS.NO);
            payInterfaceConfig.IsOpenApplyment = isSupportApplyment ? CS.NO : payInterfaceConfig.IsOpenApplyment;
            payInterfaceConfig.IsOpenCheckBill = isSupportCheckBill ? CS.NO : payInterfaceConfig.IsOpenCheckBill;
            payInterfaceConfig.IsOpenCashout = isSupportApplyment ? CS.NO : payInterfaceConfig.IsOpenCashout;
            //var result = JObject.FromObject(payInterfaceConfig);
            //result["isSupportApplyment"] = isSupportApplyment ? CS.NO : CS.YES;
            //result["isSupportCheckBill"] = isSupportCheckBill ? CS.NO : CS.YES;
            //result["isSupportCashout"] = isSupportCashout ? CS.NO : CS.YES;
            payInterfaceConfig.AddExt("isSupportApplyment", isSupportApplyment ? CS.NO : CS.YES);
            payInterfaceConfig.AddExt("isSupportCheckBill", isSupportCheckBill ? CS.NO : CS.YES);
            payInterfaceConfig.AddExt("isSupportCashout", isSupportCashout ? CS.NO : CS.YES);
            return ApiRes.Ok(payInterfaceConfig);
        }

        /// <summary>
        /// 支付接口参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("interfaceParams"), MethodLog("更新支付参数")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_ADD, PermCode.MGR.ENT_AGENT_PAY_CONFIG_ADD, PermCode.MGR.ENT_MCH_PAY_CONFIG_ADD)]
        public async Task<ApiRes> SaveOrUpdateAsync(PayInterfaceConfigDto dto)
        {
            dto.IfRate /= 100;// 存入真实费率
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
                dto.IfParams = StringUtil.Merge(dbRecoed.IfParams, dto.IfParams);
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
            await mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, dto.InfoId, null, null, null));

            return ApiRes.Ok();
        }

        private static string GetInfoType(string configMode)
        {
            var infoType = string.Empty;
            switch (configMode)
            {
                case CS.CONFIG_MODE.MGR_ISV:
                    infoType = CS.INFO_TYPE.ISV;
                    break;
                case CS.CONFIG_MODE.MGR_AGENT:
                case CS.CONFIG_MODE.AGENT_SELF:
                case CS.CONFIG_MODE.AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE.AGENT;
                    break;
                case CS.CONFIG_MODE.MGR_APPLYMENT:
                case CS.CONFIG_MODE.AGENT_APPLYMENT:
                case CS.CONFIG_MODE.MCH_APPLYMENT:
                    infoType = CS.INFO_TYPE.MCH;
                    break;
                case CS.CONFIG_MODE.MGR_MCH:
                case CS.CONFIG_MODE.AGENT_MCH:
                case CS.CONFIG_MODE.MCH_SELF_APP1:
                case CS.CONFIG_MODE.MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE.MCH_APP;
                    break;
                default:
                    break;
            }

            return infoType;
        }
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Application.DataTransfer.PayRateConfigSaveDto;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付费率配置记录表 服务实现类
    /// </summary>
    public class PayRateConfigService : AgPayService<PayRateConfigDto, PayRateConfig, long>, IPayRateConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IMchAppRepository _mchAppRepository;
        private readonly IPayWayRepository _payWayRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        private readonly IPayRateConfigRepository _payRateConfigRepository;
        private readonly IPayRateLevelConfigRepository _payRateLevelConfigRepository;
        // 注入工作单元
        private readonly IUnitOfWork _uow;

        public PayRateConfigService(IUnitOfWork uow, IMapper mapper, IMediatorHandler bus,
            IPayRateConfigRepository payRateConfigRepository,
            IPayRateLevelConfigRepository payRateLevelConfigRepository,
            IIsvInfoRepository isvInfoRepository,
            IAgentInfoRepository agentInfoRepository,
            IMchInfoRepository mchInfoRepository,
            IMchAppRepository mchAppRepository,
            IPayWayRepository payWayRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository)
            : base(mapper, bus, payRateConfigRepository)
        {
            _uow = uow;
            _payRateConfigRepository = payRateConfigRepository;
            _payRateLevelConfigRepository = payRateLevelConfigRepository;
            _isvInfoRepository = isvInfoRepository;
            _agentInfoRepository = agentInfoRepository;
            _mchInfoRepository = mchInfoRepository;
            _mchAppRepository = mchAppRepository;
            _payWayRepository = payWayRepository;
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
        }

        public async Task<PaginatedResult<PayWayDto>> GetPayWaysByInfoIdAsync(PayWayUsableQueryDto dto)
        {
            var infoType = string.Empty;
            var configMode = dto.ConfigMode;
            var wayCodes = new List<string>();

            var payIfDefine = await _payInterfaceDefineRepository.GetByIdAsync(dto.IfCode);
            var payIfWayCodes = JsonConvert.DeserializeObject<object[]>(payIfDefine.WayCodes).Select(obj => (string)((dynamic)obj).wayCode).ToList();

            switch (configMode)
            {
                case CS.CONFIG_MODE.MGR_ISV:
                    infoType = CS.INFO_TYPE.ISV;
                    wayCodes = payIfWayCodes;
                    break;
                case CS.CONFIG_MODE.MGR_AGENT:
                case CS.CONFIG_MODE.AGENT_SELF:
                case CS.CONFIG_MODE.AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE.AGENT;
                    var agent = await _agentInfoRepository.GetByIdAsync(dto.InfoId);
                    wayCodes = await GetPayWayCodesAsync(dto.IfCode, agent.IsvNo, agent.Pid, payIfWayCodes);
                    break;
                case CS.CONFIG_MODE.MGR_MCH:
                case CS.CONFIG_MODE.AGENT_MCH:
                case CS.CONFIG_MODE.MCH_SELF_APP1:
                case CS.CONFIG_MODE.MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE.MCH_APP;
                    var mchApp = await _mchAppRepository.GetByIdAsync(dto.InfoId);
                    var mchInfo = await _mchInfoRepository.GetByIdAsync(mchApp.MchNo);
                    wayCodes = payIfWayCodes;
                    if (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB))
                    {
                        wayCodes = await GetPayWayCodesAsync(dto.IfCode, mchInfo.IsvNo, mchInfo.AgentNo, payIfWayCodes);
                    }
                    break;
                default:
                    break;
            }
            var payWays = _payWayRepository.GetAllAsNoTracking().Where(w => wayCodes.Contains(w.WayCode))
                .OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            return await payWays.ToPaginatedResultAsync<PayWay, PayWayDto>(_mapper, dto.PageNumber, dto.PageSize);
        }

        private async Task<List<string>> GetPayWayCodesAsync(string ifCode, string isvNo, string agentNo, List<string> wayCodes)
        {
            // 服务商开通支付方式
            var isvWayCodes = await _payRateConfigRepository.GetByInfoIdAndIfCodeAsNoTracking(CS.CONFIG_TYPE.ISVCOST, CS.INFO_TYPE.ISV, isvNo, ifCode)
                .Where(w => wayCodes.Contains(w.WayCode)).Select(s => s.WayCode).Distinct().ToListAsync();
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                return await GetAgentPayWayCodesAsync(ifCode, isvWayCodes, agentNo);
            }
            return isvWayCodes;
        }

        private async Task<List<string>> GetAgentPayWayCodesAsync(string ifCode, List<string> wayCodes, string agentNo)
        {
            // 代理商开通支付方式
            var agentWayCodes = await _payRateConfigRepository.GetByInfoIdAndIfCodeAsNoTracking(CS.CONFIG_TYPE.AGENTRATE, CS.INFO_TYPE.AGENT, agentNo, ifCode)
                .Where(w => wayCodes.Contains(w.WayCode)).Select(s => s.WayCode).Distinct().ToListAsync();
            var agent = _agentInfoRepository.GetById(agentNo);
            if (!string.IsNullOrWhiteSpace(agent.Pid))
            {
                return await GetAgentPayWayCodesAsync(ifCode, agentWayCodes, agent.Pid);
            }
            else
            {
                return agentWayCodes;
            }
        }

        #region 丢弃
        [Obsolete("请使用 GetByInfoIdAndIfCodeJsonAsync() 代替")]
        public async Task<Dictionary<string, Dictionary<string, PayRateConfigDto>>> GetByInfoIdAndIfCodeAsync(string configMode, string infoId, string ifCode)
        {
            string isvNo;
            string infoType;
            Dictionary<string, Dictionary<string, PayRateConfigDto>> rateConfig = new Dictionary<string, Dictionary<string, PayRateConfigDto>>();
            switch (configMode)
            {
                case CS.CONFIG_MODE.MGR_ISV:
                    infoType = CS.INFO_TYPE.ISV;
                    rateConfig.Add(CS.CONFIG_TYPE.ISVCOST, await GetPayRateConfigAsync(CS.CONFIG_TYPE.ISVCOST, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE.AGENTDEF, await GetPayRateConfigAsync(CS.CONFIG_TYPE.AGENTDEF, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE.MCHAPPLYDEF, await GetPayRateConfigAsync(CS.CONFIG_TYPE.MCHAPPLYDEF, infoType, infoId, ifCode));
                    break;
                case CS.CONFIG_MODE.MGR_AGENT:
                case CS.CONFIG_MODE.AGENT_SELF:
                case CS.CONFIG_MODE.AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE.AGENT;
                    var agent = _agentInfoRepository.GetById(infoId);
                    rateConfig.Add(CS.CONFIG_TYPE.AGENTDEF, await GetPayRateConfigAsync(CS.CONFIG_TYPE.AGENTDEF, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE.AGENTRATE, await GetPayRateConfigAsync(CS.CONFIG_TYPE.AGENTRATE, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE.MCHAPPLYDEF, await GetPayRateConfigAsync(CS.CONFIG_TYPE.MCHAPPLYDEF, infoType, infoId, ifCode)); if (!configMode.Equals(CS.CONFIG_MODE.AGENT_SELF))
                    {
                        isvNo = configMode.Equals(CS.CONFIG_MODE.MGR_AGENT) ? agent.IsvNo : string.Empty;
                        await GetReadOnlyRateAsync(ifCode, rateConfig, isvNo, agent.Pid, CS.CONFIG_TYPE.AGENTDEF);
                    }
                    break;
                case CS.CONFIG_MODE.MGR_MCH:
                case CS.CONFIG_MODE.AGENT_MCH:
                case CS.CONFIG_MODE.MCH_SELF_APP1:
                case CS.CONFIG_MODE.MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE.MCH_APP;
                    var mchApp = _mchAppRepository.GetById(infoId);
                    var mchInfo = _mchInfoRepository.GetById(mchApp.MchNo);
                    rateConfig.Add(CS.CONFIG_TYPE.MCHRATE, await GetPayRateConfigAsync(CS.CONFIG_TYPE.MCHRATE, infoType, infoId, ifCode));
                    if (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB))
                    {
                        isvNo = configMode.Equals(CS.CONFIG_MODE.MGR_MCH) ? mchInfo.IsvNo : string.Empty;
                        await GetReadOnlyRateAsync(ifCode, rateConfig, isvNo, mchInfo.AgentNo, CS.CONFIG_TYPE.MCHAPPLYDEF);
                    }
                    break;
                default:
                    break;
            }
            return rateConfig;
        }

        [Obsolete("请使用 GetReadOnlyRateJsonAsync() 代替")]
        private async Task GetReadOnlyRateAsync(string ifCode, Dictionary<string, Dictionary<string, PayRateConfigDto>> rateConfig, string isvNo, string agentNo, string configType)
        {
            if (!string.IsNullOrWhiteSpace(isvNo))
            {
                // 服务商底价
                rateConfig.Add(CS.CONFIG_TYPE.READONLYISVCOST, await GetPayRateConfigAsync(CS.CONFIG_TYPE.ISVCOST, CS.INFO_TYPE.ISV, isvNo, ifCode));
            }
            // 上级代理商费率
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                // 上级代理商费率
                rateConfig.Add(CS.CONFIG_TYPE.READONLYPARENTAGENT, await GetPayRateConfigAsync(CS.CONFIG_TYPE.AGENTRATE, CS.INFO_TYPE.AGENT, agentNo, ifCode));
                // 上级默认费率
                rateConfig.Add(CS.CONFIG_TYPE.READONLYPARENTDEFRATE, await GetPayRateConfigAsync(configType, CS.INFO_TYPE.AGENT, agentNo, ifCode));
            }
            else
            {
                rateConfig.Add(CS.CONFIG_TYPE.READONLYPARENTDEFRATE, await GetPayRateConfigAsync(configType, CS.INFO_TYPE.ISV, isvNo, ifCode));
            }
        }

        [Obsolete("请使用 GetPayRateConfigJsonAsync() 代替")]
        private async Task<Dictionary<string, PayRateConfigDto>> GetPayRateConfigAsync(string configType, string infoType, string infoId, string ifCode)
        {
            Dictionary<string, PayRateConfigDto> keyValues = new Dictionary<string, PayRateConfigDto>();
            var payRateConfigs = await GetPayRateConfigsAsync(configType, infoType, infoId, ifCode);
            foreach (var payRateConfig in payRateConfigs)
            {
                keyValues.Add(payRateConfig.WayCode, payRateConfig);
            }
            return keyValues;
        }
        #endregion

        public async Task<JObject> GetByInfoIdAndIfCodeJsonAsync(string configMode, string infoId, string ifCode)
        {
            string isvNo;
            string infoType;
            JObject result = new JObject();
            switch (configMode)
            {
                case CS.CONFIG_MODE.MGR_ISV:
                    infoType = CS.INFO_TYPE.ISV;
                    result.Add(CS.CONFIG_TYPE.ISVCOST, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.ISVCOST, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE.AGENTDEF, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.AGENTDEF, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE.MCHAPPLYDEF, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.MCHAPPLYDEF, infoType, infoId, ifCode));
                    break;
                case CS.CONFIG_MODE.MGR_AGENT:
                case CS.CONFIG_MODE.AGENT_SELF:
                case CS.CONFIG_MODE.AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE.AGENT;
                    var agent = _agentInfoRepository.GetById(infoId);
                    result.Add(CS.CONFIG_TYPE.AGENTDEF, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.AGENTDEF, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE.AGENTRATE, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.AGENTRATE, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE.MCHAPPLYDEF, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.MCHAPPLYDEF, infoType, infoId, ifCode));
                    if (!configMode.Equals(CS.CONFIG_MODE.AGENT_SELF))
                    {
                        isvNo = configMode.Equals(CS.CONFIG_MODE.MGR_AGENT) ? agent.IsvNo : string.Empty;
                        await GetReadOnlyRateJsonAsync(ifCode, result, isvNo, agent.Pid, CS.CONFIG_TYPE.AGENTDEF);
                    }
                    break;
                case CS.CONFIG_MODE.MGR_MCH:
                case CS.CONFIG_MODE.AGENT_MCH:
                case CS.CONFIG_MODE.MCH_SELF_APP1:
                case CS.CONFIG_MODE.MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE.MCH_APP;
                    var mchApp = _mchAppRepository.GetById(infoId);
                    var mchInfo = _mchInfoRepository.GetById(mchApp.MchNo);
                    result.Add(CS.CONFIG_TYPE.MCHRATE, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.MCHRATE, infoType, infoId, ifCode));
                    if (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB))
                    {
                        isvNo = configMode.Equals(CS.CONFIG_MODE.MGR_MCH) ? mchInfo.IsvNo : string.Empty;
                        await GetReadOnlyRateJsonAsync(ifCode, result, isvNo, mchInfo.AgentNo, CS.CONFIG_TYPE.MCHAPPLYDEF);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        private async Task GetReadOnlyRateJsonAsync(string ifCode, JObject result, string isvNo, string agentNo, string configType)
        {
            if (!string.IsNullOrWhiteSpace(isvNo))
            {
                // 服务商底价
                result.Add(CS.CONFIG_TYPE.READONLYISVCOST, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.ISVCOST, CS.INFO_TYPE.ISV, isvNo, ifCode));
            }
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                // 上级代理商费率
                result.Add(CS.CONFIG_TYPE.READONLYPARENTAGENT, await GetPayRateConfigJsonAsync(CS.CONFIG_TYPE.AGENTRATE, CS.INFO_TYPE.AGENT, agentNo, ifCode));
                // 上级默认费率
                result.Add(CS.CONFIG_TYPE.READONLYPARENTDEFRATE, await GetPayRateConfigJsonAsync(configType, CS.INFO_TYPE.AGENT, agentNo, ifCode));
            }
            else
            {
                result.Add(CS.CONFIG_TYPE.READONLYPARENTDEFRATE, await GetPayRateConfigJsonAsync(configType, CS.INFO_TYPE.ISV, isvNo, ifCode));
            }
        }

        private async Task<JObject> GetPayRateConfigJsonAsync(string configType, string infoType, string infoId, string ifCode)
        {
            JObject result = new JObject();
            var payRateConfigs = await GetPayRateConfigsAsync(configType, infoType, infoId, ifCode);
            foreach (var item in payRateConfigs)
            {
                JObject payRateConfig = new JObject();
                payRateConfig.Add("wayCode", item.WayCode);
                payRateConfig.Add("state", item.State);
                payRateConfig.Add("feeType", item.FeeType);
                payRateConfig.Add("levelMode", item.LevelMode);
                payRateConfig.Add("applymentSupport", item.ApplymentSupport);
                if (item.FeeType.Equals(CS.FEE_TYPE_SINGLE))
                {
                    payRateConfig.Add("feeRate", item.FeeRate);
                }
                if (item.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                {
                    JArray array = new JArray();
                    foreach (var levelitem in item.PayRateLevelConfigs.GroupBy(g => g.BankCardType))
                    {
                        JObject payRateLevelConfig = new JObject();
                        payRateLevelConfig.Add("minFee", levelitem.Min(m => m.MinFee));
                        payRateLevelConfig.Add("maxFee", levelitem.Max(m => m.MaxFee));
                        if (string.IsNullOrWhiteSpace(levelitem.Key))
                        {
                            payRateLevelConfig.Add("bankCardType", levelitem.Key);
                        }
                        payRateLevelConfig.Add("levelList", JArray.FromObject(levelitem.Select(s => new
                        {
                            minAmount = s.MinAmount,
                            maxAmount = s.MaxAmount,
                            feeRate = s.FeeRate
                        })));
                        array.Add(payRateLevelConfig);
                    }
                    payRateConfig.Add(item.LevelMode, array);
                }
                result.Add(item.WayCode, payRateConfig);
            }
            return result;
        }

        private async Task<List<PayRateConfigItem>> GetPayRateConfigItemsAsync(string configType, string infoType, string infoId, string ifCode)
        {
            var payRateConfigs = await GetPayRateConfigsAsync(configType, infoType, infoId, ifCode);
            var result = payRateConfigs.Select(item =>
            {
                var r = new PayRateConfigItem()
                {
                    WayCode = item.WayCode,
                    State = item.State,
                    FeeType = item.FeeType,
                    LevelMode = item.LevelMode,
                    ApplymentSupport = item.ApplymentSupport,
                    FeeRate = item.FeeRate,
                };
                r.NORMAL = item.PayRateLevelConfigs.Where(w => string.IsNullOrEmpty(w.BankCardType)).ToList()
                .GroupBy(g => g.BankCardType)
                .Select(s => new Levels()
                {
                    MinFee = s.Min(m => m.MinFee),
                    MaxFee = s.Min(m => m.MaxFee),
                    BankCardType = s.Key,
                    LevelList = s.Select(l => new LevelList
                    {
                        MinAmount = l.MinAmount,
                        MaxAmount = l.MaxAmount,
                        FeeRate = l.FeeRate
                    }).ToList()
                }).ToList();
                r.UNIONPAY = item.PayRateLevelConfigs.Where(w => !string.IsNullOrEmpty(w.BankCardType)).ToList()
                .GroupBy(g => g.BankCardType)
                .Select(s => new Levels()
                {
                    MinFee = s.Min(m => m.MinFee),
                    MaxFee = s.Min(m => m.MaxFee),
                    BankCardType = s.Key,
                    LevelList = s.Select(l => new LevelList
                    {
                        MinAmount = l.MinAmount,
                        MaxAmount = l.MaxAmount,
                        FeeRate = l.FeeRate
                    }).ToList()
                }).ToList();
                return r;
            }).ToList();

            return result;
        }

        private async Task<List<PayRateConfigDto>> GetPayRateConfigsAsync(string configType, string infoType, string infoId, string ifCode)
        {
            var payRateConfigs = await _payRateConfigRepository.GetByInfoIdAndIfCodeAsNoTracking(configType, infoType, infoId, ifCode)
                .ToProjectedListAsync<PayRateConfig, PayRateConfigDto>(_mapper);
            var rateConfigIds = payRateConfigs.Select(s => s.Id).ToList();
            var payRateLevelConfigs = await _payRateLevelConfigRepository.GetByRateConfigIdsAsNoTracking(rateConfigIds)
                .ToProjectedListAsync<PayRateLevelConfig, PayRateLevelConfigDto>(_mapper);
            foreach (var item in payRateConfigs)
            {
                item.PayRateLevelConfigs = payRateLevelConfigs.Where(w => w.RateConfigId == item.Id).ToList();
            }

            return payRateConfigs;
        }

        public async Task<PayRateConfigItem> GetPayRateConfigItemAsync(string configType, string infoType, string infoId, string ifCode, string wayCode)
        {
            var payRateConfig = await _payRateConfigRepository.GetByUniqueKeyAsNoTrackingAsync(configType, infoType, infoId, ifCode, wayCode);

            var payRateLevelConfigs = _payRateLevelConfigRepository.GetByRateConfigIdAsNoTracking(payRateConfig.Id);

            var result = new PayRateConfigItem()
            {
                WayCode = payRateConfig.WayCode,
                State = payRateConfig.State,
                FeeType = payRateConfig.FeeType,
                LevelMode = payRateConfig.LevelMode,
                ApplymentSupport = payRateConfig.ApplymentSupport,
                FeeRate = payRateConfig.FeeRate,
            };
            result.NORMAL = payRateLevelConfigs.Where(w => string.IsNullOrEmpty(w.BankCardType)).ToList()
                .GroupBy(g => g.BankCardType)
                .Select(s => new Levels()
                {
                    MinFee = s.Min(m => m.MinFee),
                    MaxFee = s.Min(m => m.MaxFee),
                    BankCardType = s.Key,
                    LevelList = s.Select(l => new LevelList
                    {
                        MinAmount = l.MinAmount,
                        MaxAmount = l.MaxAmount,
                        FeeRate = l.FeeRate
                    }).ToList()
                }).ToList();
            result.UNIONPAY = payRateLevelConfigs.Where(w => !string.IsNullOrEmpty(w.BankCardType)).ToList()
                .GroupBy(g => g.BankCardType)
                .Select(s => new Levels()
                {
                    MinFee = s.Min(m => m.MinFee),
                    MaxFee = s.Min(m => m.MaxFee),
                    BankCardType = s.Key,
                    LevelList = s.Select(l => new LevelList
                    {
                        MinAmount = l.MinAmount,
                        MaxAmount = l.MaxAmount,
                        FeeRate = l.FeeRate
                    }).ToList()
                }).ToList();
            return result;
        }

        public async Task<bool> SaveOrUpdateAsync(PayRateConfigSaveDto dto)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                var checkResult = await PayRateConfigCheckAsync(dto);
                if (!checkResult.IsPassed)
                {
                    throw new BizException(checkResult.Message);
                }
                var infoId = dto.InfoId;
                var ifCode = dto.IfCode;
                var configMode = dto.ConfigMode;
                var delPayWayCodes = dto.DelPayWayCodes;
                var infoType = string.Empty;
                switch (configMode)
                {
                    case CS.CONFIG_MODE.MGR_ISV:
                        infoType = CS.INFO_TYPE.ISV;
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.ISVCOST, infoType, delPayWayCodes, dto.ISVCOST);
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.AGENTDEF, infoType, delPayWayCodes, dto.AGENTDEF);
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.MCHAPPLYDEF, infoType, delPayWayCodes, dto.MCHAPPLYDEF);
                        break;
                    case CS.CONFIG_MODE.MGR_AGENT:
                    case CS.CONFIG_MODE.AGENT_SELF:
                    case CS.CONFIG_MODE.AGENT_SUBAGENT:
                        infoType = CS.INFO_TYPE.AGENT;
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.AGENTRATE, infoType, delPayWayCodes, dto.AGENTRATE);
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.AGENTDEF, infoType, delPayWayCodes, dto.AGENTDEF);
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.MCHAPPLYDEF, infoType, delPayWayCodes, dto.MCHAPPLYDEF);
                        break;
                    case CS.CONFIG_MODE.MGR_MCH:
                    case CS.CONFIG_MODE.AGENT_MCH:
                    case CS.CONFIG_MODE.MCH_SELF_APP1:
                    case CS.CONFIG_MODE.MCH_SELF_APP2:
                        infoType = CS.INFO_TYPE.MCH_APP;
                        await SaveOrUpdateAsync(infoId, ifCode, CS.CONFIG_TYPE.MCHRATE, infoType, delPayWayCodes, dto.MCHRATE);
                        break;
                    default:
                        break;
                }
                await _uow.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }

            return true;
        }

        private async Task SaveOrUpdateAsync(string infoId, string ifCode, string configType, string infoType, List<string> delPayWayCodes, List<PayRateConfigItem> items)
        {
            var now = DateTime.Now;
            await DelPayWayCodeRateConfigAsync(infoId, ifCode, configType, infoType, delPayWayCodes);
            foreach (var item in items)
            {
                var entity = _payRateConfigRepository.GetByUniqueKey(configType, infoType, infoId, ifCode, item.WayCode);
                if (entity == null)
                {
                    entity = new PayRateConfig
                    {
                        ConfigType = configType,
                        InfoType = infoType,
                        InfoId = infoId,
                        IfCode = ifCode,
                        WayCode = item.WayCode,
                        FeeType = item.FeeType,
                        LevelMode = item.LevelMode,
                        FeeRate = item.FeeRate,
                        ApplymentSupport = item.ApplymentSupport,
                        State = item.State,
                        CreatedAt = now,
                        UpdatedAt = now,
                    };
                    await _payRateConfigRepository.AddAsync(entity);
                }
                else
                {
                    var payRateLevelConfigs = _payRateLevelConfigRepository.GetByRateConfigId(entity.Id);
                    _payRateLevelConfigRepository.RemoveRange(payRateLevelConfigs);

                    entity.FeeType = item.FeeType;
                    entity.LevelMode = item.LevelMode;
                    entity.FeeRate = item.FeeRate;
                    entity.ApplymentSupport = item.ApplymentSupport;
                    entity.State = item.State;
                    entity.UpdatedAt = now;
                    _payRateConfigRepository.Update(entity);
                }
                await _payRateConfigRepository.SaveChangesAsync();

                if (item.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                {
                    var payRateLevelConfigs = new List<PayRateLevelConfig>();
                    foreach (var level in (item.LevelMode.Equals(CS.LEVEL_MODE_NORMAL) ? item.NORMAL : item.UNIONPAY))
                    {
                        foreach (var levelitem in level.LevelList)
                        {
                            var payRateLevelConfig = new PayRateLevelConfig
                            {
                                RateConfigId = entity.Id,
                                BankCardType = level.BankCardType,
                                MinFee = level.MinFee,
                                MaxFee = level.MaxFee,
                                MinAmount = levelitem.MinAmount,
                                MaxAmount = levelitem.MaxAmount,
                                FeeRate = levelitem.FeeRate,
                                State = item.State,
                                CreatedAt = now,
                                UpdatedAt = now,
                            };
                            payRateLevelConfigs.Add(payRateLevelConfig);
                        }
                    }
                    await _payRateLevelConfigRepository.AddRangeAsync(payRateLevelConfigs);
                }

                await _payRateLevelConfigRepository.SaveChangesAsync();
            }
        }

        private async Task DelPayWayCodeRateConfigAsync(string infoId, string ifCode, string configType, string infoType, List<string> delPayWayCodes)
        {
            var payRateConfigs = _payRateConfigRepository.GetByInfoIdAndIfCode(configType, infoType, infoId, ifCode).Where(w => delPayWayCodes.Contains(w.WayCode));
            await DelPayWayCodeRateConfigAsync(payRateConfigs);
        }

        private async Task DelPayWayCodeRateConfigAsync(IQueryable<PayRateConfig> payRateConfigs)
        {
            var ids = payRateConfigs.Select(s => s.Id).ToList();
            var payRateLevelConfigs = _payRateLevelConfigRepository.GetByRateConfigIds(ids);
            _payRateLevelConfigRepository.RemoveRange(payRateLevelConfigs);
            await _payRateLevelConfigRepository.SaveChangesAsync();

            _payRateConfigRepository.RemoveRange(payRateConfigs);
            await _payRateConfigRepository.SaveChangesAsync();
        }

        private async Task<(bool IsPassed, string Message)> PayRateConfigCheckAsync(PayRateConfigSaveDto dto)
        {
            var infoId = dto.InfoId;
            var ifCode = dto.IfCode;
            var configMode = dto.ConfigMode;
            List<PayRateConfigItem> ISVCOST = null, AGENTDEF = null, MCHAPPLYDEF = null, AGENTRATE = null, MCHRATE = null, PARENTRATE = null;
            switch (configMode)
            {
                case CS.CONFIG_MODE.MGR_ISV:
                    var isv = await _isvInfoRepository.GetByIdAsNoTrackingAsync(infoId);
                    if (isv == null || isv.State != CS.YES)
                    {
                        return (false, "服务商不存在");
                    }
                    ISVCOST = dto.ISVCOST; // 服务商底价费率
                    AGENTDEF = dto.AGENTDEF; // 代理商默认费率
                    MCHAPPLYDEF = dto.MCHAPPLYDEF; // 商户进件默认费率
                    for (int i = 0; i < ISVCOST.Count; i++)
                    {
                        var mainFee = ISVCOST[i];
                        var wayCode = mainFee.WayCode;
                        var agentRateConfigs = GetAgentRateConfigByIsvNo(infoId, ifCode, wayCode);
                        if (agentRateConfigs.Any() && !agentRateConfigs.Any(a => a.FeeType.Equals(mainFee.FeeType)))
                        {
                            if (dto.NoCheckRuleFlag.Equals(CS.YES))
                            {
                                await DelPayWayCodeRateConfigAsync(agentRateConfigs);
                            }
                            else
                            {
                                return (false, $"[{wayCode}]的费率计算方式与[代理费率]的配置不一致");
                            }
                        }
                        var mchRateConfigs = GetMchRateConfigByIsvNo(infoId, ifCode, wayCode);
                        if (mchRateConfigs.Any() && !mchRateConfigs.Any(a => a.FeeType.Equals(mainFee.FeeType)))
                        {
                            if (dto.NoCheckRuleFlag.Equals(CS.YES))
                            {
                                await DelPayWayCodeRateConfigAsync(mchRateConfigs);
                            }
                            else
                            {
                                return (false, $"[{wayCode}]的费率计算方式与[商户费率]的配置不一致");
                            }
                        }
                        if (dto.NoCheckRuleFlag.Equals(CS.YES))
                        {
                            continue;
                        }
                        if (mainFee.FeeType.Equals(CS.FEE_TYPE_SINGLE))
                        {
                            var isvCostFeeRate = mainFee.FeeRate;
                            var agentDefFeeRate = GetFeeRate(AGENTDEF, wayCode);
                            var mchApplyDefFeeRate = GetFeeRate(MCHAPPLYDEF, wayCode);
                            if (isvCostFeeRate == null || agentDefFeeRate == null || mchApplyDefFeeRate == null)
                            {
                                return (false, $"[{wayCode}]的费率不可以空");
                            }
                            if (agentDefFeeRate < isvCostFeeRate)
                            {
                                //return (false, $"代理商默认费率异常： [{wayCode}]设置费率{{{(agentDefFeeRate * 100)}%}} 需要【大于等于】【服务商底价费率】的配置值：{{{(isvCostFeeRate * 100)}%}}");
                                return (false, GetFeeRateErrorMessage("代理商默认费率", "服务商底价费率", wayCode, agentDefFeeRate.Value, isvCostFeeRate.Value));
                            }
                            if (mchApplyDefFeeRate < agentDefFeeRate)
                            {
                                //return (false, $"商户进件默认费率异常： [{wayCode}]的设置费率{{{(mchApplyDefFeeRate * 100)}%}} 需要【大于等于】【代理商默认费率】的配置值：{{{(agentDefFeeRate * 100)}%}}");
                                return (false, GetFeeRateErrorMessage("商户进件默认费率", "代理商默认费率", wayCode, mchApplyDefFeeRate.Value, agentDefFeeRate.Value));
                            }
                        }
                        if (mainFee.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                        {
                            var levels = (mainFee.LevelMode.Equals(CS.LEVEL_MODE_NORMAL) ? mainFee.NORMAL : mainFee.UNIONPAY);
                            for (int j = 0; j < levels.Count; j++)
                            {
                                var level = levels[j];
                                var bankCardType = level.BankCardType;
                                for (int k = 0; k < level.LevelList.Count; k++)
                                {
                                    var isvCostFeeRate = level.LevelList[k].FeeRate;
                                    var agentDefFeeRate = GetFeeRate(AGENTDEF, wayCode, bankCardType, k);
                                    var mchApplyDefFeeRate = GetFeeRate(MCHAPPLYDEF, wayCode, bankCardType, k);
                                    var modeName = (mainFee.LevelMode.Equals(CS.LEVEL_MODE_UNIONPAY) ? (level.BankCardType.Equals(CS.BANK_CARD_TYPE_DEBIT) ? "借记卡" : (level.BankCardType.Equals(CS.BANK_CARD_TYPE_CREDIT) ? "贷记卡" : "")) : "");
                                    if (isvCostFeeRate == null || agentDefFeeRate == null || mchApplyDefFeeRate == null)
                                    {
                                        return (false, $"[{wayCode}]的阶梯费率不可以空");
                                    }
                                    if (agentDefFeeRate < isvCostFeeRate)
                                    {
                                        //return (false, $"代理商默认费率异常： [{wayCode}]{modeName}的第[{k}]阶梯设置费率{{{(agentDefFeeRate * 100)}%}} 需要【大于等于】【服务商底价费率】的阶梯配置值：{{{(isvCostFeeRate * 100)}%}}");
                                        return (false, GetFeeRateErrorMessage("代理商默认费率", "服务商底价费率", wayCode, agentDefFeeRate.Value, isvCostFeeRate.Value, modeName, k));
                                    }
                                    if (mchApplyDefFeeRate < agentDefFeeRate)
                                    {
                                        //return (false, $"商户进件默认费率异常： [{wayCode}]{modeName}的第[{k}]阶梯设置费率{{{(mchApplyDefFeeRate * 100)}%}} 需要【大于等于】【代理商默认费率】的阶梯配置值：{{{(agentDefFeeRate * 100)}%}}");
                                        return (false, GetFeeRateErrorMessage("商户进件默认费率", "代理商默认费率", wayCode, mchApplyDefFeeRate.Value, agentDefFeeRate.Value, modeName, k));
                                    }
                                }
                            }
                        }
                    }
                    break;
                case CS.CONFIG_MODE.MGR_AGENT:
                case CS.CONFIG_MODE.AGENT_SELF:
                case CS.CONFIG_MODE.AGENT_SUBAGENT:
                    var agent = await _agentInfoRepository.GetByIdAsNoTrackingAsync(infoId);
                    if (agent == null || agent.State != CS.YES)
                    {
                        return (false, "代理商不存在");
                    }
                    AGENTRATE = dto.AGENTRATE; // 当前代理商费率
                    AGENTDEF = dto.AGENTDEF; // 下级代理商默认费率
                    MCHAPPLYDEF = dto.MCHAPPLYDEF; // 代理商子商户进件默认
                    PARENTRATE = await GetParentRateAsync(ifCode, agent.IsvNo, agent.Pid, CS.CONFIG_TYPE.AGENTDEF);
                    for (int i = 0; i < AGENTRATE.Count; i++)
                    {
                        var mainFee = AGENTRATE[i];
                        var wayCode = mainFee.WayCode;
                        if (dto.NoCheckRuleFlag.Equals(CS.YES))
                        {
                            continue;
                        }
                        if (!PARENTRATE.FirstOrDefault(f => f.WayCode.Equals(wayCode)).FeeType.Equals(mainFee.FeeType))
                        {
                            return (false, $"[{wayCode}]的费率计算方式与[服务商底价]的配置不一致");
                        }
                        if (mainFee.FeeType.Equals(CS.FEE_TYPE_SINGLE))
                        {
                            var parentFeeRate = GetFeeRate(PARENTRATE, wayCode);
                            var agentRateFeeRate = mainFee.FeeRate;
                            var agentDefFeeRate = GetFeeRate(AGENTDEF, wayCode);
                            var mchApplyDefFeeRate = GetFeeRate(MCHAPPLYDEF, wayCode);
                            if (parentFeeRate == null || agentRateFeeRate == null || agentDefFeeRate == null || mchApplyDefFeeRate == null)
                            {
                                return (false, $"[{wayCode}]的费率不可以空");
                            }
                            if (agentRateFeeRate < parentFeeRate)
                            {
                                // return (false, $"代理商费率异常： [{wayCode}]设置费率{{{(agentDefFeeRate * 100)}%}} 需要【大于等于】【{(string.IsNullOrWhiteSpace(agent.Pid) ? "服务商底价" : "上级代理商费率")}费率】的配置值：{{{(parentFeeRate * 100)}%}}");
                                var thanName = string.IsNullOrWhiteSpace(agent.Pid) ? "服务商底价" : "上级代理商费率";
                                return (false, GetFeeRateErrorMessage("代理商费率", thanName, wayCode, agentRateFeeRate.Value, parentFeeRate.Value));
                            }
                            if (agentDefFeeRate < agentRateFeeRate)
                            {
                                //return (false, $"代理商默认费率异常： [{wayCode}]设置费率{{{(agentDefFeeRate * 100)}%}} 需要【大于等于】【代理商费率】的配置值：{{{(agentRateFeeRate * 100)}%}}");
                                return (false, GetFeeRateErrorMessage("代理商默认费率", "代理商费率", wayCode, agentDefFeeRate.Value, agentRateFeeRate.Value));
                            }
                            if (mchApplyDefFeeRate < agentDefFeeRate)
                            {
                                //return (false, $"商户进件默认费率异常： [{wayCode}]的设置费率{{{(mchApplyDefFeeRate * 100)}%}} 需要【大于等于】【代理商默认费率】的配置值：{{{(agentDefFeeRate * 100)}%}}");
                                return (false, GetFeeRateErrorMessage("商户进件默认费率", "代理商默认费率", wayCode, mchApplyDefFeeRate.Value, agentDefFeeRate.Value));
                            }
                        }
                        if (mainFee.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                        {
                            var levels = (mainFee.LevelMode.Equals(CS.LEVEL_MODE_NORMAL) ? mainFee.NORMAL : mainFee.UNIONPAY);
                            for (int j = 0; j < levels.Count; j++)
                            {
                                var level = levels[j];
                                var bankCardType = level.BankCardType;
                                for (int k = 0; k < level.LevelList.Count; k++)
                                {
                                    var parentFeeRate = GetFeeRate(PARENTRATE, wayCode, bankCardType, k);
                                    var agentRateFeeRate = level.LevelList[k].FeeRate;
                                    var agentDefFeeRate = GetFeeRate(AGENTDEF, wayCode, bankCardType, k);
                                    var mchApplyDefFeeRate = GetFeeRate(AGENTDEF, wayCode, bankCardType, k);
                                    var modeName = (mainFee.LevelMode.Equals(CS.LEVEL_MODE_UNIONPAY) ? (level.BankCardType.Equals(CS.BANK_CARD_TYPE_DEBIT) ? "借记卡" : (level.BankCardType.Equals(CS.BANK_CARD_TYPE_CREDIT) ? "贷记卡" : "")) : "");
                                    if (parentFeeRate == null || agentRateFeeRate == null || agentDefFeeRate == null || mchApplyDefFeeRate == null)
                                    {
                                        return (false, $"[{wayCode}]的阶梯费率不可以空");
                                    }
                                    if (agentRateFeeRate < parentFeeRate)
                                    {
                                        //return (false, $"代理商费率异常： [{wayCode}]{modeName}的第[{k}]阶梯设置费率{{{(agentRateFeeRate * 100)}%}} 需要【大于等于】【{(string.IsNullOrWhiteSpace(agent.Pid) ? "服务商底价" : "上级代理商费率")}】的阶梯配置值：{{{(parentFeeRate * 100)}%}}");
                                        var thanName = string.IsNullOrWhiteSpace(agent.Pid) ? "服务商底价" : "上级代理商费率";
                                        return (false, GetFeeRateErrorMessage("代理商费率", thanName, wayCode, agentRateFeeRate.Value, parentFeeRate.Value, modeName, k));
                                    }
                                    if (agentDefFeeRate < agentRateFeeRate)
                                    {
                                        //return (false, $"代理商默认费率异常： [{wayCode}]{modeName}的第[{k}]阶梯设置费率{{{(agentDefFeeRate * 100)}%}} 需要【大于等于】【代理商费率】的阶梯配置值：{{{(agentRateFeeRate * 100)}%}}");
                                        return (false, GetFeeRateErrorMessage("代理商默认费率", "代理商费率", wayCode, agentDefFeeRate.Value, agentRateFeeRate.Value, modeName, k));
                                    }
                                    if (mchApplyDefFeeRate < agentDefFeeRate)
                                    {
                                        //return (false, $"商户进件默认费率异常： [{wayCode}]{modeName}的第[{k}]阶梯的设置费率{{{(mchApplyDefFeeRate * 100)}%}} 需要【大于等于】【代理商默认费率】的阶梯配置值：{{{(agentDefFeeRate * 100)}%}}");
                                        return (false, GetFeeRateErrorMessage("商户进件默认费率", "代理商默认费率", wayCode, mchApplyDefFeeRate.Value, agentDefFeeRate.Value, modeName, k));
                                    }
                                }
                            }
                        }
                    }
                    break;
                case CS.CONFIG_MODE.MGR_MCH:
                case CS.CONFIG_MODE.AGENT_MCH:
                case CS.CONFIG_MODE.MCH_SELF_APP1:
                case CS.CONFIG_MODE.MCH_SELF_APP2:
                    var mchApp = await _mchAppRepository.GetByIdAsNoTrackingAsync(infoId);
                    if (mchApp == null || mchApp.State != CS.YES)
                    {
                        return (false, "商户应用不存在");
                    }
                    var mchInfo = _mchInfoRepository.GetById(mchApp.MchNo);
                    if (mchInfo == null || mchInfo.State != CS.YES)
                    {
                        return (false, "商户不存在");
                    }
                    MCHRATE = dto.MCHRATE; // 商户费率
                    if (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB))
                    {
                        PARENTRATE = await GetParentRateAsync(ifCode, mchInfo.IsvNo, mchInfo.AgentNo, CS.CONFIG_TYPE.MCHAPPLYDEF);
                        for (int i = 0; i < MCHRATE.Count; i++)
                        {
                            var mainFee = MCHRATE[i];
                            var wayCode = mainFee.WayCode;
                            if (dto.NoCheckRuleFlag.Equals(CS.YES))
                            {
                                continue;
                            }
                            if (!PARENTRATE.FirstOrDefault(f => f.WayCode.Equals(wayCode)).FeeType.Equals(mainFee.FeeType))
                            {
                                return (false, $"[{wayCode}]的费率计算方式与[服务商底价]的配置不一致");
                            }
                            if (mainFee.FeeType.Equals(CS.FEE_TYPE_SINGLE))
                            {
                                var parentFeeRate = GetFeeRate(PARENTRATE, wayCode);
                                var mchRateFeeRate = mainFee.FeeRate;
                                if (parentFeeRate == null || mchRateFeeRate == null)
                                {
                                    return (false, $"[{wayCode}]的费率不可以空");
                                }
                                if (mchRateFeeRate < parentFeeRate)
                                {
                                    //return (false, $"商家费率异常： [{wayCode}]设置费率{{{(mchRateFeeRate * 100)}%}} 需要【大于等于】【{(string.IsNullOrWhiteSpace(mchInfo.AgentNo) ? "服务商底价" : "上级代理商费率")}】的配置值：{{{(parentFeeRate * 100)}%}}");
                                    var thanName = string.IsNullOrWhiteSpace(mchInfo.AgentNo) ? "服务商底价" : "上级代理商费率";
                                    return (false, GetFeeRateErrorMessage("商家费率", thanName, wayCode, mchRateFeeRate.Value, parentFeeRate.Value));
                                }
                            }
                            if (mainFee.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                            {
                                var levels = (mainFee.LevelMode.Equals(CS.LEVEL_MODE_NORMAL) ? mainFee.NORMAL : mainFee.UNIONPAY);
                                for (int j = 0; j < levels.Count; j++)
                                {
                                    var level = levels[j];
                                    var bankCardType = level.BankCardType;
                                    var modeName = (mainFee.LevelMode.Equals(CS.LEVEL_MODE_UNIONPAY) ? (level.BankCardType.Equals(CS.BANK_CARD_TYPE_DEBIT) ? "借记卡" : (level.BankCardType.Equals(CS.BANK_CARD_TYPE_CREDIT) ? "贷记卡" : "")) : "");
                                    for (int k = 0; k < level.LevelList.Count; k++)
                                    {
                                        var parentFeeRate = GetFeeRate(PARENTRATE, wayCode, bankCardType, k);
                                        var mchRateFeeRate = level.LevelList[k].FeeRate;
                                        if (parentFeeRate == null || mchRateFeeRate == null)
                                        {
                                            return (false, $"[{wayCode}]的阶梯费率不可以空");
                                        }
                                        if (mchRateFeeRate < parentFeeRate)
                                        {
                                            //return (false, $"代理商费率异常： [{wayCode}]{modeName}的第[{k}]阶梯设置费率{{{(mchRateFeeRate * 100)}%}} 需要【大于等于】【上级底价费率】的阶梯配置值：{{{(parentFeeRate * 100)}%}}");
                                            return (false, GetFeeRateErrorMessage("代理商费率", "上级底价费率", wayCode, mchRateFeeRate.Value, parentFeeRate.Value, modeName, k));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return (true, string.Empty);
        }

        private IQueryable<PayRateConfig> GetAgentRateConfigByIsvNo(string isvNo, string ifCode, string wayCode)
        {
            var agentInfos = _agentInfoRepository.GetAllAsNoTracking()
                .Where(w => w.IsvNo.Equals(isvNo));
            var payRateConfigs = _payRateConfigRepository.GetAllAsNoTracking()
                .Where(w => w.ConfigType.Equals(CS.CONFIG_TYPE.AGENTRATE) && w.InfoType.Equals(CS.INFO_TYPE.AGENT) && w.IfCode.Equals(ifCode) && w.WayCode.Equals(wayCode));

            return payRateConfigs.Join(agentInfos, r => r.InfoId, a => a.AgentNo, (r, a) => r);
        }

        private IQueryable<PayRateConfig> GetMchRateConfigByIsvNo(string isvNo, string ifCode, string wayCode)
        {
            var mchInfos = _mchInfoRepository.GetAllAsNoTracking()
                .Where(w => w.IsvNo.Equals(isvNo));
            var payRateConfigs = _payRateConfigRepository.GetAllAsNoTracking()
                .Where(w => w.ConfigType.Equals(CS.CONFIG_TYPE.AGENTRATE) && w.InfoType.Equals(CS.INFO_TYPE.AGENT) && w.IfCode.Equals(ifCode) && w.WayCode.Equals(wayCode));

            return payRateConfigs.Join(mchInfos, r => r.InfoId, a => a.MchNo, (r, a) => r);
        }

        private static string GetFeeRateErrorMessage(string name, string thanName, string wayCode, decimal feeRate, decimal thanFeeRate, string modeName = "", int? levelIndex = null)
        {
            return $"{name}异常： [{wayCode}]{(levelIndex == null ? "" : $"{modeName}的第[{++levelIndex}]阶梯")}设置费率{{{(feeRate * 100):F4}%}} 需要【大于等于】【{thanName}】的{(levelIndex == null ? "" : "阶梯")}配置值：{{{(thanFeeRate * 100):F4}%}}";
        }

        private async Task<List<PayRateConfigItem>> GetParentRateAsync(string ifCode, string isvNo, string agentNo, string configType)
        {
            List<PayRateConfigItem> ISVCOST, READONLYPARENTAGENT = null, READONLYPARENTDEFRATE = null, PARENTRATE;

            // 服务商底价
            ISVCOST = await GetPayRateConfigItemsAsync(CS.CONFIG_TYPE.ISVCOST, CS.INFO_TYPE.ISV, isvNo, ifCode);
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                // 上级代理商费率
                READONLYPARENTAGENT = await GetPayRateConfigItemsAsync(CS.CONFIG_TYPE.AGENTRATE, CS.INFO_TYPE.AGENT, agentNo, ifCode);
                // 上级默认费率
                READONLYPARENTDEFRATE = await GetPayRateConfigItemsAsync(configType, CS.INFO_TYPE.AGENT, agentNo, ifCode);
            }
            //else
            //{
            //    READONLYPARENTDEFRATE = GetPayRateConfigItems(configType, CS.INFO_TYPE.ISV, isvNo, ifCode);
            //}
            PARENTRATE = READONLYPARENTDEFRATE ?? READONLYPARENTAGENT ?? ISVCOST;

            return PARENTRATE;
        }

        private static decimal? GetFeeRate(List<PayRateConfigItem> configItem, string wayCode, string bankCardType = null, int? levelIndex = null)
        {
            decimal? feeRate = null;
            if (configItem == null || !configItem.Any(w => w.WayCode.Equals(wayCode)))
            {
                return feeRate;
            }
            var isvcost = configItem.FirstOrDefault(f => f.WayCode.Equals(wayCode));
            if (isvcost.FeeType.Equals(CS.FEE_TYPE_SINGLE))
            {
                feeRate = isvcost.FeeRate;
            }
            if (isvcost.FeeType.Equals(CS.FEE_TYPE_LEVEL) && levelIndex != null)
            {
                var levels = (isvcost.LevelMode.Equals(CS.LEVEL_MODE_NORMAL) ? isvcost.NORMAL : isvcost.UNIONPAY);
                var level = levels.FirstOrDefault(f => f.BankCardType == bankCardType);
                feeRate = level.LevelList[levelIndex.Value].FeeRate;
            }
            return feeRate;
        }

        public async Task<List<PayRateConfigInfoDto>> GetPayRateConfigInfosAsync(string mchNo, string ifCode, string wayCode, long amount, string bankCardType = null)
        {
            var mchInfo = await _mchInfoRepository.GetByIdAsNoTrackingAsync(mchNo);
            if (mchInfo == null || mchInfo.Type.Equals(CS.MCH_TYPE_NORMAL))
            {
                return null;
            }
            var isvPayRateConfig = await _payRateConfigRepository.GetByUniqueKeyAsNoTrackingAsync(CS.CONFIG_TYPE.ISVCOST, CS.INFO_TYPE.ISV, mchInfo.IsvNo, ifCode, wayCode);
            var agentInfos = _agentInfoRepository.GetParentAgentsFromSqlAsNoTracking(mchInfo.AgentNo);
            var infoType = CS.INFO_TYPE.AGENT;
            var configType = CS.CONFIG_TYPE.AGENTRATE;
            var infoIds = agentInfos.Select(s => s.AgentNo).ToList();
            var payRateConfigs = _payRateConfigRepository.GetByUniqueKeysAsNoTracking(configType, infoType, ifCode, wayCode, infoIds).ToList();
            payRateConfigs.Add(isvPayRateConfig);
            var ids = payRateConfigs.Select(s => s.Id).ToList();
            var payRateLevelConfigs = _payRateLevelConfigRepository.GetByRateConfigIdsAsNoTracking(ids);

            var result = _mapper.Map<List<PayRateConfigInfoDto>>(payRateConfigs);
            foreach (var payRateConfig in result)
            {
                var agentInfo = agentInfos.FirstOrDefault(f => f.AgentNo.Equals(payRateConfig.InfoId));
                payRateConfig.InfoName = agentInfo?.AgentName;
                payRateConfig.AgentLevel = agentInfo?.Level;
                if (payRateConfig.FeeType.Equals(CS.FEE_TYPE_SINGLE))
                {
                    payRateConfig.FeeRateDesc = $"单笔费率：{payRateConfig.FeeRate.Value * 100:F4}%";
                }
                if (payRateConfig.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                {
                    var _payRateLevelConfigs = payRateLevelConfigs.Where(s => s.RateConfigId.Equals(payRateConfig.Id));
                    PayRateLevelConfig payRateLevelConfig = null;
                    if (payRateConfig.LevelMode.Equals(CS.LEVEL_MODE_NORMAL))
                    {
                        payRateLevelConfig = await _payRateLevelConfigs.FirstOrDefaultAsync(w => string.IsNullOrEmpty(w.BankCardType) && w.MinAmount < amount && w.MaxAmount <= amount);
                    }

                    if (payRateConfig.LevelMode.Equals(CS.LEVEL_MODE_UNIONPAY))
                    {
                        payRateLevelConfig = await _payRateLevelConfigs.FirstOrDefaultAsync(w => w.BankCardType.Equals(bankCardType) && w.MinAmount < amount && w.MaxAmount <= amount);
                    }

                    if (payRateLevelConfig == null)
                    {
                        return null;
                    }
                    var rate = payRateLevelConfig.FeeRate.Value;
                    var modeName = (payRateConfig.LevelMode.Equals(CS.LEVEL_MODE_UNIONPAY) ? (payRateLevelConfig.BankCardType.Equals(CS.BANK_CARD_TYPE_DEBIT) ? "借记卡" : (payRateLevelConfig.BankCardType.Equals(CS.BANK_CARD_TYPE_CREDIT) ? "贷记卡" : "")) : "阶梯");
                    var rateDesc = $"({payRateLevelConfig.MinAmount / 100M:F2}元-{payRateLevelConfig.MaxAmount / 100M:F2}元]{modeName}费率: {rate * 100:F4}%, 保底{payRateLevelConfig.MinFee / 100M:F2}元, 封顶{payRateLevelConfig.MaxFee / 100M:F2}元";

                    payRateConfig.FeeRate = rate;
                    payRateConfig.FeeRateDesc = rateDesc;
                }
            }

            return result;
        }
    }
}

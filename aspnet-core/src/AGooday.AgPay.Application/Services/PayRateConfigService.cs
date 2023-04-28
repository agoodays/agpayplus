using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    public class PayRateConfigService : IPayRateConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IPayWayRepository _payWayRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        private readonly IPayRateConfigRepository _payRateConfigRepository;
        private readonly ILevelRateConfigRepository _levelRateConfigRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayRateConfigService(IMapper mapper, IMediatorHandler bus,
            IPayRateConfigRepository payRateConfigRepository,
            ILevelRateConfigRepository levelRateConfigRepository,
            IIsvInfoRepository isvInfoRepository,
            IAgentInfoRepository agentInfoRepository,
            IMchInfoRepository mchInfoRepository,
            IPayWayRepository payWayRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payRateConfigRepository = payRateConfigRepository;
            _levelRateConfigRepository = levelRateConfigRepository;
            _isvInfoRepository = isvInfoRepository;
            _agentInfoRepository = agentInfoRepository;
            _mchInfoRepository = mchInfoRepository;
            _payWayRepository = payWayRepository;
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
        }

        public PaginatedList<PayWayDto> GetPayWaysByInfoId(PayWayUsableQueryDto dto)
        {
            string infoType = string.Empty;
            var wayCodes = new List<string>();
            switch (dto.ConfigMode)
            {
                case CS.CONFIG_MODE_MGR_ISV:
                    infoType = CS.INFO_TYPE_ISV;
                    var payIfDefine = _payInterfaceDefineRepository.GetById(dto.IfCode);
                    wayCodes = JsonConvert.DeserializeObject<object[]>(payIfDefine.WayCodes).Select(obj => (string)((dynamic)obj).wayCode).ToList();
                    break;
                case CS.CONFIG_MODE_MGR_AGENT:
                case CS.CONFIG_MODE_AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE_AGENT;
                    var agent = _agentInfoRepository.GetById(dto.InfoId);
                    wayCodes = GetPayWayCodes(dto.IfCode, agent.IsvNo, agent.Pid).ToList();
                    break;
                case CS.CONFIG_MODE_MGR_MCH:
                case CS.CONFIG_MODE_AGENT_MCH:
                case CS.CONFIG_MODE_AGENT_SELF:
                case CS.CONFIG_MODE_MCH_SELF_APP1:
                case CS.CONFIG_MODE_MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE_MCH_APP;
                    var mchInfo = _mchInfoRepository.GetById(dto.InfoId);
                    wayCodes = GetPayWayCodes(dto.IfCode, mchInfo.IsvNo, mchInfo.AgentNo).ToList();
                    break;
                default:
                    break;
            }
            var payWays = _payWayRepository.GetAll().Where(w => wayCodes.Contains(w.WayCode))
                .OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayWay>.Create<PayWayDto>(payWays.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        private IQueryable<string> GetPayWayCodes(string ifCode, string isvNo, string agentNo)
        {
            // 服务商开通支付方式
            var isvWayCodes = _payRateConfigRepository.GetByInfoIdAndIfCode(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, isvNo, ifCode)
                .Select(s => s.WayCode).Distinct();
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                // 代理商开通支付方式
                var agentWayCodes = _payRateConfigRepository.GetByInfoIdAndIfCode(CS.CONFIG_TYPE_AGENTRATE, CS.INFO_TYPE_AGENT, agentNo, ifCode)
                    .Where(w => isvWayCodes.Contains(w.WayCode)).Select(s => s.WayCode).Distinct();
                return agentWayCodes;
            }
            return isvWayCodes;
        }

        public Dictionary<string, Dictionary<string, PayRateConfigDto>> GetByInfoIdAndIfCode(string configMode, string infoId, string ifCode)
        {
            string infoType = string.Empty;
            Dictionary<string, Dictionary<string, PayRateConfigDto>> rateConfig = new Dictionary<string, Dictionary<string, PayRateConfigDto>>();
            switch (configMode)
            {
                case CS.CONFIG_MODE_MGR_ISV:
                    infoType = CS.INFO_TYPE_ISV;
                    rateConfig.Add(CS.CONFIG_TYPE_ISVCOST, GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfig(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfig(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    break;
                case CS.CONFIG_MODE_MGR_AGENT:
                case CS.CONFIG_MODE_AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE_AGENT;
                    var agent = _agentInfoRepository.GetById(infoId);
                    rateConfig.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfig(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_AGENTRATE, GetPayRateConfig(CS.CONFIG_TYPE_AGENTRATE, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfig(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    GetReadOnlyRate(ifCode, rateConfig, agent.IsvNo, agent.Pid);
                    break;
                case CS.CONFIG_MODE_MGR_MCH:
                case CS.CONFIG_MODE_AGENT_MCH:
                case CS.CONFIG_MODE_AGENT_SELF:
                case CS.CONFIG_MODE_MCH_SELF_APP1:
                case CS.CONFIG_MODE_MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE_MCH_APP;
                    var mchInfo = _mchInfoRepository.GetById(infoId);
                    rateConfig.Add(CS.CONFIG_TYPE_MCHRATE, GetPayRateConfig(CS.CONFIG_TYPE_MCHRATE, infoType, infoId, ifCode));
                    GetReadOnlyRate(ifCode, rateConfig, mchInfo.IsvNo, mchInfo.AgentNo);
                    break;
                default:
                    break;
            }
            return rateConfig;
        }

        private void GetReadOnlyRate(string ifCode, Dictionary<string, Dictionary<string, PayRateConfigDto>> rateConfig, string isvNo, string agentNo)
        {
            // 服务商底价
            rateConfig.Add(CS.CONFIG_TYPE_READONLYISVCOST, GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, isvNo, ifCode));

            // 上级代理商费率
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                rateConfig.Add(CS.CONFIG_TYPE_READONLYPARENTAGENT, GetPayRateConfig(CS.CONFIG_TYPE_AGENTRATE, CS.INFO_TYPE_AGENT, agentNo, ifCode));

                var parentAgent = _agentInfoRepository.GetById(agentNo);
                // 上级默认费率
                if (!string.IsNullOrWhiteSpace(parentAgent.Pid))
                {
                    rateConfig.Add(CS.CONFIG_TYPE_READONLYPARENTDEFRATE, GetPayRateConfig(CS.CONFIG_TYPE_AGENTDEF, CS.INFO_TYPE_AGENT, parentAgent.Pid, ifCode));
                }
                else
                {
                    rateConfig.Add(CS.CONFIG_TYPE_READONLYPARENTDEFRATE, GetPayRateConfig(CS.CONFIG_TYPE_AGENTDEF, CS.INFO_TYPE_ISV, parentAgent.IsvNo, ifCode));
                }
            }
        }

        private Dictionary<string, PayRateConfigDto> GetPayRateConfig(string configType, string infoType, string infoId, string ifCode)
        {
            Dictionary<string, PayRateConfigDto> keyValues = new Dictionary<string, PayRateConfigDto>();
            var payRateConfigs = GetPayRateConfigs(configType, infoType, infoId, ifCode);
            foreach (var payRateConfig in payRateConfigs)
            {
                keyValues.Add(payRateConfig.WayCode, payRateConfig);
            }
            return keyValues;
        }

        public JObject GetByInfoIdAndIfCodeJson(string configMode, string infoId, string ifCode)
        {
            JObject result = new JObject();
            string infoType = string.Empty;
            switch (configMode)
            {
                case CS.CONFIG_MODE_MGR_ISV:
                    infoType = CS.INFO_TYPE_ISV;
                    result.Add(CS.CONFIG_TYPE_ISVCOST, GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    break;
                case CS.CONFIG_MODE_MGR_AGENT:
                case CS.CONFIG_MODE_AGENT_SUBAGENT:
                    infoType = CS.INFO_TYPE_AGENT;
                    var agent = _agentInfoRepository.GetById(infoId);
                    result.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_AGENTRATE, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTRATE, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    GetReadOnlyRateJson(ifCode, result, agent.IsvNo, agent.Pid);
                    break;
                case CS.CONFIG_MODE_MGR_MCH:
                case CS.CONFIG_MODE_AGENT_MCH:
                case CS.CONFIG_MODE_AGENT_SELF:
                case CS.CONFIG_MODE_MCH_SELF_APP1:
                case CS.CONFIG_MODE_MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE_MCH_APP;
                    var mchInfo = _mchInfoRepository.GetById(infoId);
                    result.Add(CS.CONFIG_TYPE_MCHRATE, GetPayRateConfigJson(CS.CONFIG_TYPE_MCHRATE, infoType, infoId, ifCode));
                    GetReadOnlyRateJson(ifCode, result, mchInfo.IsvNo, mchInfo.AgentNo);
                    break;
                default:
                    break;
            }
            return result;
        }

        private void GetReadOnlyRateJson(string ifCode, JObject result, string isvNo, string agentNo)
        {
            // 服务商底价
            result.Add(CS.CONFIG_TYPE_READONLYISVCOST, GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, isvNo, ifCode));

            // 上级代理商费率
            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                result.Add(CS.CONFIG_TYPE_READONLYPARENTAGENT, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTRATE, CS.INFO_TYPE_AGENT, agentNo, ifCode));

                var parentAgent = _agentInfoRepository.GetById(agentNo);
                // 上级默认费率
                if (!string.IsNullOrWhiteSpace(parentAgent.Pid))
                {
                    result.Add(CS.CONFIG_TYPE_READONLYPARENTDEFRATE, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTDEF, CS.INFO_TYPE_AGENT, parentAgent.Pid, ifCode));
                }
                else
                {
                    result.Add(CS.CONFIG_TYPE_READONLYPARENTDEFRATE, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTDEF, CS.INFO_TYPE_ISV, parentAgent.IsvNo, ifCode));
                }
            }
        }

        private JObject GetPayRateConfigJson(string configType, string infoType, string infoId, string ifCode)
        {
            JObject result = new JObject();
            var payRateConfigs = GetPayRateConfigs(configType, infoType, infoId, ifCode);
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
                    foreach (var levelitem in item.LevelRateConfigs.GroupBy(g => g.BankCardType))
                    {
                        JObject levelRateConfig = new JObject();
                        levelRateConfig.Add("minFee", levelitem.Min(m => m.MinFee));
                        levelRateConfig.Add("maxFee", levelitem.Max(m => m.MaxFee));
                        if (string.IsNullOrWhiteSpace(levelitem.Key))
                        {
                            levelRateConfig.Add("bankCardType", levelitem.Key);
                        }
                        levelRateConfig.Add("levelList", JArray.FromObject(levelitem.Select(s => new
                        {
                            minAmount = s.MinAmount,
                            maxAmount = s.MaxAmount,
                            feeRate = s.FeeRate
                        })));
                        array.Add(levelRateConfig);
                    }
                    payRateConfig.Add(item.LevelMode, array);
                }
                result.Add(item.WayCode, payRateConfig);
            }
            return result;
        }

        public List<PayRateConfigDto> GetPayRateConfigs(string configType, string infoType, string infoId, string ifCode)
        {
            var payRateConfigs = _payRateConfigRepository.GetByInfoIdAndIfCode(configType, infoType, infoId, ifCode);
            var result = _mapper.Map<List<PayRateConfigDto>>(payRateConfigs);
            foreach (var item in result)
            {
                var levelRateConfigs = _levelRateConfigRepository.GetByRateConfigId(item.Id);
                item.LevelRateConfigs = _mapper.Map<List<LevelRateConfigDto>>(levelRateConfigs);
            }

            return result;
        }

        public bool SaveOrUpdate(PayRateConfigSaveDto dto)
        {
            switch (dto.ConfigMode)
            {
                case CS.CONFIG_MODE_MGR_ISV:
                    string infoId = dto.InfoId;
                    var ifCode = dto.IfCode;
                    var delPayWayCodes = dto.DelPayWayCodes;
                    var infoType = CS.INFO_TYPE_ISV;
                    var configType = CS.CONFIG_TYPE_ISVCOST;
                    var items = dto.ISVCOST;
                    SaveOrUpdate(infoId, ifCode, configType, infoType, delPayWayCodes, items);
                    configType = CS.CONFIG_TYPE_AGENTDEF;
                    items = dto.AGENTDEF;
                    SaveOrUpdate(infoId, ifCode, configType, infoType, delPayWayCodes, items);
                    configType = CS.CONFIG_TYPE_MCHAPPLYDEF;
                    items = dto.MCHAPPLYDEF;
                    SaveOrUpdate(infoId, ifCode, configType, infoType, delPayWayCodes, items);
                    break;
                default:
                    break;
            }

            return true;
        }

        private void SaveOrUpdate(string infoId, string ifCode, string configType, string infoType, List<string> delPayWayCodes, List<PayRateConfigSaveDto.PayRateConfigItem> items)
        {
            var now = DateTime.Now;
            DelPayWayCodeRateConfig(configType, infoType, infoId, ifCode, delPayWayCodes);
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
                    _payRateConfigRepository.Add(entity);
                }
                else
                {
                    var levelRateConfigs = _levelRateConfigRepository.GetByRateConfigId(entity.Id);
                    foreach (var levelRateConfig in levelRateConfigs)
                    {
                        _levelRateConfigRepository.Remove(levelRateConfig.Id);
                    }

                    entity.FeeType = item.FeeType;
                    entity.LevelMode = item.LevelMode;
                    entity.FeeRate = item.FeeRate;
                    entity.ApplymentSupport = item.ApplymentSupport;
                    entity.State = item.State;
                    entity.UpdatedAt = now;
                    _payRateConfigRepository.Update(entity);
                }
                _payRateConfigRepository.SaveChanges();

                if (item.FeeType.Equals(CS.FEE_TYPE_LEVEL))
                {
                    foreach (var level in (item.LevelMode.Equals(CS.LEVEL_MODE_NORMAL) ? item.NORMAL : item.UNIONPAY))
                    {
                        foreach (var levelitem in level.LevelList)
                        {
                            var levelRateConfig = new LevelRateConfig
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
                            _levelRateConfigRepository.Add(levelRateConfig);
                        }
                    }
                }

                _levelRateConfigRepository.SaveChanges();
            }
        }

        private void DelPayWayCodeRateConfig(string infoId, string ifCode, string configType, string infoType, List<string> delPayWayCodes)
        {
            foreach (var wayCode in delPayWayCodes)
            {
                var entity = _payRateConfigRepository.GetByUniqueKey(configType, infoType, infoId, ifCode, wayCode);
                if (entity != null)
                {
                    _payRateConfigRepository.Remove(entity.Id);

                    _levelRateConfigRepository.SaveChanges();

                    var levelRateConfigs = _levelRateConfigRepository.GetByRateConfigId(entity.Id);
                    foreach (var levelRateConfig in levelRateConfigs)
                    {
                        _levelRateConfigRepository.Remove(levelRateConfig.Id);
                    }

                    _levelRateConfigRepository.SaveChanges();
                }
            }
        }
    }
}

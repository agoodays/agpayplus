using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    public class PayRateConfigService : IPayRateConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayRateConfigRepository _payRateConfigRepository;
        private readonly ILevelRateConfigRepository _levelRateConfigRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayRateConfigService(IMapper mapper, IMediatorHandler bus,
            IPayRateConfigRepository payRateConfigRepository,
             ILevelRateConfigRepository levelRateConfigRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payRateConfigRepository = payRateConfigRepository;
            _levelRateConfigRepository = levelRateConfigRepository;
        }

        public Dictionary<string, Dictionary<string, PayRateConfigDto>> GetByInfoIdAndIfCode(string configMode, string infoId, string ifCode)
        {
            string infoType = string.Empty;
            Dictionary<string, Dictionary<string, PayRateConfigDto>> rateConfig = new Dictionary<string, Dictionary<string, PayRateConfigDto>>();
            switch (configMode)
            {
                case "mgrIsv":
                    infoType = CS.INFO_TYPE_ISV;
                    rateConfig.Add(CS.CONFIG_TYPE_ISVCOST, GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfig(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfig(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    break;
                case "mgrAgent":
                case "agentSubagent":
                    infoType = CS.INFO_TYPE_AGENT;
                    rateConfig.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfig(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_AGENTRATE, GetPayRateConfig(CS.CONFIG_TYPE_AGENTRATE, infoType, infoId, ifCode));
                    rateConfig.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfig(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    // 服务商底价
                    rateConfig.Add("READONLYISVCOST", GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, infoId, ifCode));
                    // 上级代理商费率
                    rateConfig.Add("READONLYPARENTAGENT", GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    // 上级默认费率
                    rateConfig.Add("READONLYPARENTDEFRATE", GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    break;
                case "mgrMch":
                case "agentMch":
                case "agentSelf":
                case "mchSelfApp1":
                case "mchSelfApp2":
                    infoType = CS.INFO_TYPE_MCH_APP;
                    rateConfig.Add(CS.CONFIG_TYPE_MCHRATE, GetPayRateConfig(CS.CONFIG_TYPE_MCHRATE, infoType, infoId, ifCode));
                    // 服务商底价
                    rateConfig.Add("READONLYISVCOST", GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, infoId, ifCode));
                    // 上级代理商费率
                    rateConfig.Add("READONLYPARENTAGENT", GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    // 上级默认费率
                    rateConfig.Add("READONLYPARENTDEFRATE", GetPayRateConfig(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    break;
                default:
                    break;
            }
            return rateConfig;
        }

        public Dictionary<string, PayRateConfigDto> GetPayRateConfig(string configType, string infoType, string infoId, string ifCode)
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
                case "mgrIsv":
                    infoType = CS.INFO_TYPE_ISV;
                    result.Add(CS.CONFIG_TYPE_ISVCOST, GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    break;
                case "mgrAgent":
                case "agentSubagent":
                    infoType = CS.INFO_TYPE_AGENT;
                    result.Add(CS.CONFIG_TYPE_AGENTDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTDEF, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_AGENTRATE, GetPayRateConfigJson(CS.CONFIG_TYPE_AGENTRATE, infoType, infoId, ifCode));
                    result.Add(CS.CONFIG_TYPE_MCHAPPLYDEF, GetPayRateConfigJson(CS.CONFIG_TYPE_MCHAPPLYDEF, infoType, infoId, ifCode));
                    // 服务商底价
                    result.Add("READONLYISVCOST", GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, infoId, ifCode));
                    // 上级代理商费率
                    result.Add("READONLYPARENTAGENT", GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    // 上级默认费率
                    result.Add("READONLYPARENTDEFRATE", GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    break;
                case "mgrMch":
                case "agentMch":
                case "agentSelf":
                case "mchSelfApp1":
                case "mchSelfApp2":
                    infoType = CS.INFO_TYPE_MCH_APP;
                    result.Add(CS.CONFIG_TYPE_MCHRATE, GetPayRateConfigJson(CS.CONFIG_TYPE_MCHRATE, infoType, infoId, ifCode));
                    // 服务商底价
                    result.Add("READONLYISVCOST", GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_ISV, infoId, ifCode));
                    // 上级代理商费率
                    result.Add("READONLYPARENTAGENT", GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    // 上级默认费率
                    result.Add("READONLYPARENTDEFRATE", GetPayRateConfigJson(CS.CONFIG_TYPE_ISVCOST, CS.INFO_TYPE_AGENT, infoId, ifCode));
                    break;
                default:
                    break;
            }
            return result;
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
    }
}

using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付接口配置参数表 服务实现类
    /// </summary>
    public class PayInterfaceConfigService : AgPayService<PayInterfaceConfigDto, PayInterfaceConfig, long>, IPayInterfaceConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;

        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        private readonly IMchAppRepository _mchAppRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;

        public PayInterfaceConfigService(IMapper mapper, IMediatorHandler bus,
            IPayInterfaceConfigRepository payInterfaceConfigRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository,
            IMchAppRepository mchAppRepository,
            IMchInfoRepository mchInfoRepository,
            IAgentInfoRepository agentInfoRepository)
            : base(mapper, bus, payInterfaceConfigRepository)
        {
            _payInterfaceConfigRepository = payInterfaceConfigRepository;

            _payInterfaceDefineRepository = payInterfaceDefineRepository;
            _mchAppRepository = mchAppRepository;
            _mchInfoRepository = mchInfoRepository;
            _agentInfoRepository = agentInfoRepository;
        }

        public override async Task<bool> AddAsync(PayInterfaceConfigDto dto)
        {
            var entity = _mapper.Map<PayInterfaceConfig>(dto);
            await _payInterfaceConfigRepository.AddAsync(entity);
            var (result, _) = await _payInterfaceConfigRepository.SaveChangesWithResultAsync();
            dto.Id = entity.Id;
            return result;
        }

        public Task<bool> IsExistUseIfCodeAsync(string ifCode)
        {
            return _payInterfaceConfigRepository.IsExistUseIfCodeAsync(ifCode);
        }

        public async Task<bool> SaveOrUpdateAsync(PayInterfaceConfigDto dto)
        {
            var entity = _mapper.Map<PayInterfaceConfig>(dto);
            await _payInterfaceConfigRepository.AddOrUpdateAsync(entity);
            var (result, _) = await _payInterfaceConfigRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<bool> RemoveAsync(string infoType, string infoId)
        {
            var records = await _payInterfaceConfigRepository.GetAll()
                .Where(w => w.InfoId.Equals(infoId) && w.InfoType.Equals(infoType)).FirstOrDefaultAsync();
            _payInterfaceConfigRepository.Remove(records);
            var (result, _) = await _payInterfaceConfigRepository.SaveChangesWithResultAsync();
            return result;
        }

        /// <summary>
        /// 根据 账户类型、账户号、接口类型 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <param name="ifCode">接口类型</param>
        /// <returns></returns>
        public async Task<PayInterfaceConfigDto> GetByInfoIdAndIfCodeAsync(string infoType, string infoId, string ifCode)
        {
            var records = await _payInterfaceConfigRepository.GetAllAsNoTracking().Where(w => w.InfoId.Equals(infoId)
            && w.InfoType.Equals(infoType) && w.IfCode.Equals(ifCode)).FirstOrDefaultAsync();
            return _mapper.Map<PayInterfaceConfigDto>(records);
        }

        public IEnumerable<PayInterfaceConfigDto> GetByInfoIdAndIfCodes(string infoType, List<string> infoIds, string ifCode)
        {
            var records = _payInterfaceConfigRepository.GetAllAsNoTracking().Where(w => infoIds.Contains(w.InfoId)
            && w.InfoType.Equals(infoType) && w.IfCode.Equals(ifCode));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(records);
        }

        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <returns></returns>
        public IEnumerable<PayInterfaceConfigDto> GetByInfoId(string infoType, string infoId)
        {
            var records = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.Equals(infoId)
                && w.InfoType.Equals(infoType) && w.State.Equals(CS.PUB_USABLE));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(records);
        }

        public IEnumerable<PayInterfaceConfigDto> GetPayOauth2ConfigByStartsWithInfoId(string infoType, string infoId)
        {
            var records = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.StartsWith(infoId)
                && w.InfoType.Equals(infoType) && w.State.Equals(CS.PUB_USABLE));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(records);
        }

        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置列表
        /// </summary>
        /// <param name="infoType"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PayInterfaceDefineDto>> SelectAllPayIfConfigListByIsvNoAsync(string infoType, string infoId)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.IsIsvMode.Equals(CS.YES) && w.State.Equals(CS.YES));
            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

            var result = (await defineList.ToListAsync()).Select(s =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(s);
                entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(s.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                return entity;
            });

            return result;
        }

        public async Task<List<PayInterfaceDefineDto>> PayIfConfigListAsyncOld(string infoType, string configMode, string infoId, string ifName, string ifCode)
        {
            bool isApplyment = configMode.EndsWith("Applyment", StringComparison.OrdinalIgnoreCase);
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES)
                && (string.IsNullOrWhiteSpace(ifName) || w.IfName.Contains(ifName))
                && (string.IsNullOrWhiteSpace(ifCode) || w.IfCode.Equals(ifCode))
                && (!isApplyment || (isApplyment && w.IsSupportApplyment.Equals(CS.YES) && w.IsOpenApplyment.Equals(CS.YES))));

            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

            AgentInfo agentInfo; IQueryable<string> ifCodes; IQueryable<PayInterfaceConfig> isvConfigList;
            switch (infoType)
            {
                case CS.INFO_TYPE.ISV:
                    defineList = defineList.Where(w => w.IsIsvMode.Equals(CS.YES));
                    break;
                case CS.INFO_TYPE.MCH:
                case CS.INFO_TYPE.MCH_APP:
                    var mchNo = infoId;
                    if (infoType.Equals(CS.INFO_TYPE.MCH_APP))
                    {
                        MchApp mchApp = await _mchAppRepository.GetByIdAsync(infoId);
                        if (mchApp == null || mchApp.State != CS.YES)
                        {
                            throw new BizException("商户应用不存在");
                        }
                        mchNo = mchApp.MchNo;
                    }
                    MchInfo mchInfo = await _mchInfoRepository.GetByIdAsync(mchNo);
                    if (mchInfo == null || mchInfo.State != CS.YES)
                    {
                        throw new BizException("商户不存在");
                    }
                    defineList = defineList.Where(w => ((mchInfo.Type.Equals(CS.MCH_TYPE_NORMAL) && w.IsMchMode.Equals(CS.YES))// 支持普通商户模式
                    || (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && w.IsIsvMode.Equals(CS.YES)))// 支持服务商模式
                    );

                    var isvPayConfigMap = new Dictionary<string, PayInterfaceConfigDto>();// 服务商支付参数配置集合
                    if (mchInfo.Type == CS.MCH_TYPE_ISVSUB)
                    {
                        // 商户类型为特约商户，服务商应已经配置支付参数
                        isvConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                            .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams)
                            && (!isApplyment || (isApplyment && w.IsOpenApplyment.Equals(CS.YES))));

                        if (!string.IsNullOrEmpty(mchInfo.AgentNo))
                        {
                            agentInfo = await _agentInfoRepository.GetByIdAsync(mchInfo.AgentNo);
                            ifCodes = GetAgentIfCodes(agentInfo, isApplyment);
                            isvConfigList = isvConfigList.Where(s => ifCodes.Contains(s.IfCode));
                        }

                        foreach (var isvConfig in isvConfigList)
                        {
                            var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                            config.MchType = mchInfo.Type;
                            isvPayConfigMap.Add(config.IfCode, config);
                        }
                    }
                    var results = defineList.ToList()
                        .Where(w => mchInfo.Type != CS.MCH_TYPE_ISVSUB || (mchInfo.Type == CS.MCH_TYPE_ISVSUB && isvPayConfigMap.TryGetValue(w.IfCode, out _)))
                        .Select(define =>
                        {
                            var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                            entity.AddExt("mchType", mchInfo.Type);// 所属商户类型
                            entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(define.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                            return entity;
                        }).ToList();
                    return results;
                case CS.INFO_TYPE.AGENT:
                    agentInfo = await _agentInfoRepository.GetByIdAsync(infoId);
                    if (agentInfo == null || agentInfo.State != CS.YES)
                    {
                        throw new BizException("代理商不存在");
                    }
                    // 商户类型为特约商户，服务商应已经配置支付参数
                    isvConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                        .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(agentInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams));
                    if (configMode.Equals(CS.CONFIG_MODE.AGENT_SELF) || !string.IsNullOrEmpty(agentInfo.Pid))
                    {
                        ifCodes = GetAgentIfCodes(agentInfo, isApplyment);
                        isvConfigList = isvConfigList.Where(s => ifCodes.Contains(s.IfCode));
                    }
                    defineList = defineList.Where(w => w.IsIsvMode.Equals(CS.YES) && isvConfigList.Select(s => s.IfCode).Contains(w.IfCode));
                    break;
                default:
                    break;
            }
            var result = (await defineList.ToListAsync()).Select(s =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(s);
                entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(s.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                return entity;
            }).ToList();

            return result;
        }

        public async Task<List<PayInterfaceDefineDto>> PayIfConfigListAsync(string infoType, string configMode, string infoId, string ifName, string ifCode)
        {
            bool isApplyment = configMode.EndsWith("Applyment", StringComparison.OrdinalIgnoreCase);

            // 构建基础支付定义查询
            var defineQuery = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES)
                && (string.IsNullOrWhiteSpace(ifName) || w.IfName.Contains(ifName))
                && (string.IsNullOrWhiteSpace(ifCode) || w.IfCode.Equals(ifCode))
                && (!isApplyment || (isApplyment && w.IsSupportApplyment.Equals(CS.YES) && w.IsOpenApplyment.Equals(CS.YES))));

            // 构建支付参数查询
            var configQuery = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

            switch (infoType)
            {
                case CS.INFO_TYPE.ISV:
                    defineQuery = defineQuery.Where(w => w.IsIsvMode.Equals(CS.YES));
                    break;

                case CS.INFO_TYPE.MCH:
                case CS.INFO_TYPE.MCH_APP:
                    return await ProcessMchConfigAsync(infoType, infoId, isApplyment, defineQuery, configQuery);

                case CS.INFO_TYPE.AGENT:
                    return await ProcessAgentConfigAsync(infoId, configMode, isApplyment, defineQuery, configQuery);
            }

            // 默认处理逻辑
            var configIfCodes = await configQuery.Where(c => c.State.Equals(CS.YES))
                .Select(c => c.IfCode)
                .Distinct()
                .ToListAsync();

            var defineList = await defineQuery.ToListAsync();

            return defineList.Select(define =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                entity.AddExt("ifConfigState", configIfCodes.Contains(define.IfCode) ? CS.YES : null);
                return entity;
            }).ToList();
        }

        private async Task<List<PayInterfaceDefineDto>> ProcessMchConfigAsync(
            string infoType, string infoId, bool isApplyment,
            IQueryable<PayInterfaceDefine> defineQuery,
            IQueryable<PayInterfaceConfig> configQuery)
        {
            string mchNo = infoId;

            // 如果是应用ID，先获取商户号
            if (infoType == CS.INFO_TYPE.MCH_APP)
            {
                var mchApp = await _mchAppRepository.GetByIdAsync(infoId);
                if (mchApp == null || mchApp.State != CS.YES)
                {
                    throw new BizException("商户应用不存在");
                }
                mchNo = mchApp.MchNo;
            }

            // 获取商户信息
            var mchInfo = await _mchInfoRepository.GetByIdAsync(mchNo);
            if (mchInfo == null || mchInfo.State != CS.YES)
            {
                throw new BizException("商户不存在");
            }

            // 根据商户类型过滤支付定义
            defineQuery = defineQuery.Where(w =>
                (mchInfo.Type.Equals(CS.MCH_TYPE_NORMAL) && w.IsMchMode.Equals(CS.YES)) ||
                (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && w.IsIsvMode.Equals(CS.YES))
            );

            // 处理特约商户的服务商配置
            Dictionary<string, PayInterfaceConfigDto> isvPayConfigMap = new();
            if (mchInfo.Type == CS.MCH_TYPE_ISVSUB)
            {
                var isvConfigQuery = _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) &&
                    !string.IsNullOrWhiteSpace(w.IfParams) && (!isApplyment || w.IsOpenApplyment.Equals(CS.YES)));

                // 处理代理商过滤
                if (!string.IsNullOrEmpty(mchInfo.AgentNo))
                {
                    var agentInfo = await _agentInfoRepository.GetByIdAsync(mchInfo.AgentNo);
                    var ifCodes = await GetAgentIfCodesAsync(agentInfo, isApplyment);
                    isvConfigQuery = isvConfigQuery.Where(s => ifCodes.Contains(s.IfCode));
                }

                var isvConfigList = await isvConfigQuery.ToListAsync();
                foreach (var isvConfig in isvConfigList)
                {
                    var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                    config.MchType = mchInfo.Type;
                    isvPayConfigMap[config.IfCode] = config;
                }
            }

            // 异步获取配置状态
            var configIfCodes = await configQuery.Where(c => c.State.Equals(CS.YES))
                .Select(c => c.IfCode)
                .Distinct()
                .ToListAsync();

            // 执行主查询
            var defineList = await defineQuery.ToListAsync();

            // 内存中处理结果
            return defineList
                .Where(define =>
                    mchInfo.Type != CS.MCH_TYPE_ISVSUB ||
                    isvPayConfigMap.ContainsKey(define.IfCode))
                .Select(define =>
                {
                    var dto = _mapper.Map<PayInterfaceDefineDto>(define);
                    dto.AddExt("mchType", mchInfo.Type);
                    dto.AddExt("ifConfigState", configIfCodes.Contains(define.IfCode) ? CS.YES : null);
                    return dto;
                })
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        private async Task<List<PayInterfaceDefineDto>> ProcessAgentConfigAsync(
            string infoId, string configMode, bool isApplyment,
            IQueryable<PayInterfaceDefine> defineQuery,
            IQueryable<PayInterfaceConfig> configQuery)
        {
            // 获取代理商信息
            var agentInfo = await _agentInfoRepository.GetByIdAsync(infoId);
            if (agentInfo == null || agentInfo.State != CS.YES)
            {
                throw new BizException("代理商不存在");
            }

            // 获取服务商配置
            var isvConfigQuery = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(agentInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) &&
                !string.IsNullOrWhiteSpace(w.IfParams));

            // 处理代理商过滤
            if (configMode == CS.CONFIG_MODE.AGENT_SELF || !string.IsNullOrEmpty(agentInfo.Pid))
            {
                var ifCodes = await GetAgentIfCodesAsync(agentInfo, isApplyment);
                isvConfigQuery = isvConfigQuery.Where(s => ifCodes.Contains(s.IfCode));
            }

            // 获取有效的接口代码
            var validIfCodes = await isvConfigQuery
                .Select(s => s.IfCode)
                .Distinct()
                .ToListAsync();

            // 过滤支付定义
            defineQuery = defineQuery
                .Where(w => w.IsIsvMode.Equals(CS.YES))
                .Where(w => validIfCodes.Contains(w.IfCode));

            // 获取配置状态
            var configIfCodes = await configQuery
                .Where(c => c.State.Equals(CS.YES))
                .Select(c => c.IfCode)
                .Distinct()
                .ToListAsync();

            // 执行查询并映射结果
            var defineList = await defineQuery.ToListAsync();

            return defineList.Select(define =>
            {
                var dto = _mapper.Map<PayInterfaceDefineDto>(define);
                dto.AddExt("ifConfigState", configIfCodes.Contains(define.IfCode) ? CS.YES : null);
                return dto;
            }).ToList();
        }

        /// <summary>
        /// 查询代理商正常可用通道代码（异步版本）
        /// </summary>
        /// <param name="agentInfo">代理商信息</param>
        /// <param name="isApplyment">是否为进件模式</param>
        /// <returns>可用的支付接口代码列表</returns>
        private async Task<List<string>> GetAgentIfCodesAsync(AgentInfo agentInfo, bool isApplyment)
        {
            if (!string.IsNullOrEmpty(agentInfo.Pid))
            {
                // 获取所有上级代理商列表（异步）
                var agentInfos = _agentInfoRepository.GetParentAgentsFromSqlAsNoTracking(agentInfo.AgentNo);
                var infoIds = agentInfos.Select(s => s.AgentNo).ToList();

                // 获取所有上级代理商的配置
                var agentConfigs = await _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => infoIds.Contains(w.InfoId) && w.InfoType.Equals(CS.INFO_TYPE.AGENT))
                    .ToListAsync();

                // 内存中处理：按接口代码分组并过滤有效配置
                return agentConfigs
                    .GroupBy(c => c.IfCode)
                    .Where(g => g.All(c =>
                        c.State == CS.YES &&
                        (!isApplyment || (isApplyment && c.IsOpenApplyment == CS.YES))
                    ))
                    .Select(g => g.Key)
                    .ToList();
            }
            else
            {
                // 直接获取当前代理商的可用接口代码
                return await _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => w.InfoId.Equals(agentInfo.AgentNo)
                        && w.InfoType.Equals(CS.INFO_TYPE.AGENT) && w.State.Equals(CS.YES)
                        && (!isApplyment || (isApplyment && w.IsOpenApplyment.Equals(CS.YES))))
                    .Select(c => c.IfCode)
                    .Distinct()
                    .ToListAsync();
            }
        }

        /// <summary>
        /// 查询代理商正常可用通道代码
        /// </summary>
        /// <param name="agentInfo"></param>
        /// <param name="isApplyment"></param>
        /// <returns></returns>
        private IQueryable<string> GetAgentIfCodes(AgentInfo agentInfo, bool isApplyment)
        {
            IQueryable<string> ifCodes;
            if (!string.IsNullOrEmpty(agentInfo.Pid))
            {
                var agentInfos = _agentInfoRepository.GetParentAgentsFromSqlAsNoTracking(agentInfo.AgentNo);
                var infoIds = agentInfos.Select(s => s.AgentNo).ToList();

                var agentConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => infoIds.Contains(w.InfoId) && w.InfoType.Equals(CS.INFO_TYPE.AGENT));

                ifCodes = agentConfigList.GroupBy(g => g.IfCode)
                    .Where(w => w.All(m => m.State.Equals(CS.YES) && (!isApplyment || (isApplyment && m.IsOpenApplyment.Equals(CS.YES)))))
                    .Select(s => s.Key);
            }
            else
            {
                var agentConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(agentInfo.AgentNo) && w.InfoType.Equals(CS.INFO_TYPE.AGENT)
                    && (!isApplyment || (isApplyment && w.IsOpenApplyment.Equals(CS.YES))));

                ifCodes = agentConfigList.Select(s => s.IfCode);
            }

            return ifCodes;
        }

        public async Task<IEnumerable<PayInterfaceDefineDto>> SelectAllPayIfConfigListByAppIdAsync(string appId)
        {
            MchApp mchApp = await _mchAppRepository.GetByIdAsync(appId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                throw new BizException("商户应用不存在");
            }

            MchInfo mchInfo = await _mchInfoRepository.GetByIdAsync(mchApp.MchNo);
            if (mchInfo == null || mchInfo.State != CS.YES)
            {
                throw new BizException("商户不存在");
            }

            // 支付定义列表查询（保持IQueryable）
            var defineQuery = _payInterfaceDefineRepository.GetAllAsNoTracking().Where(w => w.State.Equals(CS.YES)
            && ((mchInfo.Type.Equals(CS.MCH_TYPE_NORMAL) && w.IsMchMode.Equals(CS.YES))// 支持普通商户模式
            || (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && w.IsIsvMode.Equals(CS.YES)))// 支持服务商模式
            );

            // 服务商支付参数配置集合（改为异步查询）
            var isvPayConfigMap = new Dictionary<string, PayInterfaceConfigDto>();
            if (mchInfo.Type == CS.MCH_TYPE_ISVSUB)
            {
                // 商户类型为特约商户，服务商已经配置支付参数
                var isvConfigList = await _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams))
                    .ToListAsync();

                foreach (var isvConfig in isvConfigList)
                {
                    var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                    config.MchType = mchInfo.Type;
                    isvPayConfigMap.Add(config.IfCode, config);
                }
            }

            // 异步获取支付参数配置列表（只取需要的字段）
            var configIfCodes = await _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.Equals(appId) && w.InfoType.Equals(CS.INFO_TYPE.MCH_APP) && w.State.Equals(CS.YES))
                .Select(c => c.IfCode)
                .Distinct()
                .ToListAsync();

            // 异步执行主查询并处理结果
            var result = new List<PayInterfaceDefineDto>();
            var defineList = await defineQuery.ToListAsync(); // 异步执行查询

            foreach (var define in defineList)
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                entity.AddExt("mchType", mchInfo.Type);

                // 使用内存中的集合检查，避免重复查询
                entity.AddExt("ifConfigState", configIfCodes.Contains(define.IfCode) ? CS.YES : null);

                // 优化字典检查逻辑
                bool hasIsvConfig = mchInfo.Type != CS.MCH_TYPE_ISVSUB || isvPayConfigMap.ContainsKey(define.IfCode);
                entity.AddExt("subMchIsvConfig", hasIsvConfig ? null : CS.NO);

                result.Add(entity);
            }

            return result;
        }

        public async Task<IEnumerable<PayInterfaceDefineDto>> GetPayIfConfigsByMchNoAsync(string mchNo)
        {
            MchInfo mchInfo = await _mchInfoRepository.GetByIdAsync(mchNo);
            if (mchInfo == null || mchInfo.State != CS.YES)
            {
                throw new BizException("商户不存在");
            }

            // 构建基础查询 支付定义列表
            var defineQuery = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES))
                .Where(w => ((mchInfo.Type.Equals(CS.MCH_TYPE_NORMAL) && w.IsMchMode.Equals(CS.YES))// 支持普通商户模式
                || (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && w.IsIsvMode.Equals(CS.YES)))// 支持服务商模式
            );

            // 服务商支付参数配置集合
            var isvPayConfigMap = new Dictionary<string, PayInterfaceConfigDto>();
            if (mchInfo.Type == CS.MCH_TYPE_ISVSUB)
            {
                // 商户类型为特约商户，服务商已经配置支付参数
                var isvConfigList = await _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => w.State.Equals(CS.YES)
                    && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV)
                    && !string.IsNullOrWhiteSpace(w.IfParams))
                    .ToListAsync();

                foreach (var isvConfig in isvConfigList)
                {
                    var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                    config.MchType = mchInfo.Type;
                    isvPayConfigMap.Add(config.IfCode, config);
                }
            }

            // 异步获取支付定义列表
            var defineList = await defineQuery.ToListAsync();

            var results = defineList.Where(w => mchInfo.Type != CS.MCH_TYPE_ISVSUB
                || (mchInfo.Type == CS.MCH_TYPE_ISVSUB && isvPayConfigMap.ContainsKey(w.IfCode)))
                .Select(define => _mapper.Map<PayInterfaceDefineDto>(define))
                .OrderByDescending(o => o.CreatedAt);
            return results.ToList();
        }

        /// <summary>
        /// 查询商户app使用已正确配置了通道信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public Task<bool> MchAppHasAvailableIfCodeAsync(string appId, string ifCode)
        {
            return _payInterfaceConfigRepository.MchAppHasAvailableIfCodeAsync(appId, ifCode);
        }
    }
}

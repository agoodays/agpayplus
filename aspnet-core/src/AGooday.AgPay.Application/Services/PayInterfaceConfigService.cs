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
            var result = await _payInterfaceConfigRepository.SaveChangesAsync() > 0;
            dto.Id = entity.Id;
            return result;
        }

        public Task<bool> IsExistUseIfCodeAsync(string ifCode)
        {
            return _payInterfaceConfigRepository.IsExistUseIfCodeAsync(ifCode);
        }

        public async Task<bool> SaveOrUpdateAsync(PayInterfaceConfigDto dto)
        {
            var m = _mapper.Map<PayInterfaceConfig>(dto);
            _payInterfaceConfigRepository.SaveOrUpdate(m, dto.Id);
            return await _payInterfaceConfigRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAsync(string infoType, string infoId)
        {
            var payInterfaceConfig = await _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.Equals(infoId) && w.InfoType.Equals(infoType)).FirstOrDefaultAsync();
            _payInterfaceConfigRepository.Remove(payInterfaceConfig.Id);
            return await _payInterfaceConfigRepository.SaveChangesAsync() > 0;
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
            var payInterfaceConfig = await _payInterfaceConfigRepository.GetAllAsNoTracking().Where(w => w.InfoId.Equals(infoId)
            && w.InfoType.Equals(infoType) && w.IfCode.Equals(ifCode)).FirstOrDefaultAsync();
            return _mapper.Map<PayInterfaceConfigDto>(payInterfaceConfig);
        }

        public IEnumerable<PayInterfaceConfigDto> GetByInfoIdAndIfCodes(string infoType, List<string> infoIds, string ifCode)
        {
            var payInterfaceConfig = _payInterfaceConfigRepository.GetAllAsNoTracking().Where(w => infoIds.Contains(w.InfoId)
            && w.InfoType.Equals(infoType) && w.IfCode.Equals(ifCode));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(payInterfaceConfig);
        }

        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <returns></returns>
        public IEnumerable<PayInterfaceConfigDto> GetByInfoId(string infoType, string infoId)
        {
            var payInterfaceConfigs = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.Equals(infoId)
                && w.InfoType.Equals(infoType) && w.State.Equals(CS.PUB_USABLE));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(payInterfaceConfigs);
        }

        public IEnumerable<PayInterfaceConfigDto> GetPayOauth2ConfigByStartsWithInfoId(string infoType, string infoId)
        {
            var payInterfaceConfigs = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.StartsWith(infoId)
                && w.InfoType.Equals(infoType) && w.State.Equals(CS.PUB_USABLE));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(payInterfaceConfigs);
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

        public async Task<List<PayInterfaceDefineDto>> PayIfConfigListAsync(string infoType, string configMode, string infoId, string ifName, string ifCode)
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
                var agentInfos = _agentInfoRepository.GetParentAgentsFromSql(agentInfo.AgentNo);
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
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking().Where(w => w.State.Equals(CS.YES)
            && ((mchInfo.Type.Equals(CS.MCH_TYPE_NORMAL) && w.IsMchMode.Equals(CS.YES))// 支持普通商户模式
            || (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && w.IsIsvMode.Equals(CS.YES)))// 支持服务商模式
            );
            var isvPayConfigMap = new Dictionary<string, PayInterfaceConfigDto>();// 服务商支付参数配置集合
            if (mchInfo.Type == CS.MCH_TYPE_ISVSUB)
            {
                // 商户类型为特约商户，服务商应已经配置支付参数
                var isvConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                    .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams));

                foreach (var isvConfig in isvConfigList)
                {
                    var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                    config.MchType = mchInfo.Type;
                    isvPayConfigMap.Add(config.IfCode, config);
                }
            }

            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.Equals(appId) && w.InfoType.Equals(CS.INFO_TYPE.MCH_APP));

            var result = defineList.AsEnumerable().Select(define =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                entity.AddExt("mchType", mchInfo.Type);// 所属商户类型
                entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(define.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                entity.AddExt("subMchIsvConfig", mchInfo.Type == CS.MCH_TYPE_ISVSUB && !isvPayConfigMap.TryGetValue(define.IfCode, out _) ? CS.NO : null);
                return entity;
            });

            return result;
        }

        public async Task<IEnumerable<PayInterfaceDefineDto>> GetPayIfConfigsByMchNoAsync(string mchNo)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES));
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
                var isvConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking().Where(w => w.State.Equals(CS.YES)
                && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams));

                foreach (var isvConfig in isvConfigList)
                {
                    var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                    config.MchType = mchInfo.Type;
                    isvPayConfigMap.Add(config.IfCode, config);
                }
            }
            var results = defineList.AsEnumerable()
                .Where(w => mchInfo.Type != CS.MCH_TYPE_ISVSUB || (mchInfo.Type == CS.MCH_TYPE_ISVSUB && isvPayConfigMap.TryGetValue(w.IfCode, out _)))
                .Select(define =>
                {
                    var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                    return entity;
                }).OrderByDescending(o => o.CreatedAt);
            return results;
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

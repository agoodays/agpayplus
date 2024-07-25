﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

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

        public override bool Add(PayInterfaceConfigDto dto)
        {
            var entity = _mapper.Map<PayInterfaceConfig>(dto);
            _payInterfaceConfigRepository.Add(entity);
            var result = _payInterfaceConfigRepository.SaveChanges(out int _);
            dto.Id = entity.Id;
            return result;
        }

        public bool SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            var m = _mapper.Map<PayInterfaceConfig>(dto);
            _payInterfaceConfigRepository.SaveOrUpdate(m, dto.Id);
            return _payInterfaceConfigRepository.SaveChanges() > 0;
        }

        public bool IsExistUseIfCode(string ifCode)
        {
            return _payInterfaceConfigRepository.IsExistUseIfCode(ifCode);
        }

        public bool Remove(string infoType, string infoId)
        {
            var payInterfaceConfig = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoId.Equals(infoId) && w.InfoType.Equals(infoType)).FirstOrDefault();
            _payInterfaceConfigRepository.Remove(payInterfaceConfig.Id);
            return _payInterfaceConfigRepository.SaveChanges(out int _);
        }

        /// <summary>
        /// 根据 账户类型、账户号、接口类型 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <param name="ifCode">接口类型</param>
        /// <returns></returns>
        public PayInterfaceConfigDto GetByInfoIdAndIfCode(string infoType, string infoId, string ifCode)
        {
            // 跟踪与非跟踪查询：https://learn.microsoft.com/zh-cn/ef/core/querying/tracking
            var payInterfaceConfig = _payInterfaceConfigRepository.GetAllAsNoTracking().Where(w => w.InfoId.Equals(infoId)
            && w.InfoType.Equals(infoType) && w.IfCode.Equals(ifCode)).FirstOrDefault();
            return _mapper.Map<PayInterfaceConfigDto>(payInterfaceConfig);
        }

        public IEnumerable<PayInterfaceConfigDto> GetByInfoIdAndIfCodes(string infoType, List<string> infoIds, string ifCode)
        {
            // 跟踪与非跟踪查询：https://learn.microsoft.com/zh-cn/ef/core/querying/tracking
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
        public List<PayInterfaceDefineDto> SelectAllPayIfConfigListByIsvNo(string infoType, string infoId)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.IsIsvMode.Equals(CS.YES) && w.State.Equals(CS.YES));
            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

            var result = defineList.ToList().Select(s =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(s);
                entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(s.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                return entity;
            }).ToList();

            return result;
        }

        public List<PayInterfaceDefineDto> PayIfConfigList(string infoType, string configMode, string infoId, string ifName, string ifCode)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES)
                && (string.IsNullOrWhiteSpace(ifName) || w.IfName.Contains(ifName))
                && (string.IsNullOrWhiteSpace(ifCode) || w.IfCode.Equals(ifCode)));

            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                .Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

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
                        MchApp mchApp = _mchAppRepository.GetById(infoId);
                        if (mchApp == null || mchApp.State != CS.YES)
                        {
                            throw new BizException("商户应用不存在");
                        }
                        mchNo = mchApp.MchNo;
                    }
                    MchInfo mchInfo = _mchInfoRepository.GetById(mchNo);
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
                        var isvConfigList = _payInterfaceConfigRepository.GetAllAsNoTracking()
                            .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams));

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
                    AgentInfo agentInfo = _agentInfoRepository.GetById(infoId);
                    if (agentInfo == null || agentInfo.State != CS.YES)
                    {
                        throw new BizException("代理商不存在");
                    }
                    // 商户类型为特约商户，服务商应已经配置支付参数
                    var isvConfigs = _payInterfaceConfigRepository.GetAllAsNoTracking()
                        .Where(w => w.State.Equals(CS.YES) && w.InfoId.Equals(agentInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE.ISV) && !string.IsNullOrWhiteSpace(w.IfParams));
                    defineList = defineList.Where(w => w.IsIsvMode.Equals(CS.YES) && isvConfigs.Select(s => s.IfCode).Contains(w.IfCode));
                    break;
                default:
                    break;
            }
            var result = defineList.ToList().Select(s =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(s);
                entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(s.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                return entity;
            }).ToList();

            return result;
        }

        public List<PayInterfaceDefineDto> SelectAllPayIfConfigListByAppId(string appId)
        {
            MchApp mchApp = _mchAppRepository.GetById(appId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                throw new BizException("商户应用不存在");
            }
            MchInfo mchInfo = _mchInfoRepository.GetById(mchApp.MchNo);
            if (mchInfo == null || mchInfo.State != CS.YES)
            {
                throw new BizException("商户不存在");
            }
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAll().Where(w => w.State.Equals(CS.YES)
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

            var result = defineList.ToList().Select(define =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                entity.AddExt("mchType", mchInfo.Type);// 所属商户类型
                entity.AddExt("ifConfigState", configList.Any(a => a.IfCode.Equals(define.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null);
                entity.AddExt("subMchIsvConfig", mchInfo.Type == CS.MCH_TYPE_ISVSUB && !isvPayConfigMap.TryGetValue(define.IfCode, out _) ? CS.NO : null);
                return entity;
            }).ToList();

            return result;
        }

        public List<PayInterfaceDefineDto> GetPayIfConfigsByMchNo(string mchNo)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => w.State.Equals(CS.YES));
            MchInfo mchInfo = _mchInfoRepository.GetById(mchNo);
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
            var results = defineList.ToList()
                .Where(w => mchInfo.Type != CS.MCH_TYPE_ISVSUB || (mchInfo.Type == CS.MCH_TYPE_ISVSUB && isvPayConfigMap.TryGetValue(w.IfCode, out _)))
                .Select(define =>
                {
                    var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                    return entity;
                }).ToList();
            return results;
        }

        /// <summary>
        /// 查询商户app使用已正确配置了通道信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public bool MchAppHasAvailableIfCode(string appId, string ifCode)
        {
            return _payInterfaceConfigRepository.MchAppHasAvailableIfCode(appId, ifCode);
        }
    }
}

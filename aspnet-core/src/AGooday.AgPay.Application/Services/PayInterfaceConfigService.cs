using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Models;
using AGooday.AgPay.Common.Exceptions;

namespace AGooday.AgPay.Application.Services
{
    public class PayInterfaceConfigService : IPayInterfaceConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        private readonly IMchAppRepository _mchAppRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayInterfaceConfigService(IMapper mapper, IMediatorHandler bus,
            IPayInterfaceConfigRepository payInterfaceConfigRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository, 
            IMchAppRepository mchAppRepository, 
            IMchInfoRepository mchInfoRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payInterfaceConfigRepository = payInterfaceConfigRepository;
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
            _mchAppRepository = mchAppRepository;
            _mchInfoRepository = mchInfoRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(PayInterfaceConfigDto dto)
        {
            var m = _mapper.Map<PayInterfaceConfig>(dto);
            _payInterfaceConfigRepository.Add(m);
            _payInterfaceConfigRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _payInterfaceConfigRepository.Remove(recordId);
            _payInterfaceConfigRepository.SaveChanges();
        }

        public void Update(PayInterfaceConfigDto dto)
        {
            var m = _mapper.Map<PayInterfaceConfig>(dto);
            _payInterfaceConfigRepository.Update(m);
            _payInterfaceConfigRepository.SaveChanges();
        }

        public bool SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            var m = _mapper.Map<PayInterfaceConfig>(dto);
            _payInterfaceConfigRepository.SaveOrUpdate(m, dto.Id);
            return _payInterfaceConfigRepository.SaveChanges() > 0;
        }

        public PayInterfaceConfigDto GetById(long recordId)
        {
            var entity = _payInterfaceConfigRepository.GetById(recordId);
            var dto = _mapper.Map<PayInterfaceConfigDto>(entity);
            return dto;
        }

        public IEnumerable<PayInterfaceConfigDto> GetAll()
        {
            var payInterfaceConfigs = _payInterfaceConfigRepository.GetAll();
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(payInterfaceConfigs);
        }

        public bool IsExistUseIfCode(string ifCode)
        {
            return _payInterfaceConfigRepository.IsExistUseIfCode(ifCode);
        }

        /// <summary>
        /// 根据 账户类型、账户号、接口类型 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <param name="ifCode">接口类型</param>
        /// <returns></returns>
        public PayInterfaceConfigDto GetByInfoIdAndIfCode(byte infoType, string infoId, string ifCode)
        {
            var payInterfaceConfig = _payInterfaceConfigRepository.GetAll().Where(w => w.InfoId.Equals(infoId)
            && w.InfoType.Equals(infoType) && w.IfCode.Equals(ifCode)).FirstOrDefault();
            return _mapper.Map<PayInterfaceConfigDto>(payInterfaceConfig);
        }

        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <returns></returns>
        public IEnumerable<PayInterfaceConfigDto> GetByInfoId(byte infoType, string infoId)
        {
            var payInterfaceConfigs = _payInterfaceConfigRepository.GetAll().Where(w => w.InfoId.Equals(infoId)
            && w.InfoType.Equals(infoType) && w.State.Equals(CS.PUB_USABLE));
            return _mapper.Map<IEnumerable<PayInterfaceConfigDto>>(payInterfaceConfigs);
        }

        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置列表
        /// </summary>
        /// <param name="infoType"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public List<PayInterfaceDefineDto> SelectAllPayIfConfigListByIsvNo(byte infoType, string infoId)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAll().Where(w => w.IsIsvMode.Equals(CS.YES) && w.State.Equals(CS.YES));
            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAll().Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

            var result = defineList.ToList().Select(s =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(s);
                entity.IfConfigState = configList.Any(a => a.IfCode.Equals(s.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null;
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
                var isvConfigList = _payInterfaceConfigRepository.GetAll().Where(w => w.State.Equals(CS.YES)
                && w.InfoId.Equals(mchInfo.IsvNo) && w.InfoType.Equals(CS.INFO_TYPE_ISV) && !string.IsNullOrWhiteSpace(w.IfParams)
                );

                foreach (var isvConfig in isvConfigList)
                {
                    var config = _mapper.Map<PayInterfaceConfigDto>(isvConfig);
                    config.MchType = mchInfo.Type;
                    isvPayConfigMap.Add(config.IfCode, config);
                }
            }

            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAll().Where(w => w.InfoId.Equals(appId)
            && w.InfoType.Equals(CS.INFO_TYPE_MCH_APP));

            var result = defineList.ToList().Select(define =>
            {
                var entity = _mapper.Map<PayInterfaceDefineDto>(define);
                entity.MchType = mchInfo.Type;// 所属商户类型
                entity.IfConfigState = configList.Any(a => a.IfCode.Equals(define.IfCode) && a.State.Equals(CS.YES)) ? CS.YES : null;
                entity.SubMchIsvConfig = mchInfo.Type == CS.MCH_TYPE_ISVSUB && isvPayConfigMap.TryGetValue(define.IfCode, out _) ? CS.NO : null;
                return entity;
            }).ToList();

            return result;
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

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

namespace AGooday.AgPay.Application.Services
{
    public class PayInterfaceConfigService : IPayInterfaceConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayInterfaceConfigService(IMapper mapper, IMediatorHandler bus,
            IPayInterfaceConfigRepository payInterfaceConfigRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payInterfaceConfigRepository = payInterfaceConfigRepository;
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
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
        /// 根据 账户类型、账户号 获取支付参数配置列表
        /// </summary>
        /// <param name="infoType"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public List<PayInterfaceConfigDto> SelectAllPayIfConfigListByIsvNo(byte infoType, string infoId)
        {
            // 支付定义列表
            var defineList = _payInterfaceDefineRepository.GetAll().Where(w => w.IsIsvMode.Equals(CS.YES) && w.State.Equals(CS.YES));
            // 支付参数列表
            var configList = _payInterfaceConfigRepository.GetAll().Where(w => w.InfoType.Equals(infoType) && w.InfoId.Equals(infoId));

            var result = defineList.ToList().Select(s =>
            {
                var entity = _mapper.Map<PayInterfaceConfigDto>(s);
                entity.IfConfigState = configList.Any(a => a.IfCode.Equals(s.IfCode)) ? CS.YES : CS.NO;
                return entity;
            }).ToList();

            return result;
        }
    }
}

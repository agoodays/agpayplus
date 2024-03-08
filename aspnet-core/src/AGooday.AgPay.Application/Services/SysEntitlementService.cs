using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统权限表 服务实现类
    /// </summary>
    public class SysEntitlementService : ISysEntitlementService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysEntitlementRepository _sysEntitlementRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysEntitlementService(ISysEntitlementRepository sysEntitlementRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysEntitlementRepository = sysEntitlementRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysEntitlementDto dto)
        {
            var m = _mapper.Map<SysEntitlement>(dto);
            _sysEntitlementRepository.Add(m);
            _sysEntitlementRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysEntitlementRepository.Remove(recordId);
            _sysEntitlementRepository.SaveChanges();
        }

        public void Update(SysEntModifyDto dto)
        {
            var renew = _mapper.Map<SysEntitlement>(dto);
            renew.UpdatedAt = DateTime.Now;
            _sysEntitlementRepository.Update(renew);
            _sysEntitlementRepository.SaveChanges();
        }

        public SysEntitlementDto GetById(string recordId)
        {
            var entity = _sysEntitlementRepository.GetById(recordId);
            var dto = _mapper.Map<SysEntitlementDto>(entity);
            return dto;
        }

        public SysEntitlementDto GetByKeyAsNoTracking(string entId, string sysType)
        {
            var entity = _sysEntitlementRepository.GetByKeyAsNoTracking(entId, sysType);
            return _mapper.Map<SysEntitlementDto>(entity);
        }

        public SysEntitlementDto GetByKey(string entId, string sysType)
        {
            var entity = _sysEntitlementRepository.GetByKey(entId, sysType);
            var dto = _mapper.Map<SysEntitlementDto>(entity);
            return dto;
        }

        public IEnumerable<SysEntitlementDto> GetBySysType(string sysType, string entId)
        {
            var sysEnts = _sysEntitlementRepository.GetAll()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (string.IsNullOrWhiteSpace(entId) || w.EntId.Equals(entId))
                );
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEnts);
        }

        public IEnumerable<SysEntitlementDto> GetAll()
        {
            var sysEntitlements = _sysEntitlementRepository.GetAll();
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEntitlements);
        }
    }
}

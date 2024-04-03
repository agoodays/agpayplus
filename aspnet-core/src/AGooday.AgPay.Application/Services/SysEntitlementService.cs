using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Models;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统权限表 服务实现类
    /// </summary>
    public class SysEntitlementService : AgPayService<SysEntitlementDto, SysEntitlement>, ISysEntitlementService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysEntitlementRepository _sysEntitlementRepository;

        public SysEntitlementService(IMapper mapper, IMediatorHandler bus,
            ISysEntitlementRepository sysEntitlementRepository)
            : base(mapper, bus, sysEntitlementRepository)
        {
            _sysEntitlementRepository = sysEntitlementRepository;
        }

        public override bool Update(SysEntitlementDto dto)
        {
            var entity = _mapper.Map<SysEntitlement>(dto);
            entity.UpdatedAt = DateTime.Now;
            _sysEntitlementRepository.Update(entity);
            return _sysEntitlementRepository.SaveChanges(out int _);
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
            var sysEnts = _sysEntitlementRepository.GetAllAsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (string.IsNullOrWhiteSpace(entId) || w.EntId.Equals(entId))
                );
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEnts);
        }

        public IEnumerable<SysEntitlementDto> GetByIds(string sysType, List<string> entIds)
        {
            var sysEnts = _sysEntitlementRepository.GetAllAsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (!(entIds != null && entIds.Count > 0) || entIds.Contains(w.EntId)));
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEnts);
        }

        public IEnumerable<SysEntitlementDto> GetSons(string sysType, string pId, string entId)
        {
            var sysEnts = _sysEntitlementRepository.GetAllAsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && w.Pid.Equals(pId) && !w.EntId.Equals(entId));
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEnts);
        }
    }
}

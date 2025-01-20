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

        public override async Task<bool> UpdateAsync(SysEntitlementDto dto)
        {
            var entity = _mapper.Map<SysEntitlement>(dto);
            entity.UpdatedAt = DateTime.Now;
            _sysEntitlementRepository.Update(entity);
            var (result, _) = await _sysEntitlementRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<SysEntitlementDto> GetByKeyAsNoTrackingAsync(string entId, string sysType)
        {
            var entity = await _sysEntitlementRepository.GetByKeyAsNoTrackingAsync(entId, sysType);
            return _mapper.Map<SysEntitlementDto>(entity);
        }

        public async Task<SysEntitlementDto> GetByKeyAsync(string entId, string sysType)
        {
            var entity = await _sysEntitlementRepository.GetByKeyAsync(entId, sysType);
            var dto = _mapper.Map<SysEntitlementDto>(entity);
            return dto;
        }

        public IEnumerable<SysEntitlementDto> GetBySysType(string sysType, string entId)
        {
            var records = _sysEntitlementRepository.GetAllAsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (string.IsNullOrWhiteSpace(entId) || w.EntId.Equals(entId)));
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(records);
        }

        public IEnumerable<SysEntitlementDto> GetByIds(string sysType, List<string> entIds)
        {
            var records = _sysEntitlementRepository.GetAllAsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (!(entIds != null && entIds.Count > 0) || entIds.Contains(w.EntId)));
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(records);
        }

        public IEnumerable<SysEntitlementDto> GetBros(string sysType, string pId)
        {
            var records = _sysEntitlementRepository.GetAllAsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE) && w.Pid.Equals(pId));
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(records);
        }

        public IEnumerable<SysEntitlementDto> GetBros(string sysType, string pId, string entId)
        {
            return GetBros(sysType, pId).Where(w => !w.EntId.Equals(entId));
        }

        public IEnumerable<SysEntitlementDto> GetSubSysEntitlementsFromSql(string entId, string sysType)
        {
            var records = _sysEntitlementRepository.GetSubSysEntitlementsFromSqlAsNoTracking(entId, sysType);
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(records);
        }

        public IEnumerable<SysEntitlementDto> GetParentSysEntitlementsFromSql(string entId, string sysType)
        {
            var records = _sysEntitlementRepository.GetParentSysEntitlementsFromSqlAsNoTracking(entId, sysType);
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(records);
        }
    }
}

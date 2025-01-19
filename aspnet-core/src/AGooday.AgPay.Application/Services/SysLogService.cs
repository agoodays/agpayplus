using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统操作日志表 服务实现类
    /// </summary>
    public class SysLogService : AgPayService<SysLogDto, SysLog, long>, ISysLogService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysLogRepository _sysLogRepository;

        public SysLogService(IMapper mapper, IMediatorHandler bus,
            ISysLogRepository sysLogRepository)
            : base(mapper, bus, sysLogRepository)
        {
            _sysLogRepository = sysLogRepository;
        }

        public override async Task<bool> AddAsync(SysLogDto dto)
        {
            var entity = _mapper.Map<SysLog>(dto);
            await _sysLogRepository.AddAsync(entity);
            var (result, _) = await _sysLogRepository.SaveChangesWithResultAsync();
            dto.SysLogId = entity.SysLogId;
            return result;
        }

        public async Task<bool> RemoveByIdsAsync(List<long> recordIds)
        {
            var entities = _sysLogRepository.GetAll().Where(w => recordIds.Contains(w.SysLogId));
            _sysLogRepository.RemoveRange(entities);
            var (result, _) = await _sysLogRepository.SaveChangesWithResultAsync();
            return result;
        }

        public Task<PaginatedList<SysLogDto>> GetPaginatedDataAsync(SysLogQueryDto dto)
        {
            var query = _sysLogRepository.GetAllAsNoTracking()
                .Where(w => (dto.UserId.Equals(null) || w.UserId.Equals(dto.UserId))
                && (string.IsNullOrWhiteSpace(dto.UserName) || w.UserName.Contains(dto.UserName))
                && (string.IsNullOrWhiteSpace(dto.UserIp) || w.UserIp.Contains(dto.UserIp))
                && (string.IsNullOrWhiteSpace(dto.MethodRemark) || w.MethodRemark.Contains(dto.MethodRemark))
                && (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (dto.LogType.Equals(null) || w.LogType.Equals(dto.LogType))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysLog>.CreateAsync<SysLogDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}

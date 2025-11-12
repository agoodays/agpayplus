using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
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

        public Task<PaginatedResult<SysLogDto>> GetPaginatedDataAsync(SysLogQueryDto dto)
        {
            var query = _sysLogRepository.GetAllAsNoTracking()
                .WhereIfNotNull(dto.UserId, w => w.UserId.Equals(dto.UserId))
                .WhereIfNotEmpty(dto.UserName, w => w.UserName.Contains(dto.UserName))
                .WhereIfNotEmpty(dto.UserIp, w => w.UserIp.Contains(dto.UserIp))
                .WhereIfNotEmpty(dto.MethodRemark, w => w.MethodRemark.Contains(dto.MethodRemark))
                .WhereIfNotEmpty(dto.SysType, w => w.SysType.Equals(dto.SysType))
                .WhereIfNotNull(dto.LogType, w => w.LogType.Equals(dto.LogType))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd)
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<SysLog, SysLogDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}

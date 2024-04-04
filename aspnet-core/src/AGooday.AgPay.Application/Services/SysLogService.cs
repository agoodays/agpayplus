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

        public override bool Add(SysLogDto dto)
        {
            var m = _mapper.Map<SysLog>(dto);
            _sysLogRepository.Add(m);
            var result = _sysLogRepository.SaveChanges(out int _);
            dto.SysLogId = m.SysLogId;
            return result;
        }

        public bool RemoveByIds(List<long> recordIds)
        {
            foreach (var recordId in recordIds)
            {
                _sysLogRepository.Remove(recordId);
            }
            return _sysLogRepository.SaveChanges() > 0;
        }

        public PaginatedList<SysLogDto> GetPaginatedData(SysLogQueryDto dto)
        {
            var sysLogs = _sysLogRepository.GetAllAsNoTracking()
                .Where(w => (dto.UserId.Equals(null) || w.UserId.Equals(dto.UserId))
                && (string.IsNullOrWhiteSpace(dto.UserName) || w.UserName.Contains(dto.UserName))
                && (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysLog>.Create<SysLogDto>(sysLogs, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}

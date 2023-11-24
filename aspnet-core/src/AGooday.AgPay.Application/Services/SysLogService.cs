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
    public class SysLogService : ISysLogService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysLogRepository _sysLogRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysLogService(ISysLogRepository sysLogRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysLogRepository = sysLogRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysLogDto dto)
        {
            var m = _mapper.Map<SysLog>(dto);
            _sysLogRepository.Add(m);
            _sysLogRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _sysLogRepository.Remove(recordId);
            _sysLogRepository.SaveChanges();
        }

        public bool RemoveByIds(List<long> recordIds)
        {
            foreach (var recordId in recordIds)
            {
                _sysLogRepository.Remove(recordId);
            }
            return _sysLogRepository.SaveChanges() > 0;
        }

        public void Update(SysLogDto dto)
        {
            var m = _mapper.Map<SysLog>(dto);
            _sysLogRepository.Update(m);
            _sysLogRepository.SaveChanges();
        }

        public SysLogDto GetById(long recordId)
        {
            var entity = _sysLogRepository.GetById(recordId);
            var dto = _mapper.Map<SysLogDto>(entity);
            return dto;
        }

        public SysLogDto GetLastSysLog(long userId, string methodRemark, string sysType)
        {
            var entity = _sysLogRepository.GetLastSysLog(userId, methodRemark, sysType);
            var dto = _mapper.Map<SysLogDto>(entity);
            return dto;
        }

        public IEnumerable<SysLogDto> GetAll()
        {
            var sysLogs = _sysLogRepository.GetAll();
            return _mapper.Map<IEnumerable<SysLogDto>>(sysLogs);
        }

        public PaginatedList<SysLogDto> GetPaginatedData(SysLogQueryDto dto)
        {
            var sysLogs = _sysLogRepository.GetAllAsNoTracking()
                .Where(w => (dto.UserId.Equals(null) || w.UserId.Equals(dto.UserId))
                && (string.IsNullOrWhiteSpace(dto.UserName) || w.UserName.Contains(dto.UserName))
                && (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt < dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysLog>.Create<SysLogDto>(sysLogs, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}

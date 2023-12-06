using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    public class SysUserTeamService : ISysUserTeamService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserTeamRepository _sysUserTeamRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserTeamService(ISysUserTeamRepository sysUserTeamRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysUserTeamRepository = sysUserTeamRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(SysUserTeamDto dto)
        {
            var m = _mapper.Map<SysUserTeam>(dto);
            _sysUserTeamRepository.Add(m);
            var result = _sysUserTeamRepository.SaveChanges(out int _);
            dto.TeamId = m.TeamId;
            return result;
        }

        public bool Remove(long recordId)
        {
            _sysUserTeamRepository.Remove(recordId);
            return _sysUserTeamRepository.SaveChanges(out _);
        }

        public bool Update(SysUserTeamDto dto)
        {
            var renew = _mapper.Map<SysUserTeam>(dto);
            //var old = _sysUserTeamRepository.GetById(dto.StoreId);
            renew.UpdatedAt = DateTime.Now;
            _sysUserTeamRepository.Update(renew);
            return _sysUserTeamRepository.SaveChanges(out int _);
        }

        public SysUserTeamDto GetById(long recordId)
        {
            var entity = _sysUserTeamRepository.GetById(recordId);
            var dto = _mapper.Map<SysUserTeamDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserTeamDto> GetAll()
        {
            var sysUserTeams = _sysUserTeamRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserTeamDto>>(sysUserTeams);
        }

        public PaginatedList<SysUserTeamDto> GetPaginatedData(SysUserTeamQueryDto dto)
        {
            var sysUsers = _sysUserTeamRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (string.IsNullOrWhiteSpace(dto.BelongInfoId) || w.BelongInfoId.Equals(dto.BelongInfoId))
                && (string.IsNullOrWhiteSpace(dto.TeamName) || w.TeamName.Contains(dto.TeamName))
                && (string.IsNullOrWhiteSpace(dto.TeamNo) || w.TeamNo.Equals(dto.TeamNo))
                && (dto.TeamId.Equals(null) || w.TeamId.Equals(dto.TeamId))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysUserTeam>.Create<SysUserTeamDto>(sysUsers, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}

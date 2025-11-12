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
    public class SysUserTeamService : AgPayService<SysUserTeamDto, SysUserTeam>, ISysUserTeamService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserTeamRepository _sysUserTeamRepository;

        public SysUserTeamService(IMapper mapper, IMediatorHandler bus,
            ISysUserTeamRepository sysUserTeamRepository)
            : base(mapper, bus, sysUserTeamRepository)
        {
            _sysUserTeamRepository = sysUserTeamRepository;
        }

        public override async Task<bool> AddAsync(SysUserTeamDto dto)
        {
            var entity = _mapper.Map<SysUserTeam>(dto);
            await _sysUserTeamRepository.AddAsync(entity);
            var (result, _) = await _sysUserTeamRepository.SaveChangesWithResultAsync();
            dto.TeamId = entity.TeamId;
            return result;
        }

        public override async Task<bool> UpdateAsync(SysUserTeamDto dto)
        {
            var renew = _mapper.Map<SysUserTeam>(dto);
            renew.UpdatedAt = DateTime.Now;
            _sysUserTeamRepository.Update(renew);
            var (result, _) = await _sysUserTeamRepository.SaveChangesWithResultAsync();
            return result;
        }

        public Task<PaginatedResult<SysUserTeamDto>> GetPaginatedDataAsync(SysUserTeamQueryDto dto)
        {
            var query = _sysUserTeamRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.SysType, w => w.SysType.Equals(dto.SysType))
                .WhereIfNotEmpty(dto.BelongInfoId, w => w.BelongInfoId.Equals(dto.BelongInfoId))
                .WhereIfNotEmpty(dto.TeamName, w => w.TeamName.Contains(dto.TeamName))
                .WhereIfNotEmpty(dto.TeamNo, w => w.TeamNo.Equals(dto.TeamNo))
                .WhereIfNotNull(dto.TeamId, w => w.TeamId.Equals(dto.TeamId))
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<SysUserTeam, SysUserTeamDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}

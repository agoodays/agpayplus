﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
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

        public Task<PaginatedList<SysUserTeamDto>> GetPaginatedDataAsync(SysUserTeamQueryDto dto)
        {
            var query = _sysUserTeamRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (string.IsNullOrWhiteSpace(dto.BelongInfoId) || w.BelongInfoId.Equals(dto.BelongInfoId))
                && (string.IsNullOrWhiteSpace(dto.TeamName) || w.TeamName.Contains(dto.TeamName))
                && (string.IsNullOrWhiteSpace(dto.TeamNo) || w.TeamNo.Equals(dto.TeamNo))
                && (dto.TeamId.Equals(null) || w.TeamId.Equals(dto.TeamId)))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysUserTeam>.CreateAsync<SysUserTeamDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}

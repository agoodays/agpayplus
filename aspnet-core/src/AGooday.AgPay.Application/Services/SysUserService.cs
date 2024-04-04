using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Domain.Queries.SysUsers;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统操作员表 服务实现类
    /// </summary>
    public class SysUserService : AgPayService<SysUserDto, SysUser, long>, ISysUserService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserRepository _sysUserRepository;

        private readonly ISysUserTeamRepository _sysUserTeamRepository;

        public SysUserService(IMapper mapper, IMediatorHandler bus,
            ISysUserRepository sysUserRepository,
            ISysUserTeamRepository sysUserTeamRepository)
            : base(mapper, bus, sysUserRepository)
        {
            _sysUserRepository = sysUserRepository;
            _sysUserTeamRepository = sysUserTeamRepository;
        }

        public override bool Add(SysUserDto dto)
        {
            var m = _mapper.Map<SysUser>(dto);
            _sysUserRepository.Add(m);
            _sysUserRepository.SaveChanges();
            var result = _sysUserRepository.SaveChanges(out int _);
            dto.SysUserId = m.SysUserId;
            return result;
        }

        public async Task CreateAsync(SysUserCreateDto dto)
        {
            var command = _mapper.Map<CreateSysUserCommand>(dto);
            await Bus.SendCommand(command);
        }

        public async Task RemoveAsync(long sysUserId, long currentUserId, string sysType)
        {
            var command = new RemoveSysUserCommand()
            {
                SysUserId = sysUserId,
                CurrentSysUserId = currentUserId,
                SysType = sysType
            };
            await Bus.SendCommand(command);
        }

        public override bool Update(SysUserDto dto)
        {
            var entity = _mapper.Map<SysUser>(dto);
            entity.UpdatedAt = DateTime.Now;
            _sysUserRepository.Update(entity);
            return _sysUserRepository.SaveChanges(out int _);
        }

        public void ModifyCurrentUserInfo(ModifyCurrentUserInfoDto dto)
        {
            var user = _sysUserRepository.GetByUserId(dto.SysUserId);
            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
                user.AvatarUrl = dto.AvatarUrl;
            if (!string.IsNullOrWhiteSpace(dto.Realname))
                user.Realname = dto.Realname;
            if (!string.IsNullOrWhiteSpace(dto.SafeWord))
                user.SafeWord = dto.SafeWord;
            if (dto.Sex > 0)
                user.Sex = dto.Sex;
            user.UpdatedAt = DateTime.Now;
            _sysUserRepository.Update(user);
            _sysUserRepository.SaveChanges();
        }

        public async Task ModifyAsync(SysUserModifyDto dto)
        {
            var command = _mapper.Map<ModifySysUserCommand>(dto);
            await Bus.SendCommand(command);
        }

        public SysUserDto GetByKeyAsNoTracking(long recordId)
        {
            var entity = _sysUserRepository.GetByKeyAsNoTracking(recordId);
            var dto = _mapper.Map<SysUserDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserDto> GetByBelongInfoIdAsNoTracking(string belongInfoId)
        {
            var entitys = _sysUserRepository.GetByBelongInfoIdAsNoTracking(belongInfoId);
            return _mapper.Map<IEnumerable<SysUserDto>>(entitys);
        }

        public SysUserDto GetById(long recordId, string belongInfoId)
        {
            var entity = _sysUserRepository.GetAll().Where(w => w.SysUserId.Equals(recordId) && w.BelongInfoId.Equals(belongInfoId)).FirstOrDefault();
            var dto = _mapper.Map<SysUserDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserDto> GetByIds(List<long> recordIds)
        {
            var sysUsers = _sysUserRepository.GetAll().Where(w => recordIds.Contains(w.SysUserId));
            return _mapper.Map<IEnumerable<SysUserDto>>(sysUsers);
        }

        public bool IsExistTelphone(string telphone, string sysType)
        {
            return _sysUserRepository.IsExistTelphone(telphone, sysType);
        }

        public SysUserDto GetByTelphone(string telphone, string sysType)
        {
            var entity = _sysUserRepository.GetByTelphone(telphone, sysType);
            var dto = _mapper.Map<SysUserDto>(entity);
            return dto;
        }

        public PaginatedList<SysUserListDto> GetPaginatedData(SysUserQueryDto dto, long? currentUserId)
        {
            var sysUsers = (from u in _sysUserRepository.GetAllAsNoTracking()
                            join ut in _sysUserTeamRepository.GetAllAsNoTracking() on u.TeamId equals ut.TeamId into temp
                            from team in temp.DefaultIfEmpty()
                            where (string.IsNullOrWhiteSpace(dto.SysType) || u.SysType.Equals(dto.SysType))
                            && (string.IsNullOrWhiteSpace(dto.BelongInfoId) || u.BelongInfoId.Equals(dto.BelongInfoId))
                            && (string.IsNullOrWhiteSpace(dto.Realname) || u.Realname.Contains(dto.Realname))
                            && (dto.UserType.Equals(null) || u.UserType.Equals(dto.UserType))
                            && (dto.SysUserId.Equals(null) || u.SysUserId.Equals(dto.SysUserId))
                            && (currentUserId.Equals(null) || !u.SysUserId.Equals(currentUserId))
                            select new { u, team }).ToList().Select(s =>
                            {
                                var item = _mapper.Map<SysUserListDto>(s.u);
                                item.TeamName = s.team?.TeamName;
                                return item;
                            }).OrderByDescending(o => o.CreatedAt);

            var records = PaginatedList<SysUserListDto>.Create(sysUsers, dto.PageNumber, dto.PageSize);
            return records;
        }

        public async Task<PaginatedList<SysUserListDto>> GetPaginatedDataAsync(SysUserQueryDto dto, long? currentUserId)
        {
            var query = _mapper.Map<SysUserQuery>(dto);
            query.CurrentUserId = currentUserId;
            var sysUsers = (IEnumerable<(SysUser SysUser, SysUserTeam SysUserTeam)>)await Bus.SendQuery(query);
            var result = sysUsers.Select(s =>
            {
                var item = _mapper.Map<SysUserListDto>(s.SysUser);
                item.TeamName = s.SysUserTeam?.TeamName;
                return item;
            });
            var records = PaginatedList<SysUserListDto>.Create(result, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}

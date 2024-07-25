using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Domain.Queries;
using AGooday.AgPay.Domain.Queries.SysUsers;
using MediatR;

namespace AGooday.AgPay.Domain.QueryHandlers
{
    public class SysUserQueryHandler :
        IRequestHandler<GetByIdQuery<SysUser, long>, SysUser>,
        IRequestHandler<SysUserQuery, IEnumerable<(SysUser SysUser, SysUserTeam SysUserTeam)>>
    {
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserTeamRepository _sysUserTeamRepository;

        public SysUserQueryHandler(ISysUserRepository sysUserRepository,
            ISysUserTeamRepository sysUserTeamRepository)
        {
            _sysUserRepository = sysUserRepository;
            _sysUserTeamRepository = sysUserTeamRepository;
        }

        public Task<SysUser> Handle(GetByIdQuery<SysUser, long> request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException(nameof(request.Id));
            }

            return _sysUserRepository.GetByIdAsync(request.Id);
        }

        public Task<IEnumerable<(SysUser SysUser, SysUserTeam SysUserTeam)>> Handle(SysUserQuery request, CancellationToken cancellationToken)
        {
            var sysUsers = (from u in _sysUserRepository.GetAllAsNoTracking()
                            join ut in _sysUserTeamRepository.GetAllAsNoTracking() on u.TeamId equals ut.TeamId into temp
                            from team in temp.DefaultIfEmpty()
                            where (string.IsNullOrWhiteSpace(request.SysType) || u.SysType.Equals(request.SysType))
                            && (string.IsNullOrWhiteSpace(request.BelongInfoId) || u.BelongInfoId.Contains(request.BelongInfoId))
                            && (string.IsNullOrWhiteSpace(request.Realname) || u.Realname.Contains(request.Realname))
                            && (request.UserType.Equals(null) || u.UserType.Equals(request.UserType))
                            && (request.SysUserId.Equals(null) || u.SysUserId.Equals(request.SysUserId))
                            && (request.CurrentUserId.Equals(null) || !u.SysUserId.Equals(request.CurrentUserId))
                            select new { u, team }).OrderByDescending(o => o.u.CreatedAt).ToList()
                            .Select(s =>
                            {
                                return (s.u, s.team);
                            });

            return Task.FromResult(sysUsers);
        }
    }
}

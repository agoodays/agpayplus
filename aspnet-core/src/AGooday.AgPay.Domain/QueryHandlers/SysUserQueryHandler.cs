using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Domain.Queries;
using AGooday.AgPay.Domain.Queries.SysUsers;
using MediatR;

namespace AGooday.AgPay.Domain.QueryHandlers
{
    public class SysUserQueryHandler :
        IRequestHandler<GetByIdQuery<SysUser, long>, SysUser>,
        IRequestHandler<SysUserQuery, IQueryable<SysUserQueryResult>>
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

        public Task<IQueryable<SysUserQueryResult>> Handle(SysUserQuery request, CancellationToken cancellationToken)
        {
            // 构建 IQueryable 查询，但不立即执行
            var sysUsers = from u in _sysUserRepository.GetAllAsNoTracking()
                           join ut in _sysUserTeamRepository.GetAllAsNoTracking() on u.TeamId equals ut.TeamId into temp
                           from team in temp.DefaultIfEmpty()
                           where (string.IsNullOrWhiteSpace(request.SysType) || u.SysType.Equals(request.SysType))
                           && (string.IsNullOrWhiteSpace(request.BelongInfoId) || u.BelongInfoId.Contains(request.BelongInfoId))
                           && (string.IsNullOrWhiteSpace(request.Realname) || u.Realname.Contains(request.Realname))
                           && (request.UserType == null || u.UserType.Equals(request.UserType))
                           && (request.SysUserId == null || u.SysUserId.Equals(request.SysUserId))
                           && (request.CurrentUserId == null || !u.SysUserId.Equals(request.CurrentUserId))
                           orderby u.CreatedAt descending
                           select new SysUserQueryResult
                           {
                               SysUser = u,
                               SysUserTeam = team
                           };

            return Task.FromResult(sysUsers);
        }
    }
}

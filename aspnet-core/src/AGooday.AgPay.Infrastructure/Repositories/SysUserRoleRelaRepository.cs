using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserRoleRelaRepository : AgPayRepository<SysUserRoleRela>, ISysUserRoleRelaRepository
    {
        public SysUserRoleRelaRepository(AgPayDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 当前角色是否已分配到用户
        /// </summary>
        /// <param name="telphone"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public Task<bool> IsAssignedToUserAsync(string roleId)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.RoleId == roleId);
        }

        public void RemoveByUserId(long userId)
        {
            var entities = DbSet.Where(w => w.UserId == userId);
            DbSet.RemoveRange(entities);
        }
    }
}

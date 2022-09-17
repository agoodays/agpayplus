using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserRoleRelaRepository : Repository<SysUserRoleRela>, ISysUserRoleRelaRepository
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
        public bool IsAssignedToUser(string roleId)
        {
            return DbSet.AsNoTracking().Any(c => c.RoleId == roleId);
        }

        public void RemoveByUserId(long userId)
        {
            var entitys = DbSet.Where(w => w.UserId == userId);
            foreach (var entity in entitys)
            {
                DbSet.Remove(entity);
            }
        }
    }
}

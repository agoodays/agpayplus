using AGooday.AgPay.Common.Constants;
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
    public class SysUserRepository : Repository<SysUser, long>, ISysUserRepository
    {
        public SysUserRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistLoginUsername(string loginUsername, string sysType)
        {
            return DbSet.AsNoTracking().Any(c => c.LoginUsername == loginUsername && c.SysType == sysType);
        }

        public bool IsExistTelphone(string telphone, string sysType)
        {
            return DbSet.AsNoTracking().Any(c => c.Telphone == telphone && c.SysType == sysType);
        }

        public bool IsExistUserNo(string userNo, string sysType)
        {
            return DbSet.AsNoTracking().Any(c => c.UserNo == userNo && c.SysType == sysType);
        }

        public bool IsExist(long sysUserId, string sysType)
        {
            return DbSet.AsNoTracking().Any(c => c.SysUserId == sysUserId && c.SysType == sysType);
        }

        public SysUser GetByUserId(long sysUserId, string sysType)
        {
            return DbSet.Where(w => w.SysUserId == sysUserId && w.SysType == sysType).FirstOrDefault();
        }

        public long FindMchAdminUserId(string mchNo)
        {
            return DbSet.Where(w => w.BelongInfoId == mchNo && w.SysType == CS.SYS_TYPE.MCH && w.IsAdmin == CS.YES).First().SysUserId;
        }

        public void Remove(SysUser sysUser)
        {
            DbSet.Remove(sysUser);
        }
    }
}

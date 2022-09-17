using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Models;
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
    public class SysUserAuthRepository : Repository<SysUserAuth, long>, ISysUserAuthRepository
    {
        public SysUserAuthRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public void RemoveByUserId(long userId, string sysType)
        {
            var entitys = DbSet.Where(w => w.UserId == userId && w.SysType == sysType);
            foreach (var entity in entitys)
            {
                DbSet.Remove(entity);
            }
        }

        public void ResetAuthInfo(long userId, string sysType, string loginUserName, string telphone, string newPwd)
        {
            var sysUserAuths = DbSet.Where(w => w.UserId == userId && w.SysType == sysType);

            //更改登录用户名
            if (!string.IsNullOrWhiteSpace(loginUserName))
            {
                var sysUserAuth = sysUserAuths.Where(w => w.IdentityType == CS.AUTH_TYPE.LOGIN_USER_NAME).First();
                if (sysUserAuth != null)
                {
                    sysUserAuth.Identifier = loginUserName;
                    Update(sysUserAuth);
                }
            }

            //更新手机号认证
            if (!string.IsNullOrWhiteSpace(telphone))
            {
                var sysUserAuth = sysUserAuths.Where(w => w.IdentityType == CS.AUTH_TYPE.TELPHONE).First();
                if (sysUserAuth != null)
                {
                    sysUserAuth.Identifier = telphone;
                    Update(sysUserAuth);
                }
            }

            //更改密码
            if (!string.IsNullOrWhiteSpace(newPwd))
            {
                foreach (var sysUserAuth in sysUserAuths)
                {
                    sysUserAuth.Credential = newPwd;
                    Update(sysUserAuth);
                }
            }
        }
    }
}

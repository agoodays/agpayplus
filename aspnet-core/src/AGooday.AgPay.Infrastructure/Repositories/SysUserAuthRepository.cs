using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserAuthRepository : AgPayRepository<SysUserAuth, long>, ISysUserAuthRepository
    {
        public SysUserAuthRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public void RemoveByUserId(long userId, string sysType)
        {
            var entities = DbSet.Where(w => w.UserId.Equals(userId) && w.SysType.Equals(sysType));
            DbSet.RemoveRange(entities);
        }

        public async Task ResetAuthInfoAsync(long userId, string sysType, string loginUserName, string telphone, string newPwd)
        {
            var sysUserAuths = DbSet.Where(w => w.UserId.Equals(userId) && w.SysType.Equals(sysType));

            //更改登录用户名
            if (!string.IsNullOrWhiteSpace(loginUserName))
            {
                var sysUserAuth = await sysUserAuths.FirstOrDefaultAsync(w => w.IdentityType.Equals(CS.AUTH_TYPE.LOGIN_USER_NAME));
                if (sysUserAuth != null)
                {
                    sysUserAuth.Identifier = loginUserName;
                    Update(sysUserAuth);
                }
            }

            //更新手机号认证
            if (!string.IsNullOrWhiteSpace(telphone))
            {
                var sysUserAuth = await sysUserAuths.FirstOrDefaultAsync(w => w.IdentityType.Equals(CS.AUTH_TYPE.TELPHONE));
                if (sysUserAuth != null)
                {
                    sysUserAuth.Identifier = telphone;
                    Update(sysUserAuth);
                }
            }

            //更改密码
            if (!string.IsNullOrWhiteSpace(newPwd))
            {
                var hashedPassword = BCryptUtil.Hash(newPwd, out string salt);
                foreach (var sysUserAuth in sysUserAuths)
                {
                    sysUserAuth.Credential = hashedPassword;
                    sysUserAuth.Salt = salt;
                    Update(sysUserAuth);
                }
            }
        }

        public Task<SysUserAuth> GetByIdentifierAsync(byte identityType, string identifier, string sysType)
        {
            return DbSet.FirstOrDefaultAsync(w => w.IdentityType.Equals(identityType) && w.Identifier.Equals(identifier) && w.SysType.Equals(sysType));
        }

        public IEnumerable<SysUserAuth> GetUserAuths(string identifier, string sysType)
        {
            FormattableString sql = $"""
                select a.* from t_sys_user_auth a left join t_sys_user u on a.user_id = u.sys_user_id
                where a.identifier = {identifier} and a.sys_type = {sysType}
                """;
            return FromSql(sql);
        }
    }
}

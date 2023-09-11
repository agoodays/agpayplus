using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Extensions.DataAccess;

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
            var entitys = DbSet.Where(w => w.UserId .Equals( userId) && w.SysType.Equals(sysType));
            foreach (var entity in entitys)
            {
                DbSet.Remove(entity);
            }
        }

        public void ResetAuthInfo(long userId, string sysType, string loginUserName, string telphone, string newPwd)
        {
            var sysUserAuths = DbSet.Where(w => w.UserId.Equals(userId) && w.SysType.Equals(sysType));

            //更改登录用户名
            if (!string.IsNullOrWhiteSpace(loginUserName))
            {
                var sysUserAuth = sysUserAuths.FirstOrDefault(w => w.IdentityType.Equals(CS.AUTH_TYPE.LOGIN_USER_NAME));
                if (sysUserAuth != null)
                {
                    sysUserAuth.Identifier = loginUserName;
                    Update(sysUserAuth);
                }
            }

            //更新手机号认证
            if (!string.IsNullOrWhiteSpace(telphone))
            {
                var sysUserAuth = sysUserAuths.FirstOrDefault(w => w.IdentityType.Equals(CS.AUTH_TYPE.TELPHONE));
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
                    sysUserAuth.Credential = BCryptUtil.Hash(newPwd, out string salt);
                    sysUserAuth.Salt = salt;
                    Update(sysUserAuth);
                }
            }
        }

        public SysUserAuth GetByIdentifier(byte identityType, string identifier, string sysType)
        {
            return DbSet.FirstOrDefault(w => w.IdentityType.Equals(identityType) && w.Identifier.Equals(identifier) && w.SysType.Equals(sysType));
        }

        public List<SysUserAuth> GetUserAuths(string identifier, string sysType)
        {
            var sql = @"select a.* from t_sys_user_auth a left join t_sys_user u on a.user_id = u.sys_user_id
        where a.identifier = @Identifier and a.sys_type = @SysType";

            var result = Db.Database.FromSql<SysUserAuth>(sql, new { Identifier = identifier, SysType = sysType });

            return result;
        }
    }
}

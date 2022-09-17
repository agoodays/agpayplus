using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserAuthRepository : IRepository<SysUserAuth, long>
    {
        void RemoveByUserId(long userId, string sysType);
        void ResetAuthInfo(long userId, string sysType, string loginUserName, string telphone, string newPwd);
    }
}

using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserRepository : IRepository<SysUser, long>
    {
        SysUser GetByLoginUsername(string name);
    }
}

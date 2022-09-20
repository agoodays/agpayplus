using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserAuthService : IDisposable
    {
        void Add(SysUserAuthDto dto);
        void Remove(long recordId);
        void Update(SysUserAuthDto dto);
        SysUserAuthDto GetById(long recordId);
        IEnumerable<SysUserAuthDto> GetAll();
        SysUserAuthInfoDto SelectByLogin(string identifier, byte identityType, string sysType);
    }
}

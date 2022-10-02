using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserService : IDisposable
    {
        void Add(SysUserDto dto);
        void Create(SysUserCreateDto dto);
        void Remove(long recordId);
        void Remove(long sysUserId, long currentUserId, string sysType);
        void Update(SysUserDto dto);
        void Modify(SysUserModifyDto dto);
        SysUserDto GetById(long recordId);
        SysUserDto GetById(long recordId, string belongInfoId);
        IEnumerable<SysUserDto> GetAll();
        IEnumerable<SysUserDto> GetAll(List<long> recordIds);
        PaginatedList<SysUserDto> GetPaginatedData(SysUserQueryDto dto);
        Task<IEnumerable<SysUserDto>> ListAsync();
    }
}

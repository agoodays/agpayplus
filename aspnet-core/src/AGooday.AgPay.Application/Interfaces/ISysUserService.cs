using AGooday.AgPay.Application.ViewModels;
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
        void Add(SysUserVM vm);
        void Create(SysUserVM vm);
        void Remove(long recordId);
        void Update(SysUserVM recordId);
        SysUserVM GetById(long recordId);
        IEnumerable<SysUserVM> GetAll();
        Task<IEnumerable<SysUserVM>> ListAsync();
    }
}
